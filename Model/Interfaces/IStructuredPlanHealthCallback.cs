using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IStructuredPlanHealthCallback
    {
        void ConfirmPlanItemAddition(int healthID, string aspect, int importance, ConstantsAndTypes.REACTION_TYPE reaction, ConstantsAndTypes.ACTION_TYPE action, string actionText);
        void CancelPlanItemAddition();
    }
}