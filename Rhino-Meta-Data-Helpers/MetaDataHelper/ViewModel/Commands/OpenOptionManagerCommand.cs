using System;
using System.Windows.Input;
using MetaDataHelper.UserStringClass;

namespace MetaDataHelper
{
    internal class OpenOptionManagerCommand : ICommand
    {
        private UserStringTemplate _currentTemplate;

        public event EventHandler CanExecuteChanged
        {
            // Hook up CommandManager's RequerySuggested event for auto-refresh
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public OpenOptionManagerCommand(UserStringTemplate currentTemplate)
        {
            _currentTemplate = currentTemplate;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is UserStringDefinition definition &&
                   definition.ValueType == UserStringValueType.Select;
        }

        public void Execute(object parameter)
        {
            if (parameter is UserStringDefinition definition &&
                definition.ValueType == UserStringValueType.Select)
            {
                var dialog = new OptionManagerDialog(definition);
                dialog.ShowDialog();
            }
        }
    }
}