namespace MyMessageBox
{
    /// <summary>
    /// Result returned by CustomMessageBox.Show(...), consumed by ViewModels/DAL callers
    /// that need to know whether the user confirmed a destructive action (e.g. Delete).
    /// </summary>
    public enum MessageBoxResultType
    {
        None,
        Ok,
        Yes,
        No,
        Cancel
    }

    /// <summary>
    /// Which icon/color scheme the dialog displays.
    /// </summary>
    public enum MessageBoxIconType
    {
        Information,
        Success,
        Warning,
        Error,
        Question
    }

    /// <summary>
    /// Which button set the dialog shows.
    /// </summary>
    public enum MessageBoxButtonsType
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }
}
