using System;
using System.Collections.Generic;

using Android.App;
using Android.Runtime;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Java.Util;
using UniversalImageLoader.Core;
using Android.Content;
using Java.Lang;
using com.spanyardie.MindYourMood.Model.LowLevel;

namespace com.spanyardie.MindYourMood
{
    [Application]
    public class GlobalData : Application
    {
        public static string TAG = "M:GlobalData";
        public static ThoughtRecord ThoughtRecordItem { get; set; }
        public static Situation SituationItem { get; set; }
        public static List<Mood> MoodItems { get; set; }
        public static List<MoodList> MoodListItems { get; set; }
        public static string[] MoodListItemsForList { get; set; }
        public static List<AutomaticThoughts> AutomaticThoughtsItems { get; set; }
        public static List<EvidenceForHotThought> EvidenceForHotThoughtItems { get; set; }
        public static List<EvidenceAgainstHotThought> EvidenceAgainstHotThoughtItems { get; set; }
        public static List<AlternativeThoughts> AlternativeThoughtsItems { get; set; }
        public static List<RerateMood> RerateMoodsItems { get; set; }
        public static List<ThoughtRecord> ThoughtRecordsItems { get; set; }

        public static long ThoughtRecordId { get; set; }

        public static string[] AchievementChartTypes;
        public static string[] MoodlistArray;
        public static int[] ColourList;
        public static List<EmergencyNumber> EmergencyNumberList;

        public static List<AchievementChart> AchievementChartItems { get; set; }

        //Users list of contacts for calling in an emergency
        public static List<Contact> ContactsUserItems { get; set; }

        //cached list of all contacts
        public static List<Contact> ContactItems { get; set; }

        public static List<TellMyself> TellMyselfItemsList{ get; set; }
        public static List<GenericText> GenericTextItemsList { get; set; }
        public static List<SafetyPlanCard> SafetyPlanCardsItems { get; set; }

        public static List<Medication> MedicationItems { get; set; }

        public static Dictionary<string, string> _lookupTable;

        public static string CurrentIsoLanguageCode { get; set; }

        public static string CurrentIsoCountryCode { get; set; }

        public static List<Activities> ActivitiesForWeek { get; set; }

        public static List<Reactions> StructuredPlanReactions { get; set; }

        public static List<Feelings> StructuredPlanFeelings { get; set; }

        public static List<Attitudes> StructuredPlanAttitudes { get; set; }

        public static List<Relationships> StructuredPlanRelationships { get; set; }

        public static List<Health> StructuredPlanHealth { get; set; }

        public static List<Fantasies> StructuredPlanFantasies { get; set; }

        public static List<Problem> ProblemSolvingItems { get; set; }

        public static List<SolutionPlan> SolutionPlansItems { get; set; }

        public static List<Affirmation> AffirmationListItems { get; set; }

        public static List<Imagery> ImageListItems { get; set; }

        public static int MyLastSelectedImageIndex { get; set; }

        public static List<PlayList> PlayListItems { get; set; }

        public static DisplayImageOptions ImageOptions { get; set; }

        ImageLoaderConfiguration ImageLoaderConfig;

        public static string[,] MedicationTypes;
        public static string[,] MedicationList;

        public static string[] AppointmentTypes { get; set; }

        public static List<Appointments> Appointments { get; set; }
        public static ExtendedPopupWindow AppointmentPopupWindow = null;

        public static List<ResourceMedicationType> ResourceMedicationTypes { get; set; }

        public static List<ResourceCondition> ResourceConditions { get; set; }

        public static List<Setting> Settings { get; set; }

        public static SettingEmergency EmergencyLocale { get; set; }

        public static bool ShowErrorDialog { get; set; }

        public static List<PermissionBase> ApplicationPermissions { get; set; }

        public GlobalData(IntPtr handle, JniHandleOwnership transfer)
            : base (handle, transfer)
        {
            Log.Info(TAG, "Constructor: Application startup");
        }

