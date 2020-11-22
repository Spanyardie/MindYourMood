using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.spanyardie.MindYourMood.Adapters;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Content;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class SafetyPlanCardsActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:SafetyPlanCardsActivity";

        private Toolbar _toolbar;
        private StackView _safetyCardStack;
        private SafetyPlanCardDialogFragment _safetyDialog;
        private Button _done;

        private int _selectedItemIndex;

        private ImageLoader _imageLoader = null;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemIndex);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SafetyPlanCardsMenu, menu);

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

                SetContentView(Resource.Layout.SafetyPlanCardsLayout);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyPlanCardsToolbar, Resource.String.SafetyPlanCardsActionBarTitle, Color.White);

                GetFieldComponents();
                CheckMicPermission();

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.calmcardsmain,
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

                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Safety Plan Cards Activity", "SafetyPlanCardsActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_safetyCardStack != null)
                _safetyCardStack.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            if (_safetyCardStack != null)
            {
                _safetyCardStack.ItemClick += SafetyCardStack_ItemClick;
            }
            if(_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.safetyplancardsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.safetyplancardsActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.safetyplancardsActionHelp);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "SafetyPlanCardsActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(SafetyActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void SafetyCardStack_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;

                UpdateAdapter();
                var adapter = _safetyCardStack.Adapter;

                //forces a redraw
                ((BaseAdapter)adapter).NotifyDataSetChanged();

                _safetyCardStack.DisplayedChild = _selectedItemIndex;

                Log.Info(TAG, "SafetyCardStack_ItemClick: Selected Item index - " + _selectedItemIndex.ToString());
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "SafetyCardStack_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Selecting a Safety Plan Card", "SafetyPlanCardsActivity.SafetyCardStack_ItemClick");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        private void Remove()
        {
            if (GlobalData.SafetyPlanCardsItems != null)
            {
                if(GlobalData.SafetyPlanCardsItems.Count == 0) return;
            }
            else
            {
                GlobalData.SafetyPlanCardsItems = new List<SafetyPlanCard>();
                return;
            }

            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.AlertTitle = GetString(Resource.String.safetyPlanCardRemoveTitle);
                alertHelper.AlertMessage = GetString(Resource.String.safetyPlanCardRemoveQuestion);
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertPositiveCaption = GetString(Resource.String.safetyPlanCardRemoveConfirm);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.safetyPlanCardRemoveCancel);
                alertHelper.InstanceId = "remove";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Remove: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Removing Safety Plan card", "SafetyPlanCardsActivity.Remove");
            }
        }

        private void Add()
        {
            try
            {
                _safetyDialog = new SafetyPlanCardDialogFragment(this, "Add Safety Plan Card");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _safetyDialog.Show(fragmentTransaction, _safetyDialog.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Adding a Safety Plan Card", "SafetyPlanCardsActivity.Add_Click");
            }
        }

        private void GetFieldComponents()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.safetyPlanToolbar);
            _safetyCardStack = FindViewById<StackView>(Resource.Id.stkCalmCards);
            _done = FindViewById<Button>(Resource.Id.btnDone);
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
                    case Resource.Id.safetyplancardsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.safetyplancardsActionRemove:
                        Remove();
                        return true;
                    case Resource.Id.safetyplancardsActionHelp:
                        Intent intent = new Intent(this, typeof(SafetyPlanCardsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void ConfirmCard(string calmMyself, string tellMyself, string willCall, string willGoTo)
        {
            Log.Info(TAG, "ConfirmCard: Called with '" + calmMyself + "', '" + tellMyself + "', '" + willCall + "', '" + willGoTo + "'");
            try
            {
                SafetyPlanCard safetyCard = new SafetyPlanCard();
                safetyCard.CalmMyself = calmMyself;
                safetyCard.TellMyself = tellMyself;
                safetyCard.WillCall = willCall;
                safetyCard.WillGoTo = willGoTo;

                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                Log.Info(TAG, "ConfirmCard: Attempting Save to database...");
                safetyCard.Save(sqlDatabase);
                dbHelp.CloseDatabase();
                GlobalData.SafetyPlanCardsItems.Add(safetyCard);
                UpdateAdapter();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ConfirmCard: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Adding a Safety Plan Card", "SafetyPlanCardsActivity.ConfirmCard");
            }
        }

        public void CancelCard()
        {
            Toast.MakeText(this, Resource.String.CalmCardCancelledToast, ToastLength.Short).Show();
        }

        private void UpdateAdapter()
        {
            try
            {
                SafetyPlanCardsStackAdapter adapter = new SafetyPlanCardsStackAdapter(this);
                if (_safetyCardStack != null)
                {
                    _safetyCardStack.Adapter = adapter;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Updating Safety Plan Card Display", "SafetyPlanCardsActivity.UpdateAdapter");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
            }

            if (instanceId == "remove")
            {
                try
                {
                    Globals dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    var sqlDatabase = dbHelp.GetSQLiteDatabase();
                    if (sqlDatabase.IsOpen && _selectedItemIndex != -1)
                    {
                        GlobalData.SafetyPlanCardsItems[_selectedItemIndex].Remove(sqlDatabase);
                    }
                    dbHelp.CloseDatabase();
                    if (_selectedItemIndex != -1)
                        GlobalData.SafetyPlanCardsItems.RemoveAt(_selectedItemIndex);
                    UpdateAdapter();
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing a Safety Plan Card", "SafetyPlanCardsActivity.AlertPositiveButtonSelect");
                }
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                return;
            }
        }

        private void CheckMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    AttemptPermissionRequest();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "SafetyPlanCardsActivity.CheckMicPermission");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_USE_MICROPHONE)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //now update the global permission
                        if (GlobalData.ApplicationPermissions == null)
                        {
                            //if null then we can go get permissions
                            PermissionsHelper.SetupDefaultPermissionList(this);
                        }
                        else
                        {
                            //we need to update the existing permission
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.UseMicrophone).PermissionGranted = Permission.Granted;
                            }
                        }
                        //PermissionResultUpdate(Permission.Granted);
                    }
                    else
                    {
                        //PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "SafetyPlanCardsActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {

            }
        }

        private void ShowPermissionRationale()
        {
            try
            {
                if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic").SettingValue == "True") return;

                AlertHelper alertHelper = new AlertHelper(this);

                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
                alertHelper.AlertMessage = GetString(Resource.String.RequestPermissionUseMicrophoneAlertMessage);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertTitle = GetString(Resource.String.RequestPermissionUseMicrophoneAlertTitle);
                alertHelper.InstanceId = "useMic";
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "SafetyPlanCardsActivity.ShowPermissionRationale");
            }
        }

        public void AttemptPermissionRequest()
        {
            try
            {
                if (PermissionsHelper.ShouldShowPermissionRationale(this, ConstantsAndTypes.AppPermission.UseMicrophone))
                {
                    ShowPermissionRationale();
                    return;
                }
                else
                {
                    //just request the permission
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AttemptPermissionRequest: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "SafetyPlanCardsActivity.AttemptPermissionRequest");
            }
        }
    }
}