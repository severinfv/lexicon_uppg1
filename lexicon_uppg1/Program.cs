// See https://aka.ms/new-console-template for more information
using Microsoft.Win32;

UserInterface app = new UserInterface("../../../personalregister.csv");
app.Run();

class UserInterface
    /* UserInterface - main controller class to handle user interaction via the console.
     Uses CsvRegister to manage csv data. */
{
    private CsvRegister register;
    public UserInterface(string path)
    {
        register = new CsvRegister(path);
        register.Load();
    }

    public void Run()
    {
        register.PrintStats();

        while (true)
        {
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Print out a Register");
            Console.WriteLine("2. Add a record");
            Console.WriteLine("3. Edit an existing record");
            Console.WriteLine("4. Delete a record");
            Console.WriteLine("5. Find a record");
            Console.WriteLine("0. Exit");

            Console.WriteLine("Your choice: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    register.PrintAll();
                    break;
                case "2":
                    register.AddRecordFromInput();
                    register.Save();
                    break;
                case "3":
                    register.EditRecord();
                    register.Save();
                    break;
                case "4":
                    register.DeleteRecord();
                    register.Save();
                    break;
                case "5":
                    Console.Write("Enter text to search: ");
                    string text = Console.ReadLine();
                    register.Search(text);
                    break;
                case "0":
                    Console.WriteLine("Thank you!");
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}

class CsvRegister
/* 
* CsvRegister is responsible for reading from, editing, and writing to the CSV file.
* Stores headers and records internally, and provides features like searching, editing, and saving data.
*/
{
    private string path;
    private List<string> headers = new();
    private List<Record> records = new();

    public List<string> Headers => headers;

    public CsvRegister(string path)
    {
        this.path = path;
    }

    private bool reWrite = false; 
    public void Load()
    {
        var lines = File.ReadAllLines(path);
        if (lines.Length == 0) return;

        headers = lines[0].Split(',').Select(h => h.Trim()).ToList();
        records = lines.Skip(1).Select(line => new Record(headers, line)).ToList();
    }


    public void PrintStats()
    {
        Console.WriteLine($"\nTotal rows: {records.Count}");
        Console.WriteLine("\nSchema:");
        headers.ForEach(h => Console.WriteLine($"- {h}"));
    }
    public void PrintAll()
    {
        int i = 1;
        foreach (var record in records)
        {
            Console.WriteLine($"{i++}. {record}");
        }
    }

    public void Save()
    {
        if (!reWrite)
        {
            Console.WriteLine("No changes to Save.");
            return;
        }
        Console.WriteLine("\nPreview of what will be saved:");
        PrintAll();

        Console.Write("\nDo you want to overwrite the file with these changes? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLower();

        if (confirm == "y")
        {
            var lines = new List<string> { string.Join(",", headers) };
            lines.AddRange(records.Select(r => r.ToCsvLine()));
            File.WriteAllLines(path, lines);
            Console.WriteLine("Changes saved.");
            reWrite = false;
        }
        else
        {
            Console.WriteLine("Cancelled. File not modified.");
            Load();
            reWrite = false;
        }
    }

    public void Search(string text)
    {
        var found = records.Where(r => r.Contains(text)).ToList();
        Console.WriteLine($"\n{found.Count} result(s) found:");
        found.ForEach(r => Console.WriteLine(r));
    }

    public void AddRecordFromValues(List<string> values)
    {
        records.Add(new Record(headers, values));
        reWrite = true;
    }

    public void AddRecordFromInput()
    {
        var values = headers.Select(header => {
            Console.Write($"{header}: ");
            return Console.ReadLine();
        }).ToList();

        AddRecordFromValues(values);
        Console.WriteLine("Record added.");
    }

    public void EditRecord()
    {
        Console.Write("Enter row number to edit: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= records.Count)
        {
            var record = records[index - 1];
            Console.WriteLine("Current values:\n" + record);

            for (int i = 0; i < headers.Count; i++)
            {
                Console.Write($"{headers[i]} (current: {record.GetValue(i)}): ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    record.SetValue(i, input);
            }
            reWrite = true;
            Console.WriteLine("Record updated.");
        }
        else
        {
            Console.WriteLine("Invalid index.");
        }
    }

    public void DeleteRecord()
    {
        Console.Write("Enter row number to delete: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= records.Count)
        {
            records.RemoveAt(index - 1);
            reWrite = true;
            Console.WriteLine("Record deleted.");
        }
        else
        {
            Console.WriteLine("Invalid index.");
        }
    }
}

class Record

/* 
* Record - Represents one row of data in the CSV table.
* Stores values as a list of strings and provides methods for editing and displaying the record.
*/
{
    private List<string> Values;

    public Record(List<string> headers, string csvLine)
    {
        Values = csvLine.Split(',').Select(v => v.Trim()).ToList();
        while (Values.Count < headers.Count)
            Values.Add("");
    }

    public Record(List<string> headers, List<string> values)
    {
        Values = new List<string>(values);
    }

    public string GetValue(int index)
    {
        return index >= 0 && index < Values.Count ? Values[index] : "";
    }

    public void SetValue(int index, string newValue)
    {
        if (index >= 0 && index < Values.Count)
            Values[index] = newValue;
    }

    public bool Contains(string text)
    {
        return Values.Any(v => v.Contains(text, StringComparison.OrdinalIgnoreCase));
    }

    public string ToCsvLine()
    {
        return string.Join(",", Values);
    }

    public override string ToString()
    {
        return string.Join(" | ", Values);
    }
}

