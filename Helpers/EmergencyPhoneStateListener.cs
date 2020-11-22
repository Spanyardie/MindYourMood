using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Telephony;
using Android.Util;
using Android.Media;


namespace com.spanyardie.MindYourMood.Helpers
{
    public class EmergencyPhoneStateListener : PhoneStateListener, AudioManager.IOnAudioFocusChangeListener
    {
        public const string TAG = "M:EmergencyPhoneStateListener";

        private Activity _activity;
        private AudioManager _audioManager;

        private long _ticksOnCall = 0;

        public long StartTick { get; set; }

        private CallState _previousCallState = CallState.Idle;

        public EmergencyPhoneStateListener(Activity activity)
        {
            try
            {
                _activity = activity;

                _audioManager = null;

                if (_activity != null)
                {
                    _audioManager = (AudioManager)_activity.GetSystemService(Context.AudioService);
                }

                if (_audioManager != null)
                {
                    _audioManager.RequestAudioFocus(this, Stream.VoiceCall, AudioFocus.Gain);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "EmergencyPhoneStateListener (Constructor): Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorRequestingAudioFocus), "EmergencyPhoneStateListener.EmergencyPhoneStateListener");
            }
        }

        public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {
            Log.Info(TAG, "OnAudioFocusChange: Audio focus changed");
        }

        public override void OnCallStateChanged([GeneratedEnum] CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);

            try
            {
                switch (state)
                {
                    case CallState.Idle:
                        Log.Info(TAG, "OnCallStateChanged: Phone is idle");
                        if (_audioManager != null)
                        {
                            _audioManager.Mode = Mode.Normal;
                            Log.Info(TAG, "OnCallStateChanged: Speaker phone set to OFF");
                            _audioManager.SpeakerphoneOn = false;
                            _audioManager.AbandonAudioFocus(this);
                        }
                        if (_previousCallState == CallState.Offhook)
                        {
                            // we had a call and it has ended, in this api we have no way of knowing why
                            // as there are only three call states:
                            // Idle
                            // OffHook
                            // Ringing
                            //
                            // Ringing is no good to us - we are making outgoing emergency calls
                            // Idle is self-explanatory
                            // Off hook occurs as soon as we start dialling
                            //
                            // Q: What happens if the called number is busy?  How many (roughly) ticks pass before the OS kicks the call back to idle?
                            // Q: What happens when the phone rings and goes to Voicemail? 
                            // To support back to Ice-Cream-Sandwich (API 15) we must use the Telephony API, as we are here
                            // The Telecom API was added in Marshmallow (API 23) which is no good to us either as we will cut most of the user base out
                            // By supporting that
                            // Perhaps we can support both in a later version of MYM
                            _ticksOnCall = DateTime.Now.Ticks - StartTick;
                            Log.Info(TAG, "OnCallStateChanged: Calling back to parent activity status callback function with " + _ticksOnCall.ToString() + " ticks");
                            ((HelpNowActivity)_activity).CallStatusCallback(_ticksOnCall);
                            _ticksOnCall = 0;
                        }
                        else
                        {
                            _ticksOnCall = 0;
                        }
                        break;
                    case CallState.Offhook:
                        Log.Info(TAG, "OnCallStateChanged: Phone is Off the Hook");
                        if (_audioManager != null)
                        {
                            _audioManager.Mode = Mode.InCall;
                            var setting = GlobalData.Settings.Find(set => set.SettingKey == "EmergencyCallSpeaker");
                            var speakerValue = true;
                            if (setting != null)
                                speakerValue = (setting.SettingValue == "True" ? true : false);
                            if (speakerValue == true)
                            {
                                Log.Info(TAG, "OnCallStateChanged: Speaker phone set to ON");
                                _audioManager.SpeakerphoneOn = true;
                            }
                            _previousCallState = CallState.Offhook;
                        }
                        break;
                    case CallState.Ringing:
                        Log.Info(TAG, "OnCallStateChanged: Phone is ringing");
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCallStateChanged: Exception - " + e.Message);
                if (_activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorOnCallStateChange), "EmergencyPhoneStateListener.OnCallStateChanged");
            }
        }
    }
}