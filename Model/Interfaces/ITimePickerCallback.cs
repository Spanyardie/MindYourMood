using com.spanyardie.MindYourMood.Helpers;
using System;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    public interface ITimePickerCallback
    {
        void TimePicked(DateTime timePicked, ConstantsAndTypes.TIMEPICKER_CONTEXT timeContext);
    }
}