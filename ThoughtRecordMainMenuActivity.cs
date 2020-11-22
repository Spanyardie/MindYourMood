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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace JazzyViewPager
{
    [Activity]
    public class ThoughtRecordMainMenuActivity : AppCompatActivity
    {
        private LinearLayout _thoughtEnter;
        private TextView _textEnter;

        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ThoughtRecordMainMenu);

            
            GetFieldCompnents();
            SetupCallbacks();
        }

        private void GetFieldCompnents()
        {
            _thoughtEnter = FindViewById<LinearLayout>(Resource.Id.linThoughtEnter);
            _textEnter = FindViewById<TextView>(Resource.Id.txtThoughtEnter);
        }

        private void SetupCallbacks()
        {
            if(_thoughtEnter != null)
                _thoughtEnter.Click += ThoughtEnter_Click;
            if (_textEnter != null)
                _textEnter.Click += ThoughtEnter_Click;
        }

        private void ThoughtEnter_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ThoughtRecordMainActivity));
            StartActivity(intent);
        }
    }
}