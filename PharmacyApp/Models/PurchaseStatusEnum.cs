namespace PharmacyApp.Models
{
    /// <summary>
    /// Section 23 — simple MVP workflow. Draft purchases can still be edited
    /// (items added/removed); Received purchases are locked and their items
    /// have already become real Batches with stock (Section 27 forbids
    /// increasing stock merely because a draft was created).
    /// </summary>
    public enum PurchaseStatusEnum
    {
        Draft,
        Received,
        Cancelled
    }
}