namespace gmdb.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class Program : GmBase, IEnumerator
    {
        #region private properties

        private Program[] _aobjEntities;

        #endregion

        #region Constructors

        public Program(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.PROGRAM)
        {
        }

        #endregion

        #region public methods

        public Program DeepCopy
        {
            get
            {
                var objClone = (Program)this.MemberwiseClone();
                return objClone;
            }
        }

        public Program Login(string strUsername, string strPassword)
        {
            var objUsers = Read().ToList();
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var objUser = objUsers.Where(u => u.Username == strUsername && u.Password == strPassword).FirstOrDefault();
            return objUser;
        }

        public List<string> Usernames
        {
            get
            {
                return Read().Select(u => u.Username).ToList().GetRange(0, 16);
            }
        }

        public IEnumerable<Program> Read()
        {
            try
            {
                var objResult = ReadEntities();
                var objUnwraped = Read(objResult).ToList();
                return objUnwraped;
            }
            catch (Exception objException)
            {
                GmDb.Log(objException);
                throw;
            }
        }

        private IEnumerable<Program> Read(DataTable objEntities)
        {
            if (objEntities == null)
                yield break;

            _aobjEntities = new Program[objEntities.Rows.Count];

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

        private Program Wrap(DataRow objDataRow)
        {
            var objEntity = new Program(GmPath, GmUserData)
            {
                Username = objDataRow["c0"].ToString(),
                Password = objDataRow["c1"].ToString(),
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

        private string _username;
        [DataMember]
        public string Username
        {
            get { return _username; }
            set { if ((_username != value)) { SendPropertyChanging(); _username = value; SendPropertyChanged(); } }
        }

        private string _password;
        [DataMember]
        public string Password
        {
            get { return _password; }
            set { if ((_password != value)) { SendPropertyChanging(); _password = value; SendPropertyChanged(); } }
        }

        #endregion
    }
}
