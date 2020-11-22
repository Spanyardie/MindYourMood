namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IProblemSolvingIdeasCallback
    {
        void ConfirmAddition(int problemID, int problemStepID, int problemIdeaID, string problemIdeaText);
        void CancelAddition();
    }
}