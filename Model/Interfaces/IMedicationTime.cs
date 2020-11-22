using com.spanyardie.MindYourMood.Helpers;
using System;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IMedicationTime
    {
        void MedicationTimeAdded(int dose, ConstantsAndTypes.MEDICATION_FOOD medFood, ConstantsAndTypes.MEDICATION_TIME medTime, DateTime taken);
    }
}