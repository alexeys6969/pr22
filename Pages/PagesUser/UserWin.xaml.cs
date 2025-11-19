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
    /// Логика взаимодействия для UserWin.xaml
    /// </summary>
    public partial class UserWin : Page
    {
        User user_loc;
        public UserWin(User _user)
        {
            InitializeComponent();
            user_loc = _user;
            if (_user != null && _user.fio_user != null)
            {
                fio_user.Text = _user.fio_user;
                phone_user.Text = _user.phone_num;
                addrec_user.Text = _user.passport_data;
            }
        }

        private void Click_User_Redact(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(fio_user.Text) || !MainWindow.connect.ItsOnlyFIO(fio_user.Text.Trim()))
            {
                MessageBox.Show("Вы неправильно написали ФИО. Формат: Фамилия Имя Отчество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(phone_user.Text) || !MainWindow.connect.ItsNumber(phone_user.Text.Trim()))
            {
                MessageBox.Show("Вы неправильно написали номер телефона", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(addrec_user.Text.Trim()))
            {
                MessageBox.Show("Вы неправильно написали паспортные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Очистка данных от лишних пробелов
            string fio = fio_user.Text.Trim();
            string phone = phone_user.Text.Trim();
            string passport = addrec_user.Text.Trim();

            // ИСПРАВЛЕНИЕ: Правильная проверка на новый пользователь
            bool isNewUser = user_loc == null || user_loc.id == 0;

            if (isNewUser)
            {
                // Проверка на дубликат перед добавлением
                if (MainWindow.connect.users.Any(u => u.phone_num == phone || u.fio_user == fio))
                {
                    MessageBox.Show("Пользователь с таким номером телефона или ФИО уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int id = MainWindow.connect.SetLastId(ClassConnection.Connection.tabels.users);
                string query = $"INSERT INTO [users]([Код], [phone_num], [fio_user], [pasport_data]) VALUES ({id}, '{phone}', '{fio}', '{passport}')";

                // ИСПРАВЛЕНИЕ: Используем ExecuteNonQuery вместо QueryAccess
                bool success = MainWindow.connect.ExecuteNonQuery(query);
                if (success)
                {
                    // Очищаем список перед загрузкой новых данных
                    MainWindow.connect.users.Clear();
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MessageBox.Show("Успешное добавление клиента", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
                }
                else
                {
                    MessageBox.Show("Запрос на добавление клиента не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                // Для существующего пользователя - проверяем дубликаты, исключая текущего пользователя
                if (MainWindow.connect.users.Any(u => u.id != user_loc.id && (u.phone_num == phone || u.fio_user == fio)))
                {
                    MessageBox.Show("Пользователь с таким номером телефона или ФИО уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string query = $"UPDATE [users] SET [phone_num] = '{phone}', [fio_user] = '{fio}', [pasport_data] = '{passport}' WHERE [Код] = {user_loc.id}";

                // ИСПРАВЛЕНИЕ: Используем ExecuteNonQuery вместо QueryAccess
                bool success = MainWindow.connect.ExecuteNonQuery(query);
                if (success)
                {
                    // Очищаем список перед загрузкой новых данных
                    MainWindow.connect.users.Clear();
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MessageBox.Show("Успешное изменение клиента", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
                }
                else
                {
                    MessageBox.Show("Запрос на изменение клиента не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Click_Remove_User_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, есть ли у пользователя звонки
                MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);
                Call userFind = MainWindow.connect.calls.Find(x => x.user_id == user_loc.id);

                if (userFind != null)
                {
                    var result = MessageBox.Show("У данного клиента есть звонки, все равно удалить его?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    // Сначала удаляем звонки пользователя
                    string deleteCallsQuery = $"DELETE FROM [calls] WHERE [user_id] = {user_loc.id}";
                    bool callsDeleted = MainWindow.connect.ExecuteNonQuery(deleteCallsQuery);

                    if (!callsDeleted)
                    {
                        MessageBox.Show("Не удалось удалить звонки пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Затем удаляем самого пользователя
                string deleteUserQuery = $"DELETE FROM [users] WHERE [Код] = {user_loc.id}";
                bool userDeleted = MainWindow.connect.ExecuteNonQuery(deleteUserQuery);

                if (userDeleted)
                {
                    // Очищаем и перезагружаем данные
                    MainWindow.connect.users.Clear();
                    MainWindow.connect.calls.Clear();
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);

                    MessageBox.Show("Успешное удаление клиента", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.main.Anim_Move(MainWindow.main.frame_main, MainWindow.main.scroll_main, null, null, Main.page_main.users);
                }
                else
                {
                    MessageBox.Show("Запрос на удаление клиента не был обработан", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}