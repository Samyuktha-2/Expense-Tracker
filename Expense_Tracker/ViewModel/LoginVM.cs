using Expense_Tracker.Command;
using Expense_Tracker.Model;
using Expense_Tracker.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Expense_Tracker.ViewModel
{
    public class LoginVM : BaseVM
    {
        private readonly MainVM mainVM;

        UserDataService service = new UserDataService(); 

        public string Email { get; set; }
        public string Password { get; set; } 

        public ICommand LoginCommand { get; }
        public ICommand SignupCommand { get; } 
          
        public LoginVM(MainVM vm)
        {
            mainVM = vm;
            LoginCommand = new RelayCommand(Login);
            SignupCommand = new RelayCommand(Signup); 
        } 

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Enter email and password");
                return;
            }

            var userData = service.LoadUser(Email);

            if (userData != null && userData.Password == Password)
            {
                Session.CurrentUser = userData;
                mainVM.Navigate(new HomeVM(mainVM));
            }
            else
            {
                MessageBox.Show("Invalid email or password");
            }
        }

        private void Signup()
        {
            mainVM.Navigate(new SignUpVM(mainVM));
        }
         
    }
}
