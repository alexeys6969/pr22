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
using ClassConnection;

namespace Phonebook_Shashin.Elements
{
    /// <summary>
    /// Логика взаимодействия для Cancel_itm.xaml
    /// </summary>
    public partial class Cancel_itm : UserControl
    {
        public Cancel_itm()
        {
            InitializeComponent();
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.connect.filteredCalls.Clear();
        }
    }
}
