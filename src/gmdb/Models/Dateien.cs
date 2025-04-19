namespace gmdb.Models
{
    public class Dateien : GmBase
    {
        public Dateien(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.DATEIEN)
        {
            
        }

        public Dateien DeepCopy
        {
            get
            {
                var objClone = (Dateien)this.MemberwiseClone();
                return objClone;
            }
        }
    }
}
