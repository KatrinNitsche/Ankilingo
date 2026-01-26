using AnkiLingo.Data;
using AnkiLingoExcelService.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components.Forms;

namespace AnkiLingoExcelService
{
    public static class ExcelService 
    {
        public static CourseData LoadCourseFromExcel(IBrowserFile? file)
        {
            if (file == null)
            {
                Console.WriteLine("No file provided.");
                return new CourseData();
            }
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
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
                    worksheet = workbook.Worksheet(sectionName); // 1-based index for the first worksheet
                    var section = new SectionData
                    {
                        Name = sectionName,
                        Units = new List<UnitData>(),
                    };

                    int sectionRow = 1;
                    while (!worksheet.Cell($"A{sectionRow}").IsEmpty())
                    {
                        var unit = new UnitData
                        {
                            Name = worksheet.Cell($"A{sectionRow}").GetValue<string>(),
                            Description = worksheet.Cell($"B{sectionRow}").GetValue<string>()
                        };
                        section.Units.Add(unit);
                        sectionRow++;
                    }

                    sectionRow += 2; // Skip the next 2 row for entries (empty row and header row)

                    // the following rows contain the entries for the units A1 - unit name, B1 - value 1, B1 - value 2
                    while (!worksheet.Cell($"A{sectionRow}").IsEmpty())
                    {
                        var value1 = worksheet.Cell($"B{sectionRow}").GetValue<string>();
                        var value2 = worksheet.Cell($"C{sectionRow}").GetValue<string>();
                        var levelOfKnowledge = worksheet.Cell($"D{sectionRow}").GetValue<int>();
                        var lastReviewed = worksheet.Cell($"E{sectionRow}").GetValue<DateTime>();
                        var reviewCount = worksheet.Cell($"F{sectionRow}").GetValue<int>();

                        var entry = new EntryData
                        {
                            LastReviewed = lastReviewed,
                            LevelOfKnowledge = levelOfKnowledge,
                            ReviewCount = reviewCount,
                            Value1 = value1,
                            Value2 = value2
                        };

                        // Find the unit for this entry
                        var unitName = worksheet.Cell($"A{sectionRow}").GetValue<string>();
                        var unit = section.Units.FirstOrDefault(u => u.Name == unitName);
                        if (unit != null)
                        {
                            unit.Entries.Add(entry);
                        }
                        sectionRow++;
                    }
                }
                row++;
            }
            return courseData;
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
                var order = worksheet.Cell($"C{row}").GetValue<int>();
                if (!string.IsNullOrEmpty(sectionName) && sectionName != "Sections")
                {
                    var sectionDetails = GetSectionDetails(filePath, sectionName);
                    sectionDetails.Description = sectionDescription;
                    sectionDetails.Order = order;
                    courseData.Sections.Add(sectionDetails);
                }

                row++;
            }

            // addi image data
            courseData.Images = GetImages(filePath);
          
            return courseData;
        }

        /// <summary>
        /// returns the details of a specific section from the given Excel file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="sectionName">name of the section (tab name in the excel file)</param>
        /// <returns>section object</returns>
        private static SectionData GetSectionDetails(string filePath, string sectionName)
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
                    Description = worksheet.Cell($"B{row}").GetValue<string>(),
                    Order = worksheet.Cell($"C{row}").GetValue<int>(),
                };
                section.Units.Add(unit);
                row++;
            }

            row += 2; // Skip the next 2 row for entries (empty row and header row)

            // the following rows contain the entries for the units A1 - unit name, B1 - value 1, B1 - value 2            
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                var value1 = worksheet.Cell($"B{row}").GetValue<string>();
                var value2 = worksheet.Cell($"C{row}").GetValue<string>();
           
                var entry = new EntryData
                {
                    Value1 = value1,
                    Value2 = value2
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

        private static List<ImageData> GetImages(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return new List<ImageData>();
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet("Images");
            int row = 3;
            var images = new List<ImageData>();
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                var sectionName = worksheet.Cell($"A{row}").GetValue<string>();
                var unitName = worksheet.Cell($"B{row}").GetValue<string>();
                var imageName = worksheet.Cell($"C{row}").GetValue<string>();
                var image = images.FirstOrDefault(i => i.ImageName == imageName && i.SectionName == sectionName && i.UnitName == unitName);
                if (image == null)
                {
                    image = new ImageData
                    {
                        SectionName = sectionName,
                        UnitName = unitName,
                        ImageName = imageName,
                        ImageCovers = new List<ImageWord>()
                    };
                    images.Add(image);
                }

                // the following columns can be empty or contain data for an image cover
                if (worksheet.Cell($"D{row}").IsEmpty() || worksheet.Cell($"E{row}").IsEmpty() ||
                    worksheet.Cell($"F{row}").IsEmpty() || worksheet.Cell($"G{row}").IsEmpty() ||
                    worksheet.Cell($"H{row}").IsEmpty())
                {
                    row++;
                }
                else
                {
                    var index = worksheet.Cell($"D{row}").GetValue<int>();
                    var Value1 = worksheet.Cell($"E{row}").GetValue<string>();
                    var Value2 = string.Empty;
                    var LevelOfKnowledge = worksheet.Cell($"F{row}").GetValue<int>();
                    var imageCover = new ImageWord
                    {
                        EntryId = index,
                        Value = new EntryData
                        {
                            Value1 = Value1,
                            Value2 = Value2,
                            LevelOfKnowledge = LevelOfKnowledge,
                            LastReviewed = worksheet.Cell($"G{row}").GetValue<DateTime>(),
                            ReviewCount = worksheet.Cell($"H{row}").GetValue<int>()
                        },                        
                    };
                    image.ImageCovers.Add(imageCover);
                }

                row++;
            }

            return images;
        }

        public static bool UpdateImageEntry(string filePath, string sectionName, string unitName, string imageName, int index, int levelOfKnowledge)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return false;
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet("Images");
            int row = 3;
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                if (worksheet.Cell($"A{row}").GetValue<string>() == sectionName &&
                    worksheet.Cell($"B{row}").GetValue<string>() == unitName &&
                    worksheet.Cell($"C{row}").GetValue<string>() == imageName &&
                    worksheet.Cell($"D{row}").GetValue<int>() == index)
                {
                    var reviewCountCell = worksheet.Cell($"H{row}");
                    int newReviewCount = reviewCountCell.GetValue<int>() + 1;
                    reviewCountCell.Value = newReviewCount;
                    worksheet.Cell($"F{row}").Value = levelOfKnowledge;
                    worksheet.Cell($"G{row}").Value = DateTime.Now; // Update Last Reviewed to now
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
