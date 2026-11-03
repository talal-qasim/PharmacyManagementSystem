namespace PharmacyApp.Models
{
    /// <summary>
    /// The actual sellable/stock unit for a ProductVariant (Section 6/61).
    /// This is what gets deducted on a sale — never grams/mL for tubes/syrups.
    /// </summary>
    public enum StockUnitEnum
    {
        Tablet,
        Capsule,
        Tube,
        Bottle,
        Vial,
        Ampoule,
        Other
    }
}