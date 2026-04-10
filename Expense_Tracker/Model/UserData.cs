using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expense_Tracker.Model
{
    public class UserData
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();
    }
}
