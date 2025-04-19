using System;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsZahlung : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_Zahlung_Read_All()
        {
            dtStart = DateTime.Now;
            var cobjResults = new Zahlung(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Zahlung_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 483;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Zahlung_Read_With_KontoNr()
        {
            int iKontoNr = 31116;
            dtStart = DateTime.Now;
            var cobjResults = new Zahlung(iKontoNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Zahlung_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 42;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Zahlung_Read_With_Zahlungsdatum()
        {
            var dtZahlungsdatum = new DateTime(2012, 11, 19);
            dtStart = DateTime.Now;
            var cobjResults = new Zahlung(dtZahlungsdatum, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Zahlung_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 2;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Zahlung_Read_With_Rechnungsnummer()
        {
            dtStart = DateTime.Now;
            int iRechnungsnummer = 1237940;
            var objReader = new Zahlung(GmPath, GmUserData) { Rechnungsnummer = iRechnungsnummer };
            var cobjResults = objReader.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Zahlung_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            Assert.IsTrue(cobjResults[0].Rechnungsnummer == iRechnungsnummer, string.Format("Awaited value:{0} but read:{1}", iRechnungsnummer, cobjResults[0].Rechnungsnummer));
        }
    }
}
