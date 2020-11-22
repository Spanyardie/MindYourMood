using System;

using Android.App;
using Android.OS;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Content;
using Android.Views;
using com.spanyardie.MindYourMood.Model;
using Android.Runtime;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment;
using System.Collections.Generic;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class MedicationActivity : AppCompatActivity, IMedication, IAlertCallback
    {
        private Toolbar _toolbar;

        public const string TAG = "M:MedicationActivity";

        private ListView _medicationList;

        private Button _btnDone;

        private LinearLayout _medListMain;
        
        private int _selectedItemIndex = -1;

        private ImageLoader _imageLoader = null;

        public override void OnSaveInstanceState(Bundle outState, PersistableBundle outPersistentState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
            }

            base.OnSaveInstanceState(outState, outPersistentState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MedicationMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.MedicationLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.medicationToolbar, Resource.String.MedicationActionBarTitle, Color.White);

                Log.Info(TAG, "OnCreate: Setup Action bar");

                GetFieldComponents();
                Log.Info(TAG, "OnCreate: Retrieved Field Components");

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.medication,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();
                Log.Info(TAG, "OnCreate: Set up Callbacks");

                UpdateAdapter();
                Log.Info(TAG, "OnCreate: Updated adapter");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedActivityCreate), "MedicationActivity.OnCreate"); 
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_medListMain != null)
                _medListMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        private void GetFieldComponents()
        {
            try
            {
                _medicationList = FindViewById<ListView>(Resource.Id.lstMedList);
                Log.Info(TAG, "GetFieldComponents: Successfully retrieved field components");
                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
                _medListMain = FindViewById<LinearLayout>(Resource.Id.linMedListMain);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedActivityGetComponents), "MedicationActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_medicationList != null)
                {
                    _medicationList.ItemClick += MedicationList_ItemClick;
                    _medicationList.ItemLongClick += MedicationList_ItemLongClick;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _medicationList is NULL!");
                }
                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
                Log.Info(TAG, "SetupCallbacks: Succeeded in setting up callbacks");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedActivitySetupCallbacks), "MedicationActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.MedicationRemoveTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.MedicationRemoveText);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonOKCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    Toast.MakeText(this, Resource.String.MedicationDeleteSelectItem, ToastLength.Short).Show();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Removing Medication", "MedicationActivity.Remove");
            }
        }

        private void MedicationList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _medicationList.SetSelection(_selectedItemIndex);
                var medID = GlobalData.MedicationItems[_selectedItemIndex].ID;

                Intent intent = new Intent(this, typeof(MedicationMainActivity));
                intent.PutExtra("title", "Edit Medication");
                intent.PutExtra("isNew", false);
                intent.PutExtra("medicationID", medID);

                StartActivityForResult(intent, ConstantsAndTypes.EDIT_MEDICATION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "MedicationList_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedActivityMedListLongClick), "MedicationActivity.MedicationList_ItemLongClick");
            }
        }

        private void MedicationList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            _medicationList.SetSelection(_selectedItemIndex);
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.medicationActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.medicationActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.medicationActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorSettingActionIcons), "MedicationActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Log.Info(TAG, "GoBack: Navigating back to Treatment Plan");
            Intent intent = new Intent(this, typeof(TreatmentPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            Finish();
        }

        private void Add()
        {
            try
            {
                Intent intent = new Intent(this, typeof(MedicationMainActivity));
                intent.PutExtra("title", "Add Medication");
                intent.PutExtra("isNew", true);
                intent.PutExtra("medicationID", -1);

                StartActivityForResult(intent, ConstantsAndTypes.ADD_MEDICATION_REQUEST);

            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedActivityAddMed), "MedicationActivity.Add");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == ConstantsAndTypes.ADD_MEDICATION_REQUEST || requestCode == ConstantsAndTypes.EDIT_MEDICATION_REQUEST)
            {
                switch (resultCode)
                {
                    case Result.Ok:
                        ConfirmMedicationAdd();
                        break;
                    case Result.Canceled:
                        CancelledMedicationAdd();
                        break;
                }
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                if (_medicationList != null)
                {
                    MedicationListAdapter adapter = new MedicationListAdapter(this);
                    _medicationList.Adapter = adapter;
                    Log.Info(TAG, "UpdateAdapter: Updated medication List adapter");
                }
                else
                {
                    Log.Error(TAG, "UpdateAdapter: _medicationList is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedActivityUpdateAdapter), "MedicationActivity.UpdateAdapter");
            }
        }

        public void ConfirmMedicationAdd()
        {
            Log.Info(TAG, "ConfirmMedicationAdd: Updating adapter");
            UpdateAdapter();
            Toast.MakeText(this, Resource.String.MedActivityConfirmMedAddToast, ToastLength.Short).Show();
        }

        public void CancelledMedicationAdd()
        {
            Log.Info(TAG, "CancelledMedicationAdd: Received Cancel notification");
            UpdateAdapter();
            Toast.MakeText(this, Resource.String.MedActivityCancelAddToast, ToastLength.Short).Show();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    GoBack();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.medicationActionAdd:
                        Add();
                        return true;

                    case Resource.Id.medicationActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.medicationActionHelp:
                        Intent intent = new Intent(this, typeof(TreatmentMedicationHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                Medication med = GlobalData.MedicationItems[_selectedItemIndex];
                if (med != null)
                {
                    CancelAlarms(med.MedicationSpread);
                    med.Remove();
                    GlobalData.MedicationItems.Remove(med);
                    UpdateAdapter();
                    _selectedItemIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorMedicationRemove), "MedicationActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {

        }

        private void CancelAlarms(List<MedicationSpread> reminders)
        {
            if (reminders == null) return;

            AlarmHelper alarmHelper = new AlarmHelper(this);
            try
            {
                foreach(var med in reminders)
                {
                    if(med.MedicationTakeReminder != null)
                    {
                        if(med.MedicationTakeReminder.IsSet)
                        {
                            alarmHelper.CancelAlarm(med.MedicationTakeReminder.ID);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CancelAlarms: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMedicationRemove), "MedicationActivity.CancelAlarms");
            }
        }
    }
}