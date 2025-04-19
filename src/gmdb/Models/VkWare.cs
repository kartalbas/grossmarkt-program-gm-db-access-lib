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
    public class VkWare : GmBase, IEnumerator
    {
        #region private properties

        private VkWare[] _aobjEntities;

        #endregion

        #region Constructors

        public VkWare(int iBelegeId, int iEkWareId, int iArtikelNr, int iChargenNr, DateTime dtRechnungsdatum, short iPositionsNr, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.VKWARE)
        {
            BelegeId = iBelegeId;
            EkWareId = iEkWareId;
            ArtikelNr = iArtikelNr;
            ChargenNr = iChargenNr;
            Rechnungsdatum = dtRechnungsdatum;
            Positionsnummer = iPositionsNr;
        }

        public VkWare(int iBelegeId, int iEkWareId, int iArtikelNr, int iChargenNr, DateTime dtRechnungsdatum, string strGmPath, string strGmUserData)
            : this(iBelegeId, iEkWareId, iArtikelNr, iChargenNr, dtRechnungsdatum, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkWare(int iBelegeId, int iEkWareId, int iArtikelNr, int iChargenNr, string strGmPath, string strGmUserData)
            : this(iBelegeId, iEkWareId, iArtikelNr, iChargenNr, default(DateTime), GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkWare(int iBelegeId, int iEkWareId, int iArtikelNr, string strGmPath, string strGmUserData)
            : this(iBelegeId, iEkWareId, iArtikelNr, GmDb.ALL, default(DateTime), strGmPath, strGmUserData)
        {
        }

        public VkWare(int iBelegeId, int iEkWareId, string strGmPath, string strGmUserData)
            : this(iBelegeId, iEkWareId, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkWare(int iBelegeId, string strGmPath, string strGmUserData)
            : this(iBelegeId, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkWare(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkWare(DateTime dtRechnungsdatum, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            Rechnungsdatum = dtRechnungsdatum;
        }

        #endregion

        #region public methods

        public VkWare DeepCopy
        {
            get
            {
                var objClone = (VkWare)this.MemberwiseClone();
                return objClone;
            }
        }

        public VkWare Add(VkWare objEntity)
        {
            Entities.Rows.Add(Unwrap(objEntity));
            return this;
        }

        public int Count
        {
            get { return Entities.Rows.Count; }
        }

        public VkWare AtPosition(int iPosition)
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

        public void Save()
        {
            try
            {
                OrderPositions();
                WriteEntities();
                // UpdateZuletzt();
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<VkWare> Read()
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

        private IEnumerable<VkWare> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new VkWare[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
                _aobjEntities[iRow] = objEntity;
                yield return objEntity;
            }
        }

        private void OrderPositions()
        {
            try
            {
                for (int i = 0; i < Entities.Rows.Count; i++)
                    Entities.Rows[i][1] = i + 1;
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
                var objVkWare = Wrap(objRow);

                decimal dAmount = objVkWare.MengeLieferschein;
                decimal dInhalt = dAmount / objVkWare.KolliLieferschein;
                decimal dKolli = dInhalt > 0 ? dAmount / dInhalt : 0;
                decimal dVkPrice = objVkWare.SummeNettoLieferschein / dAmount;
                int iArtikelNr = objVkWare.Ware.ArtikelNr;
                int iKontoNr = objVkWare.KontoNr;
                DateTime objDatum = objVkWare.Lieferscheindatum;
                var objZuletzt = new Zuletzt(iKontoNr, iArtikelNr, objDatum, dKolli, dInhalt, dVkPrice, GmPath, GmUserData);
                objZuletzt.Add(objZuletzt);
                objZuletzt.Save();
            }
        }

        public decimal AdapedStock(ref VkBeleg objNewVkBeleg, ref EkWare objEkWareToWrite, ref VkWare objNewVkWareToWrite, ref short iPosNr, int iArtikelNr, decimal dPrice, decimal dAmount)
        {
            try
            {
                var objArtikel = new Waren(iArtikelNr, GmPath, GmUserData).Read().FirstOrDefault();
                if (objArtikel == null)
                {
                    GmDb.Log($"Article {iArtikelNr} not found");
                    return 0;
                }

                var objEkWaren = new EkWare(GmPath, GmUserData).GetStock(objArtikel.ArtikelNr).ToList();

                if (objEkWaren.Count > 0)
                {
                    foreach (var objEkWare in objEkWaren)
                    {
                        var dOldAmount = dAmount;

                        var objEkWareCalculated = objEkWare.CalculateStock((Belegart)objNewVkBeleg.Belegart, objEkWare, ref dAmount, dPrice);
                        if (objEkWareCalculated != null)
                        {
                            objEkWareToWrite.Add(objEkWareCalculated);
                        }

                        decimal dAmountForLieferschein = dOldAmount - dAmount;

                        if (dAmountForLieferschein > 0)
                        {
                            var objNewVkWarePosition = new VkWare(GmPath, GmUserData).Create(objNewVkBeleg, objArtikel, objEkWare, iPosNr++, dAmountForLieferschein, dPrice);
                            objNewVkWareToWrite.Add(objNewVkWarePosition);
                        }

                        if (dAmount == 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    var objNewVkWarePosition = new VkWare(GmPath, GmUserData).Create(objNewVkBeleg, objArtikel, null, iPosNr++, dAmount, dPrice);
                    objNewVkWareToWrite.Add(objNewVkWarePosition);
                }

                return dAmount;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public VkWare Create(VkBeleg objBeleg, Waren objWare, EkWare objEkWare, short iPosNr, decimal dAmount, decimal dVkPrice)
        {
            try
            {
                decimal dEkPrice = 0;

                if (objEkWare == null)
                {
                    objEkWare = new EkWare(0, objWare.ArtikelNr, string.Empty, string.Empty)
                    {
                        ChargenNr = 0,
                        Inhalt = objWare.Inhalt,
                        RechnungSummeNetto = 0,
                        RechnungMenge = 0,
                        LieferscheinSummeNetto = 0,
                        LieferscheinMenge = 0,
                        FileId = -1
                    };

                    dEkPrice = objWare.LetzterEK;
                }
                else
                {
                    dEkPrice = objEkWare.GetEkPrice(objEkWare);
                }

                decimal dSummeNetto = dAmount * dVkPrice;
                int dKolli = Convert.ToInt32(dAmount / objEkWare.Inhalt);

                var objVkWare = new VkWare(GmPath, GmUserData)
                {
                    Delete = 0,
                    Positionsnummer = iPosNr,
                    ArtikelNr = objWare.ArtikelNr,
                    ChargenNr = objEkWare.ChargenNr,
                    KontoNr = objBeleg.KontoNr,
                    Unbekannt1 = 0,
                    Rabatt = 0,
                    Rueckverguetung = 0,
                    Lieferscheinnummer = objBeleg.Belegnummer,
                    Rechnungsnummer = 0,
                    Rechnungsdatum = objBeleg.Belegdatum,
                    KolliLieferschein = dKolli,
                    MengeLieferschein = dAmount,
                    SummeNettoLieferschein = dSummeNetto,
                    GewinnNetto = dAmount * (dVkPrice - dEkPrice),
                    BelegeId = 0,
                    EkWareId = objEkWare.FileId + 1,
                    KolliRechnung = dKolli,
                    MengeRechnung = dAmount,
                    SummeNettoRechnung = dSummeNetto,
                    Lieferscheindatum = objBeleg.Belegdatum,
                    Ware = objWare
                };

                return objVkWare;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public VkWare CreateVkWare(
            Konten objKonto, 
            Waren objWare, 
            EkWare objEkWare,
            VkBeleg objVkBeleg,
            short iPositionsnummer,
            int iLieferscheinnummer,
            DateTime objLieferscheindatum,
            decimal dMengeLieferschein,
            decimal dSummeNettoLieferschein,
            int iRechnungsnummer,
            DateTime objRechnugsDatum,
            decimal dMengeRechnung,
            decimal dSummeNettoRechnung,
            decimal dGewinnNetto,
            decimal dRabat, 
            decimal dRueckverguetung
            )
        {
            try
            {
                int dKolli = Convert.ToInt32(dMengeLieferschein/objEkWare.Inhalt);

                var objVkWare = new VkWare(GmPath, GmUserData)
                {
                    Positionsnummer = 1,
                    ArtikelNr = objWare.ArtikelNr,
                    ChargenNr = objEkWare.ChargenNr,
                    KontoNr = objKonto.KontoNr,
                    Unbekannt1 = 0,
                    Rabatt = dRabat,
                    Rueckverguetung = dRueckverguetung,
                    Lieferscheinnummer = iLieferscheinnummer,
                    Rechnungsnummer = iRechnungsnummer,
                    Rechnungsdatum = objRechnugsDatum,
                    KolliLieferschein = dKolli,
                    MengeLieferschein = dMengeLieferschein,
                    SummeNettoLieferschein = dSummeNettoLieferschein,
                    GewinnNetto = dGewinnNetto,
                    BelegeId = 0,
                    EkWareId = objEkWare.FileId + 1,
                    KolliRechnung = dKolli,
                    MengeRechnung = dMengeRechnung,
                    SummeNettoRechnung = dSummeNettoRechnung,
                    Lieferscheindatum = objLieferscheindatum
                };

                return objVkWare;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public List<VkWare> VkWaren
        {
            get
            {
                var objVkWaren = new List<VkWare>();
                foreach (DataRow objEntity in Entities.Rows)
                {
                    var objVkWare = Wrap(objEntity);
                    objVkWare.Ware = new Waren(objVkWare.ArtikelNr, GmPath, GmUserData).Read().FirstOrDefault();
                    objVkWare.EkWare = new EkWare(GmDb.ALL, objVkWare.ArtikelNr, (short)objVkWare.ChargenNr, GmPath, GmUserData).Read().FirstOrDefault();

                    objVkWaren.Add(objVkWare);                    
                }

                return objVkWaren;
            }
        }

        public decimal GetNettoProfit()
        {
            try
            {
                var objSum = Convert.ToDecimal(Entities.Compute("Sum(c14)", ""));
                return objSum;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public decimal GetSumLieferscheinbetrag()
        {
            try
            {
                var objSum = Convert.ToDecimal(Entities.Compute("Sum(c13)", ""));
                return objSum;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public decimal GetMwstBetrag()
        {
            try
            {
                decimal dSummeMwSt = 0;

                foreach (DataRow objDataRow in Entities.Rows)
                {
                    int iArtikelNr = Convert.ToInt32(objDataRow["c2"]);
                    decimal dSummeNettoLieferschein = Convert.ToDecimal(objDataRow["c13"]);
                    var objArtikel = new Waren(iArtikelNr, GmPath, GmUserData).Read().FirstOrDefault();
                    dSummeMwSt += dSummeNettoLieferschein*objArtikel.Steuer;
                }

                return dSummeMwSt;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public int UpdateBelegeId(VkBeleg objVkBeleg)
        {
            try
            {
                int iBelegeId = objVkBeleg.FileId + 1;

                for (int iIndex = 0; iIndex < Entities.Rows.Count; iIndex++)
                {
                    Entities.Rows[iIndex]["c15"] = iBelegeId;
                }

                return iBelegeId;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public void UpdateBelegnummer(VkBeleg objVkBeleg)
        {
            try
            {
                for (int iIndex = 0; iIndex < Entities.Rows.Count; iIndex++)
                {
                    switch (objVkBeleg.Belegart)
                    {
                        case (int) Belegart.Lieferschein:
                            Entities.Rows[iIndex]["c8"] = objVkBeleg.Belegnummer;
                            break;
                        case (int) Belegart.Rechnung:
                            Entities.Rows[iIndex]["c9"] = objVkBeleg.Belegnummer;
                            break;
                    }
                }
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

        private DataTable ReadEntities()
        {
            try
            {
                return GmDb.Read(TableType, GmFile, string.Empty, BelegeId, EkWareId, ArtikelNr, ChargenNr, Converters.DateToIntYYYYMMDD(Rechnungsdatum), Positionsnummer);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataRow Unwrap(VkWare objEntity)
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
            objDataRow["c11"] = objEntity.KolliLieferschein;
            objDataRow["c12"] = objEntity.MengeLieferschein;
            objDataRow["c13"] = objEntity.SummeNettoLieferschein;
            objDataRow["c14"] = objEntity.GewinnNetto;
            objDataRow["c15"] = objEntity.BelegeId;
            objDataRow["c16"] = objEntity.EkWareId;
            objDataRow["c17"] = objEntity.KolliRechnung;
            objDataRow["c18"] = objEntity.MengeRechnung;
            objDataRow["c19"] = objEntity.SummeNettoRechnung;
            objDataRow["c20"] = objEntity.Lieferscheindatum;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private VkWare Wrap(DataRow objDataRow)
        {
            var objEntity = new VkWare(GmPath, GmUserData)
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
                KolliLieferschein = Convert.ToInt32(objDataRow["c11"]),
                MengeLieferschein = Convert.ToDecimal(objDataRow["c12"]),
                SummeNettoLieferschein = Convert.ToDecimal(objDataRow["c13"]),
                GewinnNetto = Convert.ToDecimal(objDataRow["c14"]),
                BelegeId = Convert.ToInt32(objDataRow["c15"]),
                EkWareId = Convert.ToInt32(objDataRow["c16"]),
                KolliRechnung = Convert.ToInt32(objDataRow["c17"]),
                MengeRechnung = Convert.ToDecimal(objDataRow["c18"]),
                SummeNettoRechnung = Convert.ToDecimal(objDataRow["c19"]),
                Lieferscheindatum = (DateTime)objDataRow["c20"],
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

        private int _kolliLieferschein;
        [DataMember]
        public int KolliLieferschein
        {
            get { return _kolliLieferschein; }
            set { if ((_kolliLieferschein != value)) { SendPropertyChanging(); _kolliLieferschein = value; SendPropertyChanged(); } }
        }

        private decimal _mengeLieferschein;
        [DataMember]
        public decimal MengeLieferschein
        {
            get { return _mengeLieferschein; }
            set { if ((_mengeLieferschein != value)) { SendPropertyChanging(); _mengeLieferschein = value; SendPropertyChanged(); } }
        }

        private decimal _summeNettoLieferschein;
        [DataMember]
        public decimal SummeNettoLieferschein
        {
            get { return _summeNettoLieferschein; }
            set { if ((_summeNettoLieferschein != value)) { SendPropertyChanging(); _summeNettoLieferschein = value; SendPropertyChanged(); } }
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

        private int _ekWareId;
        [DataMember]
        public int EkWareId
        {
            get { return _ekWareId; }
            set { if ((_ekWareId != value)) { SendPropertyChanging(); _ekWareId = value; SendPropertyChanged(); } }
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

        private DateTime _lieferscheindatum;
        [DataMember]
        public DateTime Lieferscheindatum
        {
            get { return _lieferscheindatum; }
            set { if ((_lieferscheindatum != value)) { SendPropertyChanging(); _lieferscheindatum = value; SendPropertyChanged(); } }
        }

        private Waren _ware;
        [DataMember]
        public Waren Ware
        {
            get { return _ware; }
            set { if ((_ware != value)) { SendPropertyChanging(); _ware = value; SendPropertyChanged(); } }
        }

        private EkWare _ekWare;
        [DataMember]
        public EkWare EkWare
        {
            get { return _ekWare; }
            set { if ((_ekWare != value)) { SendPropertyChanging(); _ekWare = value; SendPropertyChanged(); } }
        }

        private VkBeleg _vkBeleg;
        [DataMember]
        public VkBeleg VkBeleg
        {
            get { return _vkBeleg; }
            set { if ((_vkBeleg != value)) { SendPropertyChanging(); _vkBeleg = value; SendPropertyChanged(); } }
        }

        private EDocumentdetailState _state;
        [DataMember]
        public EDocumentdetailState State
        {
            get { return _state; }
            set { if ((_state != value)) { SendPropertyChanging(); _state = value; SendPropertyChanged(); } }
        }
        #endregion
    }
}
