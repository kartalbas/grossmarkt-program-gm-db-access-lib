namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class Konten : GmBase, IEnumerator
    {
        #region private properties

        private Konten[] _aobjEntities;

        #endregion

        #region constructor

        public Konten(int iKontoNr, short iGruppenNr, string strSearchString, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.KONTEN)
        {
            KontoNr = iKontoNr;
            GruppenNr = iGruppenNr;
            SearchString = strSearchString;
        }

        public Konten(int iKontoNr, short iGruppenNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, iGruppenNr, string.Empty, strGmPath, strGmUserData)
        {
        }

        public Konten(int iKontoNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public Konten(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public Konten(string strSearchString, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            SearchString = strSearchString;
        }

        public Konten(short iGruppenNr, string strGmPath, string strGmUserData)
            : this(strGmPath, strGmUserData)
        {
            GruppenNr = iGruppenNr;
        }

        #endregion

        #region public methods

        public Konten DeepCopy
        {
            get
            {
                var objClone = (Konten)this.MemberwiseClone();
                return objClone;
            }
        }

        public string SearchString { get; set; }

        public void Save(Konten objEntity)
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

        public IEnumerable<Konten> Read()
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

        private IEnumerable<Konten> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Konten[objEntities.Rows.Count];

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
            return GmDb.Read(TableType, GmFile, SearchString, KontoNr, GruppenNr);
        }

        private DataRow Unwrap(Konten objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.Name;
            objDataRow["c3"] = objEntity.Branche;
            objDataRow["c4"] = objEntity.Strasse;
            objDataRow["c5"] = objEntity.PlzOrt;
            objDataRow["c6"] = objEntity.Kurzname;
            objDataRow["c7"] = objEntity.Zustaendig;
            objDataRow["c8"] = objEntity.Vorwahl;
            objDataRow["c9"] = objEntity.Telefon1;
            objDataRow["c10"] = objEntity.Telefon2;
            objDataRow["c11"] = objEntity.Fax;
            objDataRow["c12"] = objEntity.Telex;
            objDataRow["c13"] = objEntity.Steuergruppe;
            objDataRow["c14"] = objEntity.GruppenNr;
            objDataRow["c15"] = objEntity.Kreditlimit;
            objDataRow["c16"] = objEntity.Zahlungsziel;
            objDataRow["c17"] = objEntity.Mahnung;
            objDataRow["c18"] = objEntity.Rabatt;
            objDataRow["c19"] = objEntity.Rueckverguetung;
            objDataRow["c20"] = objEntity.Markenbezeichnung;
            objDataRow["c21"] = objEntity.LiPositionVonKURechnungsgruppe;
            objDataRow["c22"] = objEntity.PositionBis;
            objDataRow["c23"] = objEntity.Land;
            objDataRow["c24"] = objEntity.Station;
            objDataRow["c25"] = objEntity.LiLeergutKUPreisgruppe;
            objDataRow["c26"] = objEntity.Tour;
            objDataRow["c27"] = objEntity.Entsorgung;
            objDataRow["c28"] = objEntity.Unbekannt1;
            objDataRow["c29"] = objEntity.Textschluessel;
            objDataRow["c30"] = objEntity.SteuerID;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private Konten Wrap(DataRow objDataRow)
        {
            var objEntity = new Konten(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                Name = objDataRow["c2"].ToString().Trim(),
                Branche = objDataRow["c3"].ToString().Trim(),
                Strasse = objDataRow["c4"].ToString().Trim(),
                PlzOrt = objDataRow["c5"].ToString().Trim(),
                Kurzname = objDataRow["c6"].ToString().Trim(),
                Zustaendig = objDataRow["c7"].ToString().Trim(),
                Vorwahl = objDataRow["c8"].ToString().Trim(),
                Telefon1 = objDataRow["c9"].ToString().Trim(),
                Telefon2 = objDataRow["c10"].ToString().Trim(),
                Fax = objDataRow["c11"].ToString().Trim(),
                Telex = objDataRow["c12"].ToString().Trim(),
                Steuergruppe = Convert.ToInt16(objDataRow["c13"]),
                GruppenNr = Convert.ToInt16(objDataRow["c14"]),
                Kreditlimit = Convert.ToDecimal(objDataRow["c15"]),
                Zahlungsziel = Convert.ToInt16(objDataRow["c16"]),
                Mahnung = objDataRow["c17"].ToString().Trim(),
                Rabatt = Convert.ToInt32(objDataRow["c18"]),
                Rueckverguetung = Convert.ToInt32(objDataRow["c19"]),
                Markenbezeichnung = objDataRow["c20"].ToString().Trim(),
                LiPositionVonKURechnungsgruppe = Convert.ToInt32(objDataRow["c21"]),
                PositionBis = Convert.ToInt32(objDataRow["c22"]),
                Land = Convert.ToInt16(objDataRow["c23"]),
                Station = Convert.ToInt16(objDataRow["c24"]),
                LiLeergutKUPreisgruppe = Convert.ToInt16(objDataRow["c25"]),
                Tour = Convert.ToInt16(objDataRow["c26"]),
                Entsorgung = Convert.ToInt16(objDataRow["c27"]),
                Unbekannt1 = objDataRow["c28"].ToString().Trim(),
                Textschluessel = Convert.ToInt16(objDataRow["c29"]),
                SteuerID = objDataRow["c30"].ToString().Trim(),
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

        private int _kontoNr;
        [DataMember]
        public int KontoNr
        {
            get { return _kontoNr; }
            set { if ((_kontoNr != value)) { SendPropertyChanging(); _kontoNr = value; SendPropertyChanged(); } }
        }

        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { if ((_name != value)) { SendPropertyChanging(); _name = value; SendPropertyChanged(); } }
        }

        private string _branche;
        [DataMember]
        public string Branche
        {
            get { return _branche; }
            set { if ((_branche != value)) { SendPropertyChanging(); _branche = value; SendPropertyChanged(); } }
        }

        private string _strasse;
        [DataMember]
        public string Strasse
        {
            get { return _strasse; }
            set { if ((_strasse != value)) { SendPropertyChanging(); _strasse = value; SendPropertyChanged(); } }
        }

        private string _plzOrt;
        [DataMember]
        public string PlzOrt
        {
            get { return _plzOrt; }
            set { if ((_plzOrt != value)) { SendPropertyChanging(); _plzOrt = value; SendPropertyChanged(); } }
        }

        private string _kurzname;
        [DataMember]
        public string Kurzname
        {
            get { return _kurzname; }
            set { if ((_kurzname != value)) { SendPropertyChanging(); _kurzname = value; SendPropertyChanged(); } }
        }

        private string _zustaendig;
        [DataMember]
        public string Zustaendig
        {
            get { return _zustaendig; }
            set { if ((_zustaendig != value)) { SendPropertyChanging(); _zustaendig = value; SendPropertyChanged(); } }
        }

        private string _vorwahl;
        [DataMember]
        public string Vorwahl
        {
            get { return _vorwahl; }
            set { if ((_vorwahl != value)) { SendPropertyChanging(); _vorwahl = value; SendPropertyChanged(); } }
        }

        private string _telefon1;
        [DataMember]
        public string Telefon1
        {
            get { return _telefon1; }
            set { if ((_telefon1 != value)) { SendPropertyChanging(); _telefon1 = value; SendPropertyChanged(); } }
        }

        private string _telefon2;
        [DataMember]
        public string Telefon2
        {
            get { return _telefon2; }
            set { if ((_telefon2 != value)) { SendPropertyChanging(); _telefon2 = value; SendPropertyChanged(); } }
        }

        private string _fax;
        [DataMember]
        public string Fax
        {
            get { return _fax; }
            set { if ((_fax != value)) { SendPropertyChanging(); _fax = value; SendPropertyChanged(); } }
        }

        private string _telex;
        [DataMember]
        public string Telex
        {
            get { return _telex; }
            set { if ((_telex != value)) { SendPropertyChanging(); _telex = value; SendPropertyChanged(); } }
        }

        private Int16 _steuergruppe;
        [DataMember]
        public Int16 Steuergruppe
        {
            get { return _steuergruppe; }
            set { if ((_steuergruppe != value)) { SendPropertyChanging(); _steuergruppe = value; SendPropertyChanged(); } }
        }

        private Int16 _gruppenNr;
        [DataMember]
        public Int16 GruppenNr
        {
            get { return _gruppenNr; }
            set { if ((_gruppenNr != value)) { SendPropertyChanging(); _gruppenNr = value; SendPropertyChanged(); } }
        }

        private decimal _kreditlimit;
        [DataMember]
        public decimal Kreditlimit
        {
            get { return _kreditlimit; }
            set { if ((_kreditlimit != value)) { SendPropertyChanging(); _kreditlimit = value; SendPropertyChanged(); } }
        }

        private Int16 _zahlungsziel;
        [DataMember]
        public Int16 Zahlungsziel
        {
            get { return _zahlungsziel; }
            set { if ((_zahlungsziel != value)) { SendPropertyChanging(); _zahlungsziel = value; SendPropertyChanged(); } }
        }

        private string _mahnung;
        [DataMember]
        public string Mahnung
        {
            get { return _mahnung; }
            set { if ((_mahnung != value)) { SendPropertyChanging(); _mahnung = value; SendPropertyChanged(); } }
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

        private string _markenbezeichnung;
        [DataMember]
        public string Markenbezeichnung
        {
            get { return _markenbezeichnung; }
            set { if ((_markenbezeichnung != value)) { SendPropertyChanging(); _markenbezeichnung = value; SendPropertyChanged(); } }
        }

        private int _liPositionVonKuRechnungsgruppe;
        [DataMember]
        public int LiPositionVonKURechnungsgruppe
        {
            get { return _liPositionVonKuRechnungsgruppe; }
            set { if ((_liPositionVonKuRechnungsgruppe != value)) { SendPropertyChanging(); _liPositionVonKuRechnungsgruppe = value; SendPropertyChanged(); } }
        }

        private int _positionBis;
        [DataMember]
        public int PositionBis
        {
            get { return _positionBis; }
            set { if ((_positionBis != value)) { SendPropertyChanging(); _positionBis = value; SendPropertyChanged(); } }
        }

        private Int16 _land;
        [DataMember]
        public Int16 Land
        {
            get { return _land; }
            set { if ((_land != value)) { SendPropertyChanging(); _land = value; SendPropertyChanged(); } }
        }

        private Int16 _station;
        [DataMember]
        public Int16 Station
        {
            get { return _station; }
            set { if ((_station != value)) { SendPropertyChanging(); _station = value; SendPropertyChanged(); } }
        }

        private Int16 _liLeergutKuPreisgruppe;
        [DataMember]
        public Int16 LiLeergutKUPreisgruppe
        {
            get { return _liLeergutKuPreisgruppe; }
            set { if ((_liLeergutKuPreisgruppe != value)) { SendPropertyChanging(); _liLeergutKuPreisgruppe = value; SendPropertyChanged(); } }
        }

        private Int16 _tour;
        [DataMember]
        public Int16 Tour
        {
            get { return _tour; }
            set { if ((_tour != value)) { SendPropertyChanging(); _tour = value; SendPropertyChanged(); } }
        }

        private Int16 _entsorgung;
        [DataMember]
        public Int16 Entsorgung
        {
            get { return _entsorgung; }
            set { if ((_entsorgung != value)) { SendPropertyChanging(); _entsorgung = value; SendPropertyChanged(); } }
        }

        private string _unbekannt1;
        [DataMember]
        public string Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private Int16 _textschluessel;
        [DataMember]
        public Int16 Textschluessel
        {
            get { return _textschluessel; }
            set { if ((_textschluessel != value)) { SendPropertyChanging(); _textschluessel = value; SendPropertyChanged(); } }
        }

        private string _steuerId;
        [DataMember]
        public string SteuerID
        {
            get { return _steuerId; }
            set { if ((_steuerId != value)) { SendPropertyChanging(); _steuerId = value; SendPropertyChanged(); } }
        }

        #endregion    
    }
}
