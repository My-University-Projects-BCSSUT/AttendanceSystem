using OfficeOpenXml;

namespace AttendanceSystem.Patterns.Singleton
{
    // Singleton Pattern for Excel Service
    public sealed class ExcelService
    {
        private static ExcelService? _instance;
        private static readonly object _lock = new object();

        private ExcelService()
        {
            // No special initialization needed for EPPlus 8.x
            // License is set in Program.cs
        }

        public static ExcelService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ExcelService();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<List<Dictionary<string, string>>> ReadExcelAsync(Stream stream)
        {
            var data = new List<Dictionary<string, string>>();
            
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;
                var colCount = worksheet.Dimension?.Columns ?? 0;

                if (rowCount == 0) return data;

                // Read headers
                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++)
                {
                    headers.Add(worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}");
                }

                // Read data rows
                for (int row = 2; row <= rowCount; row++)
                {
                    var rowData = new Dictionary<string, string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        rowData[headers[col - 1]] = worksheet.Cells[row, col].Value?.ToString() ?? string.Empty;
                    }
                    data.Add(rowData);
                }
            }

            return data;
        }

        public async Task<byte[]> ExportToExcelAsync<T>(List<T> data, string sheetName = "Sheet1")
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                
                // Get properties
                var properties = typeof(T).GetProperties();
                
                // Add headers
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Add data
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(data[row]);
                        worksheet.Cells[row + 2, col + 1].Value = value;
                    }
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                
                return package.GetAsByteArray();
            }
        }
    }
}
