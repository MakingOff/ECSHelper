using ECSHelper.ViewModels.Util;

namespace ECSHelper.ViewModels; 

public class ConfigurePayloadViewModel : BaseViewModel {
    private string _Payload;
    public string Payload {
        get => _Payload;
        set => SetProperty(ref _Payload, value);
    }
}