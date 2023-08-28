using ECSHelper.ViewModels.Util;

namespace ECSHelper.ViewModels; 

public class ChangeGameServerURLViewModel : BaseViewModel {
    private string _Host;
    public string Host {
        get => _Host;
        set => SetProperty(ref _Host, value);
    }
    
    private int _Port;
    public int Port {
        get => _Port;
        set => SetProperty(ref _Port, value);
    }
}