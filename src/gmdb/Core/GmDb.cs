namespace gmdb
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    using gmdb.Core;

    // TODO: delete
    // TODO: repair

    public class GmDb : IGmDb
    {
        #region private members

        private const string CODEPAGE = "437";
        private const int INDEX_BLOCKSIZE = 4096;

        private const int ZULETZT_BLOCK_HEADER = 4;
        private const int ZULETZT_KONTO_WIDTH = 4;
        private const int ZULETZT_BLOCKSIZE = 4088;
        private const int ZULETZT_BLOCK_OFFSET = (ZULETZT_BLOCKSIZE - ZULETZT_BLOCK_HEADER - ZULETZT_KONTO_WIDTH) / 5; // 816 bytes per block part

        private const FileShare FileShareOnRead = FileShare.ReadWrite;
        private const FileShare FileShareOnWrite = FileShare.ReadWrite;

        private static Dictionary<string, List<int[]>> TableInfos { get; set; }
        private static readonly object LOCK = new object();
        private const string SUFFIX_DAT = ".DAT";
        private const string SUFFIX_NDX = ".NDX";

        #endregion

        #region private / public members

        public const int ALL = -999;
        public const int TABLEDEF_WIDTH = 0;
        public const int TABLEDEF_TYPE = 1;
        public const int TABLEDEF_INDEX_ORDER = 4;
        public const int INDEX_COUNT = 0;
        public const int INDEX_E_LG = 1;
        public const int INDEX_L_LG = 2;
        public const int INDEX_Z_LG = 3;
        public const int INDEX_NUMBER = 5;

        public static TraceSource TraceSource { get; private set; }

        #endregion

        #region constructors

        public GmDb(string strGmPath, string strGmUserData)
        {
            try
            {
                InitTraceSource();

                Files.GMPath = strGmPath;
                Files.GMUserdata = strGmUserData;

                if (TableInfos == null)
                    TableInfos = ReadTableInfos();
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        #endregion

        #region public genereal methods

        public bool Accessable(string strFile, FileAccess enmFileAccess, FileShare enmFileShare)
        {
            lock (LOCK)
            {
                try
                {
                    TryFileAccessable(strFile, FileMode.OpenOrCreate, enmFileAccess, enmFileShare);
                    TryFileAccessable(strFile.Replace(SUFFIX_DAT, SUFFIX_NDX), FileMode.OpenOrCreate, enmFileAccess, enmFileShare);
                    return true;
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public DataTable Read(TableTypes enmTable, string strFile, string strSearchString, params int[] pobjValues)
        {
            lock (LOCK)
            {
                try
                {
                    return enmTable == TableTypes.ZULETZT
                        ? ReadZuletzt(enmTable, strFile, pobjValues)
                        : ReadTable(enmTable, strFile, strSearchString, pobjValues);
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public void Write(TableTypes enmTable, string strFile, DataTable objData)
        {
            lock (LOCK)
            {
                try
                {
                    if (enmTable == TableTypes.ZULETZT)
                        WriteZuletzt(enmTable, strFile, objData);
                    else
                        WriteTable(enmTable, strFile, objData);
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public DataTable ReadHeader(TableTypes enmTable, string strFile)
        {
            try
            {
                return ReadTableHeader(enmTable, strFile);
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        #endregion

        #region public read/write tables
        public DataTable ReadTable(TableTypes enmTable, string strFile, string strSearchString, params int[] pobjIndexValues)
        {
            lock (LOCK)
            {
                try
                {
                    string strTable = enmTable.ToString();
                    string strFilename = GetFilename(strFile);

                    List<int> cobjRowIndexes = null;
                    int iRowLength = GetRowLength(strTable);
                    int iTableRowsLength = TableInfos[strTable].Count;
                    DataTable objTargetTable = CreateTable(enmTable);

                    bool bWithIndex = CheckForIndexFields(enmTable);

                    // Get Index from original index file
                    if (bWithIndex)
                        cobjRowIndexes = SearchIndex(enmTable, strFile.Replace(SUFFIX_DAT, SUFFIX_NDX), pobjIndexValues);

                    using (var objFilestream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                    using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    {
                        // Create temporary index
                        if (cobjRowIndexes == null)
                        {
                            cobjRowIndexes = new List<int>();
                            long iLines = objReader.BaseStream.Length / iRowLength;
                            for (int iLine = 0; iLine < iLines; iLine++)
                                cobjRowIndexes.Add(iLine);
                        }

                        foreach (int iRowIndex in cobjRowIndexes)
                        {
                            objReader.BaseStream.Seek(iRowIndex * iRowLength, SeekOrigin.Begin);

                            // read header of file > value != 0 > skip (because line is deleted)
                            object objValue = ReadValue(objReader, TableInfos[strTable][0][TABLEDEF_TYPE], TableInfos[strTable][0][TABLEDEF_WIDTH]);

                            if (enmTable != TableTypes.FIRMA 
                                && enmTable != TableTypes.STEUER
                                && enmTable != TableTypes.PROGRAM
                                && enmTable != TableTypes.WARENOGR
                                && enmTable != TableTypes.WARENUGR
                                && enmTable != TableTypes.KONTENGR3)
                                if (Convert.ToInt32(objValue) != 0)
                                    continue;

                            var objDataRow = objTargetTable.NewRow();
                            if (TableInfos[strTable][0][TABLEDEF_TYPE] == (int)DataTypes.STRING)
                            {
                                objDataRow[0] = objValue.ToString().TrimEnd();                                    
                            }
                            else
                            {
                                objDataRow[0] = objValue;                                
                            }

                            bool bFound = false;

                            // read row
                            for (int iColumn = 1; iColumn < iTableRowsLength; iColumn++)
                            {
                                // load values
                                int iIndexWidth = TableInfos[strTable][iColumn][TABLEDEF_WIDTH];
                                int iType = TableInfos[strTable][iColumn][TABLEDEF_TYPE];
                                int iIndexOrder = TableInfos[strTable][iColumn][TABLEDEF_INDEX_ORDER];
                                objValue = ReadValue(objReader, iType, iIndexWidth);

                                // column type date?
                                if (iType == (int)DataTypes.DATE)
                                {
                                    objValue = Converters.IntToDate(Convert.ToInt32(objValue));
                                    objDataRow[iColumn] = objValue;
                                    objValue = Int32.Parse(Convert.ToDateTime(objValue).ToString("yyMMdd"));
                                }
                                else if (iType == (int) DataTypes.STRING)
                                {
                                    objDataRow[iColumn] = objValue.ToString().TrimEnd();                                    
                                }
                                else
                                {
                                    objDataRow[iColumn] = objValue;
                                }

                                // If search key exists, search column with this 
                                if (iType == (int)DataTypes.STRING && !string.IsNullOrEmpty(strSearchString))
                                {
                                    if (objValue.ToString().TrimEnd().ToLower().Contains(strSearchString.ToLower()))
                                        bFound = true;
                                }

                                // If the column is an index column, then compare in this section
                                if (iIndexOrder > 0)
                                {
                                    object objParameterValue = pobjIndexValues[iIndexOrder - 1];

                                    if (MatchValues(objParameterValue, objValue, iType) || (MatchValues(objParameterValue, ALL, iType) && string.IsNullOrEmpty(strSearchString)))
                                        bFound = true;
                                }
                                // Else this column is not an index column but one parameter exists, then compare in this section
                                else if (pobjIndexValues != null && (iIndexOrder == 0 && pobjIndexValues.Length == 1))
                                {
                                    object objParameterValue = pobjIndexValues[0];

                                    if (objValue.GetType() == objParameterValue.GetType())
                                    {
                                        if (MatchValues(objParameterValue, objValue, iType) || (MatchValues(objParameterValue, ALL, iType) && string.IsNullOrEmpty(strSearchString)))
                                            bFound = true;
                                    }
                                }
                                // Else no intent to search anything with index or search key, then add this row
                                else if (bWithIndex == false && string.IsNullOrEmpty(strSearchString) == true)
                                {
                                    bFound = true;
                                }
                            }

                            if (bFound) // insert always if searched without index
                            {
                                objDataRow[iTableRowsLength] = strFilename;
                                objDataRow[iTableRowsLength + 1] = iRowIndex;
                                objTargetTable.Rows.Add(objDataRow);
                            }
                        }
                    }

                    return objTargetTable;
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public void WriteTable(TableTypes enmTable, string strFile, DataTable objData)
        {
            lock (LOCK)
            {
                var objExceptionInfo = new List<string>();

                try
                {
                    // Backup(strFile);
                    CreateEmptyFile(strFile, false);
                    string strTable = enmTable.ToString();
                    bool bWithIndex = CheckForIndexFields(enmTable);

                    objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Executed WriteTable({enmTable}, {strFile}, {objData})");

                    DataTable objDataChanged = objData.GetChanges(DataRowState.Added | DataRowState.Modified);
                    if (objDataChanged == null)
                    {
                        throw new Exception($"No added/modified data found, data not saved in table {enmTable}");
                    }

                    using (var objFilestream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                    using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    {
                        objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Begin Loop over {nameof(objDataChanged)}");
                        for (int iRow = 0; iRow < objDataChanged.Rows.Count; iRow++)
                        {
                            objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Current Row: strTable: {strTable}");
                            objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Current Row: iRow: {iRow}");

                            // collect header info
                            int iRowLength = GetRowLength(strTable);
                            int iTableRowsLength = TableInfos[strTable].Count;
                            var objIndexHeader = GetIndexHeader(enmTable, strFile);

                            objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Current iRowLength: {iRowLength}");
                            objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Current iTableRowsLength: {iTableRowsLength}");

                            // collect indexes
                            List<int> cobDestIndex = GetIndexesOfDataRow(enmTable, iTableRowsLength, objDataChanged.Rows[iRow]);
                            var aobjDestIndex = CreateIndexArray(cobDestIndex);
                            List<int> cobjSrcIndex = null;
                            if (bWithIndex)
                                cobjSrcIndex = SearchIndex(enmTable, strFile, aobjDestIndex);

                            int iRowPos = 0;
                            int iTargetIndexLine = 0;

                            if (bWithIndex && cobjSrcIndex.Count == 0) // add new index
                            {
                                if (objIndexHeader.L_LG > 0)
                                {
                                    objIndexHeader.Z_LG--; // substract value for count(-1)                      
                                    iTargetIndexLine = objIndexHeader.E_LG - 1; // out: target index line
                                    cobjSrcIndex.AddRange(cobDestIndex.Select(i => -1));

                                    // get next free index line (-1)
                                    iRowPos = iTargetIndexLine * iRowLength;
                                    objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Add new index > objIndexHeader.L_LG > 0 ? Seek (Line 364) to iRowPos: {iRowPos}");
                                    objReader.BaseStream.Seek(iRowPos, SeekOrigin.Begin);
                                    var objValue = ReadValue(objReader, TableInfos[strTable][0][TABLEDEF_TYPE], TableInfos[strTable][0][TABLEDEF_WIDTH]);
                                    objIndexHeader.E_LG = Convert.ToInt32(objValue); // in: next free index line (-1)
                                    objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Add new index > objIndexHeader.L_LG > 0 ? Next free index at line: {objIndexHeader.E_LG}");
                                }
                                else
                                {
                                    iTargetIndexLine = objIndexHeader.Count; // out: target index line
                                    objIndexHeader.Count++; // increase count of lines
                                    cobjSrcIndex.AddRange(cobDestIndex.Select(i => -2));
                                    objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Add new index > objIndexHeader.L_LG <= 0 ? iTargetIndexLine: {iTargetIndexLine}");
                                }

                                cobjSrcIndex[0] = iTargetIndexLine;
                                cobDestIndex[0] = iTargetIndexLine;
                            }
                            else if (bWithIndex) // update index
                            {
                                iTargetIndexLine = cobjSrcIndex[0];
                                cobDestIndex[0] = iTargetIndexLine;
                                cobjSrcIndex = new List<int>();
                                cobjSrcIndex.AddRange(cobDestIndex.Select(i => i));
                                cobjSrcIndex[0] = iTargetIndexLine;
                            }

                            // update/add index file
                            if (bWithIndex)
                                if (UpdateIndex(enmTable, strFile, cobjSrcIndex, cobDestIndex) <= 0)
                                    throw new Exception($"An existing Index could not be updated for table {strTable}");

                            // write row to file
                            iRowPos = iTargetIndexLine * iRowLength;
                            objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: Writing now the row, SeekOrigin.Begin to iRowPos: {iRowPos}");
                            objWriter.BaseStream.Seek(iRowPos, SeekOrigin.Begin);
                            for (int iColumn = 0; iColumn < iTableRowsLength; iColumn++)
                            {
                                int iIndexWidth = TableInfos[strTable][iColumn][TABLEDEF_WIDTH];
                                int iType = TableInfos[strTable][iColumn][TABLEDEF_TYPE];
                                object objValue = objDataChanged.Rows[iRow][iColumn];

                                if (objValue is DateTime)
                                    objValue = Converters.DateToIntDDMMYY((DateTime) objValue);

                                // fit string to its width
                                if (iType == 1)
                                    objValue = EnsureStringLength(enmTable, iColumn, objValue.ToString());

                                WriteValue(objWriter, objValue, iType, iIndexWidth);
                            }

                            // update index header
                            if (bWithIndex)
                                SetIndexHeader(enmTable, strFile, objIndexHeader);
                        }

                        objExceptionInfo.Add($"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: End Loop over {nameof(objDataChanged)}");

                    }
                }
                catch (Exception objException)
                {
                    Log(objException);

                    foreach (var objItem in objExceptionInfo)
                        TraceSource.TraceEvent(TraceEventType.Error, 1001, objItem);

                    throw;
                }
            }
        }

        #endregion

        #region public read/write zuletzt table

        public DataTable ReadZuletzt(TableTypes enmTable, string strFile, int[] pobjIndexValues)
        {
            lock (LOCK)
            {
                try
                {
                    int iFindKontoNr = pobjIndexValues.Length > 0 ? pobjIndexValues[0] : ALL;
                    int iFindWarenNr = pobjIndexValues.Length > 1 ? pobjIndexValues[1] : ALL;
                    int iFindSince = pobjIndexValues.Length > 2 ? pobjIndexValues[2] : ALL;

                    var objSinceDate = Converters.IntToDate(iFindSince);

                    DataTable objTargetTable = CreateTable(enmTable);

                    List<int> cobjIndexes = SearchIndex(enmTable, strFile.Replace(SUFFIX_DAT, SUFFIX_NDX), iFindKontoNr);

                    if (cobjIndexes.Count == 0)
                        return objTargetTable;
                                      
                    const int iWidthWare = 4;
                    var abyteBlockBuffer = new byte[INDEX_BLOCKSIZE];

                    using (var objFilestream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                    using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    {
                        foreach (int iIndex in cobjIndexes)
                        {
                            const int iBlockBeginOffset = ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH;
                            objReader.BaseStream.Seek(iIndex * ZULETZT_BLOCKSIZE, SeekOrigin.Begin);
                            objReader.Read(abyteBlockBuffer, 0, ZULETZT_BLOCKSIZE);

                            int iKontoNr = Convert.ToInt32(ReadValue(abyteBlockBuffer, (int)DataTypes.INT323, ZULETZT_KONTO_WIDTH, ZULETZT_BLOCK_HEADER));

                            if (iKontoNr == iFindKontoNr || iFindKontoNr == ALL)
                            {
                                for (int iOffsetIndex = 0; iOffsetIndex < ZULETZT_BLOCK_OFFSET; iOffsetIndex = iOffsetIndex + 4)
                                {
                                    int iNextReaderPos = iBlockBeginOffset + ((0 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                                    int iWarenNr = Convert.ToInt32(ReadValue(abyteBlockBuffer, (int) DataTypes.INT323, iWidthWare, iNextReaderPos));

                                    DataRow objDataRow = null;

                                    if (iWarenNr != 0)
                                        objDataRow = objTargetTable.NewRow();
                                    else
                                        continue;

                                    if (iWarenNr == iFindWarenNr)
                                    {
                                        objDataRow = ConvertZuletztToDataRow(objTargetTable, abyteBlockBuffer, objDataRow, iKontoNr, iWarenNr, iBlockBeginOffset, iOffsetIndex);
                                        objTargetTable.Rows.Add(objDataRow);
                                        break;
                                    }
                                    else if (iFindSince != ALL)
                                    {
                                        objDataRow = ConvertZuletztToDataRow(objTargetTable, abyteBlockBuffer, objDataRow, iKontoNr, iWarenNr, iBlockBeginOffset, iOffsetIndex);

                                        var objDate = (DateTime) objDataRow[2];
                                        if (objDate >= objSinceDate)
                                            objTargetTable.Rows.Add(objDataRow);
                                    }
                                    else if (iFindWarenNr == ALL)
                                    {
                                        objDataRow = ConvertZuletztToDataRow(objTargetTable, abyteBlockBuffer, objDataRow, iKontoNr, iWarenNr, iBlockBeginOffset, iOffsetIndex);
                                        objTargetTable.Rows.Add(objDataRow);
                                    }
                                }
                            }
                        }
                    }

                    return objTargetTable;
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public void WriteZuletzt(TableTypes enmTable, string strFile, DataTable objData)
        {
            lock (LOCK)
            {
                try
                {
                    int iTargetKontoNr = Convert.ToInt32(objData.Rows[0].ItemArray[0]);
                    int iTargetWarenNr = Convert.ToInt32(objData.Rows[0].ItemArray[1]);
                    int iTargetDate = Converters.DateToIntDDMMYY((DateTime)objData.Rows[0].ItemArray[2]);
                    decimal dTargetKolli = Convert.ToDecimal(objData.Rows[0].ItemArray[3]);
                    decimal dTargetMenge = Convert.ToDecimal(objData.Rows[0].ItemArray[4]);
                    decimal dTargetPreis = Convert.ToDecimal(objData.Rows[0].ItemArray[5]);

                    List<int> cobjIndexes = SearchIndex(enmTable, strFile.Replace(SUFFIX_DAT, SUFFIX_NDX), iTargetKontoNr);

                    if (cobjIndexes.Count == 0)
                    {
                        // ADD INDEX & BLOCK
                        int iNextBlockOffset = AddZuletztBlock(iTargetKontoNr);
                        AddOrUpdateZuletzt(iNextBlockOffset * ZULETZT_BLOCKSIZE, 0, iTargetKontoNr, iTargetWarenNr, iTargetDate, dTargetKolli, dTargetMenge, dTargetPreis);
                        return;
                    }
                    else
                    {
                        // UPDATE in BLOCK
                        bool bFound = false;
                        const int iBlockBeginOffset = ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH;

                        foreach (int iIndex in cobjIndexes)
                        {
                            var abyteBlockBuffer = ReadZuletztBlock(iIndex);
                            var objKontoNr = ReadValue(abyteBlockBuffer, (int)DataTypes.INT323, ZULETZT_KONTO_WIDTH, ZULETZT_BLOCK_HEADER);

                            if (Convert.ToInt32(objKontoNr) == iTargetKontoNr)
                            {
                                int iOffsetIndex;

                                for (iOffsetIndex = 0; iOffsetIndex < ZULETZT_BLOCK_OFFSET; iOffsetIndex = iOffsetIndex + ZULETZT_KONTO_WIDTH)
                                {
                                    int iNextReaderPos = iBlockBeginOffset + ((0 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                                    var objWarenNr = ReadValue(abyteBlockBuffer, (int)DataTypes.INT323, 4, iNextReaderPos);

                                    if (Convert.ToInt32(objWarenNr) == iTargetWarenNr)
                                    {
                                        AddOrUpdateZuletzt(iIndex * ZULETZT_BLOCKSIZE, iOffsetIndex, 0, 0, iTargetDate, dTargetKolli, dTargetMenge, dTargetPreis);
                                        bFound = true;
                                        break;                                        
                                    }
                                }

                                if (bFound) break;
                            }
                        }

                        if (!bFound)
                        {
                            // ADD in BLOCK
                            foreach (int iIndex in cobjIndexes)
                            {
                                var abyteBlockBuffer = ReadZuletztBlock(iIndex);
                                var objKontoNr = ReadValue(abyteBlockBuffer, (int)DataTypes.INT323, ZULETZT_KONTO_WIDTH, ZULETZT_BLOCK_HEADER);

                                if (Convert.ToInt32(objKontoNr) == iTargetKontoNr)
                                {
                                    for (int iOffsetIndex = 0; iOffsetIndex < ZULETZT_BLOCK_OFFSET; iOffsetIndex = iOffsetIndex + ZULETZT_KONTO_WIDTH)
                                    {
                                        int iNextReaderPos = iBlockBeginOffset + ((0 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                                        var objWarenNr = ReadValue(abyteBlockBuffer, (int)DataTypes.INT323, 4, iNextReaderPos);

                                        if (Convert.ToInt32(objWarenNr) == 0)
                                        {
                                            AddOrUpdateZuletzt(iIndex * ZULETZT_BLOCKSIZE, iOffsetIndex, 0, iTargetWarenNr, iTargetDate, dTargetKolli, dTargetMenge, dTargetPreis);
                                            bFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (!bFound)
                        {
                            // ADD INDEX & BLOCK
                            int iNextBlockOffset = AddZuletztBlock(iTargetKontoNr);
                            AddOrUpdateZuletzt(iNextBlockOffset * ZULETZT_BLOCKSIZE, 0, iTargetKontoNr, iTargetWarenNr, iTargetDate, dTargetKolli, dTargetMenge, dTargetPreis);
                        }
                    }

                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        #endregion

        #region public index operations

        public List<int> SearchIndex(TableTypes enmTable, string strFile, params int[] pobjIndexValues)
        {
            try
            {
                string strIndexfilename = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                CreateEmptyFile(strIndexfilename, false);

                var cobjTableIndexes = GetTableIndexes(enmTable);
                int iIndexLength = GetIndexLength(cobjTableIndexes);
                int iRealBlockSize = INDEX_BLOCKSIZE / iIndexLength;
                var abyteBlockBuffer = new byte[INDEX_BLOCKSIZE];
                var cobjIndexes = new List<int>();

                if (pobjIndexValues == null)
                    return cobjIndexes;

                using (var objFilestream = new FileStream(strIndexfilename, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    var iLength = (int) objReader.BaseStream.Length;
                    int iBlockPosition = 0;

                    while (iBlockPosition < iLength)
                    {
                        // seek to block position and read block
                        objReader.BaseStream.Seek(iBlockPosition, SeekOrigin.Begin);
                        objReader.Read(abyteBlockBuffer, 0, INDEX_BLOCKSIZE);

                        for (int iRealBlockOffset = 0; iRealBlockOffset < iRealBlockSize; iRealBlockOffset++)
                        {
                            int iSearchIndex = 0;
                            int iIndexWidthSum = 0;

                            for (int iIndexColumn = 0; iIndexColumn < pobjIndexValues.Length; iIndexColumn++)
                            {
                                int iIndexWidth = cobjTableIndexes[iIndexColumn][TABLEDEF_WIDTH];
                                int iType = cobjTableIndexes[iIndexColumn][TABLEDEF_TYPE];
                                int iByteStartPosition = (iRealBlockSize * iIndexWidthSum) + (iRealBlockOffset * iIndexWidth);

                                object objValue = ReadValue(abyteBlockBuffer, iType, iIndexWidth, iByteStartPosition);
                                int iValue = Convert.ToInt32(objValue);

                                if (iValue != -1 && iValue != -2)
                                {
                                    if (MatchValues(pobjIndexValues[iIndexColumn], objValue, iType) ||
                                        pobjIndexValues[iIndexColumn] == ALL)
                                    {
                                        if (iSearchIndex >= pobjIndexValues.Length - 1)
                                        {
                                            int iCurrentBlock = iBlockPosition / INDEX_BLOCKSIZE;
                                            int iIndexLine = (iCurrentBlock * iRealBlockSize) + iRealBlockOffset;
                                            cobjIndexes.Add(iIndexLine);
                                            break;
                                        }
                                        iSearchIndex++;
                                    }
                                }

                                iIndexWidthSum += iIndexWidth;
                            }
                        }
                        iBlockPosition += INDEX_BLOCKSIZE;
                    }
                }

                return cobjIndexes;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public List<int> FindFreeIndex(TableTypes enmTable, string strFile)
        {
            try
            {
                // calc blockoffset
                var cobjTableIndexes = GetTableIndexes(enmTable);
                int iIndexLength = GetIndexLength(cobjTableIndexes);
                int iIndexArrayLength = GetDummyIndexArray(cobjTableIndexes).Length;
                int iRealBlockSize = INDEX_BLOCKSIZE / iIndexLength;
                int iRestBlockSize = INDEX_BLOCKSIZE % iIndexLength;
                var abyteBlockBuffer = new byte[INDEX_BLOCKSIZE];
                List<int> cobjTargetIndexArry = null;

                // find free place to insert a new indexes
                string strIndexfilename = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                CreateEmptyFile(strIndexfilename, false);

                using (var objFilestream = new FileStream(strIndexfilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    // Add index block if file length = 0
                    var iIndexFileLength = (int)objReader.BaseStream.Length;
                    if (iIndexFileLength <= 0)
                        iIndexFileLength = AddIndexBlock(enmTable, objWriter, 0, iRealBlockSize);

                    var iBlocksInIndexFile = (int)Math.Round((decimal)iIndexFileLength / INDEX_BLOCKSIZE);

                    for (int iCurrentBlock = 0; iCurrentBlock < iBlocksInIndexFile; iCurrentBlock++)
                    {
                        int iBlockPosition = iCurrentBlock * INDEX_BLOCKSIZE;
                        objReader.BaseStream.Seek(iBlockPosition, SeekOrigin.Begin);
                        objReader.Read(abyteBlockBuffer, 0, INDEX_BLOCKSIZE);

                        int iSkipHeader = iCurrentBlock == 0 ? 1 : 0;

                        for (int iRealBlockOffset = iSkipHeader; iRealBlockOffset < iRealBlockSize; iRealBlockOffset++)
                        {
                            int iIndexWidthSum = 0;
                            int iIndexValueAtFirstColumn = 0;

                            for (int iIndexColumn = 0; iIndexColumn < iIndexArrayLength; iIndexColumn++)
                            {
                                int iIndexWidth = cobjTableIndexes[iIndexColumn][TABLEDEF_WIDTH];
                                int iType = cobjTableIndexes[iIndexColumn][TABLEDEF_TYPE];
                                int iByteStartPosition = (iRealBlockSize * iIndexWidthSum) + (iRealBlockOffset * iIndexWidth);
                                object objReadValue = ReadValue(abyteBlockBuffer, iType, iIndexWidth, iByteStartPosition);
                                int iCurrentIndexPositionValue = Convert.ToInt32(objReadValue);

                                if (iIndexColumn == 0)
                                    iIndexValueAtFirstColumn = Convert.ToInt32(iCurrentIndexPositionValue);

                                // found free space in index block? (free space: -1 || -2)
                                if (iCurrentIndexPositionValue < 0 && iIndexValueAtFirstColumn < 0)
                                {
                                    if (iIndexColumn >= iIndexArrayLength - 1)
                                    {
                                        // calculate index line and create new target index array
                                        int iLine = (iCurrentBlock * iRealBlockSize) + iRealBlockOffset;
                                        cobjTargetIndexArry = CreateIndexValues(enmTable, iLine, iCurrentIndexPositionValue);

                                        // Add index block if rest size is reached + if currentblock > 0 then substract width of the header
                                        int iIndexPosition = iBlockPosition + iByteStartPosition;
                                        int iRestOfBlock = iIndexFileLength - (iIndexPosition + iIndexWidth);
                                        if (iCurrentBlock > 0)
                                            iRestOfBlock -= iIndexWidth;

                                        if (iRestOfBlock <= iRestBlockSize)
                                            AddIndexBlock(enmTable, objWriter, iBlockPosition + INDEX_BLOCKSIZE, iRealBlockSize);

                                        break;
                                    }
                                }

                                iIndexWidthSum += iIndexWidth;
                            }
                            if (cobjTargetIndexArry != null)
                                break;
                        }
                        if (cobjTargetIndexArry != null)
                            break;
                    }
                    return cobjTargetIndexArry;
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public int UpdateIndex(TableTypes enmTable, string strFile, List<int> cobjSrcIndexArrays, List<int> cobjDestIndexArrays)
        {
            try
            {
                string strIndexfilename = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                CreateEmptyFile(strIndexfilename, false);

                var cobjTableIndexes = GetTableIndexes(enmTable);
                int iTableIndexesLength = cobjTableIndexes.Sum(i => i[TABLEDEF_WIDTH]);
                int iIndexLength = GetIndexLength(cobjTableIndexes);
                int iRealBlockSize = INDEX_BLOCKSIZE / iIndexLength;
                int iRestBlockSize = INDEX_BLOCKSIZE % iIndexLength;

                // find & update or insert a new index row
                using (var objFilestream = new FileStream(strIndexfilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    int iLine = cobjDestIndexArrays[0];
                    int iBlockBeginCount = iLine / iRealBlockSize;
                    int iLineInsideBlock = iLine - (iBlockBeginCount * iRealBlockSize);
                    int iBlockPosition = iBlockBeginCount * INDEX_BLOCKSIZE;

                    var iIndexFileLength = (int)objWriter.BaseStream.Length;

                    if (iIndexFileLength <= 0)
                        iIndexFileLength = AddIndexBlock(enmTable, objWriter, 0, iRealBlockSize);

                    int iRestOfBlock = iIndexFileLength - (iBlockPosition + (iLineInsideBlock * iTableIndexesLength));

                    if (iRestOfBlock <= iRestBlockSize)
                        AddIndexBlock(enmTable, objWriter, iBlockPosition, iRealBlockSize);

                    int iIndexWidthSum = 0;

                    if (iLine > 0)
                    {
                        for (int iIndexColumn = 0; iIndexColumn < cobjDestIndexArrays.Count - 1; iIndexColumn++)
                            // cobjDestIndexArrays.Count-1 =  line value + IndexArray.Count
                        {
                            int iIndexWidth = cobjTableIndexes[iIndexColumn][TABLEDEF_WIDTH];
                            int iType = cobjTableIndexes[iIndexColumn][TABLEDEF_TYPE];
                            int iIndexSeekPosition = iBlockPosition
                                + (iRealBlockSize * iIndexWidthSum)
                                + (iLineInsideBlock * iIndexWidth);

                            objReader.BaseStream.Seek(iIndexSeekPosition, SeekOrigin.Begin);
                            object objReadValue = ReadValue(objReader, iType, iIndexWidth);
                            object objSrcValue = cobjSrcIndexArrays[iIndexColumn + 1]; // skip line value
                            object objDestValue = cobjDestIndexArrays[iIndexColumn + 1]; // skip line value

                            if (MatchValues(objSrcValue, objReadValue, iType))
                            {
                                objWriter.Seek(iIndexSeekPosition, SeekOrigin.Begin);
                                WriteValue(objWriter, objDestValue, iType, iIndexWidth);
                            }

                            iIndexWidthSum += iIndexWidth;
                        }

                        return iLine;
                    }
                    else
                    {
                        throw new Exception($"Parameter for updating an Index line is zero for table {enmTable}");
                    }
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public List<int> ReadIndex(TableTypes enmTable, string strFile, int iLine)
        {
            try
            {
                string strIndexfilename = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                CreateEmptyFile(strIndexfilename, false);

                var cobjTableIndexes = GetTableIndexes(enmTable);
                int iIndexLength = GetIndexLength(cobjTableIndexes);
                int iRealBlockSize = INDEX_BLOCKSIZE / iIndexLength;

                // find & update or insert a new index row
                using (var objFilestream = new FileStream(strIndexfilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    if (iLine > 0)
                    {
                        var cobjIndexValues = new List<int> {iLine};
                        int iIndexWidthSum = 0;
                        int iBlockBeginCount = iLine / iRealBlockSize;
                        int iLineInsideBlock = iLine - (iBlockBeginCount * iRealBlockSize);
                        int iBlockPosition = iBlockBeginCount * INDEX_BLOCKSIZE;
                        int iIndexValuesCount = GetTableIndexes(enmTable).Count;

                        for (int iIndexColumn = 0; iIndexColumn < iIndexValuesCount; iIndexColumn++)
                        {
                            int iIndexWidth = cobjTableIndexes[iIndexColumn][TABLEDEF_WIDTH];
                            int iType = cobjTableIndexes[iIndexColumn][TABLEDEF_TYPE];
                            int iIndexSeekPosition = iBlockPosition
                                + (iRealBlockSize * iIndexWidthSum)
                                + (iLineInsideBlock * iIndexWidth);
                            objReader.BaseStream.Seek(iIndexSeekPosition, SeekOrigin.Begin);
                            object objReadValue = ReadValue(objReader, iType, iIndexWidth);
                            cobjIndexValues.Add(Convert.ToInt32(objReadValue));
                            iIndexWidthSum += iIndexWidth;
                        }

                        return cobjIndexValues;
                    }
                    else
                    {
                        throw new Exception($"ReadIndex: Parameter for updating an Index line is zero for table {enmTable}");
                    }
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        #endregion

        #region public import/export tables, rebuild index operations

        public List<int[]> ExportIndex(TableTypes enmTable, string strSourceFile, string strDestinationFile, string strDelimitier)
        {
            lock (LOCK)
            {
                try
                {
                    var cobjTableIndexes = GetTableIndexes(enmTable);
                    int iIndexLength = GetIndexLength(cobjTableIndexes);
                    object[] pobjParams = GetDummyIndexArray(cobjTableIndexes);

                    // calc blockoffset
                    int iBlockOffsetSize = INDEX_BLOCKSIZE / iIndexLength;

                    // init variables
                    var sIndexfilename = strSourceFile.Replace(SUFFIX_DAT, SUFFIX_NDX);

                    var cobIndexArrays = new List<int[]>();

                    // search for indexes
                    using (var objFilestream = new FileStream(sIndexfilename, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                    using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    {
                        // read full index file in buffer
                        var iLength = (int) objReader.BaseStream.Length;
                        var bBuffer = new byte[iLength];
                        int iBuffer = objReader.Read(bBuffer, 0, iLength);

                        for (int i = 0; i <= pobjParams.Length; i++)
                            cobIndexArrays.Add(new int[iLength / iIndexLength]);

                        if (iBuffer != iLength)
                            throw new Exception($"File {strSourceFile} not read completely");

                        int iBlockPosition = 0;

                        // search for index
                        while (iBlockPosition < iLength)
                        {
                            // search in block
                            for (int iBlockOffsetPos = 0; iBlockOffsetPos < iBlockOffsetSize; iBlockOffsetPos++)
                            {
                                int iIndexWidthSum = 0;

                                // line position
                                int iBlockLine = iBlockPosition / INDEX_BLOCKSIZE * iBlockOffsetSize;
                                cobIndexArrays[0][iBlockLine + iBlockOffsetPos] = iBlockLine + iBlockOffsetPos;

                                for (int iIndexColumn = 0; iIndexColumn < pobjParams.Length; iIndexColumn++)
                                {
                                    int iIndexWidth = cobjTableIndexes[iIndexColumn][TABLEDEF_WIDTH];
                                    int iType = cobjTableIndexes[iIndexColumn][TABLEDEF_TYPE];
                                    int iIndexOrder = cobjTableIndexes[iIndexColumn][TABLEDEF_INDEX_ORDER];
                                    int iIndexPosition = iBlockPosition
                                        + (iBlockOffsetSize * iIndexWidthSum)
                                        + (iBlockOffsetPos * iIndexWidth);

                                    // index value
                                    object objValue = ReadValue(bBuffer, iType, iIndexWidth, iIndexPosition);
                                    cobIndexArrays[iIndexOrder][iBlockLine + iBlockOffsetPos] = Convert.ToInt32(objValue);

                                    iIndexWidthSum += iIndexWidth;
                                }
                            }

                            iBlockPosition += INDEX_BLOCKSIZE;
                        }
                    }

                    WriteArrayToFile(cobIndexArrays, strDelimitier, strDestinationFile);

                    return cobIndexArrays;
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public int RebuildIndex(TableTypes enmTable, string strFile)
        {
            // TODO: insert -1 on columns which line is deleted (c0 > 0)
            lock (LOCK)
            {
                try
                {
                    string strTable = enmTable.ToString();
                    string strIndexfilename = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                    CreateEmptyFile(strIndexfilename, true);

                    var cobjTableIndexes = GetTableIndexes(enmTable);
                    object[] aobjParams = GetDummyIndexArray(cobjTableIndexes);

                    // calc blockoffset
                    int iTotalIndex = -1;

                    // search for indexes
                    using (var objFilestream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                    using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    {
                        int iRowLength = GetRowLength(strTable);
                        int iTableRowsLength = TableInfos[strTable].Count;

                        long lFileLength = objReader.BaseStream.Length;
                        var iLines = (Int32)(lFileLength / iRowLength);
                        iTotalIndex = iLines - 1;

                        var cobIndexArrays = new List<int>();
                        for (int i = 0; i <= aobjParams.Length; i++)
                            cobIndexArrays.Add(-1);

                        for (int iLine = 0; iLine < iLines - 1; iLine++)
                        {
                            objReader.BaseStream.Seek(iRowLength, SeekOrigin.Current);
                            for (int iColumn = 0; iColumn <= iTableRowsLength - 1; iColumn++)
                            {
                                int iIndexWidth = cobjTableIndexes[iColumn][TABLEDEF_WIDTH];
                                int iType = cobjTableIndexes[iColumn][TABLEDEF_TYPE];
                                int iIndexOrder = cobjTableIndexes[iColumn][TABLEDEF_INDEX_ORDER];

                                if (iIndexOrder != 0)
                                {
                                    object objValue = ReadValue(objReader, iType, iIndexWidth);
                                    cobIndexArrays[0] = iLine + 1;
                                    cobIndexArrays[iIndexOrder] = Convert.ToInt32(objValue);
                                }
                                else
                                {
                                    objReader.BaseStream.Position += iIndexWidth;
                                }
                            }

                            int iIndexLine = UpdateIndex(enmTable, strFile, null, cobIndexArrays);
                            if (iIndexLine <= 0)
                                return iIndexLine;

                            objReader.BaseStream.Position -= iRowLength;
                        }
                    }

                    return iTotalIndex;
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        public List<string[]> ExportTable(TableTypes enmTable, string strSourceFile, string strDestinationFile, string strDelimitier)
        {
            lock (LOCK)
            {
                try
                {
                    string strTable = enmTable.ToString();
                    int iRowLength = GetRowLength(strTable);
                    int iTableRowsLength = TableInfos[strTable].Count;
                    var cstrLines = new List<string[]>();

                    using (var objFilestream = new FileStream(strSourceFile, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                    using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                    {
                        long iLines = objReader.BaseStream.Length / iRowLength;

                        for (long iLine = 0; iLine < iLines; iLine++)
                        {
                            objReader.BaseStream.Seek(iLine * iRowLength, SeekOrigin.Begin);
                            var astrLine = new string[iTableRowsLength + 1];
                            astrLine[0] = iLine.ToString();

                            // read row
                            for (int iColumn = 0; iColumn < iTableRowsLength; iColumn++)
                            {
                                // load values
                                int iIndexWidth = TableInfos[strTable][iColumn][TABLEDEF_WIDTH];
                                int iType = TableInfos[strTable][iColumn][TABLEDEF_TYPE];
                                object objValue = ReadValue(objReader, iType, iIndexWidth);

                                // column type date?
                                if (iType == (int) DataTypes.DATE)
                                {
                                    astrLine[iColumn + 1] = Converters.IntToDate(Convert.ToInt32(objValue)).ToString("dd-MM-yy");
                                }
                                else
                                {
                                    // shift 32bit to right only for two index header values
                                    if (iLine == 0 && (iColumn == INDEX_L_LG || iColumn == INDEX_Z_LG))
                                        objValue = Convert.ToInt32(objValue) >> 16;

                                    astrLine[iColumn + 1] = objValue.ToString();
                                }
                            }
                            cstrLines.Add(astrLine);
                        }
                    }

                    WriteArrayToFile(cstrLines, strDelimitier, strDestinationFile);

                    return cstrLines;
                }
                catch (Exception objException)
                {
                    Log(objException);
                    throw;
                }
            }
        }

        #endregion

        #region Private methods

        private int AddZuletztBlock(int iKontoNr)
        {
            try
            {
                // add new index
                if (iKontoNr <= 0)
                    throw new Exception("KontoNr must be > 0");

                // create new block
                var abyteNewBlock = new byte[ZULETZT_BLOCKSIZE];
                var abyteKontoNr = BitConverter.GetBytes(iKontoNr);
                abyteNewBlock[ZULETZT_BLOCK_HEADER + 0] = abyteKontoNr[0];
                abyteNewBlock[ZULETZT_BLOCK_HEADER + 1] = abyteKontoNr[1];
                abyteNewBlock[ZULETZT_BLOCK_HEADER + 2] = abyteKontoNr[2];
                abyteNewBlock[ZULETZT_BLOCK_HEADER + 3] = abyteKontoNr[3];

                // read adress for next block
                var abyteBlockBuffer = ReadZuletztBlock(0);
                var objNextBlockPosition = ReadValue(abyteBlockBuffer, (int) DataTypes.INT323, ZULETZT_KONTO_WIDTH, 0);
                int iNextBlockPosition = Convert.ToInt32(objNextBlockPosition);

                // add new block & update Block Header to new block adress
                WriteZuletztBlock(iNextBlockPosition * ZULETZT_BLOCKSIZE, abyteNewBlock);

                // add index
                var cobjSourceIndex = new List<int> { iNextBlockPosition, -2};
                var cobjDestIndex = new List<int> { iNextBlockPosition, iKontoNr };

                int iLine = UpdateIndex(TableTypes.ZULETZT, Files.Zuletzt, cobjSourceIndex, cobjDestIndex);
                if (iNextBlockPosition != iLine)
                    throw new Exception("Index could not be added! iNextBlockPosition != iLine");

                UpdateZuletztIndexPointer(iNextBlockPosition + 1);

                return iNextBlockPosition;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private void WriteZuletztBlock(int iBlockPosition, byte[] aBlockArray)
        {
            try
            {
                using (var objFilestream = new FileStream(Files.Zuletzt, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    objWriter.Seek(iBlockPosition, SeekOrigin.Begin);
                    objWriter.Write(aBlockArray);
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private void UpdateZuletztIndexPointer(Int32 iPointer)
        {
            try
            {
                using (var objFilestream = new FileStream(Files.Zuletzt, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    objWriter.Seek(0, SeekOrigin.Begin);
                    objWriter.Write(iPointer);
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private byte[] ReadZuletztBlock(int iIndex)
        {

            var abyteBlockBuffer = new byte[INDEX_BLOCKSIZE];

            using (var objFilestream = new FileStream(Files.Zuletzt, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
            using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
            {
                objReader.BaseStream.Seek(iIndex * ZULETZT_BLOCKSIZE, SeekOrigin.Begin);
                objReader.Read(abyteBlockBuffer, 0, ZULETZT_BLOCKSIZE);
            }

            return abyteBlockBuffer;
        }

        private void AddOrUpdateZuletzt(int iBlockOffset, int iPositionOffset, int iKontoNr, int iWareNr, int iDate, decimal dKolli, decimal dMenge, decimal dPreis)
        {
            using (var objFilestream = new FileStream(Files.Zuletzt, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
            using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
            {
                int iNextWritePos = 0;

                if (iKontoNr > 0)
                {
                    iNextWritePos = iBlockOffset + ZULETZT_BLOCK_HEADER + ((0 * ZULETZT_BLOCK_OFFSET) + iPositionOffset);
                    objWriter.BaseStream.Seek(iNextWritePos, SeekOrigin.Begin);
                    WriteValue(objWriter, iKontoNr, (int) DataTypes.INT323, 4);
                }

                if (iWareNr > 0)
                {
                    iNextWritePos = iBlockOffset + ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH + ((0 * ZULETZT_BLOCK_OFFSET) + iPositionOffset);
                    objWriter.BaseStream.Seek(iNextWritePos, SeekOrigin.Begin);
                    WriteValue(objWriter, iWareNr, (int) DataTypes.INT323, 4);
                }

                iNextWritePos = iBlockOffset + ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH + ((1 * ZULETZT_BLOCK_OFFSET) + iPositionOffset);
                objWriter.BaseStream.Seek(iNextWritePos, SeekOrigin.Begin);
                WriteValue(objWriter, iDate, (int)DataTypes.DATE, 4);

                iNextWritePos = iBlockOffset + ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH + ((2 * ZULETZT_BLOCK_OFFSET) + iPositionOffset);
                objWriter.BaseStream.Seek(iNextWritePos, SeekOrigin.Begin);
                WriteValue(objWriter, dKolli, (int)DataTypes.INT32mul100, 4);

                iNextWritePos = iBlockOffset + ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH + ((3 * ZULETZT_BLOCK_OFFSET) + iPositionOffset);
                objWriter.BaseStream.Seek(iNextWritePos, SeekOrigin.Begin);
                WriteValue(objWriter, dMenge, (int)DataTypes.INT32mul100, 4);

                iNextWritePos = iBlockOffset + ZULETZT_BLOCK_HEADER + ZULETZT_KONTO_WIDTH + ((4 * ZULETZT_BLOCK_OFFSET) + iPositionOffset);
                objWriter.BaseStream.Seek(iNextWritePos, SeekOrigin.Begin);
                WriteValue(objWriter, dPreis, (int)DataTypes.INT32mul100, 4);
            }
        }

        private DataRow ConvertZuletztToDataRow(DataTable objZuletzTable, byte[] abyteBlockBuffer, DataRow objDataRow, int iKontoNr, int iWarenNr, int iBlockBeginPos, int iOffsetIndex)
        {
            try
            {
                const int iWidthDate = 4;
                const int iWidthKolli = 4;
                const int iWidthMenge = 4;
                const int iWidthPreis = 4;

                // KontoNr
                objDataRow[0] = iKontoNr;
                // ArtikelNr
                objDataRow[1] = iWarenNr;
                // Date
                int iNextReaderPos = iBlockBeginPos + ((1 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                objDataRow[2] = Converters.IntToDate(Convert.ToInt32(ReadValue(abyteBlockBuffer, (int)DataTypes.DATE, iWidthDate, iNextReaderPos)));
                // Kolli
                iNextReaderPos = iBlockBeginPos + ((2 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                objDataRow[3] = Convert.ToDecimal(ReadValue(abyteBlockBuffer, (int)DataTypes.INT32mul100, iWidthKolli, iNextReaderPos));
                // Menge
                iNextReaderPos = iBlockBeginPos + ((3 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                objDataRow[4] = Convert.ToDecimal(ReadValue(abyteBlockBuffer, (int)DataTypes.INT32mul100, iWidthMenge, iNextReaderPos));
                // Preis
                iNextReaderPos = iBlockBeginPos + ((4 * ZULETZT_BLOCK_OFFSET) + iOffsetIndex);
                objDataRow[5] = Convert.ToDecimal(ReadValue(abyteBlockBuffer, (int)DataTypes.INT32mul100, iWidthPreis, iNextReaderPos));

                return objDataRow;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private IndexHeader GetIndexHeader(TableTypes enmTable, string strFile)
        {
            try
            {
                DataTable objTableHeader = ReadTableHeader(enmTable, strFile);

                const int iRow = 0;
                bool bLConverted = false;
                bool bZConverted = false;

                int.TryParse(objTableHeader.Rows[iRow][INDEX_COUNT].ToString(), out int iCount);
                    // count of lines > objLines+1 = next line with -2
                int.TryParse(objTableHeader.Rows[iRow][INDEX_E_LG].ToString(), out int iE_LG); // next line with -1

                int.TryParse(objTableHeader.Rows[0][INDEX_L_LG].ToString(), out int iL_LG);
                if (iL_LG > 0xFFFF)
                {
                    iL_LG = iL_LG >> 16; // shift 32bit to right
                    objTableHeader.Rows[0][INDEX_L_LG] = iL_LG;
                    bLConverted = true;
                }

                int.TryParse(objTableHeader.Rows[0][INDEX_Z_LG].ToString(), out int iZ_LG);
                if (iZ_LG > 0xFFFF)
                {
                    iZ_LG = iZ_LG >> 16; // shift 32bit to right
                    objTableHeader.Rows[0][INDEX_Z_LG] = iZ_LG;
                    bZConverted = true;
                }

                int.TryParse(objTableHeader.Rows[iRow][INDEX_NUMBER].ToString(), out int iNumber);

                var objIndexHeader = new IndexHeader()
                {
                    Count = iCount,
                    E_LG = iE_LG,
                    L_LG = iL_LG,
                    Z_LG = iZ_LG,
                    NUMBER = iNumber,
                    L_Converted = bLConverted,
                    Z_Converted = bZConverted
                };

                return objIndexHeader;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private bool SetIndexHeader(TableTypes enmTable, string strFile, IndexHeader objIndexHeader)
        {
            try
            {
                DataTable objTableHeader = ReadTableHeader(enmTable, strFile);

                objIndexHeader.E_LG = objIndexHeader.E_LG < 0 ? 0 : objIndexHeader.E_LG;
                objIndexHeader.L_LG = objIndexHeader.L_LG < 0 ? 0 : objIndexHeader.L_LG;

                const int iRow = 0;
                objTableHeader.Rows[iRow][INDEX_COUNT] = objIndexHeader.Count;
                objTableHeader.Rows[iRow][INDEX_E_LG] = objIndexHeader.E_LG;
                objTableHeader.Rows[iRow][INDEX_L_LG] = objIndexHeader.L_LG;
                objTableHeader.Rows[iRow][INDEX_Z_LG] = objIndexHeader.Z_LG;
                objTableHeader.Rows[iRow][INDEX_NUMBER] = objIndexHeader.NUMBER;

                if (objIndexHeader.L_Converted)
                {
                    int iL_LG = Convert.ToInt32(objTableHeader.Rows[0][INDEX_L_LG]);
                    iL_LG = iL_LG << 16; // shift 32bit to left
                    objTableHeader.Rows[0][INDEX_L_LG] = iL_LG;
                }

                if (objIndexHeader.Z_Converted)
                {
                    int iZ_LG = Convert.ToInt32(objTableHeader.Rows[0][INDEX_Z_LG]);
                    iZ_LG = iZ_LG << 16; // shift 32bit to left
                    objTableHeader.Rows[0][INDEX_Z_LG] = iZ_LG;
                }

                WriteTableHeader(enmTable, strFile, objTableHeader);

                return true;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private int AddIndexBlock(TableTypes enmTable, BinaryWriter objWriter, int iBlockPosition, int iRealBlockSize)
        {
            try
            {
                var cobjTableIndexes = GetTableIndexes(enmTable);
                object[] cobjDummyParams = GetDummyIndexArray(cobjTableIndexes);

                for (int iRealBlockOffset = 0; iRealBlockOffset < iRealBlockSize; iRealBlockOffset++)
                {
                    int iIndexWidthSum = 0;

                    for (int iIndexColumn = 0; iIndexColumn < cobjDummyParams.Length; iIndexColumn++)
                    {
                        int iIndexWidth = cobjTableIndexes[iIndexColumn][TABLEDEF_WIDTH];
                        int iType = cobjTableIndexes[iIndexColumn][TABLEDEF_TYPE];

                        int iIndexSeekPosition = iBlockPosition
                            + (iRealBlockSize * iIndexWidthSum)
                            + (iRealBlockOffset * iIndexWidth);

                        objWriter.Seek(iIndexSeekPosition, SeekOrigin.Begin);
                        WriteValue(objWriter, -2, iType, iIndexWidth);

                        iIndexWidthSum += iIndexWidth;
                    }
                }

                var iBlockLength = (int) objWriter.BaseStream.Length;
                int iFillByteLength = iBlockPosition + INDEX_BLOCKSIZE - iBlockLength;
                for (int iFillIndex = 0; iFillIndex < iFillByteLength; iFillIndex++)
                {
                    WriteValue(objWriter, 0, 0, 1);
                }

                return (int) objWriter.BaseStream.Length;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private bool CheckForIndexFields(TableTypes enmTable)
        {
            try
            {
                bool bIndexExists = false;
                string strTable = enmTable.ToString();
                int iTableRowsLength = TableInfos[strTable].Count;

                for (int iColumn = 1; iColumn <= iTableRowsLength - 1; iColumn++)
                {
                    int iIndexOrder = TableInfos[strTable][iColumn][TABLEDEF_INDEX_ORDER];
                    if (iIndexOrder > 0)
                    {
                        bIndexExists = true;
                        break;
                    }
                }

                return bIndexExists;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private DataTable ReadTableHeader(TableTypes enmTable, string strFile)
        {
            try
            {
                if (string.IsNullOrEmpty(strFile))
                    throw new ArgumentNullException(nameof(strFile));

                string strTable = enmTable.ToString();
                int iTableRowsLength = TableInfos[strTable].Count;
                DataTable objTargetTable = CreateTable(enmTable);

                using (var objFilestream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    objReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    var objDataRow = objTargetTable.NewRow();

                    // read row
                    for (int iColumn = 0; iColumn < iTableRowsLength; iColumn++)
                    {
                        // load values
                        int iIndexWidth = TableInfos[strTable][iColumn][TABLEDEF_WIDTH];
                        int iType = TableInfos[strTable][iColumn][TABLEDEF_TYPE];
                        object objValue = ReadValue(objReader, iType, iIndexWidth);

                        // column type date?
                        if (iType == (int) DataTypes.DATE)
                            objDataRow[iColumn] = Converters.IntToDate(Convert.ToInt32(objValue));
                        else
                            objDataRow[iColumn] = objValue;
                    }

                    objDataRow[iTableRowsLength] = GetFilename(strFile);
                    objDataRow[iTableRowsLength + 1] = 0;
                    objTargetTable.Rows.Add(objDataRow);
                }

                return objTargetTable;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private bool WriteTableHeader(TableTypes enmTable, string strFile, DataTable objHeader)
        {
            try
            {
                if (objHeader == null)
                    throw new ArgumentNullException(nameof(objHeader));

                if (objHeader.Rows.Count <= 0)
                    throw new Exception("objHeader must have a valid DataRow");

                string strTable = enmTable.ToString();
                int iTableRowsLength = TableInfos[strTable].Count;

                using (var objFilestream = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShareOnWrite))
                using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                {
                    // move to file position
                    objWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                    // write row
                    for (int iColumn = 0; iColumn < iTableRowsLength; iColumn++)
                    {
                        int iIndexWidth = TableInfos[strTable][iColumn][TABLEDEF_WIDTH];
                        int iType = TableInfos[strTable][iColumn][TABLEDEF_TYPE];
                        object objValue = objHeader.Rows[0][iColumn];

                        if (iType == (int)DataTypes.DATE)
                            objValue = Converters.DateToIntDDMMYY((DateTime) objValue);

                        if (iType == (int)DataTypes.STRING)
                            objValue = EnsureStringLength(enmTable, iColumn, objValue.ToString());

                        WriteValue(objWriter, objValue, iType, iIndexWidth);
                    }
                }

                return true;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private List<int> CreateIndexValues(TableTypes enmTable, int iLine, int iIndexValue)
        {
            try
            {
                var cobjTableIndexes = GetTableIndexes(enmTable);

                var cobjTargetIndexArry = new List<int> {iLine};

                for (int iIndexPos = 0; iIndexPos < cobjTableIndexes.Count; iIndexPos++)
                    cobjTargetIndexArry.Add(iIndexValue);

                return cobjTargetIndexArry;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public void RestoreLatest(string strFile)
        {
            try
            {
                string strDirectoryname = new FileInfo(strFile).DirectoryName;
                if (strDirectoryname == null)
                    return;

                string strFileIndex = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);

                string strFilename = new DirectoryInfo(strFile).Name;
                string strFolder = strFilename.Split(new[] {'.'})[0];
                string strNewDirectory = Path.Combine(strDirectoryname, strFolder);

                var objFile = new DirectoryInfo(strNewDirectory)
                        .GetFiles("*" + SUFFIX_DAT)
                        .OrderByDescending(f => f.LastWriteTime)
                        .First();

                var objFileIndex = new DirectoryInfo(strNewDirectory)
                        .GetFiles("*" + SUFFIX_NDX)
                        .OrderByDescending(f => f.LastWriteTime)
                        .First();

                if (objFile == null || objFileIndex == null)
                    return;

                if (objFile.Exists)
                {
                    if (File.Exists(strFile))
                        File.Delete(strFile);

                    objFile.CopyTo(strFile);
                }

                if (objFileIndex.Exists)
                {
                    if (File.Exists(strFileIndex))
                        File.Delete(strFileIndex);

                    objFileIndex.CopyTo(strFileIndex);
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private BackupInfo Backup(string strFile)
        {
            try
            {
                string strDirectoryname = new FileInfo(strFile).DirectoryName;

                if (strDirectoryname == null)
                    return null;

                string strFilename = new DirectoryInfo(strFile).Name;
                string strFolder = strFilename.Split(new[] {'.'})[0];
                string strNewDirectory = Path.Combine(strDirectoryname, strFolder);
                Directory.CreateDirectory(strNewDirectory);

                string strNewFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}_{strFilename}";
                string strTargetFile = Path.Combine(strNewDirectory, strNewFilename);

                if (File.Exists(strTargetFile))
                {
                    File.Delete(strTargetFile);
                    string strTargetFileIndex = strTargetFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                    if (strTargetFile != strTargetFileIndex)
                        if (File.Exists(strTargetFileIndex))
                            File.Delete(strTargetFileIndex);
                }

                var objBackupInfo = new BackupInfo();

                File.Copy(strFile, strTargetFile);
                objBackupInfo.SourceFile = strFile;
                objBackupInfo.DestinationFile = strTargetFile;

                string strFileIndex = strFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                if (strFile != strFileIndex)
                {
                    if (File.Exists(strFileIndex))
                    {
                        string strTargetFileIndex = strTargetFile.Replace(SUFFIX_DAT, SUFFIX_NDX);
                        File.Copy(strFileIndex, strTargetFileIndex);
                        objBackupInfo.IndexSourceFile = strFileIndex;
                        objBackupInfo.IndexDestinationFile = strTargetFileIndex;
                    }
                }

                return objBackupInfo;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public bool Restore(BackupInfo objBackupInfo)
        {
            try
            {
                File.Copy(objBackupInfo.DestinationFile, objBackupInfo.SourceFile, true);
                File.Copy(objBackupInfo.IndexDestinationFile, objBackupInfo.IndexSourceFile, true);
                return true;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private int[] CreateIndexArray(IList<int> cobIndexArray)
        {
            try
            {
                if (cobIndexArray == null)
                    throw new ArgumentNullException(nameof(cobIndexArray));

                // skip first entry > first entry = index line
                var aobjParams = new int[cobIndexArray.Count - 1];
                for (int iParam = 0; iParam < cobIndexArray.Count - 1; iParam++)
                    aobjParams[iParam] = cobIndexArray[iParam + 1];

                return aobjParams;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private List<int> GetIndexesOfDataRow(TableTypes enmTable, int iTableRowsLength, DataRow objDataRow)
        {
            try
            {
                var cobIndexArrays = new Dictionary<int, int> {{0, 0}};
                string strTable = enmTable.ToString();

                // collect indexes
                for (int iColumn = 0; iColumn < iTableRowsLength; iColumn++)
                {
                    int iIndexOrder = TableInfos[strTable][iColumn][TABLEDEF_INDEX_ORDER];

                    if (iIndexOrder > 0)
                    {
                        int iType = TableInfos[strTable][iColumn][TABLEDEF_TYPE];
                        object objValue = objDataRow[iColumn];

                        if (iType == (int) DataTypes.DATE)
                            objValue = Converters.DateToIntYYYYMMDD((DateTime) objValue);

                        cobIndexArrays.Add(iIndexOrder, Convert.ToInt32(objValue));
                    }
                }

                return cobIndexArrays
                    .ToList()
                    .OrderBy(k => k.Key)
                    .Select(d => d.Value)
                    .ToList();
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private void CreateEmptyFile(string strFile, bool bOverWrite)
        {
            try
            {
                if (bOverWrite)
                {
                    if (File.Exists(strFile))
                        File.Delete(strFile);

                    File.Create(strFile).Dispose();
                }
                else if (!File.Exists(strFile))
                {
                    File.Create(strFile).Dispose();
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private int GetIndexLength(IEnumerable<int[]> cobjTableIndexes)
        {
            try
            {
                int iIndexLength = cobjTableIndexes.Sum(i => i[TABLEDEF_WIDTH]);
                return iIndexLength;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private int GetRowLength(string strTable)
        {
            try
            {
                var cobjTableInfo = TableInfos[strTable];
                int iRowLength = cobjTableInfo.Sum(i => i[TABLEDEF_WIDTH]);
                return iRowLength;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private string EnsureStringLength(TableTypes enmTable, int iColumn, string strValue)
        {
            try
            {
                int iWidth = TableInfos[enmTable.ToString()][iColumn][TABLEDEF_WIDTH];
                int iStringLength = strValue.Length;

                if (iStringLength > iWidth)
                {
                    return strValue.Substring(0, iWidth);
                }
                else
                {
                    for (int i = 0; i < iWidth - iStringLength; i++)
                        strValue += " ";

                    return strValue;
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private string ByteArrayToString(byte[] bBuffer)
        {
            try
            {
                Encoding src = Encoding.GetEncoding(CODEPAGE);
                Encoding dst = Encoding.GetEncoding(CODEPAGE);

                byte[] bResult = Encoding.Convert(src, dst, bBuffer);
                var s = new string(dst.GetChars(bResult));
                return s;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private string ByteArrayToString(byte[] bBuffer, int iIndex, int iCount)
        {
            try
            {
                Encoding src = Encoding.GetEncoding(CODEPAGE);
                Encoding dst = Encoding.GetEncoding(CODEPAGE);

                byte[] bResult = Encoding.Convert(src, dst, bBuffer, iIndex, iCount);
                var s = new string(dst.GetChars(bResult));
                return s;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private void WriteArrayToFile(IEnumerable<string[]> cobjLines, string strDelimitier, string strFile)
        {
            try
            {
                var cobjDestinationLines = new List<string>();

                foreach (string[] astrLine in cobjLines)
                {
                    string strLine = string.Empty;
                    foreach (string strColumn in astrLine)
                        strLine = strLine + $"{strColumn}{strDelimitier}";

                    cobjDestinationLines.Add(strLine.Substring(0, strLine.Length - strDelimitier.Length));
                }

                if (File.Exists(strFile))
                    File.Delete(strFile);

                File.WriteAllLines(strFile, cobjDestinationLines.ToArray());
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private void WriteArrayToFile(List<int[]> cobjArrays, string strDelimitier, string strFile)
        {
            try
            {
                var cobjDestinationLines = new List<string>();

                for (int iLineIndex = 0; iLineIndex < cobjArrays[0].Length; iLineIndex++)
                {
                    string strLine = cobjArrays.Aggregate(string.Empty, (current, t) => current + $"{t[iLineIndex]}{strDelimitier}");
                    cobjDestinationLines.Add(strLine.Substring(0, strLine.Length - strDelimitier.Length));
                }

                if (File.Exists(strFile))
                    File.Delete(strFile);

                File.WriteAllLines(strFile, cobjDestinationLines.ToArray());
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private Dictionary<string, List<int[]>> ReadTableInfos()
        {
            try
            {
                var cobjTableInfos = new Dictionary<string, List<int[]>>();
                cobjTableInfos = ReadTableInfosFromFile(cobjTableInfos);
                cobjTableInfos = ReadTableInfosManually(cobjTableInfos);
                return cobjTableInfos;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private Dictionary<string, List<int[]>> ReadTableInfosFromFile(Dictionary<string, List<int[]>> cobjTableInfos)
        {
            try
            {
                var objHeader = CreateTableDefHeader();

                int iA = Encoding.ASCII.GetBytes("A")[0];
                int iZ = Encoding.ASCII.GetBytes("Z")[0];

                int iCount = 0;

                List<int[]> cobjColumns = null;
                string sTablename = string.Empty;

                while (objHeader.ByteBuffer.Length != iCount)
                {
                    string sTmp = ReadValue(objHeader.ByteBuffer, 1, 1, iCount).ToString();
                    int iCurrent = Encoding.ASCII.GetBytes(sTmp.Substring(0, 1))[0];

                    if ((iCurrent >= iA) && (iCurrent <= iZ))
                    {
                        if (!string.IsNullOrEmpty(sTablename))
                            cobjTableInfos.Add(sTablename, cobjColumns);

                        cobjColumns = new List<int[]>();

                        int iTmp = objHeader.StringBuffer.Substring(iCount, 9).IndexOf(".", System.StringComparison.Ordinal);
                        sTablename = ReadValue(objHeader.ByteBuffer, 1, iTmp, iCount).ToString();
                        iCount = iCount + 8 + 3 + 2;
                    }

                    var cintColumns = new int[5];
                    cintColumns[0] = Convert.ToInt32(ReadValue(objHeader.ByteBuffer, 2, 2, iCount + 0));
                    cintColumns[1] = Convert.ToInt32(ReadValue(objHeader.ByteBuffer, 2, 2, iCount + 2));
                    cintColumns[2] = Convert.ToInt32(ReadValue(objHeader.ByteBuffer, 2, 2, iCount + 4));
                    cintColumns[3] = Convert.ToInt32(ReadValue(objHeader.ByteBuffer, 2, 2, iCount + 6));
                    cintColumns[4] = Convert.ToInt32(ReadValue(objHeader.ByteBuffer, 2, 2, iCount + 8));

                    cobjColumns?.Add(cintColumns);

                    iCount = iCount + 10;
                }

                return cobjTableInfos;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private Dictionary<string, List<int[]>> ReadTableInfosManually(Dictionary<string, List<int[]>> cobjTableInfos)
        {
            try
            {
                List<int[]> cobjColumns = null;

                cobjColumns = new List<int[]>
                {
                    new int[5] {22, 1, 0, 0, 0},
                    new int[5] {2, 2, 0, 0, 0},
                    new int[5] {4, 4, 0, 0, 0},
                    new int[5] {4, 4, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.KONTENGR3.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {22, 1, 0, 0, 0},
                    new int[5] {2, 2, 0, 0, 0},
                    new int[5] {4, 4, 0, 0, 0},
                    new int[5] {4, 4, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.KONTENGR2.ToString(), cobjColumns);

                cobjColumns = new List<int[]> {new int[5] {22, 1, 0, 0, 0}, new int[5] {10, 2, 0, 0, 0}};
                cobjTableInfos.Add(TableTypes.WARENOGR.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {22, 1, 0, 0, 0},
                    new int[5] {8, 2, 0, 0, 0},
                    new int[5] {2, 2, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.WARENUGR.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {2, 2, 0, 0, 0},
                    new int[5] {20, 1, 0, 0, 0},
                    new int[5] {3, 1, 0, 0, 0},
                    new int[5] {4, 1, 0, 0, 0},
                    new int[5] {3, 1, 0, 0, 0},
                    new int[5] {2, 1, 0, 0, 0},
                    new int[5] {2, 2, 0, 0, 0},
                    new int[5] {1, 1, 0, 0, 0},
                    new int[5] {27, 1, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.COUNTRY.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {4, 6, 0, 0, 0},
                    new int[5] {8, 5, 0, 0, 0},
                    new int[5] {8, 5, 0, 0, 0},
                    new int[5] {30, 1, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.EINAUSJJ.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {2, 2, 0, 0, 0},
                    new int[5] {20, 1, 0, 0, 0},
                    new int[5] {3, 1, 0, 0, 0},
                    new int[5] {2, 2, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.TOUR.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {2, 2, 0, 0, 0},
                    new int[5] {20, 1, 0, 0, 0},
                    new int[5] {3, 1, 0, 0, 0},
                    new int[5] {2, 2, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.ZAHLENMMJJ.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {4, 3, 0, 0, 0},
                    new int[5] {4, 3, 0, 0, 0},
                    new int[5] {4, 3, 0, 0, 0},
                    new int[5] {4, 3, 0, 0, 0}
                };
                cobjTableInfos.Add(TableTypes.FIRMA.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {12, 1, 0, 0, 0},
                    new int[5] {4, 3, 0, 0, 0},
                };
                cobjTableInfos.Add(TableTypes.PROGRAM.ToString(), cobjColumns);

                cobjColumns = new List<int[]>
                {
                    new int[5] {2, 7, 0, 0, 0},
                    new int[5] {2, 7, 0, 0, 0},
                };
                cobjTableInfos.Add(TableTypes.STEUER.ToString(), cobjColumns);

                return cobjTableInfos;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private TableHeader CreateTableDefHeader()
        {
            byte[] bBuffer;

            try
            {
                var objFileInfo = new FileInfo(Files.Dateien);
                using (var objFileStream = new FileStream(Files.Dateien, FileMode.OpenOrCreate, FileAccess.Read, FileShareOnRead))
                using (var objReader = new BinaryReader(objFileStream))
                {
                    bBuffer = new byte[objFileInfo.Length];
                    int iBuffer = objReader.Read(bBuffer, 0, Convert.ToInt32(objFileInfo.Length));

                    if (iBuffer != objFileInfo.Length)
                        throw new Exception("File can't be read completely!");
                }
            }
            catch (Exception objException)
            {
                bBuffer = Properties.Resources.DATEIEN;
                Log(objException);
            }

            string sBuffer = ByteArrayToString(bBuffer);

            var objHeader = new TableHeader
            {
                ByteBuffer = bBuffer,
                StringBuffer = sBuffer
            };

            return objHeader;
        }

        private string GetFilename(string strFile)
        {
            try
            {
                var strFilename = new Regex(@"[^\\]*$",
                    RegexOptions.IgnoreCase
                    | RegexOptions.CultureInvariant
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled)
                    .Match(strFile);

                return strFilename.Value;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public DataTable CreateTable(TableTypes enmTable)
        {
            try
            {
                string strTable = enmTable.ToString();
                int iTableRowsLength = TableInfos[strTable].Count;

                // Create DataTable
                var objDt = new DataTable(strTable);

                for (int iCol = 0; iCol <= iTableRowsLength - 1; iCol++)
                {
                    if (enmTable == TableTypes.ZULETZT)
                    {
                        int iType = TableInfos[strTable][iCol][TABLEDEF_TYPE];
                        if (iType == 3 && iCol == 0)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(Int32));
                        }
                        else if (iType == 3 && iCol == 1)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(Int32));
                        }
                        else if (iType == 13 && iCol == 2)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(DateTime));
                        }
                        else if (iType == 13 && iCol == 3)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(Double));
                        }
                        else if (iType == 13 && iCol == 4)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(Double));
                        }
                        else if (iType == 13 && iCol == 5)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(Double));
                        }
                        else if (iType == 13 && iCol == 6)
                        {
                            objDt.Columns.Add("c" + iCol.ToString(), typeof(Double));
                        }

                    }
                    else
                    {
                        int iType = TableInfos[strTable][iCol][TABLEDEF_TYPE];
                        switch (iType)
                        {
                            case 1:
                                objDt.Columns.Add("c" + iCol, typeof(string));
                                break;
                            case 2:
                                objDt.Columns.Add("c" + iCol, typeof(Int16));
                                break;
                            case 3:
                                objDt.Columns.Add("c" + iCol, typeof(Int32));
                                break;
                            case 4:
                                objDt.Columns.Add("c" + iCol, typeof(Single));
                                break;
                            case 5:
                                objDt.Columns.Add("c" + iCol, typeof(Double));
                                break;
                            case 6:
                                objDt.Columns.Add("c" + iCol, typeof(DateTime));
                                break;
                            case 7:
                                objDt.Columns.Add("c" + iCol, typeof(Int16));
                                break;
                            case 8:
                                objDt.Columns.Add("c" + iCol, typeof(Double));
                                break;
                            case 9:
                                objDt.Columns.Add("c" + iCol, typeof(Int32));
                                break;
                        }
                    }
                }

                objDt.Columns.Add("FILENAME", typeof(String));
                objDt.Columns.Add("ROW", typeof(Int32));

                return objDt;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private List<int[]> GetTableIndexes(TableTypes enmTable)
        {
            try
            {
                List<int[]> cobjTableIndexes = TableInfos[enmTable.ToString()]
                    .Where(r => r[TABLEDEF_INDEX_ORDER] > 0)
                    .OrderBy(c => c[TABLEDEF_INDEX_ORDER])
                    .ToList();

                return cobjTableIndexes;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private object[] GetDummyIndexArray(ICollection cobjTableIndexes)
        {
            try
            {
                int iParamCount = cobjTableIndexes.Count;
                var aobjParams = new object[iParamCount];

                for (int i = 0; i < iParamCount; i++)
                    aobjParams[i] = -1;

                return aobjParams;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private bool MatchValues(object objValue1, object objValue2, int iType)
        {
            try
            {
                switch (iType)
                {
                    case (int)DataTypes.STRING:
                        return objValue1.ToString() == objValue2.ToString();

                    case (int)DataTypes.INT16:
                        return Convert.ToInt16(objValue1) == Convert.ToInt16(objValue2);

                    case (int)DataTypes.INT323:
                        return Convert.ToInt32(objValue1) == Convert.ToInt32(objValue2);

                    case (int)DataTypes.SINGLE:
                        return Math.Abs((Single)objValue1 - (Single)objValue2) < 0.009;

                    case (int)DataTypes.DOUBLEdiv100:
                        return Math.Abs((double) objValue1 - (double) objValue2) < 0.009;

                    case (int)DataTypes.DATE:
                        return Convert.ToInt32(objValue1) == Convert.ToInt32(objValue2);

                    case (int)DataTypes.INT16mul100:
                        if (Convert.ToInt32(objValue2) == ALL)
                            objValue2 = 0;

                        return Convert.ToUInt16(objValue1) == Convert.ToUInt16(objValue2);

                    case (int)DataTypes.INT32mul100:
                        return Convert.ToInt32(objValue1) == Convert.ToInt32(objValue2);

                    case (int)DataTypes.INT329:
                        return Convert.ToInt32(objValue1) == Convert.ToInt32(objValue2);

                    default:
                        return false;
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private object ReadValue(byte[] bBuffer, int iType, int iCount, int iIndex)
        {
            try
            {
                switch (iType)
                {
                    case (int)DataTypes.STRING:
                        return ByteArrayToString(bBuffer, iIndex, iCount);

                    case (int)DataTypes.INT16:
                        return BitConverter.ToInt16(bBuffer, iIndex);

                    case (int)DataTypes.INT323:
                        return BitConverter.ToInt32(bBuffer, iIndex);

                    case (int)DataTypes.SINGLE:
                        return BitConverter.ToSingle(bBuffer, iIndex);

                    case (int)DataTypes.DOUBLEdiv100:
                        return BitConverter.ToDouble(bBuffer, iIndex) / 100d;

                    case (int)DataTypes.DATE:
                        return BitConverter.ToInt32(bBuffer, iIndex);

                    case (int)DataTypes.INT16mul100:
                        return BitConverter.ToUInt16(bBuffer, iIndex) / 100;

                    case (int)DataTypes.INT32mul100:
                        return BitConverter.ToInt32(bBuffer, iIndex) / 100d;

                    case (int)DataTypes.INT329:
                        return BitConverter.ToInt32(bBuffer, iIndex);
                    default:
                        return null;
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private object ReadValue(BinaryReader objReader, int iType, int iWidth)
        {
            try
            {
                switch (iType)
                {
                    // 1 - CHAR
                    case 1:
                        return Encoding.GetEncoding(CODEPAGE).GetString(objReader.ReadBytes(iWidth));
                        // return ReplaceToGermanMarks(ByteArrayToString(objReader.ReadBytes(iWidth)));

                    // 2 - INTEGER ->  CVI(s)
                    case 2:
                        return objReader.ReadInt16();

                    // 3 - LONGINTEGER ->  CVL(s)
                    case 3:
                        return objReader.ReadInt32();

                    // 4 - REAL    ->  CVS(s)
                    case 4:
                        return objReader.ReadSingle();

                    // 5 - DOUBLE*100  ->  INT(CVD(s)/100)
                    case 5:
                        return objReader.ReadDouble() / 100d;

                    // 6 - DATUM LONG  ->  CVL(s)
                    case 6:
                        return objReader.ReadInt32();

                    // 7 - INTEGER*100 ->  INT(CVI(s)/100)
                    case 7:
                        return objReader.ReadUInt16() / 100;

                    // 8 - INTEGER*100 ->  INT(CVL(s)/100)
                    case 8:
                        return objReader.ReadInt32() / 100d;

                    // 9 - LONGINTEGER ->  CVL(s)
                    case 9:
                        return objReader.ReadInt32();
                    default:
                        return null;
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private void WriteValue(BinaryWriter objWriter, object objValue, int iType, int iWidth)
        {
            try
            {
                decimal dValue;

                switch (iType)
                {
                    // 0 - BYTE //8bit
                    case (int)DataTypes.BYTE:
                        objWriter.Write(Convert.ToByte(objValue));
                        break;

                    // 1 - STRING
                    case (int)DataTypes.STRING:
                        objWriter.Write(Encoding.GetEncoding(CODEPAGE).GetBytes(objValue.ToString()));
                        break;

                    // 2 - INT16 ->  CVI(s) //16bit
                    case (int)DataTypes.INT16:
                        objWriter.Write((short)Convert.ToInt16(objValue));
                        break;

                    // 3 - INT32 ->  CVL(s) //32bit
                    case (int)DataTypes.INT323:
                        objWriter.Write(Convert.ToInt32(objValue));
                        break;

                    // 4 - SINGLE    ->  CVS(s) //32bit
                    case (int)DataTypes.SINGLE:
                        objWriter.Write(Convert.ToSingle(objValue));
                        break;

                    // 5 - DOUBLEdev100  ->  INT(CVD(s)/100) //64bit
                    case (int)DataTypes.DOUBLEdiv100:
                        objWriter.Write(Convert.ToDouble(objValue) * 100d);
                        break;

                    // 6 - DATE  ->  CVL(s) //32bit
                    case (int)DataTypes.DATE:
                        objWriter.Write(Convert.ToInt32(objValue));
                        break;

                    // 7 - INT16mul100 ->  INT(CVI(s)*100) //16bit
                    case (int)DataTypes.INT16mul100:
                        dValue = Convert.ToDecimal(objValue) * 100;
                        objWriter.Write(Convert.ToUInt16(dValue));
                        break;

                    // 8 - INT32mul100 ->  INT(CVL(s)/100) //32bit
                    case (int)DataTypes.INT32mul100:
                        dValue = Convert.ToDecimal(objValue) * 100;
                        objWriter.Write(Convert.ToInt32(dValue));
                        break;

                    // 9 - INT32 ->  CVL(s) //32bit
                    case (int)DataTypes.INT329:
                        objWriter.Write(Convert.ToInt32(objValue));
                        break;
                }
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private bool TryFileAccessable(string strFile, FileMode enmFileMode, FileAccess enmFileAccess, FileShare enmFileShare)
        {
            try
            {
                using (var objFilestream = new FileStream(strFile, enmFileMode, enmFileAccess, enmFileShare))
                {
                    switch (enmFileAccess)
                    {
                        case FileAccess.Read:
                            using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                            {
                            }
                            break;
                        case FileAccess.Write:
                            using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                            {
                            }
                            break;
                        case FileAccess.ReadWrite:
                            using (var objReader = new BinaryReader(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                            using (var objWriter = new BinaryWriter(objFilestream, Encoding.GetEncoding(CODEPAGE)))
                            {
                            }
                            break;
                    }
                }

                return true;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        private bool CheckWritePermission(string strFile)
        {
            try
            {
                File.Open(strFile, FileMode.Open, FileAccess.Write).Dispose();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private bool CheckReadPermission(string strFile)
        {
            try
            {
                File.Open(strFile, FileMode.Open, FileAccess.Read).Dispose();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private FileStream TryOpen(string strFilePath, FileMode enmFileMode, FileAccess enmFileAccess, FileShare enmFileShare, int iMaximumAttempts, int iAttemptWaitMS)
        {
            try
            {
                FileStream objFileStream = null;
                int iAttempts = 0;

                // Loop allow multiple attempts
                while (true)
                {
                    try
                    {
                        objFileStream = File.Open(strFilePath, enmFileMode, enmFileAccess, enmFileShare);
                        // If we get here, the File.Open succeeded, so break out of the loop and return the FileStream
                        break;
                    }
                    catch (IOException)
                    {
                        // IOExcception is thrown if the file is in use by another process.
                        // Check the numbere of attempts to ensure no infinite loop
                        iAttempts++;
                        if (iAttempts > iMaximumAttempts)
                        {
                            // Too many attempts,cannot Open File, break and return null 
                            objFileStream = null;
                            break;
                        }
                        else
                        {
                            // Sleep before making another attempt
                            Thread.Sleep(iAttemptWaitMS);
                        }
                    }
                }
                // Reutn the filestream, may be valid or null
                return objFileStream;
            }
            catch (Exception objException)
            {
                Log(objException);
                throw;
            }
        }

        public TraceSource InitTraceSource()
        {
            var strAppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            TraceSource = new TraceSource(strAppName)
            {
                Switch = new SourceSwitch("sourceSwitch", "Verbose")
            };

            TraceSource.Listeners.Remove("Default");

            var objtextListener = new TextWriterTraceListener($@"{Environment.CurrentDirectory} \ {strAppName}.log")
            {
                Filter = new EventTypeFilter(SourceLevels.Verbose)
            };

            TraceSource.Listeners.Add(objtextListener);
            return TraceSource;
        }

        public void Log(Exception objException)
        {
            if (TraceSource == null)
                InitTraceSource();

            var strException = GetExceptions(objException);
            TraceSource.TraceEvent(TraceEventType.Error, 1000, $"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: {strException}");
            TraceSource.Flush();            
        }

        public void Log(string strMessage)
        {
            if (TraceSource == null)
            {
                InitTraceSource();
            }

            TraceSource.TraceEvent(TraceEventType.Information, 1000, $"{DateTime.Now.ToString(CultureInfo.CurrentCulture)}: {strMessage}");
            TraceSource.Flush();
        }

        private string GetExceptions(Exception objException)
        {
            var objTempException = objException;

            var cstrExceptions = new StringBuilder();
            if (objTempException != null)
            {
                cstrExceptions.AppendLine("Message: " + objTempException.Message);
                cstrExceptions.AppendLine("Exception type: " + objTempException.GetType().FullName);
                cstrExceptions.AppendLine("Stacktrace:");
                cstrExceptions.AppendLine(objTempException.StackTrace);
                cstrExceptions.AppendLine();
                objTempException = objTempException.InnerException;
            }

            return cstrExceptions.ToString();
        }

        #endregion
    }
}
