using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace CatalogueDownloader.Utilities
{
    class HtmlHandler
    {
        public static string GetPageByLink(string urlAddress)
        {
            //System.Threading.Thread.Sleep(5000);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(urlAddress);
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) return null;
                var receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                readStream = response.CharacterSet == null ? new StreamReader(receiveStream) : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                var data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return data;
            }
            catch (Exception)
            {
                throw new HttpException();
            }
            
        }

        public static string GetPageWithForm(string urlAddress, string EAN)
        {
            //System.Threading.Thread.Sleep(5000);
            var request = (HttpWebRequest)WebRequest.Create(urlAddress);
            var postData = "searchstring=" + EAN;
            var dataToPost = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dataToPost.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(dataToPost, 0, dataToPost.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK) return null;
            var receiveStream = response.GetResponseStream();
            StreamReader readStream = null;
            readStream = response.CharacterSet == null ? new StreamReader(receiveStream) : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            var data = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return data;
        }

        public static string GetUnprocessedHtml(string vendorName, string input)
        {
            string url = null;
            string unprocessedHtml = null;
            switch (vendorName)
            {
                case "Legrand":
                    url = "http://ecatalogue-export.legrand.com/catalogsearch/result/?q=" + HttpUtility.UrlEncode(input);
                    unprocessedHtml = GetPageByLink(url);
                    break;
                case "Knipex":
                    unprocessedHtml = GetPageByLink(input);
                    break;
                case "Brennenstuhl":
                    unprocessedHtml = GetPageByLink(input);
                    break;
                case "Scame":
                    url = "http://www.scame.com/en/General_Catalogue/Search/" + 
                        HttpUtility.UrlEncode(input.Replace("/", "_")) + "/-1_0000000001_cat";
                    unprocessedHtml = GetPageByLink(url);
                    break;
                default:
                    break;
            }
            
            return unprocessedHtml;
        }
    }
}
