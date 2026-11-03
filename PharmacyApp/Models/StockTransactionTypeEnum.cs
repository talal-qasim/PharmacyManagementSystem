namespace PharmacyApp.Models
{
    /// <summary>
    /// Every reason a batch's quantity can change (Section 28). Every stock movement
    /// in the system is recorded as one of these — never a silent quantity edit.
    /// </summary>
    public enum StockTransactionTypeEnum
    {
        Purchase,
        Sale,
        SaleCancellation,
        Return,
        Damage,
        Expiry,
        Adjustment

        // Future (Section 28): SupplierReturn, Transfer, Recall
    }
}