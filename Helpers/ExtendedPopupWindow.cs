using Android.Widget;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ExtendedPopupWindow : PopupWindow
    {
        public bool RequiresParentUpdate { get; set; }

        public ExtendedPopupWindow()
        {
            RequiresParentUpdate = false;
        }
    }
}