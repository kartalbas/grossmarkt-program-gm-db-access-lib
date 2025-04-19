namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Serialization;

    using gmdb.Core;

    [DataContract]
    public class EkWare : GmBase, IEnumerator
    {
        #region private properties

        private EkWare[] _aobjEntities;

        #endregion

        #region constructor

        public EkWare(int iBelegId, int iArtikelNr, short iChargenNr, int iKontoNr, short iPositionsNr, DateTime dtBelegdatum, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.EKWARE)
        {
            BelegeId = iBelegId;
            ArtikelNr = iArtikelNr;
            ChargenNr = iChargenNr;
            KontoNr = iKontoNr;
            Positionsnummer = iPositionsNr;
            Belegdatum = dtBelegdatum;
        }

        public EkWare(int iBelegId, int iArtikelNr, short iChargenNr, int iKontoNr, short iPositionsNr, string strGmPath, string strGmUserData)
            : this(iBelegId, iArtikelNr, iChargenNr, iKontoNr, iPositionsNr, default(DateTime), strGmPath, strGmUserData)
        {
        }

        public EkWare(int iBelegId, int iArtikelNr, short iChargenNr, int iKontoNr, string strGmPath, string strGmUserData)
            : this(iBelegId, iArtikelNr, iChargenNr, iKontoNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public EkWare(int iBelegId, int iArtikelNr, short iChargenNr, string strGmPath, string strGmUserData)
            : this(iBelegId, iArtikelNr, iChargenNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public EkWare(int iBelegId, int iArtikelNr, string strGmPath, string strGmUserData)
            : this(iBelegId, iArtikelNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public EkWare(int iBelegId, string strGmPath, string strGmUserData)
            : this(iBelegId, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public EkWare(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        #endregion

        #region public methods

        public EkWare DeepCopy
        {
            get
            {
                var objClone = (EkWare)this.MemberwiseClone();
                return objClone;
            }
        }

        public EkWare Add(EkWare objEntity)
        {
            Entities.Rows.Add(Unwrap(objEntity));
            return this;
        }

        public int Count
        {
            get { return Entities.Rows.Count; }
        }

        public EkWare AtPosition(int iPosition)
        {
            try
            {
                if (Entities.Rows.Count - 1 <= iPosition)
                    throw new Exception($"Position {iPosition} in Entities is out of range");

                return Wrap(Entities.Rows[iPosition]);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public EkWare Create(int iWarenNr)
        {
            try
            {
                var objArticle = new Waren(iWarenNr, GmPath, GmUserData)
                    .Read()
                    .FirstOrDefault();

                if (objArticle == null)
                {
                    throw new Exception($"Article {iWarenNr} can't be found");
                }

                var objEkWare = new EkWare(GmPath, GmUserData);
                decimal dInhalt = objArticle.Inhalt;
                objEkWare.ChargenNr = 0;
                objEkWare.Inhalt = dInhalt;
                objEkWare.LieferscheinKolli = 1;
                objEkWare.LieferscheinMenge = dInhalt;
                objEkWare.RechnungKolli = 1;
                objEkWare.RechnungMenge = dInhalt;
                objEkWare.LieferscheinSummeNetto = dInhalt * objArticle.LetzterEK;
                objEkWare.RechnungSummeNetto = dInhalt * objArticle.LetzterEK;

                return objEkWare;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private void UpdateZuletzt()
        {
            foreach (DataRow objRow in Entities.Rows)
            {
                var objEkWare = Wrap(objRow);

                decimal dAmount = objEkWare.LieferscheinMenge;
                decimal dInhalt = objEkWare.Inhalt;
                decimal dKolli = objEkWare.LieferscheinKolli;
                decimal dVkPrice = objEkWare.LieferscheinSummeNetto / dAmount;
                int iArtikelNr = objEkWare.ArtikelNr;
                int iKontoNr = objEkWare.KontoNr;
                DateTime objDatum = objEkWare.Lieferscheindatum;
                var objZuletzt = new Zuletzt(iKontoNr, iArtikelNr, objDatum, dKolli, dInhalt, dVkPrice, GmPath, GmUserData);
                objZuletzt.Add(objZuletzt);
                objZuletzt.Save();
            }
        }

        public void Save()
        {
            try
            {
                WriteEntities();
                // UpdateZuletzt();
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public void Save(EkWare objEntity)
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

        public IEnumerable<EkWare> Read()
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

        private IEnumerable<EkWare> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new EkWare[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
                _aobjEntities[iRow] = objEntity;
                yield return objEntity;
            }
        }

        public int UpdateBelegeId(EkBeleg objEkBeleg)
        {
            try
            {
                int iBelegeId = objEkBeleg.FileId + 1;

                for (int iIndex = 0; iIndex < Entities.Rows.Count; iIndex++)
                {
                    Entities.Rows[iIndex]["c2"] = iBelegeId;
                }

                return iBelegeId;
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
                {
                    throw new Exception("No data to save, Entities is null!");
                }

                GmDb.Write(TableType, GmFile, Entities);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataTable ReadEntities()
        {
            return GmDb.Read(TableType, GmFile, string.Empty, BelegeId, ArtikelNr, ChargenNr, KontoNr, Positionsnummer, Converters.DateToIntYYYYMMDD(Belegdatum));
        }

        private DataRow Unwrap(EkWare objEntity)
        {
            DataRow objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.Positionsnummer;
            objDataRow["c2"] = objEntity.BelegeId;
            objDataRow["c3"] = objEntity.ArtikelNr;
            objDataRow["c4"] = objEntity.ChargenNr;
            objDataRow["c5"] = objEntity.KontoNr;
            objDataRow["c6"] = objEntity.BelegartId;
            objDataRow["c7"] = objEntity.Unbekannt1;
            objDataRow["c8"] = objEntity.Unbekannt2;
            objDataRow["c9"] = objEntity.Herkunftsland;
            objDataRow["c10"] = objEntity.LeerkontoId;
            objDataRow["c11"] = objEntity.Groesse;
            objDataRow["c12"] = objEntity.Marke;
            objDataRow["c13"] = objEntity.Provision;
            objDataRow["c14"] = objEntity.Belegdatum;
            objDataRow["c15"] = objEntity.Unbekannt3;
            objDataRow["c16"] = objEntity.LieferscheinNr;
            objDataRow["c17"] = objEntity.Lieferscheindatum;
            objDataRow["c18"] = objEntity.RechnungsNr;
            objDataRow["c19"] = objEntity.Rechnungsdatum;
            objDataRow["c20"] = objEntity.Unbekannt4;
            objDataRow["c21"] = objEntity.LieferscheinKolli;
            objDataRow["c22"] = objEntity.LieferscheinMenge;
            objDataRow["c23"] = objEntity.LieferscheinSummeNetto;
            objDataRow["c24"] = objEntity.RechnungKolli;
            objDataRow["c25"] = objEntity.RechnungMenge;
            objDataRow["c26"] = objEntity.RechnungSummeNetto;
            objDataRow["c27"] = objEntity.LieferscheinVerkauftKolli;
            objDataRow["c28"] = objEntity.LieferscheinVerkauftMenge;
            objDataRow["c29"] = objEntity.LieferscheinVerkauftSummeNetto;
            objDataRow["c30"] = objEntity.RechnungVerkauftKolli;
            objDataRow["c31"] = objEntity.RechnungVerkauftMenge;
            objDataRow["c32"] = objEntity.RechnungVerkauftSummeNetto;
            objDataRow["c33"] = objEntity.Unbekannt5;
            objDataRow["c34"] = objEntity.Unbekannt6;
            objDataRow["c35"] = objEntity.LagerbestandKolli;
            objDataRow["c36"] = objEntity.LagerbestandMenge;
            objDataRow["c37"] = objEntity.Umwandlungsstatus;
            objDataRow["c38"] = objEntity.Unbekannt7;
            objDataRow["c39"] = objEntity.Unbekannt8;
            objDataRow["c40"] = objEntity.Unbekannt9;
            objDataRow["c41"] = objEntity.Inhalt;
            objDataRow["c42"] = objEntity.KgProKolli;
            objDataRow["c43"] = objEntity.Unbekannt10;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private EkWare Wrap(DataRow objDataRow)
        {
            var objEntity = new EkWare(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                Positionsnummer = Convert.ToInt16(objDataRow["c1"]),
                BelegeId = Convert.ToInt32(objDataRow["c2"]),
                ArtikelNr = Convert.ToInt32(objDataRow["c3"]),
                ChargenNr = Convert.ToInt32(objDataRow["c4"]),
                KontoNr = Convert.ToInt32(objDataRow["c5"]),
                BelegartId = Convert.ToInt16(objDataRow["c6"]),
                Unbekannt1 = Convert.ToDecimal(objDataRow["c7"]),
                Unbekannt2 = Convert.ToInt16(objDataRow["c8"]),
                Herkunftsland = objDataRow["c9"].ToString().Trim(),
                LeerkontoId = Convert.ToInt16(objDataRow["c10"]),
                Groesse = objDataRow["c11"].ToString().Trim(),
                Marke = objDataRow["c12"].ToString().Trim(),
                Provision = Convert.ToDecimal(objDataRow["c13"]),
                Belegdatum = (DateTime)objDataRow["c14"],
                Unbekannt3 = (DateTime)objDataRow["c15"],
                LieferscheinNr = Convert.ToInt32(objDataRow["c16"]),
                Lieferscheindatum = (DateTime)objDataRow["c17"],
                RechnungsNr = Convert.ToInt32(objDataRow["c18"]),
                Rechnungsdatum = (DateTime)objDataRow["c19"],
                Unbekannt4 = Convert.ToDecimal(objDataRow["c20"]),
                LieferscheinKolli = Convert.ToDecimal(objDataRow["c21"]),
                LieferscheinMenge = Convert.ToDecimal(objDataRow["c22"]),
                LieferscheinSummeNetto = Convert.ToDecimal(objDataRow["c23"]),
                RechnungKolli = Convert.ToDecimal(objDataRow["c24"]),
                RechnungMenge = Convert.ToDecimal(objDataRow["c25"]),
                RechnungSummeNetto = Convert.ToDecimal(objDataRow["c26"]),
                LieferscheinVerkauftKolli = Convert.ToDecimal(objDataRow["c27"]),
                LieferscheinVerkauftMenge = Convert.ToDecimal(objDataRow["c28"]),
                LieferscheinVerkauftSummeNetto = Convert.ToDecimal(objDataRow["c29"]),
                RechnungVerkauftKolli = Convert.ToDecimal(objDataRow["c30"]),
                RechnungVerkauftMenge = Convert.ToDecimal(objDataRow["c31"]),
                RechnungVerkauftSummeNetto = Convert.ToDecimal(objDataRow["c32"]),
                Unbekannt5 = Convert.ToInt32(objDataRow["c33"]),
                Unbekannt6 = Convert.ToDecimal(objDataRow["c34"]),
                LagerbestandKolli = Convert.ToDecimal(objDataRow["c35"]),
                LagerbestandMenge = Convert.ToDecimal(objDataRow["c36"]),
                Umwandlungsstatus = objDataRow["c37"].ToString().Trim(),
                Unbekannt7 = Convert.ToInt16(objDataRow["c38"]),
                Unbekannt8 = objDataRow["c39"].ToString().Trim(),
                Unbekannt9 = objDataRow["c40"].ToString().Trim(),
                Inhalt = Convert.ToDecimal(objDataRow["c41"]),
                KgProKolli = Convert.ToDecimal(objDataRow["c42"]),
                Unbekannt10 = Convert.ToDecimal(objDataRow["c43"]),
                File = objDataRow["FILENAME"].ToString().Trim(),
                FileId = Convert.ToInt32(objDataRow["ROW"])
            };

            return objEntity;
        }

        public decimal GetEkPrice(EkWare objEkWare)
        {
            decimal dBillValue = objEkWare.RechnungSummeNetto;
            decimal dBillAmount = objEkWare.RechnungMenge;
            if (dBillAmount > 0 && dBillValue > 0)
            {
                return dBillValue / dBillAmount;
            }

            decimal dOrderValue = objEkWare.LieferscheinSummeNetto;
            decimal dOrderAmount = objEkWare.LieferscheinMenge;
            if (dOrderAmount > 0 && dOrderValue > 0)
            {
                return dOrderValue / dOrderAmount;
            }

            return 0;
        }

        public IEnumerable<EkWare> GetStock(int iArtikelNr)
        {
            var cobjResult = new EkWare(GmDb.ALL, iArtikelNr, GmPath, GmUserData)
                .Read()
                .Where(ek => ek.LagerbestandKolli > 0 && ek.LagerbestandMenge > 0)
                .OrderByDescending(ek => ek.Groesse)
                .ThenBy(ek => ek.Belegdatum);

            return cobjResult;
        }

        public IEnumerable<Tuple<int, int, DateTime>> GetAllArticlesOfSuppliers()
        {
            IEnumerable<Tuple<int, int, DateTime>> cobjResult = new EkWare(GmPath, GmUserData)
                .Read()
                .Select(a => new Tuple<int, int, DateTime>(a.ArtikelNr, a.KontoNr, a.Belegdatum));
            return cobjResult;
        }

        public IEnumerable<Tuple<int, DateTime>> GetArticlesOfSupplier(int iKontoNr)
        {
            IEnumerable<Tuple<int, DateTime>> objResult = new EkWare(GmDb.ALL, GmDb.ALL, GmDb.ALL, iKontoNr, GmPath, GmUserData)
                .Read()
                .Select(a => new Tuple<int, DateTime>(a.ArtikelNr, a.Belegdatum));

            return objResult;
        }

        public IEnumerable<Tuple<int, DateTime>> GetSuppliersOfArticle(int iArtikelNr)
        {
            IEnumerable<Tuple<int, DateTime>> objResult = new EkWare(GmDb.ALL, iArtikelNr, GmPath, GmUserData)
                .Read()
                .Select(a => new Tuple<int, DateTime>(a.KontoNr, a.Belegdatum));

            return objResult;
        }

        public Tuple<decimal, decimal> GetStockSummary(int iArtikelNr)
        {
            var cobjEkWare = new EkWare(GmDb.ALL, iArtikelNr, GmPath, GmUserData).Read().Where(ek => ek.LagerbestandKolli > 0 && ek.LagerbestandMenge > 0).OrderByDescending(ek => ek.Groesse).ThenBy(ek => ek.Belegdatum).ToList();
            decimal dSumKolli = cobjEkWare.Sum(s1 => s1.LagerbestandKolli);
            decimal dSumMenge = cobjEkWare.Sum(s1 => s1.LagerbestandMenge);
            var objResult = new Tuple<decimal, decimal>(dSumKolli, dSumMenge);
            return objResult;
        }

        public static EkWare GetLastPurchase(int iArtikelNr, List<EkWare> cobjStock)
        {
            var objResult = cobjStock.Where(a => a.ArtikelNr == iArtikelNr).OrderByDescending(ek => ek.Groesse).ThenBy(ek => ek.Belegdatum).FirstOrDefault();
            return objResult;
        }

        public static decimal GetStockSummary(int iArtikelNr, List<EkWare> cobjStock)
        {
            decimal dSumKolli = cobjStock.Where(s => s.ArtikelNr == iArtikelNr).Sum(s => s.LagerbestandKolli);
            return dSumKolli;
        }

        public EkWare CalculateStock(Belegart enmBelegart, EkWare objEkWare, ref decimal dAmount, decimal dPrice)
        {
            if (dAmount == 0)
                return null;

            if (objEkWare.LagerbestandMenge >= dAmount)
            {
                objEkWare.LagerbestandMenge -= dAmount;

                if (objEkWare.LagerbestandMenge < 0)
                    objEkWare.LagerbestandMenge = 0;

                decimal dKolli = dAmount / objEkWare.Inhalt;
                if (objEkWare.LagerbestandKolli >= dKolli)
                    objEkWare.LagerbestandKolli -= dKolli;
                else
                    objEkWare.LagerbestandKolli = 0;

                if (objEkWare.LagerbestandKolli < 0)
                    objEkWare.LagerbestandKolli = 0;

                switch (enmBelegart)
                {
                    case Belegart.Lieferschein:
                        objEkWare.LieferscheinVerkauftKolli += dKolli;
                        objEkWare.LieferscheinVerkauftMenge += dAmount;
                        objEkWare.LieferscheinVerkauftSummeNetto += dAmount * dPrice;
                        break;
                    case Belegart.Gutschrift:
                    case Belegart.Rechnung:
                        objEkWare.RechnungVerkauftKolli += dKolli;
                        objEkWare.RechnungVerkauftMenge += dAmount;
                        objEkWare.RechnungVerkauftSummeNetto += dAmount * dPrice;
                        break;
                }

                dAmount = 0;
            }
            else
            {
                decimal dRestMenge = (objEkWare.LagerbestandMenge % objEkWare.Inhalt);
                decimal dStockAmount = objEkWare.LagerbestandMenge - dRestMenge;
                decimal dKolli = dStockAmount / objEkWare.Inhalt;
                objEkWare.LagerbestandKolli = 0; //(objEkWare.LagerbestandKolli - dKolli =  0)
                objEkWare.LagerbestandMenge = dRestMenge;

                switch (enmBelegart)
                {
                    case Belegart.Lieferschein:
                        objEkWare.LieferscheinVerkauftKolli += dKolli;
                        objEkWare.LieferscheinVerkauftMenge += dAmount;
                        objEkWare.LieferscheinVerkauftSummeNetto += dAmount * dPrice;
                        break;
                    case Belegart.Gutschrift:
                    case Belegart.Rechnung:
                        objEkWare.RechnungVerkauftKolli += dKolli;
                        objEkWare.RechnungVerkauftMenge += dAmount;
                        objEkWare.RechnungVerkauftSummeNetto += dAmount * dPrice;
                        break;
                }

                dAmount -= dStockAmount;
            }

            return objEkWare;
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

        private Int16 _positionsnummer;
        [DataMember]
        public Int16 Positionsnummer
        {
            get { return _positionsnummer; }
            set { if ((_positionsnummer != value)) { SendPropertyChanging(); _positionsnummer = value; SendPropertyChanged(); } }
        }

        private int _belegeId;
        [DataMember]
        public int BelegeId
        {
            get { return _belegeId; }
            set { if ((_belegeId != value)) { SendPropertyChanging(); _belegeId = value; SendPropertyChanged(); } }
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

        private Int16 _belegartId;
        [DataMember]
        public Int16 BelegartId
        {
            get { return _belegartId; }
            set { if ((_belegartId != value)) { SendPropertyChanging(); _belegartId = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt1;
        [DataMember]
        public decimal Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private Int16 _unbekannt2;
        [DataMember]
        public Int16 Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

         private string _herkunftsland;
        [DataMember]
        public string Herkunftsland
        {
            get { return _herkunftsland; }
            set { if ((_herkunftsland != value)) { SendPropertyChanging(); _herkunftsland = value; SendPropertyChanged(); } }
        }

        private Int16 _leerkontoId;
        [DataMember]
        public Int16 LeerkontoId
        {
            get { return _leerkontoId; }
            set { if ((_leerkontoId != value)) { SendPropertyChanging(); _leerkontoId = value; SendPropertyChanged(); } }
        }

        private string _groesse;
        [DataMember]
        public string Groesse
        {
            get { return _groesse; }
            set { if ((_groesse != value)) { SendPropertyChanging(); _groesse = value; SendPropertyChanged(); } }
        }

        private string _marke;
        [DataMember]
        public string Marke
        {
            get { return _marke; }
            set { if ((_marke != value)) { SendPropertyChanging(); _marke = value; SendPropertyChanged(); } }
        }

         private decimal _provision;
        [DataMember]
        public decimal Provision
        {
            get { return _provision; }
            set { if ((_provision != value)) { SendPropertyChanging(); _provision = value; SendPropertyChanged(); } }
        }

        private DateTime _belegdatum;
        [DataMember]
        public DateTime Belegdatum
        {
            get { return _belegdatum; }
            set { if ((_belegdatum != value)) { SendPropertyChanging(); _belegdatum = value; SendPropertyChanged(); } }
        }

        private DateTime _unbekannt3;
        [DataMember]
        public DateTime Unbekannt3
        {
            get { return _unbekannt3; }
            set { if ((_unbekannt3 != value)) { SendPropertyChanging(); _unbekannt3 = value; SendPropertyChanged(); } }
        }

        private int _lieferscheinNr;
        [DataMember]
        public int LieferscheinNr
        {
            get { return _lieferscheinNr; }
            set { if ((_lieferscheinNr != value)) { SendPropertyChanging(); _lieferscheinNr = value; SendPropertyChanged(); } }
        }

        private DateTime _lieferscheindatum;
        [DataMember]
        public DateTime Lieferscheindatum
        {
            get { return _lieferscheindatum; }
            set { if ((_lieferscheindatum != value)) { SendPropertyChanging(); _lieferscheindatum = value; SendPropertyChanged(); } }
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

        private decimal _unbekannt4;
        [DataMember]
        public decimal Unbekannt4
        {
            get { return _unbekannt4; }
            set { if ((_unbekannt4 != value)) { SendPropertyChanging(); _unbekannt4 = value; SendPropertyChanged(); } }
        }

        private decimal _lieferscheinKolli;
        [DataMember]
        public decimal LieferscheinKolli
        {
            get { return _lieferscheinKolli; }
            set { if ((_lieferscheinKolli != value)) { SendPropertyChanging(); _lieferscheinKolli = value; SendPropertyChanged(); } }
        }

        private decimal _lieferscheinMenge;
        [DataMember]
        public decimal LieferscheinMenge
        {
            get { return _lieferscheinMenge; }
            set { if ((_lieferscheinMenge != value)) { SendPropertyChanging(); _lieferscheinMenge = value; SendPropertyChanged(); } }
        }

        private decimal _lieferscheinSummeNetto;
        [DataMember]
        public decimal LieferscheinSummeNetto
        {
            get { return _lieferscheinSummeNetto; }
            set { if ((_lieferscheinSummeNetto != value)) { SendPropertyChanging(); _lieferscheinSummeNetto = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungKolli;
        [DataMember]
        public decimal RechnungKolli
        {
            get { return _rechnungKolli; }
            set { if ((_rechnungKolli != value)) { SendPropertyChanging(); _rechnungKolli = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungMenge;
        [DataMember]
        public decimal RechnungMenge
        {
            get { return _rechnungMenge; }
            set { if ((_rechnungMenge != value)) { SendPropertyChanging(); _rechnungMenge = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungSummeNetto;
        [DataMember]
        public decimal RechnungSummeNetto
        {
            get { return _rechnungSummeNetto; }
            set { if ((_rechnungSummeNetto != value)) { SendPropertyChanging(); _rechnungSummeNetto = value; SendPropertyChanged(); } }
        }

        private decimal _lieferscheinVerkauftKolli;
        [DataMember]
        public decimal LieferscheinVerkauftKolli
        {
            get { return _lieferscheinVerkauftKolli; }
            set { if ((_lieferscheinVerkauftKolli != value)) { SendPropertyChanging(); _lieferscheinVerkauftKolli = value; SendPropertyChanged(); } }
        }

        private decimal _lieferscheinVerkauftMenge;
        [DataMember]
        public decimal LieferscheinVerkauftMenge
        {
            get { return _lieferscheinVerkauftMenge; }
            set { if ((_lieferscheinVerkauftMenge != value)) { SendPropertyChanging(); _lieferscheinVerkauftMenge = value; SendPropertyChanged(); } }
        }

        private decimal _lieferscheinVerkauftSummeNetto;
        [DataMember]
        public decimal LieferscheinVerkauftSummeNetto
        {
            get { return _lieferscheinVerkauftSummeNetto; }
            set { if ((_lieferscheinVerkauftSummeNetto != value)) { SendPropertyChanging(); _lieferscheinVerkauftSummeNetto = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungVerkauftKolli;
        [DataMember]
        public decimal RechnungVerkauftKolli
        {
            get { return _rechnungVerkauftKolli; }
            set { if ((_rechnungVerkauftKolli != value)) { SendPropertyChanging(); _rechnungVerkauftKolli = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungVerkauftMenge;
        [DataMember]
        public decimal RechnungVerkauftMenge
        {
            get { return _rechnungVerkauftMenge; }
            set { if ((_rechnungVerkauftMenge != value)) { SendPropertyChanging(); _rechnungVerkauftMenge = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungVerkauftSummeNetto;
        [DataMember]
        public decimal RechnungVerkauftSummeNetto
        {
            get { return _rechnungVerkauftSummeNetto; }
            set { if ((_rechnungVerkauftSummeNetto != value)) { SendPropertyChanging(); _rechnungVerkauftSummeNetto = value; SendPropertyChanged(); } }
        }

        private int _unbekannt5;
        [DataMember]
        public int Unbekannt5
        {
            get { return _unbekannt5; }
            set { if ((_unbekannt5 != value)) { SendPropertyChanging(); _unbekannt5 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt6;
        [DataMember]
        public decimal Unbekannt6
        {
            get { return _unbekannt6; }
            set { if ((_unbekannt6 != value)) { SendPropertyChanging(); _unbekannt6 = value; SendPropertyChanged(); } }
        }

        private decimal _lagerbestandKolli;
        [DataMember]
        public decimal LagerbestandKolli
        {
            get { return _lagerbestandKolli; }
            set { if ((_lagerbestandKolli != value)) { SendPropertyChanging(); _lagerbestandKolli = value; SendPropertyChanged(); } }
        }

        private decimal _lagerbestandMenge;
        [DataMember]
        public decimal LagerbestandMenge
        {
            get { return _lagerbestandMenge; }
            set { if ((_lagerbestandMenge != value)) { SendPropertyChanging(); _lagerbestandMenge = value; SendPropertyChanged(); } }
        }

        private string _umwandlungsstatus;
        [DataMember]
        public string Umwandlungsstatus
        {
            get { return _umwandlungsstatus; }
            set { if ((_umwandlungsstatus != value)) { SendPropertyChanging(); _umwandlungsstatus = value; SendPropertyChanged(); } }
        }

        private Int16 _unbekannt7;
        [DataMember]
        public Int16 Unbekannt7
        {
            get { return _unbekannt7; }
            set { if ((_unbekannt7 != value)) { SendPropertyChanging(); _unbekannt7 = value; SendPropertyChanged(); } }
        }

        private string _unbekannt8;
        [DataMember]
        public string Unbekannt8
        {
            get { return _unbekannt8; }
            set { if ((_unbekannt8 != value)) { SendPropertyChanging(); _unbekannt8 = value; SendPropertyChanged(); } }
        }

        private string _unbekannt9;
        [DataMember]
        public string Unbekannt9
        {
            get { return _unbekannt9; } 
            set { if ((_unbekannt9 != value)) { SendPropertyChanging(); _unbekannt9 = value; SendPropertyChanged(); } }
        }

        private decimal _inhalt;
        [DataMember]
        public decimal Inhalt
        {
            get { return _inhalt; }
            set { if ((_inhalt != value)) { SendPropertyChanging(); _inhalt = value; SendPropertyChanged(); } }
        }

        private decimal _kgProKolli;
        [DataMember]
        public decimal KgProKolli
        {
            get { return _kgProKolli; }
            set { if ((_kgProKolli != value)) { SendPropertyChanging(); _kgProKolli = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt10;
        [DataMember]
        public decimal Unbekannt10
        {
            get { return _unbekannt10; }
            set { if ((_unbekannt10 != value)) { SendPropertyChanging(); _unbekannt10 = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
