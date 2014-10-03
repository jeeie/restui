using Photonware.RestUI.Core;
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

namespace RestUI.Content
{
    /// <summary>
    /// Sample.xaml 的交互逻辑
    /// </summary>
    public partial class Sample : UserControl
    {
        public event EventHandler<EventArgs> UserCaseSelected;

        public Sample()
        {
            InitializeComponent();
            List<UserCase> tcs = new List<UserCase>();
            tcs.Add(new UserCase("1"));
            tcs.Add(new UserCase("2"));
            listBox.ItemsSource = tcs;

            listBox.SelectionChanged += Items_CurrentChanged;
        }

        void Items_CurrentChanged(object sender, EventArgs e)
        {
            //UserCaseSelected(listBox.SelectedItem, new EventArgs());
        }
    }
}
