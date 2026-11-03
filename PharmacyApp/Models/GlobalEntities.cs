namespace PharmacyApp.Models
{
    /// <summary>
    /// Static, application-wide cache of rarely-changing configuration entities
    /// (per rulebook Rule 10.1), loaded once at startup so ViewModels don't
    /// re-query them on every screen.
    ///
    /// Populated in a later step once a Setting entity exists (e.g. near-expiry
    /// threshold days, currency symbol, tax rate — see master prompt Sections 19/45).
    /// Left empty for now; MainViewModel's startup sequence will load it when
    /// the Setting module is built.
    /// </summary>
    public static class GlobalEntities
    {
        public static User CurrentUser { get; set; }
        // public static Setting Setting { get; set; }
    }
}
