namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class Zuletzt : GmBase, IEnumerator
    {
        #region private properties

        private Zuletzt[] _aobjEntities;

        #endregion

        #region constructor

        public Zuletzt(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.ZULETZT)
        {
            this.KontoNr = GmDb.ALL;
            this.ArtikelNr = GmDb.ALL;
        }

        public Zuletzt(int iKontoNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            this.KontoNr = iKontoNr;
        }

        public Zuletzt(int iKontoNr, int iArtikelNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            this.KontoNr = iKontoNr;
            this.ArtikelNr = iArtikelNr;
        }

        public Zuletzt(int iKontoNr, int iArtikelNr, DateTime objKaufdatum, decimal dKolli, decimal dInhalt, decimal dPreis, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            this.KontoNr = iKontoNr;
            this.ArtikelNr = iArtikelNr;

            this.Kaufdatum = objKaufdatum;
            this.Kolli = dKolli;
            this.Inhalt = dInhalt;
            this.Preis = dPreis;
        }

        #endregion

        #region public methods

        public Zuletzt DeepCopy
        {
            get
            {
                var objClone = (Zuletzt)this.MemberwiseClone();
                return objClone;
            }
        }

        public DataTable Add(Zuletzt objEntity)
        {
            DataRow objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.KontoNr;
            objDataRow["c1"] = objEntity.ArtikelNr;
            objDataRow["c2"] = objEntity.Kaufdatum;
            objDataRow["c3"] = objEntity.Kolli;
            objDataRow["c4"] = objEntity.Inhalt;
            objDataRow["c5"] = objEntity.Preis;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            Entities.Rows.Add(objDataRow);

            return Entities;
        }
        public Zuletzt Create(int iKontoNr, int iWarenNr)
        {
            var objZuletzt = new Zuletzt(GmPath, GmUserData)
            {
                ArtikelNr = iWarenNr,
                KontoNr = iKontoNr
            };

            return objZuletzt;
        }
        
        public void Save()
        {
            try
            {
                if(Entities == null || Entities.Rows.Count == 0)
                    throw new Exception("no object exists to write");

                GmDb.Write(TableType, GmFile, Entities);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<Zuletzt> Read()
        {
            int iKaufdatum = GmDb.ALL;

            if (Kaufdatum != default(DateTime))
                iKaufdatum = Converters.DateToIntDDMMYY(Kaufdatum);

            Entities = GmDb.Read(TableType, GmFile, string.Empty, KontoNr, ArtikelNr, iKaufdatum);

            if (Entities == null)
                yield break;
           
            _aobjEntities = new Zuletzt[Entities.Rows.Count];

            for (int iRow = 0; iRow < Entities.Rows.Count; iRow++)
            {
                var objEntity = new Zuletzt(GmPath, GmUserData);
                var objDataRow = Entities.Rows[iRow];

                objEntity.KontoNr = Convert.ToInt32(objDataRow["c0"]);
                objEntity.ArtikelNr = Convert.ToInt32(objDataRow["c1"]);
                objEntity.Kaufdatum = (DateTime)(objDataRow["c2"]);
                objEntity.Kolli = Convert.ToDecimal(objDataRow["c3"]);
                objEntity.Inhalt = Convert.ToDecimal(objDataRow["c4"]);
                objEntity.Preis = Convert.ToDecimal(objDataRow["c5"]);
                _aobjEntities[iRow] = objEntity;

                yield return objEntity;
            }
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

        private int _artikelNr;
        [DataMember]
        public int ArtikelNr
        {
            get { return _artikelNr; }
            set { if ((_artikelNr != value)) { SendPropertyChanging(); _artikelNr = value; SendPropertyChanged(); } }
        }

        private int _kontoNr;
        [DataMember]
        public int KontoNr
        {
            get { return _kontoNr; }
            set { if ((_kontoNr != value)) { SendPropertyChanging(); _kontoNr = value; SendPropertyChanged(); } }
        }

        private DateTime _kaufdatum;
        [DataMember]
        public DateTime Kaufdatum
        {
            get { return _kaufdatum; }
            set { if ((_kaufdatum != value)) { SendPropertyChanging(); _kaufdatum = value; SendPropertyChanged(); } }
        }

        private decimal _kolli;
        [DataMember]
        public decimal Kolli
        {
            get { return _kolli; }
            set { if ((_kolli != value)) { SendPropertyChanging(); _kolli = value; SendPropertyChanged(); } }
        }

        private decimal _inhalt;
        [DataMember]
        public decimal Inhalt
        {
            get { return _inhalt; }
            set { if ((_inhalt != value)) { SendPropertyChanging(); _inhalt = value; SendPropertyChanged(); } }
        }

        private decimal _preis;
        [DataMember]
        public decimal Preis
        {
            get { return _preis; }
            set { if ((_preis != value)) { SendPropertyChanging(); _preis = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
