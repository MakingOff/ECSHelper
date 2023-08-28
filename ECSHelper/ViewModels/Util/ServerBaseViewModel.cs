using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ECSHelper.Models;
using ECSHelper.Utils;
using ECSHelper.Views;

namespace ECSHelper.ViewModels.Util; 

public abstract class ServerBaseViewModel : BaseViewModel {
    public RelayCommand ChangePortCommand { get; set; }
    public RelayCommand ClearLogsCommand { get; set; }
    public RelayCommand CopyLogsCommand { get; set; }
    
    private string _ServerHost;
    public string ServerHost {
        get => _ServerHost;
        set => SetProperty(ref _ServerHost, value);
    }

    private int _ServerPort;
    public int ServerPort {
        get => _ServerPort;
        set => SetProperty(ref _ServerPort, value);
    }
    
    private bool _IsServerRunning;
    public bool IsServerRunning {
        get => _IsServerRunning;
        set => SetProperty(ref _IsServerRunning, value);
    }
    
    private bool _ShowSentDataInLogs;
    public bool ShowSentDataInLogs {
        get => _ShowSentDataInLogs;
        set => SetProperty(ref _ShowSentDataInLogs, value);
    }
    
    private bool _ShowHexAsText;
    public bool ShowHexAsText {
        get => _ShowHexAsText;
        set => SetProperty(ref _ShowHexAsText, value);
    }

    public ServerTypes ServerType { get; set; }
    public static object Lock = new(); // Used to keep the Dispatcher reference for the lists we update inside other threads
    public ObservableCollection<LogEntry> Logs { get; set; }

    public ServerBaseViewModel() {
        ShowSentDataInLogs = true;
        
        Logs = new ObservableCollection<LogEntry>();
        BindingOperations.EnableCollectionSynchronization(Logs, Lock);
        
        ChangePortCommand = new RelayCommand(ChangePort);
        ClearLogsCommand = new RelayCommand(ClearLogs);
        CopyLogsCommand = new RelayCommand(CopyLogs);
    }
    
    private void ChangePort(object param) {
        ChangePortWindow dialog = new ChangePortWindow();
        if (dialog.DataContext is ChangePortViewModel dialogVM) {
            dialogVM.IsWS = ServerType == ServerTypes.WS;
            dialogVM.Port = ServerPort;
            
            if (dialog.ShowDialog() ?? false) {
                ServerPort = Math.Clamp(dialogVM.Port, 1025, 65535);
            }
        }
    }

    private void ClearLogs(object param) {
        Logs.Clear();
    }
    
    private void CopyLogs(object param) {
        Clipboard.SetText(string.Join(Environment.NewLine, Logs.Select(l => l.Display)));
    }
}