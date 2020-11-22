namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IProblemSolvingStepsCallback
    {
        void ConfirmAddition(int problemStepID, int problemID, string problemStepText, int priorityOrder);
        void CancelAddition();
    }
}