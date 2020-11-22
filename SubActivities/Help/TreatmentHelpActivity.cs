using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Util;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity(Label = "TreatmentHelpActivity")]
    public class TreatmentHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:TreatmentHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _medicationContainer;
        private ImageView _medicationImage;
        private TextView _medicationText;
        private LinearLayout _structuredPlanContainer;
        private ImageView _structuredPlanImage;
        private TextView _structuredPlanText;
        private LinearLayout _problemSolvingContainer;
        private ImageView _problemSolvingImage;
        private TextView _problemSolvingText;
        private LinearLayout _affirmationsContainer;
        private ImageView _affirmationsImage;
        private TextView _affirmationsText;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TreatmentHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.treatmentHelpToolbar, Resource.String.TreatmentHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TreatmentPlanHelpMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    Finish();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.treatmentHelpActionHome:
                        Intent intent = new Intent(this, typeof(MainHelpActivity));
                       Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _medicationContainer = FindViewById<LinearLayout>(Resource.Id.linTreatmentPlanHelpMedication);
                _medicationImage = FindViewById<ImageView>(Resource.Id.imgTreatmentPlanHelpMedication);
                _medicationText = FindViewById<TextView>(Resource.Id.txtSafeyPlanHelpSuicide);
                _structuredPlanContainer = FindViewById<LinearLayout>(Resource.Id.linTreatmentPlanHelpStructuredPlan);
                _structuredPlanImage = FindViewById<ImageView>(Resource.Id.imgTreatmentPlanHelpStructuredPlan);
                _structuredPlanText = FindViewById<TextView>(Resource.Id.txtTreatmentPlanHelpStructuredPlan);
                _problemSolvingContainer = FindViewById<LinearLayout>(Resource.Id.linTreatmentPlanHelpProblemSolving);
                _problemSolvingImage = FindViewById<ImageView>(Resource.Id.imgTreatmentPlanHelpProblemSolving);
                _problemSolvingText = FindViewById<TextView>(Resource.Id.txtTreatmentPlanHelpProblemSolving);
                _affirmationsContainer = FindViewById<LinearLayout>(Resource.Id.linTreatmentPlanHelpAffirmations);
                _affirmationsImage = FindViewById<ImageView>(Resource.Id.imgTreatmentPlanHelpAffirmations);
                _affirmationsText = FindViewById<TextView>(Resource.Id.txtTreatmentPlanHelpAffirmations);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "TreatmentHelpActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_medicationContainer != null)
                    _medicationContainer.Click += MedicationContainer_Click;
                if (_medicationImage != null)
                    _medicationImage.Click += MedicationContainer_Click;
                if (_medicationText != null)
                    _medicationText.Click += MedicationContainer_Click;
                if (_structuredPlanContainer != null)
                    _structuredPlanContainer.Click += StructuredPlanContainer_Click;
                if (_structuredPlanImage != null)
                    _structuredPlanImage.Click += StructuredPlanContainer_Click;
                if (_structuredPlanText != null)
                    _structuredPlanText.Click += StructuredPlanContainer_Click;

                if (_problemSolvingContainer != null)
                    _problemSolvingContainer.Click += ProblemSolvingContainer_Click;
                if (_problemSolvingImage != null)
                    _problemSolvingImage.Click += ProblemSolvingContainer_Click;
                if (_problemSolvingText != null)
                    _problemSolvingText.Click += ProblemSolvingContainer_Click;
                if (_affirmationsContainer != null)
                    _affirmationsContainer.Click += AffirmationsContainer_Click;
                if (_affirmationsImage != null)
                    _affirmationsImage.Click += AffirmationsContainer_Click;
                if (_affirmationsText != null)
                    _affirmationsText.Click += AffirmationsContainer_Click;
                if (_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Setting up Callbacks", "TreatmentHelpActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void AffirmationsContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TreatmentAffirmationsHelpActivity));
            StartActivity(intent);
        }

        private void ProblemSolvingContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TreatmentProblemSolvingHelpActivity));
            StartActivity(intent);
        }

        private void StructuredPlanContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TreatmentStructuredPlanHelpActivity));
            StartActivity(intent);
        }

        private void MedicationContainer_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TreatmentMedicationHelpActivity));
            StartActivity(intent);
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHome = menu.FindItem(Resource.Id.treatmentHelpActionHome);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHome != null)
                            itemHome.SetIcon(Resource.Drawable.ic_home_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHome != null)
                            itemHome.SetIcon(Resource.Drawable.ic_home_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHome != null)
                            itemHome.SetIcon(Resource.Drawable.ic_home_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "TreatmentHelpActivity.SetActionIcons");
            }
        }
    }
}