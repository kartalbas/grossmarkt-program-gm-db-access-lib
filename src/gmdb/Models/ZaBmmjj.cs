namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class ZaBmmjj : GmBase, IEnumerator
    {
        #region private properties

        private ZaBmmjj[] _aobjEntities;

        #endregion

        #region constructor

        public ZaBmmjj(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.ZABMMJJ)
        {
            Monate = 0;
            Monat = 0;
            Jahr = 0;
            KontoNr = (short)GmDb.ALL;
        }

        public ZaBmmjj(short sMonat, short sJahr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            Monat = sMonat;
            Jahr = sJahr;
        }

        public ZaBmmjj(short sMonat, short sJahr, int iKontoNr, string strGmPath, string strGmUserData)
            : this(sMonat, sJahr, strGmPath, strGmUserData)
        {
            this.KontoNr = iKontoNr;
        }

        public ZaBmmjj(DateTime dtBeforeDate, short sMonate, string strGmPath, string strGmUserData)
            : this(0, 0, 0, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
        }

        public ZaBmmjj(DateTime dtBeforeDate, short sMonate, int iKontoNr, string strGmPath, string strGmUserData)
            : this(0, 0, iKontoNr, strGmPath, strGmUserData)
        {
            BeforeDate = dtBeforeDate;
            Monate = sMonate;
            KontoNr = iKontoNr;
        }

        #endregion

        #region public methods

        public ZaBmmjj DeepCopy
        {
            get
            {
                var objClone = (ZaBmmjj)this.MemberwiseClone();
                return objClone;
            }
        }

        public IEnumerable<ZaBmmjj> Read()
        {
            try
            {
                GmFiles = Monate > 0
                    ? Converters.GetArchiveFiles(Files.ZABmmjj, BeforeDate, Monate)
                    : new string[1] { Converters.GetArchiveFile(Files.ZABmmjj, Monat, Jahr) };
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

                _aobjEntities = new ZaBmmjj[dtEntities.Rows.Count];

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

        private ZaBmmjj Wrap(DataRow objDataRow)
        {
            var objEntity = new ZaBmmjj(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                Zahlungsart = Convert.ToInt16(objDataRow["c2"]),
                Zahlungsdatum = (DateTime)objDataRow["c3"],
                Zahlungsbetrag = Convert.ToDecimal(objDataRow["c4"]),
                Schecknummer = Convert.ToInt32(objDataRow["c5"]),
                ZahlungsDId = Convert.ToInt32(objDataRow["c6"]),
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

        private short _zahlungsart;
        [DataMember]
        public short Zahlungsart
        {
            get { return _zahlungsart; }
            set { if ((_zahlungsart != value)) { SendPropertyChanging(); _zahlungsart = value; SendPropertyChanged(); } }
        }

        private DateTime _zahlungsdatum;
        [DataMember]
        public DateTime Zahlungsdatum
        {
            get { return _zahlungsdatum; }
            set { if ((_zahlungsdatum != value)) { SendPropertyChanging(); _zahlungsdatum = value; SendPropertyChanged(); } }
        }

        private decimal _zahlungsbetrag;
        [DataMember]
        public decimal Zahlungsbetrag
        {
            get { return _zahlungsbetrag; }
            set { if ((_zahlungsbetrag != value)) { SendPropertyChanging(); _zahlungsbetrag = value; SendPropertyChanged(); } }
        }

        private int _schecknummer;
        [DataMember]
        public int Schecknummer
        {
            get { return _schecknummer; }
            set { if ((_schecknummer != value)) { SendPropertyChanging(); _schecknummer = value; SendPropertyChanged(); } }
        }

        private int _zahlungsDId;
        [DataMember]
        public int ZahlungsDId
        {
            get { return _zahlungsDId; }
            set { if ((_zahlungsDId != value)) { SendPropertyChanging(); _zahlungsDId = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
