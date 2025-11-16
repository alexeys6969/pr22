using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using ClassModule;

namespace Phonebook_Shashin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        public enum page_main
        {
            users, calls, none
        };
        public static page_main page_select;
        public Main()
        {
            InitializeComponent();
            page_select = page_main.none;
        }
        private void Click_Phone(object sender, MouseButtonEventArgs e)
        {
            if(frame_main.Visibility == Visibility.Visible)
            {
                MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
            }
            if(page_select != page_main.users)
            {
                page_select = page_main.users;
                DoubleAnimation op = new DoubleAnimation();
                op.From = 1;
                op.To = 0;
                op.Duration = TimeSpan.FromSeconds(0.2);
                op.Completed += delegate
                {
                    parrent.Children.Clear();
                    DoubleAnimation op1 = new DoubleAnimation();
                    op1.From = 0;
                    op1.To = 1;
                    op1.Duration = TimeSpan.FromSeconds(0.2);
                    op1.Completed += delegate
                    {

                        Dispatcher.InvokeAsync(async () =>
                        {
                            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);

                            foreach (User user_itm in MainWindow.connect.users)
                            {
                                if (page_select == page_main.users)
                                {
                                    parrent.Children.Add(new Elements.User_itm(user_itm));
                                    await Task.Delay(90);
                                }
                            }

                            if(page_select == page_main.users)
                            {
                                var ff = new Pages.PagesUser.UserWin(new User());
                                parrent.Children.Add(new Elements.Add_itm(ff));
                            }
                        });
                    };
                    parrent.BeginAnimation(StackPanel.OpacityProperty, op1);
                };
                parrent.BeginAnimation(StackPanel.OpacityProperty, op);
            }
        }

        private void Click_History(object sender, MouseButtonEventArgs e)
        {
            if (frame_main.Visibility == Visibility.Visible)
            {
                MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
            }
            if (page_select != page_main.calls)
            {
                page_select = page_main.calls;
                DoubleAnimation op = new DoubleAnimation();
                op.From = 1;
                op.To = 0;
                op.Duration = TimeSpan.FromSeconds(0.2);
                op.Completed += delegate
                {
                    parrent.Children.Clear();
                    DoubleAnimation op1 = new DoubleAnimation();
                    op1.From = 0;
                    op1.To = 1;
                    op1.Duration = TimeSpan.FromSeconds(0.2);
                    op1.Completed += delegate
                    {

                        Dispatcher.InvokeAsync(async () =>
                        {
                            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);

                            foreach (Call call_itm in MainWindow.connect.calls)
                            {
                                if (page_select == page_main.calls)
                                {
                                    parrent.Children.Add(new Elements.Call_itm(call_itm));
                                    await Task.Delay(90);
                                }
                            }

                            if (page_select == page_main.calls)
                            {
                                var ff = new Pages.PagesUser.CallWin(new Call());
                                parrent.Children.Add(new Elements.Add_itm(ff));
                            }
                        });
                    };
                    parrent.BeginAnimation(StackPanel.OpacityProperty, op1);
                };
                parrent.BeginAnimation(StackPanel.OpacityProperty, op);
            }
        }

        public void Anim_Move(Control control1, Control control2, Frame frame_main = null, Page pages = null, page_main page_restart = page_main.none)
        {
            if(page_restart != page_main.none)
            {
                if(page_restart == page_main.users)
                {
                    page_select = page_main.none;
                    Click_Phone(null, null);
                } else if(page_restart == page_main.calls)
                {
                    page_select = page_main.none;
                    Click_History(null, null);
                }
            } else
            {
                DoubleAnimation op = new DoubleAnimation();
                op.From = 1;
                op.To = 0;
                op.Duration = TimeSpan.FromSeconds(0.3);
                op.Completed += delegate
                {
                    if(pages != null)
                    {
                        frame_main.Navigate(pages);
                    }

                    control1.Visibility = Visibility.Hidden;
                    control2.Visibility = Visibility.Visible;

                    DoubleAnimation op1 = new DoubleAnimation();
                    op1.From = 0;
                    op1.To = 1;
                    op1.Duration = TimeSpan.FromSeconds(0.4);

                    control2.BeginAnimation(ScrollViewer.OpacityProperty, op1);
                };
                control1.BeginAnimation(ScrollViewer.OpacityProperty, op);
            }
        }
    }
}
