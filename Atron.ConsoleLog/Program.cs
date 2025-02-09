using Atron.Application.DTO;
using Atron.ConsoleLog;
using System.Text.Json;

//var circle = new Circle(10);
//var rectangle = new Rectangle(10, 20);
//var square = new Square(10);

//Console.WriteLine($"{rectangle.GetArea()}");
//Console.WriteLine($"{circle.GetArea()}");
//Console.WriteLine($"{square.GetArea()}");

//var anon = new
//{
//    Name = "Eduardo",
//    Age = 29,
//    Address = new
//    {
//        Street = "R. Curitiba",
//        City = "RJ"
//    }
//};


var departamento = new DepartamentoDTO { Codigo = "QRZ", Descricao = "Departamento QRZ" };

Console.WriteLine(SerializeObject(departamento));

string SerializeObject(object obj)
{
    return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
}
