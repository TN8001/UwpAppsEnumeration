using System.Drawing;
using System.Threading.Tasks;
using UwpAppsEnumeration;

namespace Demo.Forms
{
    // BindingSource用クラス
    public class UwpAppWrapper
    {
        public Image? Image => appListEntry.DisplayInfo.Logo;
        public string Name => appListEntry.DisplayInfo.DisplayName;
        public string Button => "起動";

        private readonly AppListEntryEx<Image> appListEntry;
        public UwpAppWrapper(AppListEntryEx<Image> entry) => appListEntry = entry;

        public Task<bool> LaunchAsync() => appListEntry.LaunchAsync();
    }
}
