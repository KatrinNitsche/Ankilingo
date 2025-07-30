using AnkiLingoExcelService.Data;
using ClosedXML.Excel;

namespace AnkiLingoExcelService
{
    public static class ExcelService
    {
        /// <summary>
        /// returns the list of excel file names in the given directory
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>list of file names</returns>
        public static List<string> GetCourseNames(string filePath)
        {
            List<string> courseNames = new List<string>();
            if (!Directory.Exists(filePath))
            {
                Console.WriteLine($"Directory {filePath} does not exist.");
                return courseNames;
            }
            var files = Directory.GetFiles(filePath, "*.xlsx");
            if (files.Length == 0)
            {
                Console.WriteLine("No Excel files found in the directory.");
                return courseNames;
            }
            foreach (var file in files)
            {
                using var workbook = new XLWorkbook(file);
                var worksheet = workbook.Worksheet(1); // 1-based index for the first worksheet
                var cellValue = worksheet.Cell("B1").GetValue<string>();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    courseNames.Add(cellValue);
                }
            }
            return courseNames;
        }

        /// <summary>
        /// returns the course details from the first worksheet of the given Excel file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>course object</returns>
        public static CourseData GetCourseDetails(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return new CourseData();
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // 1-based index for the first worksheet
            var courseData = new CourseData
            {
                Name = worksheet.Cell("B1").GetValue<string>(),
                Description = worksheet.Cell("B2").GetValue<string>(),
                Icon = worksheet.Cell("B3").GetValue<string>()
            };

            // add sections to the course data
            int row = 6;
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                var sectionName = worksheet.Cell($"A{row}").GetValue<string>();
                var sectionDescription = worksheet.Cell($"B{row}").GetValue<string>();
                if (!string.IsNullOrEmpty(sectionName) && sectionName != "Sections")
                {
                    var sectionDetails = GetSectionDetails(filePath, sectionName);
                    sectionDetails.Description = sectionDescription;
                    courseData.Sections.Add(sectionDetails);
                }

                row++;
            }

            return courseData;
        }

        /// <summary>
        /// returns the details of a specific section from the given Excel file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="sectionName">name of the section (tab name in the excel file)</param>
        /// <returns>section object</returns>
        public static SectionData GetSectionDetails(string filePath, string sectionName)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return new SectionData
                {
                    Name = sectionName,
                    Units = new List<UnitData>()
                };
            }

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(sectionName); // 1-based index for the first worksheet
            var section = new SectionData
            {
                Name = sectionName,
                Units = new List<UnitData>(),
            };

            int row = 1;
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                var unit = new UnitData
                {
                    Name = worksheet.Cell($"A{row}").GetValue<string>(),
                    Description = worksheet.Cell($"B{row}").GetValue<string>()
                };
                section.Units.Add(unit);
                row++;
            }

            row += 2; // Skip the next 2 row for entries (empty row and header row)

            // the following rows contain the entries for the units A1 - unit name, B1 - value 1, B1 - value 2            
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                var entry = new EntryData
                {
                    Value1 = worksheet.Cell($"B{row}").GetValue<string>(),
                    Value2 = worksheet.Cell($"C{row}").GetValue<string>(),

                    // D<row> can be empty but than should be set to 0
                    LevelOfKnowledge = worksheet.Cell($"D{row}").IsEmpty()
                                       ? 0
                                       : worksheet.Cell($"D{row}").GetValue<int>(),

                    // E<row> can be empty but than should be set to min date
                    LastReviewed = worksheet.Cell($"E{row}").IsEmpty()
                                   ? DateTime.MinValue
                                   : worksheet.Cell($"E{row}").GetValue<DateTime>()
                };

                // Find the unit for this entry
                var unitName = worksheet.Cell($"A{row}").GetValue<string>();
                var unit = section.Units.FirstOrDefault(u => u.Name == unitName);
                if (unit != null)
                {
                    unit.Entries.Add(entry);
                }
                row++;
            }

            return section;
        }

        /// <summary>
        /// Updates the LevelOfKnowledge and LastReviewed fields of an entry in the specified section and unit.
        /// </summary>
        /// <param name="filePath">The path to the Excel file.</param>
        /// <param name="sectionName">name of the section (tab name in the excel file)</param>
        /// <param name="unitName">name of the unit</param>
        /// <param name="value1">value 1</param>
        /// <param name="value2">value 2</param>
        /// <param name="levelOfKnowledge">new level of knowledge</param>
        public static bool UpdateEntry(string filePath, string sectionName, string unitName, string value1, string value2, int levelOfKnowlege)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return false;
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(sectionName); // 1-based index for the first worksheet
            int row = 1;

            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                row++;
            }

            row++; // Skip the header row for entries
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                if (worksheet.Cell($"A{row}").GetValue<string>() == unitName &&
                    worksheet.Cell($"B{row}").GetValue<string>() == value1 &&
                    worksheet.Cell($"C{row}").GetValue<string>() == value2)
                {
                    worksheet.Cell($"D{row}").Value = levelOfKnowlege;
                    worksheet.Cell($"E{row}").Value = DateTime.Now; // Update Last Reviewed to now
                    workbook.Save();
                    return true;
                }
                row++;
            }
            Console.WriteLine("Entry not found.");
            return false;
        }
    }
}
