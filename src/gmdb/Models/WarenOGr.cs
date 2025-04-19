namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class WarenOGr : GmBase, IEnumerator
    {
        #region private properties

        private WarenOGr[] _aobjEntities;

        #endregion

        #region Constructors

        public WarenOGr(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.WARENOGR)
        {
        }

        #endregion

        #region public methods

        public WarenOGr DeepCopy
        {
            get
            {
                var objClone = (WarenOGr)this.MemberwiseClone();
                return objClone;
            }
        }

        public IEnumerable<WarenOGr> Read()
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

        private IEnumerable<WarenOGr> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new WarenOGr[objEntities.Rows.Count];

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

        private WarenOGr Wrap(DataRow objDataRow)
        {
            var objEntity = new WarenOGr(GmPath, GmUserData)
            {
                Name = objDataRow["c0"].ToString(),
                Unbekannt = Convert.ToInt16(objDataRow["c1"]),
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

        private short _unbekannt;
        [DataMember]
        public short Unbekannt
        {
            get { return _unbekannt; }
            set { if ((_unbekannt != value)) { SendPropertyChanging(); _unbekannt = value; SendPropertyChanged(); } }
        }

        #endregion
    }

}

