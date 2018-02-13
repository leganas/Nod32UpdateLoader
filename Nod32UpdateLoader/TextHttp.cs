using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Nod32UpdateLoader
{
    public class TextHttp : INotifyPropertyChanged
    {
        private string txt;

        public string Txt
        {
            get { return txt; }
            set
            {
                txt = value;
                OnPropertyChanged("Txt");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
