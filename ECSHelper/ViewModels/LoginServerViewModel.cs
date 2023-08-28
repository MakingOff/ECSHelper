using System;
using System.Collections.Generic;
using System.Text;
using ECSHelper.Models;
using ECSHelper.Utils;
using ECSHelper.ViewModels.Util;
using Fleck;

namespace ECSHelper.ViewModels; 

public class LoginServerViewModel : ServerBaseViewModel {
    public RelayCommand StartServerCommand { get; set; }
    public RelayCommand StopServerCommand { get; set; }
    
    public List<IWebSocketConnection> AllSockets { get; set; }
    public WebSocketServer Server { get; set; }

    public LoginServerViewModel() {
        ServerType = ServerTypes.WS;
        ServerHost = "127.0.0.1";
        ServerPort = 8000;

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
                
                socket.Send("Ok");
                if (ShowSentDataInLogs) {
                    Logs.Add(new LogEntry("<< Sent: \"Ok\""));
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