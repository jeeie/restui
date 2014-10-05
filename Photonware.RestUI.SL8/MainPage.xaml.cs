using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Photonware.RestUI.SL8.Resources;
using Photonware.RestUI.WP.Utils;
using Microsoft.Devices;
using Photonware.RestUI.SL8.ViewModels;
using Photonware.RestUI.Core;

namespace Photonware.RestUI.SL8
{
    public partial class MainPage : PhoneApplicationPage
    {

        private ExecutionResultViewModel executionViewModel = new ExecutionResultViewModel();
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 将 listbox 控件的数据上下文设置为示例数据
            DataContext = App.ViewModel;
            this.executionList.DataContext = executionViewModel;

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
  
        }

        // 为 ViewModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //{
                App.ViewModel.LoadData();
                executionViewModel.LoadData();
            //} 
                this.DataContext = App.ViewModel;
                this.executionList.DataContext = executionViewModel;

                if (e.NavigationMode == NavigationMode.New)
                {
                    string voiceCommandName = string.Empty;
                    if (NavigationContext.QueryString.TryGetValue("voiceCommandName", out voiceCommandName))
                    {
                        System.Diagnostics.Debug.WriteLine(voiceCommandName);
                        HandleVoiceCommand(voiceCommandName);
                    }
                    else
                    {
                        System.Threading.Tasks.Task.Run(async () =>
                        {
                            await Windows.Phone.Speech.VoiceCommands.VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri("ms-appx:///VoiceCommandDefinition1.xml"));
                        });
                    }
                }
        }

        private void HandleVoiceCommand(string voiceCommandName)
        {
            // Voice Commands can be typed into Cortana; when this happens, "voiceCommandMode" is populated with the
            // "textInput" value. In these cases, we'll want to behave a little differently by not speaking back.
            bool typedVoiceCommand = (NavigationContext.QueryString.ContainsKey("commandMode")
                && (NavigationContext.QueryString["commandMode"] == "text"));

            string spokenNumber = string.Empty;
            bool doSearch = false;

            switch (voiceCommandName)
            {
                case "StartTestCase":
                    if (NavigationContext.QueryString.TryGetValue("num", out spokenNumber)
                        && !String.IsNullOrEmpty(spokenNumber))
                    {
                        int number = -1;
                        if (int.TryParse(spokenNumber, out number))
                        {
                            if (number > 0 && number <= UserCaseManager.Instance.UserCases.Count)
                            {
                                UserCase uc = UserCaseManager.Instance.UserCases[number - 1].Clone();
                                UserCaseExecution exec = new UserCaseExecution(uc);
                                Photonware.RestUI.SL8.Execution.ExecutionManager.Instance.Add(exec);
                                App.SelectedUserCaseExecution = exec;
                                App.SelectedUserCase = uc;
                                NavigationService.Navigate(new Uri("/Xamls/UserCaseExecutionPage.xaml?operation=run", UriKind.Relative));
                            }
                        }
                    }
                    break;
                case "QRCode":
                    // The user explicitly asked to search, so we'll attempt to retrieve the query.
                    //NavigationContext.QueryString.TryGetValue("dictatedSearchTerms", out phraseTopicContents);
                    //doSearch = true;
                    NavigationService.Navigate(new Uri("/Xamls/QRCodePage.xaml", UriKind.Relative));
                    return;
                default:
                    break;
            }

            if (doSearch)
            {
                //HandleSearchQuery(phraseTopicContents, typedVoiceCommand);
                //System.Diagnostics.Debug.WriteLine(phraseTopicContents +", " + typedVoiceCommand);

            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            var result = MessageBox.Show("Quit RestUI?", "Quit", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Xamls/QRCodePage.xaml", UriKind.Relative));

        }

        private void userCaseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var uc = e.AddedItems[0] as UserCaseViewModel;
                App.SelectedUserCaseViewModel = uc;
                NavigationService.Navigate(new Uri("/Xamls/UserCasePage.xaml", UriKind.Relative));

            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Xamls/SettingsPage.xaml", UriKind.Relative));
        }

        private void executionList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var executionList = sender as LongListSelector;
            if (executionList.SelectedItem != null)
            {
                var selectedItem = (executionList.SelectedItem as UserCaseExecutionViewModel);
                if (selectedItem != null)
                {
                    App.SelectedUserCaseExecution = selectedItem.UserCaseExecution;
                    NavigationService.Navigate(new Uri("/Xamls/UserCaseExecutionPage.xaml?operation=show", UriKind.Relative));
                }
            }
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}