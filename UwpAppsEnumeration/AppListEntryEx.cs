using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;

namespace UwpAppsEnumeration
{
    // https://docs.microsoft.com/ja-jp/uwp/api/windows.applicationmodel.core.applistentry
    /// <summary>uwpアプリ情報</summary>
    /// <typeparam name="T">ロゴ画像の型</typeparam>
    [DebuggerDisplay("{debuggerDisplay}")]
    public class AppListEntryEx<T> where T : class
    {
        public PackageEx Package { get; } // AppListEntryにはないが 親パッケージ
        public AppDisplayInfoEx<T> DisplayInfo { get; internal set; } = null!;
        public string AppUserModelId { get; }

        private readonly AppListEntry appListEntry;
        private string debuggerDisplay => $"{DisplayInfo.DisplayName}, {Package.Id.Name}";


        internal static async Task<AppListEntryEx<T>> CreateInstanceAsync(PackageEx package, AppListEntry appListEntry, Func<Stream, string, T> stream2Logo)
        {
            var entry = new AppListEntryEx<T>(package, appListEntry);
            await entry.InitializeAsync(stream2Logo);
            return entry;
        }

        private AppListEntryEx(PackageEx package, AppListEntry appListEntry)
        {
            Package = package;
            AppUserModelId = appListEntry.AppUserModelId;
            this.appListEntry = appListEntry;
        }

        private async Task InitializeAsync(Func<Stream, string, T> stream2Logo)
        {
            var appId = appListEntry.AppUserModelId.Substring(appListEntry.AppUserModelId.IndexOf("!") + 1);
            var (backgroundColor, square44x44Logo) = GetVisualElements(Package, appId);
            if(square44x44Logo == null) throw new Exception("Square44x44Logoがありません。");
            if(backgroundColor == null) backgroundColor = "transparent";

            var logo = await GetLogoAsync(stream2Logo, backgroundColor, square44x44Logo);
            DisplayInfo = new AppDisplayInfoEx<T>(appListEntry.DisplayInfo)
            {
                Logo = logo,
                BackgroundColor = backgroundColor,
            };
        }

        private (string? backgroundColor, string? square44x44Logo) GetVisualElements(PackageEx package, string appId)
        {
            if(package.InstalledLocation != null)
            {
                var manifestPath = Path.Combine(package.InstalledLocation, "AppxManifest.xml");
                var xml = XDocument.Load(manifestPath);

                var applications = xml.ElementAnyNS("Package")?.ElementAnyNS("Applications");
                var applicationList = applications?.ElementsAnyNS("Application");
                if(applicationList != null)
                {

                    var e = applicationList.FirstOrDefault(x => x.AttributeAnyNS("Id")?.Value == appId);
                    var bk = e.ElementAnyNS("VisualElements")?.AttributeAnyNS("BackgroundColor")?.Value;
                    var sl = e.ElementAnyNS("VisualElements")?.AttributeAnyNS("Square44x44Logo")?.Value;
                    return (bk, sl);
                }
            }

            return (null, null);
        }

        private async Task<T> GetLogoAsync(Func<Stream, string, T> stream2Logo, string backgroundColor, string square44x44Logo)
        {
            var path = Path.Combine(Package.InstalledLocation, square44x44Logo);
            if(File.Exists(path)) // Square44x44Logoがあればそのまま使う
            {
                using var stream = File.OpenRead(path);
                return stream2Logo(stream, backgroundColor);
            }

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);
            var files = Directory.GetFiles(dir, $"{name}*{ext}", SearchOption.AllDirectories)
                                 .Select(x => x.ToLower())
                                 .Where(x => !x.Contains("contrast-white"));

            path = files.FirstOrDefault(x => x.Contains("scale-100") || x.Contains("scale-200"));
            if(path != null) // scaleで探す
            {
                using var stream = File.OpenRead(path);
                return stream2Logo(stream, backgroundColor);
            }

            path = files.FirstOrDefault(x => x.Contains("targetsize-44") || x.Contains("targetsize-32"));
            if(path != null) // targetsizeで探す
            {
                using var stream = File.OpenRead(path);
                return stream2Logo(stream, backgroundColor);
            }

            // ここま見つからなければあきらめて任すが Sizeが効いてる気がしない
            // 何を入れても大体150*150（タイル用のパディングの多いもの）が返ってくる
            var streamRef = appListEntry.DisplayInfo.GetLogo(new Windows.Foundation.Size(32, 32));
            using(var randomAccessStream = await streamRef.OpenReadAsync())
            using(var stream = randomAccessStream.AsStream())
            {
                return stream2Logo(stream, backgroundColor);
            }
        }

        /// <summary>アプリを起動する</summary>
        /// <returns>起動が成功した場合true</returns>
        public async Task<bool> LaunchAsync() => await appListEntry.LaunchAsync();
    }


    internal static class XElementExtensions
    {
        public static XAttribute AttributeAnyNS<T>(this T source, string localName) where T : XElement
            => source.Attributes().SingleOrDefault(e => e.Name.LocalName == localName);

        public static XElement ElementAnyNS<T>(this T source, string localName) where T : XContainer
            => source.Elements().SingleOrDefault(e => e.Name.LocalName == localName);

        public static IEnumerable<XElement> ElementsAnyNS<T>(this T source, string localName) where T : XContainer
            => source.Elements().Where(e => e.Name.LocalName == localName);
    }
}
