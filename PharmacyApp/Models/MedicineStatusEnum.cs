namespace PharmacyApp.Models
{
    /// <summary>
    /// Whether a Medicine is currently allowed to be sold (Section 34).
    /// Separate from stock quantity — a medicine can have stock but be Inactive.
    /// </summary>
    public enum MedicineStatusEnum
    {
        Active,
        Inactive
    }
}