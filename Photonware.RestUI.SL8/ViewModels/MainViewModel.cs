using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Photonware.RestUI.SL8.Resources;
using Photonware.RestUI.Core;

namespace Photonware.RestUI.SL8.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<UserCaseViewModel>();
        }

        /// <summary>
        /// ItemViewModel 对象的集合。
        /// </summary>
        public ObservableCollection<UserCaseViewModel> Items { get; private set; }

        /// <summary>
        /// 返回本地化字符串的示例属性
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

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

            foreach (UserCase uc in UserCaseManager.Instance.UserCases)
            {
                this.Items.Add(new UserCaseViewModel() { Title = uc.Text, Description = uc.Description, UserCase = uc });
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