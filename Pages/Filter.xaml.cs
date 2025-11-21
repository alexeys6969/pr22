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
        Call call_itm1;
        public Filter(Call _call)
        {
            InitializeComponent();
            call_itm1 = _call ?? new Call();

            category_select.Items.Clear();

            ComboBoxItem comb = new ComboBoxItem();
            comb.Tag = 1;
            comb.Content = "Исходящий";
            if (call_itm1.category_call == 1) comb.IsSelected = true;
            category_select.Items.Add(comb);

            ComboBoxItem comb1 = new ComboBoxItem();
            comb1.Tag = 2;
            comb1.Content = "Входящий";
            if (call_itm1.category_call == 2) comb1.IsSelected = true;
            category_select.Items.Add(comb1);

            num_select.Items.Clear();
            MainWindow.connect.LoadData(ClassConnection.Connection.tabels.users);

            foreach (User itm1 in MainWindow.connect.users)
            {
                ComboBoxItem combUser1 = new ComboBoxItem();
                combUser1.Tag = itm1.id;
                combUser1.Content =  $"{itm1.phone_num}({itm1.fio_user})";
                if (call_itm1.user_id == itm1.id) combUser1.IsSelected = true;
                num_select.Items.Add(combUser1);
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
