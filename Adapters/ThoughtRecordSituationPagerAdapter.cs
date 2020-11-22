using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Java.Lang;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;

namespace com.spanyardie.MindYourMood
{
    public class ThoughtRecordSituationPagerAdapter : PagerAdapter
    {
        private Context _context;
        private SituationView _currentView;

        private ImageButton _speakSituationItem;
        private bool _speakPermission = false;

        private string _errorMessage = "";
        private SituationView _itemView = SituationView.SituationWhat;

        public enum SituationView
        {
            SituationWhat = 0,
            SituationWho,
            SituationWhere,
            SituationWhen
        }

        public ThoughtRecordSituationPagerAdapter(Context context, bool speakPermission, string errorMessage = "", SituationView itemView = SituationView.SituationWhat)
        {
            _context = context;
            //always start with What the situation is
            _currentView = SituationView.SituationWhat;
            _errorMessage = errorMessage.Trim();
            _speakPermission = speakPermission;
            _itemView = itemView;
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

            GetFieldComponents(currentView, position);

            if(_errorMessage.Trim() != "")
            {
                if (_itemView == (SituationView)position)
                {
                    var textItem = GetTextView(currentView, position);
                    if (textItem != null)
                        textItem.Error = _errorMessage.Trim();
                }
            }

            if(_speakSituationItem != null)
                _speakSituationItem.Enabled = _speakPermission;

            //add the correct callbacks for this view
            SetupCallbacks(currentView, position);

            AssignTexts(currentView, position);

            var containerView = container.JavaCast<ViewPager>();

            containerView.AddView(currentView);

            return currentView;
        }

        private void GetFieldComponents(View view, int position)
        {
            if (view != null)
            {
                switch ((SituationView)position)
                {
                    case SituationView.SituationWhat:
                        _speakSituationItem = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhat);
                        break;
                    case SituationView.SituationWhen:
                        _speakSituationItem = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhen);
                        break;
                    case SituationView.SituationWhere:
                        _speakSituationItem = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWhere);
                        break;
                    case SituationView.SituationWho:
                        _speakSituationItem = view.FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWho);
                        break;
                }
            }
        }

        private void AssignTexts(View currentView, int position)
        {
            EditText textView = GetTextView(currentView, position);

            if (currentView != null && textView != null)
            {
                switch((SituationView)position)
                {
                    case SituationView.SituationWhat:
                        textView.Text = GlobalData.SituationItem.What.Trim();
                        break;
                    case SituationView.SituationWhen:
                        textView.Text = GlobalData.SituationItem.When.Trim();
                        break;
                    case SituationView.SituationWhere:
                        textView.Text = GlobalData.SituationItem.Where.Trim();
                        break;
                    case SituationView.SituationWho:
                        textView.Text = GlobalData.SituationItem.Who.Trim();
                        break;
                }
            }
        }

        private void SetupCallbacks(View currentView, int position)
        {
            EditText textInput = null;
           
            if(currentView != null)
            {
                var speakButton = GetSpeakView(currentView, position);

                textInput = GetTextView(currentView, position);

                if (speakButton != null)
                {
                    speakButton.Click += SpeakButton_Click;
                    speakButton.Tag = (int)_currentView;
                }

                if (textInput != null)
                {
                    textInput.TextChanged += TextInput_TextChanged;
                    textInput.Tag = (int)_currentView;
                }
            }

        }

        private void TextInput_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // 
            var currentView = Convert.ToInt32(((EditText)sender).Tag);
            SituationView thisView = (SituationView)Convert.ToInt32(currentView);

            EditText editText = (EditText)sender;

            if (editText != null)
            {
                switch (thisView)
                {
                    case SituationView.SituationWhat:
                        GlobalData.SituationItem.What = editText.Text.Trim();
                        break;
                    case SituationView.SituationWhen:
                        GlobalData.SituationItem.When = editText.Text.Trim();
                        break;
                    case SituationView.SituationWhere:
                        GlobalData.SituationItem.Where = editText.Text.Trim();
                        break;
                    case SituationView.SituationWho:
                        GlobalData.SituationItem.Who = editText.Text.Trim();
                        break;
                }
                if (editText.Text.Trim() != "")
                    editText.Error = null;
            }
            Log.Info("ThoughtRecordSituationPagerAdapter.TextInput_Changed", "Text is now - " + e.Text.ToString());
        }

        
        private void SpeakButton_Click(object sender, EventArgs e)
        {
            var button = (ImageButton)sender;

            if (button != null)
            {
                int viewIndex = (int)button.Tag;
                if (_context != null)
                {
                    ((ISituationSpeakCallback)_context).SpeakSituationItem(viewIndex);
                }
            }
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
                    title = new Java.Lang.String(((Activity)_context).GetString(Resource.String.situationQuestion1));
                    break;
                case SituationView.SituationWho:
                    title = new Java.Lang.String(((Activity)_context).GetString(Resource.String.situationQuestion2));
                    break;
                case SituationView.SituationWhere:
                    title = new Java.Lang.String(((Activity)_context).GetString(Resource.String.situationQuestion3));
                    break;
                case SituationView.SituationWhen:
                    title = new Java.Lang.String(((Activity)_context).GetString(Resource.String.situationQuestion4));
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

        private void HandleMicPermission()
        {
            if (!(PermissionsHelper.HasPermission(_context, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(_context, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                if (_speakSituationItem != null)
                {
                    _speakSituationItem.Enabled = false;
                }
            }
        }
    }
}