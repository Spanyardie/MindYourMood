namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IAffirmationCallback
    {
        void ConfirmAddition(int affirmationID, string affirmationText);
        void CancelAddition();
    }
}