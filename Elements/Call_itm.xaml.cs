using ClassModule;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Phonebook_Shashin.Elements
{
    /// <summary>
    /// Логика взаимодействия для Call_itm.xaml
    /// </summary>
    public partial class Call_itm : UserControl
    {
        Call call_loc;
        public Call_itm(Call _call)
        {
            InitializeComponent();
            call_loc = _call;

            if (_call.time_end != null && _call.user_id > 0)
            {
                User user_loc = MainWindow.connect.users.Find(x => x.id == _call.user_id);
                if (user_loc != null)
                {
                    category_call_text.Content = user_loc.fio_user;
                    number_call_text.Content = $"Номер телефона: {user_loc.phone_num}";
                }
                else
                {
                    category_call_text.Content = "Пользователь удален";
                    number_call_text.Content = "Номер не доступен";
                }

                string[] dateloc1 = _call.time_start.ToString().Split(' ');
                string[] dateloc2 = _call.time_end.ToString().Split(' ');

                string[] date1 = (dateloc1[0]).Split('.');
                string[] date2 = (dateloc2[0]).Split('.');

                System.DateTime dateStart = new DateTime(
                    int.Parse(date1[2]),
                    int.Parse(date1[1]),
                    int.Parse(date1[0]),
                    int.Parse(dateloc1[1].Split(':')[0]),
                    int.Parse(dateloc1[1].Split(':')[1]),
                    0
                );

                System.DateTime dateFinish = new DateTime(
                    int.Parse(date2[2]),
                    int.Parse(date2[1]),
                    int.Parse(date2[0]),
                    int.Parse(dateloc2[1].Split(':')[0]),
                    int.Parse(dateloc2[1].Split(':')[1]),
                    0
                );
                System.TimeSpan dateEnd = dateFinish.Subtract(dateStart);

                time_call_text.Content = $"Продолжительность звонка: {dateEnd.ToString()}";
            }
            else
            {
                MessageBox.Show("Некорректные данные звонка");
                category_call_text.Content = "Нет данных";
                number_call_text.Content = "Нет данных";
                time_call_text.Content = "Нет данных";
            }

            img_category_call.Source =
                (_call.category_call == 1) ?
                new BitmapImage(new Uri(@"/img/out.png", UriKind.RelativeOrAbsolute)) : new BitmapImage(new Uri(@"/img/in.png", UriKind.RelativeOrAbsolute));

            DoubleAnimation op = new DoubleAnimation();
            op.From = 0;
            op.To = 1;
            op.Duration = TimeSpan.FromSeconds(0.4);
            border.BeginAnimation(StackPanel.OpacityProperty, op);
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_Move(MainWindow.main.scroll_main, MainWindow.main.frame_main, MainWindow.main.frame_main, new Pages.PagesUser.CallWin(call_loc));
        }

        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);

                string vs = $"DELETE FROM [calls] WHERE [Код] = {call_loc.id}";
                var pc = MainWindow.connect.QueryAccess(vs);
                if (pc != null)
                {
                    MessageBox.Show("Успешное удаление звонка", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);
                    MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Pages.Main.page_main.calls);
                }
                else MessageBox.Show("Запрос на удаление звонка не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
