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
    public class EkBeleg : GmBase, IEnumerator
    {
        #region private properties

        private EkBeleg[] _aobjEntities;

        #endregion

        #region Constructors

        public EkBeleg(int iKontoNr, int iChargenNr, int iBelegNr, DateTime dtBelegdatum, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.EKBELEG)
        {
            KontoNr = iKontoNr;
            ChargenNr = iChargenNr;
            Belegnummer = iBelegNr;
            Belegdatum = dtBelegdatum;
        }

        public EkBeleg(int iKontoNr, int iChargenNr, int iBelegNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, iChargenNr, iBelegNr, default(DateTime), strGmPath, strGmUserData)
        {
        }

        public EkBeleg(int iKontoNr, int iChargenNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, iChargenNr, GmDb.ALL , strGmPath, strGmUserData)
        {
        }

        public EkBeleg(int iKontoNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public EkBeleg(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        #endregion

        #region public methods

        public EkBeleg DeepCopy
        {
            get
            {
                var objClone = (EkBeleg)this.MemberwiseClone();
                return objClone;
            }
        }

        public EkBeleg Save(EkBeleg objEntity)
        {
            try
            {
                if (objEntity == null)
                {
                    throw new Exception("Argument objEntitiy is null!");
                }

                if (Entities == null)
                {
                    throw new Exception("Collection Entities is null!");
                }

                objEntity.ChargenNr = objEntity.ChargenNr == 0
                    ? new Firma(GmPath, GmUserData).GetNewBelegnummer(Core.Belegart.Posnummer)
                    : objEntity.ChargenNr;

                Entities.Rows.Add(Unwrap(objEntity));
                WriteEntities();

                // read beleg to get BelegeId
                KontoNr = objEntity.KontoNr;
                ChargenNr = objEntity.ChargenNr;
                Belegnummer = objEntity.Belegnummer;
                var objResult = ReadEntities();
                var objEkBeleg = Read(objResult).FirstOrDefault();

                if (objEkBeleg == null)
                {
                    throw new Exception($"EkBeleg {objEntity.Belegnummer} for Konto {objEntity.KontoNr} couldn't be saved");
                }

                return objEkBeleg;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<EkBeleg> Read()
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

        public EkBeleg Create(
            Belegart enmBelegart,
            int iKontoNr,
            short iPersonalNr,
            int iBelegnummer,
            DateTime dBelegdatum,
            decimal dMwstBetragHoch,
            decimal dNettobetrag,
            decimal dMwstBetragNiedrig,
            decimal dBruttobetrag,
            string strLkwInfo)
        {
            try
            {
                var objKonto = new Konten(iKontoNr, GmPath, GmUserData).Read().FirstOrDefault();
                if (objKonto == null)
                {
                    throw new Exception($"Konto {iKontoNr} not found");
                }

                var objEkBeleg = new EkBeleg(GmPath, GmUserData)
                {
                    Delete = 0,
                    KontoNr = objKonto.KontoNr,
                    PersonalNr = iPersonalNr,
                    ChargenNr = new Firma(GmPath, GmUserData).GetNewBelegnummer(Core.Belegart.Posnummer),
                    Belegart = (short)enmBelegart,
                    Belegnummer = iBelegnummer,
                    Belegdatum = dBelegdatum,
                    MwstBetragHoch = dMwstBetragHoch,
                    Nettobetrag = dNettobetrag,
                    MwstBetragNiedrig = dMwstBetragNiedrig,
                    Bruttobetrag = dBruttobetrag,
                    Unbekannt1 = 0,
                    Unbekannt2 = 0,
                    Unbekannt3 = 0,
                    Unbekannt4 = 0,
                    Unbekannt5 = 0,
                    Unbekannt6 = 0,
                    LkwInfo = strLkwInfo
                };

                return objEkBeleg;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private IEnumerable<EkBeleg> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new EkBeleg[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);

                objEntity.Konto = new Konten(objEntity.KontoNr, GmPath, GmUserData).Read().FirstOrDefault();

                _aobjEntities[iRow] = objEntity;
                yield return objEntity;
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
            try
            {
                return GmDb.Read(TableType, GmFile, string.Empty, KontoNr, ChargenNr, Belegnummer, Converters.DateToIntYYYYMMDD(Belegdatum));
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataRow Unwrap(EkBeleg objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.PersonalNr;
            objDataRow["c3"] = objEntity.ChargenNr;
            objDataRow["c4"] = objEntity.Belegart;
            objDataRow["c5"] = objEntity.Belegnummer;
            objDataRow["c6"] = objEntity.Belegdatum;
            objDataRow["c7"] = objEntity.MwstBetragHoch;
            objDataRow["c8"] = objEntity.Nettobetrag;
            objDataRow["c9"] = objEntity.MwstBetragNiedrig;
            objDataRow["c10"] = objEntity.Bruttobetrag;
            objDataRow["c11"] = objEntity.Unbekannt1;
            objDataRow["c12"] = objEntity.Unbekannt2;
            objDataRow["c13"] = objEntity.Unbekannt3;
            objDataRow["c14"] = objEntity.Unbekannt4;
            objDataRow["c15"] = objEntity.Unbekannt5;
            objDataRow["c16"] = objEntity.Unbekannt6;
            objDataRow["c17"] = objEntity.LkwInfo;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private EkBeleg Wrap(DataRow objDataRow)
        {
            var objEntity = new EkBeleg(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                PersonalNr = Convert.ToInt16(objDataRow["c2"]),
                ChargenNr = Convert.ToInt32(objDataRow["c3"]),
                Belegart = Convert.ToInt16(objDataRow["c4"]),
                Belegnummer = Convert.ToInt32(objDataRow["c5"]),
                Belegdatum = (DateTime)(objDataRow["c6"]),
                MwstBetragHoch = Convert.ToDecimal(objDataRow["c7"]),
                Nettobetrag = Convert.ToDecimal(objDataRow["c8"]),
                MwstBetragNiedrig = Convert.ToDecimal(objDataRow["c9"]),
                Bruttobetrag = Convert.ToDecimal(objDataRow["c10"]),
                Unbekannt1 = Convert.ToDecimal(objDataRow["c11"]),
                Unbekannt2 = Convert.ToInt16(objDataRow["c12"]),
                Unbekannt3 = Convert.ToInt16(objDataRow["c13"]),
                Unbekannt4 = Convert.ToInt16(objDataRow["c14"]),
                Unbekannt5 = Convert.ToInt16(objDataRow["c15"]),
                Unbekannt6 = Convert.ToInt16(objDataRow["c16"]),
                LkwInfo = objDataRow["c17"].ToString(),
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

        private int _chargenNr;
        [DataMember]
        public int ChargenNr
        {
            get { return _chargenNr; }
            set { if ((_chargenNr != value)) { SendPropertyChanging(); _chargenNr = value; SendPropertyChanged(); } }
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

        private decimal _mwstBetragHoch;
        [DataMember]
        public decimal MwstBetragHoch
        {
            get { return _mwstBetragHoch; }
            set { if ((_mwstBetragHoch != value)) { SendPropertyChanging(); _mwstBetragHoch = value; SendPropertyChanged(); } }
        }

        private decimal _nettobetrag;
        [DataMember]
        public decimal Nettobetrag
        {
            get { return _nettobetrag; }
            set { if ((_nettobetrag != value)) { SendPropertyChanging(); _nettobetrag = value; SendPropertyChanged(); } }
        }

        private decimal _mwstBetragNiedrig;
        [DataMember]
        public decimal MwstBetragNiedrig
        {
            get { return _mwstBetragNiedrig; }
            set { if ((_mwstBetragNiedrig != value)) { SendPropertyChanging(); _mwstBetragNiedrig = value; SendPropertyChanged(); } }
        }

        private decimal _bruttobetrag;
        [DataMember]
        public decimal Bruttobetrag
        {
            get { return _bruttobetrag; }
            set { if ((_bruttobetrag != value)) { SendPropertyChanging(); _bruttobetrag = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt1;
        [DataMember]
        public decimal Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private short _unbekannt2;
        [DataMember]
        public short Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

        private short _unbekannt3;
        [DataMember]
        public short Unbekannt3
        {
            get { return _unbekannt3; }
            set { if ((_unbekannt3 != value)) { SendPropertyChanging(); _unbekannt3 = value; SendPropertyChanged(); } }
        }

        private short _unbekannt4;
        [DataMember]
        public short Unbekannt4
        {
            get { return _unbekannt4; }
            set { if ((_unbekannt4 != value)) { SendPropertyChanging(); _unbekannt4 = value; SendPropertyChanged(); } }
        }

        private short _unbekannt5;
        [DataMember]
        public short Unbekannt5
        {
            get { return _unbekannt5; }
            set { if ((_unbekannt5 != value)) { SendPropertyChanging(); _unbekannt5 = value; SendPropertyChanged(); } }
        }

        private short _unbekannt6;
        [DataMember]
        public short Unbekannt6
        {
            get { return _unbekannt6; }
            set { if ((_unbekannt6 != value)) { SendPropertyChanging(); _unbekannt6 = value; SendPropertyChanged(); } }
        }

        private string _lkwInfo;
        [DataMember]
        public string LkwInfo
        {
            get { return _lkwInfo; }
            set { if ((_lkwInfo != value)) { SendPropertyChanging(); _lkwInfo = value; SendPropertyChanged(); } }
        }

        private Konten _konto;
        [DataMember]
        public Konten Konto
        {
            get { return _konto; }
            set { if ((_konto != value)) { SendPropertyChanging(); _konto = value; SendPropertyChanged(); } }
        }

        private IEnumerable<EkWare> _ekWare;
        [DataMember]
        public IEnumerable<EkWare> EkWare
        {
            get { return _ekWare; }
            set { if ((_ekWare != value)) { SendPropertyChanging(); _ekWare = value; SendPropertyChanged(); } }
        }

        private IEnumerable<EkWare> _ekWarenDeleted;
        [DataMember]
        public IEnumerable<EkWare> EkWarenDeleted
        {
            get { return _ekWarenDeleted; }
            set { if ((_ekWarenDeleted != value)) { SendPropertyChanging(); _ekWarenDeleted = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
