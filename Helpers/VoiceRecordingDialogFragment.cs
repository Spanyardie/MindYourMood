using System;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Media;

using V7Sup = Android.Support.V7.App;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Content;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class VoiceRecordingDialogFragment : DialogFragment, IAlertCallback
    {
        public const string TAG = "M:VoiceRecordingDialogFragment";

        /// <summary>
        /// Calling classes MUST implement
        /// RecordingCompleted(string path, string title)
        /// RecordingCancelled()
        /// </summary>

        private TextView _recordAudioLabel;
        private Button _recordAudioButton;
        private Button _backButton;
        private Button _addButton;
        private EditText _titleText;

        private Activity _parentActivity;

        private MediaRecorder _mediaRecorder;

        private string _filePath;
        private string _title;

        private bool _isRecording;
        private bool _hasRecorded;

        public VoiceRecordingDialogFragment() { }

        private string _dialogTitle = "";

        public VoiceRecordingDialogFragment(Activity activity, string title)
        {
            _parentActivity = activity;
            _isRecording = false;
            _hasRecorded = false;
            _title = "";
            _dialogTitle = title;
            _filePath = "";
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
            {
                outState.PutString("filePath", _filePath);
                outState.PutString("title", _title);
                outState.PutBoolean("isRecording", _isRecording);
                outState.PutBoolean("hasRecorded", _hasRecorded);
                outState.PutString("dialogTitle", _dialogTitle);
            }

            base.OnSaveInstanceState(outState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (context != null)
                _parentActivity = (Activity)context;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.Normal, Resource.Style.fullscreendialog_style);
        }

        public override void OnStart()
        {
            base.OnStart();
            if (Dialog != null)
            {
                int width = ViewGroup.LayoutParams.MatchParent;
                int height = ViewGroup.LayoutParams.MatchParent;
                if (Dialog.Window != null)
                {
                    Dialog.Window.SetLayout(width, height);
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (savedInstanceState != null)
                {
                    _filePath = savedInstanceState.GetString("filePath");
                    _title = savedInstanceState.GetString("title");
                    _isRecording = savedInstanceState.GetBoolean("isRecording");
                    _hasRecorded = savedInstanceState.GetBoolean("hasRecorded");
                    _dialogTitle = savedInstanceState.GetString("dialogTitle");
                }

                if (Dialog != null)
                    Dialog.SetTitle(_dialogTitle);

                var view = inflater.Inflate(Resource.Layout.VoiceRecordingDialogFragmentLayout, container, false);

                GetFieldComponents(view);

                CheckMicPermission();

                _mediaRecorder = new MediaRecorder();

                SetupCallbacks();

                return view;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreateView: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorCreatingVoiceRecordingDialog), "VoiceRecordingDialogFragment.OnCreateView");
                return null;
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_recordAudioButton != null)
                    _recordAudioButton.Click += RecordAudioButton_Click;
                if (_addButton != null)
                    _addButton.Click += AddButton_Click;
                if (_backButton != null)
                    _backButton.Click += BackButton_Click;
                if(_mediaRecorder != null)
                    _mediaRecorder.Info += MediaRecorder_Info;
                Log.Info(TAG, "SetupCallbacks: Completed successfully");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSettingVoiceRecordingCallbacks), "VoiceRecordingDialogFragment.SetupCallbacks");
            }
        }

        private void MediaRecorder_Info(object sender, MediaRecorder.InfoEventArgs e)
        {
            Log.Info(TAG, "MediaRecorder_Info: " + e.What.ToString());
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isRecording)
                {
                    if (_hasRecorded)
                    {
                        AlertHelper alertHelper = new AlertHelper(Activity);
                        alertHelper.AlertTitle = GetString(Resource.String.recordAudioCancelTitle);
                        alertHelper.AlertMessage = GetString(Resource.String.recordAudioCancelQuestion);
                        alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                        alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                        alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                        alertHelper.InstanceId = "cancelRecord";
                        alertHelper.ShowAlert();
                    }
                    else
                    {
                        ((IVoiceRecordCallback)Activity).RecordingCancelled(_filePath.Trim());
                        Dismiss();
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "BackButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorReturningFromVoiceRecordingDialog), "VoiceRecordingDialogFragment.BackButton_Click");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isRecording)
                {
                    if (Activity != null)
                    {
                        if (_hasRecorded)
                        {
                            if (_title != null)
                            {
                                if (string.IsNullOrEmpty(_titleText.Text.Trim()))
                                {
                                    AlertHelper alertHelper = new AlertHelper(Activity);
                                    alertHelper.AlertTitle = GetString(Resource.String.recordAudioSaveMissingTitle);
                                    alertHelper.AlertMessage = GetString(Resource.String.recordAudioSaveTitleQuestion);
                                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                                    alertHelper.AlertPositiveCaption = GetString(Resource.String.recordAudioSaveMissingTitleAbort);
                                    alertHelper.AlertNegativeCaption = GetString(Resource.String.recordAudioSaveMissingTitleEnter);
                                    alertHelper.InstanceId = "cancelRecord";
                                    alertHelper.ShowAlert();
                                }
                                else
                                {
                                    if (_titleText != null)
                                        _title = _titleText.Text.Trim();
                                    if (Activity != null)
                                    {
                                        IVoiceRecordCallback parentCallBack = (IVoiceRecordCallback)Activity;
                                        if (parentCallBack != null)
                                        {
                                            Log.Info(TAG, "AddButton_Click: Recording completed, calling parent...");
                                            parentCallBack.RecordingCompleted(_filePath.Trim(), _title.Trim());
                                            Dismiss();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AddButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAddingVoiceRecording), "VoiceRecordingDialogFragment.AddButton_Click");
            }
        }

        private void RecordAudioButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (_recordAudioLabel.Text == GetString(Resource.String.recordAudioLabelReady))
                {
                    if(_recordAudioButton != null)
                        _recordAudioButton.Text = GetString(Resource.String.recordAudioButtonStop);

                    if (_recordAudioLabel != null)
                        _recordAudioLabel.Text = GetString(Resource.String.recordAudioLabelRecording);


                    //recorder.Stop();

                    // file path
                    string path = Activity.BaseContext.FilesDir.AbsolutePath;
                    if (!string.IsNullOrEmpty(path))
                    {
                        _filePath = path + "/voice_" + DateTime.Now.Day.ToString() +
                                                                    "_" + DateTime.Now.Month.ToString() +
                                                                    "_" + DateTime.Now.Year.ToString() +
                                                                    "_" + DateTime.Now.Hour.ToString() +
                                                                    "_" + DateTime.Now.Minute.ToString() +
                                                                    "_" + DateTime.Now.Second.ToString() +
                                                                    "_" + DateTime.Now.Millisecond.ToString() + ".3gpp";
                        Log.Info(TAG, "RecordAudioButton_Click: Full file and Path - '" + _filePath.Trim() + "'");

                        _isRecording = true;

                        Log.Info(TAG, "RecordAudioButton_Click: Recording Audio...");
                        SetControlsOnRecordingStateChange();
                        _mediaRecorder.SetAudioSource(AudioSource.Mic);
                        _mediaRecorder.SetOutputFormat(OutputFormat.ThreeGpp);
                        _mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
                        _mediaRecorder.SetOutputFile(_filePath.Trim());
                        _mediaRecorder.Prepare();
                        _mediaRecorder.Start();
                        Log.Info(TAG, "RecordAudioButton_Click: Start called on MediaRecorder...");
                    }
                    return;
                }

                if(_recordAudioLabel.Text == GetString(Resource.String.recordAudioLabelRecording))
                {
                    if (_recordAudioLabel != null)
                        _recordAudioLabel.Text = GetString(Resource.String.recordAudioLabelReady);
                    if (_recordAudioButton != null)
                        _recordAudioButton.Text = GetString(Resource.String.recordAudioButtonRecord);
                    Log.Info(TAG, "RecordAudioButton_Click: Attempting to stop recording...");
                    _mediaRecorder.Stop();
                    _mediaRecorder.Reset();
                    _mediaRecorder.Release();
                    Log.Info(TAG, "RecordAudioButton_Click: Successfully stopped MediaRecorder");
                    _isRecording = false;
                    _hasRecorded = true;

                    SetControlsOnRecordingStateChange();
                }

            }
            catch(Exception ex)
            {
                Log.Error(TAG, "RecordAudioButton_Click: Exception - " + ex.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, ex, Activity.GetString(Resource.String.ErrorAddingVoiceRecording), "VoiceRecordingDialogFragment.RecordAudioButton_Click");
            }
        }

        private void SetControlsOnRecordingStateChange()
        {
            try
            {
                if (_isRecording)
                {
                    Log.Info(TAG, "SetControlsOnRecordingStateChange: Setting image greyscale as isRecording");
                    if (_backButton != null)
                    {
                        _backButton.Enabled = false;
                    }
                    if (_addButton != null)
                    {
                        _addButton.Enabled = false;
                    }
                }
                else
                {
                    Log.Info(TAG, "SetControlsOnRecordingStateChange: Setting normal image as not isRecording");
                    if (_backButton != null)
                    {
                        _backButton.Enabled = true;
                    }
                    if (_addButton != null)
                    {
                        _addButton.Enabled = true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetControlsOnRecordingStateChange: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorSettingVoiceRecordingControls), "VoiceRecordingDialogFragment.SetControlsOnRecordingStateChange");
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _recordAudioLabel = view.FindViewById<TextView>(Resource.Id.txtRecordAudioLabel);
                _recordAudioButton = view.FindViewById<Button>(Resource.Id.btnRecordAudio);
                _backButton = view.FindViewById<Button>(Resource.Id.btnCancelled);
                _addButton = view.FindViewById<Button>(Resource.Id.btnCompleted);
                _titleText = view.FindViewById<EditText>(Resource.Id.edtTitle);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if (Activity != null)
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(Activity, e, Activity.GetString(Resource.String.ErrorGettingVoiceRecordingComponents), "VoiceRecordingDialogFragment.GetFieldComponents");
            }
        }

        public override void Dismiss()
        {
            base.Dismiss();
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (Activity != null)
            {
                ((IVoiceRecordCallback)Activity).RecordingCancelled(_filePath.Trim());
            }
            Dismiss();
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {

        }

        private void CheckMicPermission()
        {
            try
            {
                if (!(PermissionsHelper.HasPermission(Activity, ConstantsAndTypes.AppPermission.UseMicrophone) && PermissionsHelper.PermissionGranted(Activity, ConstantsAndTypes.AppPermission.UseMicrophone)))
                {
                    Toast.MakeText(Activity, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                    if (_recordAudioButton != null)
                        _recordAudioButton.Enabled = false;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CheckMicPermission: Exception - " + e.Message);
                ErrorDisplay.ShowErrorAlert(Activity, e, "Checking Microphone permission", "VoiceRecordingDialogFragment.CheckMicPermission");
            }
        }
    }
}