using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Nod32UpdateLoader
{
    class Setting
    {
        public static Setting current { get; set; } = new Setting();

        // Признак автоматического старта загрузки после запуска приложения
        public bool AutoStart { get; set; } = false;
        // Признак старта в свёрнутом режиме
        public bool HidenStart { get; set; } = false;
        // Путь до папки распаковки
        public string ExtractPath { get; set; } = "extract";

        public static void LoadSetting()
        {
            try
            {
                String global = File.ReadAllText(@"setting.json");
                Setting loadSetting = new JavaScriptSerializer().Deserialize<Setting>(global);
                Setting.current.ExtractPath = loadSetting.ExtractPath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public static void SaveSetting()
        {
            try
            {
                var global = new JavaScriptSerializer().Serialize(Setting.current);
                File.WriteAllText(@"setting.json", global, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }   
}
