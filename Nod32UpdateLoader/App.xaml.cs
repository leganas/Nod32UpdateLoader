using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Nod32UpdateLoader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // If no command line arguments were provided, don't process them
            if (e.Args.Length == 0) return;

            // Get command line arguments
            foreach (string argument in e.Args)
            {
                switch (argument)
                {
                    case "/hide":
                        // Process arg 1
                        Setting.current.HidenStart = true;
                        break;
                    case "/auto":
                        // Process arg 1
                        Setting.current.AutoStart = true;
                        break;
                }
            }
        }
    }
}
