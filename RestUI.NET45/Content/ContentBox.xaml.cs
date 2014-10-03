using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Photonware.RestUI.Core;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace RestUI.Content
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ContentBox : UserControl
    {

        private CustomAction action = null;

        public ContentBox()
        {
            InitializeComponent();
            this.UpdateContentAsync();

            this.textBox.TextChanged += textBox_TextChanged;

        }

        void textBox_TextChanged(object sender, EventArgs e)
        {
            if (action != null)
            {
                action.Content = this.textBox.Text;
                UserCaseManager.Instance.Modified = true;
            }
        }

        public void SetAction(CustomAction action)
        {
            this.action = action;
            this.textBox.TextChanged -= textBox_TextChanged;
            this.UpdateContentAsync();
            exampleButton.IsEnabled = this.action != null;
            this.textBox.IsEnabled = this.action != null;
            if ((this.action != null) && (ActionManager.Instance.GetAction(action.Key) != null))
            {

                this.actionText.Text = ActionManager.Instance.GetAction(action.Key).Text;
                if (!string.IsNullOrEmpty(ActionManager.Instance.GetAction(action.Key).Xshd))
                {
                    using (StringReader strReader = new StringReader(ActionManager.Instance.GetAction(action.Key).Xshd))
                    {
                        using (XmlTextReader xmlReader = new XmlTextReader(strReader))
                        {
                            try
                            {
                                
                                this.textBox.SyntaxHighlighting = HighlightingLoader.Load(xmlReader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                            }
                            catch { 
                                this.textBox.SyntaxHighlighting = null; 
                            }
                        }
                    }
                }
                else
                {
                    this.textBox.SyntaxHighlighting = null; 
                }
            }
            else
            {
                this.actionText.Text = string.Empty;
                this.textBox.SyntaxHighlighting = null;
            }
            this.textBox.TextChanged += textBox_TextChanged;
        }

        private void UpdateContentAsync()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new System.Action(UpdateContent)).Wait();
        }

        private void UpdateContent()
        {
            //textBox.Clear();
            //int tmp = textBox.UndoLimit;
            //textBox.UndoLimit = 0;
            textBox.Text = action == null || action.Content == null ? string.Empty : action.Content;
            //textBox.UndoLimit = tmp;
        }

        private void exampleButton_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = new System.Windows.Controls.ContextMenu();

            lock (ActionManager.Instance.SyncRoot)
            {
                Photonware.RestUI.Core.Action act = ActionManager.Instance.GetAction(action.Key);
                foreach (ActionExample ae in act.Examples)
                {
                    MenuItem item = new MenuItem() { Header = ae.Text, Tag = ae };
                    item.Click += item_Click;
                    menu.Items.Add(item);
                }
            }

            menu.IsEnabled = true;
            menu.IsOpen = true;
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            if (this.action == null) return;
            MenuItem item = sender as MenuItem;
            if (item == null) return;
            
            ActionExample ae = item.Tag as ActionExample;
            this.action.Content = ae.Content;
            //this.textBox.TextChanged -= textBox_TextChanged;
            this.textBox.Document.Text = this.action.Content;
            //this.textBox.TextChanged += textBox_TextChanged;
        }

        private void wrapButton_Checked(object sender, RoutedEventArgs e)
        {
            textBox.WordWrap = true;
        }

        private void wrapButton_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox.WordWrap = false;
        }
    }
}
