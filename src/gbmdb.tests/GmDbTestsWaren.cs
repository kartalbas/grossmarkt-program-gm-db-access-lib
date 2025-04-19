using System;
using System.Collections.Generic;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsWaren : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_Waren_Read_SearchWithKeyword()
        {
            //searching with string
            int iWarenNr = 0;
            Int16 iGruppenNr = 0;
            int iAwaitedCount = 32;
            string strSearch = "ege";
            dtStart = DateTime.Now;
            var cobjResults = new Waren(strSearch, GmPath, GmUserData).Read().ToList();

            dtStop = DateTime.Now;
            Log("GmDb_Waren_Read_SearchWithKeyword: for {0}/{1} times:{2}/{3}/{4}", iWarenNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Waren_Read_With_WarenNr_GruppenNr()
        {
            int iWarenNr = 0;
            Int16 iGruppenNr = 0;
            List<Waren> cobjResults = null;

            //one specific warennr + groupnr
            iWarenNr = 1095;
            iGruppenNr = 83;
            dtStart = DateTime.Now;
            cobjResults = new Waren(iWarenNr, iGruppenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Waren_Read_With_WarenNr_GruppenNr: for {0}/{1} times:{2}/{3}/{4}", iWarenNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].ArtikelNr == iWarenNr, string.Format("No result with WarenNr:{0} and GruppenNR{1}", iWarenNr, iGruppenNr));

            //one specific warennr + groupnr
            iWarenNr = 9804;
            iGruppenNr = 47;
            dtStart = DateTime.Now;
            cobjResults = new Waren(iWarenNr, iGruppenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("CheckIndexTestsWAREN: for {0}/{1} times:{2}/{3}/{4}", iWarenNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].ArtikelNr == iWarenNr, string.Format("No result with WarenNr:{0} and GruppenNR{1}", iWarenNr, iGruppenNr));
        }

        [TestMethod]
        public void GmDb_Waren_Read_With_WarenNr()
        {
            int iWarenNr = 0;
            Int16 iGruppenNr = 0;
            List<Waren> cobjResults = null;

            //one specific warennr without groupnr
            iWarenNr = 7122;
            dtStart = DateTime.Now;
            cobjResults = new Waren(iWarenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Waren_Read_With_WarenNr: for {0}/{1} times:{2}/{3}/{4}", iWarenNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].ArtikelNr == iWarenNr, string.Format("No result with WarenNr:{0}", iWarenNr));
        }

        [TestMethod]
        public void GmDb_Waren_Read_With_GruppenNr()
        {
            int iWarenNr = 0;
            Int16 iGruppenNr = 0;
            List<Waren> cobjResults = null;

            //one specific groupnr without warennr
            iGruppenNr = 78;
            int iCountGroups = 14;
            dtStart = DateTime.Now;
            cobjResults = new Waren(iGruppenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Waren_Read_With_GruppenNr: for {0}/{1} times:{2}/{3}/{4}", iWarenNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iCountGroups, string.Format("Awaited collection count:{0}, but read:{1}", iCountGroups, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Waren_Read_All()
        {
            int iWarenNr = 0;
            Int16 iGruppenNr = 0;
            List<Waren> cobjResults = null;

            //all
            int iAwaitedCount = 2896;
            dtStart = DateTime.Now;
            cobjResults = new Waren(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Waren_Read_All: for {0}/{1} times:{2}/{3}/{4}", iWarenNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited collection count:{0}, but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Waren_Export_Index()
        {
            string strNewFilename = string.Format(@"D:\{0:yyyy-MM-dd_HH-mm-ss-fff}_{1}", DateTime.Now, "WAREN.XLS");
            GmDb.Instance(GmPath, GmUserData).ExportIndex(TableTypes.WAREN, Files.Waren, strNewFilename, "\t");
        }
    }
}
