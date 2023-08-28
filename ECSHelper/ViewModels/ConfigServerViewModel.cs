using System;
using System.Collections.Generic;
using System.Text;
using ECSHelper.Models;
using ECSHelper.Utils;
using ECSHelper.ViewModels.Util;
using Fleck;

namespace ECSHelper.ViewModels; 

public class ConfigServerViewModel : ServerBaseViewModel {
    private const string SEND1 = "F640BB78A2E78CBB12D07B6F58AFCDB948010000000000007B1D0E4427EE09157B1D0E4427EE09155902000028B52FFD60590155090046923922306DF3A0AC673B5B96A1709345BA1A46672853C54B4C034B2644DF0A1C2C0C0CD60130002E003000292967FEDFAA85348C8B62ACAD96697D08FE8DA8EDE98CFDF1981B51EB0DF7220583A1501A038D05431158A8F55F1B0991F145CFB309F33504827DE7677179C3BD6ACB05EB7EB384DFEC8B9E5D6674249C255B12F3354E72B64456E858060F6B1B32B96A1D5414C5EAE9ECE029182C30042427260F1D29291ADBEA66BAAFCF175B94DF27B39A99365FAB363F6701000F33CE2F5A87960D05D249CB92383A9B43B768BD269A537B1067D71C9174C3DC33C731A68B89FC1635D4F6844E39180070E684CECC4A45C73003BCCB00AD086CEF9C927B052CDBEDDA486C719DD63A98D90EC4F00E0D4C006F1E6D58ED413AA64862B46065CC035B70C130CC1CC10C";
    private const string SEND1_5 = "F640BB78A2E78CBBE4EE6BC73A96E643010000000000000010";
    
    public RelayCommand StartServerCommand { get; set; }
    public RelayCommand StopServerCommand { get; set; }
    
    public List<IWebSocketConnection> AllSockets { get; set; }
    public WebSocketServer Server { get; set; }

    public ConfigServerViewModel() {
        ServerType = ServerTypes.WS;
        ServerHost = "127.0.0.1";
        ServerPort = 8003;

        StartServerCommand = new RelayCommand(StartServer);
        StopServerCommand = new RelayCommand(StopServer);
    }

    private void StartServer(object param) {
        if (IsServerRunning) {
            return;
        }
        
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
                
                socket.Send(Convert.FromHexString(SEND1));
                if (ShowSentDataInLogs) {
                    if (ShowHexAsText) {
                        Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(SEND1.ToByteArray())}"));
                    } else {
                        Logs.Add(new LogEntry($"<< Sent: {SEND1}"));
                    }
                }
                
                socket.Send(Convert.FromHexString(SEND1_5));
                if (ShowSentDataInLogs) {
                    if (ShowHexAsText) {
                        Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(SEND1_5.ToByteArray())}"));
                    } else {
                        Logs.Add(new LogEntry($"<< Sent: {SEND1_5}"));
                    }
                }
                
                Logs.Add(new LogEntry());
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
}