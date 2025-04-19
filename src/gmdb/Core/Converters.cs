namespace gmdb
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Converters
    {
        public static int DateToIntDDMMYY(DateTime objDate)
        {
            return objDate != default(DateTime)
                ? Convert.ToInt32(objDate.ToString("ddMMyy"))
                : GmDb.ALL;
        }

        public static int DateToIntYYYYMMDD(DateTime objDate)
        {
            int iDatum = GmDb.ALL;

            if (objDate == default(DateTime))
                return iDatum;

            if(objDate >= new DateTime(2000,1,1,0,0,0))
                iDatum = Convert.ToInt32(objDate.ToString("yyyyMMdd"));
            else
                iDatum = Convert.ToInt32(objDate.ToString("yyMMdd"));

            return iDatum;
        }

        public static string GetArchiveFile(string strTemplate, short sMonat, short sJahr)
        {
            string strFile = string.Empty;

            if (sMonat >= 0)
                strFile = strTemplate.ToLower().Replace("mm", sMonat.ToString("00"));
            if (sJahr >= 0)
                strFile = strFile.ToLower().Replace("jj", sJahr.ToString("00"));

            return strFile;
        }

        public static string[] GetArchiveFile(string strSearchPattern)
        {
            var objRegEx = new Regex(@"[A-Z]{4}[0-9]{4}.[A-Z]{3}");

            var cstrFiles = Directory.GetFiles(gmdb.Files.GMArchive, strSearchPattern)
                .Where(f => objRegEx.IsMatch(f)).ToArray();

            return cstrFiles;
        }

        public static string[] GetArchiveFiles(string strTemplate, DateTime dtBeforeDate, short sMonate)
        {
            var astrFiles = new string[sMonate];

            var iCurrentMonth = dtBeforeDate.Month;
            var iCurrentYear = dtBeforeDate.Year - 2000;

            for (short iMonat = 0; iMonat < sMonate; iMonat++)
            {
                astrFiles[iMonat] = GetArchiveFile(strTemplate, (short)iCurrentMonth, (short)iCurrentYear);
                iCurrentMonth--;
                if (iCurrentMonth == 0)
                {
                    iCurrentMonth = 12;
                    iCurrentYear--;
                }
            }

            return astrFiles;
        }

        public static DateTime IntToDate(int iDate)
        {
            try
            {
                if (iDate < 9999)
                {
                    return DateTime.ParseExact("121201", "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (iDate < 99999)
                {
                    return DateTime.ParseExact("0" + iDate.ToString(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (iDate > 100000)
                {
                    return DateTime.ParseExact(iDate.ToString(), "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                    return default(DateTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(DateTime);
            }
        }

        public static byte Asc(char src, int iCodePage)
        {
            try
            {
                string source = src.ToString(CultureInfo.InvariantCulture);
                return Encoding.GetEncoding(iCodePage).GetBytes(source)[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 32;
            }
        }

        public static char Chr(byte src, int iCodePage)
        {
            try
            {
                return Encoding.GetEncoding(iCodePage).GetChars(new byte[] { src })[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (char)32;
            }
        }
    }
}
