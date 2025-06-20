using AnkiLingo.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.IdentityModel.Tokens;

namespace AnkiLingoUploader
{
    public static class ReadExcelFile
    {
        public static Course? GetCourseInformation(string file)
        {
            Course courseInformation = new Course()
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Icon = ReadCellValue(file, "B3", "apple"),
                Description = ReadCellValue(file, "B2", string.Empty),
                Name = ReadCellValue(file, "B1", "New Course"),
                Id = 0
            };
                                    
            return courseInformation;
        }

        public static string ReadCellValue(string filePath, string cellAddress, string defaultValue)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // 1-based index for the first worksheet
            var cellValue = worksheet.Cell(cellAddress).GetValue<string>();

            if (string.IsNullOrEmpty(cellValue)) return defaultValue;
            return cellValue;
        }

        public static ICollection<AnkiLingo.Data.Section> GetSections(string file, Course courseInformation)
        {
            List<AnkiLingo.Data.Section> sectionList = new List<AnkiLingo.Data.Section>();

            using var workbook = new XLWorkbook(file);
            var worksheet = workbook.Worksheet(1); // 1-based index for the first worksheet
            // Starting from A6
            var numberOfRows = worksheet.RowCount();
            for (int i = 6; i <= numberOfRows; i++)
            {
                var currentCellOne = $"A{i}";
                var currentCellTwo = $"B{i}";

                if (string.IsNullOrEmpty(worksheet.Cell(currentCellOne).GetValue<string>())) break;

                sectionList.Add(new AnkiLingo.Data.Section()
                {
                    CourseId = courseInformation.Id,
                    Course = courseInformation,
                    Id = 0,
                    Name = worksheet.Cell(currentCellOne).GetValue<string>(),
                    Description = worksheet.Cell(currentCellTwo).GetValue<string>(),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });                 
            }

            return sectionList;
        }

        public static ICollection<Unit> GetUnitsForSection(string file, Course courseInformation, AnkiLingo.Data.Section section)
        {
            List<Unit> unitList = new List<Unit>();

            using var workbook = new XLWorkbook(file);
            var worksheet = workbook.Worksheet(section.Name); // 1-based index for the first worksheet

            var numberOfRows = worksheet.RowCount();
            for (int i = 1; i <= numberOfRows; i++)
            {
                var currentCellOne = $"A{i}";
                var currentCellTwo = $"B{i}";

                if (string.IsNullOrEmpty(worksheet.Cell(currentCellOne).GetValue<string>())) break;

                unitList.Add(new Unit()
                {
                    SectionId = section.Id,
                    Section = section,
                    Id = 0,
                    Name = worksheet.Cell(currentCellOne).GetValue<string>(),
                    Description = worksheet.Cell(currentCellTwo).GetValue<string>(),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });
            }

            return unitList;
        }

        public static ICollection<Entry> GetEntriesForSection(string file, Course courseInformation, AnkiLingo.Data.Section section, Unit unit)
        {
            List<Entry> entries = new List<Entry>();

            using var workbook = new XLWorkbook(file);
            var worksheet = workbook.Worksheet(section.Name); // 1-based index for the first worksheet

            var numberOfRows = worksheet.RowCount();
            var entriesStartAt = 0;
            for (int i = 1; i <= numberOfRows; i++)
            {
                var currentCellOne = $"A{i}";

                entriesStartAt = i;
                if (string.IsNullOrEmpty(worksheet.Cell(currentCellOne).GetValue<string>())) break;
            }

            for (int i = entriesStartAt+2; i < numberOfRows; i++)
            {
                var currentCellOne = $"A{i}";
                var currentCellTwo = $"B{i}";
                var currentCellThree = $"C{i}";

                if (string.IsNullOrEmpty(worksheet.Cell(currentCellOne).GetValue<string>())) break;
                if (worksheet.Cell(currentCellOne).GetValue<string>() != unit.Name) continue;
                entries.Add(new Entry()
                {
                    UnitId = unit.Id,
                    Unit = unit,
                    Id = 0,
                    Name = string.Empty,
                    Description = string.Empty,
                    Value1 = worksheet.Cell(currentCellTwo).GetValue<string>(),
                    Value2 = worksheet.Cell(currentCellThree).GetValue<string>(),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });              
            }

            return entries;
        }
    }
}
