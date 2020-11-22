using com.spanyardie.MindYourMood.Helpers;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IStructuredPlanRelationshipsCallback
    {
        void ConfirmPlanItemAddition(int attitudesID, string withWhom, ConstantsAndTypes.RELATIONSHIP_TYPE type, int strength, int feeling, ConstantsAndTypes.ACTION_TYPE action, string actionText);
        void CancelPlanItemAddition();
    }
}