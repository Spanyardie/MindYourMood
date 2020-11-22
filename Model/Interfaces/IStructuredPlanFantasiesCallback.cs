using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IStructuredPlanFantasiesCallback
    {
        void ConfirmPlanItemAddition(int fantasiesID, string ofWhat, int strength, ConstantsAndTypes.REACTION_TYPE reaction, ConstantsAndTypes.ACTION_TYPE action, string actionText);
        void CancelPlanItemAddition();
    }
}