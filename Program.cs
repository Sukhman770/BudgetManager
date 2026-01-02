using Spectre.Console;
using BudgetManager.Models;
using BudgetManager.Services;

var store = new DataStore<Expense>("Data/expenses.json");

bool running = true;

while (running)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Budget Manager[/]")
            .AddChoices(
                "Lägg till utgift",
                "Visa alla utgifter",
                "Uppdatera utgift",
                "Ta bort utgift",
                "Avsluta"));

    switch (choice)
    {
        case "Lägg till utgift":
            AddExpense();
            break;

        case "Visa alla utgifter":
            ShowExpenses();
            break;

        case "Uppdatera utgift":
            UpdateExpense();
            break;

        case "Ta bort utgift":
            DeleteExpense();
            break;

        case "Avsluta":
            running = false;
            break;
    }
}

void AddExpense()
{
    try
    {
        var title = AnsiConsole.Ask<string>("Titel:");
        var category = AnsiConsole.Ask<string>("Kategori:");

        decimal amount;
        while (!decimal.TryParse(
            AnsiConsole.Ask<string>("Belopp:"), out amount))
        {
            AnsiConsole.MarkupLine("[red]Skriv ett giltigt nummer.[/]");
        }

        var expense = new Expense
        {
            Title = title,
            Category = category,
            Amount = amount,
            Date = DateTime.Now
        };

        store.Add(expense);
        AnsiConsole.MarkupLine("[green]Utgift tillagd![/]");
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Fel: {ex.Message}[/]");
    }
}

void ShowExpenses()
{
    var expenses = store.GetAll()
                        .OrderByDescending(e => e.Date); // LINQ

    var table = new Table().RoundedBorder();
    table.AddColumn("Titel");
    table.AddColumn("Kategori");
    table.AddColumn("Belopp");
    table.AddColumn("Datum");

    foreach (var e in expenses)
    {
        table.AddRow(
            e.Title,
            e.Category,
            $"{e.Amount} kr",
            e.Date.ToShortDateString());
    }

    AnsiConsole.Write(table);
}

void UpdateExpense()
{
    var expenses = store.GetAll();
    if (!expenses.Any())
    {
        AnsiConsole.MarkupLine("[yellow]Inga utgifter att uppdatera.[/]");
        return;
    }

    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<Expense>()
            .Title("Välj utgift att uppdatera")
            .UseConverter(e => $"{e.Title} ({e.Amount} kr)")
            .AddChoices(expenses));

    selected.Title = AnsiConsole.Ask<string>("Ny titel:", selected.Title);
    selected.Category = AnsiConsole.Ask<string>("Ny kategori:", selected.Category);

    decimal amount;
    while (!decimal.TryParse(
        AnsiConsole.Ask<string>("Nytt belopp:"), out amount))
    {
        AnsiConsole.MarkupLine("[red]Skriv ett giltigt nummer.[/]");
    }

    selected.Amount = amount;

    store.Remove(selected);
    store.Add(selected);

    AnsiConsole.MarkupLine("[green]Utgift uppdaterad![/]");
}

void DeleteExpense()
{
    var expenses = store.GetAll();
    if (!expenses.Any())
    {
        AnsiConsole.MarkupLine("[yellow]Inga utgifter att ta bort.[/]");
        return;
    }

    var selected = AnsiConsole.Prompt(
        new SelectionPrompt<Expense>()
            .Title("Välj utgift att ta bort")
            .UseConverter(e => $"{e.Title} ({e.Amount} kr)")
            .AddChoices(expenses));

    store.Remove(selected);
    AnsiConsole.MarkupLine("[green]Utgift borttagen![/]");
}
