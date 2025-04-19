namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class Waren : GmBase, IEnumerator
    {
        #region private properties

        private Waren[] _aobjEntities;

        #endregion

        #region constructor

        public Waren(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.WAREN)
        {
            ArtikelNr = (Int16)GmDb.ALL;
            WarengruppeNr = (Int16)GmDb.ALL;
            SearchString = "";
        }

        public Waren(string strSearchString, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            SearchString = strSearchString;
        }

        public Waren(Int32 iArtikelNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            ArtikelNr = iArtikelNr;
        }

        public Waren(Int16 iWarengruppeNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            WarengruppeNr = iWarengruppeNr;
        }

        public Waren(int iArtikelNr, Int16 iWarengruppeNr, string strGmPath, string strGmUserData)
            : this(iWarengruppeNr, strGmPath, strGmUserData)
        {
            ArtikelNr = iArtikelNr;
        }

        public Waren(int iArtikelNr, Int16 iWarengruppeNr, string strSearchString, string strGmPath, string strGmUserData)
            : this(iArtikelNr, iWarengruppeNr, strGmPath, strGmUserData)
        {
            SearchString = strSearchString;
        }

        #endregion

        #region public methods

        public Waren DeepCopy
        {
            get
            {
                var objClone = (Waren)this.MemberwiseClone();
                return objClone;
            }
        }

        public string SearchString { get; set; }

        public Waren Add(Waren objEntity)
        {
            Entities.Rows.Add(Unwrap(objEntity));
            return this;
        }

        public void Save(Waren objEntity)
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

        public IEnumerable<Waren> Read()
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

        private IEnumerable<Waren> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Waren[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
                _aobjEntities[iRow] = objEntity;
                yield return objEntity;
            }
        }

        private DataTable ReadEntities()
        {
            try
            {
                return GmDb.Read(TableType, GmFile, SearchString, ArtikelNr, WarengruppeNr);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataRow Unwrap(Waren objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.ArtikelNr;
            objDataRow["c2"] = objEntity.Bezeichnung1;
            objDataRow["c3"] = objEntity.Bezeichnung2;
            objDataRow["c4"] = objEntity.Herkunftsland;
            objDataRow["c5"] = objEntity.Mengenschluessel;
            objDataRow["c6"] = objEntity.Steuerschluessel;
            objDataRow["c7"] = objEntity.WarengruppeNr;
            objDataRow["c8"] = objEntity.Unbekannt1;
            objDataRow["c9"] = objEntity.Inhalt;
            objDataRow["c10"] = objEntity.MengeneinheitProEur;
            objDataRow["c11"] = objEntity.PreiseinheitProKg;
            objDataRow["c12"] = objEntity.Verkaufspreis1;
            objDataRow["c13"] = objEntity.Preisuntergrenze;
            objDataRow["c14"] = objEntity.GewichtPerKolli;
            objDataRow["c15"] = objEntity.Verkaufspreis2;
            objDataRow["c16"] = objEntity.Unbekannt2;
            objDataRow["c17"] = objEntity.Leergutkonto;
            objDataRow["c18"] = objEntity.Unbekannt3;
            objDataRow["c19"] = objEntity.Verkaufspreis3;
            objDataRow["c20"] = objEntity.Unbekannt4;
            objDataRow["c21"] = objEntity.LetzterEK;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private Waren Wrap(DataRow objDataRow)
        {
            var objEntity = new Waren(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                ArtikelNr = Convert.ToInt32(objDataRow["c1"]),
                Bezeichnung1 = objDataRow["c2"].ToString().Trim(),
                Bezeichnung2 = objDataRow["c3"].ToString().Trim(),
                Herkunftsland = objDataRow["c4"].ToString().Trim(),
                Mengenschluessel = Convert.ToInt16(objDataRow["c5"]),
                Steuerschluessel = Convert.ToInt16(objDataRow["c6"]),
                WarengruppeNr = Convert.ToInt16(objDataRow["c7"]),
                Unbekannt1 = Convert.ToInt32(objDataRow["c8"]),
                Inhalt = Convert.ToDecimal(objDataRow["c9"]),
                MengeneinheitProEur = objDataRow["c10"].ToString().Trim(),
                PreiseinheitProKg = objDataRow["c11"].ToString().Trim(),
                Verkaufspreis1 = Convert.ToDecimal(objDataRow["c12"]),
                Preisuntergrenze = Convert.ToDecimal(objDataRow["c13"]),
                GewichtPerKolli = Convert.ToDecimal(objDataRow["c14"]),
                Verkaufspreis2 = Convert.ToDecimal(objDataRow["c15"]),
                Unbekannt2 = Convert.ToInt16(objDataRow["c16"]),
                Leergutkonto = Convert.ToInt16(objDataRow["c17"]),
                Unbekannt3 = Convert.ToInt16(objDataRow["c18"]),
                Verkaufspreis3 = Convert.ToDecimal(objDataRow["c19"]),
                Unbekannt4 = objDataRow["c20"].ToString().Trim(),
                LetzterEK = Convert.ToDecimal(objDataRow["c21"]),
                File = objDataRow["FILENAME"].ToString().Trim(),
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

        private int _artikelNr;
        [DataMember]
        public int ArtikelNr
        {
            get { return _artikelNr; }
            set { if ((_artikelNr != value)) { SendPropertyChanging(); _artikelNr = value; SendPropertyChanged(); } }
        }

        private string _bezeichnung1;
        [DataMember]
        public string Bezeichnung1
        {
            get { return _bezeichnung1; }
            set { if ((_bezeichnung1 != value)) { SendPropertyChanging(); _bezeichnung1 = value; SendPropertyChanged(); } }
        }

        private string _bezeichnung2;
        [DataMember]
        public string Bezeichnung2
        {
            get { return _bezeichnung2; }
            set { if ((_bezeichnung2 != value)) { SendPropertyChanging(); _bezeichnung2 = value; SendPropertyChanged(); } }
        }

        private string _herkunftsland;
        [DataMember]
        public string Herkunftsland
        {
            get { return _herkunftsland; }
            set { if ((_herkunftsland != value)) { SendPropertyChanging(); _herkunftsland = value; SendPropertyChanged(); } }
        }

        private Int16 _mengenschluessel;
        [DataMember]
        public Int16 Mengenschluessel
        {
            get { return _mengenschluessel; }
            set { if ((_mengenschluessel != value)) { SendPropertyChanging(); _mengenschluessel = value; SendPropertyChanged(); } }
        }

        private Int16 _steuerschluessel;
        [DataMember]
        public Int16 Steuerschluessel
        {
            get { return _steuerschluessel; }
            set 
            {
                if ((_steuerschluessel != value))
                {
                    SendPropertyChanging();
                    _steuerschluessel = value;

                    if( Globals != null && Globals.Steuer.Any())
                    {
                        switch (_steuerschluessel)
                        {
                            case 1:
                                Steuer = Globals.Steuer.ElementAt(0).Steuersatz1 / 100;
                                break;
                            case 2:
                                Steuer = Globals.Steuer.ElementAt(0).Steuersatz2 / 100;
                                break;
                        }
                    }

                    SendPropertyChanged();
                } 
            }
        }

        private Int16 _warengruppeNr;
        [DataMember]
        public Int16 WarengruppeNr
        {
            get { return _warengruppeNr; }
            set { if ((_warengruppeNr != value)) { SendPropertyChanging(); _warengruppeNr = value; SendPropertyChanged(); } }
        }

        private int _unbekannt1;
        [DataMember]
        public int Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private decimal _inhalt;
        [DataMember]
        public decimal Inhalt
        {
            get { return _inhalt; }
            set { if ((_inhalt != value)) { SendPropertyChanging(); _inhalt = value; SendPropertyChanged(); } }
        }

        private string _mengeneinheitProEur;
        [DataMember]
        public string MengeneinheitProEur
        {
            get { return _mengeneinheitProEur; }
            set { if ((_mengeneinheitProEur != value)) { SendPropertyChanging(); _mengeneinheitProEur = value; SendPropertyChanged(); } }
        }

        private string _preiseinheitProKg;
        [DataMember]
        public string PreiseinheitProKg
        {
            get { return _preiseinheitProKg; }
            set { if ((_preiseinheitProKg != value)) { SendPropertyChanging(); _preiseinheitProKg = value; SendPropertyChanged(); } }
        }

        private decimal _verkaufspreis1;
        [DataMember]
        public decimal Verkaufspreis1
        {
            get { return _verkaufspreis1; }
            set { if ((_verkaufspreis1 != value)) { SendPropertyChanging(); _verkaufspreis1 = value; SendPropertyChanged(); } }
        }

        private decimal _preisuntergrenze;
        [DataMember]
        public decimal Preisuntergrenze
        {
            get { return _preisuntergrenze; }
            set { if ((_preisuntergrenze != value)) { SendPropertyChanging(); _preisuntergrenze = value; SendPropertyChanged(); } }
        }

        private decimal _gewichtPerKolli;
        [DataMember]
        public decimal GewichtPerKolli
        {
            get { return _gewichtPerKolli; }
            set { if ((_gewichtPerKolli != value)) { SendPropertyChanging(); _gewichtPerKolli = value; SendPropertyChanged(); } }
        }

        private decimal _verkaufspreis2;
        [DataMember]
        public decimal Verkaufspreis2
        {
            get { return _verkaufspreis2; }
            set { if ((_verkaufspreis2 != value)) { SendPropertyChanging(); _verkaufspreis2 = value; SendPropertyChanged(); } }
        }

        private Int16 _unbekannt2;
        [DataMember]
        public Int16 Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

        private Int16 _leergutkonto;
        [DataMember]
        public Int16 Leergutkonto
        {
            get { return _leergutkonto; }
            set { if ((_leergutkonto != value)) { SendPropertyChanging(); _leergutkonto = value; SendPropertyChanged(); } }
        }

        private Int16 _unbekannt3;
        [DataMember]
        public Int16 Unbekannt3
        {
            get { return _unbekannt3; }
            set { if ((_unbekannt3 != value)) { SendPropertyChanging(); _unbekannt3 = value; SendPropertyChanged(); } }
        }

        private decimal _verkaufspreis3;
        [DataMember]
        public decimal Verkaufspreis3
        {
            get { return _verkaufspreis3; }
            set { if ((_verkaufspreis3 != value)) { SendPropertyChanging(); _verkaufspreis3 = value; SendPropertyChanged(); } }
        }

        private string _unbekannt4;
        [DataMember]
        public string Unbekannt4
        {
            get { return _unbekannt4; }
            set { if ((_unbekannt4 != value)) { SendPropertyChanging(); _unbekannt4 = value; SendPropertyChanged(); } }
        }

        private decimal _letzterEk;
        [DataMember]
        public decimal LetzterEK
        {
            get { return _letzterEk; }
            set { if ((_letzterEk != value)) { SendPropertyChanging(); _letzterEk = value; SendPropertyChanged(); } }
        }

        private decimal _steuer;
        [DataMember]
        public decimal Steuer
        {
            get { return _steuer; }
            set { if ((_steuer != value)) { SendPropertyChanging(); _steuer = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
