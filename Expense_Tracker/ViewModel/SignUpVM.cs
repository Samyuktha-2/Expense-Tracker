using Expense_Tracker.Command;
using Expense_Tracker.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace Expense_Tracker.ViewModel
{
    public class SignUpVM : BaseVM
    {
        private MainVM mainVM;
        private string errorMessage;

        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand SignupCommand { get; }

        public SignUpVM(MainVM vm)
        {
            mainVM = vm;
            SignupCommand = new RelayCommandT(Signup);
        }

        private void Signup(object parameter)
        {

            var panel = parameter as StackPanel;

            var passwordBox = panel.FindName("PasswordBox") as PasswordBox;
            var confirmPasswordBox = panel.FindName("ConfirmPasswordBox") as PasswordBox;

            Password = passwordBox.Password;
            ConfirmPassword = confirmPasswordBox.Password;
            if(Password == ConfirmPassword)
            {
                ErrorMessage = "";
                SaveUserAccount();
                CreateUserFile(Email);
                mainVM.Navigate(new LoginVM(mainVM));
            }
            else
            {
                ErrorMessage = "Passwords must match";
                return;
            }
            
        }

        public void SaveUserAccount()
        {
            string file = "users.json";

            List<User> users;

            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);
                users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            else
            {
                users = new List<User>();
            }

            users.Add(new User
            {
                Email = Email,
                Name = Name,
                Password = Password
            });

            string updatedJson = JsonConvert.SerializeObject(users, Formatting.Indented);

            File.WriteAllText(file, updatedJson);
        }


        public void CreateUserFile(string email)
        {
            string folder = "UserDatas";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string safeEmail = email.Replace("@", "_").Replace(".", "_");
            string filePath = Path.Combine(folder, $"{safeEmail}.json");

            if (!File.Exists(filePath))
            {
                var user = new UserData
                {
                    Email = email,
                    Name = Name,
                    Password = Password
                };

                string json = JsonConvert.SerializeObject(user, Formatting.Indented);

                File.WriteAllText(filePath, json);
            }
        }
    }
}
