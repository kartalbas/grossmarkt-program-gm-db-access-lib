namespace gmdb.Models
{
    using System.Collections.Generic;

    public class Globals
    {
        private static Globals _objInstance;
        private static readonly object LOCK = new object();
        private string GmPath { get; set; }
        private string GmUserData { get; set; }

        public Globals(string strGmPath, string strGmUserData)
        {
            GmPath = strGmPath;
            GmUserData = strGmUserData;
        }

        private IEnumerable<Steuer> _objSteuer;
        public IEnumerable<Steuer> Steuer 
        {
            get
            {
                if(_objSteuer == null)
                    _objSteuer = new Steuer(GmPath, GmUserData).Read();

                return _objSteuer;
            }
        }

        public static Globals Instance(string strGmPath, string strGmUserData)
        {
            lock (LOCK)
            {
                if (_objInstance == null)
                    _objInstance = new Globals(strGmPath, strGmUserData);

                return _objInstance;
            }
        }
    }
}
