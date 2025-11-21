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
    /// Логика взаимодействия для FilterItm.xaml
    /// </summary>
    public partial class FilterItm : UserControl
    {
        Page page_str;
        public FilterItm(Page _page_str)
        {
            InitializeComponent();
            page_str = _page_str;

            DoubleAnimation op = new DoubleAnimation();
            op.From = 0;
            op.To = 1;
            op.Duration = TimeSpan.FromSeconds(0.4);
            border.BeginAnimation(StackPanel.OpacityProperty, op);
        }

        private void Click_Filter(object sender, RoutedEventArgs e)
        {
            MainWindow.main.Anim_Move(MainWindow.main.scroll_main, MainWindow.main.frame_main, MainWindow.main.frame_main, page_str);
        }
    }
}
