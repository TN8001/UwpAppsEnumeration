using System;
using System.IO;
using Windows.ApplicationModel;

namespace UwpAppsEnumeration
{
    // https://docs.microsoft.com/ja-jp/uwp/api/windows.applicationmodel.packagesignaturekind
    public enum PackageSignatureKindEx
    {
        None,
        Developer,
        Enterprise,
        Store,
        System
    }

    // https://docs.microsoft.com/ja-jp/uwp/api/windows.applicationmodel.package
    public class PackageEx
    {
        public PackageIdEx Id { get; }
        public string? InstalledLocation { get; }
        //public StorageFolder InstalledLocation
        public bool IsFramework { get; }
        //public string Description { get; } // 常にstring.Emptyっぽい
        //public string DisplayName { get; } // 常にstring.Emptyっぽい
        public bool IsBundle { get; }
        public bool IsDevelopmentMode { get; }
        public bool IsResourcePackage { get; }
        //public Uri Logo { get; } // 常にNullReferenceExceptionが来るっぽい
        public string PublisherDisplayName { get; }
        public DateTimeOffset InstalledDate { get; }
        //public PackageStatusEx Status { get; } // 常にall falseっぽい
        public bool IsOptional { get; }
        public PackageSignatureKindEx SignatureKind { get; }
        public string? EffectiveLocation { get; }
        //public StorageFolder EffectiveLocation { get; }
        public string? MutableLocation { get; }
        //public StorageFolder MutableLocation { get; }
        //public DateTimeOffset InstallDate { get; } // 常にNotImplementedExceptionが来るっぽい


        internal PackageEx(Package package)
        {
            Id = new PackageIdEx(package.Id);
            try
            {
                InstalledLocation = package.InstalledLocation?.Path; // 時々FileNotFoundException
            }
            catch(FileNotFoundException) { }

            IsFramework = package.IsFramework;
            //Description = package.Description; // always empty
            //DisplayName = package.DisplayName; // always empty
            IsBundle = package.IsBundle;
            IsDevelopmentMode = package.IsDevelopmentMode;
            IsResourcePackage = package.IsResourcePackage;
            //Logo = package.Logo; // NullReferenceException
            PublisherDisplayName = package.PublisherDisplayName;
            InstalledDate = package.InstalledDate;
            //Status = new PackageStatusEx(package.Status); // all false
            IsOptional = package.IsOptional;
            SignatureKind = (PackageSignatureKindEx)package.SignatureKind;
            try
            {
                EffectiveLocation = package.EffectiveLocation?.Path; // 時々FileNotFoundException
            }
            catch(FileNotFoundException) { }

            try
            {
                MutableLocation = package.MutableLocation?.Path; // 時々FileNotFoundException
            }
            catch(FileNotFoundException) { }

            //InstallDate = package.InstallDate; // NotImplementedException
        }
    }
}