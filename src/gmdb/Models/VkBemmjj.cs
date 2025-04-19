namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    [DataContract]
    public class VkBemmjj : GmBase, IEnumerator
    {
        #region private properties

        private VkBemmjj[] _aobjEntities;

        #endregion

        #region constructor

        public VkBemmjj(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.VKBEMMJJ)
        {
            Monate = 0;
            Monat = 0;
            Jahr = 0;
            KontoNr = (short)GmDb.ALL;
        }

        public VkBemmjj(short sMonat, short sJahr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            Monat = sMonat;
            Jahr = sJahr;
        }

        public VkBemmjj(short sMonat, short sJahr, int iKontoNr, string strGmPath, string strGmUserData)
            : this(sMonat, sJahr, strGmPath, strGmUserData)
        {
            this.KontoNr = iKontoNr;
        }

        public VkBemmjj(DateTime dtBeforeDate, short sMonate, string strGmPath, string strGmUserData)
            : this(0, 0, 0, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
        }

        public VkBemmjj(DateTime dtBeforeDate, short sMonate, int iKontoNr, string strGmPath, string strGmUserData)
            : this(0, 0, iKontoNr, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
        }

        #endregion

        #region public methods

        public VkBemmjj DeepCopy
        {
            get
            {
                var objClone = (VkBemmjj)this.MemberwiseClone();
                return objClone;
            }
        }

        public string[] GetAllArchivedFiles()
        {
            return Converters.GetArchiveFile("VKBE*.DAT");
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


        public IEnumerable<VkBemmjj> Read()
        {
            try
            {
                GmFiles = Monate > 0
                    ? Converters.GetArchiveFiles(Files.VKBEmmjj, BeforeDate, Monate)
                    : new string[1] { Converters.GetArchiveFile(Files.VKBEmmjj, Monat, Jahr) };
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

                _aobjEntities = new VkBemmjj[dtEntities.Rows.Count];

                for (int iRow = 0; iRow < dtEntities.Rows.Count; iRow++)
                {
                    var objDataRow = dtEntities.Rows[iRow];
                    var objEntity = Wrap(objDataRow);
                    objEntity.Konto = new Konten(objEntity.KontoNr, GmPath, GmUserData).Read().FirstOrDefault();
                    _aobjEntities[iRow] = objEntity;
                    yield return objEntity;
                }
            }
        }

        private DataTable ReadEntities(string strFile)
        {
            try
            {
                return KontoNr > 0
                    ? GmDb.Read(TableType, strFile, string.Empty, KontoNr)
                    : GmDb.Read(TableType, strFile, string.Empty);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private VkBemmjj Wrap(DataRow objDataRow)
        {
            var objEntity = new VkBemmjj(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                PersonalNr = Convert.ToInt16(objDataRow["c2"]),
                KontrollNr = Convert.ToInt32(objDataRow["c3"]),
                Belegart = Convert.ToInt16(objDataRow["c4"]),
                Belegnummer = Convert.ToInt32(objDataRow["c5"]),
                Belegdatum = (DateTime) (objDataRow["c6"]),
                Unbekannt3 = Convert.ToDecimal(objDataRow["c7"]),
                NettobetragLiefereschein = Convert.ToDecimal(objDataRow["c8"]),
                MwstBetrag = Convert.ToDecimal(objDataRow["c9"]),
                BruttoRechnungsbetrag = Convert.ToDecimal(objDataRow["c10"]),
                NettoGewinnBetrag = Convert.ToDecimal(objDataRow["c11"]),
                Unbekannt4 = Convert.ToDecimal(objDataRow["c12"]),
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

        private int _kontoNr;
        [DataMember]
        public int KontoNr
        {
            get { return _kontoNr; }
            set { if ((_kontoNr != value)) { SendPropertyChanging(); _kontoNr = value; SendPropertyChanged(); } }
        }

        private short _personalNr;
        [DataMember]
        public short PersonalNr
        {
            get { return _personalNr; }
            set { if ((_personalNr != value)) { SendPropertyChanging(); _personalNr = value; SendPropertyChanged(); } }
        }

        private int _kontrollNr;
        [DataMember]
        public int KontrollNr
        {
            get { return _kontrollNr; }
            set { if ((_kontrollNr != value)) { SendPropertyChanging(); _kontrollNr = value; SendPropertyChanged(); } }
        }

        private short _belegart;
        [DataMember]
        public short Belegart
        {
            get { return _belegart; }
            set { if ((_belegart != value)) { SendPropertyChanging(); _belegart = value; SendPropertyChanged(); } }
        }

        private int _belegnummer;
        [DataMember]
        public int Belegnummer
        {
            get { return _belegnummer; }
            set { if ((_belegnummer != value)) { SendPropertyChanging(); _belegnummer = value; SendPropertyChanged(); } }
        }

        private DateTime _belegdatum;
        [DataMember]
        public DateTime Belegdatum
        {
            get { return _belegdatum; }
            set { if ((_belegdatum != value)) { SendPropertyChanging(); _belegdatum = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt3;
        [DataMember]
        public decimal Unbekannt3
        {
            get { return _unbekannt3; }
            set { if ((_unbekannt3 != value)) { SendPropertyChanging(); _unbekannt3 = value; SendPropertyChanged(); } }
        }

        private decimal _nettobetragLiefereschein;
        [DataMember]
        public decimal NettobetragLiefereschein
        {
            get { return _nettobetragLiefereschein; }
            set { if ((_nettobetragLiefereschein != value)) { SendPropertyChanging(); _nettobetragLiefereschein = value; SendPropertyChanged(); } }
        }

        private decimal _mwstBetrag;
        [DataMember]
        public decimal MwstBetrag
        {
            get { return _mwstBetrag; }
            set { if ((_mwstBetrag != value)) { SendPropertyChanging(); _mwstBetrag = value; SendPropertyChanged(); } }
        }

        private decimal _bruttoRechnungsbetrag;
        [DataMember]
        public decimal BruttoRechnungsbetrag
        {
            get { return _bruttoRechnungsbetrag; }
            set { if ((_bruttoRechnungsbetrag != value)) { SendPropertyChanging(); _bruttoRechnungsbetrag = value; SendPropertyChanged(); } }
        }

        private decimal _nettoGewinnBetrag;
        [DataMember]
        public decimal NettoGewinnBetrag
        {
            get { return _nettoGewinnBetrag; }
            set { if ((_nettoGewinnBetrag != value)) { SendPropertyChanging(); _nettoGewinnBetrag = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt4;
        [DataMember]
        public decimal Unbekannt4
        {
            get { return _unbekannt4; }
            set { if ((_unbekannt4 != value)) { SendPropertyChanging(); _unbekannt4 = value; SendPropertyChanged(); } }
        }

        private Konten _konto;
        [DataMember]
        public Konten Konto
        {
            get { return _konto; }
            set { if ((_konto != value)) { SendPropertyChanging(); _konto = value; SendPropertyChanged(); } }
        }
        #endregion
    }
}

