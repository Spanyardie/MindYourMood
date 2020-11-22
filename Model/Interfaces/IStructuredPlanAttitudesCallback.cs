using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IStructuredPlanAttitudesCallback
    {
        void ConfirmPlanItemAddition(int attitudesID, string toWhat, ConstantsAndTypes.ATTITUDE_TYPES type, int belief, int feeling, ConstantsAndTypes.ACTION_TYPE action, string actionText);
        void CancelPlanItemAddition();
    }
}