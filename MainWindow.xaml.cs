using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Phonebook_Shashin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Connection connect;
        public static Pages.Main main;
        public MainWindow()
        {
            InitializeComponent();
            connect = new Connection();
            connect.LoadData(Connection.tabels.users);
            connect.LoadData(Connection.tabels.calls);
            main = new Pages.Main();
            OpenPageMain();
        }

        public void OpenPageMain()
        {
            DoubleAnimation opgridAnimation = new DoubleAnimation();
            opgridAnimation.From = 1;
            opgridAnimation.To = 0;
            opgridAnimation.Duration = TimeSpan.FromSeconds(0.6);
            opgridAnimation.Completed += delegate
            {
                frame.Navigate(main);
                DoubleAnimation opgrisdAnimation = new DoubleAnimation();
                opgrisdAnimation.From = 0;
                opgrisdAnimation.To = 1;
                opgrisdAnimation.Duration = TimeSpan.FromSeconds(1.2);
                frame.BeginAnimation(Frame.OpacityProperty, opgrisdAnimation);
            };
            frame.BeginAnimation(Frame.OpacityProperty, opgridAnimation);
        }
    }
}
