using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using Android.Graphics;
using Android.Speech;
using com.spanyardie.MindYourMood.Model;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Views.InputMethods;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.SubActivities.Help.Resources;
using Android.Content.PM;

namespace com.spanyardie.MindYourMood.SubActivities.Resources
{
    [Activity(WindowSoftInputMode = SoftInput.StateHidden)]
    public class ResourcesAppointmentItemActivity : AppCompatActivity, ITimePickerCallback, IAlertCallback, IAnswerQuestion, IGenericTextCallback
    {
        public const string TAG = "M:ResourcesAppointmentItemActivity";

        private Spinner _appointmentTypes;
        private Toolbar _toolbar;
        private ImageButton _speakLocation;
        private ImageButton _speakWith;
        private ImageButton _speakNotes;
        private ImageButton _appointmentTime;
        private EditText _editLocation;
        private EditText _editWith;
        private EditText _editNotes;
        private TextView _appointmentDate;
        private ListView _questions;
        private AppointmentQuestionDialogFragment _question;

        private DateTime _dateOfAppointment = DateTime.Today;
        private TextView _timeOfAppointment;
        private DateTime _appointmentDateTime;

        private TextView _questionAnswer;

        private const int STATUS_NEW = 1;
        private const int STATUS_EDIT = 0;

        private int _status = STATUS_NEW;

        private int _appointmentID = -1;

        private Appointments _appointment = null;
        private int _selectedItemIndex = -1;

        private bool _isDirtyQuestions = false;
        private bool _blockReturnOnEmpty = false;

        private bool _answeringQuestion = false;

        private enum SPEAK_TYPE
        {
            None = -1,
            Location,
            With,
            Notes
        }

        private SPEAK_TYPE _currentSpeakType = SPEAK_TYPE.None;

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                SetContentView(Resource.Layout.ResourcesAppointmentActivityLayout);

                GetFieldComponents();

                CheckMicPermission();

                if (Intent != null)
                {
                    if (Intent.HasExtra("appointmentDate"))
                    {
                        if (_appointmentDate != null)
                            _appointmentDate.Text = Intent.GetStringExtra("appointmentDate");
                    }
                    if (Intent.HasExtra("appointmentStatus"))
                    {
                        _status = Intent.GetIntExtra("appointmentStatus", STATUS_NEW);
                    }
                    if (Intent.HasExtra("appointmentID"))
                    {
                        _appointmentID = Intent.GetIntExtra("appointmentID", -1);
                    }
                    if (Intent.HasExtra("appointmentTime"))
                    {
                        _appointmentDateTime = Convert.ToDateTime(Intent.GetStringExtra("appointmentTime"));
                        _timeOfAppointment.Text = _appointmentDateTime.ToShortTimeString();
                    }
                }
                else
                {
                    _dateOfAppointment = DateTime.Today;
                    if (_appointmentDate != null)
                        _appointmentDate.Text = _dateOfAppointment.ToLongDateString();
                    _status = STATUS_NEW;
                    _appointmentID = -1;
                }

                SetupCallbacks();

                SetupAppointmentTypeSpinner();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.appointmentToolbar, Resource.String.AppointmentSingleTitle, Color.White);

