using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ECSHelper.Models;
using ECSHelper.Utils;
using ECSHelper.ViewModels.Util;
using ECSHelper.Views;
using Fleck;

namespace ECSHelper.ViewModels; 

public class MatchmakingServerViewModel : ServerBaseViewModel {
    public RelayCommand StartServerCommand { get; set; }
    public RelayCommand StopServerCommand { get; set; }
    public RelayCommand ChangeGameServerCommand { get; set; }

    public List<IWebSocketConnection> AllSockets { get; set; }
    public WebSocketServer Server { get; set; }

    private string _GameServer_IP;
    public string GameServer_IP {
        get => _GameServer_IP;
        set => SetProperty(ref _GameServer_IP, value);
    }

    private int _GameServer_Port;
    public int GameServer_Port {
        get => _GameServer_Port;
        set => SetProperty(ref _GameServer_Port, value);
    }

    private bool _SendOnce;
    public bool SendOnce {
        get => _SendOnce;
        set => SetProperty(ref _SendOnce, value);
    }

    public MatchmakingServerViewModel() {
        ServerType = ServerTypes.WS;
        ServerHost = "127.0.0.1";
        ServerPort = 8001;
        GameServer_IP = "127.0.0.1";
        GameServer_Port = 8090;

        StartServerCommand = new RelayCommand(StartServer);
        StopServerCommand = new RelayCommand(StopServer);
        ChangeGameServerCommand = new RelayCommand(ChangeGameServer);
    }

    private void StartServer(object param) {
        if (IsServerRunning) {
            return;
        }

        bool alreadySended = false;
        AllSockets = new List<IWebSocketConnection>();
        Server = new WebSocketServer($"ws://{ServerHost}:{ServerPort}");
        
        Server.Start(socket => {
            socket.OnOpen = () => {
                string clientInfo = $"{socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}";
                Logs.Add(new LogEntry($"Socket Opened ({clientInfo})"));
                Logs.Add(new LogEntry());
                AllSockets.Add(socket);
            };
            
            socket.OnClose = () => {
                string clientInfo = $"{socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}";
                Logs.Add(new LogEntry($"Socket Closed ({clientInfo})"));
                Logs.Add(new LogEntry());
                AllSockets.Remove(socket);
            };
            
            socket.OnMessage = message => {
                Logs.Add(new LogEntry($">> Cleartext message received: {message}"));
                
                socket.Send("Ok");
                if (ShowSentDataInLogs) {
                    Logs.Add(new LogEntry("<< Sent: \"Ok\""));
                }
                
                Logs.Add(new LogEntry());
            };
            
            socket.OnBinary = bytes => {
                if (ShowHexAsText) {
                    Logs.Add(new LogEntry($">> Data Received: {Encoding.UTF8.GetString(bytes)}"));
                } else {
                    Logs.Add(new LogEntry($">> Data Received: {Convert.ToHexString(bytes)}"));
                }

                if (SendOnce && alreadySended) {
                    return;
                }
                
                // Getting the hex values for our targeted IP:Port
                int[] gameServerIP_Split = GameServer_IP.Split(".").Select(int.Parse).ToArray();
                string gameServerIP_Hex = gameServerIP_Split[0].ToString("X").PadLeft(2, '0') + gameServerIP_Split[1].ToString("X").PadLeft(2, '0') +
                                          gameServerIP_Split[2].ToString("X").PadLeft(2, '0') + gameServerIP_Split[3].ToString("X").PadLeft(2, '0');
                string gameServerPort_Hex = GameServer_Port.ToString("X").PadLeft(4, '0');
                
                string matchmaker_send1 = "F640BB78A2E78CBBCBBEBFDA33CF288F040000000000000000000000";
                // Default: 127.0.0.1:6794 - 7F0000011A8A
                //          127.0.0.1:8090 - 7F0000011F9A
                string matchmaker_send2 = "F640BB78A2E78CBBF3EBBF19875FBFFA700100000000000000000400E80300000ACBCD4D" + gameServerIP_Hex + gameServerPort_Hex + "00000AF74E430346EC7A1A8900000AF7450C034BBF9F1A8D00000AF75E6C365D374B1A8A00000AF55ED80DD6C1FF1A8A00000A4A8E19030987F11A8D00000A4A855D0309B1791A8B00000A4A97CB23B08B511A8800000A4A8B841285E4C21A8A00000AF2C27C365F26C21A8D00000A2BDC6F0D38E6EA1A8900000A2BD54F036556431A8800000A2BCC5736B7DE2E1A8C00000A2BD8E60365BE031A8A00000A01697F36A6E4681A8B00000A0147832CCC071B1A8800000A01561212D7A22C1A8B00000A014EA12CCA43FC1A8A00000AA040F70FB5C7331A8D00000AA040F60FB5C7831A8900000AA049E20FB5C7D41A8800000AA046B00FB5C9551A8800000A64CD650FB5B2111A8800000A64C0DC0FB5B1CF1A8A00000A64C80A0FB5B1641A8B00000A64C1AA0FB5B5AF1A8800000A9404EC482900331A8B00000A940657482900AA1A8C00000A94064B482900871A8D00000A94032D482900C11A8A0000";
                // Original length = 2482
                string matchmaker_send3 = "F640BB78A2E78CBB6C6C16F2C4D35A8DC10400000000000000000400E80300007B22726567696F6E735B305D7C726567696F6E6964223A22307838453841384141433831453737314237222C22726567696F6E735B305D7C656E64706F696E74223A22334DC02D22726567696F6E735B315D7C726567696F6E6964223A22307838453841384141433831464137314234222C22726567696F6E735B315D7C656E64706F696E74223A22" + "3132372E302E302E31" + "222C22726567696F6E735B325D7C726567696F6E6964223A22307838453841384141433831464137314237222C22726567696F6E735B325D7C656E64706F696E74223A2267616D656C6966742E61702D736F757468656173742D312E616D617A6F6E6177732E636F6D222C22726567696F6E735B335D7C726567696F6E6964223A22307836464239413131353237464141304631222C22726567696F6E735B335D7C656E64706F696E74223A2267616D656C6966742E75732D656173742D312E616D617A6F6E6177732E636F6D222C22726567696F6E735B345D7C726567696F6E6964223A22307836464239413131353237464141304632222C22726567696F6E735B345D7C656E64706F696E74223A2267616D656C6966742E75732D656173742D322E616D617A6F6E6177732E636F6D222C22726567696F6E735B355D7C726567696F6E6964223A22307836464239413131353237464141364631222C22726567696F6E735B355D7C656E64706F696E74223A227261642D6368696361676F2D656E64706F696E742D6C622D313639313736343934332E75732D656173742D312E656C622E616D617A6F6E6177732E636F6D222C22726567696F6E735B365D7C726567696F6E6964223A22307836464239413131353237464141364632222C22726567696F6E735B365D7C656E64706F696E74223A227261642D64616C6C61732D656E64706F696E742D6C622D313639313736343934332E75732D656173742D312E656C622E616D617A6F6E6177732E63AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA6F6D222C22726567696F6E735B375D7C726567696F6E6964223A22307836464239413131353237464141364633222C22726567696F6E735B375D7C656E64706F696E74223A227261642D686F7573746F6E2D656E64706F696E742D6C622D3132333435363738392E75732D656173742D312E656C622E616D617A6F6E6177732E636F6D222C22726567696F6E735B385D7C726567696F6E6964223A22307836464239413131353237464142324631222C22726567696F6E735B385D7C656E64706F696E74223A2267616D656C6966742E75732D776573742D312E616D617A6F6E6177732E636F6D222C22726567696F6E735B395D7C726567696F6E6964223A22307836464239413131353337464341364631222C22726567696F6E735B395D7C656E64706F696E74223A2267616D656C6966742E65752D63656E7472616C2D312E616D617A6F6E6177732E636F6D222C22726567696F6E735B31305D7C726567696F6E6964223A22307836464239413131353337464342324632222C22726567696F6E735B31305D7C656E64706F696E74223A2267616D656C6966742E65752D776573742D322E616D617A6F6E6177732E636F6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D6D227D";
                
                socket.Send(Convert.FromHexString(matchmaker_send1));
                if (ShowSentDataInLogs) {
                    if (ShowHexAsText) {
                        Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(matchmaker_send1.ToByteArray())}"));
                    } else {
                        Logs.Add(new LogEntry($"<< Sent: {matchmaker_send1}"));
                    }
                }

                socket.Send(Convert.FromHexString(matchmaker_send2));
                if (ShowSentDataInLogs) {
                    if (ShowHexAsText) {
                        Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(matchmaker_send2.ToByteArray())}"));
                    } else {
                        Logs.Add(new LogEntry($"<< Sent: {matchmaker_send2}"));
                    }
                }

                socket.Send(Convert.FromHexString(matchmaker_send3));
                if (ShowSentDataInLogs) {
                    if (ShowHexAsText) {
                        Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(matchmaker_send3.ToByteArray())}"));
                    } else {
                        Logs.Add(new LogEntry($"<< Sent: {matchmaker_send3}"));
                    }
                }

                Logs.Add(new LogEntry());
                alreadySended = true;
            };
            
