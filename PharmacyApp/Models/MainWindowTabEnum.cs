namespace PharmacyApp.Models
{
    /// <summary>
    /// Drives the headerless TabControl in MainWindow.xaml (Rule 8.3).
    /// Add one value here per top-level module as it's built. Order below follows
    /// the build roadmap (Step 1 = Dashboard placeholder only; subsequent steps
    /// append their tab as each module is implemented).
    /// </summary>
    public enum MainWindowTabEnum
    {
        Dashboard,

        // Step 2 — lookup entities
        MedicineCategory,
        Manufacturer,
        Supplier,

        // Step 3 — medicine domain
        Medicine,

        // Step 4 — inventory
        Batch,
        StockTransaction,

        // Step 5 — purchasing
        Purchase,

        // Step 6 — sales
        Sale,

        // Step 7 — reports
        Reports,

        // Step 8 — auth/roles
        UserManagement
    }
}
