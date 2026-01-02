namespace BudgetManager.Models;

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _title = "";
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Titel får inte vara tom.");
            _title = value;
        }
    }

    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
