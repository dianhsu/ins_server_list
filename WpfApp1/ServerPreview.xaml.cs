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
using QueryMaster.GameServer;
using QueryMaster;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

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
            dataGrid.DataContext = serverData;
           
        }
        private void UpdateDataGrid()
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);
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
                            });
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

        private void TuanZi_Checked(object sender, RoutedEventArgs e)
        {
            serverData.Clear();
            server_config = ServerConfig.tuanzi;
            UpdateDataGrid();
        }

        private void XiaoCao_Checked(object sender, RoutedEventArgs e)
        {
            serverData.Clear();
            server_config = ServerConfig.xiaocao;
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
                    serverDetail.Show();
                }
                target = VisualTreeHelper.GetParent(target);
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sip = (ServerInfoPreview)dataGrid.SelectedItem;
            string copyText = string.Format("{0}:{1}", sip.Host, sip.Port);
            Clipboard.SetText(copyText);
            MessageBox.Show(string.Format("IP和端口已经复制到剪切板: {0}", copyText));
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
