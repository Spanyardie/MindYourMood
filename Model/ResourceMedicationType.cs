using System.Collections.Generic;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood.Model
{
    public class ResourceMedicationType : ResourceMedicationTypeBase
    {
        private List<ResourceMedicationItem> _medicationItems;

        public List<ResourceMedicationItem> MedicationItems {
            get { return _medicationItems; }
            set { _medicationItems = value; }
        }

        public ResourceMedicationType()
        {
            ID = -1;
            MedicationTypeTitle = "";
            MedicationTypeDescription = "";

            _medicationItems = new List<ResourceMedicationItem>();

            IsNew = true;
            IsDirty = false;
        }
    }
}