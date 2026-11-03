namespace PharmacyApp.Models
{
    /// <summary>
    /// The physical form of a medicine. Drives which StockUnit values make sense
    /// for a given ProductVariant (Section 6/61 of the master prompt).
    /// </summary>
    public enum DosageFormEnum
    {
        Tablet,
        Capsule,
        Syrup,
        Liquid,
        Tube,       // ointment/gel/cream
        EyeDrops,
        Injection,
        Other
    }
}