using System;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsZuletzt : GmDbTestsBase
    {

        [TestMethod]
        public void GmDb_Zuletzt_Update_With_KontoNr_WarenNR()
        {
            const int iKontoNr = 10202;
            const int iWarenNr = 2665;
            const decimal dKolli = 9;
            const decimal dInhalt = 8.0M;
            const decimal dPreis = 1.99M;
            var objKaufdatum = new DateTime(2014, 9, 11);

            dtStart = DateTime.Now;
            var objZuletzt = new Zuletzt(iKontoNr, iWarenNr, objKaufdatum, dKolli, dInhalt, dPreis, GmPath, GmUserData);
            objZuletzt.Add(objZuletzt);
            objZuletzt.Save();

            var objResult = new Zuletzt(iKontoNr, iWarenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Zuletzt_Update_With_KontoNr_WarenNR: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iWarenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            Assert.IsTrue(objResult[0].Kolli == dKolli, string.Format("Awaited ZULETZT Kolli: {0}, read:{1}", dKolli, objResult[0].Kolli));
            Assert.IsTrue(objResult[0].Inhalt == dInhalt, string.Format("Awaited ZULETZT Inhalt: {0}, read:{1}", dInhalt, objResult[0].Inhalt));
            Assert.IsTrue(objResult[0].Preis == dPreis, string.Format("Awaited ZULETZT Preis: {0}, read:{1}", dPreis, objResult[0].Preis));
        }

        [TestMethod]
        public void GmDb_Zuletzt_Save_With_KontoNr_WarenNR()
        {
            const int iKontoNr = 10202;
            const int iWarenNr = 1009;
            const decimal dKolli = 9;
            const decimal dInhalt = 8.0M;
            const decimal dPreis = 1.99M;
            var objKaufdatum = new DateTime(2014, 9, 11);

            dtStart = DateTime.Now;
            var objZuletzt = new Zuletzt(iKontoNr, iWarenNr, objKaufdatum, dKolli, dInhalt, dPreis, GmPath, GmUserData);
            objZuletzt.Add(objZuletzt);
            objZuletzt.Save();

            var objResult = new Zuletzt(iKontoNr, iWarenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Zuletzt_Update_With_KontoNr_WarenNR: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iWarenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            Assert.IsTrue(objResult[0].Kolli == dKolli, string.Format("Awaited ZULETZT Kolli: {0}, read:{1}", dKolli, objResult[0].Kolli));
            Assert.IsTrue(objResult[0].Inhalt == dInhalt, string.Format("Awaited ZULETZT Inhalt: {0}, read:{1}", dInhalt, objResult[0].Inhalt));
            Assert.IsTrue(objResult[0].Preis == dPreis, string.Format("Awaited ZULETZT Preis: {0}, read:{1}", dPreis, objResult[0].Preis));
        }

        [TestMethod]
        public void GmDb_Zuletzt_AddAndSave_With_KontoNr_WarenNR()
        {
            GmDb.Instance(GmPath, GmUserData).ExportIndex(TableTypes.ZULETZT, gmdb.Files.Zuletzt, @"D:\zuletzt.ndx.csv", ";");

            const int iKontoNr = 10004;
            const int iWarenNr = 1009;
            const decimal dKolli = 9;
            const decimal dInhalt = 8.0M;
            const decimal dPreis = 1.99M;
            var objKaufdatum = new DateTime(2014, 9, 11);

            dtStart = DateTime.Now;
            var objZuletzt = new Zuletzt(iKontoNr, iWarenNr, objKaufdatum, dKolli, dInhalt, dPreis, GmPath, GmUserData);
            objZuletzt.Add(objZuletzt);
            objZuletzt.Save();

            var objResult = new Zuletzt(iKontoNr, iWarenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Zuletzt_Update_With_KontoNr_WarenNR: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iWarenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            Assert.IsTrue(objResult[0].Kolli == dKolli, string.Format("Awaited ZULETZT Kolli: {0}, read:{1}", dKolli, objResult[0].Kolli));
            Assert.IsTrue(objResult[0].Inhalt == dInhalt, string.Format("Awaited ZULETZT Inhalt: {0}, read:{1}", dInhalt, objResult[0].Inhalt));
            Assert.IsTrue(objResult[0].Preis == dPreis, string.Format("Awaited ZULETZT Preis: {0}, read:{1}", dPreis, objResult[0].Preis));
        }

        [TestMethod]
        public void GmDb_Zuletzt_Read_With_KontoNr_WarenNR()
        {
            //read only price for specific kontonr and warennr
            int iKontoNr = 10202;
            int iWarenNr = 1069;

            dtStart = DateTime.Now;
            var objResult = new Zuletzt(iKontoNr, iWarenNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Zuletzt_Read_With_KontoNr_WarenNR: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, iWarenNr, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            decimal dKolli = 1;
            decimal dInhalt = 1.41M;
            decimal dPreis =17.5M;
            Assert.IsTrue(objResult[0].Kolli == dKolli, string.Format("Awaited ZULETZT Kolli: {0}, read:{1}", dKolli, objResult[0].Kolli));
            Assert.IsTrue(objResult[0].Inhalt == dInhalt, string.Format("Awaited ZULETZT Inhalt: {0}, read:{1}", dInhalt, objResult[0].Inhalt));
            Assert.IsTrue(objResult[0].Preis == dPreis, string.Format("Awaited ZULETZT Preis: {0}, read:{1}", dPreis, objResult[0].Preis));
        }

        [TestMethod]
        public void GmDb_Zuletzt_Read_With_KontoNr()
        {
            //read all article prices for specific kontonr
            int iKontoNr = 20060;

            dtStart = DateTime.Now;
            var objResult = new Zuletzt(iKontoNr, GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Zuletzt_Read_With_KontoNr: for {0}/{1} times:{2}/{3}/{4}", iKontoNr, GmDb.ALL , dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 1717;
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited ZULETZT count:{0}, read:{1}", iAwaitedCount, objResult.Count));
        }

        [TestMethod]
        public void GmDb_Zuletzt_Read_SinceDate()
        {
            //read all lines
            dtStart = DateTime.Now;
            var objReader = new Zuletzt(GmPath, GmUserData) { Kaufdatum = new DateTime(2014, 01, 01) };
            var objResult = objReader.Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Zuletzt_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 35771;
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited ZULETZT count: {0}, read{1}", iAwaitedCount, objResult.Count));
        }

        [TestMethod]
        public void GmDb_Zuletzt_Read_All()
        {
            //read all lines
            dtStart = DateTime.Now;
            var objResult = new Zuletzt(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;
            Log("GmDb_Zuletzt_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 163793;
            Assert.IsTrue(objResult.Count == iAwaitedCount, string.Format("Awaited ZULETZT count: {0}, read{1}", iAwaitedCount, objResult.Count));
        }

    }
}
