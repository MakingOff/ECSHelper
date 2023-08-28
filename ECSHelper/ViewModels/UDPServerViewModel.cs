using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using ECSHelper.Models;
using ECSHelper.Utils;
using ECSHelper.ViewModels.Util;
using ECSHelper.Views;

namespace ECSHelper.ViewModels; 

public class UDPServerViewModel : ServerBaseViewModel {
    public RelayCommand StartServerCommand { get; set; }
    public RelayCommand StopServerCommand { get; set; }
    public RelayCommand ChangePayloadCommand { get; set; }
    
    public UdpClient Client { get; set; }

    private bool _SendBackSameData;
    public bool SendBackSameData {
        get => _SendBackSameData;
        set => SetProperty(ref _SendBackSameData, value);
    }
    
    private bool _SendBackCustomData;
    public bool SendBackCustomData {
        get => _SendBackCustomData;
        set => SetProperty(ref _SendBackCustomData, value);
    }
    
    private string _CustomDataToSend;
    public string CustomDataToSend {
        get => _CustomDataToSend;
        set => SetProperty(ref _CustomDataToSend, value);
    }

    public UDPServerViewModel() {
        ServerType = ServerTypes.UDP;
        ServerHost = "127.0.0.1";
        ServerPort = 8090;

        SendBackSameData = true;
        ShowSentDataInLogs = false;

        StartServerCommand = new RelayCommand(StartServer);
        StopServerCommand = new RelayCommand(StopServer);
        ChangePayloadCommand = new RelayCommand(ChangePayload);
    }

    private void StartServer(object param) {
        if (IsServerRunning) {
            return;
        }

        IPEndPoint receiver = new IPEndPoint(IPAddress.Any, ServerPort);
        Client = new UdpClient(receiver);

        Client.BeginReceive(OnData, Client);
        
        IsServerRunning = true;
        Logs.Add(new LogEntry($"Server Started. Listening on {ServerHost}:{ServerPort}"));
        Logs.Add(new LogEntry());
    }

    private void OnData(IAsyncResult res) {
        if (!IsServerRunning) {
            return;
        }
        
        IPEndPoint source = new IPEndPoint(IPAddress.Any, 0);

        byte[] data = Client.EndReceive(res, ref source);
        if (ShowHexAsText) {
            Logs.Add(new LogEntry($">> Data received from {source}: {Encoding.UTF8.GetString(data)}"));
        } else {
            Logs.Add(new LogEntry($">> Data received from {source}: {Convert.ToHexString(data)}"));
        }

        if (SendBackSameData) {
            Client.Send(data, data.Length, source);

            if (ShowSentDataInLogs) {
                if (ShowHexAsText) {
                    Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(data)}"));
                } else {
                    Logs.Add(new LogEntry($"<< Sent: {Convert.ToHexString(data)}"));
                }
            }
        }

        if (SendBackCustomData) {
            Client.Send(Convert.FromHexString(CustomDataToSend), Convert.FromHexString(CustomDataToSend).Length, source);

            if (ShowSentDataInLogs) {
                if (ShowHexAsText) {
                    Logs.Add(new LogEntry($"<< Sent: {Encoding.UTF8.GetString(CustomDataToSend.ToByteArray())}"));
                } else {
                    Logs.Add(new LogEntry($"<< Sent: {CustomDataToSend}"));
                }
            }
        }
        
        Client.BeginReceive(OnData, Client);
    }

    private void StopServer(object param) {
        if (!IsServerRunning) {
            return;
        }
        
        IsServerRunning = false;

        Client.Close();
        Client.Dispose();
        Client = null;
        
        Logs.Add(new LogEntry());
        Logs.Add(new LogEntry("Server Stopped"));
    }

    private void ChangePayload(object param) {
        ConfigurePayloadWindow dialog = new ConfigurePayloadWindow();
        if (dialog.DataContext is ConfigurePayloadViewModel dialogVM) {
            dialogVM.Payload = CustomDataToSend;
            
            if (dialog.ShowDialog() ?? false) {
                if (dialogVM.Payload.HasValue()) {
                    CustomDataToSend = Regex.Replace(dialogVM.Payload, @"\s+", "").ToUpper();
                } else {
                    CustomDataToSend = "";
                }
            }
        }
    }
}