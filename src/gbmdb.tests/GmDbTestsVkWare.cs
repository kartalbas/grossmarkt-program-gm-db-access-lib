using System;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsVkWare : GmDbTestsBase
    {
        private int iBelegID = GmDb.ALL;
        private int iArtikelID = GmDb.ALL;
        private int iWarenNr = GmDb.ALL;
        private int iChargenNr = GmDb.ALL;
        private int iBelegNr = GmDb.ALL;
        private DateTime dtRechnungsdatum = default(DateTime);
        private short iPosNr = GmDb.ALL;
        private int iAwaitedCount = 0;

        [TestMethod]
        public void GmDb_VkWare_Read_With_BelegeId()
        {
            //specific VKBeleg 
            iBelegID = 618;
            iAwaitedCount = 18;
            dtStart = DateTime.Now;
            var cobjResults = new VkWare(iBelegID, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_VkWare_Read_With_BelegeId: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iChargenNr, dtRechnungsdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read: {1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkWare_Read_With_Belegdatum()
        {
            dtRechnungsdatum = new DateTime(2012, 11, 08);
            iAwaitedCount = 2873;

            dtStart = DateTime.Now;
            var cobjResults = new VkWare(dtRechnungsdatum, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iChargenNr, dtRechnungsdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkWare_Read_With_ChargenNr()
        {
            iChargenNr = 6503;
            iAwaitedCount = 23;
            dtStart = DateTime.Now;
            var cobjResults = (new VkWare(GmPath, GmUserData) { ChargenNr = iChargenNr }).Read().ToList();
            dtStop = DateTime.Now;

            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iChargenNr, dtRechnungsdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkWare_Read_With_BelegId_ChargenNr()
        {
            iBelegID = 202;
            iChargenNr = 6713;
            iAwaitedCount = 8;
            dtStart = DateTime.Now;
            var cobjResults = new VkWare(iBelegID, GmDb.ALL, GmDb.ALL, iChargenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            iBelegNr = 1238375;
            Log("CheckIndexTestsVkWare: for {0}/{1}/{2}/{3}/{4}/{5} times:{6}/{7}/{8}", iBelegID, iArtikelID, iWarenNr, iChargenNr, dtRechnungsdatum, iPosNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].Rechnungsnummer == iBelegNr, string.Format("VKWare {0} not found!", iBelegNr));
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited vkware count: {0}, read{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_VkWare_Export_Index()
        {
            string strNewFilename = string.Format(@"D:\{0:yyyy-MM-dd_HH-mm-ss-fff}_{1}", DateTime.Now, "VKWARE.XLS");
            GmDb.Instance(GmPath, GmUserData).ExportIndex(TableTypes.VKWARE, Files.VKWare, strNewFilename, "\t");
        }
    }
}
