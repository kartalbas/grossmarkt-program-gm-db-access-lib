using System;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsFirma : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_Firma_Read_All()
        {
            dtStart = DateTime.Now;
            var cobjResults = new Firma(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_Firma_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 64;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

        [TestMethod]
        public void GmDb_Firma_Read_Save_Read()
        {
            dtStart = DateTime.Now;
            var cobjResults = new Firma(GmPath, GmUserData).Read().ToList();

            var objFirma = cobjResults[0];
            int iRechnungsnummer = objFirma.Rechnungsnummer;
            int iLieferscheinnummer = objFirma.Lieferscheinnummer;
            int iPosnummer = objFirma.Posnummer;
            int iZukaufpositionen = objFirma.Zukaufpositionen;

            objFirma.Rechnungsnummer++;
            objFirma.Lieferscheinnummer++;
            objFirma.Posnummer++;
            objFirma.Zukaufpositionen++;
            objFirma.Save(objFirma);

            cobjResults = new Firma(GmPath, GmUserData).Read().ToList();
            objFirma = cobjResults[0];
            Assert.IsTrue(objFirma.Rechnungsnummer == iRechnungsnummer + 1);
            Assert.IsTrue(objFirma.Lieferscheinnummer == iLieferscheinnummer + 1);
            Assert.IsTrue(objFirma.Posnummer == iPosnummer + 1);
            Assert.IsTrue(objFirma.Zukaufpositionen == iZukaufpositionen + 1);

            objFirma.Rechnungsnummer--;
            objFirma.Lieferscheinnummer--;
            objFirma.Posnummer--;
            objFirma.Zukaufpositionen--;
            objFirma.Save(objFirma);

            cobjResults = new Firma(GmPath, GmUserData).Read().ToList();
            objFirma = cobjResults[0];
            Assert.IsTrue(objFirma.Rechnungsnummer == iRechnungsnummer);
            Assert.IsTrue(objFirma.Lieferscheinnummer == iLieferscheinnummer);
            Assert.IsTrue(objFirma.Posnummer == iPosnummer);
            Assert.IsTrue(objFirma.Zukaufpositionen == iZukaufpositionen);

            dtStop = DateTime.Now;

            Log("GmDb_Firma_Read_Save_Read: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());
        }
    }
}
