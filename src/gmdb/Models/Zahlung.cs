namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class Zahlung : GmBase, IEnumerator
    {
        #region private properties

        private Zahlung[] _aobjEntities;

        #endregion

        #region constructor

        public Zahlung(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.ZAHLUNG)
        {
            KontoNr = GmDb.ALL;
            Zahlungsdatum = default(DateTime);
            Rechnungsnummer = GmDb.ALL;
        }

        public Zahlung(int iKontoNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            KontoNr = iKontoNr;
        }

        public Zahlung(DateTime dtZahlungsdatum, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            Zahlungsdatum = dtZahlungsdatum;
        }

        #endregion

        #region public methods

        public Zahlung DeepCopy
        {
            get
            {
                var objClone = (Zahlung)this.MemberwiseClone();
                return objClone;
            }
        }

        public string SearchString { get; set; }

        public DataTable Add(Zahlung objEntity)
        {
            DataRow objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.Zahlungsart;
            objDataRow["c3"] = objEntity.Zahlungsdatum;
            objDataRow["c4"] = objEntity.Zahlungsbetrag;
            objDataRow["c5"] = objEntity.Schecknummer;
            objDataRow["c6"] = objEntity.Rechnungsnummer;
            objDataRow["c7"] = objEntity.Rechnungsdatum;
            objDataRow["c8"] = objEntity.Rechnungsbetrag;
            objDataRow["c9"] = objEntity.Unbekannt1;
            objDataRow["c10"] = objEntity.Unbekannt2;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            Entities.Rows.Add(objDataRow);

            return Entities;
        }

        public void Save()
        {
            try
            {
                if (Entities == null)
                    throw new Exception("No data to save, Entities is null!");

                GmDb.Write(TableType, GmFile, Entities);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }

        }

        public IEnumerable<Zahlung> Read()
        {
            try
            {
                var objResult = ReadEntities();
                return Read(objResult);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataTable ReadEntities()
        {
            try
            {
                var iDatum = GmDb.ALL;
                if (Zahlungsdatum != default(DateTime))
                    iDatum = Converters.DateToIntYYYYMMDD(Zahlungsdatum);

                return GmDb.Read(TableType, GmFile, string.Empty, KontoNr, iDatum, Rechnungsnummer);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private IEnumerable<Zahlung> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Zahlung[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
                _aobjEntities[iRow] = objEntity;
                yield return objEntity;
            }
        }

        private DataRow Unwrap(Zahlung objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.Zahlungsart;
            objDataRow["c3"] = objEntity.Zahlungsdatum;
            objDataRow["c4"] = objEntity.Zahlungsbetrag;
            objDataRow["c5"] = objEntity.Schecknummer;
            objDataRow["c6"] = objEntity.Rechnungsnummer;
            objDataRow["c7"] = objEntity.Rechnungsdatum;
            objDataRow["c8"] = objEntity.Rechnungsbetrag;
            objDataRow["c9"] = objEntity.Unbekannt1;
            objDataRow["c10"] = objEntity.Unbekannt2;
            objDataRow["FILENAME"] = objEntity.File;
            objDataRow["ROW"] = objEntity.FileId;
            return objDataRow;
        }

        private Zahlung Wrap(DataRow objDataRow)
        {
            var objEntity = new Zahlung(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                Zahlungsart = Convert.ToInt16(objDataRow["c2"]),
                Zahlungsdatum = (DateTime)objDataRow["c3"],
                Zahlungsbetrag = Convert.ToDecimal(objDataRow["c4"]),
                Schecknummer = Convert.ToInt32(objDataRow["c5"]),
                Rechnungsnummer = Convert.ToInt32(objDataRow["c6"]),
                Rechnungsdatum = (DateTime)objDataRow["c7"],
                Rechnungsbetrag = Convert.ToDecimal(objDataRow["c8"]),
                Unbekannt1 = Convert.ToDecimal(objDataRow["c9"]),
                Unbekannt2 = Convert.ToDecimal(objDataRow["c10"]),
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
