namespace gmdb
{
    using System.IO;

    public class Files
    {
        // path
        public static string GMPath { get; set; }

        public static string GMUserdata { get; set; }

        public static string GMProg => Path.Combine(GMPath, "PROG");

        public static string GMArchive => Path.Combine(GMPath, GMUserdata, "ARCHIV");

        public static string GMBackup => Path.Combine(GMPath, GMUserdata, "BACKUP");

        public static string GMDaten => Path.Combine(GMPath, GMUserdata, "DATEN");

        public static string Dateien => Path.Combine(GMProg, "DATEIEN.BIN");

        public static string Program => Path.Combine(GMPath, GMUserdata, "PROGRAM.FRM");

        public static string Steuer => Path.Combine(GMPath, GMUserdata, "STEUER.TAB");

        // userdata
        public static string Country => Path.Combine(GMProg, "COUNTRY.DAT");

        public static string Firma => Path.Combine(GMDaten, "FIRMA.DAT");

        public static string VkTexte => Path.Combine(GMDaten, "VKTEXTE.DAT");

        public static string KontenGR2 => Path.Combine(GMDaten, "KONTENGR.2");

        public static string KontenGR3 => Path.Combine(GMDaten, "KONTENGR.3");

        public static string WarenUGR => Path.Combine(GMDaten, "WAREN.UGR");

        public static string WarenOGR => Path.Combine(GMDaten, "WAREN.OGR");

        public static string Tour => Path.Combine(GMDaten, "TOUR.DAT");

        public static string Konten => Path.Combine(GMDaten, "KONTEN.DAT");

        public static string Waren => Path.Combine(GMDaten, "WAREN.DAT");

        public static string EKBeleg => Path.Combine(GMDaten, "EKBELEG.DAT");

        public static string VKBeleg => Path.Combine(GMDaten, "VKBELEG.DAT");

        public static string EKWare => Path.Combine(GMDaten, "EKWARE.DAT");

        public static string VKWare => Path.Combine(GMDaten, "VKWARE.DAT");

        public static string VKTexte => Path.Combine(GMDaten, "VKTEXTE.DAT");

        public static string Zahlung => Path.Combine(GMDaten, "ZAHLUNG.DAT");

        public static string Zuletzt => Path.Combine(GMDaten, "ZULETZT.DAT");

        public static string Offen => Path.Combine(GMDaten, "OFFEN.DAT");

        public static string Position => Path.Combine(GMDaten, "POSITION.DAT");

        // archive
        public static string BVjj => Path.Combine(GMArchive, "BVjj.DAT");

        public static string Partiejj => Path.Combine(GMArchive, "PARTIEjj.DAT");

        public static string EinAusjj => Path.Combine(GMArchive, "EINAUSjj.ARC");

        public static string EKDatajj => Path.Combine(GMArchive, "EKDATAjj.DAT");

        public static string EKWarejj => Path.Combine(GMArchive, "EKWAREjj.DAT");

        public static string VKWAmmjj => Path.Combine(GMArchive, "VKWAmmjj.DAT");

        public static string VKBEmmjj => Path.Combine(GMArchive, "VKBEmmjj.DAT");

        public static string ZABmmjj => Path.Combine(GMArchive, "ZABmmjj.DAT");

        public static string ZADmmjj => Path.Combine(GMArchive, "ZADmmjj.DAT");

        public static string Zahlenjj => Path.Combine(GMArchive, "ZAHLENjj.ARC");
    }
}
