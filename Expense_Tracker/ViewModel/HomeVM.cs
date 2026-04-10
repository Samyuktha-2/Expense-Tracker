using Expense_Tracker.Model;
using System.Windows.Input;
using Expense_Tracker.Command;
using System.Collections.ObjectModel;
using System;
using Expense_Tracker.Service;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;

namespace Expense_Tracker.ViewModel
{
    public class HomeVM : BaseVM
    {
        private readonly MainVM mainVM;

        public ICollectionView TransactionsView
        {
            get => transactionsView;
            set
            {
                transactionsView = value;
                OnPropertyChanged(nameof(TransactionsView));
            }
        }
        public string ThemeIcon => ThemeService.CurrentTheme == "Dark" ? "☀" : "🌙";
        private UserDataService service = new UserDataService();

        private string amountError;
        private string amountInput;
        private string dateError;

        private DateTime date = DateTime.Today;

        private string filterText;
        private DateTime? filterFromDate;
        private DateTime? filterToDate;
        private string filterType = "All";
        private string filterCategory = "All";
        private ICollectionView transactionsView;



        private string type;
        private string category;
        private decimal amount;
        private string description;

        public Transaction SelectedTransaction { get; set; }
        public string UserName => Session.CurrentUser.Name;
        public string UserInitial
        {
            get
            {
                if (string.IsNullOrEmpty(Session.CurrentUser?.Name))
                {
                    return "";
                }
                return Session.CurrentUser.Name.Substring(0, 1).ToUpper();
            }
        }

        public ObservableCollection<Transaction> Transactions
        {
            get
            {
                return Session.CurrentUser.Transactions;
            }
        }

