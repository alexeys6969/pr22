using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using static Phonebook_Shashin.Pages.Main;

namespace Phonebook_Shashin.Pages
{
    /// <summary>
    /// Логика взаимодействия для Filter.xaml
    /// </summary>
    public partial class Filter : Page
    {
        Call call_itm;
        public Filter(Call _call)
        {
            InitializeComponent();
            call_itm = _call ?? new Call();

            category_select.Items.Clear();

            ComboBoxItem combItm = new ComboBoxItem();
            combItm.Tag = 1;
            combItm.Content = "Исходящий";
            if (call_itm.category_call == 1) combItm.IsSelected = true;
            category_select.Items.Add(combItm);

            ComboBoxItem combItm1 = new ComboBoxItem();
            combItm1.Tag = 2;
            combItm1.Content = "Входящий";
            if (call_itm.category_call == 2) combItm1.IsSelected = true;
            category_select.Items.Add(combItm1);

            num_select.Items.Clear();
            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);

            foreach (User itm in MainWindow.connect.users)
            {
                ComboBoxItem combUser = new ComboBoxItem();
                combUser.Tag = itm.id;
                combUser.Content = itm.phone_num;
                if (call_itm.user_id == itm.id) combUser.IsSelected = true;
                num_select.Items.Add(combUser);
            }
        }

        private void Click_Call_Redact(object sender, RoutedEventArgs e)
        {
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

            if (num_select.SelectedItem == null)
            {
                MessageBox.Show("Выберите номер");
                return;
            }

            // Проверка категории звонка
            if (category_select.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию звонка");
                return;
            }

            User selectedNum = MainWindow.connect.users.Find(x => x.id == Convert.ToInt32(((ComboBoxItem)num_select.SelectedItem).Tag));
            int id_calls_categ = Convert.ToInt32(((ComboBoxItem)category_select.SelectedItem).Tag);

            string dateStartStr = dateStart.ToString("MM.dd.yyyy");
            string dateEndStr = dateFinish.ToString("MM.dd.yyyy");
            string fio = selectedNum.fio_user;

            string query = $"SELECT * FROM [calls] WHERE [user_id] = {selectedNum.id} AND [date] BETWEEN '{dateStartStr}' AND '{dateEndStr}'";

            using (var reader = MainWindow.connect.QueryAccess(query))
            {
                if (reader != null)
                {
                    // Очищаем filteredCalls и заполняем отфильтрованными данными
                    MainWindow.connect.filteredCalls.Clear();

                    while (reader.Read())
                    {
                        Call newCall = new Call();
                        newCall.id = Convert.ToInt32(reader.GetValue(0));
                        newCall.user_id = Convert.ToInt32(reader.GetValue(1));
                        newCall.category_call = Convert.ToInt32(reader.GetValue(2));
                        newCall.date = Convert.ToString(reader.GetValue(3));
                        newCall.time_start = Convert.ToString(reader.GetValue(4));
                        newCall.time_end = Convert.ToString(reader.GetValue(5));
                        MainWindow.connect.filteredCalls.Add(newCall);
                    }

                    MessageBox.Show($"Успешное выполнение запроса", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshCallsData();
                }
                else
                {
                    MessageBox.Show("Ошибка при выполнении запроса", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshCallsData()
        {
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
    }
}
