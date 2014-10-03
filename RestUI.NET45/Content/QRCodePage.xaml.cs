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
using ZXing;
using BarcodeReader = ZXing.Presentation.BarcodeReader;
using BarcodeWriter = ZXing.Presentation.BarcodeWriter;
using BarcodeWriterGeometry = ZXing.Presentation.BarcodeWriterGeometry;

namespace RestUI.Content
{
    /// <summary>
    /// QRCodePage.xaml 的交互逻辑
    /// </summary>
    public partial class QRCodePage : UserControl
    {
        public QRCodePage()
        {
            InitializeComponent();
            this.Url = string.Empty;
            this.Loaded += QRCodePage_Loaded;
            this.Unloaded += QRCodePage_Unloaded;
            
        }

        void QRCodePage_Unloaded(object sender, RoutedEventArgs e)
        {
            imageBarcodeEncoder.Source = null;
        }

        void QRCodePage_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ipList = this.GetLocalIPList();
            foreach (string ip in ipList)
            {
                this.urlComboBox.Items.Add(string.Format("http://{0}:{1}/{2}", ip, this.HttpPort, ""));
            }

            if (urlComboBox.Items.Count > 0)
            {
                urlComboBox.SelectedIndex = 0;
            }

            this.GenerateQRCode();
        }

        public string Url { get; set; }

        public int HttpPort { get; set; }

        public void SetUrl(string url)
        {
            this.Url = url;
            //this.urlText.Text = url;

            string[] ipList = this.GetLocalIPList();
            foreach (string ip in ipList)
            {
                this.urlComboBox.Items.Add(ip);
            }

            if (urlComboBox.Items.Count > 0)
            {
                urlComboBox.SelectedIndex = 0;
            }

            this.GenerateQRCode();
        }
        private void GenerateQRCode()
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200,
                    //Height = (int)imageBarcodeEncoder.Height,
                    //Width = (int)imageBarcodeEncoder.Width,
                    Margin = 0
                }
            };
            
            var image = writer.Write(this.Url);
            imageBarcodeEncoder.Source = image;
            imageBarcodeEncoder.Visibility = Visibility.Visible;
        }

        private string[] GetLocalIPList()
        {
            StringBuilder sb = new StringBuilder();

            System.Net.IPHostEntry _IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress _IPAddress in _IPHostEntry.AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    sb.AppendLine(_IPAddress.ToString());
                }
            }
            return sb.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Url = urlComboBox.SelectedValue as string;
            Dispatcher.Invoke((Action)delegate
            {
                this.GenerateQRCode();
            });
        }
    }
}
