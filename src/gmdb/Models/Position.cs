namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class Position : GmBase, IEnumerator
    {
        #region private properties

        private Position[] _aobjEntities;

        #endregion

        #region constructor

        public Position(int iChargenNr, int iKontoNr, DateTime dtBelegdatum2, short sUnbekannt15, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.POSITION)
        {
            ChargenNr = iChargenNr;
            KontoNr = iKontoNr;
            Belegdatum2 = dtBelegdatum2;
            Unbekannt15 = sUnbekannt15;
        }

        public Position(int iChargenNr, int iKontoNr, DateTime dtBelegdatum2, string strGmPath, string strGmUserData)
            : this(iChargenNr, iKontoNr, dtBelegdatum2, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public Position(int iChargenNr, int iKontoNr, string strGmPath, string strGmUserData)
            : this(iChargenNr, iKontoNr, default(DateTime), strGmPath, strGmUserData)
        {
        }

        public Position(int iChargenNr, string strGmPath, string strGmUserData)
            : this(iChargenNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public Position(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        #endregion

        #region public methods

        public Position DeepCopy
        {
            get
            {
                var objClone = (Position)this.MemberwiseClone();
                return objClone;
            }
        }

        public Position Add(Position objEntity)
        {
            Entities.Rows.Add(Unwrap(objEntity));
            return this;
        }

        public int Count
        {
            get { return Entities.Rows.Count; }
        }

        public Position AtPosition(int iPosition)
        {
            try
            {
                if (Entities.Rows.Count - 1 <= iPosition)
                {
                    throw new Exception($"Position {iPosition} in Entities is out of range");
                }

                return Wrap(Entities.Rows[iPosition]);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public Position Create(
            int iChargenNr,
            int iKontoNr,
            DateTime dtBelegdatum1,
            string strFahrerLkw,
            string strWahrenart,
            int iGesamtKolli,
            decimal dGesamtGewicht,
            decimal dProvisionRabatt,
            DateTime dtBelegdatum2,
            int iLieferscheinNr,
            DateTime dtBelegDatum3,
            decimal dRechnungsbetragNetto,
            decimal dGesamtkostenNetto)
        {
            var objPosition = new Position(GmPath, GmUserData)
            {
                Delete = 0,
                ChargenNr = iChargenNr,
                KontoNr = iKontoNr,
                Unbekannt1 = 1,
                BelegDatum1 = dtBelegdatum1,
                FahrerLkw = strFahrerLkw,
                Unbekannt2 = string.Empty,
                Unbekannt3 = string.Empty,
                Unbekannt4 = 0,
                Warenart = strWahrenart,
                Unbekannt5 = 0,
                GesamtKolli = iGesamtKolli,
                GesamtGewicht = dGesamtGewicht,
                ProvisionRabatt = dProvisionRabatt,
                Unbekannt6 = 0,
                Unbekannt7 = 0,
                Unbekannt8 = 0,
                Unbekannt9 = default(DateTime),
                Unbekannt10 = string.Empty,
                Unbekannt11 = default(DateTime),
                Belegdatum2 = dtBelegdatum2,
                Unbekannt12 = default(DateTime),
                LieferscheinNr = iLieferscheinNr,
                Belegdatum3 = dtBelegDatum3,
                Unbekannt13 = 0,
                Unbekannt14 = 0,
                Unbekannt15 = 0,
                Unbekannt16 = 0,
                Unbekannt17 = 0,
                Unbekannt18 = 0,
                RechnungsBetragNetto = dRechnungsbetragNetto,
                Unbekannt19 = 0,
                Unbekannt20 = 0,
                GesamtkostenNetto = dGesamtkostenNetto,
                Unbekannt21 = 0,
                Unbekannt22 = 0,
                Unbekannt23 = 0,
                Unbekannt24 = 0,
                Unbekannt25 = 0,
                Unbekannt26 = 0,
                Unbekannt27 = 0,
                Unbekannt28 = 0,
                Unbekannt29 = 0,
                Unbekannt30 = string.Empty,
                Unbekannt31 = string.Empty
            };

            return objPosition;
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

        public void Save(Position objEntity)
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

        public IEnumerable<Position> Read()
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

        private IEnumerable<Position> Read(DataTable objEntities)
        {
            if (objEntities == null)
            {
                yield break;
            }

            _aobjEntities = new Position[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
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
            return GmDb.Read(TableType, GmFile, "", ChargenNr, KontoNr, Converters.DateToIntYYYYMMDD(Belegdatum2), Unbekannt15);
        }

        private DataRow Unwrap(Position objEntity)
        {
            DataRow objDataRow = Entities.NewRow();

            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.ChargenNr;
            objDataRow["c2"] = objEntity.KontoNr;
            objDataRow["c3"] = objEntity.Unbekannt1;
            objDataRow["c4"] = objEntity.BelegDatum1;
            objDataRow["c5"] = objEntity.FahrerLkw;
            objDataRow["c6"] = objEntity.Unbekannt2;
            objDataRow["c7"] = objEntity.Unbekannt3;
            objDataRow["c8"] = objEntity.Unbekannt4;
            objDataRow["c9"] = objEntity.Warenart;
            objDataRow["c10"] = objEntity.Unbekannt5;
            objDataRow["c11"] = objEntity.GesamtKolli;
            objDataRow["c12"] = objEntity.GesamtGewicht;
            objDataRow["c13"] = objEntity.ProvisionRabatt;
            objDataRow["c14"] = objEntity.Unbekannt6;
            objDataRow["c15"] = objEntity.Unbekannt7;
            objDataRow["c16"] = objEntity.Unbekannt8;
            objDataRow["c17"] = objEntity.Unbekannt9;
            objDataRow["c18"] = objEntity.Unbekannt10;
            objDataRow["c19"] = objEntity.Unbekannt11;
            objDataRow["c20"] = objEntity.Belegdatum2;
            objDataRow["c21"] = objEntity.Unbekannt12;
            objDataRow["c22"] = objEntity.LieferscheinNr;
            objDataRow["c23"] = objEntity.Belegdatum3;
            objDataRow["c24"] = objEntity.Unbekannt13;
            objDataRow["c25"] = objEntity.Unbekannt14;
            objDataRow["c26"] = objEntity.Unbekannt15;
            objDataRow["c27"] = objEntity.Unbekannt16;
            objDataRow["c28"] = objEntity.Unbekannt17;
            objDataRow["c29"] = objEntity.Unbekannt18;
            objDataRow["c30"] = objEntity.RechnungsBetragNetto;
            objDataRow["c31"] = objEntity.Unbekannt19;
            objDataRow["c32"] = objEntity.Unbekannt20;
            objDataRow["c33"] = objEntity.GesamtkostenNetto;
            objDataRow["c34"] = objEntity.Unbekannt21;
            objDataRow["c35"] = objEntity.Unbekannt22;
            objDataRow["c36"] = objEntity.Unbekannt23;
            objDataRow["c37"] = objEntity.Unbekannt24;
            objDataRow["c38"] = objEntity.Unbekannt25;
            objDataRow["c39"] = objEntity.Unbekannt26;
            objDataRow["c40"] = objEntity.Unbekannt27;
            objDataRow["c41"] = objEntity.Unbekannt28;
            objDataRow["c42"] = objEntity.Unbekannt29;
            objDataRow["c43"] = objEntity.Unbekannt30;
            objDataRow["c44"] = objEntity.Unbekannt31;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private Position Wrap(DataRow objDataRow)
        {
            var objEntity = new Position(GmPath, GmUserData);
            objEntity.Delete = Convert.ToInt32(objDataRow["c0"]);
            objEntity.ChargenNr = Convert.ToInt32(objDataRow["c1"]);
            objEntity.KontoNr = Convert.ToInt32(objDataRow["c2"]);
            objEntity.Unbekannt1 = Convert.ToInt32(objDataRow["c3"]);
            objEntity.BelegDatum1 = (DateTime)objDataRow["c4"];
            objEntity.FahrerLkw = objDataRow["c5"].ToString().Trim();
            objEntity.Unbekannt2 = objDataRow["c6"].ToString().Trim();
            objEntity.Unbekannt3 = objDataRow["c7"].ToString().Trim();
            objEntity.Unbekannt4 = Convert.ToInt32(objDataRow["c8"]);
            objEntity.Warenart = objDataRow["c9"].ToString().Trim();
            objEntity.Unbekannt5 = Convert.ToInt32(objDataRow["c10"]);
            objEntity.GesamtKolli = Convert.ToInt32(objDataRow["c11"]);
            objEntity.GesamtGewicht = Convert.ToDecimal(objDataRow["c12"]);
            objEntity.ProvisionRabatt = Convert.ToDecimal(objDataRow["c13"]);
            objEntity.Unbekannt6 = Convert.ToDecimal(objDataRow["c14"]);
            objEntity.Unbekannt7 = Convert.ToDecimal(objDataRow["c15"]);
            objEntity.Unbekannt8 = Convert.ToDecimal(objDataRow["c16"]);
            objEntity.Unbekannt9 = (DateTime)objDataRow["c17"];
            objEntity.Unbekannt10 = objDataRow["c18"].ToString().Trim();
            objEntity.Unbekannt11 = (DateTime)objDataRow["c19"];
            objEntity.Belegdatum2 = (DateTime)objDataRow["c20"];
            objEntity.Unbekannt12 = (DateTime)objDataRow["c21"];
            objEntity.LieferscheinNr = Convert.ToInt32(objDataRow["c22"]);
            objEntity.Belegdatum3 = (DateTime)objDataRow["c23"];
            objEntity.Unbekannt13 = Convert.ToInt32(objDataRow["c24"]);
            objEntity.Unbekannt14 = Convert.ToInt32(objDataRow["c25"]);
            objEntity.Unbekannt15 = Convert.ToInt16(objDataRow["c26"]);
            objEntity.Unbekannt16 = Convert.ToDecimal(objDataRow["c27"]);
            objEntity.Unbekannt17 = Convert.ToDecimal(objDataRow["c28"]);
            objEntity.Unbekannt18 = Convert.ToDecimal(objDataRow["c29"]);
            objEntity.RechnungsBetragNetto = Convert.ToDecimal(objDataRow["c30"]);
            objEntity.Unbekannt19 = Convert.ToDecimal(objDataRow["c31"]);
            objEntity.Unbekannt20 = Convert.ToDecimal(objDataRow["c32"]);
            objEntity.GesamtkostenNetto = Convert.ToDecimal(objDataRow["c33"]);
            objEntity.Unbekannt21 = Convert.ToDecimal(objDataRow["c34"]);
            objEntity.Unbekannt22 = Convert.ToDecimal(objDataRow["c35"]);
            objEntity.Unbekannt23 = Convert.ToDecimal(objDataRow["c36"]);
            objEntity.Unbekannt24 = Convert.ToDecimal(objDataRow["c37"]);
            objEntity.Unbekannt25 = Convert.ToDecimal(objDataRow["c38"]);
            objEntity.Unbekannt26 = Convert.ToDecimal(objDataRow["c39"]);
            objEntity.Unbekannt27 = Convert.ToDecimal(objDataRow["c40"]);
            objEntity.Unbekannt28 = Convert.ToDecimal(objDataRow["c41"]);
            objEntity.Unbekannt29 = Convert.ToInt16(objDataRow["c42"]);
            objEntity.Unbekannt30 = objDataRow["c43"].ToString().Trim();
            objEntity.Unbekannt31 = objDataRow["c44"].ToString().Trim();
            objEntity.File = objDataRow["FILENAME"].ToString().Trim();
            objEntity.FileId = Convert.ToInt32(objDataRow["ROW"]);
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

        private int _unbekannt1;
        [DataMember]
        public int Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private DateTime _belegDatum1;
        [DataMember]
        public DateTime BelegDatum1
        {
            get { return _belegDatum1; }
            set { if ((_belegDatum1 != value)) { SendPropertyChanging(); _belegDatum1 = value; SendPropertyChanged(); } }
        }

        private string _fahrerLkw;
        [DataMember]
        public string FahrerLkw
        {
            get { return _fahrerLkw; }
            set { if ((_fahrerLkw != value)) { SendPropertyChanging(); _fahrerLkw = value; SendPropertyChanged(); } }
        }

        private string _unbekannt2;
        [DataMember]
        public string Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }


        private string _unbekannt3;
        [DataMember]
        public string Unbekannt3
        {
            get { return _unbekannt3; }
            set { if ((_unbekannt3 != value)) { SendPropertyChanging(); _unbekannt3 = value; SendPropertyChanged(); } }
        }

        private int _unbekannt4;
        [DataMember]
        public int Unbekannt4
        {
            get { return _unbekannt4; }
            set { if ((_unbekannt4 != value)) { SendPropertyChanging(); _unbekannt4 = value; SendPropertyChanged(); } }
        }

        private string _warenart;
        [DataMember]
        public string Warenart
        {
            get { return _warenart; }
            set { if ((_warenart != value)) { SendPropertyChanging(); _warenart = value; SendPropertyChanged(); } }
        }

        private int _unbekannt5;
        [DataMember]
        public int Unbekannt5
        {
            get { return _unbekannt5; }
            set { if ((_unbekannt5 != value)) { SendPropertyChanging(); _unbekannt5 = value; SendPropertyChanged(); } }
        }

        private int _gesamtKolli;
        [DataMember]
        public int GesamtKolli
        {
            get { return _gesamtKolli; }
            set { if ((_gesamtKolli != value)) { SendPropertyChanging(); _gesamtKolli = value; SendPropertyChanged(); } }
        }

        private decimal _gesamtGewicht;
        [DataMember]
        public decimal GesamtGewicht
        {
            get { return _gesamtGewicht; }
            set { if ((_gesamtGewicht != value)) { SendPropertyChanging(); _gesamtGewicht = value; SendPropertyChanged(); } }
        }

        private decimal _provisionRabatt;
        [DataMember]
        public decimal ProvisionRabatt
        {
            get { return _provisionRabatt; }
            set { if ((_provisionRabatt != value)) { SendPropertyChanging(); _provisionRabatt = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt6;
        [DataMember]
        public decimal Unbekannt6
        {
            get { return _unbekannt6; }
            set { if ((_unbekannt6 != value)) { SendPropertyChanging(); _unbekannt6 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt7;
        [DataMember]
        public decimal Unbekannt7
        {
            get { return _unbekannt7; }
            set { if ((_unbekannt7 != value)) { SendPropertyChanging(); _unbekannt7 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt8;
        [DataMember]
        public decimal Unbekannt8
        {
            get { return _unbekannt8; }
            set { if ((_unbekannt8 != value)) { SendPropertyChanging(); _unbekannt8 = value; SendPropertyChanged(); } }
        }

        private DateTime _unbekannt9;
        [DataMember]
        public DateTime Unbekannt9
        {
            get { return _unbekannt9; }
            set { if ((_unbekannt9 != value)) { SendPropertyChanging(); _unbekannt9 = value; SendPropertyChanged(); } }
        }

        private string _unbekannt10;
        [DataMember]
        public string Unbekannt10
        {
            get { return _unbekannt10; }
            set { if ((_unbekannt10 != value)) { SendPropertyChanging(); _unbekannt10 = value; SendPropertyChanged(); } }
        }

        private DateTime _unbekannt11;
        [DataMember]
        public DateTime Unbekannt11
        {
            get { return _unbekannt11; }
            set { if ((_unbekannt11 != value)) { SendPropertyChanging(); _unbekannt11 = value; SendPropertyChanged(); } }
        }

        private DateTime _belegdatum2;
        [DataMember]
        public DateTime Belegdatum2
        {
            get { return _belegdatum2; }
            set { if ((_belegdatum2 != value)) { SendPropertyChanging(); _belegdatum2 = value; SendPropertyChanged(); } }
        }

        private DateTime _unbekannt12;
        [DataMember]
        public DateTime Unbekannt12
        {
            get { return _unbekannt12; }
            set { if ((_unbekannt12 != value)) { SendPropertyChanging(); _unbekannt12 = value; SendPropertyChanged(); } }
        }

        private int _lieferscheinNr;
        [DataMember]
        public int LieferscheinNr
        {
            get { return _lieferscheinNr; }
            set { if ((_lieferscheinNr != value)) { SendPropertyChanging(); _lieferscheinNr = value; SendPropertyChanged(); } }
        }

        private DateTime _belegdatum3;
        [DataMember]
        public DateTime Belegdatum3
        {
            get { return _belegdatum3; }
            set { if ((_belegdatum3 != value)) { SendPropertyChanging(); _belegdatum3 = value; SendPropertyChanged(); } }
        }

        private int _unbekannt13;
        [DataMember]
        public int Unbekannt13
        {
            get { return _unbekannt13; }
            set { if ((_unbekannt13 != value)) { SendPropertyChanging(); _unbekannt13 = value; SendPropertyChanged(); } }
        }

        private int _unbekannt14;
        [DataMember]
        public int Unbekannt14
        {
            get { return _unbekannt14; }
            set { if ((_unbekannt14 != value)) { SendPropertyChanging(); _unbekannt14 = value; SendPropertyChanged(); } }
        }

        private Int16 _unbekannt15;
        [DataMember]
        public Int16 Unbekannt15
        {
            get { return _unbekannt15; }
            set { if ((_unbekannt15 != value)) { SendPropertyChanging(); _unbekannt15 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt16;
        [DataMember]
        public decimal Unbekannt16
        {
            get { return _unbekannt16; }
            set { if ((_unbekannt16 != value)) { SendPropertyChanging(); _unbekannt16 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt17;
        [DataMember]
        public decimal Unbekannt17
        {
            get { return _unbekannt17; }
            set { if ((_unbekannt17 != value)) { SendPropertyChanging(); _unbekannt17 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt18;
        [DataMember]
        public decimal Unbekannt18
        {
            get { return _unbekannt18; }
            set { if ((_unbekannt18 != value)) { SendPropertyChanging(); _unbekannt18 = value; SendPropertyChanged(); } }
        }

        private decimal _rechnungsBetragNetto;
        [DataMember]
        public decimal RechnungsBetragNetto
        {
            get { return _rechnungsBetragNetto; }
            set { if ((_rechnungsBetragNetto != value)) { SendPropertyChanging(); _rechnungsBetragNetto = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt19;
        [DataMember]
        public decimal Unbekannt19
        {
            get { return _unbekannt19; }
            set { if ((_unbekannt19 != value)) { SendPropertyChanging(); _unbekannt19 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt20;
        [DataMember]
        public decimal Unbekannt20
        {
            get { return _unbekannt20; }
            set { if ((_unbekannt20 != value)) { SendPropertyChanging(); _unbekannt20 = value; SendPropertyChanged(); } }
        }

        private decimal _gesamtkostenNetto;
        [DataMember]
        public decimal GesamtkostenNetto
        {
            get { return _gesamtkostenNetto; }
            set { if ((_gesamtkostenNetto != value)) { SendPropertyChanging(); _gesamtkostenNetto = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt21;
        [DataMember]
        public decimal Unbekannt21
        {
            get { return _unbekannt21; }
            set { if ((_unbekannt21 != value)) { SendPropertyChanging(); _unbekannt21 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt22;
        [DataMember]
        public decimal Unbekannt22
        {
            get { return _unbekannt22; }
            set { if ((_unbekannt22 != value)) { SendPropertyChanging(); _unbekannt22 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt23;
        [DataMember]
        public decimal Unbekannt23
        {
            get { return _unbekannt23; }
            set { if ((_unbekannt23 != value)) { SendPropertyChanging(); _unbekannt23 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt24;
        [DataMember]
        public decimal Unbekannt24
        {
            get { return _unbekannt24; }
            set { if ((_unbekannt24 != value)) { SendPropertyChanging(); _unbekannt24 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt25;
        [DataMember]
        public decimal Unbekannt25
        {
            get { return _unbekannt25; }
            set { if ((_unbekannt25 != value)) { SendPropertyChanging(); _unbekannt25 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt26;
        [DataMember]
        public decimal Unbekannt26
        {
            get { return _unbekannt26; }
            set { if ((_unbekannt26 != value)) { SendPropertyChanging(); _unbekannt26 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt27;
        [DataMember]
        public decimal Unbekannt27
        {
            get { return _unbekannt27; }
            set { if ((_unbekannt27 != value)) { SendPropertyChanging(); _unbekannt27 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt28;
        [DataMember]
        public decimal Unbekannt28
        {
            get { return _unbekannt28; }
            set { if ((_unbekannt28 != value)) { SendPropertyChanging(); _unbekannt28 = value; SendPropertyChanged(); } }
        }

        private Int16 _unbekannt29;
        [DataMember]
        public Int16 Unbekannt29
        {
            get { return _unbekannt29; }
            set { if ((_unbekannt29 != value)) { SendPropertyChanging(); _unbekannt29 = value; SendPropertyChanged(); } }
        }

        private string _unbekannt30;
        [DataMember]
        public string Unbekannt30
        {
            get { return _unbekannt30; }
            set { if ((_unbekannt30 != value)) { SendPropertyChanging(); _unbekannt30 = value; SendPropertyChanged(); } }
        }

        private string _unbekannt31;
        [DataMember]
        public string Unbekannt31
        {
            get { return _unbekannt31; }
            set { if ((_unbekannt31 != value)) { SendPropertyChanging(); _unbekannt31 = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
