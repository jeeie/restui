using Photonware.RestUI.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Photonware.RestUI.SL8.ViewModels
{
    public class UserCaseViewModel : INotifyPropertyChanged
    {
        private string _title;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _description;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public UserCase UserCase
        {
            get;
            set;
        }

        public bool IsActionLoaded
        {
            get;
            private set;
        }

        public ObservableCollection<CustomActionViewModel> Actions { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public UserCaseViewModel()
        {
            this.Actions = new ObservableCollection<CustomActionViewModel>();
        }

        public void LoadAction()
        {
            if (this.UserCase != null)
            {
                foreach (CustomAction action in this.UserCase.Actions)
                {
                    Photonware.RestUI.Core.Action _action = ActionManager.Instance.GetAction(action.Key);
                    
                    string s = _action == null ? "unsupported action" : _action.Text;
                    this.Actions.Add(new CustomActionViewModel() { Title = action.Text, Description = s, ActionName = s, Content = action.Content, CustomAction = action });
                }
                this.IsActionLoaded = true;
            }
        }
    }
}