        ~GlobalData()
        {
            Log.Info(TAG, "Destructor: Application Exit");
        }

        public override void OnCreate()
        {
            base.OnCreate();

            try
            {
                SetupDataFields();

                ImageOptions = new DisplayImageOptions.Builder()
                    .CacheInMemory(true)
                    .CacheOnDisk(true)
                    .ResetViewBeforeLoading()
                    .ImageScaleType(UniversalImageLoader.Core.Assist.ImageScaleType.Exactly)
                    .BitmapConfig(Android.Graphics.Bitmap.Config.Rgb565)
                    .Build();
                Log.Info(TAG, "OnCreate: Set up default options for Image Loader - cached in memory and cached on disk both true");

                ImageLoaderConfig = new ImageLoaderConfiguration.Builder(Context)
                    .DefaultDisplayImageOptions(ImageOptions)
                    .Build();
                ImageLoader.Instance.Init(ImageLoaderConfig);
                Log.Info(TAG, "OnCreate: Initialised Image Loader with default options set");

                CurrentIsoLanguageCode = Locale.Default.ISO3Language;
                CurrentIsoCountryCode = Locale.Default.ISO3Country;
                Log.Info(TAG, "OnCreate: Determined Iso Language code - " + CurrentIsoLanguageCode);
                Log.Info(TAG, "OnCreate: Determined Iso Country code - " + CurrentIsoCountryCode);

                //grab current application permissions
                ApplicationPermissions = PermissionsHelper.SetupDefaultPermissionList(this);

                //******* thread task start
                Log.Info(TAG, "OnCreate: Initiating runnable thread...");
                new Thread(new Runnable
                        (
                            () =>
                            {
                                Log.Info(TAG, "Runnable: Starting task ThreadTaskInitialisation");
                                ThreadTaskInitialisation(BaseContext);
                                Log.Info(TAG, "Runnable: Finished runnable task");
                            }
                        )
                    ).Start();

                Log.Info(TAG, "OnCreate: In Main UI thread following initialisation of runnable task");
                // thread task end
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "OnCreate: Error during GlobalData creation - " + e.Message);
            }
        }

