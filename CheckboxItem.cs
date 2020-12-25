using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DrlTraceView
{
    class CheckboxItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CheckboxItem(bool check,string text)
        {
            _Checked = check;
            _Text = text;
        }

        bool _Checked;
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                OnPropertyChanged();
            }
        }


        string _Text;
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
