namespace gmdb.Models
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public class GmBase : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static readonly PropertyChangingEventArgs _emptyChangingEventArgs = new PropertyChangingEventArgs(string.Empty);
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            this.PropertyChanging?.Invoke(this, _emptyChangingEventArgs);
        }

        protected virtual void SendPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected DataTable Entities { get; set; }

        protected static GmDb GmDb { get; set; }

        protected static Globals Globals { get; set; }

        protected TableTypes TableType { get; set; }
        [DataMember]
        protected string GmFile { get; set; }
        [DataMember]
        protected string GmPath { get; set; }
        [DataMember]
        protected string GmUserData { get; set; }

        protected string[] GmFiles { get; set; }

        public int CurrentPos { get; set; }

        protected short Monat { get; set; }

        protected short Jahr { get; set; }

        protected short Monate { get; set; }

        protected DateTime BeforeDate { get; set; }

        public GmBase(string strGmPath, string strGmUserData, TableTypes enmTableTypes)
        {
            GmPath = strGmPath;
            GmUserData = strGmUserData;

            Files.GMPath = GmPath;
            Files.GMUserdata = GmUserData;

            GmDb = GmDb.Instance(GmPath, GmUserData);
            Globals = Globals.Instance(GmPath, GmUserData);

            GmFile = string.Empty;
            GmFiles = null;

            TableType = enmTableTypes;
            Entities = GmDb.CreateTable(enmTableTypes);
            CreateDefaultValues(enmTableTypes);

            CurrentPos = 0;
        }

        public FileInfo FileInfo
        {
            get
            {
                try
                {
                    var objFileInfo = GmFile.Length == 0 ? new FileInfo(GetFilename) : new FileInfo(GmFile);
                    return objFileInfo;
                }
                catch (Exception objException)
                {
                    GmDb.Log(objException);
                    throw;
                }
            }
        }

        public string GetFilename
        {
            get
            {
                string fileType = string.Empty;

                switch(TableType)
                {
                    case TableTypes.VKBEMMJJ:
                        fileType = Files.VKBEmmjj;
                        break;
                    case TableTypes.VKWAMMJJ:
                        fileType = Files.VKWAmmjj;
                        break;
                }

                return Converters.GetArchiveFile(fileType, Monat, Jahr);
            }
        }

        private void CreateDefaultValues(TableTypes enmTableTypes)
        {
            switch (enmTableTypes)
            {
                case TableTypes.KONTEN:
                    GmFile = Files.Konten;
                    break;

                case TableTypes.FIRMA:
                    GmFile = Files.Firma;
                    break;

                case TableTypes.VKTEXTE:
                    GmFile = Files.VkTexte;
                    break;

                case TableTypes.COUNTRY:
                    GmFile = Files.Country;
                    break;

                case TableTypes.DATEIEN:
                    GmFile = Files.Dateien;
                    break;

                case TableTypes.EKBELEG:
                    GmFile = Files.EKBeleg;
                    break;

                case TableTypes.EKWARE:
                    GmFile = Files.EKWare;
                    break;

                case TableTypes.POSITION:
                    GmFile = Files.Position;
                    break;

                case TableTypes.KONTENGR2:
                    GmFile = Files.KontenGR2;
                    break;

                case TableTypes.KONTENGR3:
                    GmFile = Files.KontenGR3;
                    break;

                case TableTypes.OFFEN:
                    GmFile = Files.Offen;
                    break;

                case TableTypes.TOUR:
                    GmFile = Files.Tour;
                    break;

                case TableTypes.VKBELEG:
                    GmFile = Files.VKBeleg;
                    break;

                case TableTypes.VKBEMMJJ:
                    break;

                case TableTypes.VKWAMMJJ:
                    break;

                case TableTypes.VKWARE:
                    GmFile = Files.VKWare;
                    break;

                case TableTypes.WAREN:
                    GmFile = Files.Waren;
                    break;

                case TableTypes.WARENOGR:
                    GmFile = Files.WarenOGR;
                    break;

                case TableTypes.WARENUGR:
                    GmFile = Files.WarenUGR;
                    break;

                case TableTypes.ZABMMJJ:
                    break;

                case TableTypes.ZADMMJJ:
                    break;

                case TableTypes.ZAHLUNG:
                    GmFile = Files.Zahlung;
                    break;

                case TableTypes.ZULETZT:
                    GmFile = Files.Zuletzt;
                    break;

                case TableTypes.PROGRAM:
                    GmFile = Files.Program;
                    break;

                case TableTypes.STEUER:
                    GmFile = Files.Steuer;
                    break;
            }
        }

        public void Clear()
        {
            Entities = GmDb.CreateTable(TableType);
        }

        private int _fileId;
        [DataMember]
        public int FileId
        {
            get { return _fileId; }
            set { if ((_fileId != value)) { SendPropertyChanging(); _fileId = value; SendPropertyChanged(); } }
        }

        private string _file;
        [DataMember]
        public string File
        {
            get { return _file; }
            set { if ((_file != value)) { SendPropertyChanging(); _file = value; SendPropertyChanged(); } }
        }

        public bool TestReadable()
        {
            try
            {
                return GmDb.Accessable(GmFile, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }
        public bool TestWritable()
        {
            try
            {
                return GmDb.Accessable(GmFile, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public bool TestReadAndWritable()
        {
            try
            {
                return GmDb.Accessable(GmFile, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }
    }
}
