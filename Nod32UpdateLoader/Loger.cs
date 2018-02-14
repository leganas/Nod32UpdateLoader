using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.Expressions;
using System.Windows;

namespace Nod32UpdateLoader
{
    /// <summary>
    /// Реализует аналог Console , типа логи
    /// </summary>
    public class Loger
    {
        /// <summary>
        ///  Коллекция логов существующая в потоке UI
        /// </summary>
        public static ObservableCollection<String> log = new ObservableCollection<string>();

        /// <summary>
        /// Задаёт внешнюю коллекцию логирования
        /// </summary>
        /// <param name="log"></param>
        public static void setLogCollection(ObservableCollection<String> log)
        {
            Loger.log = log;
        }

        /// <summary>
        /// Добавить строку текста в лог
        /// </summary>
        /// <param name="text"></param>
        public static void addLog(String text, bool writeFromFile)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Loger.log.Insert(0, text);
                    if (log.Count > 100) log.RemoveAt(100);
                }));

                if (writeFromFile)
                {
                    DateTime localDate = DateTime.Now;
                    var culture = new CultureInfo("ru-RU");
                    System.IO.StreamWriter writer = new System.IO.StreamWriter("log.txt", true);
                    writer.WriteLine(localDate.ToString(culture) + " | " +text);
                    writer.Close();
                }
            }
            catch
            {
            }
        }
    }
}
