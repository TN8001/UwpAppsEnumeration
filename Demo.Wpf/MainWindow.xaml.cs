using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UwpAppsEnumeration;

namespace Demo.Wpf
{
    // xamlでジェネリックが使えないため単に包んだだけ
    public class UwpAppWrapper
    {
        public AppListEntryEx<ImageSource> Entry { get; }
        public UwpAppWrapper(AppListEntryEx<ImageSource> entry) => Entry = entry;
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<UwpAppWrapper> Apps { get; } = new ObservableCollection<UwpAppWrapper>();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                button.IsEnabled = false;
                Apps.Clear();

                await foreach(var entry in UwpApps.EnumerateAsync(toImageSource))
                {
                    Apps.Add(new UwpAppWrapper(entry));
                    Debug.WriteLine($"{entry.DisplayInfo.DisplayName}, {entry.DisplayInfo.Logo?.Width}");
                }

                button.IsEnabled = true;
            }

            static ImageSource toImageSource(Stream stream)
            {
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = stream;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();
                imageSource.Freeze();

                return imageSource;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                if(button.DataContext is UwpAppWrapper wrapper)
                {
                    _ = wrapper.Entry.LaunchAsync();
                }
            }
        }
    }
}