        public static void SetupDataFields()
        {
            try
            {
                ThoughtRecordId = 0;
                ThoughtRecordItem = new ThoughtRecord();
                SituationItem = new Situation();
                MoodItems = new List<Mood>();
                MoodListItems = new List<MoodList>();
                AutomaticThoughtsItems = new List<AutomaticThoughts>();
                EvidenceForHotThoughtItems = new List<EvidenceForHotThought>();
                EvidenceAgainstHotThoughtItems = new List<EvidenceAgainstHotThought>();
                AlternativeThoughtsItems = new List<AlternativeThoughts>();
                RerateMoodsItems = new List<RerateMood>();
                ThoughtRecordsItems = new List<ThoughtRecord>();

                Log.Info(TAG, "Exiting SetupDataFields with ThoughtRecordId = " + ThoughtRecordId.ToString());
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "SetupDataFields: Error occurred initialising default data objects - " + e.Message);
            }

        }

        public static void RemoveThoughtRecord()
        {
            try
            {
                Log.Info(TAG, "RemoveThoughtRecord: ThoughtRecordId - " + ThoughtRecordId.ToString());
                if (ThoughtRecordId > 0)
                {
                    Log.Info(TAG, "RemoveThoughtRecord: Id > 0, proceeding to remove");
                    var dbHelp = new Globals();
                    dbHelp.OpenDatabase();
                    ThoughtRecordItem.RemoveThisThoughtRecord(dbHelp.GetSQLiteDatabase());
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RemoveThoughtRecord: Reset ThoughtRecordItem");
                    ThoughtRecordId = 0;
                    ThoughtRecordItem = new ThoughtRecord(); //empty one for next use
                }
                if (ThoughtRecordId == 0)
                    Log.Info(TAG, "RemoveThoughtRecord: Skipped Removal, ID is zero!");
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveThoughtRecord: Error removing Thought Record with ID " + ThoughtRecordId.ToString() + " - " + e.Message);
            }
        }

        public static void RemoveSituation()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    Log.Info(TAG, "Situation with ID " + SituationItem.SituationId.ToString() + " about to be removed...");
                    if (SituationItem.SituationId != 0)
                        SituationItem.Remove(sqlDatabase);
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveSituation: Error occurred removing Situation - " + e.Message);
            }
        }

        public static void RemoveMoods()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var mood in MoodItems)
                    {
                        if (mood.MoodsId != 0)
                        {
                            Log.Info(TAG, "RemoveMoods: Removing Mood with ID " + mood.MoodsId.ToString() + ", for Thought Record with ID " + mood.ThoughtRecordId.ToString());
                            mood.Remove(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveMoods: Error occurred removing Moods - " + e.Message);
            }
        }

        public static void RemoveAutomaticThoughts()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in AutomaticThoughtsItems)
                    {
                        if (thought.AutomaticThoughtsId != 0)
                        {
                            Log.Info(TAG, "RemoveAutomaticThoughts: Removing Automatic Thought with ID " + thought.AutomaticThoughtsId.ToString() + ", for Thought Record with ID " + thought.ThoughtRecordId);
                            thought.Remove(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveAutomaticThoughts: Error occurred removing Automatic Thoughts - " + e.Message);
            }
        }

        public static void RemoveEvidenceForHotThought()
        {
            Globals dbHelp = new Globals();
            dbHelp.OpenDatabase();
            var sqlDatabase = dbHelp.GetSQLiteDatabase();
            if (sqlDatabase != null)
            {
                foreach (var thought in EvidenceForHotThoughtItems)
                {
                    if (thought.AutomaticThoughtsId != 0)
                    {
                        Log.Info(TAG, "RemoveEvidenceForHotThought: Removing Evidence for Hot Thought with ID " + thought.EvidenceForHotThoughtId.ToString() + ", for Thought Record with ID " + thought.ThoughtRecordId.ToString());
                        thought.Remove(sqlDatabase);
                    }
                }
            }
            dbHelp.CloseDatabase();
        }

        public static void RemoveEvidenceAgainstHotThought()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in EvidenceAgainstHotThoughtItems)
                    {
                        if (thought.EvidenceAgainstHotThoughtId != 0)
                        {
                            Log.Info(TAG, "RemoveEvidenceAgainstHotThought: Removing Evidence Against Hot Thought with ID " + thought.AutomaticThoughtsId.ToString() + ", for Thought Record with ID " + thought.ThoughtRecordId.ToString());
                            thought.Remove(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveEvidenceAgainstHotThought: Error occurred removing Evidence Against Hot Thought - " + e.Message);
            }
        }

        public static void RemoveAlternativeThoughts()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in AlternativeThoughtsItems)
                    {
                        if (thought.AlternativeThoughtsId != 0)
                        {
                            Log.Info(TAG, "RemoveAlternativeThoughts: Removing Alternative Thought with ID " + thought.AlternativeThoughtsId.ToString() + ", for Thought Record with ID " + thought.ThoughtRecordId.ToString());
                            thought.Remove(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveAlternativeThoughts: Error occurred removing Alternative Thoughts - " + e.Message);
            }
        }

        public static void RemoveReratedMoods()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    foreach (var thought in RerateMoodsItems)
                    {
                        if (thought.RerateMoodId != 0)
                        {
                            Log.Info(TAG, "RemoveReratedMoods: Removing Rerated Mood with ID " + thought.MoodsId.ToString() + ", for Thought Record with ID " + thought.ThoughtRecordId.ToString());
                            thought.Remove(sqlDatabase);
                        }
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "RemoveReratedMoods: Error occurred removing Rerated Moods - " + e.Message);
            }
        }

        public static void GetThoughtRecordsForDate(DateTime theDate)
        {
            try
            {
                Log.Info(TAG, "Getting thought records for date " + theDate.ToShortDateString());
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    if (sqlDatabase.IsOpen)
                    {
                        dbHelp.GetAllThoughtRecordsForDate(theDate);
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetThoughtRecordsForDate: Error occurred getting Thought Records for date (" + theDate.ToShortDateString() + ") - " + e.Message);
            }
        }

        public static void GetAppointmentsForDate(DateTime theDate)
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    if (sqlDatabase.IsOpen)
                    {
                        dbHelp.GetAllAppointmentsForDate(theDate);
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetAppointmentsForDate: Error occurred getting Appointment items for date (" + theDate.ToShortDateString() + " - " + e.Message);
            }
        }

        public static void GetAchievementChartItemsForDate(DateTime theDate)
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                var sqlDatabase = dbHelp.GetSQLiteDatabase();
                if (sqlDatabase != null)
                {
                    if (sqlDatabase.IsOpen)
                    {
                        dbHelp.GetAllAchievementChartItemsForDate(theDate);
                    }
                }
                dbHelp.CloseDatabase();
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetAchievementChartItemsForDate: Error occurred getting Achievement Chart items for date (" + theDate.ToShortDateString() + " - " + e.Message);
            }
        }

        public static void GetEmergencyNumbersForCountry(string country, out string policeNumber, out string ambulanceNumber, out string fireNumber, out string notes)
        {
            policeNumber = "";
            ambulanceNumber = "";
            fireNumber = "";
            notes = "";

            try
            {
                var eNumbers = EmergencyNumberList.Find(num => num.CountryName == country.Trim());
                if (eNumbers != null)
                {
                    policeNumber = eNumbers.PoliceNumber.Trim();
                    ambulanceNumber = eNumbers.AmbulanceNumber.Trim();
                    fireNumber = eNumbers.FireNumber.Trim();
                    notes = eNumbers.Notes.Trim();
                }
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "GetEmergencyNumbersForCountry: Exception - " + e.Message);
            }
        }

        public static string[] GetEmergencyNumberCountriesStringArray()
        {
            string[] countryList = null;
            try
            {
                countryList = new string[EmergencyNumberList.Count];
                for(var country = 0; country < EmergencyNumberList.Count; country++)
                {
                    countryList[country] = EmergencyNumberList[country].CountryName.Trim();
                }
            }
            catch(System.Exception e)
            {
                Log.Error(TAG, "GetEmergencyNumberCountriesStringArray: Exception - " + e.Message);
            }

            return countryList;
        }

        public static int GetIndexOfCountry(string locale)
        {
            int item = 0;

            string[] countries = GetEmergencyNumberCountriesStringArray();
            if(countries != null && countries.Length > 0)
            {
                for(var index = 0; index < countries.Length; index++)
                {
                    if(countries[index].Trim() == locale.Trim())
                    {
                        item = index;
                        break;
                    }
                }
            }
            return item;
        }

        public static void GetAllApplicationSettings()
        {
            Globals dbHelp = new Globals();
            Setting setting;
            try
            {
                dbHelp.OpenDatabase();
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.GetAllSettings();
                    dbHelp.CloseDatabase();
                }

                string language = Locale.Default.Language.ToLower();
                string theValue = "";

                if (Settings.Count == 0)
                {
                    //create default settings
                    setting = new Setting();
                    setting.SettingKey = "ShowHelpNow";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "ConfirmationAudio";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "AlarmNotificationType";
                    setting.SettingValue = "-1";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "ShowErrorDialog";
                    setting.SettingValue = "True";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "NagContacts";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "NagMic";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "NagSendSms";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "NagMakeCalls";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "NagReadExternalStorage";
                    setting.SettingValue = "False";
                    setting.SaveSetting();

                    setting = new Setting();
                    setting.SettingKey = "EmergencyCallSpeaker";
                    setting.SettingValue = "True";
                    setting.SaveSetting();

                    theValue = "I need your help";
                    if (language == "spa")
                        theValue = "Necesito tu ayuda";

                    setting = new Setting();
                    setting.SettingKey = "EmergencyEmailSubject";
                    setting.SettingValue = theValue;
                    setting.SaveSetting();

                    theValue = "Please can you contact me, I am feeling very vulnerable and I need your help";
                    if (language == "spa")
                        theValue = "Por favor, puede ponerse en contacto conmigo, me siento muy vulnerable y necesito su ayuda";

                    setting = new Setting();
                    setting.SettingKey = "EmergencyEmailBody";
                    setting.SettingValue = theValue;
                    setting.SaveSetting();

                    theValue = "Please can you contact me, I am feeling vulnerable and I need your help";
                    if (language == "spa")
                        theValue = "Por favor, puede ponerse en contacto conmigo, me siento muy vulnerable y necesito su ayuda";

                    setting = new Setting();
                    setting.SettingKey = "EmergencySms";
                    setting.SettingValue = theValue;
                    setting.SaveSetting();

                }
                if (Settings.Find(set => set.SettingKey == "ShowHelpNow") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "ShowHelpNow";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "ConfirmationAudio") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "ConfirmationAudio";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "AlarmNotificationType") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "AlarmNotificationType";
                    setting.SettingValue = "-1";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "ShowErrorDialog") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "ShowErrorDialog";
                    setting.SettingValue = "True";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "NagContacts") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "NagContacts";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "NagMic") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "NagMic";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "NagSendSms") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "NagSendSms";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "NagMakeCalls") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "NagMakeCalls";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "NagReadExternalStorage") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "NagReadExternalStorage";
                    setting.SettingValue = "False";
                    setting.SaveSetting();
                }
                if (Settings.Find(set => set.SettingKey == "EmergencyCallSpeaker") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "EmergencyCallSpeaker";
                    setting.SettingValue = "True";
                    setting.SaveSetting();
                }

                theValue = "I need your help";
                if (language == "spa")
                    theValue = "Necesito tu ayuda";
                if (Settings.Find(set => set.SettingKey == "EmergencyEmailSubject") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "EmergencyEmailSubject";
                    setting.SettingValue = theValue;
                    setting.SaveSetting();
                }

                theValue = "Please can you contact me, I am feeling very vulnerable and I need your help";
                if (language == "spa")
                    theValue = "Por favor, puede ponerse en contacto conmigo, me siento muy vulnerable y necesito su ayuda";
                if (Settings.Find(set => set.SettingKey == "EmergencyEmailBody") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "EmergencyEmailBody";
                    setting.SettingValue = theValue;
                    setting.SaveSetting();
                }

                theValue = "Please can you contact me, I am feeling vulnerable and I need your help";
                if (language == "spa")
                    theValue = "Por favor, puede ponerse en contacto conmigo, me siento vulnerable y necesito su ayuda";
                if (Settings.Find(set => set.SettingKey == "EmergencySms") == null)
                {
                    setting = new Setting();
                    setting.SettingKey = "EmergencySms";
                    setting.SettingValue = theValue;
                    setting.SaveSetting();
                }
            }
            catch (System.Exception e)
            {
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.CloseDatabase();
                }
                Log.Error(TAG, "GetAllApplicationSettings: Exception - " + e.Message);
            }
        }

        public static void ThreadTaskInitialisation(Context baseContext)
        {
            Initialisation init = new Initialisation();

            Log.Info(TAG, "ThreadTaskInitialisation: Initialising MoodList array...");
            init.CreateMoodListArray(Context);

            Log.Info(TAG, "ThreadTaskInitialisation: Forcing first time use of Database");
            Globals dbHelp = new Globals();
            dbHelp.OpenDatabase();
            dbHelp.CloseDatabase();

            //early setup for settings
            GetAllApplicationSettings();

            Log.Info(TAG, "ThreadTaskInitialisation: Initialising MoodList...");
            if (MoodlistArray.Length > 0)
            {
                //does the length match what the constant says it should be
                if (MoodlistArray.Length != ConstantsAndTypes.MAX_NUMBER_OF_MOODLIST_ITEMS)
                {
                    //if there is a difference in length then recreate the array
                    init.CreateMoodListArray(Context);
                    Log.Info(TAG, "ThreadTaskInitialisation: Recreating Mood List array because array length (" + MoodlistArray.Length.ToString() + ") is not the same as the defined constant (" + ConstantsAndTypes.MAX_NUMBER_OF_MOODLIST_ITEMS.ToString() + ")");
                }
                else
                {
                    Log.Info(TAG, "ThreadTaskInitialisation: MoodList array has a length that agrees with the defined constant (" + ConstantsAndTypes.MAX_NUMBER_OF_MOODLIST_ITEMS.ToString() + ")");
                }
            }
            else
            {
                //we need to recreate it anyway
                init.CreateMoodListArray(Context);
                Log.Info(TAG, "ThreadTaskInitialisation: Creating Mood List array from scratch");
            }
            Log.Info(TAG, "ThreadTaskInitialisation: Creating Mood List...");
            init.CreateMoodList();
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Achievement Chart Types...");
            init.CreateAchievementChartTypes(Context);
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Colour List...");
            init.CreateColourList();
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Medication Types...");
            init.CreateMedicationTypes(baseContext);
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Medication List...");
            init.CreateMedicationList(baseContext);

            Log.Info(TAG, "ThreadTaskInitialisation: Building list of Contacts...");
            //Cannot do this if the user has not given permission yet!
            if (PermissionsHelper.HasPermission(Context, ConstantsAndTypes.AppPermission.ReadContacts)
                && PermissionsHelper.PermissionGranted(Context, ConstantsAndTypes.AppPermission.ReadContacts))
            {
                ContactItems = init.RetrieveAllContacts(baseContext);
            }

            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Emergency Contacts...");
            init.RetrieveEmergencyContacts(baseContext);
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Tell Myself items...");
            init.RetrieveAllTellMyselfEntries();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Generic Text items...");
            init.RetrieveAllGenericTextEntries();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Safety Plan Card items...");
            init.RetrieveAllSafetyPlanCardEntries();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Medication list items...");
            init.RetrieveMedicationList();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Activity list for current week...");
            init.RetrieveActivitesForCurrentWeek();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Reactions List...");
            init.RetrieveAllReactions();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Feelings List...");
            init.RetrieveAllFeelings();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Attitudes List...");
            init.RetrieveAllAttitudes();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Relationships List...");
            init.RetrieveAllRelationships();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Health List...");
            init.RetrieveAllHealth();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Fantasies List...");
            init.RetrieveAllFantasies();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Problems List...");
            init.RetrieveAllProblems();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Solution Plan List...");
            init.RetrieveAllSolutionPlans();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Affirmation List...");
            init.RetrieveAllAffirmations();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving Images List...");
            init.RetrieveAllImages();
            Log.Info(TAG, "ThreadTaskInitialisation: Retrieving PlayLists List...");
            init.RetrieveAllPlayLists();
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Appointment Types");
            init.CreateAppointmentTypes(Context);
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Resource Medication Types");
            init.RetrieveAllResourceMedicationTypes();
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Resource Conditions");
            init.RetrieveAllResourceConditions();
            Log.Info(TAG, "ThreadTaskInitialisation: Initialising Settings");
            init.RetrieveAllSettings();
            init.GetErrorAlertShowSetting();

            AchievementChartItems = new List<AchievementChart>();
        }

        public static Context GetApplicationContext()
        {
            return Context;
        }
    }
}