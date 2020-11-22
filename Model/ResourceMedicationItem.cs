using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class ResourceMedicationItem : ResourceMedicationItemBase
    {

        public ResourceMedicationItem()
        {
            ID = -1;
            MedicationTypeID = -1;
            MedicationItemTitle = "";
            MedicationItemDescription = "";
            SideEffects = "";
            Dosage = "";

            IsNew = true;
            IsDirty = false;
        }
    }
}