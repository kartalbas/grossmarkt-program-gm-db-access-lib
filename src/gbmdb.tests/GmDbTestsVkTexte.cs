using System;
using System.Linq;
using gmdb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gmdb.tests
{
    [TestClass]
    public class GmDbTestsVkTexte : GmDbTestsBase
    {
        [TestMethod]
        public void GmDb_VkTexte_Read_All()
        {

            var a = gmdb.GmDb.Instance(GmPath, GmUserData).ReadHeader(TableTypes.VKTEXTE, gmdb.Files.VkTexte);

            dtStart = DateTime.Now;
            var cobjResults = new VkTexte(GmPath, GmUserData).Read().ToList();
            dtStop = DateTime.Now;

            Log("GmDb_VkTexte_Read_All: for {0}/{1} times:{2}/{3}/{4}", GmDb.ALL, GmDb.ALL, dtStart.ToShortTimeString(), dtStop.ToShortTimeString(), dtStop.Subtract(dtStart).TotalSeconds.ToString());

            int iAwaitedCount = 64;
            Assert.IsTrue(cobjResults.Count == iAwaitedCount, string.Format("Awaited count:{0} but read:{1}", iAwaitedCount, cobjResults.Count));
        }

    }
}
