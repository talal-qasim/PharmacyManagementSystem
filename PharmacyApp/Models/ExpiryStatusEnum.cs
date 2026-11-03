namespace PharmacyApp.Models
{
    /// <summary>
    /// A batch's expiry classification (Sections 18-20), independent of its
    /// stock-level status (Normal/Low/Out of Stock lives on ProductVariant, Step 5+).
    /// </summary>
    public enum ExpiryStatusEnum
    {
        Normal,
        NearExpiry,
        Expired
    }
}