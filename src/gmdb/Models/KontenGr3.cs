namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class KontenGr3 : GmBase, IEnumerator
    {
        #region private properties

        private KontenGr3[] _aobjEntities;

        #endregion

        #region Constructors

        public KontenGr3(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.KONTENGR3)
        {
            
        }

        #endregion

        #region public methods

        public KontenGr3 DeepCopy
        {
            get
            {
                var objClone = (KontenGr3)this.MemberwiseClone();
                return objClone;
            }
        }

        public IEnumerable<KontenGr3> Read()
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

        private IEnumerable<KontenGr3> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new KontenGr3[objEntities.Rows.Count];

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
                return GmDb.Read(TableType, GmFile, string.Empty, null);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private KontenGr3 Wrap(DataRow objDataRow)
        {
            var objEntity = new KontenGr3(GmPath, GmUserData)
            {
                Name = objDataRow["c0"].ToString(),
                Unbekannt1 = Convert.ToInt16(objDataRow["c1"]),
                Unbekannt2 = Convert.ToDecimal(objDataRow["c2"]),
                Unbekannt3 = Convert.ToDecimal(objDataRow["c3"]),

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

        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { if ((_name != value)) { SendPropertyChanging(); _name = value; SendPropertyChanged(); } }
        }

        private short _unbekannt1;
        [DataMember]
        public short Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt2;
        [DataMember]
        public decimal Unbekannt2
        {
            get { return _unbekannt2; }
            set { if ((_unbekannt2 != value)) { SendPropertyChanging(); _unbekannt2 = value; SendPropertyChanged(); } }
        }

        private decimal _unbekannt3;
        [DataMember]
        public decimal Unbekannt3
        {
            get { return _unbekannt3; }
            set { if ((_unbekannt3 != value)) { SendPropertyChanging(); _unbekannt3 = value; SendPropertyChanged(); } }
        }

        #endregion
    }

}

