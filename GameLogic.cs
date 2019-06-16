using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class GameLogic
    {
        public enum moveType
        {
            O = -1,
            EMPTY = 0,
            X = 1
        };

        public enum levelType
        {
            EASY,
            MEDIUM,
            HARD
        };

        private int[][] a = new int[32][];		//winning cases of each cell
        private int[] gameMatrix = new int[56]; //initally zeros
        private int[] nextState = new int[56];
        private moveType[] visited = new moveType[32];
        private List<int>[] spirals = new List<int>[32];
        private List<int>[] rings = new List<int>[32];
        private List<int>[] lines = new List<int>[32];

        public GameLogic()
        {
            initialize();
        }

        public bool isPlayed(int cell)
        { return visited[cell] != moveType.EMPTY; }

        private void initialize()
        {
            for (int i = 0; i < 32; ++i)
            {
                a[i] = new int[56];
                spirals[i] = new List<int>();
                rings[i] = new List<int>();
                lines[i] = new List<int>();
                for (int j = 0; j < 4; ++j)
                {
                    int v = i % 8;
                    v = (v - j + 8) % 8;
                    a[i][i / 8 * 8 + v] = 1;
                    rings[i].Add(i / 8 * 8 + v);
                }
                a[i][32 + i % 8] = 1;
                lines[i].Add(32 + i % 8);
                a[i][40 + (i + i / 8) % 8] = 1;
                spirals[i].Add(40 + (i + i / 8) % 8);
                a[i][48 + (i - i / 8 + 8) % 8] = 1;
                spirals[i].Add(48 + (i - i / 8 + 8) % 8);
            }
        }

        public bool humanMove(moveType type, int cell)
        {
            if (visited[cell] != moveType.EMPTY)
                return false;
            visited[cell] = type;
            for (int i = 0; i < 56; i++)
                gameMatrix[i] += (int)type * a[cell][i];
            return true;
        }

        private int evaluate(moveType player, int[] gameMatrix)
        {
            moveType opp = (moveType)(-1 * (int)player);
            for (int i = 0; i < 56; i++)
            {
                if (gameMatrix[i] == 4 * (int)player)
                    return 100;
                else if (gameMatrix[i] == 4 * (int)opp)
                    return -100;
            }
            return 0;
        }

        private int minimax(moveType[] visited, int[] gameMatrix, moveType player, int depth, bool isMax, int alpha, int beta)
        {
            bool finished = true;
            for (int i = 0; i < 32; i++)
            {
                if (visited[i] == moveType.EMPTY)
                {
                    finished = false;
                    break;
                }
            }
            if (finished)
            {
                int score = evaluate(player, gameMatrix);
                if (score == 100)
                    score -= depth;
                else if (score == -100)
                    score += depth;
                return score;
            }
            moveType opp = (moveType)(-1 * (int)player);

            int best = (int)-1e9;
            if (!isMax)
                best = (int)1e9;
            for (int cell = 0; cell < 32; cell++)
            {
                if (visited[cell] == moveType.EMPTY)
                {
                    bool exit = false;
                    visited[cell] = player;
                    if (!isMax)
                        visited[cell] = opp;
                    for (int i = 0; i < 56; i++)
                    {
                        if (isMax)
                            gameMatrix[i] += (int)player * a[cell][i];
                        else
                            gameMatrix[i] += (int)opp * a[cell][i];
                    }
                    if (isMax)
                    {
                        int value = minimax(visited, gameMatrix, player, depth + 1, false, alpha, beta);
                        best = Math.Max(best, value);
                        alpha = Math.Max(alpha, best);
                        if (beta <= alpha)
                            exit = true;
                    }
                    else
                    {
                        int value = minimax(visited, gameMatrix, player, depth + 1, true, alpha, beta);
                        best = Math.Min(best, value);
                        beta = Math.Min(beta, best);
                        if (beta <= alpha)
                            exit = true;
                    }
                    //undo the move
                    for (int i = 0; i < 56; i++)
                    {
                        if(isMax)
                            gameMatrix[i] -= (int)player * a[cell][i];
                        else
                            gameMatrix[i] -= (int)opp * a[cell][i];
                    }
                    visited[cell] = moveType.EMPTY;
                    if (exit)
                        break;
                }
            }
            return best;
        }

        private int findBestMove(moveType player)
        {
            int bestCell = -1;
            int bestVal = (int)-1e9;
            for (int cell = 0; cell < 32; cell++)
            {
                if(visited[cell] == moveType.EMPTY)
                {
                    visited[cell] = player;
                    for (int i = 0; i < 56; i++)
                    {
                        nextState[i] = gameMatrix[i] + (int)player * a[cell][i];
                    }
                    int moveVal = minimax(visited, nextState, player, 0, false, (int)-1e9, (int)1e9);
                    //undo the move 
                    visited[cell] = moveType.EMPTY;
                    for (int i = 0; i < 56; i++)
                    {
                        nextState[i] = gameMatrix[i];
                    }
                    if(moveVal > bestVal)
                    {
                        bestVal = moveVal;
                        bestCell = cell;
                    }
                }
            }
            return bestCell;
        }

        private int mediumMove(moveType type)
        {
            List<int> optimalPlays = new List<int>();
            int mx = (int)-1e9;  //maximum score
            for (int move = 0; move < 32; move++)
            {
                if (visited[move] != moveType.EMPTY)
                    continue;
                for (int i = 0; i < 56; i++)
                {
                    nextState[i] = gameMatrix[i] + (int)type * a[move][i];
                }
                int sScore = 0; //state score
                moveType oppType = (type == moveType.X) ? moveType.O : moveType.X;
                for (int i = 0; i < 56; i++)
                {
                    if (nextState[i] == (int)type * 4)
                        sScore += (int)1e7;
                    else if (nextState[i] == (int)oppType * 3)
                        sScore += (int)-1e5;
                    else if (nextState[i] == (int)type * 3)
                        sScore += (int)1e3;
                    else if (nextState[i] == (int)type * 2)
                        sScore += 1;
                    else if (nextState[i] == (int)oppType * 2)
                        sScore += -1;
                }

                moveType spiralTrap = checkSpiralTrap();
                if (spiralTrap == type)
                    sScore += 10;
                else if (spiralTrap == oppType)
                    sScore += (int)-1e3;
                moveType othertrapopponent = checkOtherTraps(move, gameMatrix);
                moveType othertrap = checkOtherTraps(move, nextState);
                if (othertrap == type)
                    sScore += 10;
                else if (othertrapopponent == oppType)
                    sScore += (int)3.6e3;
                if (sScore > mx)
                {
                    mx = sScore;
                    optimalPlays.Clear();
                    optimalPlays.Add(move);
                }
                else if (sScore == mx)
                    optimalPlays.Add(move);
            }
            int randIdx = new Random().Next(optimalPlays.Count());
            int play = optimalPlays[randIdx];
            return play;
        }

        private int randomMove()
        {
            List<int> moves = new List<int>();
            for (int cell = 0; cell < 32; cell++)
            {
                if (visited[cell] == moveType.EMPTY)
                    moves.Add(cell);
            }
            int randIdx = new Random().Next(moves.Count());
            return moves[randIdx];
        }

        public int computerMove(moveType type, levelType level)
        {
            int cnt = 0;
            for (int i = 0; i < 32; i++)
            {
                if (visited[i] != moveType.EMPTY)
                    cnt++;
            }
            int move = -1;
            if (level == levelType.EASY)
                move = randomMove();
            else if (level == levelType.HARD && cnt>=20)
                move = findBestMove(type);
            else 
                move = mediumMove(type);

            visited[move] = type;
            for (int i = 0; i < 56; i++)
                gameMatrix[i] += (int)type * a[move][i];

            return move;
        }
        public List<int> checkWinning()
        {
            List<int> vec = new List<int>();
            for (int i = 0; i < 56; i++)
            {
                if (Math.Abs(gameMatrix[i]) == 4)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (a[j][i] == 1)
                            vec.Add(j);
                    }
                    return vec;
                }
            }
            return vec;
        }


        private moveType checkSpiralTrap()
        {
            //every left spiral has four opposite right spirals that it intersects with
            //check for each left spiral, if it and one of its opposites has score 2
            //so if one put another on on that spiral, he will have two 3s.
            List<int> lSpiral = nextState.ToList().GetRange(40, 8);
            List<int> rSpiral = nextState.ToList().GetRange(48, 8);
            for (int left = 0; left < 8; left++)
            {
                if (Math.Abs(lSpiral[left]) == 2)
                {
                    int right = left;
                    for (int i = 0; i < 4; i++)
                    {
                        if (rSpiral[right] == lSpiral[left])
                            return (moveType)(rSpiral[right] / 2);
                        right = (right + 2) % 8;
                    }
                }
            }
            return moveType.EMPTY;
        }
        private moveType checkOtherTraps(int move, int[] gameMatrix)
        {
            for (int i = 0; i < spirals[move].Count; ++i)
                for (int j = 0; j < lines[move].Count; ++j)
                {
                    int u = spirals[move][i], v = lines[move][j];
                    if (gameMatrix[u] == 2 && gameMatrix[v] == 2)
                        return moveType.X;
                    if (gameMatrix[u] == -2 && gameMatrix[v] == -2)
                        return moveType.O;
                }
            for (int i = 0; i < rings[move].Count; ++i)
                for (int j = 0; j < lines[move].Count; ++j)
                {
                    int u = rings[move][i], v = lines[move][j];
                    if (gameMatrix[u] == 2 && gameMatrix[v] == 2)
                        return moveType.X;
                    if (gameMatrix[u] == -2 && gameMatrix[v] == -2)
                        return moveType.O;
                }
            for (int i = 0; i < spirals[move].Count; ++i)
                for (int j = 0; j < rings[move].Count; ++j)
                {
                    int u = spirals[move][i], v = rings[move][j];
                    if (gameMatrix[u] == 2 && gameMatrix[v] == 2)
                        return moveType.X;
                    if (gameMatrix[u] == -2 && gameMatrix[v] == -2)
                        return moveType.O;
                }
            for (int i = 0; i < rings[move].Count; ++i)
                for (int j = 0; j < rings[move].Count; ++j)
                {
                    int u = rings[move][i], v = rings[move][j];
                    if (u != v && gameMatrix[u] == 2 && gameMatrix[v] == 2)
                        return moveType.X;
                    if (u != v && gameMatrix[u] == -2 && gameMatrix[v] == -2)
                        return moveType.O;
                }
            return moveType.EMPTY;
        }

    }
}
