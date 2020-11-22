using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Adapters;

namespace com.spanyardie.MindYourMood.Model.Interfaces
{
    interface IMusicPickerCallback
    {
        void ConfirmAddition(int playListID, List<Track> _tracks);
        void CancelAddition();
    }
}