using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_APP.Views;

public partial class ResourcesDemoView : UserControl
{
    private bool _isGreen = true;

    public ResourcesDemoView()
    {
        InitializeComponent();
    }

    private void ChangeDynamicResource_Click(object sender, RoutedEventArgs e)
    {
        _isGreen = !_isGreen;
        var newColor = _isGreen ? Colors.Green : Colors.Red;
        Resources["DynamicBrush"] = new SolidColorBrush(newColor);
    }
}
