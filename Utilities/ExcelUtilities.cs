using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace CatalogueDownloader.Utilities
{
    class ExcelUtilities
    {
        public static Workbook CreateWorkbook(string excelPath)
        {
            var excel = new Application();
            var wb = excel.Workbooks.Open(excelPath);
            return wb;
        }

        public static dynamic SelectExcelSheet(Workbook wb, string sheetName)
        {
            var excelSheet = wb.Sheets[sheetName];
            excelSheet.Select(Type.Missing);
            return excelSheet;
        }

        public static void WriteToExcel(dynamic excelSheet, List<string> valueList, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                excelSheet.Cells.WrapText = true;
                excelSheet.Cells[i, 7].Value = valueList[i - start];
            }
        }

        public static void ReadFromExcel(Dictionary<string, string> eanDictionary, dynamic excelSheet, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                eanDictionary.Add(excelSheet.Cells[i, 1].Value.ToString(), null);
            }
        }
    }
}
