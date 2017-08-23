using System;
using System.Collections.Generic;
using System.Linq;
using CatalogueDownloader.Utilities;
using HtmlAgilityPack;

namespace CatalogueDownloader.Controllers
{
    class Brennenstuhl
    {
        private string[] charExceptions =
        {
            "this luminaire is compatible with bulbs of the energy class",
            "this luminaire is compatible with bulbs of the energy classes",
            "the luminaire is sold with a bulb of the energy class",
            "digital",
            "flexible",
            "modern",
            "1 master, 4 slave and 4 permanent sockets (alternative adjustable"
        };

        public string GetProductUrl(string EAN)
        {
            string url = "http://www.brennenstuhl.com/index.php?module=products&lang=en&index[products][action]=livesearch&index[products][pattern]=" + EAN;
            try
            {
                string unprocessedHtml = HtmlHandler.GetPageByLink(url);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                var result = doc.DocumentNode.SelectNodes("//a[@href]").First();
                var href = result.Attributes["href"].Value;
                return "https://www.brennenstuhl.com/" + href;
            }
            catch (Exception e)
            {
                Console.WriteLine("No product found!");
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
                var result = doc.DocumentNode.SelectNodes("//*[contains(@class,'page-products-details__description')]");
                var dic = new Dictionary<string, string>();
                innerElement.LoadHtml(result[0].InnerHtml);
                int productCharIndex = 1;

                foreach (var r in innerElement.DocumentNode.SelectNodes("//li"))
                {
                    var whole = r.InnerText;
                    if (whole.Contains(":"))
                    {
                        var arr = whole.Split(':');
                        if (charExceptions.Contains(arr[0].ToLower()))
                        {
                            dic.Add("Product characteristic " + productCharIndex,
                            whole);
                            productCharIndex++;
                        }
                        else
                        {
                            dic.Add(arr[0],
                            arr[1]);
                        }
                    }
                    else
                    {
                        dic.Add("Product characteristic " + productCharIndex,
                        whole);
                        productCharIndex++;
                    }
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No product found " + ean + " !");
                return null;
            }
        }

        public Dictionary<string, string> GetPicture(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                var result = doc.DocumentNode.SelectNodes("//*[contains(@class,'products-gallery__wrapper')]");
                var dic = new Dictionary<string, string>();
                innerElement.LoadHtml(result[0].InnerHtml);
                var link = innerElement.DocumentNode.SelectNodes("//a")[0].GetAttributeValue("href", string.Empty)
                    .Remove(0, 1);
                dic.Add("PictureURL", "https://www.brennenstuhl.com" + link);

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No picture found " + ean + " !");
                return null;
            }
        }

        public Dictionary<string, string> GetDownloads(string unprocessedHtml, string ean)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                var result = doc.DocumentNode.SelectNodes("//*[contains(@class,'page-products-details__downloads')]");
                var dic = new Dictionary<string, string>();
                innerElement.LoadHtml(result[0].InnerHtml);
                int downloadIndex = 1;

                foreach (var r in innerElement.DocumentNode.SelectNodes("//li"))
                {
                    var link = r.SelectNodes("//a[@href]")[0].GetAttributeValue("href", string.Empty);
                    link = "https://www.brennenstuhl.com/" + link;
                    dic.Add("Document " + downloadIndex,
                        link);
                    downloadIndex++;
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("No downloads for " + ean + " !");
                return null;
            }
        }
    }
}
