namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IGenericTextCallback
    {
        void ConfirmText(string textEntered, int genericTextID);
        void CancelText();
        int SelectedItemIndex { get; }
        void OnGenericDialogDismiss();
    }
}