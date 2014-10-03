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
    /// InputContent.xaml 的交互逻辑
    /// </summary>
    public partial class InputContent : UserControl
    {

        public string Text { get; private set; }
        public InputContent(string originString)
        {
            InitializeComponent();
            this.textBox.Text = originString;
            this.textBox.TextChanged += textBox_TextChanged;
            this.textBox.Focus();
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Text = textBox.Text;
        }

    }
}
