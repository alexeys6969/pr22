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

            try
            {
                if (_call != null && _call.time_start != null && _call.time_end != null && _call.user_id > 0)
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

                    // Безопасный расчет длительности
                    if (DateTime.TryParse(_call.time_start, out DateTime startDate) &&
                        DateTime.TryParse(_call.time_end, out DateTime endDate))
                    {
                        TimeSpan duration = endDate - startDate;
                        time_call_text.Content = $"Продолжительность: {duration:hh\\:mm\\:ss}";
                    }
                    else
                    {
                        time_call_text.Content = "Ошибка формата времени";
                    }
                }
                else
                {
                    category_call_text.Content = "Неполные данные";
                    number_call_text.Content = "Нет данных";
                    time_call_text.Content = "Нет данных";
                }

                // Устанавливаем иконку
                img_category_call.Source =
                    (_call.category_call == 1) ?
                    new BitmapImage(new Uri(@"/img/out.png", UriKind.RelativeOrAbsolute)) :
                    new BitmapImage(new Uri(@"/img/in.png", UriKind.RelativeOrAbsolute));

                // Анимация
                DoubleAnimation op = new DoubleAnimation();
                op.From = 0;
                op.To = 1;
                op.Duration = TimeSpan.FromSeconds(0.4);
                border.BeginAnimation(StackPanel.OpacityProperty, op);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания элемента звонка: " + ex.Message);
            }
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_Move(MainWindow.main.scroll_main, MainWindow.main.frame_main, MainWindow.main.frame_main, new Pages.PagesUser.CallWin(call_loc));
        }

        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот звонок?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string vs = $"DELETE FROM [calls] WHERE [Код] = {call_loc.id}";
                    bool success = MainWindow.connect.ExecuteNonQuery(vs);

                    if (success)
                    {
                        MessageBox.Show("Успешное удаление звонка", "Успешно",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Обновляем данные
                        MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);

                        // Обновляем интерфейс
                        if (MainWindow.main != null)
                        {
                            MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main,
                                null, null, Pages.Main.page_main.calls);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении звонка", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
