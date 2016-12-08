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
using System.Windows.Shapes;
using TimeMiner.Slave.Hooks;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        IWinHook hk;
        public TestWindow()
        {
            InitializeComponent();
            hk = new KeyboardHook();
            hk.Hook();
        }

        private void btTest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(hk.ActionsCount.ToString());
            hk.Reset();
        }
    }
}
