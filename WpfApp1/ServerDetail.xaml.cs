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
using System.Windows.Shapes;
using QueryMaster.GameServer;
using QueryMaster;
namespace WpfApp1
{
    /// <summary>
    /// ServerDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ServerDetail : Window
    {
        public ServerDetail(string Host, ushort Port)
        {
            InitializeComponent();
            Server server = ServerQuery.GetServerInstance(EngineType.Source, Host, Port);
            ServerInfo server_info = server.GetInfo();
            var server_players = server.GetPlayers();
            ServerName.Content = server_info.Name;
            Players.Content = string.Format("{0}/{1}", server_info.Players, server_info.MaxPlayers);
            Map.Content = server_info.Map;
            Ping.Content = server_info.Ping;
            dataGrid.DataContext = server_players;
        }
    }
}
