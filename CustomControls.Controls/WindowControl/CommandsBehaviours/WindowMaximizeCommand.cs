using System;
using System.Windows;
using System.Windows.Input;

namespace CustomControls.Controls.WindowControl.CommandsBehaviours
{
    public class WindowMaximizeCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter is Window window)
                window.WindowState = window.WindowState == WindowState.Maximized ? 
                    WindowState.Normal : WindowState.Maximized;
        }
    }
}