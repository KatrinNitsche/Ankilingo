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
                var value1 = worksheet.Cell($"B{row}").GetValue<string>();
                var value2 = worksheet.Cell($"C{row}").GetValue<string>();
                var levelOfKnowledge = worksheet.Cell($"D{row}").GetValue<int>();
                var lastReviewed = worksheet.Cell($"E{row}").GetValue<DateTime>();
                var reviewCount = worksheet.Cell($"F{row}").GetValue<int>();

                var entry = new EntryData
                {
                    LastReviewed = lastReviewed,
                    LevelOfKnowledge = levelOfKnowledge,
                    ReviewCount = reviewCount,
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

        public static List<EntryData> GetEntries(string filePath, string sectionName)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return new List<EntryData>();
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(sectionName); // 1-based index for the first worksheet
            int row = 1;
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                row++;
            }
            row++; // Skip the header row for entries
            var entries = new List<EntryData>();
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                if (worksheet.Cell($"B{row}").GetValue<string>() == "Value 1")
                {
                    row++; // Skip the header row for entries
                    continue;
                }

                var Value1 = worksheet.Cell($"B{row}").GetValue<string>();
                var Value2 = worksheet.Cell($"C{row}").GetValue<string>();
                var LevelOfKnowledge = worksheet.Cell($"D{row}").GetValue<int>();
                var LastReviewed = worksheet.Cell($"E{row}").GetValue<DateTime>();
                var ReviewCount = worksheet.Cell($"F{row}").GetValue<int>();

                var entry = new EntryData
                {
                    Value1 = Value1,
                    Value2 = Value2,
                    LevelOfKnowledge = LevelOfKnowledge,
                    LastReviewed = LastReviewed,
                    ReviewCount = ReviewCount
                };

                entries.Add(entry);
                row++;
            }
            return entries;
        }

        //public static List<ImageData> GetImages(string filePath)
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        Console.WriteLine($"File {filePath} does not exist.");
        //        return new List<ImageData>();
        //    }
        //    using var workbook = new XLWorkbook(filePath);
        //    var worksheet = workbook.Worksheet("Images");
        //    int row = 3;
        //    var images = new List<ImageData>();
        //    while (!worksheet.Cell($"A{row}").IsEmpty())
        //    {
        //        var sectionName = worksheet.Cell($"A{row}").GetValue<string>();
        //        var unitName = worksheet.Cell($"B{row}").GetValue<string>();
        //        var imageName = worksheet.Cell($"C{row}").GetValue<string>();
        //        var image = images.FirstOrDefault(i => i.ImageName == imageName && i.SectionName == sectionName && i.UnitName == unitName);
        //        if (image == null)
        //        {
        //            image = new ImageData
        //            {
        //                SectionName = sectionName,
        //                UnitName = unitName,
        //                ImageName = imageName,
        //                ImageCovers = new List<ImageWord>()
        //            };
        //            images.Add(image);
        //        }

        //        // the following columns can be empty or contain data for an image cover
        //        if (worksheet.Cell($"D{row}").IsEmpty() || worksheet.Cell($"E{row}").IsEmpty() ||
        //            worksheet.Cell($"F{row}").IsEmpty() || worksheet.Cell($"G{row}").IsEmpty() ||
        //            worksheet.Cell($"H{row}").IsEmpty())
        //        {
        //            row++;
        //        }
        //        else
        //        {
        //            var index = worksheet.Cell($"D{row}").GetValue<int>();
        //            var Value1 = worksheet.Cell($"E{row}").GetValue<string>();
        //            var Value2 = string.Empty;
        //            var LevelOfKnowledge = worksheet.Cell($"F{row}").GetValue<int>();
        //            var imageCover = new ImageWord
        //            {
        //                Id = index,
        //                Value = new EntryData
        //                {
        //                    Value1 = Value1,
        //                    Value2 = Value2,
        //                    LevelOfKnowledge = LevelOfKnowledge,
        //                    LastReviewed = worksheet.Cell($"G{row}").GetValue<DateTime>(),
        //                    ReviewCount = worksheet.Cell($"H{row}").GetValue<int>()
        //                }
        //            };
        //            image.ImageCovers.Add(imageCover);
        //        }

        //        row++;
        //    }

        //    return images;
        //}

        //public static bool UpdateImageEntry(string filePath, string sectionName, string unitName, string imageName, int index, int levelOfKnowledge)
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        Console.WriteLine($"File {filePath} does not exist.");
        //        return false;
        //    }
        //    using var workbook = new XLWorkbook(filePath);
        //    var worksheet = workbook.Worksheet("Images");
        //    int row = 3;
        //    while (!worksheet.Cell($"A{row}").IsEmpty())
        //    {
        //        if (worksheet.Cell($"A{row}").GetValue<string>() == sectionName &&
        //            worksheet.Cell($"B{row}").GetValue<string>() == unitName &&
        //            worksheet.Cell($"C{row}").GetValue<string>() == imageName &&
        //            worksheet.Cell($"D{row}").GetValue<int>() == index)
        //        {
        //            var reviewCountCell = worksheet.Cell($"H{row}");                     
        //            int newReviewCount = reviewCountCell.GetValue<int>() + 1;
        //            reviewCountCell.Value = newReviewCount;
        //            worksheet.Cell($"F{row}").Value = levelOfKnowledge;
        //            worksheet.Cell($"G{row}").Value = DateTime.Now; // Update Last Reviewed to now
        //            workbook.Save();
        //            return true;
        //        }
        //        row++;
        //    }
        //    Console.WriteLine("Entry not found.");
        //    return false;
        //}

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
                    var reviewCountCell = worksheet.Cell($"F{row}");
                    var lastReviewed = worksheet.Cell($"E{row}").GetValue<DateTime>();
                    var oldLevelOfKnowledge = worksheet.Cell($"D{row}").GetValue<int>();

                    if (oldLevelOfKnowledge < levelOfKnowlege)
                    {
                        // Increment review count if the level of knowledge has increased
                        int newReviewCount = reviewCountCell.GetValue<int>() + 1;
                        reviewCountCell.Value = newReviewCount;
                    }

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

        /// <summary>
        /// Read user data from the first worksheet of the given Excel file
        /// </summary>
        /// <param name="filePath">excel file</param>
        /// <returns>user information</returns>
        public static UserData GetUserData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return new UserData();
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // 1-based index for the first worksheet
            var userData = new UserData
            {
                StreakLength = worksheet.Cell("B4").GetValue<int>(),
                GemsCount = worksheet.Cell("B2").GetValue<int>(),
                CurrentCourse = worksheet.Cell("B1").GetValue<string>(),
                LastStudy = worksheet.Cell("B5").GetValue<DateTime>(),
                XPCount = worksheet.Cell("B3").GetValue<int>()
            };
            return userData;
        }

        public static void UpdateUserData(string filePath, UserData userData, int? XP = null, TimeOnly? duration = null)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist.");
                return;
            }
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);

            if (userData != null)
            {
                worksheet.Cell("B4").Value = userData.StreakLength;
                worksheet.Cell("B2").Value = userData.GemsCount;
                worksheet.Cell("B1").Value = userData.CurrentCourse;
                workbook.Save();
                return; // If userData is provided, we update it and exit
            }

            if (XP.HasValue)
            {
                // Increase the current streak by 1 if the last study was longer than 24 hourse ago 
                var lastStudyCell = worksheet.Cell("B5");
                var lastStudy = lastStudyCell.GetValue<DateTime>();
                if (lastStudy.Date < DateTime.Now.Date && (DateTime.Now - lastStudy).TotalHours < 48)
                {
                    var oldStreakLength = worksheet.Cell("B4").GetValue<int>();
                    worksheet.Cell("B4").Value = oldStreakLength + 1; // Increment streak length
                }
                else
                {
                    // Reset streak length if the last study was longer than 48 hours ago
                    if ((DateTime.Now - lastStudy).TotalHours > 48)
                    {
                        worksheet.Cell("B4").Value = 1; // Reset streak length
                    }
                }

                // update gems count based on XP
                var oldGemsCount = worksheet.Cell("B2").GetValue<int>();
                worksheet.Cell("B2").Value = oldGemsCount + (XP.Value / 10); // Assuming 10 XP = 1 gem

                // Update the XP value in cell B3
                var oldXPValue = worksheet.Cell("B3").GetValue<int>();
                worksheet.Cell("B3").Value = oldXPValue + XP.Value;
                workbook.Save();
            }

            if (duration.HasValue)
            {
                worksheet.Cell("B5").Value = DateTime.Now;
                workbook.Save();
            }

            // Starting from Row 9 is a list of study sessions with the following columns:
            // A - Date, B - XP earned, C - Duration
            // a new row should be added for a study session if there wasn't one added for today
            int row = 9;
            while (!worksheet.Cell($"A{row}").IsEmpty())
            {
                if (worksheet.Cell($"A{row}").GetValue<DateTime>().Date == DateTime.Now.Date)
                {
                    // A row for today already exists, so we can exit
                    return;
                }
                row++;
            }

            // If we reach here, it means no row for today exists, so we add a new one
            worksheet.Cell($"A{row}").Value = DateTime.Now.Date;
            if (XP.HasValue)
            {
                worksheet.Cell($"B{row}").Value = XP.Value;
            }
            if (duration.HasValue)
            {
                worksheet.Cell($"C{row}").Value = duration.Value.ToString();
            }

            workbook.Save();
        }
    }
}
