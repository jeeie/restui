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
    public partial class ActionList : UserControl
    {
        public event EventHandler<EventArgs> ActionSelected;

        private UserCase userCase = null;
        private bool itemSelected = false;
        private long lastMouseDownTime = 0;

        private ContextMenu menu = new System.Windows.Controls.ContextMenu();

        public ActionList()
        {
            InitializeComponent();

            listBox.SelectionChanged += listBox_SelectionChanged;
            listBox.MouseRightButtonUp += listBox_MouseRightButtonUp;
            this.UpdateListAsync();

            menu.Items.Add(new MenuItem() { Header = "Add", Name = "add" });
            menu.Items.Add(new MenuItem() { Header = "Delete", Name = "delete" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Copy", Name = "copy" });
            menu.Items.Add(new MenuItem() { Header = "Paste", Name = "paste" });
            menu.Items.Add(new MenuItem() { Header = "Duplicate", Name = "duplicate" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Enable", Name = "enable" });
            menu.Items.Add(new MenuItem() { Header = "Disable", Name="disable" });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Tentative", Name="tentative", IsCheckable=true, IsChecked=false });
            menu.Items.Add(new Separator());
            menu.Items.Add(new MenuItem() { Header = "Rename", Name = "rename" });


            foreach (var item in menu.Items)
            {
                if (item is MenuItem)
                (item as MenuItem).Click += item_Click;
            }


            // add Copy Command
            CommandBinding copyCmd = new CommandBinding();
            copyCmd.Command = ApplicationCommands.Copy;
            copyCmd.Executed += copyCmd_Executed;
            copyCmd.CanExecute += copyCmd_CanExecute;

            this.CommandBindings.Add(copyCmd);

            KeyBinding cmdKey = new KeyBinding();
            cmdKey.Key = Key.C;
            cmdKey.Modifiers = ModifierKeys.Control;
            cmdKey.Command = copyCmd.Command;

            this.InputBindings.Add(cmdKey);

            // add Paste Command
            CommandBinding pasteCmd = new CommandBinding();
            pasteCmd.Command = ApplicationCommands.Paste;
            pasteCmd.Executed += pasteCmd_Executed;
            pasteCmd.CanExecute += pasteCmd_CanExecute;

            this.CommandBindings.Add(pasteCmd);

            cmdKey = new KeyBinding();
            cmdKey.Key = Key.V;
            cmdKey.Modifiers = ModifierKeys.Control;
            cmdKey.Command = pasteCmd.Command;

            this.InputBindings.Add(cmdKey);

            
            this.listBox.AllowDrop = true;
            this.listBox.MouseMove += listBox_MouseMove;
            this.listBox.PreviewMouseLeftButtonDown += listBox_MouseLeftButtonDown;
            this.listBox.DragOver += listBox_DragOver;
            this.listBox.Drop += listBox_Drop;
            this.listBox.PreviewDragEnter += listBox_PreviewDragEnter;
        }

        void pasteCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void pasteCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
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

                            CustomAction ca = CustomAction.ParseJsonString(line);
                            if (ca != null)
                            {
                                this.userCase.AddAction(ca);
                            }

                        }
                    }
                    this.UpdateListAsync();
                }
            }
        }

        void copyCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.listBox.SelectedItems.Count > 0;
        }

        void copyCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (CustomAction ca in listBox.SelectedItems)
            {
                sb.AppendLine(ca.ToJsonString());
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
        }



        void listBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            this.listBox.AllowDrop = e.Data.GetDataPresent(typeof(CustomAction));
        }

        void listBox_DragOver(object sender, DragEventArgs e)
        {
            this.listBox.AllowDrop = e.Data.GetDataPresent(typeof(CustomAction));

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

        void listBox_Drop(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(this.listBox);

            if (e.Data.GetDataPresent(typeof(CustomAction)))
            {

                var item = e.Data.GetData(typeof(CustomAction));

                var result = VisualTreeHelper.HitTest(listBox, pos);

                var target = listBox.ContainerFromElement(result.VisualHit);

                if (target == null)
                {
                    this.userCase.RemoveAction(item as CustomAction);
                    this.userCase.AddAction(item as CustomAction);
                    this.UpdateListAsync();
                    return;
                }

                pos.Y -= (target as ListBoxItem).ActualHeight / 2;

                if (pos.Y < 0)
                {
                    if ((target as ListBoxItem).Content == item) return;
                    this.userCase.RemoveAction(item as CustomAction);
                    this.userCase.InsertActionBefore((target as ListBoxItem).Content as CustomAction, item as CustomAction);
                    this.UpdateListAsync();
                }
                else
                {
                    result = VisualTreeHelper.HitTest(listBox, pos);
                    target = listBox.ContainerFromElement(result.VisualHit);

                    if (target is ListBoxItem)
                    {
                        if ((target as ListBoxItem).Content == item) return;
                        this.userCase.RemoveAction(item as CustomAction);
                        this.userCase.InsertActionAfter((target as ListBoxItem).Content as CustomAction, item as CustomAction);
                        this.UpdateListAsync();
                    }
                }
            }
        }

        void listBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.lastMouseDownTime = DateTime.Now.Ticks;

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

        void listBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (lastMouseDownTime == 0) return;

                long now = DateTime.Now.Ticks;
                if ((now - lastMouseDownTime) / 1000 / 10 < 200)
                {
                    lastMouseDownTime = 0;
                    return;
                }

                if (!itemSelected) return;

                if (this.listBox.SelectedItems == null || this.listBox.SelectedItems.Count <= 0) return;

                if (this.listBox.SelectedValue != null)
                {
                    this.listBox.AllowDrop = true;
                    
                    DragDrop.DoDragDrop(this.listBox, this.listBox.SelectedItem, DragDropEffects.Move);
                }
            }
        }

        void item_Click(object sender, RoutedEventArgs e)
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
                    foreach (CustomAction ca in listBox.SelectedItems)
                    {
                        sb.AppendLine(ca.ToJsonString());
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

                                    CustomAction ca = CustomAction.ParseJsonString(line);
                                    if (ca != null)
                                    {
                                        this.userCase.AddAction(ca);
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
                case "tentative":
                    bool enabled = item.IsChecked;
                    var customAction = listBox.SelectedItem as CustomAction;
                    if (customAction != null)
                    {
                        customAction.IsTentative = enabled;
                    }
                    this.UpdateListAsync();

                    break;
                case "rename":
                    CustomAction _ca = listBox.SelectedItem as CustomAction;
                    var v = new FirstFloor.ModernUI.Windows.Controls.ModernDialog
                    {
                        Title = "Rename",
                        Content = new InputContent(_ca.Text),
                        MinWidth=400
                    };
                    
                    var okCommand = new FirstFloor.ModernUI.Presentation.RelayCommand(o =>
                    {
                        v.DialogResult = true;
                        v.Close();
                    });
                    v.OkButton.Command = okCommand;
                    v.Buttons = new Button[] { v.OkButton, v.CancelButton };

                    v.ShowDialog();
                    if ((bool)v.DialogResult)
                    {

                        _ca.Text = (v.Content as InputContent).Text;
                        this.UpdateListAsync();

                    }
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
            (menu.Items.GetItemAt(5) as MenuItem).IsEnabled = listBox.SelectedItems.Count == 1;
            (menu.Items.GetItemAt(7) as MenuItem).IsEnabled = listBox.SelectedItem != null;
            (menu.Items.GetItemAt(8) as MenuItem).IsEnabled = listBox.SelectedItem != null;
            (menu.Items.GetItemAt(10) as MenuItem).IsEnabled = listBox.SelectedItems.Count == 1;
            (menu.Items.GetItemAt(12) as MenuItem).IsEnabled = listBox.SelectedItems.Count == 1;

            if ((menu.Items.GetItemAt(10) as MenuItem).IsEnabled)
            {
                var ca = listBox.SelectedItem as CustomAction;
                (menu.Items.GetItemAt(10) as MenuItem).IsChecked = ca.IsTentative;
            }

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
            if (userCase == null)
            {
                listBox.ItemsSource = null;
                return;
            }
            listBox.ItemsSource = new ObservableCollection<CustomAction>(this.userCase.Actions);
        }

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            deleteButton.IsEnabled = listBox.SelectedItem != null;
            duplicateButton.IsEnabled = upButton.IsEnabled = downButton.IsEnabled = listBox.SelectedItems.Count == 1;
            
            if (ActionSelected != null)
                ActionSelected(listBox.SelectedItem, new EventArgs());
        }


        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = new System.Windows.Controls.ContextMenu();

            lock (ActionManager.Instance.SyncRoot)
            {
                foreach (string key in ActionManager.Instance.Actions.Keys)
                {
                    Photonware.RestUI.Core.Action action = ActionManager.Instance.Actions[key];
                    MenuItem item = new MenuItem() { Header = action.Text, Tag = action };
                    item.Click += menuitem_Click;
                    menu.Items.Add(item);
                }
            }

            menu.IsEnabled = true;
            menu.IsOpen = true;


           // UserCaseManager.Instance.AddUserCase(new UserCase("User Case"));
            //UserCaseManager.Instance.AddUserCase(new UserCase("test1"));
           // this.UpdateListAsync();
        }

        void menuitem_Click(object sender, RoutedEventArgs e)
        {
            if (this.userCase == null) return;
            MenuItem item = sender as MenuItem;
            if (item == null) return;
            CustomAction caction = new CustomAction();
            Photonware.RestUI.Core.Action action = item.Tag as Photonware.RestUI.Core.Action;
            caction.Key = action.Key;
            caction.Text = action.Text;
            this.userCase.AddAction(caction);
            this.UpdateListAsync();
        }


        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in listBox.SelectedItems)
            {
                CustomAction action = item as CustomAction;
                this.userCase.RemoveAction(action);
            }

            this.UpdateListAsync();
        }


        public void SetUsercase(UserCase userCase)
        {
            this.userCase = userCase;
            this.UpdateListAsync();
        }

        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            CustomAction action = this.listBox.SelectedItem as CustomAction;
            this.userCase.MoveUp(action);
            this.UpdateListAsync();
        }

        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            CustomAction action = this.listBox.SelectedItem as CustomAction;
            this.userCase.MoveDown(action);
            this.UpdateListAsync();
        }

        private void Duplicate_Button_Click(object sender, RoutedEventArgs e)
        {
            CustomAction ca = listBox.SelectedItem as CustomAction;
            if (ca == null) return;
            var newAction = ca.Clone();
            newAction.Text += " - Copy";
            this.userCase.AddAction(newAction);
            this.UpdateListAsync();
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
