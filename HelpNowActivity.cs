using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech.Tts;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model;
using Android.Graphics;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Java.Util;
using Android.Content.PM;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class HelpNowActivity : AppCompatActivity, TextToSpeech.IOnInitListener, IAlertCallback
    {
        public const string TAG = "M:HelpNowActivity";

        private Toolbar _toolbar;

        private LinearLayout _linCallUs;
        private LinearLayout _linEmailUs;
        private LinearLayout _linMessageUs;

        private ImageView _imgCallUs;
        private ImageView _imgEmailUs;
        private ImageView _imgMessageUs;

        private Button _btnDone;

        private TextToSpeech _spokenWord;

        private bool _showHelpNowSetting = true;
        private bool _confirmationAudioSetting = true;

        private int _maximumCallCount = 0;
        private int _currentCallIndex = -1;
        private bool _cyclingEmergencyCalls = false;
        private List<Contact> _emergencyCallContacts = null;
        private TelephonyManager _telephonyManager = null;
        private EmergencyPhoneStateListener _phoneStateListener;

        private ConstantsAndTypes.HELP_NOW_TYPES _emergencyType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                {
                    _maximumCallCount = savedInstanceState.GetInt("maximumCallCount");
                    _currentCallIndex = savedInstanceState.GetInt("currentCallIndex");
                }

                SetContentView(Resource.Layout.HelpNowLayout);

                GetShowHelpNowSetting();

                GetAudioConfirmationSetting();

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.safetyPlanToolbar, Resource.String.SafetyPlanActionBarTitle, Color.White);

                SetupCallbacks();

                if (_confirmationAudioSetting)
                {
                    if (_spokenWord == null)
                    {
                        _spokenWord = new TextToSpeech(this, this);
                        Log.Info(TAG, "OnCreate: _spokenWord initialised");
                    }
                }
                if (_emergencyCallContacts == null)
                    _emergencyCallContacts = new List<Contact>();

                InitialiseTelephony();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Error occurred during creation - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Help Now Activity", "HelpNowActivity.OnCreate");
            }
        }

        private void GetAudioConfirmationSetting()
        {
            try
            {
                if (GlobalData.Settings != null)
                {
                    Setting setting = GlobalData.Settings.Find(set => set.SettingKey == "ConfirmationAudio");
                    if (setting != null)
                        _confirmationAudioSetting = (setting.SettingValue == "True" ? true : false);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAudioConfirmationSetting: Error occurred getting ShowHelpNow setting - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting ShowHelpNow setting", "HelpNowActivity.GetAudioConfirmationSetting");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _linCallUs = FindViewById<LinearLayout>(Resource.Id.linCallUs);
                _linEmailUs = FindViewById<LinearLayout>(Resource.Id.linEmailUs);
                _linMessageUs = FindViewById<LinearLayout>(Resource.Id.linMessageUs);

                _imgCallUs = FindViewById<ImageView>(Resource.Id.imgCallUs);
                _imgEmailUs = FindViewById<ImageView>(Resource.Id.imgEmailUs);
                _imgMessageUs = FindViewById<ImageView>(Resource.Id.imgMessageUs);

                _btnDone = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Error occurred during creation - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Help Now Activity", "HelpNowActivity.OnCreate");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_linCallUs != null)
                    _linCallUs.Click += LinCallUs_Click;
                if(_linEmailUs != null)
                    _linEmailUs.Click += LinEmailUs_Click;
                if(_linMessageUs != null)
                    _linMessageUs.Click += LinMessageUs_Click;

                if (_imgCallUs != null)
                    _imgCallUs.Click += LinCallUs_Click;
                if (_imgEmailUs != null)
                    _imgEmailUs.Click += LinEmailUs_Click;
                if (_imgMessageUs != null)
                    _imgMessageUs.Click += LinMessageUs_Click;

                if(_btnDone != null)
                    _btnDone.Click += Done_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Error occurred setting up Callbacks - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up Callbacks", "HelpNowActivity.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.HelpNowMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemHelp = menu.FindItem(Resource.Id.helpNowActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "HelpNowActivity.SetActionIcons");
            }
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
                if (item.ItemId == Resource.Id.helpNowActionHelp)
                {
                    Intent intent = new Intent(this, typeof(HelpNowHelpActivity));
                    StartActivity(intent);
                }
            }
            return base.OnOptionsItemSelected(item);
        }

        private void LinMessageUs_Click(object sender, EventArgs e)
        {
            try
            {
                var smsContacts =
                    (from eachContact in GlobalData.ContactsUserItems
                     where eachContact.ContactEmergencySms == true
                     select eachContact).ToList();
                if (smsContacts != null && smsContacts.Count > 0)
                {
                    ShowEmergencyDialogAlert(ConstantsAndTypes.HELP_NOW_TYPES.EmergencySms);
                }
                else
                {
                    Toast.MakeText(this, Resource.String.emergencySmsNoContactsSetupForTexting, ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "LinMessageUs_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Sending Emergency Sms Message", "HelpNowActivity.LinMessageUs_Click");
            }
        }

        private void LinEmailUs_Click(object sender, EventArgs e)
        {
            try
            {
                var emailContacts =
                    (from eachContact in GlobalData.ContactsUserItems
                     where eachContact.ContactEmergencyEmail == true
                     select eachContact).ToList();
                if (emailContacts != null && emailContacts.Count > 0)
                {
                    ShowEmergencyDialogAlert(ConstantsAndTypes.HELP_NOW_TYPES.EmergencyEmail);
                }
                else
                {
                    Toast.MakeText(this, Resource.String.emergencyEmailNoContactsSetupForEmailing, ToastLength.Long).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "LinEmailUs_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Sending an Emergency Email", "HelpNowActivity.LinEmailUs_Click");
            }
        }

        private void LinCallUs_Click(object sender, EventArgs e)
        {
            try
            {
                _emergencyCallContacts =
                    (from eachContact in GlobalData.ContactsUserItems
                     where eachContact.ContactEmergencyCall == true
                     select eachContact).ToList();

                _currentCallIndex = -1;
                _maximumCallCount = 0;

                if (_emergencyCallContacts != null)
                {
                    if (_emergencyCallContacts.Count > 0)
                    {
                        _maximumCallCount = _emergencyCallContacts.Count;
                        _currentCallIndex = 0;
                        ShowEmergencyDialogAlert(ConstantsAndTypes.HELP_NOW_TYPES.EmergencyTelephone);
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.emergencyCallNoContactsSetupForCalling, ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "LinCallUs_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Making an Emergency Call", "HelpNowActivity.LinCallUs_Click");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_spokenWord != null)
            {
                _spokenWord.Shutdown();
                Log.Info(TAG, "OnDestroy: Shutdown of Spoken Word");
            }
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                if (_spokenWord != null)
                {
                    if (_spokenWord.IsSpeaking)
                        _spokenWord.Stop();
                }
            }

            return base.DispatchKeyEvent(e);
        }

        private void GetShowHelpNowSetting()
        {
            try
            {
                if (GlobalData.Settings != null)
                {
                    Setting setting = GlobalData.Settings.Find(set => set.SettingKey == "ShowHelpNow");
                    if (setting != null)
                        _showHelpNowSetting = (setting.SettingValue == "True" ? true : false);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetShowHelpNowSetting: Error occurred getting ShowHelpNow setting - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting ShowHelpNow setting", "HelpNowActivity.GetShowHelpNowSetting");
            }
        }

        private void EmergencySmsSend()
        {
            string smsToList = "smsto:";
            string separator = ";";

            //Samsung devices apparently don't accept semi-colon but use a comma instead - gahh!!!
            if (Build.Manufacturer.ToLower() == "samsung")
            {
                Log.Info(TAG, "EmergencySmsSend: Detected a Samsung device, changing separator to a comma");
                separator = ",";
            }

            Log.Info(TAG, "EmergencySmsSend: User has confirmed sending of Emergency Sms");
            try
            {
                if (GlobalData.ContactsUserItems != null && GlobalData.ContactsUserItems.Count > 0)
                {
                    var smsContacts =
                        (from eachContact in GlobalData.ContactsUserItems
                         where eachContact.ContactEmergencySms == true
                         select eachContact).ToList();

                    if (smsContacts != null)
                    {
                        if (smsContacts.Count > 0)
                        {
                            var index = 1;
                            foreach (var contact in smsContacts)
                            {
                                Log.Info(TAG, "EmergencySmsSend: Adding Contact for an Emergency Sms - " + contact.ContactName.Trim());
                                if (contact.ContactTelephoneNumber.Trim().Length > 0)
                                {
                                    smsToList += contact.ContactTelephoneNumber.Trim().Replace(" ", "") + (index != smsContacts.Count ? separator : "");
                                    index++;
                                }
                            }
                            Log.Info(TAG, "EmergencySmsSend: Send to list is - '" + smsToList + "'");
                            if (smsToList.Length > 6) //the length of 'smsto:' is 6
                            {
                                Intent intent = new Intent(Intent.ActionSendto, Android.Net.Uri.Parse(smsToList));
                                var setting = GlobalData.Settings.Find(set => set.SettingKey == "EmergencySms");
                                string theValue = "";
                                if (setting == null)
                                {
                                    theValue = GetString(Resource.String.emergencySmsDefaultBodyText);
                                }
                                else
                                {
                                    theValue = setting.SettingValue.Trim();
                                }
                                intent.PutExtra("sms_body", theValue);
                                StartActivity(intent);
                            }
                        }
                    }
                }
                else
                {
                    Log.Info(TAG, "EmergencySmsSend: ContactsUserItems is null or has no items");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "EmergencySmsSend: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Sending Emergency Sms Message", "HelpNowActivity.EmergencySmsSend");
            }
        }

        private void EmergencyEmailSend()
        {
            string[] addresses = null;

            Log.Info(TAG, "EmergencyEmailSend: User has confirmed Emailing Emergency Contacts");
            try
            {
                var emailContacts =
                    (from eachContact in GlobalData.ContactsUserItems
                     where eachContact.ContactEmergencyEmail == true
                     select eachContact).ToList();

                if (emailContacts != null)
                {
                    if (emailContacts.Count > 0)
                    {
                        addresses = new string[emailContacts.Count];
                        var addressIndex = 0;
                        foreach (var contact in emailContacts)
                        {
                            Log.Info(TAG, "EmergencyEmailSend: Sending an Emergency Email to " + contact.ContactName.Trim());
                            addresses[addressIndex] = contact.ContactEmail.Trim();
                            addressIndex++;
                        }

                        var settingSubject = GlobalData.Settings.Find(set => set.SettingKey == "EmergencyEmailSubject");
                        var settingBody = GlobalData.Settings.Find(set => set.SettingKey == "EmergencyEmailBody");
                        string subjectValue = "";
                        string bodyValue = "";
                        if (settingSubject == null)
                        {
                            subjectValue = GetString(Resource.String.emergencyEmailSendSubject);
                        }
                        else
                        {
                            subjectValue = settingSubject.SettingValue.Trim();
                        }
                        if (settingBody == null)
                        {
                            bodyValue = GetString(Resource.String.emergencyEmailSendBody);
                        }
                        else
                        {
                            bodyValue = settingBody.SettingValue.Trim();
                        }
                        Intent intent = new Intent(Intent.ActionSendto);
                        intent.SetData(Android.Net.Uri.Parse("mailto:"));
                        intent.PutExtra(Intent.ExtraEmail, addresses);
                        intent.PutExtra(Intent.ExtraSubject, subjectValue);
                        intent.PutExtra(Intent.ExtraText, bodyValue);
                        StartActivity(intent);
                        Log.Info(TAG, "EmergencyEmailSend: Sent Emergency Email to specified addresses!");
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.emergencyEmailNoContactsSetupForEmailing, ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "EmergencyEmailSend: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Sending an Emergency Email", "HelpNowActivity.EmergencyEmailSend");
            }
        }

        public void CallStatusCallback(long callTickLength)
        {
            //what does the ticklength tell us about the call just attempted?
            Log.Info(TAG, "CallStatusCallback: Dialler reports call tick length of " + callTickLength.ToString());
            //Continue on to the next call in the list if there is one
            EmergencyMakeCall();
        }

        private void EmergencyMakeCall()
        {
            Log.Info(TAG, "EmergencyMakeCall: User has confirmed Calling Emergency Contacts");
            try
            {
                if (_cyclingEmergencyCalls)
                {
                    if (_maximumCallCount != 0)
                    {
                        if (_currentCallIndex != -1)
                        {
                            if (_currentCallIndex < _maximumCallCount)
                            {
                                var contact = _emergencyCallContacts[_currentCallIndex];
                                if (contact != null)
                                {
                                    Toast.MakeText(this, GetString(Resource.String.SafetyPlanDialPreambleToast) + " " + contact.ContactName + " " + GetString(Resource.String.SafetyPlanDialMidTextToast) + " " + contact.ContactTelephoneNumber, ToastLength.Long).Show();
                                    var uri = Android.Net.Uri.Parse("tel:" + contact.ContactTelephoneNumber);
                                    Intent callIntent = new Intent(Intent.ActionCall, uri);
                                    Log.Info(TAG, "EmergencyMakeCall: Dialling " + contact.ContactTelephoneNumber + ", contact " + (_currentCallIndex + 1).ToString() + " of " + _maximumCallCount.ToString());
                                    _phoneStateListener.StartTick = DateTime.Now.Ticks;
                                    StartActivity(callIntent);
                                    _currentCallIndex++;
                                }
                            }
                            else
                            {
                                //no more contacts
                                _currentCallIndex = -1;
                                _maximumCallCount = 0;
                                _cyclingEmergencyCalls = false;
                                Log.Info(TAG, "EmergencyMakeCall: Finished Emergency Call list cycle!");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "EmergencyMakeCall: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Making an Emergency Call", "HelpNowActivity.EmergencyMakeCall");
            }
        }

        private void InitialiseTelephony()
        {
            //the next bit of code sets up a listener via the telephony manager
            _telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);
            if (_telephonyManager != null)
            {
                _phoneStateListener = new EmergencyPhoneStateListener(this);
                if (_phoneStateListener != null)
                {
                    _telephonyManager.Listen(_phoneStateListener, PhoneStateListenerFlags.CallState);
                }
            }
        }

        private void GetEmergencyTitleAndQuestionForNowType(ConstantsAndTypes.HELP_NOW_TYPES emergencyType, out string resourceStringTitle, out string resourceStringQuestion)
        {
            try
            {
                resourceStringTitle = "";
                resourceStringQuestion = "";

                switch (emergencyType)
                {
                    case ConstantsAndTypes.HELP_NOW_TYPES.EmergencyTelephone:
                        resourceStringTitle = GetString(Resource.String.emergencyCallButtonPressedTitle);
                        resourceStringQuestion = GetString(Resource.String.emergencyCallButtonPressedText);
                        break;
                    case ConstantsAndTypes.HELP_NOW_TYPES.EmergencySms:
                        resourceStringTitle = GetString(Resource.String.emergencySmsButtonPressedTitle);
                        resourceStringQuestion = GetString(Resource.String.emergencySmsButtonPressedText);
                        break;
                    case ConstantsAndTypes.HELP_NOW_TYPES.EmergencyEmail:
                        resourceStringTitle = GetString(Resource.String.emergencyEmailButtonPressedTitle);
                        resourceStringQuestion = GetString(Resource.String.emergencyEmailButtonPressedText);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetEmergencyTitleAndQuestionForNowType: Exception - " + e.Message);
                resourceStringTitle = "";
                resourceStringQuestion = "";
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting the Title and Question for the Emergency Type", "HelpNowActivity.GetEmergencyTitleAndQuestionForNowType");
            }
        }

        private void ShowEmergencyDialogAlert(ConstantsAndTypes.HELP_NOW_TYPES emergencyType)
        {
            var resourceStringTitle = "";
            var resourceStringQuestion = "";

            try
            {
                if (emergencyType == ConstantsAndTypes.HELP_NOW_TYPES.EmergencySms)
                {
                    if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.SendSms) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.SendSms)))
                    {
                        if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagSendSms").SettingValue == "False")
                        {
                            CheckPermission(ConstantsAndTypes.AppPermission.SendSms);
                            return;
                        }
                        else
                        {
                            Toast.MakeText(this, Resource.String.SendSmsPermissionDenialToast, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                if (emergencyType == ConstantsAndTypes.HELP_NOW_TYPES.EmergencyTelephone)
                {
                    if (!(PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.MakeCalls) && PermissionsHelper.PermissionGranted(this, ConstantsAndTypes.AppPermission.MakeCalls)))
                    {
                        if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMakeCalls").SettingValue == "False")
                        {
                            CheckPermission(ConstantsAndTypes.AppPermission.MakeCalls);
                            return;
                        }
                        else
                        {
                            Toast.MakeText(this, Resource.String.MakeCallsPermissionDenialToast, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                StopSpeaking();

                GetEmergencyTitleAndQuestionForNowType(emergencyType, out resourceStringTitle, out resourceStringQuestion);

                SayText(resourceStringQuestion);

                _emergencyType = emergencyType;

                AlertHelper alertHelper = new AlertHelper(this);
                if (alertHelper != null)
                {
                    alertHelper.AlertTitle = resourceStringTitle;
                    alertHelper.AlertMessage = resourceStringQuestion;
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.emergencyConfirmationPositive);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.emergencyConfirmationNegative);
                    alertHelper.InstanceId = "emergencyAlert";
                    alertHelper.ShowAlert();
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowEmergencyDialogAlert: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Showing Emergency Alert Dialog", "HelpNowActivity.ShowEmergencyDialogAlert");
            }
        }

        private void PerformSpecifiedEmergencyAction(ConstantsAndTypes.HELP_NOW_TYPES emergencyType)
        {
            try
            {
                switch (emergencyType)
                {
                    case ConstantsAndTypes.HELP_NOW_TYPES.EmergencyTelephone:
                        Log.Info(TAG, "PerformSpecifiedEmergencyAction: User confirmed Emergency Action for Call");
                        _cyclingEmergencyCalls = true;
                        EmergencyMakeCall();
                        break;
                    case ConstantsAndTypes.HELP_NOW_TYPES.EmergencySms:
                        Log.Info(TAG, "PerformSpecifiedEmergencyAction: User confirmed Emergency Action for Sms");
                        EmergencySmsSend();
                        break;
                    case ConstantsAndTypes.HELP_NOW_TYPES.EmergencyEmail:
                        Log.Info(TAG, "PerformSpecifiedEmergencyAction: User confirmed Emergency Action for Email");
                        EmergencyEmailSend();
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "PerformSpecifiedEmergencyAction Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Performing a selected Emergency Action", "HelpNowActivity.PerformSpecifiedEmergencyAction");
            }
        }

        public void OnInit([GeneratedEnum] OperationResult status)
        {
            if (status == OperationResult.Success)
            {
                if (_spokenWord != null)
                {
                    if (_spokenWord.IsSpeaking)
                        _spokenWord.Stop();
                    if (Locale.Default.ISO3Language.ToLower() == "eng")
                    {
                        Locale english = new Locale("en", "gbr");
                        _spokenWord.SetLanguage(english);
                    }

                    if (Locale.Default.ISO3Language.ToLower() == "spa")
                    {
                        Locale spanish = new Locale("es", "ESP");
                        _spokenWord.SetLanguage(spanish);
                    }
                    if(Locale.Default.ISO3Language.ToLower() == "fra")
                    {
                        Locale french = new Locale("fr", "FRA");
                        _spokenWord.SetLanguage(french);
                    }
                }
            }
        }

        private void StopSpeaking()
        {
            if (_spokenWord != null)
            {
                if (_spokenWord.IsSpeaking)
                    _spokenWord.Stop();
            }
        }

        private void SayText(string textToSay)
        {
            if (_spokenWord != null)
            {
                if (_spokenWord.IsSpeaking)
                    _spokenWord.Stop();

                _spokenWord.Speak(textToSay, QueueMode.Flush, null);
            }
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "sendSms")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.SendSms);
                return;
            }
            if (instanceId == "makeCalls")
            {
                PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.MakeCalls);
                return;
            }

            PerformSpecifiedEmergencyAction(_emergencyType);
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "sendSms")
            {
                Toast.MakeText(this, Resource.String.SendSmsPermissionDenialToast, ToastLength.Short).Show();
                return;
            }
            if (instanceId == "makeCalls")
            {
                Toast.MakeText(this, Resource.String.MakeCallsPermissionDenialToast, ToastLength.Short).Show();
                return;
            }

            Log.Info(TAG, "ShowEmergencyDialogAlert: User cancelled Emergency Action");
        }

        private void CheckPermission(ConstantsAndTypes.AppPermission permission)
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(this, permission) && PermissionsHelper.PermissionGranted(this, permission)))
                {
                    AttemptPermissionRequest(permission);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "SafetyActivity.CheckPermission");
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_SEND_SMS)
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
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.SendSms))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.SendSms).PermissionGranted = Permission.Granted;
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.SendSmsPermissionDenialToast, ToastLength.Short).Show();
                    }
                }
                if (requestCode == ConstantsAndTypes.REQUEST_CODE_PERMISSION_MAKE_CALLS)
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
                            if (PermissionsHelper.HasPermission(this, ConstantsAndTypes.AppPermission.MakeCalls))
                            {
                                GlobalData.ApplicationPermissions.Find(perm => perm.ApplicationPermission == ConstantsAndTypes.AppPermission.MakeCalls).PermissionGranted = Permission.Granted;
                            }
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.MakeCallsPermissionDenialToast, ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnRequestPermissionsResult: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "HelpNowActivity.OnRequestPermissionsResult");
            }
        }
        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {

            }
        }

        private void ShowPermissionRationale(ConstantsAndTypes.AppPermission permission)
        {
            try
            {
                if (permission == ConstantsAndTypes.AppPermission.SendSms)
                    if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagSendSms").SettingValue == "True") return;

                if (permission == ConstantsAndTypes.AppPermission.MakeCalls)
                    if (GlobalData.Settings.Find(setting => setting.SettingKey == "NagMakeCalls").SettingValue == "True") return;

                AlertHelper alertHelper = new AlertHelper(this);

                string instanceId = "";
                string alertTitle = "";
                string alertMessage = "";

                if (permission == ConstantsAndTypes.AppPermission.SendSms)
                {
                    instanceId = "sendSms";
                    alertTitle = GetString(Resource.String.RequestPermissionSendSmsAlertTitle);
                    alertMessage = GetString(Resource.String.RequestPermissionSendSmsAlertMessage);
                }
                if (permission == ConstantsAndTypes.AppPermission.MakeCalls)
                {
                    instanceId = "makeCalls";
                    alertTitle = GetString(Resource.String.RequestPermissionMakeCallsAlertTitle);
                    alertMessage = GetString(Resource.String.RequestPermissionMakeCallsAlertMessage);
                }

                alertHelper.AlertIconResourceID = Resource.Drawable.SymbolInformation;
                alertHelper.AlertMessage = alertMessage;
                alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                alertHelper.AlertTitle = alertTitle;
                alertHelper.InstanceId = instanceId;
                alertHelper.ShowAlert();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "ShowPermissionRationale: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "HelpNowActivity.ShowPermissionRationale");
            }
        }

        public void AttemptPermissionRequest(ConstantsAndTypes.AppPermission permission)
        {
            try
            {
                if (PermissionsHelper.ShouldShowPermissionRationale(this, permission))
                {
                    ShowPermissionRationale(permission);
                    return;
                }
                else
                {
                    //just request the permission
                    PermissionsHelper.RequestApplicationPermission(this, permission);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AttemptPermissionRequest: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "HelpNowActivity.AttemptPermissionRequest");
            }
        }
    }
}