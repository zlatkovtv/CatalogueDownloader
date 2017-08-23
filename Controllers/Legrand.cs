using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace CatalogueDownloader.Controllers
{
    class Legrand
    {
        private int charCounter = 1;

        public Dictionary<string, string> GetMasterData(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.DocumentNode.SelectNodes("//*[contains(@class,'general-infos')]")[0].InnerHtml;
                doc.LoadHtml(techInfo);
                var chars = doc.DocumentNode.SelectNodes("//li");
                var dic = new Dictionary<string, string>();
                foreach (var r in chars)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//span")[0].InnerText.Replace("/s+", "/s");
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name,
                            innerElement.DocumentNode.SelectNodes("//span")[1].InnerText.Replace("/s+", "/s"));
                    }
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Product not found " + EAN + "!!!");
                return null;
            }
        }

        public Dictionary<string, string> GetProductChars(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.DocumentNode.SelectNodes("//*[contains(@class,'short-description')]")[0].InnerHtml;
                doc.LoadHtml(techInfo);
                var mainChars = doc.DocumentNode.SelectNodes("//b");
                var secondaryChars = doc.DocumentNode.SelectNodes("//li");
                var allChars = (secondaryChars != null) ? mainChars.Union(secondaryChars) : mainChars;
                var dic = new Dictionary<string, string>();
                foreach (var r in allChars)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var whole = r.InnerText.Replace("/s+", "/s");

                    if (whole.Contains(":"))
                    {
                        var arr = whole.Split(':');
                        if (string.IsNullOrEmpty(arr[1].Trim()))
                        {
                            dic.Add("Product characteristic " + charCounter,
                            arr[0]);
                            charCounter++;
                        }
                        else if (arr[0].Trim().Length >= 30 || arr[0].Trim().Length <= 3)
                        {
                            dic.Add("Product characteristic " + charCounter,
                            whole);
                            charCounter++;
                        }
                        else
                        {
                            dic.Add(arr[0],
                            arr[1]);
                        }
                    }
                    else
                    {
                        dic.Add("Product characteristic " + charCounter,
                        whole);
                        charCounter++;
                    }
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Product not found " + EAN + "!!!");
                return null;
            }
        }

        public Dictionary<string, string> GetGeneralChars(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                var collection = doc.DocumentNode.SelectNodes("//*[contains(@class,'description')]");
                string techInfo = collection[1].InnerHtml;
                doc.LoadHtml(techInfo);
                var mainChars = doc.DocumentNode.SelectNodes("//b");
                var secondaryChars = doc.DocumentNode.SelectNodes("//li");
                var allChars = (secondaryChars != null) ? mainChars.Union(secondaryChars) : mainChars;
                var dic = new Dictionary<string, string>();
                foreach (var r in allChars)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var whole = r.InnerText.Replace("/s+", "/s");
                    if (whole.Contains(":"))
                    {
                        var arr = whole.Split(':');
                        if (string.IsNullOrEmpty(arr[1].Trim()))
                        {
                            dic.Add("Product characteristic " + charCounter,
                            arr[0]);
                            charCounter++;
                        }
                        else if (arr[0].Trim().Length >= 30 || arr[0].Trim().Length <= 3)
                        {
                            dic.Add("Product characteristic " + charCounter,
                            whole);
                            charCounter++;
                        }
                        else
                        {
                            dic.Add(arr[0],
                            arr[1]);
                        }
                    }
                    else
                    {
                        dic.Add("Product characteristic " + charCounter,
                        whole);
                        charCounter++;
                    }
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! EXCEPTION CAUGHT AT " + EAN + "!!!");
                return null;
            }
        }

        public Dictionary<string, string> GetMoreInfo(string unprocessedHtml, string ean, int tableNumber)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.DocumentNode.SelectNodes("//*[contains(@class,'panel')]")[tableNumber].InnerHtml;
                doc.LoadHtml(techInfo);
                var chars = doc.DocumentNode.SelectNodes("//a");
                var dic = new Dictionary<string, string>();
                int documentCounter = 1;
                foreach (var r in chars)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var link = r.GetAttributeValue("href", string.Empty);
                    dic.Add("Document " + documentCounter, link);
                    documentCounter++;
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No additional info at table " + tableNumber + " / " + ean + "!!!");
                return null;
            }
        }

        public Dictionary<string, string> GetPicture(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string res = doc.DocumentNode
                    .SelectNodes("//*[contains(@class,'cloud-zoom')]")[0]
                    .GetAttributeValue("href", string.Empty);

                var dic = new Dictionary<string, string>();
                dic.Add("PictureURL", res);
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No picture " + ean);
                return null;
            }
        }
    }
}
