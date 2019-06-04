using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace WpfApp14
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data data = new Data();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = data;
        }
    }

    public class Base : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected int Length => 10;
    }

    public class Data : Base
    {
        public Data()
        {
            Items = new ObservableCollection<ItemA>(
                Enumerable.Range(0, Length).Select(i => new ItemA(this, i))
            );
            SelectedItem = Items.First();
        }

        public ObservableCollection<ItemA> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableCollection<ItemA> _Items;

        public ItemA SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }
        private ItemA _SelectedItem;

        public ICommand OnClick
        {
            get
            {
                return _OnClick = _OnClick ?? new RelayCommand(_ =>
                {
                    var r1 = new Random(DateTime.Now.Millisecond);
                    var r2 = new Random(r1.Next(int.MaxValue));
                    var r3 = new Random(r2.Next(int.MaxValue));
                    Items[r1.Next(0, Length - 1)].Items[r2.Next(0, Length - 1)].Items[r3.Next(0, Length - 1)].OnClick.Execute(null);
                });
            }
        }
        private ICommand _OnClick;

    }

    public class ItemA : Base
    {
        public ItemA(Data parent, int i)
        {
            Parent = parent;
            Name = i.ToString();
            Items = new ObservableCollection<ItemB>(
                Enumerable.Range(0, Length).Select(j => new ItemB(this, j))
            );
            SelectedItem = Items.First();
        }

        public Data Parent { get; set; }

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private string _Name;

        public ObservableCollection<ItemB> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableCollection<ItemB> _Items;

        public ItemB SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }
        private ItemB _SelectedItem;
    }

    public class ItemB : Base
    {
        public ItemB(ItemA parent, int j)
        {
            Parent = parent;
            Name = $"{Parent.Name}-{j}";

            Items = new ObservableCollection<ItemC>(
                Enumerable.Range(0, Length).Select(k => new ItemC(this, k))
            );
            SelectedItem = Items.First();
        }

        public ItemA Parent { get; set; }

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private string _Name;

        public ObservableCollection<ItemC> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }
        private ObservableCollection<ItemC> _Items;

        public ItemC SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value); }
        }
        private ItemC _SelectedItem;

    }

    public class ItemC : Base
    {
        public ItemC(ItemB parent, int j)
        {
            Parent = parent;
            Name = $"{Parent.Name}-{j}";
        }

        public ItemB Parent { get; set; }

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private string _Name;

        public ICommand OnClick
        {
            get
            {
                return _OnClick = _OnClick ?? new RelayCommand(_ =>
                {
                    Parent.Parent.Parent.SelectedItem = Parent.Parent;
                    Parent.Parent.SelectedItem = Parent;
                    Parent.SelectedItem = this;
                });
            }
        }
        private ICommand _OnClick;
    }

    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter)) _execute(parameter);
        }
    }
}
