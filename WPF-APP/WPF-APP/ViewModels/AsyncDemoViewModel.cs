using System.Windows.Input;
using WPF_APP.Commands;

namespace WPF_APP.ViewModels;

/// <summary>
/// Demonstrates async operations in WPF
/// Key Concepts: async/await, Task-based operations, UI thread safety
/// </summary>
public class AsyncDemoViewModel : ViewModelBase
{
    private string _status = "Ready";
    private int _progress = 0;
    private bool _isProcessing = false;

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public int Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }

    public ICommand StartAsyncCommand { get; }
    public ICommand StartLongRunningCommand { get; }
    public ICommand CancelCommand { get; }

    private CancellationTokenSource? _cancellationTokenSource;

    public AsyncDemoViewModel()
    {
        StartAsyncCommand = new RelayCommand(async () => await ExecuteAsync(), () => !IsProcessing);
        StartLongRunningCommand = new RelayCommand(async () => await ExecuteLongRunning(), () => !IsProcessing);
        CancelCommand = new RelayCommand(Cancel, () => IsProcessing);
    }

    private async Task ExecuteAsync()
    {
        IsProcessing = true;
        Status = "Processing...";
        Progress = 0;

        try
        {
            for (int i = 0; i <= 100; i += 10)
            {
                await Task.Delay(300);
                Progress = i;
                Status = $"Processing... {i}%";
            }

            Status = "Completed successfully!";
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task ExecuteLongRunning()
    {
        IsProcessing = true;
        _cancellationTokenSource = new CancellationTokenSource();
        Status = "Long running task started...";
        Progress = 0;

        try
        {
            for (int i = 0; i <= 100; i++)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                
                await Task.Delay(100, _cancellationTokenSource.Token);
                Progress = i;
                Status = $"Processing... {i}%";
            }

            Status = "Long running task completed!";
        }
        catch (OperationCanceledException)
        {
            Status = "Operation cancelled by user";
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        Status = "Cancelling...";
    }
}
