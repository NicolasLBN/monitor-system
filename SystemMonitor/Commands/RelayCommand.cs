using System.Windows.Input;

namespace SystemMonitor.Commands;

/// <summary>
/// A command whose sole purpose is to relay its functionality to other objects by invoking delegates
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Creates a new command that can always execute
    /// </summary>
    /// <param name="execute">The execution logic</param>
    public RelayCommand(Action<object?> execute) : this(execute, null)
    {
    }

    /// <summary>
    /// Creates a new command
    /// </summary>
    /// <param name="execute">The execution logic</param>
    /// <param name="canExecute">The execution status logic</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state
    /// </summary>
    /// <param name="parameter">Data used by the command</param>
    /// <returns>true if this command can be executed; otherwise, false</returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="parameter">Data used by the command</param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}

/// <summary>
/// A generic command whose sole purpose is to relay its functionality to other objects by invoking delegates
/// </summary>
/// <typeparam name="T">The type of the command parameter</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Creates a new command that can always execute
    /// </summary>
    /// <param name="execute">The execution logic</param>
    public RelayCommand(Action<T?> execute) : this(execute, null)
    {
    }

    /// <summary>
    /// Creates a new command
    /// </summary>
    /// <param name="execute">The execution logic</param>
    /// <param name="canExecute">The execution status logic</param>
    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state
    /// </summary>
    /// <param name="parameter">Data used by the command</param>
    /// <returns>true if this command can be executed; otherwise, false</returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute((T?)parameter);
    }

    /// <summary>
    /// Executes the command
    /// </summary>
    /// <param name="parameter">Data used by the command</param>
    public void Execute(object? parameter)
    {
        _execute((T?)parameter);
    }
}
