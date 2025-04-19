namespace gmdb.Core
{
    internal class IndexHeader
    {
        // COUNT = 0 > total lines
        // E_LG  = 1 > pointer to next free line
        // L_LG  = 2 > pointer to last deleted line
        // Z_LG  = 3 > total free lines
        public int Count { get; set; }

        public int E_LG { get; set; }

        public int L_LG { get; set; }

        public int Z_LG { get; set; }

        public int NUMBER { get; set; }

        public bool L_Converted { get; set; }

        public bool Z_Converted { get; set; }
    }
}
