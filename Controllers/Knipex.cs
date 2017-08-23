using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CatalogueDownloader.Utilities;
using HtmlAgilityPack;

namespace CatalogueDownloader.Controllers
{
    class Knipex
    {
        private string[] charExceptions =
        {
            
        };

        private int charCounter = 1;

        public string GetProductUrl(string EAN)
        {
            string url = "https://www.knipex.com/en/search/?s=" + EAN;
            try
            {
                string unprocessedHtml = HtmlHandler.GetPageWithForm(url, EAN);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("searchout").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//a[@href]").First();
                var href = result.Attributes["href"].Value;
                href = HttpUtility.HtmlDecode(href);
                return "http://www.knipex.com" + href;
            }
            catch (Exception e)
            {
                Console.WriteLine("No product found " + EAN);
            }
            return null;
        }

        public string GetFinalSearchresult(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("tabelleGroup").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//a");
                foreach (var r in result)
                {
                    var temp = Regex.Replace(r.InnerText, @"\s+", "");
                    if (!temp.Equals(ean))
                    {
                        continue;
                    }
                    var link = r.GetAttributeValue("href", string.Empty);
                    return HtmlHandler.GetUnprocessedHtml("Knipex", "https://www.knipex.com" + link.Replace("&amp;", "&"));
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("No final result " + ean);
                return null;
            }
        }

        public Dictionary<string, string> GetTechData(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("fragment-1").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//tr");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//th")[0].InnerText;
                    if (name.Trim().Length < 3)
                    {
                        continue;
                    }

                    if (name.ToLower().Contains("clamping width"))
                    {
                        dic.Add("Clamping width",
                        innerElement.DocumentNode.SelectNodes("//td")[1].InnerText);
                        continue;
                    }

                    if (name.ToLower().Contains("capacit") || name.ToLower().Contains("dimension exterior") || name.ToLower().Contains("wire stripping"))
                    {
                        dic.Add("Product characteristic " + charCounter,
                        name + ": " + innerElement.DocumentNode.SelectNodes("//td")[1].InnerText);
                        charCounter++;
                    }
                    else
                    {
                        dic.Add(name,
                        innerElement.DocumentNode.SelectNodes("//td")[1].InnerText);
                    }
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No tech data " + ean);
                Console.WriteLine(e.Message);
                return new Dictionary<string, string>();
            }
        }

        public Dictionary<string, string> GetDescription(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("fragment-2").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//li");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    dic.Add("Product characteristic " + charCounter,
                        r.InnerText);
                    charCounter++;
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No description " + ean);
                return null;
            }
        }

        public Dictionary<string, string> GetApplications(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("fragment-3").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//tr");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//th")[0].InnerText;
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name,
                        innerElement.DocumentNode.SelectNodes("//tr")[1].InnerText);
                    }
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No tech data " + ean);
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
                    .SelectNodes("//*[contains(@class,'lightbox zoom')]")[0]
                    .GetAttributeValue("href", string.Empty);

                var dic = new Dictionary<string, string>();
                dic.Add("PictureURL", "https://www.knipex.com" + res);
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
