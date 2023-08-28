using ECSHelper.ViewModels.Util;

namespace ECSHelper.ViewModels; 

public class ChangePortViewModel : BaseViewModel {
    private int _Port;
    public int Port {
        get => _Port;
        set => SetProperty(ref _Port, value);
    }

    private bool _IsWS;
    public bool IsWS {
        get => _IsWS;
        set => SetProperty(ref _IsWS, value);
    }
}