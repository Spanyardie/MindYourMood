namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IMoodsAdjustCallback
    {
        void ConfirmAddition(int moodID, string moodText);
        void CancelAddition();
    }
}