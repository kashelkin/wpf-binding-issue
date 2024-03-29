﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfBinding.MVVM
{
    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<object, CancellationToken, Task<TResult>> _command;
        private readonly Func<bool> _canExecute;
        private readonly CancelAsyncCommand _cancelCommand;
        private NotifyTaskCompletion<TResult> _execution;
        private readonly bool _allowMultipleExecution;

        public AsyncCommand(Func<object, CancellationToken, Task<TResult>> command, bool allowMultipleExecution = false, Func<bool> canExecute = null)
        {
            _command = command;
            _cancelCommand = new CancelAsyncCommand();
            _allowMultipleExecution = allowMultipleExecution;
            _canExecute = canExecute ?? (() => true);
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute() && (_allowMultipleExecution || Execution == null || Execution.IsCompleted);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            _cancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<TResult>(_command(parameter, _cancelCommand.Token));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            _cancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public NotifyTaskCompletion<TResult> Execution
        {
            get { return _execution; }
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class CancelAsyncCommand : ICommand
        {
            private CancellationTokenSource _cts = new CancellationTokenSource();
            private bool _commandExecuting;

            public CancellationToken Token { get { return _cts.Token; } }

            public void NotifyCommandStarting()
            {
                _commandExecuting = true;
                if (!_cts.IsCancellationRequested)
                    return;
                _cts = new CancellationTokenSource();
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                _commandExecuting = false;
                RaiseCanExecuteChanged();
            }

            bool ICommand.CanExecute(object parameter)
            {
                return _commandExecuting && !_cts.IsCancellationRequested;
            }

            void ICommand.Execute(object parameter)
            {
                _cts.Cancel();
                RaiseCanExecuteChanged();
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            private void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public static class AsyncCommand
    {
        public static AsyncCommand<object> Create(Func<Task> command)
        {
            return new AsyncCommand<object>(async (p, token) => { await command(); return null; });
        }

        public static AsyncCommand<object> Create(Func<Task> command, Func<bool> canExecute)
        {
            return new AsyncCommand<object>(async (p, token) => { await command(); return null; }, canExecute: canExecute);
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
        {
            return new AsyncCommand<TResult>((p,token) => command());
        }

        public static AsyncCommand<object> Create(Func<CancellationToken, Task> command)
        {
            return new AsyncCommand<object>(async (p, token) => { await command(token); return null; });
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command)
        {
            return new AsyncCommand<TResult>((p, token) => command(token));
        }

        public static AsyncCommand<object> Create(Func<object, Task> command, bool allowMultipleExecution = false)
        {
            return new AsyncCommand<object>(async (p, token) => { await command(p); return null; }, allowMultipleExecution);
        }
    }
}
