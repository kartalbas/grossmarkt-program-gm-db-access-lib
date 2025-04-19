namespace gmdb.Core
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [Flags]
    public enum Belegart
    {
        [EnumMember]
        Gutschrift = 1,
        [EnumMember]
        Lieferschein = 2,
        [EnumMember]
        Rechnung = 3,
        [EnumMember]
        Posnummer = 4,
        [EnumMember]
        Zukaufpositionen = 5
    }

    [DataContract]
    [Flags]
    public enum Zahlungsart
    {
        [EnumMember]
        Barzahlung = 1,
        [EnumMember]
        Scheck = 2,
        [EnumMember]
        Uberweisung = 3,
        [EnumMember]
        Lastschrift = 4,
        [EnumMember]
        Wechsel = 5,
        [EnumMember]
        Ruckscheck = 6,
        [EnumMember]
        SkontoRabatt = 7,
        [EnumMember]
        Verrechnung = 8,
        [EnumMember]
        Korrektur = 9
    }

    [DataContract]
    [Flags]
    public enum Umsatzbesteuerung
    {
        [EnumMember]
        Land1 = 1,
        [EnumMember]
        Land2 = 2,
        [EnumMember]
        Land3 = 3,
        [EnumMember]
        Land4 = 4,
        [EnumMember]
        Land5 = 5
    }

    [DataContract]
    [Flags]
    public enum EDocumentState
    {
        [EnumMember]
        NEW = 0,
        [EnumMember]
        CHANGED = 1,
    }
    [DataContract]
    [Flags]
    public enum EDocumentdetailState
    {
        [EnumMember]
        NEW = 0,
        [EnumMember]
        CHANGED = 1,
        [EnumMember]
        REMOVED = 2,
        [EnumMember]
        EDIT = 3
    }
}
