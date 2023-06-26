using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MetaDataHelper.UserStringClass;

namespace MetaDataHelper
{
    class AddUserStringDefinitionCommand: ICommand
    {

        private UserStringTemplate _currenTemplate;

        public bool CanExecute(object parameter)
        {
            return true;
            //throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            var param = parameter as String;
            //var param = parameter as string;
            var newUserStringDef = new UserStringDefinition("key");
            this._currenTemplate.Add(newUserStringDef);

        }

        public event EventHandler CanExecuteChanged;

        public AddUserStringDefinitionCommand(UserStringTemplate currentTemplate)
        {
            _currenTemplate = currentTemplate;
        }

    }
}
