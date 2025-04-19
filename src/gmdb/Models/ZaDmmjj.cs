namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class ZaDmmjj : GmBase, IEnumerator
    {
        #region private properties

        private ZaDmmjj[] _aobjEntities;

        #endregion

        #region constructor

        public ZaDmmjj(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.ZADMMJJ)
        {
            Monate = 0;
            Monat = 0;
            Jahr = 0;
            ZahlungBId = (short)GmDb.ALL;
        }

        public ZaDmmjj(short sMonat, short sJahr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            Monat = sMonat;
            Jahr = sJahr;
        }

        public ZaDmmjj(short sMonat, short sJahr, int sZahlungBId, string strGmPath, string strGmUserData)
            : this(sMonat, sJahr, strGmPath, strGmUserData)
        {
            this.ZahlungBId = sZahlungBId;
        }

        public ZaDmmjj(DateTime dtBeforeDate, short sMonate, string strGmPath, string strGmUserData)
            : this(0, 0, 0, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
        }

        public ZaDmmjj(DateTime dtBeforeDate, short sMonate, int sZahlungBId, string strGmPath, string strGmUserData)
            : this(0, 0, sZahlungBId, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
            this.ZahlungBId = sZahlungBId;
        }

        #endregion

        #region public methods

        public ZaDmmjj DeepCopy
        {
            get
            {
                var objClone = (ZaDmmjj)this.MemberwiseClone();
                return objClone;
            }
        }

        public IEnumerable<ZaDmmjj> Read()
        {
            try
            {
                GmFiles = Monate > 0
                    ? Converters.GetArchiveFiles(Files.ZADmmjj, BeforeDate, Monate)
                    : new string[1] { Converters.GetArchiveFile(Files.ZADmmjj, Monat, Jahr) };
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

                _aobjEntities = new ZaDmmjj[dtEntities.Rows.Count];

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
                return ZahlungBId > 0
                    ? GmDb.Read(TableType, strFile, string.Empty, ZahlungBId)
                    : GmDb.Read(TableType, strFile, string.Empty);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private ZaDmmjj Wrap(DataRow objDataRow)
        {
            var objEntity = new ZaDmmjj(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                ZahlungBId = Convert.ToInt16(objDataRow["c1"]),
                Rechnungsnummer = Convert.ToInt32(objDataRow["c2"]),
                Rechnungsdatum = (DateTime)objDataRow["c3"],
                Rechnungsbetrag = Convert.ToDecimal(objDataRow["c4"]),
                Unbekannt1 = Convert.ToDecimal(objDataRow["c5"]),
                Unbekannt2 = Convert.ToDecimal(objDataRow["c6"]),
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

        private string _mmjj;
        [DataMember]
        public string MMJJ
        {
            get { return _mmjj; }
            set { if ((_mmjj != value)) { SendPropertyChanging(); _mmjj = value; SendPropertyChanged(); } }
        }

        private int _delete;
        [DataMember]
        public int Delete
        {
            get { return _delete; }
            set { if ((_delete != value)) { SendPropertyChanging(); _delete = value; SendPropertyChanged(); } }
        }

        private int _zahlungBId;
        [DataMember]
        public int ZahlungBId
        {
            get { return _zahlungBId; }
            set { if ((_zahlungBId != value)) { SendPropertyChanging(); _zahlungBId = value; SendPropertyChanged(); } }
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

        private decimal _rechnungsbetrag;
        [DataMember]
        public decimal Rechnungsbetrag
        {
            get { return _rechnungsbetrag; }
            set { if ((_rechnungsbetrag != value)) { SendPropertyChanging(); _rechnungsbetrag = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt1;
        [DataMember]
        public decimal Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt2;
        [DataMember]
        public decimal Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
