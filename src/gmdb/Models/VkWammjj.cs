namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    public class VkWammjj : GmBase, IEnumerator
    {
        #region private properties

        private VkWammjj[] _aobjEntities;

        #endregion

        #region constructor

        public VkWammjj(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.VKWAMMJJ)
        {
            Monate = 0;
            Monat = 0;
            Jahr = 0;
            KontoNr = (short)GmDb.ALL;
        }

        public VkWammjj(short sMonat, short sJahr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            Monat = sMonat;
            Jahr = sJahr;
            File = string.Format("VKWA{0}{1}.DAT", sMonat.ToString("D2"), sJahr.ToString("D2"));
        }

        public VkWammjj(short sMonat, short sJahr, int iArtikelNr, string strGmPath, string strGmUserData)
            : this(sMonat, sJahr, strGmPath, strGmUserData)
        {
            ArtikelNr = iArtikelNr;
        }

        public VkWammjj(DateTime dtBeforeDate, short sMonate, string strGmPath, string strGmUserData)
            : this(0, 0, 0, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
        }

        public VkWammjj(DateTime dtBeforeDate, short sMonate, int iArtikelNr, string strGmPath, string strGmUserData)
            : this(0, 0, iArtikelNr, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
        }

        #endregion

        #region public methods

        public VkWammjj DeepCopy
        {
            get
            {
                var objClone = (VkWammjj)this.MemberwiseClone();
                return objClone;
            }
        }

        public string[] GetAllArchivedFiles()
        {
            return Converters.GetArchiveFile("VKWA*.DAT");
        }

        public List<Tuple<short, short>> GetAllArchivedFilesDateIndexes()
        {
            var cobjResults = new List<Tuple<short, short>>();

            foreach (var strFile in GetAllArchivedFiles())
            {
                string strValue = Regex.Match(strFile, @"(?:\d{4})").Value;

                if (strValue.Length == 4)
                {
                    int iMonth = 0;
                    int iYear = 0;

                    int.TryParse(strValue.Substring(0, 2), out iMonth);
                    int.TryParse(strValue.Substring(2, 2), out iYear);

                    cobjResults.Add(new Tuple<short, short>((short)iMonth, (short)iYear));
                }
            }

            return cobjResults;
        }

        public Tuple<decimal, decimal> GetSoldSummary(short sMonth, short sYear)
        {
            return GetSoldSummary(sMonth, sYear, 0);
        }

        public Tuple<decimal, decimal> GetSoldSummary(short sMonth, short sYear, int iArtikelNr)
        {
            IEnumerable<VkWammjj> objRead;

            if (iArtikelNr > 0)
                objRead = new VkWammjj(sMonth, sYear, iArtikelNr, GmPath, GmUserData).Read().ToList();
            else
                objRead = new VkWammjj(sMonth, sYear, GmPath, GmUserData).Read().ToList();

            decimal dSoldParcels = objRead.Sum(s => s.KolliRechnung);
            decimal dSoldPieces = objRead.Sum(s => s.MengeRechnung);
            var objResult = new Tuple<decimal, decimal>(dSoldParcels, dSoldPieces);
            return objResult;
        }

        public static Tuple<decimal, decimal> GetSoldSummary(short sMonth, short sYear, int iArtikelNr, List<VkWammjj> cobjVkWammjj )
        {
            decimal dSoldParcels = cobjVkWammjj.Where(s => s.ArtikelNr == iArtikelNr).Sum(s => s.KolliRechnung);
            decimal dSoldPieces = cobjVkWammjj.Where(s => s.ArtikelNr == iArtikelNr).Sum(s => s.MengeRechnung);
            var objResult = new Tuple<decimal, decimal>(dSoldParcels, dSoldPieces);
            return objResult;                
        }

        public static Tuple<decimal, decimal> GetSoldSummary(int iArtikelNr, List<VkWammjj> cobjVkWammjj)
        {
            decimal dSoldParcels = cobjVkWammjj.Where(s => s.ArtikelNr == iArtikelNr).Sum(s => s.KolliRechnung);
            decimal dSoldPieces = cobjVkWammjj.Where(s => s.ArtikelNr == iArtikelNr).Sum(s => s.MengeRechnung);
            var objResult = new Tuple<decimal, decimal>(dSoldParcels, dSoldPieces);
            return objResult;
        }

        public IEnumerable<VkWammjj> Read()
        {
            try
            {
                GmFiles = Monate > 0
                    ? Converters.GetArchiveFiles(Files.VKWAmmjj, BeforeDate, Monate)
                    : new string[1] { Converters.GetArchiveFile(Files.VKWAmmjj, Monat, Jahr) };
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }

            foreach (string strFile in GmFiles)
            {
                DataTable dtEntities = ReadEntities(strFile);

                if (dtEntities == null)
                    continue;

                _aobjEntities = new VkWammjj[dtEntities.Rows.Count];

                for (int iRow = 0; iRow < dtEntities.Rows.Count; iRow++)
                {
                    var objDataRow = dtEntities.Rows[iRow];
                    var objEntity = Wrap(objDataRow);
                    _aobjEntities[iRow] = objEntity;
                    yield return objEntity;
                }
            }
        }

        private DataTable ReadEntities(string strFile)
        {
            try
            {
                return ArtikelNr > 0
                    ? GmDb.Read(TableType, strFile, string.Empty, ArtikelNr)
                    : GmDb.Read(TableType, strFile, string.Empty);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataRow Unwrap(VkWammjj objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.Positionsnummer;
            objDataRow["c2"] = objEntity.ArtikelNr;
            objDataRow["c3"] = objEntity.ChargenNr;
            objDataRow["c4"] = objEntity.KontoNr;
            objDataRow["c5"] = objEntity.Unbekannt1;
            objDataRow["c6"] = objEntity.Rabatt;
            objDataRow["c7"] = objEntity.Rueckverguetung;
            objDataRow["c8"] = objEntity.Lieferscheinnummer;
            objDataRow["c9"] = objEntity.Rechnungsnummer;
            objDataRow["c10"] = objEntity.Rechnungsdatum;
            objDataRow["c11"] = objEntity.KolliRechnung;
            objDataRow["c12"] = objEntity.MengeRechnung;
            objDataRow["c13"] = objEntity.SummeNettoRechnung;
            objDataRow["c14"] = objEntity.GewinnNetto;
            objDataRow["c15"] = objEntity.BelegeId;
            objDataRow["c16"] = objEntity.Unbekannt2;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private VkWammjj Wrap(DataRow objDataRow)
        {
            var objEntity = new VkWammjj(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                Positionsnummer = Convert.ToInt16(objDataRow["c1"]),
                ArtikelNr = Convert.ToInt32(objDataRow["c2"]),
                ChargenNr = Convert.ToInt32(objDataRow["c3"]),
                KontoNr = Convert.ToInt32(objDataRow["c4"]),
                Unbekannt1 = Convert.ToInt16(objDataRow["c5"]),
                Rabatt = Convert.ToDecimal(objDataRow["c6"]),
                Rueckverguetung = Convert.ToDecimal(objDataRow["c7"]),
                Lieferscheinnummer = Convert.ToInt32(objDataRow["c8"]),
                Rechnungsnummer = Convert.ToInt32(objDataRow["c9"]),
                Rechnungsdatum = (DateTime)objDataRow["c10"],
                KolliRechnung = Convert.ToInt32(objDataRow["c11"]),
                MengeRechnung = Convert.ToDecimal(objDataRow["c12"]),
                SummeNettoRechnung = Convert.ToDecimal(objDataRow["c13"]),
                GewinnNetto = Convert.ToDecimal(objDataRow["c14"]),
                BelegeId = Convert.ToInt32(objDataRow["c15"]),
                Unbekannt2 = Convert.ToInt16(objDataRow["c16"]),


                File = objDataRow["FILENAME"].ToString(),
                FileId = Convert.ToInt32(objDataRow["ROW"])
            };

            return objEntity;
        }

        #endregion

        #region IEnumerator, IEnumerable implementation

        object IEnumerator.Current
        {
            get { return _aobjEntities[CurrentPos]; }
        }

        bool IEnumerator.MoveNext()
        {
            return ++CurrentPos <= _aobjEntities.Length;
        }

        void IEnumerator.Reset()
        {
            CurrentPos = 0;
        }

        #endregion

        #region public bindable properties

        private int _delete;
        [DataMember]
        public int Delete
        {
            get { return _delete; }
            set { if ((_delete != value)) { SendPropertyChanging(); _delete = value; SendPropertyChanged(); } }
        }

        private short _positionsnummer;
        [DataMember]
        public short Positionsnummer
        {
            get { return _positionsnummer; }
            set { if ((_positionsnummer != value)) { SendPropertyChanging(); _positionsnummer = value; SendPropertyChanged(); } }
        }

        private int _artikelNr;
        [DataMember]
        public int ArtikelNr
        {
            get { return _artikelNr; }
            set { if ((_artikelNr != value)) { SendPropertyChanging(); _artikelNr = value; SendPropertyChanged(); } }
        }

        private int _chargenNr;
        [DataMember]
        public int ChargenNr
        {
            get { return _chargenNr; }
            set { if ((_chargenNr != value)) { SendPropertyChanging(); _chargenNr = value; SendPropertyChanged(); } }
        }

        private int _kontoNr;
        [DataMember]
        public int KontoNr
        {
            get { return _kontoNr; }
            set { if ((_kontoNr != value)) { SendPropertyChanging(); _kontoNr = value; SendPropertyChanged(); } }
        }

        private short _unbekannt1;
        [DataMember]
        public short Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private decimal _rabatt;
        [DataMember]
        public decimal Rabatt
        {
            get { return _rabatt; }
            set { if ((_rabatt != value)) { SendPropertyChanging(); _rabatt = value; SendPropertyChanged(); } }
        }

        private decimal _rueckverguetung;
        [DataMember]
        public decimal Rueckverguetung
        {
            get { return _rueckverguetung; }
            set { if ((_rueckverguetung != value)) { SendPropertyChanging(); _rueckverguetung = value; SendPropertyChanged(); } }
        }

        private int _lieferscheinnummer;
        [DataMember]
        public int Lieferscheinnummer
        {
            get { return _lieferscheinnummer; }
            set { if ((_lieferscheinnummer != value)) { SendPropertyChanging(); _lieferscheinnummer = value; SendPropertyChanged(); } }
        }

        private int _rechnungsnummer;
        [DataMember]
        public int Rechnungsnummer
        {
            get { return _rechnungsnummer; }
            set { if ((_rechnungsnummer != value)) { SendPropertyChanging(); _rechnungsnummer = value; SendPropertyChanged(); } }
        }

        private DateTime _rechnungsdatum;
        [DataMember]
        public DateTime Rechnungsdatum
        {
            get { return _rechnungsdatum; }
            set { if ((_rechnungsdatum != value)) { SendPropertyChanging(); _rechnungsdatum = value; SendPropertyChanged(); } }
        }

        private int _kolliRechnung;
        [DataMember]
        public int KolliRechnung
        {
            get { return _kolliRechnung; }
            set { if ((_kolliRechnung != value)) { SendPropertyChanging(); _kolliRechnung = value; SendPropertyChanged(); } }
        }

        private decimal _mengeRechnung;
        [DataMember]
        public decimal MengeRechnung
        {
            get { return _mengeRechnung; }
            set { if ((_mengeRechnung != value)) { SendPropertyChanging(); _mengeRechnung = value; SendPropertyChanged(); } }
        }

        private decimal _summeNettoRechnung;
        [DataMember]
        public decimal SummeNettoRechnung
        {
            get { return _summeNettoRechnung; }
            set { if ((_summeNettoRechnung != value)) { SendPropertyChanging(); _summeNettoRechnung = value; SendPropertyChanged(); } }
        }

        private decimal _gewinnNetto;
        [DataMember]
        public decimal GewinnNetto
        {
            get { return _gewinnNetto; }
            set { if ((_gewinnNetto != value)) { SendPropertyChanging(); _gewinnNetto = value; SendPropertyChanged(); } }
        }

        private int _belegeId;
        [DataMember]
        public int BelegeId
        {
            get { return _belegeId; }
            set { if ((_belegeId != value)) { SendPropertyChanging(); _belegeId = value; SendPropertyChanged(); } }
        }

        private short _unbekannt2;
        [DataMember]
        public short Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
