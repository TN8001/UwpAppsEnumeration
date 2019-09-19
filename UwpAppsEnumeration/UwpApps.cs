using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Management.Deployment;

namespace UwpAppsEnumeration
{
    public class UwpApps
    {
        /// <summary>現ユーザーにインストールされているuwpアプリを列挙する</summary>
        /// <typeparam name="T">ロゴ画像の型</typeparam>
        /// <param name="stream2Logo">ストリームから画像を生成するデリゲート</param>
        /// <returns>アプリ情報</returns>
        public static async IAsyncEnumerable<AppListEntryEx<T>> EnumerateAsync<T>(Func<Stream, string, T> stream2Logo) where T : class
        {
            // AppListEntryが複数あった場合 同一PackageがAppListEntry分列挙される模様
            // カレンダーとメール等
            // アプリ単位で列挙されたほうが嬉しいのでAppListEntry単位にしている
            var packages = new PackageManager().FindPackagesForUser("").Distinct();

            foreach(var package in packages)
            {
                var ex = new PackageEx(package);

                foreach(var entry in await package.GetAppListEntriesAsync())
                {
                    AppListEntryEx<T> result;
                    try
                    {
                        result = await AppListEntryEx<T>.CreateInstanceAsync(ex, entry, stream2Logo);
                    }
                    catch
                    {
                        continue;
                    }

                    yield return result;
                }
            }
        }
    }
}
