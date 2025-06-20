// take file from first parameter
using AnkiLingo.Data;
using AnkiLingoUploader;
using AnkiLingo.Data;
using AnkiLingo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // or specify the path to the API project
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

string connectionString = configuration.GetConnectionString("DefaultConnection");

var services = new ServiceCollection();
services.AddDbContext<DataContext>(opt =>
    opt.UseSqlServer(connectionString));

services.AddScoped<ICourseService, CourseService>();

var serviceProvider = services.BuildServiceProvider();
var courseService = serviceProvider.GetRequiredService<ICourseService>();


string sourceFile = args.Length > 0 ? args[0] : @"C:\temp\AnkiLingo.xlsx";
bool overrideCourse = args.Length > 1 ? bool.Parse(args[1]) : false;

Console.WriteLine("Anki Lingo Uploader");
Console.WriteLine("-------------------------------------");
Console.WriteLine("Source File: {0}", sourceFile);
Console.WriteLine("Override setting: {0}", overrideCourse);

Course courseInformation = ReadExcelFile.GetCourseInformation(sourceFile);
Console.WriteLine("-------------------------------------");
Console.WriteLine($"Course Name: {courseInformation.Name} ");
Console.WriteLine($"Course Description: {courseInformation.Description} ");
Console.WriteLine($"Course Icon: {courseInformation.Icon} ");

courseInformation.Sections = ReadExcelFile.GetSections(sourceFile, courseInformation);
Console.WriteLine("-------------------------------------");
Console.WriteLine("Sections:");
foreach (Section section in courseInformation.Sections)
{
    Console.WriteLine($"{section.Name} - { section.Description}");
    section.Units = ReadExcelFile.GetUnitsForSection(sourceFile, courseInformation, section);
    foreach (Unit unit in section.Units)
    {
        Console.WriteLine($"\t{unit.Name} - {unit.Description}");
        unit.Entries = ReadExcelFile.GetEntriesForSection(sourceFile, courseInformation, section, unit);
        foreach (Entry item in unit.Entries)
        {
            Console.WriteLine($"\t\t{item.Value1} - {item.Value2}");
        }
    }
}

var result = courseService.Add(courseInformation);
