using System;
using System.Linq;
using System.Runtime.InteropServices;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsVkBeleg : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_VkBeleg_Read_All()
        {
            int iKontoNr = GmDb.ALL;
            int iBelegNr = GmDb.ALL;
            var dtBelegdatum = default(DateTime);
            int iKontrollNr = GmDb.ALL;
            int iAwaitedCount = 411;

            var objVkBeleg = new VkBeleg(GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objVkBeleg.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_VkBeleg_Read_All: for {0}/{1}/{2}/{3}/{4} times:{5}/{6}/{7}", iKontoNr, iBelegNr, dtBelegdatum, iKontoNr, iKontrollNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited VKBeleg count: {0}, read: {1}", iAwaitedCount, objResult.Count));
        }

        [TestMethod]
        public void GmDb_VkBeleg_Read_With_KontoNr()
        {
            int iKontoNr = 10354;
            int iBelegNr = GmDb.ALL;
            var dtBelegdatum = default(DateTime);
            int iKontrollNr = GmDb.ALL;
            int iAwaitedCount = 2;

            var objVkBeleg = new VkBeleg(iKontoNr, GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objVkBeleg.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_VkBeleg_Read_With_KontoNr: for {0}/{1}/{2}/{3}/{4} times:{5}/{6}/{7}", iKontoNr, iBelegNr, dtBelegdatum, iKontoNr, iKontrollNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited VKBeleg count: {0}, read: {1}", iAwaitedCount, objResult.Count));
        }

        [TestMethod]
        public void GmDb_VkBeleg_Read_With_BelegNr()
        {
            int iKontoNr = GmDb.ALL;
            int iBelegNr = 1054895;
            var dtBelegdatum = default(DateTime);
            int iKontrollNr = GmDb.ALL;
            int iAwaitedCount = 1;

            var objVkBeleg = new VkBeleg(iKontoNr, iBelegNr, GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objVkBeleg.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_VkBeleg_Read_With_BelegNr: for {0}/{1}/{2}/{3}/{4} times:{5}/{6}/{7}", iKontoNr, iBelegNr, dtBelegdatum, iKontoNr, iKontrollNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited VKBeleg count: {0}, read: {1}", iAwaitedCount, objResult.Count));
        }

        [TestMethod]
        public void GmDb_VkBeleg_Read_With_Belegdatum()
        {
            int iKontoNr = GmDb.ALL;
            int iBelegNr = GmDb.ALL;
            var dtBelegdatum = new DateTime(2012, 11, 8);
            int iKontrollNr = GmDb.ALL;
            int iAwaitedCount = 171;

            var objVkBeleg = new VkBeleg(iKontoNr, iBelegNr, dtBelegdatum, GmPath, GmUserData);
            dtStart = DateTime.Now;
            var objResult = objVkBeleg.Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_VkBeleg_Read_With_Belegdatum: for {0}/{1}/{2}/{3}/{4} times:{5}/{6}/{7}", iKontoNr, iBelegNr, dtBelegdatum, iKontoNr, iKontrollNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited VKBeleg count: {0}, read: {1}", iAwaitedCount, objResult.Count));
        }

        //[TestMethod]
        //public void GmDb_VkWare_Add_VkBeleg_And_VkWare_With_Restore()
        //{
        //    Add_VkBeleg_And_VkWare(true);
        //}

        //[TestMethod]
        //public void GmDb_VkWare_Add_VkBeleg_And_VkWare_Without_Restore()
        //{
        //    Add_VkBeleg_And_VkWare(false);
        //}

        //public void Add_VkBeleg_And_VkWare(bool bWithRestore)
        //{
        //    //create vkbeleg
        //    int iKontoNr = 30000;
        //    int iBelegnummer = 2000;
        //    short iPersonalNr = 1;
        //    decimal dBelegSummeNetto = 0;
        //    var objNewVkBelegToWrite = new VkBeleg(GmPath, GmUserData).CreateLieferschein(iKontoNr, iBelegnummer, DateTime.Now, 0, 0, 0, iPersonalNr, "");

        //    //create vkware
        //    var objEkWareToWrite = new EkWare(GmPath, GmUserData);
        //    var objNewVkWareToWrite = new VkWare(GmPath, GmUserData);

        //    short iPosNr = 1;
        //    int iArtikelNr = 0;
        //    int iKolli = 0;
        //    decimal dInhalt = 0;
        //    decimal dAmount = 0;
        //    decimal dPrice = 0;

        //    iArtikelNr = 501;
        //    iKolli = 17;
        //    dInhalt = 9;
        //    dAmount = iKolli * dInhalt;
        //    dPrice = 7.49m;
        //    dBelegSummeNetto = dAmount * dPrice;
        //    objNewVkWareToWrite.AdapedStock(ref objNewVkBelegToWrite, ref objEkWareToWrite, ref objNewVkWareToWrite, iPosNr, iArtikelNr, dPrice, dAmount);
        //    iPosNr += 2;
        //    Assert.IsTrue(iPosNr == 3, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: iPosNr {0} not found!", iPosNr));
        //    Assert.IsTrue(objEkWareToWrite.Count == 2, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.Count {0} not found!", objEkWareToWrite.Count));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(0).LagerbestandKolli == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandKolli {0} not found!", objEkWareToWrite.AtPosition(0).LagerbestandKolli));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(0).LagerbestandMenge == 2, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandMenge {0} not found!", objEkWareToWrite.AtPosition(0).LagerbestandMenge));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(1).LagerbestandKolli == 84, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandKolli {0} not found!", objEkWareToWrite.AtPosition(1).LagerbestandKolli));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(1).LagerbestandMenge == 756, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandMenge {0} not found!", objEkWareToWrite.AtPosition(1).LagerbestandMenge));
        //    Assert.IsTrue(objNewVkWareToWrite.Count == 2, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.Count {0} not found!", objNewVkWareToWrite.Count));
        //    Assert.IsTrue(objNewVkWareToWrite.AtPosition(0).ChargenNr == 5394, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.AtPosition(0).ChargenNr {0} not found!", objNewVkWareToWrite.AtPosition(0).ChargenNr));
        //    Assert.IsTrue(objNewVkWareToWrite.AtPosition(1).ChargenNr == 5395, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.AtPosition(0).ChargenNr {0} not found!", objNewVkWareToWrite.AtPosition(1).ChargenNr));

        //    iArtikelNr = 502;
        //    iKolli = 17;
        //    dInhalt = 6;
        //    dAmount = iKolli * dInhalt;
        //    dPrice = 7.79m;
        //    dBelegSummeNetto += dAmount * dPrice;
        //    objNewVkWareToWrite.AdapedStock(ref objNewVkBelegToWrite, ref objEkWareToWrite, ref objNewVkWareToWrite, iPosNr, iArtikelNr, dPrice, dAmount);
        //    iPosNr += 2;
        //    Assert.IsTrue(iPosNr == 5, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: iPosNr {0} not found!", iPosNr));
        //    Assert.IsTrue(objEkWareToWrite.Count == 4, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.Count {0} not found!", objEkWareToWrite.Count));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(2).LagerbestandKolli == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandKolli {0} not found!", objEkWareToWrite.AtPosition(2).LagerbestandKolli));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(2).LagerbestandMenge == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandMenge {0} not found!", objEkWareToWrite.AtPosition(2).LagerbestandMenge));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(3).LagerbestandKolli == 16, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandKolli {0} not found!", objEkWareToWrite.AtPosition(3).LagerbestandKolli));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(3).LagerbestandMenge == 96, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandMenge {0} not found!", objEkWareToWrite.AtPosition(3).LagerbestandMenge));
        //    Assert.IsTrue(objNewVkWareToWrite.Count == 4, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.Count {0} not found!", objNewVkWareToWrite.Count));
        //    Assert.IsTrue(objNewVkWareToWrite.AtPosition(2).ChargenNr == 5394, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.AtPosition(0).ChargenNr {0} not found!", objNewVkWareToWrite.AtPosition(2).ChargenNr));
        //    Assert.IsTrue(objNewVkWareToWrite.AtPosition(3).ChargenNr == 5395, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.AtPosition(0).ChargenNr {0} not found!", objNewVkWareToWrite.AtPosition(3).ChargenNr));

        //    iArtikelNr = 503;
        //    iKolli = 9;
        //    dInhalt = 5;
        //    dAmount = iKolli * dInhalt;
        //    dPrice = 6.99m;
        //    dBelegSummeNetto += dAmount * dPrice;
        //    objNewVkWareToWrite.AdapedStock(ref objNewVkBelegToWrite, ref objEkWareToWrite, ref objNewVkWareToWrite, iPosNr, iArtikelNr, dPrice, dAmount);
        //    iPosNr += 1;
        //    Assert.IsTrue(iPosNr == 6, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: iPosNr {0} not found!", iPosNr));
        //    Assert.IsTrue(objEkWareToWrite.Count == 5, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.Count {0} not found!", objEkWareToWrite.Count));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(4).LagerbestandKolli == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandKolli {0} not found!", objEkWareToWrite.AtPosition(4).LagerbestandKolli));
        //    Assert.IsTrue(objEkWareToWrite.AtPosition(4).LagerbestandMenge == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToWrite.AtPosition(0).LagerbestandMenge {0} not found!", objEkWareToWrite.AtPosition(4).LagerbestandMenge));
        //    Assert.IsTrue(objNewVkWareToWrite.Count == 5, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.Count {0} not found!", objNewVkWareToWrite.Count));
        //    Assert.IsTrue(objNewVkWareToWrite.AtPosition(4).ChargenNr == 5394, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareToWrite.AtPosition(0).ChargenNr {0} not found!", objNewVkWareToWrite.AtPosition(4).ChargenNr));

        //    //save vkbeleg
        //    objNewVkBelegToWrite.NettobetragLiefereschein = objNewVkWareToWrite.GetSumLieferscheinbetrag();
        //    objNewVkBelegToWrite.NettoGewinnBetrag = objNewVkWareToWrite.GetNettoProfit();
        //    objNewVkBelegToWrite = objNewVkBelegToWrite.Save(objNewVkBelegToWrite);

        //    int iFileId = 1;
        //    Assert.IsNotNull(objNewVkBelegToWrite, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: Konto {0} is null!", iKontoNr));
        //    Assert.IsTrue(objNewVkBelegToWrite.FileId == iFileId, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: FileId {0} not found!", iFileId));
        //    Assert.IsTrue(objNewVkBelegToWrite.KontoNr == iKontoNr, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KontoNr {0} not found!", iKontoNr));
        //    Assert.IsTrue(objNewVkBelegToWrite.Belegnummer == iBelegnummer, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: Belegnummer {0} not found!", iBelegnummer));
        //    Assert.IsTrue(objNewVkBelegToWrite.NettobetragLiefereschein == dBelegSummeNetto, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: NettobetragLiefereschein {0} not found!", dBelegSummeNetto));
        //    Assert.IsTrue(objNewVkBelegToWrite.NettoGewinnBetrag == 345.00m, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: NettoGewinnBetrag {0} not found!", 345.00m));

        //    //save vkware
        //    objNewVkWareToWrite.UpdateBelegeId(objNewVkBelegToWrite);
        //    var objNewVkWareSaved = objNewVkWareToWrite.Save();
        //    Assert.IsTrue(objNewVkWareSaved, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: objNewVkWareSaved could not be saved!"));

        //    var objSavedBeleg = new VkBeleg(objNewVkBelegToWrite.KontoNr, objNewVkBelegToWrite.Belegnummer, GmPath, GmUserData).Read().FirstOrDefault();
        //    var objSavedVkWare = new VkWare(objSavedBeleg.FileId + 1, GmPath, GmUserData).Read().ToList();

        //    Assert.IsTrue(objSavedVkWare[0].KolliLieferschein == 11, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 11));
        //    Assert.IsTrue(objSavedVkWare[0].MengeLieferschein == 99, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 99));
        //    Assert.IsTrue(objSavedVkWare[0].SummeNettoLieferschein == 741.51m, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: SummeNettoLieferschein {0} not found!", 741.51m));

        //    Assert.IsTrue(objSavedVkWare[1].KolliLieferschein == 6, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 6));
        //    Assert.IsTrue(objSavedVkWare[1].MengeLieferschein == 54, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 54));
        //    Assert.IsTrue(objSavedVkWare[1].SummeNettoLieferschein == 404.46m, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: SummeNettoLieferschein {0} not found!", 404.46m));

        //    Assert.IsTrue(objSavedVkWare[2].KolliLieferschein == 3, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 3));
        //    Assert.IsTrue(objSavedVkWare[2].MengeLieferschein == 18, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 18));
        //    Assert.IsTrue(objSavedVkWare[2].SummeNettoLieferschein == 140.22m, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: SummeNettoLieferschein {0} not found!", 140.22m));

        //    Assert.IsTrue(objSavedVkWare[3].KolliLieferschein == 14, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 14));
        //    Assert.IsTrue(objSavedVkWare[3].MengeLieferschein == 84, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 84));
        //    Assert.IsTrue(objSavedVkWare[3].SummeNettoLieferschein == 654.36m, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: SummeNettoLieferschein {0} not found!", 654.36m));

        //    Assert.IsTrue(objSavedVkWare[4].KolliLieferschein == 9, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 9));
        //    Assert.IsTrue(objSavedVkWare[4].MengeLieferschein == 45, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: KolliLieferschein {0} not found!", 34));
        //    Assert.IsTrue(objSavedVkWare[4].SummeNettoLieferschein == 314.55m, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: SummeNettoLieferschein {0} not found!", 314.55m));

        //    //save ekware
        //    var objEkWareToSaved = objEkWareToWrite.Save();
        //    Assert.IsTrue(objEkWareToSaved, "GmDb_VkWare_Add_VkBeleg_And_VkWare: objEkWareToSaved is FALSE!");

        //    var objEkWare501_6288 = new EkWare(GmDb.ALL, 501, 5394, 70505, 1, new DateTime(2012, 10, 31), GmPath, GmUserData).Read().FirstOrDefault();
        //    Assert.IsTrue(objEkWare501_6288.LagerbestandKolli == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandKolli {0} not found!", 0));
        //    Assert.IsTrue(objEkWare501_6288.LagerbestandMenge == 2, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandMenge {0} not found!", 2));
        //    Assert.IsTrue(objEkWare501_6288.LieferscheinVerkauftKolli == 44, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftKolli {0} not found!", 44));
        //    Assert.IsTrue(objEkWare501_6288.LieferscheinVerkauftMenge == 403, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftMenge {0} not found!", 403));

        //    var objEkWare501_32674 = new EkWare(GmDb.ALL, 501, 5395, 70505, 2, new DateTime(2012, 11, 7), GmPath, GmUserData).Read().FirstOrDefault();
        //    Assert.IsTrue(objEkWare501_32674.LagerbestandKolli == 84, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandKolli {0} not found!", 84));
        //    Assert.IsTrue(objEkWare501_32674.LagerbestandMenge == 756, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandMenge {0} not found!", 756));
        //    Assert.IsTrue(objEkWare501_32674.LieferscheinVerkauftKolli == 6, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftKolli {0} not found!", 6));
        //    Assert.IsTrue(objEkWare501_32674.LieferscheinVerkauftMenge == 54, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftMenge {0} not found!", 54));

        //    var objEkWare502_1795 = new EkWare(GmDb.ALL, 502, 5394, 70505, 3, new DateTime(2012, 10, 31), GmPath, GmUserData).Read().FirstOrDefault();
        //    Assert.IsTrue(objEkWare502_1795.LagerbestandKolli == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandKolli {0} not found!", 0));
        //    Assert.IsTrue(objEkWare502_1795.LagerbestandMenge == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandMenge {0} not found!", 0));
        //    Assert.IsTrue(objEkWare502_1795.LieferscheinVerkauftKolli == 29, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftKolli {0} not found!", 29));
        //    Assert.IsTrue(objEkWare502_1795.LieferscheinVerkauftMenge == 180, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftMenge {0} not found!", 180));

        //    var objEkWare502_32677 = new EkWare(GmDb.ALL, 502, 5395, 70505, 4, new DateTime(2012, 11, 7), GmPath, GmUserData).Read().FirstOrDefault();
        //    Assert.IsTrue(objEkWare502_32677.LagerbestandKolli == 16, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandKolli {0} not found!", 16));
        //    Assert.IsTrue(objEkWare502_32677.LagerbestandMenge == 96, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandMenge {0} not found!", 96));
        //    Assert.IsTrue(objEkWare502_32677.LieferscheinVerkauftKolli == 14, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftKolli {0} not found!", 14));
        //    Assert.IsTrue(objEkWare502_32677.LieferscheinVerkauftMenge == 84, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftMenge {0} not found!", 84));

        //    var objEkWare503_16515 = new EkWare(GmDb.ALL, 503, 5394, 70505, 7, new DateTime(2012, 10, 31), GmPath, GmUserData).Read().FirstOrDefault();
        //    Assert.IsTrue(objEkWare503_16515.LagerbestandKolli == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandKolli {0} not found!", 0));
        //    Assert.IsTrue(objEkWare503_16515.LagerbestandMenge == 0, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LagerbestandMenge {0} not found!", 0));
        //    Assert.IsTrue(objEkWare503_16515.LieferscheinVerkauftKolli == 22, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftKolli {0} not found!", 22));
        //    Assert.IsTrue(objEkWare503_16515.LieferscheinVerkauftMenge == 110, string.Format("GmDb_VkWare_Add_VkBeleg_And_VkWare: LieferscheinVerkauftMenge {0} not found!", 110));

        //    try
        //    {
        //        if (bWithRestore)
        //        {
        //            GmDb.Instance(GmPath, GmUserData).RestoreLatest(Files.VKBeleg);
        //            GmDb.Instance(GmPath, GmUserData).RestoreLatest(Files.VKWare);
        //            GmDb.Instance(GmPath, GmUserData).RestoreLatest(Files.EKWare);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        [TestMethod]
        public void GmDb_VkBeleg_Export_Index()
        {
            string strNewFilename = string.Format(@"D:\{0:yyyy-MM-dd_HH-mm-ss-fff}_{1}", DateTime.Now, "VKBELEG.XLS");
            GmDb.Instance(GmPath, GmUserData).ExportIndex(TableTypes.VKBELEG, Files.VKBeleg, strNewFilename, "\t");
        }
    }
}
