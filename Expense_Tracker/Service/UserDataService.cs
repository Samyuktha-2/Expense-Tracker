using Expense_Tracker.Model;
using Newtonsoft.Json;
using System.IO;

namespace Expense_Tracker.Service
{
    public class UserDataService
    {
        private string folder = "UserDatas";

        public UserDataService()
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public UserData LoadUser(string email)
        {
            string safeEmail = email.Replace("@", "_").Replace(".", "_");
            string path = Path.Combine(folder, $"{safeEmail}.json");

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<UserData>(json);
        }

        public void SaveUser(UserData user)
        {
            string safeEmail = user.Email.Replace("@", "_").Replace(".", "_");
            string path = Path.Combine(folder, $"{safeEmail}.json");

            string json = JsonConvert.SerializeObject(user, Formatting.Indented);

            File.WriteAllText(path, json);
        }
    }
}