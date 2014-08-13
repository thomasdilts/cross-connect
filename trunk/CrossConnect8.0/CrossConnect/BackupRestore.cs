using Microsoft.Live;
using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace CrossConnect
{
    public class BackupRestoreConstants
    {
        public delegate void Progress(double percentTotal, double percentPartial, bool IsFinal, string debug, string Message, string MessageTranslateable1, string MessageTranslateable2);
        public const string PersistantObjectsWindowsFileName = "_Windows.xml";
        public const string PersistantObjectsThemesFileName = "_Themes.xml";
        public const string PersistantObjectsDisplaySettingsFileName = "_DisplaySettings.xml";
        public const string PersistantObjectsMarkersFileName = "_Markers.xml";
        public const string PersistantObjectsHighlightFileName = "_Highlights.xml";
    }

    public interface IBackupRestore
    {
        bool IsCanceled { get; set; }
        bool IsConnected
        {
            get;
        }

        void LogOut();
        Task<string> AuthenticateUser();

        Task DoExport(BackupRestore.BackupManifest manifest, string crossConnectFolder, BackupRestoreConstants.Progress progressCallback);
        void DoImport(BackupRestore.BackupManifest manifestSelected, string crossConnectFolder, BackupRestoreConstants.Progress progressCallback);
    }

    public class BackupRestore
    {
        [DataContract]
        public class BackupManifest
        {
            [DataMember]
            public bool bibles;

            [DataMember]
            public bool settings;

            [DataMember]
            public bool bookmarks;

            [DataMember]
            public bool themes;

            [DataMember]
            public bool highlighting;

            [DataMember]
            public bool windowSetup;

            [DataMember]
            public bool IsWindowsPhone;
        }
    }
}