        public string AmountError
        {
            get => amountError;
            set
            {
                amountError = value;
                OnPropertyChanged(nameof(AmountError));
            }
        }
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public string Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
                UpdateCategory();
            }
        }

        public string Category
        {
            get => category;
            set
            {
                category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
        public decimal Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
        public string AmountInput
        {
            get => amountInput;
            set
            {
                amountInput = value;
                OnPropertyChanged(nameof(AmountInput));
                ValidateAmount();
            }
        }
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public decimal TotalIncome => Session.CurrentUser.Transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
        public decimal TotalExpense => Session.CurrentUser.Transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        public decimal TotalBalance => TotalIncome - TotalExpense;

        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                OnPropertyChanged(nameof(FilterText));
                TransactionsView.Refresh();
            }
        }
        public DateTime? FilterFromDate
        {
            get => filterFromDate;
            set
            {
                filterFromDate = value;
                OnPropertyChanged(nameof(FilterFromDate));
                ValidateDate();
                TransactionsView.Refresh();
            }
        }
        public DateTime? FilterToDate
        {
            get => filterToDate;
            set
            {
                filterToDate = value;
                OnPropertyChanged(nameof(FilterToDate));
                ValidateDate();
                TransactionsView.Refresh();
            }
        }
        public string FilterType
        {
            get => filterType;
            set
            {
                filterType = value;
                OnPropertyChanged(nameof(FilterType));
                TransactionsView.Refresh();
            }
        }
        public string FilterCategory
        {
            get => filterCategory;
            set
            {
                filterCategory = value;
                OnPropertyChanged(nameof(FilterCategory));
                TransactionsView.Refresh();
            }
        }
        public string DateError
        {
            get => dateError;
            set
            {
                dateError = value;
                OnPropertyChanged(nameof(DateError));
            }
        }
        private bool isReportVisible = false;
        private ReportVM reportVM;

        public bool IsReportVisible
        {
            get => isReportVisible;
            set
            {
                isReportVisible = value;
                OnPropertyChanged(nameof(IsReportVisible));
            }
        }

        public ICommand AddTransactionCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand ThemeCommand { get; }
        public ICommand ReportCommand { get; }
        public ICommand HomePageCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public ReportVM ReportVM
        {
            get => reportVM; set
            {
                reportVM = value;
                OnPropertyChanged(nameof(ReportVM));
            }
        }

        public HomeVM(MainVM vm)
        {
            mainVM = vm;
            AddTransactionCommand = new RelayCommand(AddTransaction);
            TransactionsView = CollectionViewSource.GetDefaultView(Session.CurrentUser.Transactions);
            TransactionsView.Filter = FilterTransactions;
            ClearFilterCommand = new RelayCommand(ClearFilters);
            ThemeCommand = new RelayCommand(ThemeChange);
            ReportCommand = new RelayCommand(Report);
            HomePageCommand = new RelayCommand(HomePage);
            Categories = new ObservableCollection<string>();
            EditCommand = new RelayCommandT(EditTransaction);
            DeleteCommand = new RelayCommandT(DeleteTransaction);
        }

        private void AddTransaction()
        {
            ValidateAmount();

            if (SelectedTransaction != null)
            {
                SelectedTransaction.Date = Date;
                SelectedTransaction.Type = Type;
                SelectedTransaction.Category = Category;
                SelectedTransaction.Amount = Amount;
                SelectedTransaction.Description = Description;

                SelectedTransaction = null;
            }
            else
            {
                var newTransaction = new Transaction
                {
                    Date = Date,
                    Type = Type,
                    Category = Category,
                    Amount = Amount,
                    Description = Description
                };

                Session.CurrentUser.Transactions.Add(newTransaction);

            }
            service.SaveUser(Session.CurrentUser);
            RefreshTotals();
            ClearFields();
        }

        private void ValidateAmount()
        {
            if (string.IsNullOrWhiteSpace(AmountInput))
            {
                AmountError = "Amount is required";
                return;
            }

            if (!decimal.TryParse(AmountInput, out decimal parsedAmount))
            {
                AmountError = "Amount must be numeric";
                return;
            }

            Amount = parsedAmount;

            AmountError = "";
        }

        private void ValidateDate()
        {
            if (FilterFromDate != null && FilterToDate != null && FilterFromDate > FilterToDate)
            {
                DateError = "Date range is incorrect";
            }
            else
            {
                DateError = "";
            }
        }

        private void ClearFields()
        {
            Date = DateTime.Today;
            Type = null;
            Category = null;
            AmountInput = "";
            Description = "";

            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Type));
            OnPropertyChanged(nameof(Category));
            OnPropertyChanged(nameof(AmountInput));
            OnPropertyChanged(nameof(Description));
        }

        private void RefreshTotals()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpense));
            OnPropertyChanged(nameof(TotalBalance));
        }

        private bool FilterTransactions(object obj)
        {
            Transaction transaction = obj as Transaction;

            if (transaction == null)
                return false;

            // Search filter
            if (!string.IsNullOrEmpty(FilterText))
            {
                if (transaction.Description == null ||
                    !transaction.Description.ToLower().Contains(FilterText.ToLower()))
                    return false;
            }

            // From date filter
            if (FilterFromDate != null && transaction.Date < FilterFromDate.Value)
                return false;

            // To date filter
            if (FilterToDate != null && transaction.Date > FilterToDate.Value)
                return false;

            // Type filter
            if (FilterType != "All" && transaction.Type != FilterType)
                return false;

            //Category filter
            if (FilterCategory != "All" && transaction.Category != FilterCategory)
                return false;

            return true;
        }

        private void ClearFilters()
        {
            FilterText = "";
            FilterFromDate = null;
            FilterToDate = null;
            FilterType = "All";
            FilterCategory = "All";

            TransactionsView.Refresh();
        }

        private void ThemeChange()
        {
            ThemeService.ToggleTheme();
            OnPropertyChanged(nameof(ThemeIcon));
        }

        private void Report()
        {
            IsReportVisible = true;
            ReportVM = new ReportVM(Session.CurrentUser.Transactions);
        }

        private void HomePage()
        {
            IsReportVisible = false;
        }

        public ObservableCollection<string> Categories { get; set; }

        public List<string> IncomeCategories { get; } = new List<string>()
        {
            "Salary", "Business", "Investment", "Other Income"
        };

        public List<string> ExpenseCategories { get; } = new List<string>()
        {
            "Food", "Clothes","Health Care", "Entertainment","Transport", "Groceries", "Bills", "Other Expense"
        };

        private void UpdateCategory()
        {
            Categories.Clear();

            var source = Type == "Income" ? IncomeCategories : ExpenseCategories;

            foreach (var s in source)
            {
                Categories.Add(s);
            }
        }

        private void EditTransaction(object obj)
        {
            var transaction = obj as Transaction;

            if (transaction == null) return;

            SelectedTransaction = transaction;

            Date = transaction.Date;
            Type = transaction.Type;
            Category = transaction.Category;
            AmountInput = transaction.Amount.ToString();
            Description = transaction.Description;
        }

        private void DeleteTransaction(object obj)
        {
            var transaction = obj as Transaction;
            if (transaction != null)
            {
                Transactions.Remove(transaction);
            }
            service.SaveUser(Session.CurrentUser);
            RefreshTotals();
        }
    }
}
