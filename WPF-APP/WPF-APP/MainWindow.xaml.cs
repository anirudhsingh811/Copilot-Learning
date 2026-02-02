using System.Windows;
using System.Windows.Controls;
using WPF_APP.Views;
using WPF_APP.ViewModels;

namespace WPF_APP;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentArea.Content = new ConvertersDemoView { DataContext = new ConvertersDemoViewModel() };
    }

    private void Converters_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new ConvertersDemoView { DataContext = new ConvertersDemoViewModel() };
    }

    private void Styles_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new StylesDemoView();
    }

    private void Resources_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new ResourcesDemoView();
    }

    private void Async_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new AsyncDemoView { DataContext = new AsyncDemoViewModel() };
    }
}
