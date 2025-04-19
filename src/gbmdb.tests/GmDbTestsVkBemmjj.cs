using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsVkBemmjj : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_VkBemmjj_Read_Month()
        {
            short sMonat = 9;
            short sJahr = 12;
            dtStart = DateTime.Now;
            var cobjResults = new VkBemmjj(sMonat, sJahr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 3520;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkBemmjj_Read_Last6Months()
        {
            short sMonate = 6;
            var dtBeforeDate = dtStart = new DateTime(2012, 11, 30);

            dtStart = DateTime.Now;
            var cobjResults = new VkBemmjj(dtBeforeDate, sMonate, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 18907;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkBemmjj_Read_With_KundenNr_Last6Months()
        {
            short sMonate = 6;
            int iKundenNr = 12155;
            var dtBeforeDate = dtStart = new DateTime(2012, 11, 30);

            dtStart = DateTime.Now;
            var cobjResults = new VkBemmjj(dtBeforeDate, sMonate, iKundenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 82;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkBemmjj_Read_Month_With_KontoNr()
        {
            short sMonat = 10;
            short sJahr = 12;
            int iKundenNr = 12155;
            dtStart = DateTime.Now;
            var cobjResults = new VkBemmjj(sMonat, sJahr, iKundenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmbDbTestsZaBmmjj: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 12;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }
    }
}
