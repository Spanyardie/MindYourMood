using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class ResourceCondition : ResourceConditionBase
    {
        public ResourceCondition()
        {
            ConditionId = -1;
            ConditionTitle = "";
            ConditionDescription = "";
            ConditionCitation = "";
        }
    }
}