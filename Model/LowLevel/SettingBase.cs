namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class SettingBase
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}