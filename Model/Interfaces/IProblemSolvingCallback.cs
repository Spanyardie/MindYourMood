namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IProblemSolvingCallback
    {
        void ConfirmAddition(int problemID, string problemText);
        void CancelAddition();
    }
}