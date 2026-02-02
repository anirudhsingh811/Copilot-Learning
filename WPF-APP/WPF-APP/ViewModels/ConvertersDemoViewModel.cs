namespace WPF_APP.ViewModels;

public class ConvertersDemoViewModel : ViewModelBase
{
    private bool _isEnabled = true;
    private string _inputText = "hello world";
    private string _firstName = "John";
    private string _lastName = "Doe";

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public string InputText
    {
        get => _inputText;
        set => SetProperty(ref _inputText, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }
}
