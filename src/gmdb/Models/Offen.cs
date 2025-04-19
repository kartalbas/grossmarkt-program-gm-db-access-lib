namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class Offen : GmBase, IEnumerator
    {
        #region private properties

        private Offen[] _aobjEntities;

        #endregion

        #region constructor

        public Offen(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.OFFEN)
        {
            KontoNr = GmDb.ALL;
            RechnungsNr = GmDb.ALL;
        }

        public Offen(Int32 iKontoNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            KontoNr = iKontoNr;
        }

        public Offen(Int32 iKontoNr, Int32 iRechnungsNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            KontoNr = iKontoNr;
            RechnungsNr = iRechnungsNr;
        }

        #endregion

        #region public methods
        public Offen DeepCopy
        {
            get
            {
                var objClone = (Offen)this.MemberwiseClone();
                return objClone;
            }
        }

        public DataTable Add(Offen objEntity)
        {
            DataRow objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.RechnungsNr;
            objDataRow["c3"] = objEntity.Rechnungsdatum;
            objDataRow["c4"] = objEntity.Mahnungsdatum;
            objDataRow["c5"] = objEntity.Rechnungsbetrag;
            objDataRow["c6"] = objEntity.Offenerbetrag;
            objDataRow["c7"] = objEntity.Mahnungsstufe;
            objDataRow["c8"] = objEntity.Unbekannt1;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            Entities.Rows.Add(objDataRow);

            return Entities;
        }

        public void Save()
        {
            try
            {
                WriteEntities();
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public void Save(Offen objEntity)
        {
            try
            {
                var objResult = Unwrap(objEntity);
                Entities.Rows.Add(objResult);

                WriteEntities();
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private void WriteEntities()
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

        public IEnumerable<Offen> Read()
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
                return GmDb.Read(TableType, GmFile, string.Empty, KontoNr, RechnungsNr, GmDb.ALL);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private IEnumerable<Offen> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Offen[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
                _aobjEntities[iRow] = objEntity;
                yield return objEntity;
            }
        }

        private DataRow Unwrap(Offen objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.RechnungsNr;
            objDataRow["c3"] = objEntity.Rechnungsdatum;
            objDataRow["c4"] = objEntity.Mahnungsdatum;
            objDataRow["c5"] = objEntity.Rechnungsbetrag;
            objDataRow["c6"] = objEntity.Offenerbetrag;
            objDataRow["c7"] = objEntity.Mahnungsstufe;
            objDataRow["c8"] = objEntity.Unbekannt1;
            objDataRow["FILENAME"] = objEntity.File;
            objDataRow["ROW"] = objEntity.FileId;
            return objDataRow;
        }

        private Offen Wrap(DataRow objDataRow)
        {
            var objEntity = new Offen(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                RechnungsNr = Convert.ToInt32(objDataRow["c2"]),
                Rechnungsdatum = (DateTime)objDataRow["c3"],
                Mahnungsdatum = (DateTime)objDataRow["c4"],
                Rechnungsbetrag = Convert.ToDecimal(objDataRow["c5"]),
                Offenerbetrag = Convert.ToDecimal(objDataRow["c6"]),
                Mahnungsstufe = Convert.ToInt32(objDataRow["c7"]),
                Unbekannt1 = Convert.ToInt32(objDataRow["c8"]),
                File = objDataRow["FILENAME"].ToString(),
                FileId = Convert.ToInt32(objDataRow["ROW"])
            };

            return objEntity;
        }

        #endregion

        #region IEnumerator, IEnumerable implementation

        public IEnumerable<Offen> GetEnumartor()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

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

        private int _rechnungsNr;
        [DataMember]
        public int RechnungsNr
        {
            get { return _rechnungsNr; }
            set { if ((_rechnungsNr != value)) { SendPropertyChanging(); _rechnungsNr = value; SendPropertyChanged(); } }
        }

        private DateTime _rechnungsdatum;
        [DataMember]
        public DateTime Rechnungsdatum
        {
            get { return _rechnungsdatum; }
            set { if ((_rechnungsdatum != value)) { SendPropertyChanging(); _rechnungsdatum = value; SendPropertyChanged(); } }
        }

        private DateTime _mahnungsdatum;
        [DataMember]
        public DateTime Mahnungsdatum
        {
            get { return _mahnungsdatum; }
            set { if ((_mahnungsdatum != value)) { SendPropertyChanging(); _mahnungsdatum = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungsbetrag;
        [DataMember]
        public decimal Rechnungsbetrag
        {
            get { return _rechnungsbetrag; }
            set { if ((_rechnungsbetrag != value)) { SendPropertyChanging(); _rechnungsbetrag = value; SendPropertyChanged(); } }
        }

        private decimal _offenerbetrag;
        [DataMember]
        public decimal Offenerbetrag
        {
            get { return _offenerbetrag; }
            set { if ((_offenerbetrag != value)) { SendPropertyChanging(); _offenerbetrag = value; SendPropertyChanged(); } }
        }

        private int _mahnungsstufe;
        [DataMember]
        public int Mahnungsstufe
        {
            get { return _mahnungsstufe; }
            set { if ((_mahnungsstufe != value)) { SendPropertyChanging(); _mahnungsstufe = value; SendPropertyChanged(); } }
        }

        private int _unbekannt1;
        [DataMember]
        public int Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
