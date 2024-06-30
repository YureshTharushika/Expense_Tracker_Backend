namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        // Foreign key for Account
        public int AccountId { get; set; }
        
        // Foreign key for Budget
        public int? BudgetId { get; set; }
        
    }
}
