using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace gmdb.tests
{
    [TestClass]
    public class _GmIndexReaderTests
    {
        public static string GmPath { get; set; }
        public static string GmUserData { get; set; }

        public _GmIndexReaderTests()
        {
            GmPath = @"D:\GM";
            GmUserData = "USERDATA";
        }

        public TestContext TestContext { get; set; }

        #region Helper methods

        private void Log(string strFormat, params object[] aobjArgs)
        {
            Console.Write("{0}: ", DateTime.Now);
            Console.WriteLine(strFormat, aobjArgs);
        }

        #endregion

        [TestMethod]
        public void CheckIndexTestsVkWare()
        {
            int iBelegID = 0;
            int iArtikelID = 0;
            int iWarenNr = 0;
            int iPositionsNr = 0;
            int iBelegdatum = 0;
            int iPosNr = 0;

            int iAwaitedCount = 0;
            GmDb objReader = GmDb.Instance(GmPath, GmUserData);
            DateTime dtStart = default(DateTime);
            DateTime dtStop = default(DateTime);

            //specific VKBeleg 
            iBelegID = 618;
            iArtikelID = GmDb.ALL;
            iWarenNr = GmDb.ALL;
            iPositionsNr = GmDb.ALL;
            iBelegdatum = GmDb.ALL;
            iPosNr = GmDb.ALL;
            iAwaitedCount = 18;
            dtStart = DateTime.Now;
            DataTable objVKWare11 = objReader.Read(TableTypes.VKWARE, Files.VKWare, "", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objVKWare11.Rows.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, objVKWare11.Rows.Count));

            //VKBeleg with Belegdatum
            iBelegID = GmDb.ALL;
            iArtikelID = GmDb.ALL;
            iWarenNr = GmDb.ALL;
            iPositionsNr = GmDb.ALL;
            iBelegdatum = 20121108;
            iPosNr = GmDb.ALL;
            iAwaitedCount = 2873;
            dtStart = DateTime.Now;
            DataTable objVKWare12 = objReader.Read(TableTypes.VKWARE, Files.VKWare, "", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objVKWare12.Rows.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, objVKWare12.Rows.Count));

            //VKBeleg with Belegdatum
            iBelegID = GmDb.ALL;
            iArtikelID = GmDb.ALL;
            iWarenNr = GmDb.ALL;
            iPositionsNr = 6503;
            iBelegdatum = GmDb.ALL;
            iPosNr = GmDb.ALL;
            iAwaitedCount = 23;
            dtStart = DateTime.Now;
            DataTable objVKWare13 = objReader.Read(TableTypes.VKWARE, Files.VKWare, "", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objVKWare13.Rows.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, objVKWare13.Rows.Count));

            //VKBeleg with Belegdatum
            iBelegID = 202;
            iArtikelID = GmDb.ALL;
            iWarenNr = GmDb.ALL;
            iPositionsNr = GmDb.ALL;
            iBelegdatum = GmDb.ALL;
            iPosNr = 20;
            int iBelegNr = 29405;
            dtStart = DateTime.Now;
            DataTable objVKWare14 = objReader.Read(TableTypes.VKWARE, Files.VKWare, "", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iPositionsNr, iBelegdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(Convert.ToInt32(objVKWare14.Rows[0]["c4"].ToString()) == iBelegNr, string.Format("VKWare {0} not found!", iBelegNr));
        }

        [TestMethod]
        public void CheckIndexTestsEkBeleg()
        {
            int iKontoNr = 0;
            int iPositionsNr = 0;
            int iBelegNr = 0;
            int iBelegdatum = 0;

            GmDb objReader = GmDb.Instance(GmPath, GmUserData);
            DateTime dtStart = default(DateTime);
            DateTime dtStop = default(DateTime);

            iKontoNr = GmDb.ALL;
            iPositionsNr = GmDb.ALL;
            iBelegNr = 9538460;
            iBelegdatum = GmDb.ALL;
            dtStart = DateTime.Now;
            DataTable objEKBeleg11 = objReader.Read(TableTypes.EKBELEG, Files.EKBeleg, "", iKontoNr, iPositionsNr, iBelegNr, iBelegdatum);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsEkBeleg: for {0}/{1}/{2}/{3} times:{4}/{5}/{6}", iKontoNr, iPositionsNr, iBelegNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(Convert.ToInt32(objEKBeleg11.Rows[0]["c5"].ToString()) == iBelegNr, string.Format("EKBeleg {0} not found!", iBelegNr));

            iKontoNr = 70771;
            iPositionsNr = GmDb.ALL;
            iBelegNr = GmDb.ALL;
            iBelegdatum = GmDb.ALL;
            int iAwaitedCount = 64;
            dtStart = DateTime.Now;
            DataTable objEKBeleg12 = objReader.Read(TableTypes.EKBELEG, Files.EKBeleg, "", iKontoNr, iPositionsNr, iBelegNr, iBelegdatum);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsEkBeleg: for {0}/{1}/{2}/{3} times:{4}/{5}/{6}", iKontoNr, iPositionsNr, iBelegNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objEKBeleg12.Rows.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, objEKBeleg12.Rows.Count));

            iKontoNr = 70771;
            iPositionsNr = GmDb.ALL;
            iBelegNr = 9538206;
            iBelegdatum = 20120405;
            dtStart = DateTime.Now;
            DataTable objEKBeleg13 = objReader.Read(TableTypes.EKBELEG, Files.EKBeleg, "", iKontoNr, iPositionsNr, iBelegNr, iBelegdatum);
            dtStop = DateTime.Now;
            Log("CheckIndexTestsEkBeleg: for {0}/{1}/{2}/{3} times:{4}/{5}/{6}", iKontoNr, iPositionsNr, iBelegNr, iBelegdatum, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            iBelegNr = 9538206;
            Assert.IsTrue(Convert.ToInt32(objEKBeleg13.Rows[0]["c5"].ToString()) == iBelegNr, string.Format("EKBeleg {0} not found!", iBelegNr));
        }
    }
}
