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
            try
            {
                var zip = ZipFile.Read(filename);
                zip.ExtractProgress += zip_ExtractProgress;
                context = SynchronizationContext.Current;
                new Thread(
                    delegate () {
                        ExtractAsync(path, zip);
                    }).Start();
            }
            catch (Exception e)
            {
                Loger.addLog(e.ToString(),true);
            }
        }

        /// <summary>
        /// ����� ���������� ���� ������ � ��������� �����.
        /// </summary>
        /// <param name="to">����� � ������� ����� ���������� �����.</param>
        /// <param name="zip">��������� ������ ZipFile, �� �������� ����� ���������� ����������.</param>
        void ExtractAsync(string to, ZipFile zip)
        {
            try
            {
                zip.ExtractAll(to, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (Exception e)
            {
                Loger.addLog(e.ToString(), true);
            }
            finally
            {
                zip.Dispose();
            }
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
                            },
                            null
                        );
                    break;
            }
        }

        public void getNOD32Update(String url)
        {
            String result = "Start";
            Text.Txt = @"�������� ����� �����/������";
            new Thread(() =>
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.Encoding = Encoding.UTF8;
                        result = client.DownloadString(url);
                    }
                    catch (Exception e)
                    {
                        Loger.addLog(e.ToString(), true);
//                        throw;
                    }
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Text.Txt = result;
                    String login = "";
                    String pass = "";

                    try
                    {
                        var parser = new HtmlParser();
                        IHtmlDocument document = parser.Parse(result);
                        IHtmlCollection<IElement> collection = document.GetElementsByClassName("spoiler-body");
                        var str = collection.ElementAt(0).ChildNodes[1];
                        login = str.ChildNodes[1].TextContent;
                        pass = str.ChildNodes[5].TextContent;
                    }
                    catch (Exception e)
                    {
                        Loger.addLog(e.ToString(),true);
                        Environment.Exit(0);
                    }

                    using (var client = new WebClient())
                    {
                        // ���������� ��������� �������� �����
                        client.DownloadProgressChanged += (s, e) =>
                        {
                            Text.Txt = Convert.ToString("Download nod32 update virus data base : " + e.ProgressPercentage + "%");
                        };

                        // �������� ����� ���� ��� ������� ����� ���������
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
                            catch (System.IO.IOException ee)
                            {
                                Loger.addLog(ee.ToString(), true);
                                Environment.Exit(0);
                            }
                        };
                        try
                        {
                            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1")
                                .GetBytes($"{login.Replace(" ", string.Empty)}:{pass.Replace(" ", string.Empty)}"));
                            client.Headers.Add("Authorization: Basic " + encoded);
                            client.DownloadFileAsync(new Uri(@"http://download.eset.com/download/engine/ess/offline_update_ess.zip"), @"offline_update_ess.zip");
                        }
                        catch (Exception e)
                        {
                            Loger.addLog(e.ToString(), true);
                            Environment.Exit(0);
                        }
                    }
                }));
            }).Start();
        }
    }
}