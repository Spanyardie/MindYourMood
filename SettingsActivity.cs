using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Content;
using com.spanyardie.MindYourMood.SubActivities.Help;

namespace com.spanyardie.MindYourMood
{
    [Activity()]
    public class SettingsActivity : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:SettingsActivity";

        private Toolbar _toolbar;

        private Switch _emergencyButton;
        private Switch _emergencyAudioButton;
        private Switch _errordialogButton;
        private Switch _nagContactsSwitch;
        private Switch _nagMicSwitch;
        private Switch _nagSendSmsSwitch;
        private Switch _nagMakeCallsSwitch;
        private Switch _nagReadExternalStorageSwitch;

        private Spinner _alarmType;

        private Switch _emergencyCallSpeakerButton;
        private TextView _emergencySmsText;
        private TextView _emergencyEmailSubjectText;
        private TextView _emergencyEmailBodyText;

        private Setting _showEmergency = null;
        private Setting _confirmationAudio = null;
        private Setting _notificationType = null;
        private Setting _showErrorDialog = null;
        private Setting _nagContacts;
        private Setting _nagMic;
        private Setting _nagSendSms;
        private Setting _nagMakeCalls;
        private Setting _nagReadExternalStorage;

        private Setting _emergencyCallSpeaker;
        private Setting _emergencySms;
        private Setting _emergencyEmailSubject;
        private Setting _emergencyEmailBody;

        private ImageButton _moodsAdjustButton;


        private bool _updatingSettings = false;

        private bool _madeChanges = false;

