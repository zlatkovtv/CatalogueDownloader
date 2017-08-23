using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace CatalogueDownloader.Controllers
{
    class Scame
    {
        public Dictionary<string, string> GetMasterData(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("MainContent_UDA_DettaglioArticolo_UP_C2").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//li");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//label")[0].InnerText;
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name,
                        innerElement.DocumentNode.SelectNodes("//div")[0].InnerText);
                    }
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! EXCEPTION CAUTHT AT " + EAN + "!!!");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public Dictionary<string, string> GetTechData(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("MainContent_UDA_DettaglioArticolo_PnlDett_C3").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//li");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//label")[0].InnerText;
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name,
                        innerElement.DocumentNode.SelectNodes("//div")[0].InnerText);
                    }
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("!!! EXCEPTION CAUGHT AT " + EAN + "!!!");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public Dictionary<string, string> GetApprovals(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("MainContent_UDA_DettaglioArticolo_PnlDett_C4").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//tr");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var link = innerElement.DocumentNode.SelectNodes("//a[@href]")[1].GetAttributeValue("href", string.Empty).Remove(0, 1);
                    link = "http://www.scame.com" + link;
                    var name = innerElement.DocumentNode.SelectNodes("//a")[0].InnerText;
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name, link);
                    }
                }
                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Empty approvals " + EAN + "!!!");
                return null;
            }
        }

        public Dictionary<string, string> GetStandarts(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("MainContent_UDA_DettaglioArticolo_PnlDett_C5").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//tr");
                var dic = new Dictionary<string, string>();
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//a")[0].InnerText;
                    var link = innerElement.DocumentNode.SelectNodes("//a[@href]")[0].GetAttributeValue("href", string.Empty).Remove(0, 1);
                    link = "http://www.scame.com" + link;
                    if (!dic.ContainsKey(name))
                    {
                        dic.Add(name, link);
                    }
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Empty standarts " + EAN + "!!!");
                return null;
            }
        }

        public Dictionary<string, string> GetAnnexed(string unprocessedHtml, string EAN)
        {
            try
            {
                HtmlDocument doc = new HtmlDocument();
                HtmlDocument innerElement = new HtmlDocument();
                doc.LoadHtml(unprocessedHtml);
                string techInfo = doc.GetElementbyId("MainContent_UDA_DettaglioArticolo_PnlDett_C7").InnerHtml;
                doc.LoadHtml(techInfo);
                var result = doc.DocumentNode.SelectNodes("//tr");
                var dic = new Dictionary<string, string>();
                int count = 2;
                foreach (var r in result)
                {
                    innerElement.LoadHtml(r.InnerHtml);
                    var name = innerElement.DocumentNode.SelectNodes("//a")[0].InnerText;
                    name = name.Split(new string[] {"&nbsp;"}, StringSplitOptions.None)[0];
                    var link = innerElement.DocumentNode.SelectNodes("//a[@href]")[0].GetAttributeValue("href", string.Empty).Remove(0, 1);
                    link = "http://www.scame.com" + link;
                    if (dic.ContainsKey(name))
                    {
                        name = name + count.ToString();
                        count++;
                    }
                    dic.Add(name, link);
                }

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Empty annexed " + EAN + "!!!");
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
                string res = doc.DocumentNode.SelectNodes("//*[contains(@id,'MainContent_UDA_DettaglioArticolo_LNK_DOWNIMAGE')]")[0]
                    .GetAttributeValue("href", string.Empty);
                var dic = new Dictionary<string, string>();
                
                dic.Add("PictureURL", "http://www.scame.com" + res.Remove(0, 11));

                return dic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Empty picture " + ean + "!!!");
                return null;
            }
        }
    }
}
