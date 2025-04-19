using System;
using System.Data;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsOffen : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_Offen_Read_All()
        {
            int iKontoNr = GmDb.ALL;
            int iBelegNr = GmDb.ALL;
            int iBelegdatum = GmDb.ALL;

            dtStart = DateTime.Now;
            var objResult = new Offen(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("CheckIndexTestsOffen: for {0}/{1}/{2} times:{3}/{4}/{5}", iKontoNr, iBelegNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 2975;
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited OFFEN count: {0}, read{1}", iAwaitedCount, objResult.Count));
        }

        [TestMethod]
        public void GmDb_Offen_Read_With_KontoNr()
        {
            int iKontoNr = 10374;
            int iBelegNr = GmDb.ALL;
            int iBelegdatum = GmDb.ALL;

            dtStart = DateTime.Now;
            var objResult = new Offen(iKontoNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("CheckIndexTestsOffen: for {0}/{1}/{2} times:{3}/{4}/{5}", iKontoNr, iBelegNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 3;
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited OFFEN count: {0}, read{1}", iAwaitedCount, objResult.Count));            
        }

        [TestMethod]
        public void GmDb_Offen_Read_With_KontoNr_BelegNr()
        {
            int iKontoNr = 81846;
            int iBelegNr = 2238022;
            int iBelegdatum = GmDb.ALL;

            dtStart = DateTime.Now;
            var objResult = new Offen(iKontoNr, iBelegNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("CheckIndexTestsOffen: for {0}/{1}/{2} times:{3}/{4}/{5}", iKontoNr, iBelegNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 1;
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited OFFEN count: {0}, read{1}", iAwaitedCount, objResult.Count));
            DateTime dtBelegdatum = Convert.ToDateTime("2012-10-30 00:00:00.000");
            decimal dtBetrag = 831.06M;
            decimal dtOffen = 831.06M;
            Assert.IsTrue(objResult[0].Rechnungsdatum == dtBelegdatum, string.Format("Awaited ZULETZT Menge: {0}, read{1}", dtBelegdatum, objResult[0].Rechnungsdatum));
            Assert.IsTrue(objResult[0].Rechnungsbetrag == dtBetrag, string.Format("Awaited ZULETZT Anzahl: {0}, read{1}", dtBetrag, objResult[0].Rechnungsbetrag));
            Assert.IsTrue(objResult[0].Offenerbetrag == dtOffen, string.Format("Awaited ZULETZT Preis: {0}, read{1}", dtOffen, objResult[0].Offenerbetrag));
        }
    }
}
