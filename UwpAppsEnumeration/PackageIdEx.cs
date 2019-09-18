using System;
using Windows.ApplicationModel;

namespace UwpAppsEnumeration
{
    // https://docs.microsoft.com/ja-jp/uwp/api/windows.system.processorarchitecture
    public enum ProcessorArchitectureEx
    {
        X86 = 0,
        Arm = 5,
        X64 = 9,
        Neutral = 11,
        Arm64 = 12,
        X86OnArm64 = 14,
        Unknown = 0xFFFF
    }

    // https://docs.microsoft.com/ja-jp/uwp/api/windows.applicationmodel.packageid
    public class PackageIdEx
    {
        public ProcessorArchitectureEx Architecture { get; }
        public string FamilyName { get; }
        public string FullName { get; }
        public string Name { get; }
        public string Publisher { get; }
        public string PublisherId { get; }
        public string ResourceId { get; }
        public Version Version { get; }
        //public string Author { get; } // 常にInvalidCastExceptionが来るっぽい
        //public string ProductId { get; } // 常にInvalidCastExceptionが来るっぽい

        internal PackageIdEx(PackageId packageId)
        {
            Architecture = (ProcessorArchitectureEx)packageId.Architecture;
            FamilyName = packageId.FamilyName;
            FullName = packageId.FullName;
            Name = packageId.Name;
            Publisher = packageId.Publisher;
            PublisherId = packageId.PublisherId;
            ResourceId = packageId.ResourceId;
            Version = new Version(packageId.Version.Major, packageId.Version.Minor, packageId.Version.Build, packageId.Version.Revision);
            //Author = packageId.Author; // InvalidCastException
            //ProductId = packageId.ProductId; // InvalidCastException
        }
    }
}
