using FirstFloor.ModernUI.Windows.Controls;
using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace RestUI.Content
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UserCaseList : UserControl
    {
        public event EventHandler<EventArgs> UserCaseSelected;

        private bool itemSelected = false;

        private ContextMenu menu = new System.Windows.Controls.ContextMenu();

        public UserCaseList()
        {
            InitializeComponent();

            listBox.SelectionChanged += listBox_SelectionChanged;
            //listBox.MouseDoubleClick += listBox_MouseDoubleClick;
            //txtNode.LostFocus += txtNode_LostFocus;
            this.UpdateList();

            listBox.MouseRightButtonUp += listBox_MouseRightButtonUp;

            menu.Items.Add(new MenuItem() { Header = "Add", Name = "add" });
            menu.Items.Add(new MenuItem() { Header = "Delete", Name = "delete" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Copy", Name = "copy" });
            menu.Items.Add(new MenuItem() { Header = "Paste", Name = "paste" });
            menu.Items.Add(new MenuItem() { Header = "Duplicate", Name = "duplicate" });
            menu.Items.Add(new Separator());
            //menu.Items.Add(new MenuItem() { Header = "Enable", Name = "enable" });
            //menu.Items.Add(new MenuItem() { Header = "Disable", Name = "disable" });
            //menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Rename", Name = "rename" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Start Transferring", Name = "transfer" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Execute", Name = "execute" });

            foreach (var item in menu.Items)
            {
                if (item is MenuItem)
                    (item as MenuItem).Click += UserCaseList_Click;
            }

            this.listBox.MouseMove += listBox_MouseMove;
            this.listBox.DragOver += listBox_DragOver;
            this.listBox.PreviewMouseLeftButtonDown += listBox_PreviewMouseLeftButtonDown;
        }



        void UserCaseList_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null) return;
            string command = item.Name;
            switch (command)
            {
                case "add":
                    this.Add_Button_Click(this, null);
                    break;
                case "delete":
                    this.Delete_Button_Click(this, null);
                    break;
                case "copy":
                    Clipboard.Clear();
                    StringBuilder sb = new StringBuilder();
                    foreach (UserCase _uc in listBox.SelectedItems)
                    {
                        sb.AppendLine(_uc.ToJsonString());
                    }
                    DataObject data = new DataObject(DataFormats.Text, sb.ToString());
                    try
                    {
                        Clipboard.SetDataObject(data);
                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        System.Threading.Thread.Sleep(10);
                        try
                        {
                            Clipboard.SetDataObject(data);
                        }
                        catch (System.Runtime.InteropServices.COMException)
                        {

                        }
                    }
                    break;
                case "paste":
                    if (Clipboard.ContainsData(DataFormats.Text))
                    {
                        string json = Clipboard.GetData(DataFormats.Text) as string;
                        if (!string.IsNullOrEmpty(json))
                        {
                            using (System.IO.StringReader reader = new System.IO.StringReader(json))
                            {
                                string line = string.Empty;
                                while ((line = reader.ReadLine()) != null)
                                {

                                    UserCase _uc = UserCase.ParseJsonString(line);
                                    if (_uc != null)
                                    {
                                        UserCaseManager.Instance.AddUserCase(_uc);
                                    }

                                }
                            }
                            this.UpdateListAsync();
                        }
                    }
                    break;
                case "duplicate":
                    this.Duplicate_Button_Click(this, null);
                    break;
                case "enable":
                    foreach (CustomAction ca in listBox.SelectedItems)
                    {
                        ca.IsDisabled = false;
                    }
                    this.UpdateListAsync();
                    break;
                case "disable":
                    foreach (CustomAction ca in listBox.SelectedItems)
                    {
                        ca.IsDisabled = true;
                    }
                    this.UpdateListAsync();

                    break;
                case "rename":
                    UserCase uc = listBox.SelectedItem as UserCase;
                    var v = new FirstFloor.ModernUI.Windows.Controls.ModernDialog
                    {
                        Title = "Rename",
                        Content = new InputContent(uc.Text),
                        MinWidth = 400
                    };

                    var okCommand = new FirstFloor.ModernUI.Presentation.RelayCommand(o =>
                    {
                        v.DialogResult = true;
                        v.Close();
                    });
                    v.OkButton.Command = okCommand;
                    v.Buttons = new Button[] { v.OkButton, v.CancelButton };
                    v.OkButton.IsDefault = true;

                    v.ShowDialog();
                    if ((bool)v.DialogResult)
                    {

                        uc.Text = (v.Content as InputContent).Text;
                        this.UpdateListAsync();

                    }
                    break;
                case "transfer":
                    UserCase uc1 = listBox.SelectedItem as UserCase;
                    var httpServer = new RestUI.Utils.SimpleHttpServer(new List<UserCase>() { uc1 }, 0);
                    //httpServer.StartToTransfer(new List<UserCase>() { uc1 });
                    httpServer.Start();
                    //ModernDialog.ShowMessage(httpServer.Url, "url", MessageBoxButton.OK);

                    var wnd = new ModernWindow
{
    Style = (Style)App.Current.Resources["EmptyWindow"],
    Content = new QRCodePage
    {
        Margin = new Thickness(32),
        Url = httpServer.Url,
        HttpPort = httpServer.Port
    },
    Title = "QRCode",
    Height=450,
    Width=560,
    IsTitleVisible = true,
    ShowInTaskbar=false
};
                    wnd.ResizeMode = ResizeMode.NoResize;
                    wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    wnd.Topmost = true;
                    wnd.ShowDialog();

                    httpServer.Stop();
                    break;
                case "execute":
                    Execute_Button_Click(this, null);
                    break;
                default:
                    break;
            }
        }

        void listBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            (menu.Items.GetItemAt(1) as MenuItem).IsEnabled = listBox.SelectedItem != null;
            (menu.Items.GetItemAt(3) as MenuItem).IsEnabled = listBox.SelectedItem != null;
            (menu.Items.GetItemAt(4) as MenuItem).IsEnabled = false;
            (menu.Items.GetItemAt(5) as MenuItem).IsEnabled = listBox.SelectedItem != null;
            (menu.Items.GetItemAt(7) as MenuItem).IsEnabled = listBox.SelectedItems.Count == 1;
            (menu.Items.GetItemAt(9) as MenuItem).IsEnabled = listBox.SelectedItems.Count >= 1;
            (menu.Items.GetItemAt(11) as MenuItem).IsEnabled = listBox.SelectedItems.Count == 1;

            if (Clipboard.ContainsData(DataFormats.Text))
            {
                string json = Clipboard.GetData(DataFormats.Text) as string;
                if (!string.IsNullOrEmpty(json))
                {
                    (menu.Items.GetItemAt(4) as MenuItem).IsEnabled = true;
                }
            }

            menu.IsEnabled = true;
            menu.IsOpen = true;
        }

        public void UpdateListAsync()
        {
            this.listBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new System.Action(UpdateList));
        }

        public void UpdateList()
        {
            
            listBox.ItemsSource = new ObservableCollection<UserCase>(UserCaseManager.Instance.UserCases);
        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            duplicateButton.IsEnabled = deleteButton.IsEnabled = listBox.SelectedItem != null;
            executeButton.IsEnabled = listBox.SelectedItem != null;
            if (UserCaseSelected != null)
                UserCaseSelected(listBox.SelectedItem, new EventArgs());
        }


        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            UserCaseManager.Instance.AddUserCase(new UserCase("New User Case"));
            //UserCaseManager.Instance.AddUserCase(new UserCase("test1"));
            this.UpdateListAsync();
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            UserCase uc = listBox.SelectedItem as UserCase;
            if (uc == null) return;
            UserCaseManager.Instance.RemoveUserCase(uc);
            this.UpdateListAsync();
        }

        private void Execute_Button_Click(object sender, RoutedEventArgs e)
        {
            UserCase uc = listBox.SelectedItem as UserCase;
            if (uc == null) return;
            UserCase o = uc.Clone();

            var wnd = new ModernWindow
            {
                Style = (Style)App.Current.Resources["EmptyWindow"],
                Content = new UserCaseExecutionPage
                {
                    Margin = new Thickness(32),
                    UserCase = o
                },
                Width = 600,
                Height = 480,
                Title = o.Text,
                IsTitleVisible = true
            };

            wnd.Show();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserCaseManager.Instance.save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Failed: " + ex.Message, "ERROR");
            }
        }

        private void Duplicate_Button_Click(object sender, RoutedEventArgs e)
        {
            UserCase uc = listBox.SelectedItem as UserCase;
            if (uc == null) return;
            var newUserCase = uc.Clone();
            newUserCase.Text += " - Copy";
            UserCaseManager.Instance.AddUserCase(newUserCase);
            this.UpdateListAsync();
        }


        void listBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this.listBox);

            var result = VisualTreeHelper.HitTest(listBox, pos);

            var target = listBox.ContainerFromElement(result.VisualHit);

            if (target is ListBoxItem)
            {
                this.itemSelected = true;
            }
            else
            {
                this.itemSelected = false;
            }
        }


        private void listBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            ListBox li = sender as ListBox;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(li);

            double tolerance = 10;
            double verticalPos = e.GetPosition(li).Y;
            double offset = 3;

            if (verticalPos < tolerance) // Top of visible list?
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset - offset); //Scroll up.
            }
            else if (verticalPos > li.ActualHeight - tolerance) //Bottom of visible list?
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset + offset); //Scroll down.
            }
        }

        private void listBox_Drop(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(this.listBox);

            if (e.Data.GetDataPresent(typeof(UserCase)))
            {

                var item = e.Data.GetData(typeof(UserCase));

                var result = VisualTreeHelper.HitTest(listBox, pos);

                var target = listBox.ContainerFromElement(result.VisualHit);

                if (target == null)
                {
                    UserCaseManager.Instance.RemoveUserCase(item as UserCase);
                    UserCaseManager.Instance.AddUserCase(item as UserCase);
                    this.UpdateListAsync();
                    return;
                }

                pos.Y -= (target as ListBoxItem).ActualHeight / 2;
                

                if (pos.Y < 0 )
                {
                    if ((target as ListBoxItem).Content == item) return;
                    UserCaseManager.Instance.RemoveUserCase(item as UserCase);
                    UserCaseManager.Instance.InsertBefore((target as ListBoxItem).Content as UserCase, item as UserCase);
                    this.UpdateListAsync();
                }
                else
                {
                    result = VisualTreeHelper.HitTest(listBox, pos);
                    target = listBox.ContainerFromElement(result.VisualHit);

                    if (target is ListBoxItem)
                    {
                        if ((target as ListBoxItem).Content == item) return;
                        UserCaseManager.Instance.RemoveUserCase(item as UserCase);
                        UserCaseManager.Instance.InsertAfter((target as ListBoxItem).Content as UserCase, item as UserCase);
                        this.UpdateListAsync();
                    }
                }
            }
        }


        void listBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!itemSelected) return;

                if (this.listBox.SelectedItems == null || this.listBox.SelectedItems.Count <= 0) return;

                if (this.listBox.SelectedValue != null)
                {
                    DragDrop.DoDragDrop(this.listBox, this.listBox.SelectedItem, DragDropEffects.Move);
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }

    }
}
