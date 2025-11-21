using ClassModule;
using System;
using System.Collections.Generic;
using System.Globalization;
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

                    // УЛУЧШЕННЫЙ ПАРСИНГ ДАТЫ И ВРЕМЕНИ
                    DateTime? startDate = ParseDateTime(_call.time_start);
                    DateTime? endDate = ParseDateTime(_call.time_end);

                    if (startDate.HasValue && endDate.HasValue)
                    {
                        TimeSpan duration = endDate.Value - startDate.Value;
                        time_call_text.Content = $"Продолжительность: {duration:hh\\:mm\\:ss}";
                    }
                    else
                    {
                        // Альтернативный метод парсинга если стандартный не сработал
                        TryAlternativeParse(_call.time_start, _call.time_end);
                    }
                }
                else
                {
                    category_call_text.Content = "Неполные данные";
                    number_call_text.Content = "Нет данных";
                    time_call_text.Content = "Нет данных";
                }

                img_category_call.Source =
                    (_call.category_call == 1) ?
                    new BitmapImage(new Uri(@"/img/out.png", UriKind.RelativeOrAbsolute)) :
                    new BitmapImage(new Uri(@"/img/in.png", UriKind.RelativeOrAbsolute));


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

        private DateTime? ParseDateTime(string dateTimeStr)
        {
            if (string.IsNullOrEmpty(dateTimeStr)) return null;

            // Пробуем разные форматы
            string[] formats = {
        "MM.dd.yyyy HH:mm",
        "dd.MM.yyyy HH:mm",
        "yyyy-MM-dd HH:mm",
        "MM/dd/yyyy HH:mm",
        "dd/MM/yyyy HH:mm"
    };

            if (DateTime.TryParseExact(dateTimeStr, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            // Пробуем стандартный парсинг
            if (DateTime.TryParse(dateTimeStr, out result))
            {
                return result;
            }

            return null;
        }

        private void TryAlternativeParse(string timeStart, string timeEnd)
        {
            try
            {
                // Разделяем дату и время вручную
                string[] startParts = timeStart.Split(' ');
                string[] endParts = timeEnd.Split(' ');

                if (startParts.Length >= 2 && endParts.Length >= 2)
                {
                    string startDateStr = startParts[0];
                    string startTimeStr = startParts[1];
                    string endDateStr = endParts[0];
                    string endTimeStr = endParts[1];

                    // Парсим дату (пробуем разные разделители)
                    string[] dateFormats = { "MM.dd.yyyy", "dd.MM.yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" };

                    if (DateTime.TryParseExact(startDateStr, dateFormats, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime startDate) &&
                        DateTime.TryParseExact(endDateStr, dateFormats, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime endDate))
                    {
                        // Парсим время
                        if (TimeSpan.TryParse(startTimeStr, out TimeSpan startTime) &&
                            TimeSpan.TryParse(endTimeStr, out TimeSpan endTime))
                        {
                            DateTime fullStart = startDate.Date + startTime;
                            DateTime fullEnd = endDate.Date + endTime;

                            TimeSpan duration = fullEnd - fullStart;
                            time_call_text.Content = $"Продолжительность: {duration:hh\\:mm\\:ss}";
                            return;
                        }
                    }
                }

                time_call_text.Content = "Ошибка формата времени";
            }
            catch
            {
                time_call_text.Content = "Не удалось распарсить время";
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
