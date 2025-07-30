using AnkiLingoExcelService;

Console.WriteLine("Course Service - Excel files");

string excelPath = "C:\\temp\\AnkiLingo";

if (!Directory.Exists(excelPath))
{
    Console.WriteLine($"Directory {excelPath} does not exist.");
    return;
}

var courseList = ExcelService.GetCourseNames(excelPath);
var courseFileName = courseList[0] + ".xlsx";
var courseDetails =ExcelService.GetCourseDetails(Path.Combine(excelPath, courseFileName));

var lastReviewed = courseDetails.Sections[0].Units[0].Entries[0].LastReviewed;
Console.WriteLine($"Course Name: {courseDetails.Name}");
Console.WriteLine($"Course Description: {courseDetails.Description}");
Console.WriteLine($"Course Icon: {courseDetails.Icon}");
Console.WriteLine($"Last Reviewed: {lastReviewed}");