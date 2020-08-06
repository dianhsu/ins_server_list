using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QueryMaster.GameServer;
using QueryMaster;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ServerInfoPreview> serverData = new ObservableCollection<ServerInfoPreview>();
        List<Tuple<string, ushort>> server_config = new List<Tuple<string, ushort>>();
        public MainWindow()
        {
            InitializeComponent();
            if (ServerConfig.xiaocao_list.Count == 0)
            {
                ThreadPool.QueueUserWorkItem(_ => {
                    HttpClient httpClient = new HttpClient();
                    string url = string.Format("http://api.steampowered.com/ISteamApps/GetServersAtAddress/v0001?addr={0}&format=json",ServerConfig.xiaocao);
                    Task<string> task0 = httpClient.GetStringAsync(url);
                    task0.Wait();
                    string sresult = task0.Result;
                    JObject jObject = JObject.Parse(sresult);
                    try
                    {
                        JArray jArray = (JArray)jObject["response"]["servers"];

                        foreach (JToken item in jArray)
                        {
                            string addr = item["addr"].ToString();
                            var ip_port = addr.Split(':');
                            ServerConfig.xiaocao_list.Add(new Tuple<string, ushort>(ip_port[0], (ushort)int.Parse(ip_port[1])));
                        }
                    }
                    catch (Exception)
                    {
                    }
                });
            }
            dataGrid.DataContext = serverData;
           
        }
        private void UpdateDataGrid()
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(8, 8);
            foreach (var item in server_config)
            {
                ThreadPool.QueueUserWorkItem(_=>
                {
                    Tuple<string, ushort> tp = item;                    
                    Server server = ServerQuery.GetServerInstance(EngineType.Source, tp.Item1, tp.Item2, false, 1000, 1000);
                    var server_info = server.GetInfo();                  
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,(ThreadStart)delegate ()
                    {
                        if (server_info != null)
                        {
                            serverData.Add(new ServerInfoPreview()
                            {
                                Map = server_info.Map,
                                Name = server_info.Name,
                                Players = string.Format("{0}/{1}", server_info.Players, server_info.MaxPlayers),
                                Host = tp.Item1,
                                Port = tp.Item2,
                                Ping = server_info.Ping
                            }); ;
                            dataGrid.Items.Refresh();
                        }
                    });
                });
            }
        }

        private void Other_Checked(object sender, RoutedEventArgs e)
        {
            serverData.Clear();
            server_config = ServerConfig.other;
            UpdateDataGrid();
        }

        private void XiaoCao_Checked(object sender, RoutedEventArgs e)
        {
            serverData.Clear();
            server_config = ServerConfig.xiaocao_list;
            UpdateDataGrid();
        }
        

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            Point aP = e.GetPosition(datagrid);
            IInputElement obj = datagrid.InputHitTest(aP);
            DependencyObject target = obj as DependencyObject;


            while (target != null)
            {
                if (target is DataGridRow)
                {
                    var s = (ServerInfoPreview)this.dataGrid.SelectedItem;
                    ServerDetail serverDetail = new ServerDetail(s.Host, s.Port);
                    serverDetail.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    serverDetail.Owner = this;
                    serverDetail.Show();
                    
                }
                target = VisualTreeHelper.GetParent(target);
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sip = (ServerInfoPreview)dataGrid.SelectedItem;
            string copyText = string.Format("{0}", sip.Host);
            Clipboard.SetText(copyText);
            MessageBox.Show(string.Format("IP已经复制到剪切板: {0}", copyText));
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_fresh_Click(object sender, RoutedEventArgs e)
        {
            serverData.Clear();
            UpdateDataGrid();
        }
    }
}
