using System.Text;
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
            if (string.IsNullOrEmpty(asset.System.DisplayName))
            {
                asset.System.DisplayName = asset.AssetId.SystemMetadata.Product;
            }
            if (string.IsNullOrWhiteSpace(asset.System.Description))
            {
                // Build description from system data
                var sb = new StringBuilder();
                sb.AppendLine(string.Join(' ', asset.System.Data.Caption, asset.Processors.FirstOrDefault()?.Caption));

                var props = asset.System.Data.Properties.ToDictionary(p => p.Name);

                // Memory
                var memoryString = props["Total Physical Memory"].Value;
                var memory = (decimal.TryParse(memoryString, out var v) ? v : 0m) / (decimal)1073741824.0;
                sb.AppendFormat("{0:F0}Gb RAM | ", memory);

                sb.Append(string.Join(',', props["System Type"].Value, props["Description"].Value));
                asset.System.Description = sb.ToString();
            }
            this.DataContext = asset;
        }
    }
}