using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IStructuredPlanReactionsCallback
    {
        void ConfirmPlanItemAddition(int reactionsID, string toWhat, int strength, ConstantsAndTypes.REACTION_TYPE reaction, ConstantsAndTypes.ACTION_TYPE action, string actionText);
        void CancelPlanItemAddition();
    }
}