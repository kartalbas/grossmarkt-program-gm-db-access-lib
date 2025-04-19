using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsKonten : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_Konten_Read_All()
        {
            dtStart = DateTime.Now;
            var cobjResults = new Konten(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Konten_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 4368;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Konten_Read_With_KontoNr_GruppenNr()
        {
            int iKontoNr = 0;
            short iGruppenNr = 0;
            List<Konten> cobjResults = null;

            //one specific kontonr + groupnr
            iKontoNr = 31705;
            iGruppenNr = 32;
            dtStart = DateTime.Now;

            cobjResults = new Konten(iKontoNr, iGruppenNr, GmPath, GmUserData).Read().ToList();

            dtStop = DateTime.Now;
            Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].KontoNr == iKontoNr, string.Format("Konten:{0} not found", iKontoNr));

            //one specific kontonr + groupnr
            iKontoNr = 20109;
            iGruppenNr = 28;
            dtStart = DateTime.Now;
            cobjResults = new Konten(iKontoNr, iGruppenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].KontoNr == iKontoNr, string.Format("Konten:{0} not found", iKontoNr));
        }

        [TestMethod]
        public void GmDb_Konten_Read_With_KontenNr()
        {
            int iKontoNr = 0;
            short iGruppenNr = 0;
            List<Konten> cobjResults = null;

            //one specific kontonr without groupnr
            iKontoNr = 31705;
            dtStart = DateTime.Now;
            cobjResults = new Konten(iKontoNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults[0].KontoNr == iKontoNr, string.Format("Konten:{0} not found!", iKontoNr));
        }

        [TestMethod]
        public void GmDb_Konten_Read_SearchWithKeyword()
        {
            int iKontoNr = 0;
            short iGruppenNr = 0;
            List<Konten> cobjResults = null;

            //searching with string
            int iAwaitedCount = 102;
            string strSearch = "mark";
            dtStart = DateTime.Now;
            cobjResults = new Konten(strSearch, GmPath, GmUserData).Read().ToList();

            dtStop = DateTime.Now;
            Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited group count:{0} but read{1}", iAwaitedCount, cobjResults.Count));            
        }

        [TestMethod]
        public void GmDb_Konten_Read_With_GruppenNr()
        {
            int iKontoNr = 0;
            short iGruppenNr = 0;
            List<Konten> cobjResults = null;

            //one specific groupnr without kontonr
            iKontoNr = GmDb.ALL;
            iGruppenNr = 28;
            int iCountGroups = 64;
            dtStart = DateTime.Now;
            cobjResults = new Konten(iGruppenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iCountGroups, string.Format("Awaited group count:{0} but read:{1}", iCountGroups, cobjResults.Count));

            //one specific groupnr without kontonr
            int iAwaitedCount = 4368;
            dtStart = DateTime.Now;
            cobjResults = new Konten(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited group count:{0} read{1}", iAwaitedCount, cobjResults.Count)); 
        }

        //[TestMethod]
        //public void GmDb_Konten_ReBuild_Index()
        //{
        //    GmDb objGmDb = GmDb.Instance(GmPath, GmUserData);

        //    dtStart = DateTime.Now;
        //    int iTotalIndexes = objGmDb.RebuildIndex(TableTypes.KONTEN, Files.Konten);
        //    dtStop = DateTime.Now;

        //    int iAwaited = 5184;

        //    Log("GmDb_Konten_ReCreate_Index: for {0} times:{1}/{2}/{3}", iTotalIndexes, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
        //    Assert.IsTrue(iTotalIndexes == iAwaited, string.Format("Awaited count:{0} but read:{1}", iAwaited, iTotalIndexes));
        //}

        [TestMethod]
        public void GmDb_Konten_Update_Index()
        {
            //GmDb objGmDb = GmDb.Instance(GmPath, GmUserData);
            //string strFile = Files.Konten;

            ////read one specific kontonr + groupnr
            //int iKontoNr = 31705;
            //int iGruppenNr = 32;
            //dtStart = DateTime.Now;
            //DataTable objKonto = objGmDb.Read(TableTypes.KONTEN, strFile, "", iKontoNr, iGruppenNr);
            //dtStop = DateTime.Now;

            ////create new index based on readed konto
            //var cobjSrcIndexes = new List<int>
            //{
            //    461,
            //    Convert.ToInt32(objKonto.Rows[0]["c1"].ToString()),
            //    Convert.ToInt32(objKonto.Rows[0]["c14"].ToString())
            //};

            ////create new index for updating the readed konto index
            //var cobjDestIndexes = new List<int>();
            //int iNewKontoNr = 69999;
            //int iNewGruppenNr = 99;
            //int iAwaitedIndex = 461;
            //cobjDestIndexes.Add(0);
            //cobjDestIndexes.Add(iNewKontoNr);
            //cobjDestIndexes.Add(iNewGruppenNr);

            ////update existing index with new index
            //dtStart = DateTime.Now;
            //bool bUpdated = objGmDb.UpdateIndex(TableTypes.KONTEN, strFile, cobjSrcIndexes, cobjDestIndexes);
            //Assert.IsTrue(bUpdated, string.Format("No Index for Konten {0} found!", iNewKontoNr));
            //List<int> cobjNewIndex = objGmDb.SearchIndex(TableTypes.KONTEN, strFile, iNewKontoNr, iNewGruppenNr);
            //Assert.IsTrue(cobjNewIndex[0] == iAwaitedIndex, string.Format("Update Konten to new index failed at {0}!", iAwaitedIndex));
            //cobjDestIndexes[0] = iAwaitedIndex;

            ////update existing index with original index
            //bUpdated = objGmDb.UpdateIndex(TableTypes.KONTEN, strFile, cobjDestIndexes, cobjSrcIndexes);
            //Assert.IsTrue(bUpdated, string.Format("No Index for Konten {0} found!", iKontoNr));
            //cobjNewIndex = objGmDb.SearchIndex(TableTypes.KONTEN, strFile, iKontoNr, iGruppenNr);
            //Assert.IsTrue(cobjNewIndex[0] == iAwaitedIndex, string.Format("Update Konten to old index failed at {0}!", iAwaitedIndex));
            //dtStop = DateTime.Now;
            //Log("CheckIndexTestsKONTEN: for {0}/{1} times:{2}/{3}/{4}", iNewKontoNr, iNewGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
        }

        [TestMethod]
        public void GmDb_Konten_Add_Index()
        {
            //GmDb objGmDb = GmDb.Instance(GmPath, GmUserData);
            //string strFile = Files.Konten;

            ////create new index for updating the readed konto index
            //var cobjDestIndexes = new List<int>();
            //int iNewKontoNr = 69999;
            //int iNewGruppenNr = 99;
            //cobjDestIndexes.Add(0);
            //cobjDestIndexes.Add(iNewKontoNr);
            //cobjDestIndexes.Add(iNewGruppenNr);

            //int iAwaitedIndex = objGmDb.AddIndex(TableTypes.KONTEN, strFile, cobjDestIndexes);
            //Assert.IsTrue(iAwaitedIndex == 2107, string.Format("GmDb_Konten_Add_Index: No Index for Konten {0} found after adding a new index!", iNewKontoNr));
            //var cobjNewIndex = objGmDb.SearchIndex(TableTypes.KONTEN, strFile, iNewKontoNr, iNewGruppenNr);

            ////delete the newly added index
            //cobjDestIndexes = new List<int>();
            //cobjDestIndexes.Add(iAwaitedIndex);
            //cobjDestIndexes.Add(iNewKontoNr);
            //cobjDestIndexes.Add(iNewGruppenNr);

            //List<int> cobjDummyIndexes = new List<int>();
            //cobjDummyIndexes.Add(iAwaitedIndex);
            //cobjDummyIndexes.Add(-1);
            //cobjDummyIndexes.Add(-1);
            //bool bDeleted = objGmDb.UpdateIndex(TableTypes.KONTEN, strFile, cobjDestIndexes, cobjDummyIndexes);
            //Assert.IsTrue(bDeleted, string.Format("GmDb_Konten_Add_Index: index deleted was not possible {0}!", bDeleted));

            //cobjNewIndex = objGmDb.SearchIndex(TableTypes.KONTEN, strFile, iNewKontoNr, iNewGruppenNr);
            //Assert.IsTrue(cobjNewIndex.Count == 0, string.Format("GmDb_Konten_Add_Index: Found added index for Konten failed {0}!", iAwaitedIndex));
            //dtStop = DateTime.Now;
        }

        [TestMethod]
        public void GmDb_Konten_Export_Index()
        {
            string strNewFilename = string.Format(@"D:\{0:yyyy-MM-dd_HH-mm-ss-fff}_{1}", DateTime.Now, "KONTEN.XLS");
            GmDb.Instance(GmPath, GmUserData).ExportIndex(TableTypes.KONTEN, Files.Konten, strNewFilename, "\t");            
        }

        //[TestMethod]
        //public void GmDb_Konten_Add_Konto()
        //{
        //    int iNewKontoNr = 69999;
        //    short iGruppenNr = GmDb.ALL;

        //    var objKonto = new Konten(GmPath, GmUserData)
        //    {
        //        Delete = 0,
        //        KontoNr = iNewKontoNr,
        //        Name = "simetrix GmbH",
        //        Branche = "Software Entwicklung",
        //        Strasse = "Via Ginellas 3",
        //        PlzOrt = "CH-7402 Bonaduz",
        //        Kurzname = "Mobile Lösungen",
        //        Zustaendig = "Mehmet Kartalbas",
        //        Vorwahl = "+4179",
        //        Telefon1 = "3449399",
        //        Telefon2 = "3449399",
        //        Fax = "0123456789",
        //        Telex = "9876543210",
        //        Steuergruppe = 1,
        //        GruppenNr = 6,
        //        Kreditlimit = 10000,
        //        Zahlungsziel = 30,
        //        Mahnung = "J",
        //        Rabatt = 0,
        //        Rueckverguetung = 0,
        //        Markenbezeichnung = string.Empty,
        //        LiPositionVonKURechnungsgruppe = 0,
        //        PositionBis = 0,
        //        Land = 0,
        //        Station = 0,
        //        LiLeergutKUPreisgruppe = 0,
        //        Tour = 0,
        //        Entsorgung = 0,
        //        Unbekannt1 = string.Empty,
        //        Textschluessel = 0,
        //        SteuerID = "555/555/55"
        //    };

        //    var objNewKonto = objKonto.Save(objKonto).FirstOrDefault();

        //    Log("GmDb_Konten_Add_Konto: for {0}/{1} times:{2}/{3}/{4}", iNewKontoNr, iGruppenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
        //    GmDb.Instance(GmPath, GmUserData).RestoreLatest(Files.Konten);

        //    Assert.IsNotNull(objNewKonto, string.Format("GmDb_Konten_Add_Konto: Konto {0} is null!", iNewKontoNr));
        //    Assert.IsTrue(objNewKonto.KontoNr == iNewKontoNr, string.Format("GmDb_Konten_Add_Konto: Konto {0} not found!", iNewKontoNr));
        //}
    }
}
