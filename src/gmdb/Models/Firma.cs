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
    public class Firma : GmBase, IEnumerator
    {
        #region private properties

        private Firma[] _aobjEntities;

        #endregion

        #region constructor

        public Firma(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.FIRMA)
        {
        }

        #endregion

        #region public methods

        public Firma DeepCopy
        {
            get
            {
                var objClone = (Firma)this.MemberwiseClone();
                return objClone;
            }
        }

        public int GetNewBelegnummer(Belegart enmBelegart)
        {
            try
            {
                var cobjFirma = Read();
                var objFirma = cobjFirma.ElementAt(0);

                int iRetrunValue = 0;

                switch (enmBelegart)
                {
                    case Belegart.Lieferschein:
                        iRetrunValue = objFirma.Lieferscheinnummer++;
                        break;
                    case Belegart.Rechnung:
                        iRetrunValue = objFirma.Rechnungsnummer++;
                        break;
                    case Belegart.Posnummer:
                        iRetrunValue = objFirma.Posnummer++;
                        break;
                    case Belegart.Zukaufpositionen:
                        iRetrunValue = objFirma.Zukaufpositionen++;
                        break;
                }

                objFirma.Save(objFirma);

                return iRetrunValue;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<Firma> Save(Firma objEntity)
        {
            try
            {
                Entities.Rows.Add(Unwrap(objEntity));

                WriteEntities();

                var objNewEntities = ReadEntities();
                return Read(objNewEntities);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<Firma> Read()
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

        private IEnumerable<Firma> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Firma[objEntities.Rows.Count];

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
            return GmDb.Read(TableType, GmFile, string.Empty, null);
        }

        private DataRow Unwrap(Firma objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Rechnungsnummer;
            objDataRow["c1"] = objEntity.Lieferscheinnummer;
            objDataRow["c2"] = objEntity.Posnummer;
            objDataRow["c3"] = objEntity.Zukaufpositionen;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private Firma Wrap(DataRow objDataRow)
        {
            var objEntity = new Firma(GmPath, GmUserData)
            {
                Rechnungsnummer = Convert.ToInt32(objDataRow["c0"]),
                Lieferscheinnummer = Convert.ToInt32(objDataRow["c1"]),
                Posnummer = Convert.ToInt32(objDataRow["c2"]),
                Zukaufpositionen = Convert.ToInt32(objDataRow["c3"]),
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

        private int _rechnungsnummer;
        [DataMember]
        public int Rechnungsnummer
        {
            get { return _rechnungsnummer; }
            set { if ((_rechnungsnummer != value)) { SendPropertyChanging(); _rechnungsnummer = value; SendPropertyChanged(); } }
        }

        private int _lieferscheinnummer;
        [DataMember]
        public int Lieferscheinnummer
        {
            get { return _lieferscheinnummer; }
            set { if ((_lieferscheinnummer != value)) { SendPropertyChanging(); _lieferscheinnummer = value; SendPropertyChanged(); } }
        }

        private int _posnummer;
        [DataMember]
        public int Posnummer
        {
            get { return _posnummer; }
            set { if ((_posnummer != value)) { SendPropertyChanging(); _posnummer = value; SendPropertyChanged(); } }
        }

        private int _zukaufpositionen;
        [DataMember]
        public int Zukaufpositionen
        {
            get { return _zukaufpositionen; }
            set { if ((_zukaufpositionen != value)) { SendPropertyChanging(); _zukaufpositionen = value; SendPropertyChanged(); } }
        }

        #endregion    
    }
}
