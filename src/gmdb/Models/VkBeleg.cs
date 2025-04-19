using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using gmdb.Core;

namespace gmdb.Models
{
    [DataContract]
    public class VkBeleg : GmBase, IEnumerator
    {
        #region private properties

        private VkBeleg[] _aobjEntities;

        #endregion

        #region Constructors

        public VkBeleg(int iKontoNr, int iBelegNr, DateTime dtBelegdatum, int iKontrollNr, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.VKBELEG)
        {
            KontoNr = iKontoNr;
            Belegnummer = iBelegNr;
            Belegdatum = dtBelegdatum;
            KontrollNr = iKontrollNr;
        }

        public VkBeleg(int iKontoNr, int iBelegNr, DateTime dtBelegdatum, string strGmPath, string strGmUserData)
            : this(iKontoNr, iBelegNr, dtBelegdatum, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkBeleg(int iKontoNr, int iBelegNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, iBelegNr, default(DateTime) , strGmPath, strGmUserData)
        {
        }

        public VkBeleg(int iKontoNr, string strGmPath, string strGmUserData)
            : this(iKontoNr, GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        public VkBeleg(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        #endregion

        #region public methods

        public VkBeleg DeepCopy
        {
            get
            {
                var objClone = (VkBeleg)this.MemberwiseClone();
                return objClone;
            }
        }

        public VkBeleg Save(VkBeleg objEntity)
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

                objEntity.Belegnummer = objEntity.Belegnummer == 0
                    ? new Firma(GmPath, GmUserData).GetNewBelegnummer((Belegart)objEntity.Belegart)
                    : objEntity.Belegnummer;

                Entities.Rows.Add(Unwrap(objEntity));
                WriteEntities();

                // read beleg to get BelegeId
                KontoNr = objEntity.KontoNr;
                Belegnummer = objEntity.Belegnummer;
                var objResult = ReadEntities();
                var objVkBeleg = Read(objResult).FirstOrDefault();

                // save info
                if (objVkBeleg == null)
                {
                    throw new Exception($"Beleg {objEntity.Belegnummer} for Konto {objEntity.KontoNr} couldn't be saved ");
                }

                objVkBeleg.Info = objEntity.Info;
                objVkBeleg.Info = new VkTexte(GmPath, GmUserData).Update(objVkBeleg);

                return objVkBeleg;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<VkBeleg> Read()
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

        public VkBeleg Create(Belegart enmBelegart, int iKontoNr, int iBelegnummer, DateTime dBelegdatum, decimal dNettobetragLiefereschein, decimal dMwStBetrag, decimal dProfit, short iPersonalNr, string strUserInfo)
        {
            try
            {
                var objKonto = new Konten(iKontoNr, GmPath, GmUserData).Read().FirstOrDefault();
                if (objKonto == null)
                    throw new Exception($"Konto {iKontoNr} not found");

                var objVkBeleg = new VkBeleg(GmPath, GmUserData)
                {
                    Delete = 0,
                    KontoNr = objKonto.KontoNr,
                    PersonalNr = iPersonalNr,
                    KontrollNr = 0,
                    Belegart = (short) enmBelegart,
                    Belegnummer = iBelegnummer,
                    Belegdatum = DateTime.Now,
                    Unbekannt1 = 0,
                    NettobetragLiefereschein = dNettobetragLiefereschein,
                    MwstBetrag = dMwStBetrag,
                    BruttoRechnungsbetrag = 0,
                    NettoGewinnBetrag = dProfit,
                    Unbekannt2 = 0,
                    Stationsnummer = objKonto.Station,
                    Tournummer = objKonto.Tour,
                    Unbekannt3 = 0,
                    Unbekannt4 = 0,
                    Unbekannt5 = strUserInfo.Length > 20 ? strUserInfo.Substring(0, 19) : strUserInfo
                };

                return objVkBeleg;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private IEnumerable<VkBeleg> Read(DataTable objEntities)
        {
            if (objEntities == null)
            {
                yield break;
            }

            _aobjEntities = new VkBeleg[objEntities.Rows.Count];

            for (int iRow = 0; iRow < objEntities.Rows.Count; iRow++)
            {
                var objDataRow = objEntities.Rows[iRow];
                var objEntity = Wrap(objDataRow);
               
                objEntity.Konto = new Konten(objEntity.KontoNr, GmPath, GmUserData).Read().FirstOrDefault();

                var objVkTexte = new VkTexte(objEntity.FileId + 1, GmPath, GmUserData).Read().FirstOrDefault();
                objEntity.Info = objVkTexte != null ? objVkTexte.Text : string.Empty;

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
                    throw new Exception("No data to save. Entities is null!");
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
                return GmDb.Read(TableType, GmFile, string.Empty, KontoNr, Belegnummer, Converters.DateToIntYYYYMMDD(Belegdatum), KontrollNr);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataRow Unwrap(VkBeleg objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.KontoNr;
            objDataRow["c2"] = objEntity.PersonalNr;
            objDataRow["c3"] = objEntity.KontrollNr;
            objDataRow["c4"] = objEntity.Belegart;
            objDataRow["c5"] = objEntity.Belegnummer;
            objDataRow["c6"] = objEntity.Belegdatum;
            objDataRow["c7"] = objEntity.Unbekannt1;
            objDataRow["c8"] = objEntity.NettobetragLiefereschein;
            objDataRow["c9"] = objEntity.MwstBetrag;
            objDataRow["c10"] = objEntity.BruttoRechnungsbetrag;
            objDataRow["c11"] = objEntity.NettoGewinnBetrag;
            objDataRow["c12"] = objEntity.Unbekannt2;
            objDataRow["c13"] = objEntity.Stationsnummer;
            objDataRow["c14"] = objEntity.Tournummer;
            objDataRow["c15"] = objEntity.Unbekannt3;
            objDataRow["c16"] = objEntity.Unbekannt4;
            objDataRow["c17"] = objEntity.Unbekannt5;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private VkBeleg Wrap(DataRow objDataRow)
        {
            var objEntity = new VkBeleg(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                KontoNr = Convert.ToInt32(objDataRow["c1"]),
                PersonalNr = Convert.ToInt16(objDataRow["c2"]),
                KontrollNr = Convert.ToInt32(objDataRow["c3"]),
                Belegart = Convert.ToInt16(objDataRow["c4"]),
                Belegnummer = Convert.ToInt32(objDataRow["c5"]),
                Belegdatum = (DateTime)(objDataRow["c6"]),
                Unbekannt1 = Convert.ToDecimal(objDataRow["c7"]),
                NettobetragLiefereschein = Convert.ToDecimal(objDataRow["c8"]),
                MwstBetrag = Convert.ToDecimal(objDataRow["c9"]),
                BruttoRechnungsbetrag = Convert.ToDecimal(objDataRow["c10"]),
                NettoGewinnBetrag = Convert.ToDecimal(objDataRow["c11"]),
                Unbekannt2 = Convert.ToInt16(objDataRow["c12"]),
                Stationsnummer = Convert.ToInt16(objDataRow["c13"]),
                Tournummer = Convert.ToInt16(objDataRow["c14"]),
                Unbekannt3 = Convert.ToInt16(objDataRow["c15"]),
                Unbekannt4 = Convert.ToInt16(objDataRow["c16"]),
                Unbekannt5 = objDataRow["c17"].ToString(),
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

        private int _kontrollNr;
        [DataMember]
        public int KontrollNr
        {
            get { return _kontrollNr; }
            set { if ((_kontrollNr != value)) { SendPropertyChanging(); _kontrollNr = value; SendPropertyChanged(); } }
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

        private decimal _unbekannt1;
        [DataMember]
        public decimal Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private decimal _nettobetragLiefereschein;
        [DataMember]
        public decimal NettobetragLiefereschein
        {
            get { return _nettobetragLiefereschein; }
            set { if ((_nettobetragLiefereschein != value)) { SendPropertyChanging(); _nettobetragLiefereschein = value; SendPropertyChanged(); } }
        }

        private decimal _mwstBetrag;
        [DataMember]
        public decimal MwstBetrag
        {
            get { return _mwstBetrag; }
            set { if ((_mwstBetrag != value)) { SendPropertyChanging(); _mwstBetrag = value; SendPropertyChanged(); } }
        }

        private decimal _bruttoRechnungsbetrag;
        [DataMember]
        public decimal BruttoRechnungsbetrag
        {
            get { return _bruttoRechnungsbetrag; }
            set { if ((_bruttoRechnungsbetrag != value)) { SendPropertyChanging(); _bruttoRechnungsbetrag = value; SendPropertyChanged(); } }
        }

        private decimal _nettoGewinnBetrag;
        [DataMember]
        public decimal NettoGewinnBetrag
        {
            get { return _nettoGewinnBetrag; }
            set { if ((_nettoGewinnBetrag != value)) { SendPropertyChanging(); _nettoGewinnBetrag = value; SendPropertyChanged(); } }
        }

        private short _unbekannt2;
        [DataMember]
        public short Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

        private short _stationsnummer;
        [DataMember]
        public short Stationsnummer
        {
            get { return _stationsnummer; }
            set { if ((_stationsnummer != value)) { SendPropertyChanging(); _stationsnummer = value; SendPropertyChanged(); } }
        }

        private short _tournummer;
        [DataMember]
        public short Tournummer
        {
            get { return _tournummer; }
            set { if ((_tournummer != value)) { SendPropertyChanging(); _tournummer = value; SendPropertyChanged(); } }
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

        private string _unbekannt5;
        [DataMember]
        public string Unbekannt5
        {
            get { return _unbekannt5; }
            set { if ((_unbekannt5 != value)) { SendPropertyChanging(); _unbekannt5 = value; SendPropertyChanged(); } }
        }

        private string _info;
        [DataMember]
        public string Info
        {
            get { return _info; }
            set { if ((_info != value)) { SendPropertyChanging(); _info = value; SendPropertyChanged(); } }
        }

        private EDocumentState _state;
        [DataMember]
        public EDocumentState State
        {
            get { return _state; }
            set { if ((_state != value)) { SendPropertyChanging(); _state = value; SendPropertyChanged(); } }
        }

        private Konten _konto;
        [DataMember]
        public Konten Konto
        {
            get { return _konto; }
            set { if ((_konto != value)) { SendPropertyChanging(); _konto = value; SendPropertyChanged(); } }
        }

        private IEnumerable<VkWare> _vkWaren;
        [DataMember]
        public IEnumerable<VkWare> VkWaren
        {
            get { return _vkWaren; }
            set { if ((_vkWaren != value)) { SendPropertyChanging(); _vkWaren = value; SendPropertyChanged(); } }
        }

        private IEnumerable<VkWare> _vkWarenDeleted;
        [DataMember]
        public IEnumerable<VkWare> VkWarenDeleted
        {
            get { return _vkWarenDeleted; }
            set { if ((_vkWarenDeleted != value)) { SendPropertyChanging(); _vkWarenDeleted = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
