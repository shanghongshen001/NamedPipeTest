using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core;

namespace win4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class MainWindow : Window, IMessagesInterface
    {
        public NamedPipeServer ServerHost;
        private Basic basic;
        public MainWindow()
        {
            InitializeComponent();
            ServerHost = new NamedPipeServer(this, ClientNames.win4);
            Browser.Navigate(System.Environment.CurrentDirectory + "\\win4.html");
            basic = new Basic(this);
            Browser.ObjectForScripting = basic;

        }

        public string DoMessage(string text)
        {
            this.Browser.InvokeScript("addMesage", text);
            return "";
        }

    }

    [System.Runtime.InteropServices.ComVisibleAttribute(true)]//将该类设置为com可访问  
    public class Basic
    {
        private MainWindow mainWindow;

        public Basic(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public void ClickEvent(string str, string values)
        {
            var enes = values.Trim(',').Split(',').Select(r => Convert.ToInt32(r)).Select(r => (ClientNames)r);
            foreach (var ele in enes)
            {
                mainWindow.ServerHost.SendMessage(ele.ToString(), mainWindow.ServerHost.SelfName + ":" + str);
            }
        }
    }
}
