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
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.StructuredPlan;

namespace com.spanyardie.MindYourMood.SubActivities.Help.Treatment
{
    [Activity()]
    public class TreatmentStructuredPlanHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:TreatmentStructuredPlanHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _feelingsContainer;
        private ImageView _feelingsImage;
        private TextView _feelingsText;
        private LinearLayout _healthContainer;
        private ImageView _healthImage;
        private TextView _healthText;
        private LinearLayout _reactionsContainer;
        private ImageView _reactionsImage;
        private TextView _reactionsText;
        private LinearLayout _fantasiesContainer;
        private ImageView _fantasiesImage;
        private TextView _fantasiesText;
        private LinearLayout _attitudesContainer;
        private ImageView _attitudesImage;
        private TextView _attitudesText;
        private LinearLayout _relationshipsContainer;
        private ImageView _relationshipsImage;
        private TextView _relationshipsText;

        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TreatmentStructuredPlanHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.treatmentStructuredPlanHelpToolbar, Resource.String.TreatmentStructuredPlanHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.StructuredPlanHelpMenu, menu);

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
                    case Resource.Id.treatmentStructuredPlanHelpActionHome:
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
                _feelingsContainer = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHelpFeelings);
                _feelingsImage = FindViewById<ImageView>(Resource.Id.imgStructuredPlanHelpFeelings);
                _feelingsText = FindViewById<TextView>(Resource.Id.txtStructuredPlanHelpFeelings);
                _healthContainer = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHelpHealth);
                _healthImage = FindViewById<ImageView>(Resource.Id.imgStructuredPlanHelpHealth);
                _healthText = FindViewById<TextView>(Resource.Id.txtStructuredPlanHelpHealth);
                _reactionsContainer = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHelpReactions);
                _reactionsImage = FindViewById<ImageView>(Resource.Id.imgStructuredPlanHelpReactions);
                _reactionsText = FindViewById<TextView>(Resource.Id.txtStructuredPlanHelpReactions);
                _fantasiesContainer = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHelpFantasies);
                _fantasiesImage = FindViewById<ImageView>(Resource.Id.imgStructuredPlanHelpFantasies);
                _fantasiesText = FindViewById<TextView>(Resource.Id.txtStructuredPlanHelpFantasies);
                _attitudesContainer = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHelpAttitudes);
                _attitudesImage = FindViewById<ImageView>(Resource.Id.imgStructuredPlanHelpAttitudes);
                _attitudesText = FindViewById<TextView>(Resource.Id.txtStructuredPlanHelpAttitudes);
                _relationshipsContainer = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHelpRelationships);
                _relationshipsImage = FindViewById<ImageView>(Resource.Id.imgStructuredPlanHelpRelationships);
                _relationshipsText = FindViewById<TextView>(Resource.Id.txtStructuredPlanHelpRelationships);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "TreatmentStructuredPlanHelpActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_feelingsContainer != null)
                    _feelingsContainer.Click += FeelingsContainer_Click;
                if (_feelingsImage != null)
                    _feelingsImage.Click += FeelingsContainer_Click;
                if (_feelingsText != null)
                    _feelingsText.Click += FeelingsContainer_Click;
                if (_healthContainer != null)
                    _healthContainer.Click += HealthContainer_Click;
                if (_healthImage != null)
                    _healthImage.Click += HealthContainer_Click;
                if (_healthText != null)
                    _healthText.Click += HealthContainer_Click;
                if (_reactionsContainer != null)
                    _reactionsContainer.Click += ReactionsContainer_Click;
                if (_reactionsImage != null)
                    _reactionsImage.Click += ReactionsContainer_Click;
                if (_reactionsText != null)
                    _reactionsText.Click += ReactionsContainer_Click;
                if (_fantasiesContainer != null)
                    _fantasiesContainer.Click += FantasiesContainer_Click;
                if (_fantasiesImage != null)
                    _fantasiesImage.Click += FantasiesContainer_Click;
                if (_fantasiesText != null)
                    _fantasiesText.Click += FantasiesContainer_Click;
                if (_attitudesContainer != null)
                    _attitudesContainer.Click += AttitudesContainer_Click;
                if (_attitudesImage != null)
                    _attitudesImage.Click += AttitudesContainer_Click;
                if (_attitudesText != null)
                    _attitudesText.Click += AttitudesContainer_Click;
                if (_relationshipsContainer != null)
                    _relationshipsContainer.Click += RelationshipsContainer_Click;
                if (_relationshipsImage != null)
                    _relationshipsImage.Click += RelationshipsContainer_Click;
                if (_relationshipsText != null)
                    _relationshipsText.Click += RelationshipsContainer_Click;
                if (_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up Callbacks", "TreatmentStructuredPlanHelpActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void RelationshipsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(StructuredPlanRelationshipsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "RelationshipsContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "TreatmentStructuredPlanHelpActivity.RelationshipsContainer_Click");
            }
        }

        private void AttitudesContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(StructuredPlanAttitudesHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AttitudesContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "TreatmentStructuredPlanHelpActivity.AttitudesContainer_Click");
            }
        }

        private void FantasiesContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(StructuredPlanFantasiesHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "FantasiesContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "TreatmentStructuredPlanHelpActivity.FantasiesContainer_Click");
            }
        }

        private void ReactionsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(StructuredPlanReactionsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "ReactionsContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "TreatmentStructuredPlanHelpActivity.ReactionsContainer_Click");
            }
        }

        private void HealthContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(StructuredPlanHealthHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "HealthContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "TreatmentStructuredPlanHelpActivity.HealthContainer_Click");
            }
        }

        private void FeelingsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(StructuredPlanFeelingsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "FeelingsContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "TreatmentStructuredPlanHelpActivity.FeelingsContainer_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHome = menu.FindItem(Resource.Id.treatmentStructuredPlanHelpActionHome);

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
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SafetyPlanHelpActivity.SetActionIcons");
            }
        }
    }
}