        private bool _firstTimeAlarmTypeSelection = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.Settings);

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.settingsToolbar, Resource.String.SettingsActionbarTitle, Color.White);

                GetFieldComponents();

                SetupCallbacks();

                SetupSpinners();

                GetSettings();

                UpdateSettings();

            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateAngerActivity), "SettingsActivity.OnCreate");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_emergencyButton != null)
                    _emergencyButton.CheckedChange += EmergencyButton_CheckedChange;
                if(_emergencyAudioButton != null)
                    _emergencyAudioButton.CheckedChange += EmergencyAudioButton_CheckedChange;
                if(_errordialogButton != null)
                    _errordialogButton.CheckedChange += ErrordialogButton_CheckedChange;
                if(_alarmType != null)
                    _alarmType.ItemSelected += AlarmType_ItemSelected;

                if(_nagContactsSwitch != null)
                    _nagContactsSwitch.CheckedChange += NagContactsSwitch_CheckedChange;
                if(_nagMicSwitch != null)
                    _nagMicSwitch.CheckedChange += NagMicSwitch_CheckedChange;

                if (_nagSendSmsSwitch != null)
                    _nagSendSmsSwitch.CheckedChange += NagSendSmsSwitch_CheckedChange;
                if (_nagMakeCallsSwitch != null)
                    _nagMakeCallsSwitch.CheckedChange += NagMakeCallsSwitch_CheckedChange;
                if (_nagReadExternalStorageSwitch != null)
                    _nagReadExternalStorageSwitch.CheckedChange += NagReadExternalStorageSwitch_CheckedChange;

                if(_emergencyCallSpeakerButton != null)
                    _emergencyCallSpeakerButton.CheckedChange += EmergencyCallSpeakerButton_CheckedChange;

                if(_emergencySmsText != null)
                    _emergencySmsText.TextChanged += EmergencySmsText_TextChanged;
                if(_emergencyEmailSubjectText != null)
                    _emergencyEmailSubjectText.TextChanged += EmergencyEmailSubjectText_TextChanged;
                if(_emergencyEmailBodyText != null)
                    _emergencyEmailBodyText.TextChanged += EmergencyEmailBodyText_TextChanged;

                if(_moodsAdjustButton != null)
                    _moodsAdjustButton.Click += MoodsAdjustButton_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
            }
        }

        private void MoodsAdjustButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MoodsAdjustActivity));
            StartActivity(intent);
        }

        private void EmergencyEmailBodyText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (_emergencyEmailBody == null)
                _emergencyEmailBody = new Setting();

            _emergencyEmailBody.SettingKey = "EmergencyEmailBody";
            _emergencyEmailBody.SettingValue = _emergencyEmailBodyText.Text.Trim();
            if (!_emergencyEmailBody.IsNew)
                _emergencyEmailBody.IsDirty = true;

            _madeChanges = true;
        }

        private void EmergencyEmailSubjectText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (_emergencyEmailSubject == null)
                _emergencyEmailSubject = new Setting();

            _emergencyEmailSubject.SettingKey = "EmergencyEmailSubject";
            _emergencyEmailSubject.SettingValue = _emergencyEmailSubjectText.Text.Trim();
            if (!_emergencyEmailSubject.IsNew)
                _emergencyEmailSubject.IsDirty = true;

            _madeChanges = true;
        }

        private void EmergencySmsText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (_emergencySms == null)
                _emergencySms = new Setting();

            _emergencySms.SettingKey = "EmergencySms";
            _emergencySms.SettingValue = _emergencySmsText.Text.Trim();
            if (!_emergencySms.IsNew)
                _emergencySms.IsDirty = true;

            _madeChanges = true;
        }

        private void EmergencyCallSpeakerButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {

            if (_emergencyCallSpeaker == null)
                _emergencyCallSpeaker = new Setting();

            _emergencyCallSpeaker.SettingKey = "EmergencyCallSpeaker";
            _emergencyCallSpeaker.SettingValue = (_emergencyCallSpeakerButton.Checked ? "True" : "False");
            if (!_emergencyCallSpeaker.IsNew)
                _emergencyCallSpeaker.IsDirty = true;

            _madeChanges = true;
        }

        private void NagReadExternalStorageSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_nagReadExternalStorage == null)
                _nagReadExternalStorage = new Setting();

            _nagReadExternalStorage.SettingKey = "NagReadExternalStorage";
            _nagReadExternalStorage.SettingValue = (_nagReadExternalStorageSwitch.Checked ? "True" : "False");
            if (!_nagReadExternalStorage.IsNew)
                _nagReadExternalStorage.IsDirty = true;

            _madeChanges = true;
        }

        private void NagMakeCallsSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_nagMakeCalls == null)
                _nagMakeCalls = new Setting();

            _nagMakeCalls.SettingKey = "NagMakeCalls";
            _nagMakeCalls.SettingValue = (_nagMakeCallsSwitch.Checked ? "True" : "False");
            if (!_nagMakeCalls.IsNew)
                _nagMakeCalls.IsDirty = true;

            _madeChanges = true;
        }

        private void NagSendSmsSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_nagSendSms == null)
                _nagSendSms = new Setting();

            _nagSendSms.SettingKey = "NagSendSms";
            _nagSendSms.SettingValue = (_nagSendSmsSwitch.Checked ? "True" : "False");
            if (!_nagSendSms.IsNew)
                _nagSendSms.IsDirty = true;

            _madeChanges = true;
        }

        private void NagMicSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_nagMic == null)
                _nagMic = new Setting();

            _nagMic.SettingKey = "NagMic";
            _nagMic.SettingValue = (_nagMicSwitch.Checked ? "True" : "False");
            if (!_nagMic.IsNew)
                _nagMic.IsDirty = true;

            _madeChanges = true;
        }

        private void NagContactsSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_nagContacts == null)
                _nagContacts = new Setting();

            _nagContacts.SettingKey = "NagContacts";
            _nagContacts.SettingValue = (_nagContactsSwitch.Checked ? "True" : "False");
            if (!_nagContacts.IsNew)
                _nagContacts.IsDirty = true;

            _madeChanges = true;
        }

        private void AlarmType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (_updatingSettings) return;

            if(_firstTimeAlarmTypeSelection)
            {
                _firstTimeAlarmTypeSelection = false;
                return;
            }

            if (_notificationType == null)
                _notificationType = new Setting();

            _notificationType.SettingKey = "AlarmNotificationType";
            _notificationType.SettingValue = (e.Position == 0 ? -1 : e.Position).ToString();
            if (!_notificationType.IsNew)
                _notificationType.IsDirty = true;

            _madeChanges = true;
        }

        private void ErrordialogButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_showErrorDialog == null)
                _showErrorDialog = new Setting();

            _showErrorDialog.SettingKey = "ShowErrorDialog";
            _showErrorDialog.SettingValue = (_errordialogButton.Checked ? "True" : "False");
            if (!_showErrorDialog.IsNew)
                _showErrorDialog.IsDirty = true;

            _madeChanges = true;
        }

        private void EmergencyAudioButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_confirmationAudio == null)
                _confirmationAudio = new Setting();

            _confirmationAudio.SettingKey = "ConfirmationAudio";
            _confirmationAudio.SettingValue = (_emergencyAudioButton.Checked ? "True" : "False");
            if (!_confirmationAudio.IsNew)
                _confirmationAudio.IsDirty = true;

            _madeChanges = true;
        }

        private void EmergencyButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_updatingSettings) return;

            if (_showEmergency == null)
                _showEmergency = new Setting();

            _showEmergency.SettingKey = "ShowHelpNow";
            _showEmergency.SettingValue = (_emergencyButton.Checked ? "True" : "False");
            if (!_showEmergency.IsNew)
                _showEmergency.IsDirty = true;

            _madeChanges = true;
        }

        private void UpdateSettings()
        {
            try
            {
                _updatingSettings = true;

                if(_showEmergency != null)
                    _emergencyButton.Checked = (_showEmergency.SettingValue == "True" ? true : false);
                if(_confirmationAudio != null)
                    _emergencyAudioButton.Checked = (_confirmationAudio.SettingValue == "True" ? true : false);
                if(_notificationType != null)
                {
                    int item = -1;
                    if(_notificationType.SettingValue != "-1")
                    {
                        item = Convert.ToInt32(_notificationType.SettingValue.Trim());
                        _alarmType.SetSelection(item);
                    }
                }
                if (_showErrorDialog != null)
                    _errordialogButton.Checked = (_showErrorDialog.SettingValue == "True" ? true : false);

                if (_nagContacts != null)
                    _nagContactsSwitch.Checked = (_nagContacts.SettingValue == "True" ? true : false);

                if (_nagMic != null)
                    _nagMicSwitch.Checked = (_nagMic.SettingValue == "True" ? true : false);

                if (_nagSendSms != null)
                    _nagSendSmsSwitch.Checked = (_nagSendSms.SettingValue == "True" ? true : false);

                if (_nagMakeCalls != null)
                    _nagMakeCallsSwitch.Checked = (_nagMakeCalls.SettingValue == "True" ? true : false);

                if (_nagReadExternalStorage != null)
                    _nagReadExternalStorageSwitch.Checked = (_nagReadExternalStorage.SettingValue == "True" ? true : false);

                if (_emergencyCallSpeaker != null)
                    _emergencyCallSpeakerButton.Checked = (_emergencyCallSpeaker.SettingValue == "True" ? true : false);

                if (_emergencySms != null)
                    _emergencySmsText.Text = _emergencySms.SettingValue.Trim();

                if (_emergencyEmailSubject != null)
                    _emergencyEmailBodyText.Text = _emergencyEmailBody.SettingValue.Trim();

                if (_emergencyEmailBody != null)
                    _emergencyEmailBodyText.Text = _emergencyEmailBody.SettingValue.Trim();

                _updatingSettings = false;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateSettings: Exception - " + e.Message);
            }
        }

        private void GetSettings()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.Settings == null)
                {
                    GlobalData.Settings = new List<Setting>();
                }

                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.GetAllSettings();
                    dbHelp.CloseDatabase();
                }

                _showEmergency = GlobalData.Settings.Find(setting => setting.SettingKey == "ShowHelpNow");
                _confirmationAudio = GlobalData.Settings.Find(setting => setting.SettingKey == "ConfirmationAudio");
                _notificationType = GlobalData.Settings.Find(setting => setting.SettingKey == "AlarmNotificationType");
                _showErrorDialog = GlobalData.Settings.Find(setting => setting.SettingKey == "ShowErrorDialog");

                _nagContacts = GlobalData.Settings.Find(setting => setting.SettingKey == "NagContacts");
                _nagMic = GlobalData.Settings.Find(setting => setting.SettingKey == "NagMic");
                _nagSendSms = GlobalData.Settings.Find(setting => setting.SettingKey == "NagSendSms");
                _nagMakeCalls = GlobalData.Settings.Find(setting => setting.SettingKey == "NagMakeCalls");
                _nagReadExternalStorage = GlobalData.Settings.Find(setting => setting.SettingKey == "NagReadExternalStorage");

                _emergencyCallSpeaker = GlobalData.Settings.Find(setting => setting.SettingKey == "EmergencyCallSpeaker");
                _emergencyEmailSubject = GlobalData.Settings.Find(setting => setting.SettingKey == "EmergencyEmailSubject");
                _emergencyEmailBody = GlobalData.Settings.Find(setting => setting.SettingKey == "EmergencyEmailBody");
                _emergencySms = GlobalData.Settings.Find(setting => setting.SettingKey == "EmergencySms");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetSettings: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.CloseDatabase();
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    if (_madeChanges)
                    {
                        ShowSaveAlert();
                    }
                    else
                    {
                        Finish();
                    }

                    return true;
                }

                switch(item.ItemId)
                {
                    case Resource.Id.SettingsActionSave:
                        SaveSettings();
                        break;
                    case Resource.Id.SettingsActionHelp:
                        Intent intent = new Intent(this, typeof(SettingsHelpActivity));
                        StartActivity(intent);
                        break;
                }
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ShowSaveAlert()
        {
            try
            {
                AlertHelper alertHelper = new AlertHelper(this);
                alertHelper.InstanceId = "0";
                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                alertHelper.AlertTitle = GetString(Resource.String.SettingsAlertTitle);
                alertHelper.AlertMessage = GetString(Resource.String.SettingsAlertQuestion);
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.ShowAlert();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ShowSaveAlert: Exception - " + e.Message);
            }
        }

        private void SaveSettings()
        {
            if (_madeChanges)
            {
                if (_showEmergency != null)
                    _showEmergency.SaveSetting();
                if (_confirmationAudio != null)
                    _confirmationAudio.SaveSetting();
                if (_notificationType != null)
                    _notificationType.SaveSetting();
                if (_nagContacts != null)
                    _nagContacts.SaveSetting();
                if (_nagMic != null)
                    _nagMic.SaveSetting();
                if (_nagSendSms != null)
                    _nagSendSms.SaveSetting();
                if (_nagMakeCalls != null)
                    _nagMakeCalls.SaveSetting();
                if (_nagReadExternalStorage != null)
                    _nagReadExternalStorage.SaveSetting();

                if (_showErrorDialog != null)
                {
                    _showErrorDialog.SaveSetting();
                    GlobalData.ShowErrorDialog = (_showErrorDialog.SettingValue == "True" ? true : false);
                }

                if (_emergencyCallSpeaker != null)
                    _emergencyCallSpeaker.SaveSetting();
                if (_emergencyEmailSubject != null)
                    _emergencyEmailSubject.SaveSetting();
                if (_emergencyEmailBody != null)
                    _emergencyEmailBody.SaveSetting();
                if (_emergencySms != null)
                    _emergencySms.SaveSetting();

                _madeChanges = false;
                Toast.MakeText(this, Resource.String.ToastSettingsSaved, ToastLength.Short).Show();
                Finish();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SettingsMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void GetFieldComponents()
        {
            try
            {
                _emergencyButton = FindViewById<Switch>(Resource.Id.swEmergencyButton);
                _emergencyAudioButton = FindViewById<Switch>(Resource.Id.swEmergencyAudioButton);
                _errordialogButton = FindViewById<Switch>(Resource.Id.swErrorDialogButton);

                _nagContactsSwitch = FindViewById<Switch>(Resource.Id.swNagContactsButton);
                _nagMicSwitch = FindViewById<Switch>(Resource.Id.swNagUseMicButton);
                _nagSendSmsSwitch = FindViewById<Switch>(Resource.Id.swNagSendSmsButton);
                _nagMakeCallsSwitch = FindViewById<Switch>(Resource.Id.swNagMakeCallsButton);
                _nagReadExternalStorageSwitch = FindViewById<Switch>(Resource.Id.swNagReadExternalStorageButton);

                _alarmType = FindViewById<Spinner>(Resource.Id.spnAlarmType);

                _emergencySmsText = FindViewById<TextView>(Resource.Id.edtSmsText);
                _emergencyEmailSubjectText = FindViewById<TextView>(Resource.Id.edtEmailSubjectText);
                _emergencyEmailBodyText = FindViewById<TextView>(Resource.Id.edtEmailBodyText);
                _emergencyCallSpeakerButton = FindViewById<Switch>(Resource.Id.swEmergencyCallSpeaker);

                _moodsAdjustButton = FindViewById<ImageButton>(Resource.Id.imgbtnMoodsAdjust);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "SettingsActivity.GetFieldComponents");
            }
        }

        private void SetupSpinners()
        {
            SetupAlarmTypeSpinner();
        }

        private void SetupAlarmTypeSpinner()
        {
            try
            {
                string[] alarmTypes = StringHelper.AlarmNotificationTypes();
                if(alarmTypes != null && alarmTypes.Length > 0)
                {
                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, alarmTypes);
                    if (adapter != null)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _alarmType.Adapter = adapter;
                        Log.Info(TAG, "SetupAlarmTypeSpinner: Set Alarm type spinner Adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupAlarmTypeSpinner: Failed to create Adapter");
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up Alarm type spinner", "SettingsActivity.SetupAlarmTypeSpinner");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemSave = menu.FindItem(Resource.Id.SettingsActionSave);
                var itemHelp = menu.FindItem(Resource.Id.SettingsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemSave != null)
                            itemSave.SetIcon(Resource.Drawable.ic_thumb_up_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemSave != null)
                            itemSave.SetIcon(Resource.Drawable.ic_thumb_up_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemSave != null)
                            itemSave.SetIcon(Resource.Drawable.ic_thumb_up_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "AffirmationsActivity.SetActionIcons");
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            SaveSettings();
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.ToastSettingSaveCancel, ToastLength.Short).Show();
            _madeChanges = false;
            Finish();
        }
    }
}