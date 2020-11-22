namespace com.spanyardie.MindYourMood.Model.LowLevel
{
    public class ResourceMedicationItemBase
    {
        public int ID { get; set; }
        public int MedicationTypeID { get; set; }
        public string MedicationItemTitle { get; set; }
        public string MedicationItemDescription { get; set; }
        public string SideEffects { get; set; }
        public string Dosage { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
    }
}