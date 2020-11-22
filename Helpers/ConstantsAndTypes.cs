using Android;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class ConstantsAndTypes
    {
        public const int REQUEST_CODE_PERMISSION_READ_CONTACTS = 1000;
        public const int REQUEST_CODE_PERMISSION_USE_MICROPHONE = 1005;
        public const int REQUEST_CODE_PERMISSION_WRITE_SMS = 1010;
        public const int REQUEST_CODE_PERMISSION_SEND_SMS = 1011;
        public const int REQUEST_CODE_PERMISSION_MAKE_CALLS = 1020;
        public const int REQUEST_CODE_PERMISSION_MODIFY_AUDIO_SETTINGS = 1030;
        public const int REQUEST_CODE_PERMISSION_READ_PHONE_STATE = 1035;
        public const int REQUEST_CODE_PERMISSION_READ_PROFILE = 1040;
        public const int REQUEST_CODE_PERMISSION_RECEIVE_BOOT_COMPLETED = 1045;
        public const int REQUEST_CODE_PERMISSION_SET_ALARM = 1050;
        public const int REQUEST_CODE_PERMISSION_WAKE_LOCK = 1055;
        public const int REQUEST_CODE_PERMISSION_READ_EXTERNAL_STORAGE = 1060;

        public const string READ_CONTACTS = Manifest.Permission.ReadContacts;
        public const string USE_MICROPHONE = Manifest.Permission.RecordAudio;
        public const string WRITE_SMS = Manifest.Permission.WriteSms;
        public const string SEND_SMS = Manifest.Permission.SendSms;
        public const string MAKE_CALLS = Manifest.Permission.CallPhone;
        public const string MODIFY_AUDIO_SETTINGS = Manifest.Permission.ModifyAudioSettings;
        public const string READ_PHONE_STATE = Manifest.Permission.ReadPhoneState;
        public const string READ_PROFILE = Manifest.Permission.ReadProfile;
        public const string RECEIVE_BOOT_COMPLETED = Manifest.Permission.ReceiveBootCompleted;
        public const string SET_ALARM = Manifest.Permission.SetAlarm;
        public const string WAKE_LOCK = Manifest.Permission.WakeLock;
        public const string READ_EXTERNAL_STORAGE = Manifest.Permission.ReadExternalStorage;

        public enum AppPermission
        {
            ReadContacts = 0,
            UseMicrophone,
            WriteSms,
            SendSms,
            MakeCalls,
            ModifyAudioSettings,
            ReadExternalStorage,
            ReadPhoneState,
            ReadProfile,
            ReceiveBootCompleted,
            SetAlarm,
            WakeLock
        }

        public enum ScreenSize
        {
            Small = 0,
            Normal,
            Large,
            ExtraLarge
        }

        public const int SPEAK_CALM = 4000;
        public const int SPEAK_TELL = 4001;
        public const int SPEAK_GO = 4002;
        public const int SPEAK_CALL = 4003;
        public const int SPEAK_TO_WHAT = 3000;
        public const int SPEAK_OF = 3001;
        public const int SPEAK_ACTION_OF = 3002;
        public const int SPEAK_ABOUT = 3003;
        public const int SPEAK_ASPECT = 3004;
        public const int SPEAK_WITH = 3005;

        public const int EDIT_RELATIONSHIP = 2600;
        public const int ADD_RELATIONSHIP = 2601;
        public const int EDIT_REACTIONS = 2500;
        public const int ADD_REACTIONS = 2501;
        public const int EDIT_HEALTH = 2400;
        public const int ADD_HEALTH = 2401;
        public const int EDIT_FEELING = 2300;
        public const int ADD_FEELING = 2301;
        public const int EDIT_FANTASY = 2200;
        public const int ADD_FANTASY = 2201;
        public const int EDIT_ATTITUDE = 2100;
        public const int ADD_ATTITUDE = 2101;

        public enum MedicationSpeakType
        {
            Name = 1000,
            DailyDose,
            MonthDay
        }

        public const int ADD_MEDICATION_REQUEST = 2000;
        public const int EDIT_MEDICATION_REQUEST = 2001;

        public enum SituationCategories
        {
            What = 0,
            Where,
            When,
            Who
        }

        public const int VOICE_RECOGNITION_REQUEST = 1234;

        public enum NumericComparator
        {
            LessThan = -1,
            EqualTo = 0,
            GreaterThan = 1
        }

        public enum NotificationCategories
        {
            Default = -1,
            Achievement = 0,
            Activity,
            Medication,
            Feelings,
            Reactions,
            Attitudes,
            Relationships,
            Health,
            Fantasy,
            Affirmation
        }

        public enum PROCON_TYPES
        {
            Pro = 0,
            Con
        }

        public enum ATTITUDE_TYPES
        {
            Optimism = 0,
            Pessimism,
            Confident,
            Interested,
            Independent,
            Jealous,
            Courteous,
            Cooperative,
            Considerate,
            Inferior,
            Happy,
            Frank,
            Respectful,
            Authoritative,
            Sincere,
            Persistent,
            Honest,
            Sympathetic,
            Realistic,
            Faithful,
            Flexible,
            Decisive,
            Trusting,
            Thoughtful,
            Determined,
            Loving,
            Hostile,
            Modest,
            Reliable,
            Tolerant,
            Humble,
            Cautious,
            Sarcastic,
            Helping,
            HardWorking
        }

        public enum RELATIONSHIP_TYPE
        {
            Mother = 0,
            Father,
            Sister,
            Brother,
            Son,
            Daughter,
            Aunt,
            Uncle,
            Grandmother,
            Grandfather,
            Wife,
            Husband,
            Partner,
            Niece,
            Nephew,
            Relative,
            Friend,
            Aquaintance,
            WorkColleague
        }

        public enum REACTION_TYPE
        {
            Positive = 0,
            Negative,
            Ambivalent
        }

        public enum ACTION_TYPE
        {
            DoMore = 0,
            DoLess,
            Maintain
        }

        public enum ACTIVITY_HOURS
        {
            SixAMToEightAM = 0,
            EightAMToTenAM,
            TenAMToTwelvePM,
            TwelvePMToTwoPM,
            TwoPMToFourPM,
            FourPMToSixPM,
            SixPMToEightPM,
            EightPMToTenPM,
            TenPMToTwelveAM
        }

        public enum ALARM_INTERVALS
        {
            EveryMinute = 60000,
            EveryFiveMinutes = 300000,
            EveryTenMinutes = 600000,
            QuarterHourly = 900000,
            HalfHourly = 1800000,
            Hourly = 3600000,
            HalfDay = 43200000,
            Daily = 86400000,
            Weekly = 604800000
        }

        public enum TELL_TYPE
        {
            Audio = 0,
            Textual
        }

        public enum GENERIC_TEXT_TYPE
        {
            StopSuicidalThoughts = 0,
            WarningSigns,
            MethodsOfCoping,
            KeepCalm,
            OthersCanDo,
            SafePlaces,
            MoodsAdjust
        }

        public enum HELP_NOW_TYPES
        {
            EmergencyTelephone = 0,
            EmergencyEmail,
            EmergencySms
        }

        public enum CONTACT_CONTEXT_MENU_ITEMS
        {
            ContactEmergencyCall = 0,
            ContactEmergencySms,
            ContactEmergencyEmail
        }

        public enum PRESCRIPTION_TYPE
        {
            Daily = 0,
            Weekly
        }

        public enum MEDICATION_TIME
        {
            Morning = 0,
            LunchTime,
            DinnerTime,
            Evening
        }

        public enum TIMEPICKER_CONTEXT
        {
            MedicationTime = 0,
            MedicationReminder,
            Appointment
        }

        public enum MEDICATION_FOOD
        {
            Before = 0,
            After,
            With,
            DoesntMatter
        }

        public enum DAYS_OF_THE_WEEK
        {
            Undefined = -1,
            Monday = 0,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        public const int DATABASE_VERSION = 2;

        public const string CREATE_ALTERNATIVETHOUGHTS_TABLE_V1 = @"CREATE TABLE [AlternativeThoughts] " +
                                                                        "([Alternative] TEXT NULL, " +
                                                                        "[AlternativeThoughtsID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                                        "[BeliefRating] INTEGER DEFAULT '0' NULL, " +
                                                                        "[ThoughtRecordId] INTEGER DEFAULT '0' NULL );";

        public const string CREATE_AUTOMATICTHOUGHTS_TABLE_V1 = @"CREATE TABLE [AutomaticThoughts] " +
                                                                    "([AutomaticThoughtsID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                                    "[HotThought] BOOLEAN NULL, " +
                                                                    "[Thought] TEXT NULL, " +
                                                                    "[ThoughtRecordID] INTEGER NULL);";

        public const string CREATE_ACHIEVEMENTCHART_TABLE_V1 = @"CREATE TABLE [ChuffChart] (" +
                                                            "[AchievementID] INTEGER PRIMARY KEY AUTOINCREMENT NULL, " +
                                                            "[Achievement] TEXT NULL, " +
                                                            "[ChuffChartType] INTEGER DEFAULT '0' NULL," +
                                                            "[AchievementDate] TEXT NULL);";

        public const string CREATE_EVIDENCEAGAINSTHOTTHOUGHT_TABLE_V1 = @"CREATE TABLE [EvidenceAgainstHotThought] " +
                                                                            "([EvidenceAgainstHotThoughtID] INTEGER PRIMARY KEY AUTOINCREMENT NULL, " +
                                                                            "[AutomaticThoughtsID] INTEGER NULL, " +
                                                                            "[Evidence] TEXT NULL, " +
                                                                            "[ThoughtRecordID] INTEGER NULL);";

        public const string CREATE_EVIDENCEFORHOTTHOUGHT_TABLE_V1 = @"CREATE TABLE [EvidenceForHotThought] " +
                                                                        "([EvidenceForHotThoughtID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                                        "[AutomaticThoughtsID] INTEGER NULL, " +
                                                                        "[Evidence] TEXT NULL, " +
                                                                        "[ThoughtRecordID] INTEGER NULL);";

        public const string CREATE_MOOD_TABLE_V1 = @"CREATE TABLE [Mood] " +
                                                        "([MoodsID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                        "[MoodListID] INTEGER NULL, " +
                                                        "[MoodRating] INTEGER NULL, " +
                                                        "[ThoughtRecordID] INTEGER NULL);";
        public const string CREATE_MOODLIST_TABLE_V1 = @"CREATE TABLE [MoodList] " +
                                                            "([MoodID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                            "[IsoCountry] VARCHAR(3) DEFAULT '''''' NULL, " +
                                                            "[MoodName] TEXT NULL, " +
                                                            "[IsDefault] VARCHAR(10) DEFAULT 'false' NULL)";

        public const string CREATE_RERATEMOOD_TABLE_V1 = "CREATE TABLE [ReRateMood] " +
                                                            "([RerateMoodID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                            "[MoodListID] INTEGER NULL, " +
                                                            "[MoodRating] INTEGER NULL, " +
                                                            "[MoodsID] INTEGER NULL, " +
                                                            "[ThoughtRecordID] INTEGER NULL);";

        public const string CREATE_SITUATION_TABLE_V1 = @"CREATE TABLE [Situation] " +
                                                            "([SituationID] INTEGER PRIMARY KEY AUTOINCREMENT NULL, " +
                                                            "[ThoughtRecordID] INTEGER NULL, " +
                                                            "[Who] TEXT NULL, " +
                                                            "[What] TEXT NULL, " +
                                                            "[When] TEXT NULL, " +
                                                            "[Where] TEXT NULL);";

        public const string CREATE_THOUGHTRECORD_TABLE_V1 = @"CREATE TABLE [ThoughtRecord] " +
                                                                "([ThoughtRecordID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                                                "[RecordDate] TIMESTAMP NOT NULL DEFAULT current_timestamp);";

        public const string CREATE_VIEW_COMPLETEMOODSRATING_V1 = @"CREATE VIEW [vwCompleteMoodsRating] AS " +
                                                                        "SELECT " +
                                                                            "tr.ThoughtRecordID, " +
                                                                            "tr.RecordDate, " +
                                                                            "Mood.MoodRating, " +
                                                                            "Mood.MoodListID, " +
                                                                            "ml.MoodName " +
                                                                        "FROM " +
                                                                            "ThoughtRecord tr " +
                                                                        "INNER JOIN " +
                                                                            "Mood " +
                                                                            "ON Mood.ThoughtRecordID = tr.ThoughtRecordID " +
                                                                        "INNER JOIN " +
                                                                            "MoodList ml " +
                                                                            "ON ml.MoodID = Mood.MoodListID;";

        public const string CREATE_VIEW_COMPLETERERATEMOODS_V1 = @"CREATE VIEW [vwCompleteRerateMoods] AS " +
                                                                    "SELECT " +
                                                                        "tr.ThoughtRecordID, " +
                                                                        "rm.MoodListID, " +
                                                                        "ml.MoodName, " +
                                                                        "rm.MoodRating " +
                                                                    "FROM " +
                                                                        "ThoughtRecord tr " +
                                                                    "INNER JOIN " +
                                                                        "RerateMood rm " +
                                                                        "ON rm.ThoughtRecordID = tr.ThoughtRecordID " +
                                                                    "INNER JOIN " +
                                                                        "MoodList ml " +
                                                                        "ON ml.MoodID = rm.MoodListID;";

        public const string CREATE_VIEW_MOODRATING_V1 = @"CREATE VIEW [vwMoodRating] AS  " +
                                                            "SELECT " +
                                                                "tr.ThoughtRecordID, " +
                                                                "tr.RecordDate, " +
                                                                "Mood.MoodRating, " +
                                                                "ml.MoodID, " +
                                                                "ml.MoodName " +
                                                            "FROM " +
                                                                "ThoughtRecord tr " +
                                                            "INNER JOIN " +
                                                                "Mood " +
                                                                "ON Mood.ThoughtRecordID = tr.ThoughtRecordID " +
                                                            "INNER JOIN " +
                                                                "MoodList ml " +
                                                                "ON ml.MoodID = Mood.MoodListID;";

        public const string CREATE_CONTACTS_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Contacts] (
                                                                    [ID] INTEGER  PRIMARY KEY AUTOINCREMENT NULL,
                                                                    [URI] INTEGER  UNIQUE NOT NULL,
                                                                    [ContactName] VARCHAR(50) DEFAULT '''''' NULL,
                                                                    [ContactTelephoneNumber] VARCHAR(50) DEFAULT '''''' NULL,
                                                                    [ContactEmail] VARCHAR(350) DEFAULT '' NULL,
                                                                    [ContactUseEmergencyCall] BOOLEAN DEFAULT 'false' NULL,
                                                                    [ContactUseEmergencySms] BOOLEAN DEFAULT 'false' NULL,
                                                                    [ContactUseEmergencyEmail] BOOLEAN DEFAULT 'false' NULL,
                                                                    [ContactPhotoId] VARCHAR(75) DEFAULT '''''' NULL);";

        public const string CREATE_TELLMYSELF_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [TellMyself] (
                                                                    [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [TellType] INTEGER DEFAULT '-1' NOT NULL,
                                                                    [TellText] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                    [TellTitle] VARCHAR(150) DEFAULT '''''' NOT NULL);";

        public const string CREATE_GENERICTEXT_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [GenericText] (
                                                                    [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [TextType] INTEGER DEFAULT '-1' NOT NULL,
                                                                    [TextValue] VARCHAR(4000) DEFAULT '''''' NOT NULL);";

        public const string CREATE_SAFETYPLANCARDS_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [SafetyPlanCards] (
                                                                        [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [CalmMyself] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                        [TellMyself] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                        [WillCall] VARCHAR(100) DEFAULT '''''' NOT NULL,
                                                                        [WillGoTo] VARCHAR(100) DEFAULT '''''' NOT NULL);";

        public const string CREATE_MEDICATION_REMINDER_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [MedicationReminder] (
                                                                            [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [MedicationSpreadID] INTEGER DEFAULT '0' NOT NULL,
                                                                            [MedicationDay] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [MedicationTime] VARCHAR(15) DEFAULT '''''' NOT NULL);";

        public const string CREATE_MEDICATION_TIME_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [MedicationTime] (
                                                                            [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [MedicationSpreadID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [MedicationTime] INTEGER DEFAULT '0' NOT NULL,
                                                                            [TakenTime] VARCHAR(15) DEFAULT '''''' NOT NULL);";

        public const string CREATE_PRESCRIPTION_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Prescription] (
                                                                        [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [MedicationID] INTEGER DEFAULT '-1' NOT NULL,
                                                                        [PrescriptionType] INTEGER DEFAULT '-1' NOT NULL,
                                                                        [WeeklyDay] INTEGER DEFAULT '-1' NOT NULL,
                                                                        [MonthlyDay] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_MEDICATION_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Medication] (
                                                                    [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [MedicationName] VARCHAR(50) DEFAULT '''''' NOT NULL,
                                                                    [TotalDailyDosage] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_MEDICATION_SPREAD_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [MedicationSpread] (
                                                                            [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [MedicationID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [Dosage] INTEGER DEFAULT '0' NOT NULL,
                                                                            [FoodRelevance] INTEGER DEFAULT '-1' NOT NULL);";


        public const string UPDATE_MOODLIST_TABLE_ENTER_GBR_CODES_VERSION_1 = @"UPDATE [MoodList] SET [IsoCountry] = 'GBR';";

        public const string CREATE_ACTIVITIES_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Activities] (
                                                                    [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [ActivityDate] TEXT NOT NULL);";

        public const string CREATE_ACTIVITYTIMES_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ActivityTimes] (
                                                                        [ActivityTimesID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [ActivityID] INTEGER NOT NULL,
                                                                        [ActivityName] VARCHAR(200) DEFAULT '''''' NOT NULL,
                                                                        [ActivityTime] INTEGER DEFAULT '0' NOT NULL,
                                                                        [Achievement] INTEGER DEFAULT '0' NOT NULL,
                                                                        [Intimacy] INTEGER DEFAULT '0' NOT NULL,
                                                                        [Pleasure] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_REACTIONS_TABLE_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Reactions] (
                                                                    [ReactionsID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [ToWhat] VARCHAR(400) DEFAULT '''''' NOT NULL,
                                                                    [Strength] INTEGER DEFAULT '-1' NOT NULL,
                                                                    [Type] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Action] INTEGER DEFAULT '0' NOT NULL,
                                                                    [ActionOf] VARCHAR(400) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_FEELINGS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Feelings] (
                                                                    [FeelingsID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [AboutWhat] VARCHAR(400) DEFAULT '''''' NOT NULL,
                                                                    [Strength] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Type] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Action] INTEGER DEFAULT '0' NOT NULL,
                                                                    [ActionOf] VARCHAR(400) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_ATTITUDES_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Attitudes] (
                                                                    [AttitudesID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [ToWhat] VARCHAR(400) DEFAULT '''''' NOT NULL,
                                                                    [TypeOf] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Belief] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Feeling] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Action] INTEGER DEFAULT '0' NOT NULL,
                                                                    [ActionOf] VARCHAR(400) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_RELATIONSHIPS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Relationships] (
                                                                        [RelationshipsID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [WithWhom] VARCHAR(100) DEFAULT '''''' NOT NULL,
                                                                        [Type] INTEGER DEFAULT '0' NOT NULL,
                                                                        [Strength] INTEGER DEFAULT '0' NOT NULL,
                                                                        [Feeling] INTEGER DEFAULT '0' NOT NULL,
                                                                        [Action] INTEGER DEFAULT '0' NOT NULL,
                                                                        [ActionOf] VARCHAR(400) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_HEALTH_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Health] (
                                                                [HealthID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                [Aspect] VARCHAR(100) DEFAULT '''''' NOT NULL,
                                                                [Importance] INTEGER DEFAULT '0' NOT NULL,
                                                                [Type] INTEGER DEFAULT '0' NOT NULL,
                                                                [Action] INTEGER DEFAULT '0' NOT NULL,
                                                                [ActionOf] VARCHAR(400) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_FANTASIES_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Fantasies] (
                                                                    [FantasiesID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [OfWhat] VARCHAR(200) DEFAULT '''''' NOT NULL,
                                                                    [Strength] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Type] INTEGER DEFAULT '0' NOT NULL,
                                                                    [Action] INTEGER DEFAULT '0' NOT NULL,
                                                                    [ActionOf] VARCHAR(400) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_PROBLEMS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Problems] (
                                                                    [ProblemID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                    [ProblemText] VARCHAR(500) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_PROBLEMSTEPS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ProblemSteps] (
                                                                        [ProblemStepID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [ProblemID] INTEGER DEFAULT '0' NOT NULL,
                                                                        [ProblemStep] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                        [PriorityOrder] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_TABLE_PROBLEMIDEAS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ProblemIdeas] (
                                                                        [ProblemIdeaID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [ProblemStepID] INTEGER DEFAULT '-1' NOT NULL,
                                                                        [ProblemID] INTEGER DEFAULT '-1' NOT NULL,
                                                                        [ProblemIdeaText] VARCHAR(500) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_PROBLEMPROSANDCONS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ProblemProsAndCons] (
                                                                            [ProblemProAndConID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [ProblemIdeaID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [ProblemStepID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [ProblemID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [ProblemProAndConText] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                            [ProblemProAndConType] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_TABLE_SOLUTIONPLANS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [SolutionPlans] (
                                                                            [SolutionPlanID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [ProblemIdeaID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [SolutionStep] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                            [PriorityOrder] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_TABLE_SOLUTIONREVIEWS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [SolutionReviews] (
                                                                            [SolutionReviewID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [ProblemIdeaID] INTEGER DEFAULT '-1' NOT NULL,
                                                                            [ReviewText] VARCHAR(1000) DEFAULT '''''' NOT NULL,
                                                                            [Achieved] BOOLEAN DEFAULT 'false' NOT NULL,
                                                                            [AchievedDate] VARCHAR(50) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_AFFIRMATIONS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Affirmations] (
                                                                            [AffirmationID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [AffirmationText] VARCHAR(500) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_IMAGERY_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Imagery] (
                                                                            [ImageryID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [ImageryURI] VARCHAR(500) DEFAULT '''''' NOT NULL,
                                                                            [ImageryComment] VARCHAR(500) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_PLAYLISTS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [PlayLists] (
                                                                        [PlayListID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [PlayListName] VARCHAR(200) DEFAULT '''''' NOT NULL,
                                                                        [PlayListTrackCount] INTEGER DEFAULT '0' NOT NULL);";

        public const string CREATE_TABLE_TRACKS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Tracks] (
                                                                        [TrackID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                        [PlayListID] INTEGER DEFAULT '-1' NOT NULL,
                                                                        [TrackName] VARCHAR(150) DEFAULT '''''' NOT NULL,
                                                                        [TrackArtist] VARCHAR(150) DEFAULT '''''' NOT NULL,
                                                                        [TrackOrderNumber] INTEGER DEFAULT '0' NOT NULL,
                                                                        [TrackUri] VARCHAR(100) DEFAULT '''''' NOT NULL,
                                                                        [TrackDuration] FLOAT DEFAULT '0' NOT NULL);";

        public const string CREATE_TABLE_APPOINTMENTS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Appointments] (
                                                                    [AppointmentID] INTEGER  PRIMARY KEY AUTOINCREMENT NULL,
                                                                    [AppointmentDate] TEXT  NULL,
                                                                    [AppointmentType] INTEGER  NULL,
                                                                    [Location] VARCHAR(150)  NULL,
                                                                    [WithWhom] VARCHAR(75)  NULL,
                                                                    [AppointmentTime] TIME  NULL,
                                                                    [Notes] TEXT  NULL);";

        public const string CREATE_TABLE_APPOINTMENTQUESTIONS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [AppointmentQuestions] (
                                                                            [QuestionsID] INTEGER  PRIMARY KEY AUTOINCREMENT NULL,
                                                                            [AppointmentID] INTEGER  NULL,
                                                                            [Question] TEXT  NULL,
                                                                            [Answer] TEXT  NULL);";

        public const string CREATE_TABLE_RESOURCE_MEDICATION_TYPES_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ResourceMedicationTypes] (
                                                                                [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                                [MedicationTypeTitle] VARCHAR(255) DEFAULT '''''' NOT NULL,
                                                                                [MedicationTypeDescription] TEXT DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_RESOURCE_MEDICATION_ITEM_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ResourceMedicationItem] (
                                                                                [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                                [MedicationTypeID] INTEGER  NOT NULL, 
                                                                                [MedicationName] VARCHAR(100) DEFAULT '''''' NOT NULL,
                                                                                [MedicationDescription] TEXT DEFAULT '''''' NOT NULL,
                                                                                [SideEffects] TEXT DEFAULT '''''' NOT NULL,
                                                                                [Dosage] TEXT DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_RESOURCE_CONDITIONS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [ResourceConditions] (
                                                                            [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                            [ConditionTitle] VARCHAR(200) DEFAULT '''''' NOT NULL,
                                                                            [ConditionDescription] TEXT DEFAULT '''''' NOT NULL,
                                                                            [ConditionCitation] VARCHAR(250) DEFAULT '''''' NOT NULL);";

        public const string CREATE_TABLE_SETTINGS_VERSION_1 = @"CREATE TABLE IF NOT EXISTS [Settings] (
                                                                [ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
                                                                [SettingKey] VARCHAR(100) DEFAULT '''''' NOT NULL,
                                                                [SettingValue] VARCHAR(100) DEFAULT '''''' NOT NULL);";

        public const int MAX_NUMBER_OF_MOODLIST_ITEMS = 29;

    }
}