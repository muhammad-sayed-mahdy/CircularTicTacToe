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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            GameLogic.levelType level = GameLogic.levelType.HARD;
            if (EasyRadioButoon.IsChecked.Value == true)
                level = GameLogic.levelType.EASY;
            else if (MediumRadioButton.IsChecked.Value == true)
                level = GameLogic.levelType.MEDIUM;

            GameWindow Wind =  new GameWindow(GameWindow.GameMode.ONEPLAYER, ORadioButton.IsChecked.Value, level);
            Wind.Owner = this;
            Wind.Show();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            GameWindow Wind =  new GameWindow(GameWindow.GameMode.TWOPLAYERS, false, GameLogic.levelType.EASY);
            Wind.Owner = this;
            Wind.Show();
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            CreditsLable.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        private void CreditsLable_MouseLeave(object sender, MouseEventArgs e)
        {
            CreditsLable.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private void CreditsLable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new Credits().Show();
        }
    }
}
