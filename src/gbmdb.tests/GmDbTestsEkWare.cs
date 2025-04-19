using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsEkWare : GmDbTestsBase
    {
        private short iBelegID = GmDb.ALL;
        private int iWarenNr = GmDb.ALL;
        private short iPositionsNr = GmDb.ALL;
        private int iKontoNr = GmDb.ALL;
        private int iPoslineNr = GmDb.ALL;
        private int iBelegdatum = GmDb.ALL;
        private short iChargenNr = GmDb.ALL;
        private int iAwaited = GmDb.ALL;
        private DateTime dBelegdatum = default(DateTime);

        [TestMethod]
        public void GmDb_EkWaren_Read_All()
        {
            iAwaited = 32805;

            var objEkWaren = new EkWare(GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objEkWaren.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_EkWaren_Read_All: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iWarenNr, iPositionsNr, iKontoNr, iPoslineNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objResult.Count == iAwaited, string.Format("Awaited EKWare count: {0}, read: {1}", iAwaited, objResult.Count));            
        }

        [TestMethod]
        public void GmDb_EkWaren_Read_With_KontoNr()
        {
            iKontoNr = 70771;

            var objEkWaren = new EkWare(GmPath, GmUserData) { KontoNr = iKontoNr };
            dtStart = DateTime.Now;
            var objResult = objEkWaren.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_EkWaren_Read_With_KontoNr: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iWarenNr, iPositionsNr, iKontoNr, iPoslineNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            iAwaited = 114;
            Assert.IsTrue(objResult.Count == iAwaited, string.Format("Awaited EKWare count: {0}, read: {1}", iAwaited, objResult.Count));
        }

        [TestMethod]
        public void GmDb_EkWaren_Read_With_WarenNr()
        {
            iWarenNr = 345;

            var objEkWaren = new EkWare(iWarenNr, GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objEkWaren.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_EkWaren_Read_With_WarenNr: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iWarenNr, iPositionsNr, iKontoNr, iPoslineNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            iAwaited = 19;
            Assert.IsTrue(objResult.Count == iAwaited, string.Format("Awaited count of rows:{0} but reade: {1}", iAwaited, objResult.Count));
        }

        [TestMethod]
        public void GmDb_EkWaren_Read_With_WarenNr_Positionsnummer()
        {
            iWarenNr = 345;
            iChargenNr = 5939;

            var objEkWaren = new EkWare(iBelegID, iWarenNr, iChargenNr, GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objEkWaren.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_EkWaren_Read_With_WarenNr_Positionsnummer: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iWarenNr, iChargenNr, iKontoNr, iPoslineNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            iAwaited = 702;
            Assert.IsTrue(objResult[0].Inhalt == iAwaited, string.Format("VKWare {0} not with stock {1} found!", iWarenNr, iAwaited));
        }

        [TestMethod]
        public void GmDb_EkWaren_Read_With_All_Indexes()
        {
            iBelegID = 7221;
            iWarenNr = 501;
            iChargenNr = 5394;
            iKontoNr = 70505;
            iPositionsNr = 1;
            dBelegdatum = new DateTime(2012, 10, 31);

            dtStart = DateTime.Now;
            var objEkWaren = new EkWare(iBelegID, iWarenNr, iChargenNr, iKontoNr, iPositionsNr, dBelegdatum, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_EkWaren_Read_With_WarenNr_Positionsnummer: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iWarenNr, iChargenNr, iKontoNr, iPoslineNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            iAwaited = 7221;
            Assert.IsTrue(objEkWaren[0].BelegeId == iAwaited, string.Format("VKWare {0} not with stock {1} found!", iWarenNr, iAwaited));
        }

        [TestMethod]
        public void GmDb_EkWare_Export_Index()
        {
            string strNewFilename = string.Format(@"D:\{0:yyyy-MM-dd_HH-mm-ss-fff}_{1}", DateTime.Now, "EKWARE.XLS");
            GmDb.Instance(GmPath, GmUserData).ExportIndex(TableTypes.EKWARE, Files.EKWare, strNewFilename, "\t");
        }
    }
}
