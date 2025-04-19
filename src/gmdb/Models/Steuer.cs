namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    [DataContract]
    public class Steuer : GmBase, IEnumerator
    {
        #region private properties

        private Steuer[] _aobjEntities;

        #endregion

        #region Constructors

        public Steuer(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.STEUER)
        {
        }

        #endregion

        #region public methods

        public Steuer DeepCopy
        {
            get
            {
                var objClone = (Steuer)this.MemberwiseClone();
                return objClone;
            }
        }

        public IEnumerable<Steuer> Read()
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

        private IEnumerable<Steuer> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Steuer[objEntities.Rows.Count];

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

        private Steuer Wrap(DataRow objDataRow)
        {
            var objEntity = new Steuer(GmPath, GmUserData)
            {
                Steuersatz1 = Convert.ToDecimal(objDataRow["c0"]),
                Steuersatz2 = Convert.ToDecimal(objDataRow["c1"]),
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

        private decimal _steuersatz1;
        [DataMember]
        public decimal Steuersatz1
        {
            get { return _steuersatz1; }
            set { if ((_steuersatz1 != value)) { SendPropertyChanging(); _steuersatz1 = value; SendPropertyChanged(); } }
        }

        private decimal _steuersatz2;
        [DataMember]
        public decimal Steuersatz2
        {
            get { return _steuersatz2; }
            set { if ((_steuersatz2 != value)) { SendPropertyChanging(); _steuersatz2 = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
