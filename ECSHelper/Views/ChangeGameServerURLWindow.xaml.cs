using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ECSHelper.Views; 

public partial class ChangeGameServerURLWindow : Window {
    public ChangeGameServerURLWindow() {
        InitializeComponent();
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e) {
        DialogResult = true;
    }

    private void IPValidationTextBox(object sender, TextCompositionEventArgs e) {
        e.Handled = new Regex("[^0-9.]+").IsMatch(e.Text);
    }
    
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
        e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
}