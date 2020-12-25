using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrlTraceView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        List<APICall> _APICalls;
        private List<CheckboxItem> _DistinctApiCallList;
        ICollectionView _APICallsFilterView;
        Dictionary<string, CheckboxItem> dic = new Dictionary<string, CheckboxItem>();


        public List<APICall> APICalls
        {
            get
            {
                return _APICalls;
            }
            set
            {
                _APICalls = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _APICallsView;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadFile(files[0]);
            }
        }

        private void LoadFile(string file)
        {
            APICalls = APICallLoader.LoadLog(file);
            InitAPICallsFilter(APICalls);
            _APICallsView = CollectionViewSource.GetDefaultView(APICalls);
            _APICallsView.Filter = j => dic[((APICall)j).Name].Checked;// _DistinctApiCallList.Where(i=>i.Text== ((APICall)j).Name).First().Checked;
            lvAPICalls.ItemsSource = _APICallsView;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void lvAPICalls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lvAPICalls.SelectedItem as APICall;
            lvParams.ItemsSource = item?.Parameters;
        }

        void InitAPICallsFilter(List<APICall> list)
        {
            _DistinctApiCallList = list.Select(i => i.Name).Distinct().OrderBy(i => i).Select(i => new CheckboxItem(true, i)).ToList();
            _APICallsFilterView = CollectionViewSource.GetDefaultView(_DistinctApiCallList);
            lvFilter.ItemsSource = _APICallsFilterView;

            foreach (var i in _DistinctApiCallList)
                dic.Add(i.Text, i);
        }

        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = tbFilter.Text;

            if (string.IsNullOrWhiteSpace(txt))
                _APICallsFilterView.Filter = null;
            else
                _APICallsFilterView.Filter = i => CultureInfo.InvariantCulture.CompareInfo.IndexOf(((CheckboxItem)i).Text, txt, CompareOptions.IgnoreCase) >= 0;
        }

        private void CheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            _APICallsView.Refresh();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Selection";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            if (dlg.ShowDialog() == true)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var i in _DistinctApiCallList)
                {
                    if (i.Checked)
                        sb.AppendLine(i.Text);
                }

                System.IO.File.WriteAllText(dlg.FileName, sb.ToString());
            }
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    var lines = File.ReadLines(filename);
                    foreach (var line in lines)
                    {
                        foreach (var i in _DistinctApiCallList)
                        {
                            if (i.Text == line)
                                i.Checked = true;
                        }
                    }
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in _DistinctApiCallList)
            {
                i.Checked = false;
            }
        }

        private void ButtonSet_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in _DistinctApiCallList)
            {
                i.Checked = true;
            }

            lvAPICalls.ScrollIntoView(lvAPICalls.SelectedItem);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                LoadFile(openFileDialog.FileName);
        }

        private void MenuClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
