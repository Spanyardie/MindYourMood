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
using com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan.SafetyPlanItems;

namespace com.spanyardie.MindYourMood.SubActivities.Help
{
    [Activity()]
    public class SafetyPlanHelpActivity : AppCompatActivity
    {
        public static string TAG = "M:SafetyPlanHelpActivity";

        private Toolbar _toolbar;

        private LinearLayout _suicideContainer;
        private ImageView _suicideImage;
        private TextView _suicideText;
        private LinearLayout _warningSignsContainer;
        private ImageView _warningSignsImage;
        private TextView _warningSignsText;
        private LinearLayout _copingMethodsContainer;
        private ImageView _copingMethodsImage;
        private TextView _copingMethodsText;
        private LinearLayout _keepCalmContainer;
        private ImageView _keepCalmImage;
        private TextView _keepCalmText;
        private LinearLayout _tellMyselfContainer;
        private ImageView _tellMyselfImage;
        private TextView _tellMyselfText;
        private LinearLayout _othersDoContainer;
        private ImageView _othersDoImage;
        private TextView _othersDoText;
        private LinearLayout _contactContainer;
        private ImageView _contactImage;
        private TextView _contactText;
        private LinearLayout _safePlacesContainer;
        private ImageView _safePlacesImage;
        private TextView _safePlacesText;
        private Button _done;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SafetyPlanHelpLayout);

            _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyPlanHelpToolbar, Resource.String.SafetyPlanHelpTitle, Color.White);

            GetFieldComponents();
            SetupCallbacks();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SafetyPlanHelpMenu, menu);

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
                    case Resource.Id.safetyPlanHelpActionHome:
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
                _suicideContainer = FindViewById<LinearLayout>(Resource.Id.linSafeyPlanHelpSuicidalThoughts);
                _suicideImage = FindViewById<ImageView>(Resource.Id.imgSafeyPlanHelpSuicide);
                _suicideText = FindViewById<TextView>(Resource.Id.txtSafeyPlanHelpSuicide);
                _warningSignsContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpWarningSigns);
                _warningSignsImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpWarningSigns);
                _warningSignsText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpWarningSigns);
                _copingMethodsContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpCopingMethods);
                _copingMethodsImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpCopingMethods);
                _copingMethodsText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpCopingMethods);
                _keepCalmContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpKeepCalm);
                _keepCalmImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpKeepCalm);
                _keepCalmText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpKeepCalm);
                _tellMyselfContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpTellMyself);
                _tellMyselfImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpTellMyself);
                _tellMyselfText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpTellMyself);
                _othersDoContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpOthersDo);
                _othersDoImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpOthersDo);
                _othersDoText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpOthersDo);
                _contactContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpEmergencyContact);
                _contactImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpContacts);
                _contactText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpContacts);
                _safePlacesContainer = FindViewById<LinearLayout>(Resource.Id.linSafetyPlanHelpSafePlaces);
                _safePlacesImage = FindViewById<ImageView>(Resource.Id.imgSafetyPlanHelpSafePlaces);
                _safePlacesText = FindViewById<TextView>(Resource.Id.txtSafetyPlanHelpSafePlaces);
                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "SafetyPlanHelpActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_suicideContainer != null)
                    _suicideContainer.Click += SuicideContainer_Click;
                if (_suicideImage != null)
                    _suicideImage.Click += SuicideContainer_Click;
                if (_suicideText != null)
                    _suicideText.Click += SuicideContainer_Click;
                if (_warningSignsContainer != null)
                    _warningSignsContainer.Click += WarningSignsContainer_Click;
                if (_warningSignsImage != null)
                    _warningSignsImage.Click += WarningSignsContainer_Click;
                if (_warningSignsText != null)
                    _warningSignsText.Click += WarningSignsContainer_Click;

                if (_copingMethodsContainer != null)
                    _copingMethodsContainer.Click += CopingMethodsContainer_Click;
                if (_copingMethodsImage != null)
                    _copingMethodsImage.Click += CopingMethodsContainer_Click;
                if (_copingMethodsText != null)
                    _copingMethodsText.Click += CopingMethodsContainer_Click;
                if (_keepCalmContainer != null)
                    _keepCalmContainer.Click += KeepCalmContainer_Click;
                if (_keepCalmImage != null)
                    _keepCalmImage.Click += KeepCalmContainer_Click;
                if (_keepCalmText != null)
                    _keepCalmText.Click += KeepCalmContainer_Click;
                if (_tellMyselfContainer != null)
                    _tellMyselfContainer.Click += TellMyselfContainer_Click;
                if (_tellMyselfImage != null)
                    _tellMyselfImage.Click += TellMyselfContainer_Click;
                if (_tellMyselfText != null)
                    _tellMyselfText.Click += TellMyselfContainer_Click;
                if (_othersDoContainer != null)
                    _othersDoContainer.Click += OthersDoContainer_Click;
                if (_othersDoImage != null)
                    _othersDoImage.Click += OthersDoContainer_Click;
                if (_othersDoText != null)
                    _othersDoText.Click += OthersDoContainer_Click;
                if (_contactContainer != null)
                    _contactContainer.Click += ContactContainer_Click;
                if (_contactImage != null)
                    _contactImage.Click += ContactContainer_Click;
                if (_contactText != null)
                    _contactText.Click += ContactContainer_Click;
                if (_safePlacesContainer != null)
                    _safePlacesContainer.Click += SafePlacesContainer_Click;
                if (_safePlacesImage != null)
                    _safePlacesImage.Click += SafePlacesContainer_Click;
                if (_safePlacesText != null)
                    _safePlacesText.Click += SafePlacesContainer_Click;
                if (_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up Callbacks", "SafetyPlanHelpActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SafePlacesContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanSafePlacesHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SafePlacesContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Safe Places help", "SafetyPlanHelpActivity.SafePlacesContainer_Click");
            }
        }

        private void ContactContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanContactHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "ContactContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Contact help", "SafetyPlanHelpActivity.ContactContainer_Click");
            }
        }

        private void OthersDoContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanOthersDoHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "OthersDoContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Others Do help", "SafetyPlanHelpActivity.OthersDoContainer_Click");
            }
        }

        private void TellMyselfContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanTellMyselfHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "TellMyselfContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Tell Myself help", "SafetyPlanHelpActivity.TellMyselfContainer_Click");
            }
        }

        private void KeepCalmContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanKeepCalmHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "KeepCalmContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Keep Calm help", "SafetyPlanHelpActivity.KeepCalmContainer_Click");
            }
        }

        private void CopingMethodsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanCopingMethodsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "CopingMethodsContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Coping Methods help", "SafetyPlanHelpActivity.CopingMethodsContainer_Click");
            }
        }

        private void WarningSignsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanWarningSignsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "WarningSignsContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Warning Signs help", "SafetyPlanHelpActivity.WarningSignsContainer_Click");
            }
        }

        private void SuicideContainer_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(this, typeof(SafetyPlanSuicidalThoughtsHelpActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SuicideContainer_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Showing Suicide help", "SafetyPlanHelpActivity.SuicideContainer_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHome = menu.FindItem(Resource.Id.safetyPlanHelpActionHome);

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