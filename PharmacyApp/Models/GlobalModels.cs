using System;
using MyMessageBox;
using PharmacyApp.ViewModels;

namespace PharmacyApp.Models
{
    /// <summary>
    /// Static application-wide holders (per rulebook Rule 10.1):
    /// - MainViewModel: the single root shell ViewModel instance, so any deeply nested
    ///   ViewModel can reach shell-level state if it ever needs to (e.g. force navigation).
    /// - FoundExceptionUI: the one place every catch block in the app calls into.
    /// </summary>
    public static class GlobalModels
    {
        public static MainViewModel MainViewModel { get; set; }

        /// <summary>
        /// Central UI-facing exception handler (per rulebook Rule 10.2).
        /// Logs to file via Common.HandleException.FoundException(ex), then shows
        /// a modal error dialog so the pharmacy employee knows the action failed
        /// instead of the app silently doing nothing.
        /// </summary>
        public static void FoundExceptionUI(Exception ex)
        {
            Common.HandleException.FoundException(ex);

            CustomMessageBoxView.Show(
                "Error",
                ex?.Message ?? "An unexpected error occurred.",
                MessageBoxIconType.Error,
                MessageBoxButtonsType.Ok);
        }
    }
}
