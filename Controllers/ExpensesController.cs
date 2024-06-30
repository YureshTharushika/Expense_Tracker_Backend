using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly Context _context;

        public ExpensesController(Context context)
        {
            _context = context;

        }

        [HttpGet]
        public ActionResult<List<Expense>> GetAll() => _context.Expenses.ToList();

        [HttpGet("{id}")]
        public ActionResult<Expense> GetExpenseById(int id)
        {
            var expense = _context.Expenses.Find(id);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(Expense expense)
        {
            // Retrieve the related account
            var account = await _context.Accounts.FindAsync(expense.AccountId);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            if (expense.BudgetId.HasValue && !_context.Budgets.Any(b => b.Id == expense.BudgetId))
            {
                return BadRequest("Invalid BudgetId");
            }

            // Deduct the expense amount from the account balance
            account.Balance -= expense.Amount;

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // Save changes to the account
            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Expense expense)
        {
            var existingExpense = _context.Expenses.Find(id);

            if (existingExpense == null)
            {
                return NotFound();
            }

            existingExpense.Description = expense.Description;
            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.AccountId = expense.AccountId;
            existingExpense.BudgetId = expense.BudgetId;

            _context.Expenses.Update(existingExpense);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var expense = _context.Expenses.Find(id);

            if (expense == null)
            {
                return NotFound();
            }

            // Retrieve the related account
            var account = _context.Accounts.Find(expense.AccountId);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            // Add the expense amount back to the account balance
            account.Balance += expense.Amount;

            _context.Expenses.Remove(expense);
            _context.SaveChanges();

            // Save changes to the account
            _context.Entry(account).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("byAccount/{accountId}")]
        public ActionResult<IEnumerable<Expense>> GetExpensesByAccount(int accountId)
        {
            var expenses = _context.Expenses.Where(e => e.AccountId == accountId).ToList();
            if (expenses == null)
            {
                return NotFound();
            }
            return expenses;
        }

        [HttpGet("byBudget/{budgetId}")]
        public ActionResult<IEnumerable<Expense>> GetExpensesByBudget(int budgetId)
        {
            var expenses = _context.Expenses.Where(e => e.BudgetId == budgetId).ToList();
            if (expenses == null)
            {
                return NotFound();
            }
            return expenses;
        }


    }
}
