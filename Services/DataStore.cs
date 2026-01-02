using System.Text.Json;

namespace BudgetManager.Services;

public class DataStore<T>
{
    private string _filePath;
    private List<T> _items = new List<T>();

    public DataStore(string filePath)
    {
        _filePath = filePath;

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Load();
    }


    public void Add(T item)
    {
        _items.Add(item);
        Save();
    }

    public void Remove(T item)
    {
        _items.Remove(item);
        Save();
    }

    public List<T> GetAll() => _items;

    public T? Find(Func<T, bool> predicate)
        => _items.FirstOrDefault(predicate);

    private void Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Save(); // Skapa fil om den saknas
                return;
            }
            var json = File.ReadAllText(_filePath);
            _items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FEL] Kunde inte läsa fil: {ex.Message}");
            _items = new List<T>();
        }
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FEL] Kunde inte spara fil: {ex.Message}");
        }
    }
}
