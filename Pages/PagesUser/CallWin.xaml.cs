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
using ClassModule;

namespace Phonebook_Shashin.Pages.PagesUser
{
    /// <summary>
    /// Логика взаимодействия для CallWin.xaml
    /// </summary>
    public partial class CallWin : Page
    {
        Call call_itm;
        public CallWin(Call _call)
        {
            InitializeComponent();
            call_itm = _call;
            if (_call != null && _call.time_start != null && _call.time_end != null)
            {
                try
                {
                    string[] dateTimeStart = _call.time_start.Split(' ');
                    string[] dateStart = dateTimeStart[0].Split('.');
                    date_start_call.SelectedDate = new DateTime(int.Parse(dateStart[2]), int.Parse(dateStart[1]), int.Parse(dateStart[0]));
                    time_start.Text = dateTimeStart[1];

                    string[] dateTimeFinish = _call.time_end.Split(' ');
                    string[] dateFinish = dateTimeFinish[0].Split('.');
                    date_end_call.SelectedDate = new DateTime(int.Parse(dateFinish[2]), int.Parse(dateFinish[1]), int.Parse(dateFinish[0]));
                    time_finish.Text = dateTimeFinish[1];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных звонка: " + ex.Message);
                    // Устанавливаем значения по умолчанию
                    date_start_call.SelectedDate = DateTime.Now;
                    date_end_call.SelectedDate = DateTime.Now;
                    time_start.Text = "00:00";
                    time_finish.Text = "00:00";
                }
            }
            else
            {
                // Значения по умолчанию для нового звонка
                date_start_call.SelectedDate = DateTime.Now;
                date_end_call.SelectedDate = DateTime.Now;
                time_start.Text = "00:00";
                time_finish.Text = "00:00";
            }

            // Заполняем комбобокс категорий
            call_category_text.Items.Clear();

            ComboBoxItem combItm = new ComboBoxItem();
            combItm.Tag = 1;
            combItm.Content = "Исходящий";
            if (call_itm.category_call == 1) combItm.IsSelected = true;
            call_category_text.Items.Add(combItm);

            ComboBoxItem combItm1 = new ComboBoxItem();
            combItm1.Tag = 2;
            combItm1.Content = "Входящий";
            if (call_itm.category_call == 2) combItm1.IsSelected = true;
            call_category_text.Items.Add(combItm1);

            // Заполняем комбобокс пользователей
            user_select.Items.Clear();
            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);

            foreach (User itm in MainWindow.connect.users)
            {
                ComboBoxItem combUser = new ComboBoxItem();
                combUser.Tag = itm.id;
                combUser.Content = itm.fio_user;
                if (call_itm.user_id == itm.id) combUser.IsSelected = true;
                user_select.Items.Add(combUser);
            }

            // Выбираем первого пользователя по умолчанию для нового звонка
            if (call_itm.id == 0 && user_select.Items.Count > 0)
            {
                user_select.SelectedIndex = 0;
            }

            // Выбираем категорию по умолчанию для нового звонка
            if (call_itm.id == 0 && call_category_text.Items.Count > 0)
            {
                call_category_text.SelectedIndex = 0;
            }
        }

        private void Click_Call_Redact(object sender, RoutedEventArgs e)
        {
            if (!CheckTime(time_start.Text))
            {
                MessageBox.Show("Время старта не выбрано");
                return;
            }

            if (!CheckTime(time_finish.Text))
            {
                MessageBox.Show("Время конца не выбрано");
                return;
            }

            if (date_start_call.SelectedDate == null || date_end_call.SelectedDate == null)
            {
                MessageBox.Show("Вы не указали дату");
                return;
            }

            DateTime dateStart = date_start_call.SelectedDate.Value;
            DateTime dateFinish = date_end_call.SelectedDate.Value;

            // Проверка дат
            if (dateFinish < dateStart)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала");
                return;
            }

            // Проверка времени при одинаковых датах
            if (dateStart.Date == dateFinish.Date)
            {
                TimeSpan startTime = TimeSpan.Parse(time_start.Text);
                TimeSpan endTime = TimeSpan.Parse(time_finish.Text);
                if (endTime <= startTime)
                {
                    MessageBox.Show("Время окончания должно быть позже времени начала");
                    return;
                }
            }

            // Проверка пользователя
            if (user_select.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }

            // Проверка категории звонка
            if (call_category_text.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию звонка");
                return;
            }

            User selectedUser = MainWindow.connect.users.Find(x => x.id == Convert.ToInt32(((ComboBoxItem)user_select.SelectedItem).Tag));
            int id_calls_categ = Convert.ToInt32(((ComboBoxItem)call_category_text.SelectedItem).Tag);

            string dateStartStr = dateStart.ToString("MM.dd.yyyy");
            string dateEndStr = dateFinish.ToString("MM.dd.yyyy");
            string timeStartStr = $"{dateStartStr} {time_start.Text}";
            string timeEndStr = $"{dateEndStr} {time_finish.Text}";

            try
            {
                if (call_itm.id == 0) // Это новый звонок
                {
                    int id = MainWindow.connect.SetLastId(ClassConnection.Connection.tabels.calls);
                    string query = $"INSERT INTO [calls]([Код], [user_id], [category_call], [date], [time_start], [time_end]) VALUES ({id}, " +
                        $"{selectedUser.id}, {id_calls_categ}, '{dateStartStr}', '{timeStartStr}', '{timeEndStr}')";

                    bool success = MainWindow.connect.ExecuteNonQuery(query);
                    if (success)
                    {
                        MessageBox.Show("Успешное добавление звонка", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                        // ОБНОВЛЯЕМ ДАННЫЕ И ИНТЕРФЕЙС
                        RefreshCallsData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении звонка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else // Редактирование существующего звонка
                {
                    string query = $"UPDATE [calls] SET [user_id] = {selectedUser.id}, " +
                        $"[category_call] = {id_calls_categ}, " +
                        $"[date] = '{dateStartStr}', " +
                        $"[time_start] = '{timeStartStr}', " +
                        $"[time_end] = '{timeEndStr}' WHERE [Код] = {call_itm.id}";

                    bool success = MainWindow.connect.ExecuteNonQuery(query);
                    if (success)
                    {
                        MessageBox.Show("Успешное изменение звонка", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                        RefreshCallsData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при изменении звонка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshCallsData()
        {
            // Обновляем данные
            MainWindow.connect.calls.Clear();
            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);

            // Возвращаемся на главную страницу и обновляем список звонков
            if (MainWindow.main != null)
            {
                MainWindow.main.Anim_Move(
                    MainWindow.main.frame_main,
                    MainWindow.main.scroll_main,
                    null,
                    null,
                    Pages.Main.page_main.calls
                );
            }
        }

        private void Click_Cancel_Call_Redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main);
        }

        public bool CheckTime(string str)
        {
            string[] str1 = str.Split(':');
            if (str1.Length == 2)
            {
                if (str1[0].Trim() != "" && str1[1].Trim() != "")
                {
                    if (int.Parse(str1[0]) >= 0 && int.Parse(str1[0]) <= 23)
                    {
                        if (int.Parse(str1[1]) >= 0 && int.Parse(str1[1]) <= 59)
                        {
                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        private void Click_Remove_Call_Redact(object sender, RoutedEventArgs e)
        {
try
    {
        string vs = $"DELETE FROM [calls] WHERE [Код] = {call_itm.id}";
        // ИСПРАВЬ: используй QueryAccess
        var pc = MainWindow.connect.QueryAccess(vs);
        if (pc != null)
        {
            MessageBox.Show("Успешное удаление звонка", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);
            MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.calls);
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