                if (_status == STATUS_EDIT)
                {
                    LoadAppointment();
                    DisplayAppointment();
                    UpdateAdapter();
                }
                else
                {
                    _appointment = new Appointments();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Creating Appointment item activity", "ResourcesAppointmentItemActivity.OnCreate");
            }
        }

        private void LoadAppointment()
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null)
                {
                    if(sqlDatabase.IsOpen)
                    {
                        _appointment = dbHelp.GetAppointmentByID(_appointmentID);           
                        sqlDatabase.Close();
                    }
                }
            }
            catch(Exception e)
            {
                if(sqlDatabase != null)
                {
                    if (sqlDatabase.IsOpen)
                        sqlDatabase.Close();
                }
                Log.Error(TAG, "LoadAppointment: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Loading Appointment", "ResourcesAppointmentItemActivity.LoadAppointment");
            }
        }

        private void DisplayAppointment()
        {
            try
            {
                if(_appointment != null)
                {
                    if (_editLocation != null)
                        _editLocation.Text = _appointment.Location;
                    if (_appointmentTypes != null)
                        _appointmentTypes.SetSelection(_appointment.AppointmentType);
                    if (_editWith != null)
                        _editWith.Text = _appointment.WithWhom;
                    if (_editNotes != null)
                        _editNotes.Text = _appointment.Notes;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "DisplayAppointment: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Displaying Appointment", "ResourcesAppointmentItemActivity.DisplayAppointment");
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AppointmentItemMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.AppointmentItemActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.AppointmentItemActionRemove);
                var itemSave = menu.FindItem(Resource.Id.AppointmentItemActionSave);
                var itemHelp = menu.FindItem(Resource.Id.AppointmentItemActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemSave != null)
                            itemSave.SetIcon(Resource.Drawable.ic_thumb_up_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemSave != null)
                            itemSave.SetIcon(Resource.Drawable.ic_thumb_up_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ResourcesAppointmentItemActivity.SetActionIcons");
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (item != null)
                {
                    if (GlobalData.AppointmentPopupWindow != null)
                    {
                        GlobalData.AppointmentPopupWindow.RequiresParentUpdate = false;
                        GlobalData.AppointmentPopupWindow.Dismiss();
                        GlobalData.AppointmentPopupWindow = null;
                    }
                    if (item.ItemId == Android.Resource.Id.Home)
                    {
                        HideSoftKeyboard();
                        if (_isDirtyQuestions)
                        {
                            var alert = new AlertHelper(this);
                            alert.AlertIconResourceID = Resource.Drawable.SymbolDelete;
                            alert.AlertMessage = "You have added questions for this appointment. Save before going back?";
                            alert.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                            alert.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                            alert.AlertTitle = "Save Appointment";
                            alert.InstanceId = "dirtyquestions";
                            alert.ShowAlert();
                        }
                        else
                        {
                            SetResult(Result.Canceled);
                            Finish();
                        }
                        return true;
                    }

                    switch (item.ItemId)
                    {
                        case Resource.Id.AppointmentItemActionSave:
                            SaveAppointment();
                            if (!_blockReturnOnEmpty)
                            {
                                SetResult(Result.Ok);
                                Finish();
                            }
                            return true;
                        case Resource.Id.AppointmentItemActionAdd:
                            AddQuestion();
                            return true;
                        case Resource.Id.AppointmentItemActionRemove:
                            RemoveQuestion();
                            return true;

                        case Resource.Id.AppointmentItemActionHelp:
                            Intent intent = new Intent(this, typeof(ResourcesAppointmentHelpActivity));
                            StartActivity(intent);
                            return true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnOptionItemSelected: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Selecting option", "ResourcesAppointmentItemActivity.OnOptionItemSelected");
            }
            return base.OnOptionsItemSelected(item);
        }

        private void HideSoftKeyboard()
        {
            InputMethodManager im = (InputMethodManager)GetSystemService(Context.InputMethodService);
            im.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
        }

        private void RemoveQuestion()
        {
            try
            {
                if (_selectedItemIndex != -1)
                {
                    var alert = new AlertHelper(this);
                    alert.AlertIconResourceID = Resource.Drawable.SymbolDelete;
                    alert.AlertMessage = GetString(Resource.String.AppointmentQuestionRemoveQuestion);
                    alert.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alert.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alert.AlertTitle = GetString(Resource.String.AppointmentQuestionRemoveAlertTitle);
                    alert.InstanceId = "remove";
                    alert.ShowAlert();
                }
                else
                {
                    if (_appointment.Questions.Count > 0)
                    {
                        Toast.MakeText(this, Resource.String.AppointmentQuestionRemoveSelectToast, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.AppointmentQuestionRemoveNoQuestionsToast, ToastLength.Short).Show();
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveQuestion: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.AppointmentQuestionRemoveAlertTitle), "ResourcesAppointmentItemActivity.RemoveQuestion");
            }
        }

        private void AddQuestion()
        {
            try
            {
                _question = new AppointmentQuestionDialogFragment(this, _appointmentID);

                var fragmentTransaction = FragmentManager.BeginTransaction();
                _question.Show(fragmentTransaction, _question.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AddQuestion: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorAddingAppointmentQuestion), "ResourcesAppointmentItemActivity.AddQuestion");
            }
        }

        private void SaveAppointment()
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                _blockReturnOnEmpty = false;

                if (_editLocation != null)
                {
                    if (_editLocation.Text.Trim() == "")
                    {
                        _editLocation.Error = GetString(Resource.String.AppointmentItemLocationMissing);
                        _blockReturnOnEmpty = true;
                        return;
                    }
                }
                if(_editWith != null)
                {
                    if(_editWith.Text.Trim() == "")
                    {
                        _editWith.Error = GetString(Resource.String.AppointmentItemWithMissing);
                        _blockReturnOnEmpty = true;
                        return;
                    }
                }

                if (_appointment == null)
                {
                    _appointment = new Appointments();
                }
                else
                {
                    if (_status == STATUS_NEW)
                    {
                        _appointment.IsNew = true;
                        _appointment.IsDirty = false;
                    }
                    else
                    {
                        _appointment.IsNew = false;
                        _appointment.IsDirty = true;
                    }
                }
                _appointment.AppointmentDate = Convert.ToDateTime(_appointmentDate.Text);
                _appointment.AppointmentTime = _appointmentDateTime;
                _appointment.AppointmentType = _appointmentTypes.SelectedItemPosition;
                _appointment.Location = _editLocation.Text.Trim();
                _appointment.WithWhom = _editWith.Text.Trim();
                if (_editNotes != null)
                {
                    _appointment.Notes = _editNotes.Text.Trim();
                }

                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                sqlDatabase = dbHelp.GetSQLiteDatabase();
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    _appointment.Save(sqlDatabase);
                    _appointment.IsDirty = false;
                    _appointment.IsNew = false;
                    _isDirtyQuestions = false;
                    sqlDatabase.Close();
                }

                HideSoftKeyboard();
            }
            catch(Exception e)
            {
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                Log.Error(TAG, "SaveAppointment: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Saving Appointment", "ResourcesAppointmentItemActivity.SaveAppointment");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_speakLocation != null)
                    _speakLocation.Click += SpeakLocation_Click;
                if (_speakWith != null)
                    _speakWith.Click += SpeakWith_Click;
                if (_speakNotes != null)
                    _speakNotes.Click += SpeakNotes_Click;
                if (_appointmentTime != null)
                    _appointmentTime.Click += AppointmentTime_Click;
                if (_questions != null)
                    _questions.ItemClick += Questions_ItemClick;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting up callbacks", "ResourcesAppointmentItemActivity.SetupCallbacks");
            }
        }

        private void Questions_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                _selectedItemIndex = e.Position;
                UpdateAdapter();
                _questions.SetSelection(_selectedItemIndex);

                //display any attached answer for this question
                if (_appointment.Questions != null && _appointment.Questions.Count > 0)
                {
                    var question = _appointment.Questions[_selectedItemIndex];
                    if (_questionAnswer != null)
                    {
                        _questionAnswer.Text = question.Answer.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Questions_ItemClick: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Processing question selection", "ResourcesAppointmentItemActivity.Questions_ItemClick");
            }
        }

        private void AppointmentTime_Click(object sender, EventArgs e)
        {
            try
            {
                TimePickerDialogFragment timeFragment = new TimePickerDialogFragment(this, this, DateTime.Now, ConstantsAndTypes.TIMEPICKER_CONTEXT.Appointment, "Select Appointment Time");
                var transaction = FragmentManager.BeginTransaction();
                timeFragment.Show(transaction, timeFragment.Tag);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "AppointmentTime_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.AppointmentItemTimeSelect), "ResourcesAppointmentItemActivity.AppointmentTime_Click");
            }
        }

        private void SpeakNotes_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AppointmentSpeakNotesPrompt));

                Log.Info(TAG, "SpeakNotes_Click: Created intent, sending request...");
                _currentSpeakType = SPEAK_TYPE.Notes;
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SpeakNotes_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Requesting voice recognition", "ResourcesAppointmentItemActivity.SpeakNotes_Click");
            }
        }

        private void SpeakWith_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AppointmentSpeakWithPrompt));

                Log.Info(TAG, "SpeakWith_Click: Created intent, sending request...");
                _currentSpeakType = SPEAK_TYPE.With;
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SpeakWith_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Requesting voice recognition", "ResourcesAppointmentItemActivity.SpeakWith_Click");
            }
        }

        private void SpeakLocation_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.AppointmentSpeakLocationPrompt));

                Log.Info(TAG, "SpeakLocation_Click: Created intent, sending request...");
                _currentSpeakType = SPEAK_TYPE.Location;
                StartActivityForResult(intent, ConstantsAndTypes.VOICE_RECOGNITION_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SpeakLocation_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Requesting voice recognition", "ResourcesAppointmentItemActivity.SpeakLocation_Click");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                if (requestCode == ConstantsAndTypes.VOICE_RECOGNITION_REQUEST && resultCode == Result.Ok)
                {
                    IList<string> matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);

                    if (matches != null)
                    {
                        switch (_currentSpeakType)
                        {
                            case SPEAK_TYPE.Location:
                                if (_editLocation != null)
                                    _editLocation.Text = matches[0];
                                break;
                            case SPEAK_TYPE.With:
                                if (_editWith != null)
                                    _editWith.Text = matches[0];
                                break;
                            case SPEAK_TYPE.Notes:
                                if (_editNotes != null)
                                    _editNotes.Text = matches[0];
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing voice recognition response", "ResourcesAppointmentItemActivity.OnActivityResult");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                _appointmentTypes = FindViewById<Spinner>(Resource.Id.spnAppointmentTypes);
                _speakLocation = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakLocation);
                _speakWith = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakWith);
                _speakNotes = FindViewById<ImageButton>(Resource.Id.imgbtnSpeakNotes);
                _editLocation = FindViewById<EditText>(Resource.Id.edtLocation);
                _editWith = FindViewById<EditText>(Resource.Id.edtWith);
                _editNotes = FindViewById<EditText>(Resource.Id.edtNotes);
                _appointmentDate = FindViewById<TextView>(Resource.Id.txtAppointmentDate);
                _appointmentTime = FindViewById<ImageButton>(Resource.Id.imgbtnAppointmentTime);
                _timeOfAppointment = FindViewById<TextView>(Resource.Id.txtAppointmentTime);
                _questions = FindViewById<ListView>(Resource.Id.lvwQuestions);
                _questionAnswer = FindViewById<TextView>(Resource.Id.txtQuestionAnswer);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Getting field components", "ResourcesAppointmentItemActivity.GetFieldComponents");
            }
        }

        public void QuestionAdded(int appointmentID, string question)
        {
            SQLiteDatabase sqlDatabase = null;

            try
            {
                if(question.Trim() != "")
                {
                    AppointmentQuestion newQuestion = new AppointmentQuestion();
                    newQuestion.AppointmentID = appointmentID;
                    newQuestion.Question = question.Trim();
                    newQuestion.IsNew = true;
                    _appointment.Questions.Add(newQuestion);
                    _isDirtyQuestions = true;
                    UpdateAdapter();
                }
            }
            catch(Exception e)
            {
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    sqlDatabase.Close();
                    sqlDatabase = null;
                }
                Log.Error(TAG, "QuestionAdded: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAddingAppointmentQuestion), "ResourcesAppointmentItemActivity.QuestionAdded");
            }
        }

        private void UpdateAdapter()
        {
            try
            {
                var adapter = new AppointmentQuestionListAdapter(this, _appointment.Questions);

                if (adapter != null)
                    _questions.Adapter = adapter;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAppointmentQuestionListAdapter), "ResourcesAppointmentItemActivity.UpdateAdapter");
            }
        }

        public void NoChanges()
        {
            Toast.MakeText(this, Resource.String.AppointmentQuestionNoChangesToast, ToastLength.Short).Show();
        }

        private void SetupAppointmentTypeSpinner()
        {
            if (_appointmentTypes != null)
            {
                try
                {
                    string[] types = GlobalData.AppointmentTypes;

                    ArrayAdapter adapter = new ArrayAdapter(this, Resource.Layout.SpinnerGeneral, types);
                    if (adapter != null && types.Length > 0)
                    {
                        adapter.SetDropDownViewResource(Resource.Layout.SpinnerGeneralDropdownItem);
                        _appointmentTypes.Adapter = adapter;
                        _appointmentTypes.SetSelection(0);
                        Log.Info(TAG, "SetupAppointmentTypeSpinner: Set Type adapter");
                    }
                    else
                    {
                        Log.Error(TAG, "SetupAppointmentTypeSpinner: Failed to create adapter");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "SetupAppointmentTypeSpinner: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorAppointmentTypesSpinner), "ResourcesAppointmentItemActivity.SetupAppointmentTypeSpinner");
                }
            }
            else
            {
                Log.Error(TAG, "SetupAppointmentTypeSpinner: _appointmentTypes is NULL!");
            }
        }

        public void TimePicked(DateTime timePicked, ConstantsAndTypes.TIMEPICKER_CONTEXT timeContext)
        {
            try
            {
                if (_timeOfAppointment != null)
                {
                    _timeOfAppointment.Text = timePicked.ToShortTimeString();
                    _appointmentDateTime = timePicked;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "TimePicked: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Processing time pick result", "ResourcesAppointmentItemActivity.TimePicked");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            SQLiteDatabase sqlDatabase = null;
            try
            {
                if (instanceId == "useMic")
                {
                    PermissionsHelper.RequestApplicationPermission(this, ConstantsAndTypes.AppPermission.UseMicrophone);
                    return;
                }

                if (instanceId == "remove")
                {
                    var question = _appointment.Questions[_selectedItemIndex];
                    if (question != null)
                    {
                        Globals dbHelp = new Globals();
                        dbHelp.OpenDatabase();
                        sqlDatabase = dbHelp.GetSQLiteDatabase();
                        if (sqlDatabase != null && sqlDatabase.IsOpen)
                        {
                            question.Remove(sqlDatabase);
                            _appointment.Questions.Remove(question);
                            UpdateAdapter();
                            sqlDatabase.Close();
                            sqlDatabase = null;
                            _selectedItemIndex = -1;
                        }
                    }
                }
                else
                {
                    SaveAppointment();
                    SetResult(Result.Ok);
                    Finish();
                }
            }
            catch(Exception ex)
            {
                if(sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    sqlDatabase.Close();
                    sqlDatabase = null;
                }
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.AppointmentQuestionRemoveAlertTitle), "ResourcesAppointmentItemActivity.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            if (instanceId == "useMic")
            {
                Toast.MakeText(this, Resource.String.MicrophonePermissionDenialToast, ToastLength.Short).Show();
                PermissionResultUpdate(Permission.Denied);
                return;
            }

            if (instanceId == "remove")
            {
                Toast.MakeText(this, GetString(Resource.String.AppointmentQuestionNoChangesToast), ToastLength.Short).Show();
            }
            else
            {
                SetResult(Result.Canceled);
                Finish();
            }
        }

        public void InitiateQuestionAnswer(int position)
        {
            if (_answeringQuestion)
            {
                Log.Info(TAG, "InitiateQuestionAnswer: Returning because already answering question");
                return;
            }

            try
            {
                Log.Info(TAG, "InitiateQuestionAnswer: _answeringQuestion - TRUE");
                _answeringQuestion = true;
                Log.Info(TAG, "InitiateQuestionAnswer: Creating GenericTextDialogFragment for answering question");
                var textDialog = new GenericTextDialogFragment(this, "Answer", "Please enter an answer to the question", _questionAnswer.Text.Trim(), position);
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                textDialog.Show(transaction, textDialog.Tag);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "InitiateQuestionAnswer: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Answering Question", "ResourcesAppointmentItemActivity.InitiateQuestionAnswer");
            }
        }

        public void ConfirmText(string textEntered, int genericTextID)
        {
            try
            {
                var question = _appointment.Questions[genericTextID];
                if(question != null)
                {
                    question.Answer = textEntered.Trim();
                    if(question.AppointmentID != -1)
                        question.IsDirty = true;
                    if (_questionAnswer != null)
                        _questionAnswer.Text = textEntered.Trim();
                    if(_status == STATUS_EDIT)
                        _appointment.IsDirty = true;
                    SaveAppointment();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ConfirmText: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Answering Question", "ResourcesAppointmentItemActivity.ConfirmText");
            }

            _answeringQuestion = false;
        }

        public void CancelText()
        {
            _answeringQuestion = false;
            Toast.MakeText(this, "Canceled answering question", ToastLength.Short).Show();
        }

        public void OnGenericDialogDismiss()
        {
            Log.Info(TAG, "OnGenericDialogDismiss: Called!");
            _answeringQuestion = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, "Checking Microphone permission", "ResourcesAppointmentItemActivity.CheckMicPermission");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Processing permission result", "ResourcesAppointmentItemActivity.OnRequestPermissionsResult");
            }
        }

        public void PermissionResultUpdate(Permission permission)
        {
            if (permission == Permission.Denied)
            {
                //find all the Mic image buttons and disable them
                _speakLocation.SetImageResource(Resource.Drawable.micgreyscale);
                _speakWith.SetImageResource(Resource.Drawable.micgreyscale);
                _speakNotes.SetImageResource(Resource.Drawable.micgreyscale);

                _speakLocation.Enabled = false;
                _speakWith.Enabled = false;
                _speakNotes.Enabled = false;
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
                ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorMicPermissionShowRationalAlert), "ResourcesAppointmentItemActivity.ShowPermissionRationale");
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
                ErrorDisplay.ShowErrorAlert(this, e, "Attempting permission request", "ResourcesAppointmentItemActivity.AttemptPermissionRequest");
            }
        }
    }
}