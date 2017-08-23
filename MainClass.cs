using System;
using System.Collections.Generic;
using System.Linq;
using CatalogueDownloader.Controllers;
using CatalogueDownloader.Models;
using CatalogueDownloader.Utilities;

namespace CatalogueDownloader
{
    class MainClass
    {
        static CharDatabase charDB;
        static void Main(string[] args)
        {
             charDB = new CharDatabase();
//            SortProductCategories("CATA", 2, 2, 57);
//            Console.WriteLine("step 1 complete");
//
            //SortProductCategories("Legrand", 2, 2, 160);
            //Console.WriteLine("step 2 complete");

//            ExecuteLegrandOperations("C:/Tigoo/LegrandNew.xlsx", 2, 160);
//            Console.WriteLine("step 1 complete");
//
            ExecuteScameOperations("C:/Tigoo/Scame.xlsx", 3, 313);
            Console.WriteLine("step 2 complete");
//
//            ExecuteKnipexOperations("C:/Tigoo/Knipex.xlsx", 3, 125);
//            Console.WriteLine("step 3 complete");

//            ExecuteBsOperations("C:/Tigoo/Bs.xlsx", 2, 208);
//            Console.WriteLine("step 4 complete");

            var ordered = charDB.charList.GroupBy(x => x.CharName).Select(grp => grp.ToList()).ToList().Distinct();

            Environment.Exit(0);
        }

