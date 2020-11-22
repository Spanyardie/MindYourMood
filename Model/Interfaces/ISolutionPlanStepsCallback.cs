namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface ISolutionPlanStepsCallback
    {
        void ConfirmAddition(int solutionPlanStepID, int problemIdeaID, string solutionStepText, int priorityOrder);
        void CancelAddition();
    }
}