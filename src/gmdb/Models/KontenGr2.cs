namespace gmdb.Models
{
    public class KontenGr2 : GmBase
    {
        public KontenGr2(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.KONTENGR2)
        {
            
        }
        public KontenGr2 DeepCopy
        {
            get
            {
                var objClone = (KontenGr2)this.MemberwiseClone();
                return objClone;
            }
        }
    }
}
