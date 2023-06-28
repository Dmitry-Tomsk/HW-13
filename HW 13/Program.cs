
using HW_13;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

Class_F test_F = new Class_F { i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5 };
Stopwatch stopwatch = new Stopwatch();
int iteration = 1000;

#region
Console.WriteLine("Сериализуемый класс: class F " +
    $"( i1 = {test_F.i1.ToString()}, i2 = {test_F.i2.ToString()}, " +
    $"i3 = {test_F.i3.ToString()}, i4 = {test_F.i4.ToString()}, i5 = {test_F.i5.ToString()}; )");
#endregion

Console.WriteLine("Сериализация в CSV");
var csv = SerializeCSV();
var csv2 = DeserializeCsv(csv);
Console.WriteLine("Сериализация в Json");
var json = SerializeJson(csv2);
DeserializeJson(json);


string SerializeCSV()
{
    //Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    string result = $"{GetPropertyName(test_F.GetType().GetProperties())}\r\n";
    for (int i = 0; i < iteration; i++)
    {
        result += $"{SerializeObj(test_F)}";
    }

    stopwatch.Stop();
    Console.WriteLine($"Сериализация {iteration} объектов в CSV завершена за {stopwatch.ElapsedMilliseconds} мс");
    
    return result;
}

string GetPropertyName(PropertyInfo[] _property)
{
    string result = string.Empty;
    var propertyInfo = _property.Select( x => x.Name ).ToArray();
    result = string.Join( ",", propertyInfo );

    return result;
}

string SerializeObj(object _inputObject)
{
    string result = string.Empty;
    var value = _inputObject.GetType().GetProperties().Select( x => x.GetValue(_inputObject)!.ToString()).ToArray();
    result += string.Join( ",", value);

    return $"{result}\r\n";
}

List<Class_F> DeserializeCsv(string CsvStr)
{
    stopwatch.Reset();
    stopwatch.Start();
    List<Class_F> tstList = new List<Class_F>();
    int ind = CsvStr.IndexOf("\r\n") + 2;
    string cuttedString = CsvStr.Substring(ind, CsvStr.Length - ind);
    List<string> CSVstrings = cuttedString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

    foreach (string csvStr in CSVstrings)
    {
        Class_F obj = DeserializeObj<Class_F>(csvStr);
        tstList.Add(obj);
    }

    stopwatch.Stop();
    Console.WriteLine($"Десериализация {tstList.Count} объектов из CSV завершена за {stopwatch.ElapsedMilliseconds} мс");

    return tstList;
}

T DeserializeObj<T>(string InputObject)
{
    Type type = typeof(T);
    string[] propValues = InputObject.Split(',');
    T result = (T)Activator.CreateInstance(typeof(T))!;
    int index = 0;
    foreach (var prop in type.GetProperties())
    {
        var value = Convert.ChangeType(propValues[index], prop.PropertyType);
        prop.SetValue(result, value);
        index++;
    }

    return result!;
}

string SerializeJson(List<Class_F> class_F)
{
    stopwatch.Reset();
    stopwatch.Start();
    string result = JsonSerializer.Serialize(class_F);
    stopwatch.Stop();
    Console.WriteLine($"Сериализация {class_F.Count} объектов в JSON завершена за {stopwatch.ElapsedMilliseconds} мс");

    return result;
}

void DeserializeJson(string json)
{
    stopwatch.Reset();
    stopwatch.Start();
    List<Class_F> list_f = JsonSerializer.Deserialize<List<Class_F>>(json)!;
    stopwatch.Stop();
    Console.WriteLine($"Десериализация {list_f!.Count} объектов из JSON завершена за {stopwatch.ElapsedMilliseconds} мс");
}