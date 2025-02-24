using System;
using System.Windows.Input;

public class RelayCommand : ICommand
{
	private readonly Action<object> _execute;
	private readonly Func<object, bool> _canExecute;
	private ICommand? startSave;

	public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	public RelayCommand(ICommand? startSave, object canAddBuisnessSoftware)
	{
		this.startSave = startSave;
	}

	public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

	public void Execute(object parameter) => _execute(parameter);

	public event EventHandler CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}
}