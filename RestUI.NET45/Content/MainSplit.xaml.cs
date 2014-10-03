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
    /// MainSplit.xaml 的交互逻辑
    /// </summary>
    public partial class MainSplit : UserControl
    {
        public MainSplit()
        {
            InitializeComponent();
        }

        private void UserCaseList_UserCaseSelected(object sender, EventArgs e)
        {
            actionList.SetUsercase(sender as Photonware.RestUI.Core.UserCase);   
        }

        private void actionList_ActionSelected(object sender, EventArgs e)
        {
            contentBox.SetAction(sender as Photonware.RestUI.Core.CustomAction);
        }
    }
}
