namespace gmdb
{
    using System.Data;

    public class TableHeader
    {
        public int RowLength { get; set; }

        public int TableRowsLength { get; set; }

        public DataTable Table { get; set; }

        public string StringBuffer { get; set; }

        public byte[] ByteBuffer { get; set; }
    }
}
