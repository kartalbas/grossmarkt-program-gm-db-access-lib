namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class VkTexte : GmBase, IEnumerator
    {
        #region private properties

        private VkTexte[] _aobjEntities;

        #endregion

        #region Constructors

        public VkTexte(int iBelegeId, string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.VKTEXTE)
        {
            BelegeId = iBelegeId;
        }

        public VkTexte(string strGmPath, string strGmUserData)
            : this(GmDb.ALL, strGmPath, strGmUserData)
        {
        }

        #endregion

        #region public methods

        public VkTexte DeepCopy
        {
            get
            {
                var objClone = (VkTexte)this.MemberwiseClone();
                return objClone;
            }
        }

        public string Update(VkBeleg objVkBeleg)
        {
            try
            {
                if (objVkBeleg != null && !string.IsNullOrEmpty(objVkBeleg.Info) && objVkBeleg.Info.Length > 0)
                {
                    //create
                    int iBelegeId = objVkBeleg.FileId + 1;
                    var objVkTexte = new VkTexte(GmPath, GmUserData)
                    {
                        Delete = 0,
                        BelegeId = iBelegeId,
                        Unbekannt1 = 0,
                        Text = objVkBeleg.Info
                    };
                    //save
                    objVkTexte.Save(objVkTexte);
                    var objSavedVkTexte = new VkTexte(iBelegeId, GmPath, GmUserData).Read().FirstOrDefault();
                    if (objSavedVkTexte != null)
                        objVkBeleg.Info = objSavedVkTexte.Text;

                    if (objSavedVkTexte == null)
                        throw new Exception(string.Format("Beleg Text '{0}' couldn't be saved ", objVkBeleg.Info));
                    //return
                    return objSavedVkTexte.Text;
                }

                return string.Empty;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public void Save(VkTexte objEntity)
        {
            try
            {
                var objResult = Unwrap(objEntity);
                Entities.Rows.Add(objResult);

                if (Entities == null)
                    throw new Exception("No data to save, Entities is null!");

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
                GmDb.Write(TableType, GmFile, Entities);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        public IEnumerable<VkTexte> Read()
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

        private IEnumerable<VkTexte> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new VkTexte[objEntities.Rows.Count];

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
                return GmDb.Read(TableType, GmFile, string.Empty, BelegeId);
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private DataRow Unwrap(VkTexte objEntity)
        {
            var objDataRow = Entities.NewRow();
            objDataRow["c0"] = objEntity.Delete;
            objDataRow["c1"] = objEntity.BelegeId;
            objDataRow["c2"] = objEntity.Unbekannt1;
            objDataRow["c3"] = objEntity.Text;
            objDataRow["ROW"] = objEntity.FileId;
            objDataRow["FILENAME"] = objEntity.File;
            return objDataRow;
        }

        private VkTexte Wrap(DataRow objDataRow)
        {
            var objEntity = new VkTexte(GmPath, GmUserData)
            {
                Delete = Convert.ToInt32(objDataRow["c0"]),
                BelegeId = Convert.ToInt32(objDataRow["c1"]),
                Unbekannt1 = Convert.ToInt16(objDataRow["c2"]),
                Text = objDataRow["c3"].ToString(),
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

        private int _belegeId;
        [DataMember]
        public int BelegeId
        {
            get { return _belegeId; }
            set { if ((_belegeId != value)) { SendPropertyChanging(); _belegeId = value; SendPropertyChanged(); } }
        }

        private short _unbekannt1;
        [DataMember]
        public short Unbekannt1
        {
            get { return _unbekannt1; }
            set { if ((_unbekannt1 != value)) { SendPropertyChanging(); _unbekannt1 = value; SendPropertyChanged(); } }
        }

        private string _text;
        [DataMember]
        public string Text
        {
            get { return _text; }
            set { if ((_text != value)) { SendPropertyChanging(); _text = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
