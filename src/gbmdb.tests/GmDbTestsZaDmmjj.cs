using System;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsZaDmmjj : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_ZaDmmjj_Read_Month()
        {
            short sMonat = 9;
            short sJahr = 12;
            dtStart = DateTime.Now;
            var cobjResults = new ZaDmmjj(sMonat, sJahr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 4771;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_ZaDmmjj_Read_Last6Months()
        {
            short sMonate = 6;
            var dtBeforeDate = dtStart = new DateTime(2012, 11, 30);

            dtStart = DateTime.Now;
            var cobjResults = new ZaDmmjj(dtBeforeDate, sMonate, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 25529;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_ZaDmmjj_Read_Month_With_ZahlungId()
        {
            short sMonat = 10;
            short sJahr = 12;
            int iZahlungId = 578;
            dtStart = DateTime.Now;
            var cobjResults = new ZaDmmjj(sMonat, sJahr, iZahlungId, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 6;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }
    }
}
