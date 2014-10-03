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
    public class UserCaseExecutionViewModel : INotifyPropertyChanged
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

        private string _status;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public UserCaseExecution UserCaseExecution { get; set; }

        public bool IsDataLoaded { get; private set; }
        public ObservableCollection<ExecutionAction> ExecutionActions { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public UserCaseExecutionViewModel()
        {

        }

        public void LoadData()
        {
            //if (!IsDataLoaded)
           // {
                if (UserCaseExecution != null)
                {
                    this.ExecutionActions = new ObservableCollection<ExecutionAction>(UserCaseExecution.Context.ExecutionActions);
                    this.NotifyPropertyChanged("ExecutionActions");
                    this.Title = this.UserCaseExecution.Text;
                    this.Status = this.UserCaseExecution.Status.ToString(); ;
                    IsDataLoaded = true;
                }

            //}
        }
    }
}