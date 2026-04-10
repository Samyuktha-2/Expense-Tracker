using Expense_Tracker.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Expense_Tracker.ViewModel
{
    public class ReportVM : BaseVM
    {
        private string selectedMonthName;
        private ObservableCollection<Transaction> monthlyTransactions;
        private decimal totalMonthIncome;
        private decimal totalMonthExpense;
        private ObservableCollection<Transaction> _transactions;
        private SeriesCollection _expenseSeries;
        private SeriesCollection _incomeSeries;
        private int selectedYear;

        public List<string> Months { get; } = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public string SelectedMonthName
        {
            get => selectedMonthName;
            set
            {
                selectedMonthName = value;
                OnPropertyChanged(nameof(SelectedMonthName));
                LoadMonthlyData();
            }
        }

        public int SelectedYear
        {
            get => selectedYear;
            set
            {
                selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                LoadMonthlyData();
            }
        }
        public ObservableCollection<Transaction> MonthlyTransactions
        {
            get => monthlyTransactions;
            set
            {
                monthlyTransactions = value;
                OnPropertyChanged(nameof(MonthlyTransactions));
            }
        }
        public decimal TotalMonthIncome
        {
            get => totalMonthIncome;
            set
            {
                totalMonthIncome = value;
                OnPropertyChanged(nameof(TotalMonthIncome));
            }
        }
        public decimal TotalMonthExpense
        {
            get => totalMonthExpense;
            set
            {
                totalMonthExpense = value;
                OnPropertyChanged(nameof(TotalMonthExpense));
            }
        }

        public ReportVM(ObservableCollection<Transaction> transactions)
        {
            _transactions = transactions;
            
            SelectedMonthName = DateTime.Now.ToString("MMMM");
            SelectedYear = DateTime.Now.Year;

            _transactions.CollectionChanged += (s, e) =>
            {
                LoadMonthlyData();
            };
        } 

        // Charts
        public SeriesCollection ExpenseSeries
        {
            get { return _expenseSeries; }
            set
            {
                _expenseSeries = value;
                OnPropertyChanged(nameof(ExpenseSeries));
            }
        }
        public SeriesCollection IncomeSeries
        {
            get { return _incomeSeries; }
            set
            {
                _incomeSeries = value;
                OnPropertyChanged(nameof(IncomeSeries));
            }
        }

        private void LoadMonthlyData()
        {
            int monthNumber = DateTime.ParseExact(
                SelectedMonthName,
                "MMMM",
                System.Globalization.CultureInfo.InvariantCulture).Month;

            var data = _transactions
                .Where(x => x.Date.Month == monthNumber &&
                            x.Date.Year == SelectedYear)
                .ToList();

            MonthlyTransactions = new ObservableCollection<Transaction>(data);

            TotalMonthExpense = data
                .Where(x => x.Type == "Expense")
                .Sum(x => x.Amount);

            TotalMonthIncome = data
                .Where(x => x.Type == "Income")
                .Sum(x => x.Amount);

            UpdateCharts(data);
        }

        // Chart Logic
        private void UpdateCharts(List<Transaction> data)
        {
            ExpenseSeries = new SeriesCollection();
            IncomeSeries = new SeriesCollection();

            var expenseCategory = data.Where(x => x.Type == "Expense").GroupBy(x => x.Category).Select(g => new
            {
                Category = g.Key,
                Amount = g.Sum(x => x.Amount)
            });

            foreach (var item in expenseCategory)
            {
                ExpenseSeries.Add(new PieSeries
                {
                    Title = item.Category,
                    Values = new ChartValues<double> { (double)item.Amount },
                    DataLabels = true
                });
            }

            IncomeSeries.Add(new ColumnSeries
            {
                Title = "Income",
                Values = new ChartValues<double> { (double)TotalMonthIncome }
            });

            IncomeSeries.Add(new ColumnSeries
            {
                Title = "Expense",
                Values = new ChartValues<double> { (double)TotalMonthExpense }
            });

            OnPropertyChanged(nameof(ExpenseSeries));
            OnPropertyChanged(nameof(IncomeSeries));
        }
    }
}
