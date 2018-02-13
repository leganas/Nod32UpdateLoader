using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;

namespace Nod32UpdateLoader
{


    public class MainWindowDataContainer
    {
        public TextHttp Text { get; set; } = new TextHttp();

        public MainWindowDataContainer()
        {
            getHttp("http://progzona.ru/bezopasnost/bazae/8-bazy.html");
            
        }

        private void getHttp(String url)
        {
            String result = "Проверка";
            new Thread(() =>
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    result = client.DownloadString(url);
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Text.Txt = result;

                    var parser = new HtmlParser();
                    IHtmlDocument document = parser.Parse(result);

                    IHtmlCollection<IElement> collection = document.GetElementsByClassName("spoiler-body");
                    var str = collection.ElementAt(0).ChildNodes[1];
                    String login = str.ChildNodes[1].TextContent;
                    String pass = str.ChildNodes[5].TextContent;

                    using (var client = new WebClient())
                    {
                        client.DownloadProgressChanged += (s, e) =>
                        {
                            Text.Txt = Convert.ToString(e.ProgressPercentage);
                        };
                        client.DownloadFileCompleted += (s, e) =>
                        {
                            Text.Txt = "Download complite";
                            // any other code to process the file
                        };


                        String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes($"{login.Replace(" ", string.Empty)}:{pass.Replace(" ", string.Empty)}"));
                        client.Headers.Add("Authorization: Basic " + encoded);
                        client.DownloadFileAsync(new Uri(@"http://download.eset.com/download/engine/ess/offline_update_ess.zip"), @"offline_update_ess.zip");
                    }
                }));
            }).Start();
        }
    }
}