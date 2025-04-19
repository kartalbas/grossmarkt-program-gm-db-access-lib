namespace gmdb.Models
{
    public class Country : GmBase
    {
        public Country(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.COUNTRY)
        {        
        }

        public Country DeepCopy
        {
            get
            {
                var objClone = (Country)this.MemberwiseClone();
                return objClone;
            }
        }
    }
}
