namespace gmdb
{
    using System.Data;

    public interface IGmDb
    {
        DataTable Read(TableTypes enmTable, string strFile, string strSearchString, params int[] pobjValues);

        void Write(TableTypes enmTable, string strFile, DataTable objData);

        int RebuildIndex(TableTypes enmTable, string strFile);
    }
}
