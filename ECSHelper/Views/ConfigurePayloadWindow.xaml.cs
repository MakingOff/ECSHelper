using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ECSHelper.Views; 

public partial class ConfigurePayloadWindow : Window {
    public ConfigurePayloadWindow() {
        InitializeComponent();
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e) {
        DialogResult = true;
    }

    private void HexValidationTextBox(object sender, TextCompositionEventArgs e) {
        e.Handled = new Regex("[^0-9a-fA-F]+").IsMatch(e.Text);
    }
}