using System.Windows;
using Tudormobile.IronLedgerLib;
using Tudormobile.IronLedgerLib.UI;

namespace AssetViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // TODO: Refactor all this
            var assetId = new AssetIdFactory().Create();
            var components = new ComponentDataFactory().Create();
            var asset = new ObservableAsset(
                assetId: assetId,
                systemData: components.System,
                diskData: components.Disks,
                processorData: components.Processors,
                memoryData: components.Memory
                );
            this.DataContext = asset;
        }
    }
}