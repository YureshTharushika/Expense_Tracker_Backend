namespace ExpenseTracker.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal AllocatedAmount { get; set; }
        public int AccountId { get; set; }

        // Navigation property to Expenses
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
