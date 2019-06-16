using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Expression.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private Arc[] cells;
        private Path[] Xs;
        private Arc[] Os;
        public enum GameMode
        {
            ONEPLAYER,
            TWOPLAYERS
        }

        private GameLogic.moveType currentMove;
        private GameLogic.levelType level;
        private GameLogic game;
        private GameMode mode;
        private bool closedViaCode = false;

        public GameWindow(GameMode mode, bool isComFirst, GameLogic.levelType level)
        {
            InitializeComponent();

            cells = new Arc[32] { cell0, cell1, cell2, cell3, cell4, cell5, cell6, cell7, cell8, cell9, cell10, cell11,
                cell12, cell13, cell14, cell15, cell16, cell17, cell18, cell19, cell20, cell21, cell22, cell23, cell24, cell25,
                cell26, cell27,cell28, cell29, cell30, cell31};
            Xs = new Path[32] {x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18, x19, x20,
                x21, x22, x23, x24, x25, x26, x27, x28, x29, x30, x31};
            Os = new Arc[32] {o0, o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15, o16, o17, o18, o19, o20,
                o21, o22, o23, o24, o25, o26, o27, o28, o29, o30, o31};

            foreach (Arc c in cells)
            {
                c.MouseEnter += new MouseEventHandler(ShapeMouseEnter);
                c.MouseLeave += new MouseEventHandler(ShapeMouseLeave);
                c.MouseLeftButtonDown += new MouseButtonEventHandler(CellClickEvent);
            }

            foreach (Arc o in Os)
            {
                o.MouseEnter += new MouseEventHandler(ShapeMouseEnter);
                o.MouseLeave += new MouseEventHandler(ShapeMouseLeave);
                o.MouseLeftButtonDown += new MouseButtonEventHandler(CellClickEvent);
            }

            foreach (Path x in Xs)
            {
                x.MouseEnter += new MouseEventHandler(ShapeMouseEnter);
                x.MouseLeave += new MouseEventHandler(ShapeMouseLeave);
                x.MouseLeftButtonDown += new MouseButtonEventHandler(CellClickEvent);
            }

            game = new GameLogic();
            this.mode = mode;
            this.level = level;
            if (mode == GameMode.ONEPLAYER && isComFirst)
            {
                int m = game.computerMove(GameLogic.moveType.X, level);
                ShowPlay(m, GameLogic.moveType.X);
                currentMove = GameLogic.moveType.O;
            }
            else
                currentMove = GameLogic.moveType.X;
        }

        private void CellClickEvent(object sender, MouseButtonEventArgs e)
        {
            Shape s = sender as Shape;
            int idx = Array.IndexOf(cells, s);
            if (idx == -1)
                idx = Array.IndexOf(Os, s);
            if (idx == -1)
                idx = Array.IndexOf(Xs, s);
            if (game.isPlayed(idx))
                return;
            game.humanMove(currentMove, idx);
            ShowPlay(idx, currentMove);
            if (checkWining())
                return;
            if (checkDraw())
                return;
            currentMove = (currentMove == GameLogic.moveType.X) ? GameLogic.moveType.O : GameLogic.moveType.X;
            if(mode == GameMode.ONEPLAYER)
            {
                int cMove = game.computerMove(currentMove, level);
                ShowPlay(cMove, currentMove);
                if (checkWining())
                    return;
                if (checkDraw())
                    return;
                currentMove = (currentMove == GameLogic.moveType.X) ? GameLogic.moveType.O : GameLogic.moveType.X;
            }
            
        }

        private bool checkWining()
        {
            List<int> vec = game.checkWinning();
            if (vec.Count == 0)
                return false;
            else if(currentMove == GameLogic.moveType.X)
            {
                foreach (int idx in vec)
                {
                    Xs[idx].Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
                MessageBox.Show("X won!");
                closedViaCode = true;
                this.Close();
                return true;
            }
            else
            {
                foreach (int idx in vec)
                {
                    Os[idx].Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                }
                MessageBox.Show("O won!");
                closedViaCode = true;
                this.Close();
                return true;
            }
        }

        private bool checkDraw()
        {
            for (int i = 0; i < 32; i++)
            {
                if (!game.isPlayed(i))
                    return false;
            }
            MessageBox.Show("Draw!");
            closedViaCode = true;
            this.Close();
            return true;
        }

        private void ShapeMouseEnter(object sender, MouseEventArgs e)
        {
            Shape s = sender as Shape;
            int idx = Array.IndexOf(cells, s);
            if (idx == -1)
                idx = Array.IndexOf(Os, s);
            if (idx == -1)
                idx = Array.IndexOf(Xs, s);
            if (game.isPlayed(idx))
                return;
            if (currentMove== GameLogic.moveType.X)
            {
                Xs[idx].Visibility = Visibility.Visible;
                Xs[idx].Opacity = 0.30;
            }
            else
            {
                Os[idx].Visibility = Visibility.Visible;
                Os[idx].Opacity = 0.30;
            }
        }

        private void ShapeMouseLeave(object sender, MouseEventArgs e)
        {
            Shape s = sender as Shape;
            int idx = Array.IndexOf(cells, s);
            if (idx == -1)
                idx = Array.IndexOf(Os, s);
            if (idx == -1)
                idx = Array.IndexOf(Xs, s);
            if (game.isPlayed(idx))
                return;
            if (currentMove == GameLogic.moveType.X)
                Xs[idx].Visibility = Visibility.Collapsed;
            else
                Os[idx].Visibility = Visibility.Collapsed;
        }

        private void ShowPlay(int idx, GameLogic.moveType type)
        {
            if (type == GameLogic.moveType.X)
            {
                Xs[idx].Visibility = Visibility.Visible;
                Xs[idx].Opacity = 1;
            }
            else
            {
                Os[idx].Visibility = Visibility.Visible;
                Os[idx].Opacity = 1;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new instructions().Show();
        }

        private void LineArrow_MouseEnter(object sender, MouseEventArgs e)
        {
            ReturnButton.Stroke = new SolidColorBrush(Color.FromRgb(0, 100, 255));
        }

        private void ReturnButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ReturnButton.Stroke = new SolidColorBrush(Color.FromRgb(138, 183, 241));
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(closedViaCode)
            {
                Owner.Show();
                return; 
            }
            string sMessageBoxText = "Do you want to end the game and return to the Main Menu?";
            string sCaption = "Exit";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:
                    Owner.Show();
                    break;

                case MessageBoxResult.No:
                    e.Cancel = true;
                    break;

            }
        }

        private void ReturnButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            HelpImage.Opacity = 1;
        }

        private void HelpImage_MouseLeave(object sender, MouseEventArgs e)
        {
            HelpImage.Opacity = 0.60;
        }

    }
}
