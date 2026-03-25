using System.Windows;
using Tudormobile.IronLedgerLib;

namespace AssetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new AssetIdFactory().Create();
        }
    }
}