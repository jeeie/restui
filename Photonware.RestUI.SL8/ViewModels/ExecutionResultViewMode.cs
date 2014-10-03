using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Photonware.RestUI.SL8.Resources;
using Photonware.RestUI.Core;
using Photonware.RestUI.SL8.Execution;

namespace Photonware.RestUI.SL8.ViewModels
{
    public class ExecutionResultViewModel : INotifyPropertyChanged
    {
        public ExecutionResultViewModel()
        {
            this.Items = new ObservableCollection<UserCaseExecutionViewModel>();
        }

        /// <summary>
        /// ItemViewModel 对象的集合。
        /// </summary>
        public ObservableCollection<UserCaseExecutionViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建一些 ItemViewModel 对象并将其添加到 Items 集合中。
        /// </summary>
        public void LoadData()
        {
            this.Items.Clear();

            foreach (UserCaseExecution userCase in ExecutionManager.Instance.UserCases)
            {
                this.Items.Add(new UserCaseExecutionViewModel() { Title = userCase.Text, Status=userCase.Status.ToString(), UserCaseExecution = userCase });
            }

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}