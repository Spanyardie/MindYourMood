namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ResourceMedicationTypeBase
    {
        public int ID { get; set; }
        public string MedicationTypeTitle { get; set; }
        public string MedicationTypeDescription { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}