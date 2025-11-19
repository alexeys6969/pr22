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
    /// Логика взаимодействия для User_itm.xaml
    /// </summary>
    public partial class User_itm : UserControl
    {
        User user_loc;
        public User_itm(User _user)
        {
            InitializeComponent();
            user_loc = _user;
            try
            {
                if (_user?.fio_user != null)
                {
                    name_user.Content = _user.fio_user;
                    phone_user.Content = $"Номер: {_user.phone_num}";
                }
                else
                {
                    name_user.Content = "Нет данных";
                    phone_user.Content = "Номер: не указан";
                }

                DoubleAnimation opgrid = new DoubleAnimation();
                opgrid.From = 0;
                opgrid.To = 1;
                opgrid.Duration = TimeSpan.FromSeconds(0.4);
                border.BeginAnimation(StackPanel.OpacityProperty, opgrid);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания элемента пользователя: " + ex.Message);
            }
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_Move(MainWindow.main.scroll_main, MainWindow.main.frame_main, MainWindow.main.frame_main, new Pages.PagesUser.UserWin(user_loc));
        }

        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                // ТЕПЕРЬ ДОСТАТОЧНО УДАЛИТЬ ТОЛЬКО ПОЛЬЗОВАТЕЛЯ
                // Звонки удалятся автоматически благодаря связи
                string deleteUser = $"DELETE FROM [users] WHERE [Код] = {user_loc.id}";
                bool success = MainWindow.connect.ExecuteNonQuery(deleteUser);

                if (success)
                {
                    MessageBox.Show("Клиент и все его звонки удалены", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    // ОБНОВИ ДАННЫЕ
                    MainWindow.connect.users.Clear();
                    MainWindow.connect.calls.Clear();
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);
                    MainWindow.connect.LoadData(ClassConnection.Connection.tabels.calls);

                    // УДАЛИ ЭЛЕМЕНТ ИЗ ИНТЕРФЕЙСА
                    var parent = this.Parent as StackPanel;
                    parent?.Children.Remove(this);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
