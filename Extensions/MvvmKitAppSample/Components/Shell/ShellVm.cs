using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.Shell
{
    public class ShellVm : ComponentBase
    {
        #region Properties

        private string _Title;
        public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }

        #endregion

        #region Commands

        #endregion

        public ShellVm()
        {
            Title = "Hello MvvmKit Runtime";
        }

        [InjectionMethod]
        public void Inject()
        {

        }
    }
}
