namespace gmdb.Models
{
    public class Tour : GmBase
    {
        public Tour(string strGmPath, string strGmUserData)
            : base(strGmPath, strGmUserData, TableTypes.TOUR)
        {
            
        }
        public Tour DeepCopy
        {
            get
            {
                var objClone = (Tour)this.MemberwiseClone();
                return objClone;
            }
        }
    }
}
