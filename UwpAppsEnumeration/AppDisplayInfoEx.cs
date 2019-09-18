using Windows.ApplicationModel;

namespace UwpAppsEnumeration
{
    // https://docs.microsoft.com/ja-jp/uwp/api/windows.applicationmodel.appdisplayinfo
    /// <summary>uwpアプリ表示情報</summary>
    /// <typeparam name="T">ロゴ画像の型</typeparam>
    public class AppDisplayInfoEx<T> where T : class
    {
        public string Description { get; }
        public string DisplayName { get; }
        public T? Logo { get; internal set; }
        public string? BackgroundColor { get; internal set; } // AppDisplayInfoにはないが AppxManifest.xmlから取得


        internal AppDisplayInfoEx(AppDisplayInfo displayInfo)
        {
            Description = displayInfo.Description;
            DisplayName = displayInfo.DisplayName;
        }
    }
}
