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
using Android.Support.V4.View;
using Java.Lang;
using Android.Util;

namespace JazzyViewPager
{
    public class ThoughtRecordSituationPagerAdapter : PagerAdapter
    {
        private Context _context;
        private SituationView _currentView;

        public enum SituationView
        {
            SituationWhat = 0,
            SituationWho,
            SituationWhere,
            SituationWhen
        }

        public ThoughtRecordSituationPagerAdapter(Context context)
        {
            _context = context;
            //always start with What the situation is
            _currentView = SituationView.SituationWhat;
        }

        public override int Count
        {
            get
            {
                return 4;
            }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view == objectValue;
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {

            //get our View based on position
            View currentView = GetViewOnPosition((SituationView)position);
            //add the correct callbacks for this view
            SetupCallbacks(currentView, position);

            var containerView = container.JavaCast<ViewPager>();

            containerView.AddView(currentView);

            return currentView;
        }

        private void SetupCallbacks(View currentView, int position)
        {
            EditText textInput = null;
           
            if(currentView != null)
            {
                var speakButton = GetSpeakView(currentView, position);
                textInput = GetTextView(currentView, position);

                if(speakButton != null)
                    speakButton.Click += SpeakButton_Click;

                if (textInput != null)
                {
                    textInput.TextChanged += TextInput_TextChanged;
                    textInput.Tag = _currentView.ToString();
                }
            }

        }

        private void TextInput_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // 
            var currentView = ((EditText)sender).Tag;

            Log.Info("ThoughtRecordSituationPagerAdapter.TextInput_Changed", "Text is now - " + e.Text.ToString());
        }

        private void SpeakButton_Click(object sender, EventArgs e)
        {
            //Initiate speak
        }

        private View GetViewOnPosition(SituationView position)
        {
            View view = null;

            switch(position)
            {
                case SituationView.SituationWhat:
                    view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.SituationStepWhatView, null);
                    _currentView = SituationView.SituationWhat;
                    break;

                case SituationView.SituationWho:
                    view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.SituationStepWhoView, null);
                    _currentView = SituationView.SituationWho;
                    break;

                case SituationView.SituationWhere:
                    view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.SituationStepWhereView, null);
                    _currentView = SituationView.SituationWhere;
                    break;

                case SituationView.SituationWhen:
                    view = ((Activity)_context).LayoutInflater.Inflate(Resource.Layout.SituationStepWhenView, null);
                    _currentView = SituationView.SituationWhen;
                    break;
            }

            return view;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object objectValue)
        {
            var viewPager = container.JavaCast<ViewPager>();
            viewPager.RemoveView(objectValue as View);
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            Java.Lang.String title = new Java.Lang.String("");
            SituationView view = (SituationView)position;

            switch(view)
            {
                case SituationView.SituationWhat:
                    title = new Java.Lang.String("1. What is the Situation");
                    break;
                case SituationView.SituationWho:
                    title = new Java.Lang.String("2. Who were you with");
                    break;
                case SituationView.SituationWhere:
                    title = new Java.Lang.String("3. Where were you");
                    break;
                case SituationView.SituationWhen:
                    title = new Java.Lang.String("4. When did it happen");
                    break;
                default:
                    break;
            }

            return title;
        }

        private ImageButton GetSpeakView(View currentView, int position)
        {
            ImageButton speakView = null;

            SituationView view = (SituationView)position;

            if (currentView != null)
            {
                switch (view)
                {
                    case SituationView.SituationWhat:
                        speakView = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhat);
                        break;
                    case SituationView.SituationWhen:
                        speakView = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhen);
                        break;
                    case SituationView.SituationWhere:
                        speakView = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhere);
                        break;
                    case SituationView.SituationWho:
                        speakView = currentView.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWho);
                        break;
                }
            }
            return speakView;
        }

        private EditText GetTextView(View currentView, int position)
        {
            EditText speakView = null;

            SituationView view = (SituationView)position;

            if (currentView != null)
            {
                switch (view)
                {
                    case SituationView.SituationWhat:
                        speakView = currentView.FindViewById<EditText>(Resource.Id.edtSituationItemWhat);
                        break;
                    case SituationView.SituationWhen:
                        speakView = currentView.FindViewById<EditText>(Resource.Id.edtSituationItemWhen);
                        break;
                    case SituationView.SituationWhere:
                        speakView = currentView.FindViewById<EditText>(Resource.Id.edtSituationItemWhere);
                        break;
                    case SituationView.SituationWho:
                        speakView = currentView.FindViewById<EditText>(Resource.Id.edtSituationItemWho);
                        break;
                }
            }
            return speakView;
        }
    }
}