        private static void SortProductCategories(string product, int columnNumber, int startIndex, int endIndex)
        {
            var excelPath = "C:/Tigoo/EmoKrastev.xlsx";
            var wb = ExcelUtilities.CreateWorkbook(excelPath);
            try
            {
                var excelSheet = ExcelUtilities.SelectExcelSheet(wb, product);
                for (int i = startIndex; i <= endIndex; i++)
                {
                    string str = excelSheet.Cells[i, columnNumber].Value.ToString();
                    List<string> splitCategories = str.Split('.').ToList();
                    if (splitCategories.Count < 2)
                    {
                        continue;
                    }

                    int lgIndex = Array.FindIndex(splitCategories.ToArray(), x => x.ToLower().Equals(product.ToLower()));
                    splitCategories.RemoveAt(lgIndex);
                    splitCategories.Insert(0, product);
                    str = string.Join(".", splitCategories);
                    excelSheet.Cells[i, columnNumber].Value = str;
                    //Console.WriteLine("Line modified!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                wb.Close();
            }
        }

        private static void ExecuteLegrandOperations(string excelPath, int startIndex, int endIndex)
        {
            var wb = ExcelUtilities.CreateWorkbook(excelPath);
            var excelSheet = ExcelUtilities.SelectExcelSheet(wb, "Legrand");
            var eanDictionary = new Dictionary<string, string>();
            ExcelUtilities.ReadFromExcel(eanDictionary, excelSheet, startIndex, endIndex);
            var copiedDict = eanDictionary.ToDictionary(entry => entry.Key,
                entry => entry.Value);
            var fieldsDict = new Dictionary<string, int>();
            foreach (var ean in eanDictionary.Keys)
            {
                var legrand = new Legrand();
                var unprocessedHtml = HtmlHandler.GetUnprocessedHtml("Legrand", ean);
                List<Dictionary<string, string>> fieldCollections = new List<Dictionary<string, string>>();
                try
                {
                    copiedDict = legrand.GetMasterData(unprocessedHtml, ean).ToDictionary(k => k.Key, v => v.Value);
                }
                catch (Exception)
                {
                    startIndex++;
                    continue;
                }
                
                fieldCollections.Add(legrand.GetProductChars(unprocessedHtml, ean));
                fieldCollections.Add(legrand.GetGeneralChars(unprocessedHtml, ean));
                fieldCollections.Add(legrand.GetMoreInfo(unprocessedHtml, ean, 0));
                fieldCollections.Add(legrand.GetMoreInfo(unprocessedHtml, ean, 1));
                fieldCollections.Add(legrand.GetPicture(unprocessedHtml, ean));

                copiedDict = fieldCollections
                    .Where(fieldCollection => fieldCollection != null)
                    .Aggregate(copiedDict, (current, fieldCollection) => current.Union(fieldCollection).ToDictionary(k => k.Key, v => v.Value));

                Console.WriteLine(startIndex + ". " + ean + " downloaded");

                charDB.AddDic(copiedDict);
                
                foreach (var entry in copiedDict)
                {
                    var fieldIndex = 0;
                    if (!fieldsDict.ContainsKey(entry.Key))
                    {
                        fieldsDict.Add(entry.Key, 7 + fieldsDict.Count);
                    }

                    fieldIndex = fieldsDict[entry.Key];
                    excelSheet.Cells.WrapText = true;
                    excelSheet.Cells[1, fieldIndex].Value = entry.Key;
                    excelSheet.Cells[startIndex, fieldIndex].Value = entry.Value;
                }
                Console.WriteLine(startIndex + ". " + ean + " written");
                startIndex++;
            }

            wb.Close();
        }

        private static void ExecuteBsOperations(string path, int startIndex, int endIndex)
        {
            var wb = ExcelUtilities.CreateWorkbook(path);
            var excelSheet = ExcelUtilities.SelectExcelSheet(wb, "Brennenshtuhl");
            var eanDictionary = new Dictionary<string, string>();
            ExcelUtilities.ReadFromExcel(eanDictionary, excelSheet, startIndex, endIndex);
            var copiedDict = eanDictionary.ToDictionary(entry => entry.Key,
                entry => entry.Value);
            var fieldsDict = new Dictionary<string, int>();

            foreach (var ean in eanDictionary.Keys)
            {
                var bs = new Brennenstuhl();

                //get link
                string bsFullUrl = bs.GetProductUrl(ean);
                if (string.IsNullOrEmpty(bsFullUrl))
                {
                    copiedDict[ean] = null;
                    startIndex++;
                    continue;
                }
                //get link end

                var unprocessedHtml = HtmlHandler.GetUnprocessedHtml("Brennenstuhl", bsFullUrl);
                List<Dictionary<string, string>> fieldCollections = new List<Dictionary<string, string>>();
                try
                {
                    copiedDict = bs.GetTechData(unprocessedHtml, ean).ToDictionary(k => k.Key, v => v.Value);
                }
                catch (Exception)
                {
                    startIndex++;
                    continue;
                }

                fieldCollections.Add(bs.GetDownloads(unprocessedHtml, ean));
                fieldCollections.Add(bs.GetPicture(unprocessedHtml, ean));

                copiedDict = fieldCollections
                    .Where(fieldCollection => fieldCollection != null)
                    .Aggregate(copiedDict, (current, fieldCollection) => current.Union(fieldCollection).ToDictionary(k => k.Key, v => v.Value));

                Console.WriteLine(startIndex + ". " + ean + " downloaded");

                charDB.AddDic(copiedDict);

                foreach (var entry in copiedDict)
                {
                    var fieldIndex = 0;
                    if (!fieldsDict.ContainsKey(entry.Key))
                    {
                        fieldsDict.Add(entry.Key, 7 + fieldsDict.Count);
                    }

                    fieldIndex = fieldsDict[entry.Key];
                    excelSheet.Cells.WrapText = true;
                    excelSheet.Cells[1, fieldIndex].Value = entry.Key;
                    excelSheet.Cells[startIndex, fieldIndex].Value = entry.Value;
                }
                Console.WriteLine(startIndex + ". " + ean + " written");
                startIndex++;
            }

            wb.Close();
        }

        private static void ExecuteKnipexOperations(string excelPath, int startIndex, int endIndex)
        {
            var wb = ExcelUtilities.CreateWorkbook(excelPath);
            var excelSheet = ExcelUtilities.SelectExcelSheet(wb, "Knipex");
            var eanDictionary = new Dictionary<string, string>();
            ExcelUtilities.ReadFromExcel(eanDictionary, excelSheet, startIndex, endIndex);
            var copiedDict = eanDictionary.ToDictionary(entry => entry.Key,
                entry => entry.Value);
            var fieldsDict = new Dictionary<string, int>();

            foreach (var ean in eanDictionary.Keys)
            {
                var knipex = new Knipex();
                var newEan = ean.Remove(0, 1);
                string knipexFullUrl = knipex.GetProductUrl(newEan);
                if (string.IsNullOrEmpty(knipexFullUrl))
                {
                    copiedDict[ean] = null;
                    startIndex++;
                    continue;
                }
                var unprocessedHtml = HtmlHandler.GetUnprocessedHtml("Knipex", knipexFullUrl);
                var unprocessedHtml2 = knipex.GetFinalSearchresult(unprocessedHtml, newEan);
                List<Dictionary<string, string>> fieldCollections = new List<Dictionary<string, string>>();
                if (unprocessedHtml2 == null)
                {
                    unprocessedHtml2 = unprocessedHtml;
                }
                copiedDict = knipex.GetTechData(unprocessedHtml2, ean).ToDictionary(k => k.Key, v => v.Value);

                fieldCollections.Add(knipex.GetDescription(unprocessedHtml2, ean));
                fieldCollections.Add(knipex.GetPicture(unprocessedHtml2, ean));
                //fieldCollections.Add(knipex.GetApplications(unprocessedHtml, ean));

                copiedDict = fieldCollections
                    .Where(fieldCollection => fieldCollection != null)
                    .Aggregate(copiedDict, (current, fieldCollection) => current.Union(fieldCollection).ToDictionary(k => k.Key, v => v.Value));

                Console.WriteLine(startIndex + ". " + ean + " downloaded");

                charDB.AddDic(copiedDict);

                foreach (var entry in copiedDict)
                {
                    var fieldIndex = 0;
                    if (!fieldsDict.ContainsKey(entry.Key))
                    {
                        fieldsDict.Add(entry.Key, 7 + fieldsDict.Count);
                    }

                    fieldIndex = fieldsDict[entry.Key];
                    excelSheet.Cells.WrapText = true;
                    excelSheet.Cells[1, fieldIndex].Value = entry.Key;
                    excelSheet.Cells[startIndex, fieldIndex].Value = entry.Value;
                }
                Console.WriteLine(startIndex + ". " + ean + " written");
                startIndex++;
            }

            wb.Close();
        }

        private static void ExecuteScameOperations(string excelPath, int startIndex, int endIndex)
        {
            var wb = ExcelUtilities.CreateWorkbook(excelPath);
            var excelSheet = ExcelUtilities.SelectExcelSheet(wb, "Scame");
            var eanDictionary = new Dictionary<string, string>();
            ExcelUtilities.ReadFromExcel(eanDictionary, excelSheet, startIndex, endIndex);
            
            var copiedDict = eanDictionary.ToDictionary(entry => entry.Key,
                entry => entry.Value);
            var fieldsDict = new Dictionary<string, int>();
            foreach (var ean in eanDictionary.Keys)
            {
                var scame = new Scame();
                var unprocessedHtml = HtmlHandler.GetUnprocessedHtml("Scame", ean);
                List<Dictionary<string, string> > fieldCollections = new List<Dictionary<string, string>>();
                copiedDict = scame.GetMasterData(unprocessedHtml, ean).ToDictionary(k => k.Key, v => v.Value);
                fieldCollections.Add(scame.GetTechData(unprocessedHtml, ean));
                fieldCollections.Add(scame.GetApprovals(unprocessedHtml, ean));
                fieldCollections.Add(scame.GetStandarts(unprocessedHtml, ean));
                fieldCollections.Add(scame.GetAnnexed(unprocessedHtml, ean));
                fieldCollections.Add(scame.GetPicture(unprocessedHtml, ean));

                copiedDict = fieldCollections
                    .Where(fieldCollection => fieldCollection != null)
                    .Aggregate(copiedDict, (current, fieldCollection) => current.Union(fieldCollection).ToDictionary(k => k.Key, v => v.Value));

                charDB.AddDic(copiedDict);

                Console.WriteLine(startIndex + ". " + ean + " downloaded");

                foreach (var entry in copiedDict)
                {
                    var fieldIndex = 0;
                    if (!fieldsDict.ContainsKey(entry.Key))
                    {
                        fieldsDict.Add(entry.Key, 7 + fieldsDict.Count);
                    }

                    fieldIndex = fieldsDict[entry.Key];
                    excelSheet.Cells.WrapText = true;
                    excelSheet.Cells[1, fieldIndex].Value = entry.Key;
                    excelSheet.Cells[startIndex, fieldIndex].Value = entry.Value;
                }
                Console.WriteLine(startIndex + ". " + ean + " written");
                startIndex++;
            }

            wb.Close();
        }
    }
}
