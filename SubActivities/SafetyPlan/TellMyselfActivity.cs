using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Adapters;
using Android.Media;
using Java.IO;
using Android.Content;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.SafetyPlan.SafetyPlanItems;
using Android.Content.PM;
using Android.Runtime;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.SafetyPlan
{
    [Activity]
    public class TellMyselfActivity : AppCompatActivity, IGenericTextCallback, IVoiceRecordCallback, IAlertCallback
    {
        public static string TAG = "M:TellMyselfActivity";

        private Toolbar _toolbar;
        private IMenu _menu;

        public VoiceRecordingDialogFragment _voiceRecordingSelector;
        public GenericTextDialogFragment _genericTextFragment;
        private Button _done;

        private ListView _listTellMyself;


        private MediaPlayer _mediaPlayer;
        private bool _isPlaying;
        private int _selectedItemIndex;

        private ImageLoader _imageLoader = null;

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }
        }

        public TellMyselfActivity()
        {
            _selectedItemIndex = -1;
            _mediaPlayer = new MediaPlayer();
            _isPlaying = false;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if(outState != null)
            {
                outState.PutInt("selectedItemIndex", _selectedItemIndex);
                outState.PutBoolean("isPlaying", _isPlaying);
            }

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.TellMyselfMenu, menu);

            SetActionIcons(menu);

            _menu = menu;

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if(savedInstanceState != null)
                {
                    _selectedItemIndex = savedInstanceState.GetInt("selectedItemIndex");
                    _isPlaying = savedInstanceState.GetBoolean("isPlaying");
                }

                SetContentView(Resource.Layout.TellMyselfLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.tellMyselfToolbar, Resource.String.safetyPlanThingsTellMyselfActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.tellmyselfmain,
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

                if (_selectedItemIndex != -1)
                    _listTellMyself.SetSelection(_selectedItemIndex);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Error occurred during creation - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreatingTellMyselfActivity), "TellMyselfActivity.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_listTellMyself != null)
                _listTellMyself.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            if (_listTellMyself != null)
            {
                _listTellMyself.ItemClick += ListTellMyself_ItemClick;
                _listTellMyself.ItemLongClick += ListTellMyself_ItemLongClick;
            }

            if(_mediaPlayer != null)
                _mediaPlayer.Completion += MediaPlayer_Completion;

            if(_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void ListTellMyself_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            try
            {
                var adapter = _listTellMyself.Adapter;
                var selectedIndexID = (int)adapter.GetItemId(e.Position);
                var genericTextItem = GlobalData.TellMyselfItemsList.Find(gen => gen.ID == selectedIndexID);

                //we don't want to edit a recording really
                if (genericTextItem.TellType == ConstantsAndTypes.TELL_TYPE.Audio)
                {
                    Toast.MakeText(this, Resource.String.TellMyselfAudioNoEditText, ToastLength.Short).Show();
                    return;
                }

                GenericTextDialogFragment genFragment = new GenericTextDialogFragment(this, "Tell Yourself...", GetString(Resource.String.TellMyselfGenericTextTitle), genericTextItem.TellText, selectedIndexID);

                FragmentTransaction transaction = FragmentManager.BeginTransaction();

                genFragment.Show(transaction, genFragment.Tag);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ListTellMyself_ItemLongClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorTellMyselfEditEntry), "TellMyselfActivity.ListTellMyself_ItemLongClick");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAddText = menu.FindItem(Resource.Id.tellmyselfActionAddText);
                var itemAddVoice = menu.FindItem(Resource.Id.tellmyselfActionAddVoice);
                var itemRemove = menu.FindItem(Resource.Id.tellmyselfActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.tellmyselfActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAddText != null)
                            itemAddText.SetIcon(Resource.Drawable.ic_text_fields_white_24dp);
                        if(itemAddVoice != null)
                            itemAddVoice.SetIcon(Resource.Drawable.ic_keyboard_voice_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAddText != null)
                            itemAddText.SetIcon(Resource.Drawable.ic_text_fields_white_36dp);
                        if (itemAddVoice != null)
                            itemAddVoice.SetIcon(Resource.Drawable.ic_keyboard_voice_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAddText != null)
                            itemAddText.SetIcon(Resource.Drawable.ic_text_fields_white_48dp);
                        if (itemAddVoice != null)
                            itemAddVoice.SetIcon(Resource.Drawable.ic_keyboard_voice_white_48dp);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "TellMyselfActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(SafetyPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void Remove()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    var resourceString = "";
                    AlertHelper alertHelper = new AlertHelper(this);
                    if (GlobalData.TellMyselfItemsList[_selectedItemIndex].TellType == ConstantsAndTypes.TELL_TYPE.Audio)
                    {
                        resourceString = GetString(Resource.String.tellMyselfRemoveTitleAudio);
                        alertHelper.AlertTitle = resourceString;
                        resourceString = GetString(Resource.String.tellMyselfRemoveQuestionAudio);
                    }
                    else
                    {
                        resourceString = GetString(Resource.String.tellMyselfRemoveTitleText);
                        alertHelper.AlertTitle = resourceString;
                        resourceString = GetString(Resource.String.tellMyselfRemoveQuestionText);
                    }
                    alertHelper.AlertMessage = resourceString;
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                else
                {
                    if (GlobalData.TellMyselfItemsList != null && GlobalData.TellMyselfItemsList.Count > 0)
                    {
                        Toast.MakeText(this, Resource.String.TellMyselfToastSelectItem, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.TellMyselfToastNoItems, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Remove: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorRemovingTellMyselfItem), "TellMyselfActivity.Remove");
            }
        }

        private void MediaPlayer_Completion(object sender, EventArgs e)
        {
            try
            {
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.Reset();
                    _mediaPlayer.Release();
                    _mediaPlayer = null;
                    _isPlaying = false;
                }
                SetActionButtonsAvailability(true);
                UpdateAdapter();
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "MediaPlayer_Completion: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorTellMyselfResetMediaPlayer), "TellMyselfActivity.MediaPlayer_Completion");
            }
        }

        private void ListTellMyself_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Log.Info(TAG, "ListTellMyself_ItemClick: Item selected at position " + e.Position.ToString());
            _selectedItemIndex = e.Position;
            UpdateAdapter();
            if (_listTellMyself != null)
                _listTellMyself.SetSelection(_selectedItemIndex);
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        public void PlayAudio(int tag)
        {
            if (_isPlaying) return;

            try
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.Completion += MediaPlayer_Completion;
                }
                else
                {
                    _mediaPlayer.Reset();
                }

                if (_listTellMyself != null)
                {
                    _selectedItemIndex = tag;
                    UpdateAdapter();
                    _listTellMyself.SetSelection(tag);
                }

                SetActionButtonsAvailability(false);

                var filePath = GlobalData.TellMyselfItemsList[tag].TellText;
                Log.Info(TAG, "PlayAudio: Found filepath of " + filePath);
                //weird bug when preparing on ICS tablet - filestream and pass the file descriptor as the datasource
                FileInputStream file = new FileInputStream(filePath);
                _mediaPlayer.SetDataSource(file.FD);
                _mediaPlayer.SetAudioStreamType(Stream.Music);
                _mediaPlayer.Prepare();
                _mediaPlayer.Start();
                _isPlaying = true;
                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "PlayAudio: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorTellMyselfPlayAudio), "TellMyselfActivity.PlayAudio");
            }
        }

        private void SetActionButtonsAvailability(bool availability)
        {
            try
            {
                var itemAddText = _menu.FindItem(Resource.Id.tellmyselfActionAddText);
                var itemAddVoice = _menu.FindItem(Resource.Id.tellmyselfActionAddVoice);
                var itemRemove = _menu.FindItem(Resource.Id.tellmyselfActionRemove);

                switch (availability)
                {
                    case false:
                        if (itemAddText != null)
                        {
                            itemAddText.SetEnabled(false);
                        }
                        if (itemAddVoice != null)
                        {
                            itemAddVoice.SetEnabled(false);
                        }
                        if (itemRemove != null)
                        {
                            itemRemove.SetEnabled(false);
                        }
                        break;
                    case true:
                    default:
                        if (itemAddText != null)
                        {
                            itemAddText.SetEnabled(true);
                        }
                        if (itemAddVoice != null)
                        {
                            itemAddVoice.SetEnabled(true);
                        }
                        if (itemRemove != null)
                        {
                            itemRemove.SetEnabled(true);
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionButtonsAvailability: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorTellMyselfSettingActionButtons), "TellMyselfActivity.SetActionButtonsAvailability");
            }
        }

        private void AddText()
        {
            try
            {
                _genericTextFragment = new GenericTextDialogFragment(this, "Tell Yourself...", GetString(Resource.String.TellMyselfGenericTextTitle));

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _genericTextFragment.Show(fragmentTransaction, _genericTextFragment.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorTellMyselfAddText), "TellMyselfActivity.AddText_Click");
            }
        }

        private void AddAudio()
        {
            if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.UseMicrophone)))
            {
                CheckMicPermission();
                return;
            }

            try
            {
                _voiceRecordingSelector = new VoiceRecordingDialogFragment(this, "Add Audio");

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _voiceRecordingSelector.Show(fragmentTransaction, _voiceRecordingSelector.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorTellMyselfAddAudio), "TellMyselfActivity.AddAUdio_Click");
            }
        }

        private void GetFieldComponents()
        {
            _listTellMyself = FindViewById<ListView>(Resource.Id.lstTellMyself);
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
                    case Resource.Id.tellmyselfActionAddText:
                        AddText();
                        return true;

                    case Resource.Id.tellmyselfActionAddVoice:
                        AddAudio();
                        return true;

                    case Resource.Id.tellmyselfActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.tellmyselfActionHelp:
                        Intent intent = new Intent(this, typeof(SafetyPlanTellMyselfHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public void RecordingCompleted(string path, string title)
        {
            try
            {
                TellMyself tell = new TellMyself();
                tell.TellType = ConstantsAndTypes.TELL_TYPE.Audio;
                tell.TellText = path.Trim();
                tell.TellTitle = title.Trim();
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                tell.Save(sqlDatabase);
                dbHelp.CloseDatabase();
                GlobalData.TellMyselfItemsList.Add(tell);
                Log.Info(TAG, "RecordingCompleted: Added recording " + title.Trim() + ", ID '" + tell.ID.ToString() + "'");

                UpdateAdapter();

                Toast.MakeText(this, GetString(Resource.String.RecordedAudioToast) + " '" + title + "'", ToastLength.Short).Show();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RecordingCompleted: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCompletingTellMyselfAudio), "TellMyselfActivity.RecordingCompleted");
            }
        }

        public void RecordingCancelled(string filePath)
        {
            try
            {
                File file = new File(filePath);
                if (file != null)
                    file.Delete();
                Log.Info(TAG, "RecordingCancelled: Deleted cancelled recording '" + filePath.Trim() + "'");

                Toast.MakeText(this, Resource.String.CancelledRecordingToast, ToastLength.Short).Show();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RecordingCancelled: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCancellingAudioRecording), "TellMyselfActivity.RecordingCancelled");
            }
        }

        //Required callback for generic text dialog entry
        public void ConfirmText(string textEntered, int genericTextID)
        {
            try
            {
                if (textEntered.Trim() == "") return;

                TellMyself tell = new TellMyself();
                if (genericTextID != -1)
                {
                    tell.IsNew = false;
                    tell.IsDirty = true;
                    tell.ID = genericTextID;
                }
                else
                {
                    tell.IsDirty = false;
                    tell.IsNew = true;
                }

                tell.TellType = ConstantsAndTypes.TELL_TYPE.Textual;
                tell.TellText = textEntered.Trim();
                tell.TellTitle = "";
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                tell.Save(sqlDatabase);
                dbHelp.CloseDatabase();

                if (genericTextID == -1)
                {
                    GlobalData.TellMyselfItemsList.Add(tell);
                    Log.Info(TAG, "ConfirmText: Added text " + textEntered.Trim() + ", ID '" + tell.ID.ToString() + "'");
                }
                else
                {
                    var index = GlobalData.TellMyselfItemsList.IndexOf(GlobalData.TellMyselfItemsList.Find(gen => gen.ID == genericTextID));
                    GlobalData.TellMyselfItemsList[index] = tell;
                    Log.Info(TAG, "ConfirmText: Updated text " + textEntered.Trim() + ", ID '" + tell.ID.ToString() + "'");
                }

                UpdateAdapter();

                Toast.MakeText(this, GetString(Resource.String.TellMyselfAddedToast) + " '" + textEntered + "'", ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorTellMyselfAddText), "TellMyselfActivity.ConfirmText");
            }
        }

        //Required callback for generic text dialog entry
        public void CancelText()
        {
            Toast.MakeText(this, Resource.String.CancelledTellMyselfToast, ToastLength.Short).Show();
        }

        private void UpdateAdapter()
        {
            TellMyselfListAdapter adapter = new TellMyselfListAdapter(this, _selectedItemIndex, _isPlaying);
            if(adapter != null)
            {
                if (_listTellMyself != null)
                    _listTellMyself.Adapter = adapter;
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                if (instanceId == "remove")
                {
                    if (_selectedItemIndex != -1)
                    {
                        Globals dbHelp = new Globals();
                        dbHelp.OpenDatabase();
                        var sqlDatabase = dbHelp.GetSQLiteDatabase();
                        var item = GlobalData.TellMyselfItemsList[_selectedItemIndex];
                        item.Remove(sqlDatabase);
                        dbHelp.CloseDatabase();
                        if (item.TellType == ConstantsAndTypes.TELL_TYPE.Audio)
                        {
                            File file = new File(item.TellText);
                            file.Delete();
                        }
                        GlobalData.TellMyselfItemsList.Remove(item);
                        Log.Info(TAG, "AlertPositiveButtonSelect: Removed item");

                        _selectedItemIndex = -1;

                        UpdateAdapter();
                    }
                }

                if (instanceId == "useMic")
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Tell Myself", "TellMyselfActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
            }
        }

        public void OnGenericDialogDismiss()
        {
            
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "TellMyselfActivity.CheckMicPermission");
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
                        PermissionResultUpdate(Permission.Granted);
                    }
                    else
                    {
                        PermissionResultUpdate(Permission.Denied);
                        Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "TellMyselfActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Granted)
            {
                AddAudio();
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "TellMyselfActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "TellMyselfActivity.AttemptPermissionRequest");
            }
        }
    }
}