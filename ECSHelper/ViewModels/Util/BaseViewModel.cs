using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ECSHelper.ViewModels.Util; 

public abstract class BaseViewModel : INotifyPropertyChanged {
    #region PropertyChanged
    
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "") {
        if (EqualityComparer<T>.Default.Equals(storage, value)) {
            return;
        }
        
        storage = value;
        OnPropertyChanged(propertyName);
    }
    
    #endregion
}