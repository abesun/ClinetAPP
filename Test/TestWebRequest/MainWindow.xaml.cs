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
using ClientAPP.Core.Contract.Websocket;
using Newtonsoft.Json;
namespace TestWebRequest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WSProtocol wsp = new WSProtocol()
            {
                Header = new WSPHeader() { BodyType = BodyType.Request, SN = "1", Time = DateTime.Now.ToString(), Ver = "1.0" },
            };
            WSRequest request = new WSRequest() { Command = WSVideoRequest.OpenWindow, Module = WSDefine.VideoModule, RequestSN = "1", Sync = true };
            WSVideoRequest_OpenWindow req = new WSVideoRequest_OpenWindow()
            {
                LayoutName = "16",
                Height = 600,
                Width = 800,
                LocationX = 0,
                LocationY = 0,
                PanelID = "1",
                ScreenID = 0,
                ShowVCTitle = true,
                ShowWindowBorder = true,
                TopMost = true
            };
            request.Params = req;

            wsp.Body = request;
            this.ricktextbox.AppendText($"{JsonConvert.SerializeObject(wsp)}\n");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WSProtocol wsp = new WSProtocol()
            {
                Header = new WSPHeader() { BodyType = BodyType.Request, SN = "1", Time = DateTime.Now.ToString(), Ver = "1.0" },
            };
            WSRequest request = new WSRequest() { Command = WSVideoRequest.StartPreview, Module = WSDefine.VideoModule, RequestSN = "1", Sync = true };
            WSVideoRequest_StartPreview req = new WSVideoRequest_StartPreview()
            {
                CameraCode="", CameraID="1", CameraName="测试", PanelID="1", StreamIndex=0, VCIndex=0,
                SourceID="1", SourceIP="192.168.0.1", SourceName="海康8200", SourcePort=9000, SourceType=200, SourcePassword="password", SourceUser="admin"
            };
            request.Params = req;

            wsp.Body = request;
            this.ricktextbox.AppendText($"{JsonConvert.SerializeObject(wsp)}\n");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WSProtocol wsp = new WSProtocol()
            {
                Header = new WSPHeader() { BodyType = BodyType.Request, SN = "1", Time = DateTime.Now.ToString(), Ver = "1.0" },
            };
            WSRequest request = new WSRequest() { Command = WSVideoRequest.StartPlayback, Module = WSDefine.VideoModule, RequestSN = "1", Sync = true };
            WSVideoRequest_StartPlayback req = new WSVideoRequest_StartPlayback()
            {
                CameraCode = "",
                CameraID = "1",
                CameraName = "测试",
                PanelID = "1",
                VCIndex = 0,
                SourceID = "1",
                SourceIP = "192.168.0.1",
                SourceName = "海康8200",
                SourcePort = 9000,
                SourceType = 200,
                SourcePassword = "password",
                SourceUser = "admin",
                StartTime = DateTime.Now.AddHours(-1),
                EndTime = DateTime.Now
            };
            request.Params = req;

            wsp.Body = request;
            this.ricktextbox.AppendText($"{JsonConvert.SerializeObject(wsp)}\n");
        }
    }
}
