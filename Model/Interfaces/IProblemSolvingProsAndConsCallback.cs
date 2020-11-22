using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IProblemSolvingProsAndConsCallback
    {
        void ConfirmAddition(int problemID, int problemStepID, int problemIdeaID, int problemProAndConID, string problemProAndConText, ConstantsAndTypes.PROCON_TYPES problemProAndConType);
        void CancelAddition();
    }
}