            socket.OnError = ex => {
                Logs.Add(new LogEntry($">> Exception: {ex.Message}"));
                
                socket.Send("Ok");
                if (ShowSentDataInLogs) {
                    Logs.Add(new LogEntry("<< Sent: \"Ok\""));
                }

                Logs.Add(new LogEntry());
            };
        });

        IsServerRunning = true;
        Logs.Add(new LogEntry($"Server Started. Listening on ws://{ServerHost}:{ServerPort}"));
        Logs.Add(new LogEntry());
    }

    private void StopServer(object param) {
        if (!IsServerRunning) {
            return;
        }

        foreach (IWebSocketConnection socket in AllSockets) {
            socket.Close();
        }
        AllSockets.Clear();
        Server.Dispose();
        Server = null;

        IsServerRunning = false;
        Logs.Add(new LogEntry("Server Stopped"));
    }

    private void ChangeGameServer(object param) {
        ChangeGameServerURLWindow dialog = new ChangeGameServerURLWindow();
        if (dialog.DataContext is ChangeGameServerURLViewModel dialogVM) {
            dialogVM.Host = GameServer_IP;
            dialogVM.Port = GameServer_Port;
            
            if (dialog.ShowDialog() ?? false) {
                if (IPAddress.TryParse(dialogVM.Host, out _)) {
                    GameServer_IP = dialogVM.Host;
                }
                
                GameServer_Port = Math.Clamp(dialogVM.Port, 1025, 65535);
            }
        }
    }
}