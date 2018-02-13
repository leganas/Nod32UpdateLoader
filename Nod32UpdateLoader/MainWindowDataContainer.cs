using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Ionic.Zip;

namespace Nod32UpdateLoader
{


    public class MainWindowDataContainer
    {
        public TextHttp Text { get; set; } = new TextHttp();
        public String zipfolder = "extract";

        public MainWindowDataContainer()
        {
            Setting.LoadSetting();
            if (Setting.current.AutoStart) getNOD32Update(@"http://progzona.ru/bezopasnost/bazae/8-bazy.html");
        }

        SynchronizationContext context;

        private void unZip(String filename, String path)
        {
            var zip = ZipFile.Read(filename);
            zip.ExtractProgress += zip_ExtractProgress;
//            progressBar1.Maximum = zip.Count;

            context = SynchronizationContext.Current;
            new Thread(
                delegate () {
                    ExtractAsync(path, zip);
                }).Start();
        }

        /// <summary>
        /// ћетод распаковки всех файлов в указанную папку.
        /// </summary>
        /// <param name="to">ѕапка в которую будет распакован архив.</param>
        /// <param name="zip">Ёкземпл€р класса ZipFile, из которого нужно произвести распаковку.</param>
        void ExtractAsync(string to, ZipFile zip)
        {
            zip.ExtractAll(to, ExtractExistingFileAction.OverwriteSilently);
            zip.Dispose();
        }

        void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    if (context != null)
                        context.Send(
                            (o) => {
                               
                                Text.Txt = string.Format(
                                    "Unpacking {0} from {1}",
                                    e.EntriesExtracted,
                                    e.EntriesTotal
                                );
                                if (e.EntriesExtracted == e.EntriesTotal) Environment.Exit(0);
//                                progressBar1.Value = e.EntriesExtracted;
                            },
                            null
                        );
                    break;
            }
        }

        public void getNOD32Update(String url)
        {
            String result = "Start";
            Text.Txt = @"ѕолучаем новые логин/пароль";
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
                        // ќбновление прогресса загрузки файла
                        client.DownloadProgressChanged += (s, e) =>
                        {
                            Text.Txt = Convert.ToString("Download nod32 update virus data base : " + e.ProgressPercentage + "%");
                        };

                        // ƒействи€ после того как закачка файла завершена
                        client.DownloadFileCompleted += (s, e) =>
                        {
                            Text.Txt = "Download complite";
                            try
                            {
                                zipfolder = Setting.current.ExtractPath;
                                //DirectoryInfo directoryinfo = new DirectoryInfo(zipfolder);
                                //if (directoryinfo.Exists) directoryinfo.Delete(true);
                                unZip(@"offline_update_ess.zip", zipfolder);
                            }
                            catch (System.IO.IOException ee) { Console.WriteLine(ee.Message); }
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