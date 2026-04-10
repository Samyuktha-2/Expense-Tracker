using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expense_Tracker.ViewModel
{
    public class MainVM : BaseVM
    {
        private object currentView;

        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private Stack<object> navigationStack = new Stack<object>();

        public MainVM()
        {
            Navigate(new LoginVM(this));
        }

        public void Navigate(object vm)
        {
            navigationStack.Push(vm);
            CurrentView = vm;
        }
         
        public void GoBack()
        {
            if(navigationStack.Count > 1)
            {
                navigationStack.Pop();
                CurrentView = navigationStack.Peek();
            }
        }
    }
}
