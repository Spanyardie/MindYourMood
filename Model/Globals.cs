using Android.Database;
using Android.Database.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Content;
using Java.Util;

namespace com.spanyardie.MindYourMood
{
    public class Globals
    {
        private SQLiteDatabase _sqlDatabase;
        private string _filePath;

        public static string TAG = "M:Globals";
        public Globals()
        {
            var documentsPath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            _filePath = Path.Combine(documentsPath, "MindOverMood.s3db");
        }

        public bool OpenDatabase()
        {
            bool retVal = false;

            try
            {
                _sqlDatabase = SQLiteDatabase.OpenDatabase(_filePath, null, DatabaseOpenFlags.OpenReadwrite);
            }
            catch (Exception e)
            {
                var msg = e.Message;
                Log.Error(TAG, "OpenDatabase: Exception - " + msg);
                _sqlDatabase = null;
            }

            if (_sqlDatabase == null)
            {
                Log.Info(TAG, "OpenDatabase: Attempt to open database failed - trying OpenOrCreate");
                try
                {
                    _sqlDatabase = SQLiteDatabase.OpenOrCreateDatabase(_filePath, null);
                    if (_sqlDatabase != null)
                    {
                        //this should only be reached on first attempt to access the database (as it hasn't been created yet)
                        //therefore we will set the version number here to 1
                        _sqlDatabase.Version = 1;
                        CreateTables();
                        InsertMoodListData();
                        Log.Info(TAG, "OpenDatabase: Creation succeeded");
                        retVal = true;
                    }
                    else
                    {
                        Log.Info(TAG, "OpenDatabase: Creation failed - FATAL ERROR");
                        retVal = false;
                    }
                }
                catch
                {
                    retVal = false;
                }
            }
            else
            {
                //we have a valid existing open database so check if we require an update
                //Log.Info(TAG, "OpenDatabase: Database at " + _filePath + " opened successfully");
                int versionCode = _sqlDatabase.Version;
                bool needsUpgrade = _sqlDatabase.NeedUpgrade(ConstantsAndTypes.DATABASE_VERSION);
                if (needsUpgrade)
                {
                    Log.Info(TAG, "Upgrading database from version " + versionCode.ToString() + " to version " + ConstantsAndTypes.DATABASE_VERSION.ToString());
                    UpgradeDatabase(_sqlDatabase.Version);
                    retVal = true;
                }
            }

            return retVal;
        }

        public SQLiteDatabase GetSQLiteDatabase()
        {
            return _sqlDatabase;
        }

        public void UpgradeDatabase(int oldVersion)
        {
            //**************************************************************************************
            //
            //here we will put the code for the update for the version 
            //
            //      CURRENT VERSION = 1
            //      
            //
            //**************************************************************************************
            if(ConstantsAndTypes.DATABASE_VERSION >= 2)
            {
                Log.Info(TAG, "UpgradeDatabase: oldVersion " + oldVersion.ToString() + ", new version " + ConstantsAndTypes.DATABASE_VERSION.ToString());

                string sql = "";
                try {
                    if (ConstantsAndTypes.DATABASE_VERSION > 1)
                    {
                        if (ConstantsAndTypes.DATABASE_VERSION > oldVersion)
                        {
                            for (var version = oldVersion + 1; version <= ConstantsAndTypes.DATABASE_VERSION; version++)
                            {
                                try
                                {
                                    switch (version)
                                    {
                                        //case 2:
                                        //    break;
                                        default:
                                            break;
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Log.Error(TAG, "UpgradeDatabase: Exception - " + ex.Message);
                                    continue;
                                }
                            }
                            _sqlDatabase.Version = ConstantsAndTypes.DATABASE_VERSION;
                            Log.Info(TAG, "UpgradeDatabase: Set database version to " + ConstantsAndTypes.DATABASE_VERSION + " after upgrade");
                        }
                    }
                }
                catch(SQLiteException e)
                {
                    Log.Info(TAG, "UpgradeDatabase: Exception - " + e.Message);
                }
            } 
        }

        private void OneTimeAddDefaultSettings()
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //ShowHelpNow
                    values.Put("SettingKey", "ShowHelpNow");
                    values.Put("SettingValue", "True");
                    _sqlDatabase.Insert("Settings", null, values);

                    //ConfirmationAudio
                    values = new ContentValues();
                    values.Put("SettingKey", "ConfirmationAudio");
                    values.Put("SettingValue", "True");
                    _sqlDatabase.Insert("Settings", null, values);

                    //EmergencyLocale
                    //values = new ContentValues();
                    //values.Put("SettingKey", "EmergencyLocale");
                    //values.Put("SettingValue", "United Kingdom");
                    //_sqlDatabase.Insert("Settings", null, values);

                    //AlarmNotificationType
                    values = new ContentValues();
                    values.Put("SettingKey", "AlarmNotificationType");
                    values.Put("SettingValue", "-1");
                    _sqlDatabase.Insert("Settings", null, values);

                    //ShowErrorDialog
                    values = new ContentValues();
                    values.Put("SettingKey", "ShowErrorDialog");
                    values.Put("SettingValue", "True");
                    _sqlDatabase.Insert("Settings", null, values);

                    //--------------------------------------------
                    string language = Locale.Default.ISO3Language.ToLower();
                    //EmergencySms
                    values = new ContentValues();
                    values.Put("SettingKey", "EmergencySms");
                    string theValue = "Please can you contact me, I am feeling vulnerable and I need your help";
                    if (language == "spa")
                        theValue = "Por favor, puede ponerse en contacto conmigo, me siento muy vulnerable y necesito su ayuda";
                    if (language == "fra")
                        theValue = "S\'il vous plaît pouvez-vous me contacter, je me sens vulnérable et j\'ai besoin de votre aide";
                    values.Put("SettingValue", theValue);
                    _sqlDatabase.Insert("Settings", null, values);

                    //EmergencyEmailSubject
                    values = new ContentValues();
                    values.Put("SettingKey", "EmergencyEmailSubject");
                    theValue = "I need your help";
                    if (language == "spa")
                        theValue = "Necesito tu ayuda";
                    if (language == "fra")
                        theValue = "J\'ai besoin de ton aide";

                    values.Put("SettingValue", theValue);
                    _sqlDatabase.Insert("Settings", null, values);

                    //EmergencyEmailBody
                    values = new ContentValues();
                    values.Put("SettingKey", "EmergencyEmailBody");
                    theValue = "Please can you contact me, I am feeling very vulnerable and I need your help";
                    if (language == "spa")
                        theValue = "Por favor, puede ponerse en contacto conmigo, me siento muy vulnerable y necesito su ayuda";
                    if (language == "fra")
                        theValue = "S\'il vous plaît pouvez-vous me contacter, je me sens très vulnérable et j\'ai besoin de votre aide";

                    values.Put("SettingValue", theValue);
                    _sqlDatabase.Insert("Settings", null, values);

                    //EmergencyCallSpeaker
                    values = new ContentValues();
                    values.Put("SettingKey", "EmergencyCallSpeaker");
                    values.Put("SettingValue", "True");
                    _sqlDatabase.Insert("Settings", null, values);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OneTimeAddDefaultSettings: Exception - " + e.Message);
                throw new Exception("An error occurred Adding default settings - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddConditions(Context context)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Acute stress disorder
                    //----------------------------------------------------------
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionAcutestressdisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionAcutestressdisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionAcutestressdisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Agoraphobia
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionAgoraphobia));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionAgoraphobiaDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionAgoraphobiaCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Amnestic disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionAmnesticdisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionAmnesticdisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionAmnesticdisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Anorexia nervosa
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionAnorexianervosa));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionAnorexianervosaDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionAnorexianervosaCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Attention deficit disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionAttentiondeficitdisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionAttentiondeficitdisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionAttentiondeficitdisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Bereavement
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionBereavement));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionBereavementDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionBereavementCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //bipolar disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.Conditionbipolardisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionbipolardisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionbipolardisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Body dysmorphic disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionBodydysmorphicdisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionBodydysmorphicdisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionBodydysmorphicdisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Borderline personality disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionBorderlinepersonalitydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionBorderlinepersonalitydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionBorderlinepersonalitydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Bulimia nervosa
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionBulimianervosa));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionBulimianervosaDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionBulimianervosaCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Dissociative identity disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionDissociativeidentitydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionDissociativeidentitydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionDissociativeidentitydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Ekbom's Syndrome
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionEkbomsSyndrome));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionEkbomsSyndromeDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionEkbomsSyndromeCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Erotomania
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionErotomania));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionErotomaniaDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionErotomaniaCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Generalized anxiety disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionGeneralizedanxietydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionGeneralizedanxietydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionGeneralizedanxietydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Histrionic personality disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionHistrionicpersonalitydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionHistrionicpersonalitydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionHistrionicpersonalitydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Hypochondriasis
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionHypochondriasis));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionHypochondriasisDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionHypochondriasisCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Intermittent explosive disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionIntermittentexplosivedisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionIntermittentexplosivedisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionIntermittentexplosivedisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Kleptomania
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionKleptomania));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionKleptomaniaDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionKleptomaniaCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Major Depressive Disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionMajorDepressiveDisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionMajorDepressiveDisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionMajorDepressiveDisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Minor depressive disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionMinordepressivedisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionMinordepressivedisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionMinordepressivedisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Munchausen's syndrome
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionMunchausenssyndrome));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionMunchausenssyndromeDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionMunchausenssyndromeCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Narcissistic personality disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionNarcissisticpersonalitydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionNarcissisticpersonalitydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionNarcissisticpersonalitydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Obsessive-compulsive disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionObsessivecompulsivedisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionObsessivecompulsivedisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionObsessivecompulsivedisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Obsessive-compulsive personality disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionObsessivecompulsivepersonalitydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionObsessivecompulsivepersonalitydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionObsessivecompulsivepersonalitydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Panic disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionPanicdisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionPanicdisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionPanicdisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Paranoid personality disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionParanoidpersonalitydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionParanoidpersonalitydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionParanoidpersonalitydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Psychosis
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionPsychosis));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionPsychosisDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionPsychosisCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Posttraumatic stress disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionPosttraumaticstressdisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionPosttraumaticstressdisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionPosttraumaticstressdisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Schizophrenia
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionSchizophrenia));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionSchizophreniaDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionSchizophreniaCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Seasonal affective disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionSeasonalaffectivedisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionSeasonalaffectivedisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionSeasonalaffectivedisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Selective mutism
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionSelectivemutism));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionSelectivemutismDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionSelectivemutismCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Separation anxiety disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionSeparationanxietydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionSeparationanxietydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionSeparationanxietydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Social anxiety disorder
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("ConditionTitle", context.GetString(Resource.String.ConditionSocialanxietydisorder));
                    values.Put("ConditionDescription", context.GetString(Resource.String.ConditionSocialanxietydisorderDescription));
                    values.Put("ConditionCitation", context.GetString(Resource.String.ConditionSocialanxietydisorderCitation));
                    _sqlDatabase.Insert("ResourceConditions", null, values);
                    //----------------------------------------------------------

                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsAAP: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsAAP: Exception - " + e.Message);
                throw new Exception("An error occurred Adding AAP medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddCountryMoodList()
        {
            string language = Locale.Default.ISO3Language.ToLower();

            switch(language)
            {
                case "spa":
                    AddOneTimeSpanish();
                    break;
                case "fra":
                    AddOneTimeFrench();
                    break;
                default:
                    language = "eng";
                    break;
            }
        }

        public void CloseDatabase()
        {
            _sqlDatabase.Close();
        }

        private void CreateTables()
        {
            string sql = "";

            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    sql = ConstantsAndTypes.CREATE_ALTERNATIVETHOUGHTS_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Alternative Thoughts table successfully");
                    sql = ConstantsAndTypes.CREATE_AUTOMATICTHOUGHTS_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Automatic Thoughts table successfully");
                    sql = ConstantsAndTypes.CREATE_ACHIEVEMENTCHART_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Achievement Chart table successfully");
                    sql = ConstantsAndTypes.CREATE_EVIDENCEAGAINSTHOTTHOUGHT_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Evidence Against Hot Thought table successfully");
                    sql = ConstantsAndTypes.CREATE_EVIDENCEFORHOTTHOUGHT_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Evidence For Hot Thought table successfully");
                    sql = ConstantsAndTypes.CREATE_MOOD_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Mood table successfully");
                    sql = ConstantsAndTypes.CREATE_MOODLIST_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Mood List table successfully");
                    sql = ConstantsAndTypes.CREATE_RERATEMOOD_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Rerate Mood table successfully");
                    sql = ConstantsAndTypes.CREATE_SITUATION_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Situation table successfully");
                    sql = ConstantsAndTypes.CREATE_THOUGHTRECORD_TABLE_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Thought Record table successfully");
                    sql = ConstantsAndTypes.CREATE_VIEW_COMPLETEMOODSRATING_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Complete Moods Rating View successfully");
                    sql = ConstantsAndTypes.CREATE_VIEW_COMPLETERERATEMOODS_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created RerateMoods Rating View successfully");
                    sql = ConstantsAndTypes.CREATE_VIEW_MOODRATING_V1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Moods Rating View successfully");


                    sql = ConstantsAndTypes.CREATE_CONTACTS_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Contacts table successfully");
                    sql = ConstantsAndTypes.CREATE_TELLMYSELF_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created TellMyself table successfully");
                    sql = ConstantsAndTypes.CREATE_GENERICTEXT_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created GenericText table successfully");
                    sql = ConstantsAndTypes.CREATE_SAFETYPLANCARDS_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created SafetyPlanCards table successfully");
                    sql = ConstantsAndTypes.CREATE_MEDICATION_REMINDER_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Medication Reminder table successfully");
                    sql = ConstantsAndTypes.CREATE_MEDICATION_TIME_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Medication Time table successfully");
                    sql = ConstantsAndTypes.CREATE_PRESCRIPTION_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Prescription table successfully");
                    sql = ConstantsAndTypes.CREATE_MEDICATION_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Medication table successfully");
                    sql = ConstantsAndTypes.CREATE_MEDICATION_SPREAD_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created Medication Spread table successfully");
                    sql = ConstantsAndTypes.UPDATE_MOODLIST_TABLE_ENTER_GBR_CODES_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Updated existing Mood List, added GBR for all UK entries successfully");
                    //Adding Mood List next for Spanish mood list - this is a one time call and the function contents will change for each new country added
                    OneTimeAddCountryMoodList();
                    Log.Info(TAG, "CreateTables: Updated existing Mood List, added ESP mood entries successfully");
                    sql = ConstantsAndTypes.CREATE_ACTIVITIES_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Activities successfully");
                    sql = ConstantsAndTypes.CREATE_ACTIVITYTIMES_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ActivityTimes successfully");
                    sql = ConstantsAndTypes.CREATE_REACTIONS_TABLE_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Reactions successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_FEELINGS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Feelings successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_ATTITUDES_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Attitudes successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_RELATIONSHIPS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Relationships successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_HEALTH_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Health successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_FANTASIES_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Fantasies successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_PROBLEMS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Problems successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_PROBLEMSTEPS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ProblemSteps successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_PROBLEMIDEAS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ProblemIdeas successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_PROBLEMPROSANDCONS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ProblemProsAndCons successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_SOLUTIONPLANS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table SolutionPlans successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_SOLUTIONREVIEWS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table SolutionReviews successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_AFFIRMATIONS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Affirmations successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_IMAGERY_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Imagery successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_PLAYLISTS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table PlayLists successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_TRACKS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Tracks successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_APPOINTMENTS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Appointments successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_APPOINTMENTQUESTIONS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table AppointmentQuestions successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_RESOURCE_MEDICATION_TYPES_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ResourceMedicationTypes successfully");
                    sql = ConstantsAndTypes.CREATE_TABLE_RESOURCE_MEDICATION_ITEM_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ResourceMedicationItem successfully");
                    OneTimeAddMedicationTypes(GlobalData.GetApplicationContext());
                    sql = ConstantsAndTypes.CREATE_TABLE_RESOURCE_CONDITIONS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table ResourceConditions successfully");
                    OneTimeAddConditions(GlobalData.GetApplicationContext());
                    sql = ConstantsAndTypes.CREATE_TABLE_SETTINGS_VERSION_1;
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "CreateTables: Created table Settings successfully");
                    OneTimeAddDefaultSettings();
                }
            }
            catch (Exception e)
            {
                Log.Info(TAG, "Exception " + e.Message);
            }
        }

        private void InsertMoodListData()
        {
            string sql = "";

            try
            {
                for(var a = 0; a< GlobalData.MoodlistArray.GetLength(0); a++)
                {
                    sql = "INSERT INTO [MoodList]([MoodName], [IsoCountry], [IsDefault]) VALUES ('" + GlobalData.MoodlistArray[a] + "', '" + SystemHelper.GetIsoCountryAlias() + "', 'true');";
                    Log.Info(TAG, "Inserting MoodList item - " + sql);
                    _sqlDatabase.ExecSQL(sql);
                }

            }
            catch(SQLiteException s)
            {
                Log.Error(TAG, "InsertMoodListData: Exception - " + s.Message);
            }
        }

        public int GetAutoIndexID(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                int autoIndexId = 0;
                if (sqLiteDatabase.IsOpen)
                {
                    string commandText = "SELECT  last_insert_rowid();";
                    var command = sqLiteDatabase.RawQuery(commandText, null);

                    if ((command != null) && command.MoveToFirst())
                    {
                        autoIndexId = command.GetInt(0);
                    }
                    command.Close();

                    Log.Info(TAG, "autoIndexId - " + autoIndexId);
                    return autoIndexId;
                }
                else
                {
                    Log.Info(TAG, "");
                    return 0;
                }
            }
            catch(SQLException e)
            {
                Log.Error(TAG, "GetAutoIndexID: Unable to get autoIndexId - " + e.Message);
                throw new Exception("Unable to get Auto Index ID - " + e.Message);
            }
        }

        public List<AutomaticThoughts> GetAllAutomaticThoughts()
        {
            List<AutomaticThoughts> thoughtList = new List<AutomaticThoughts>();

            string[] arrColumns = new string[4];

            arrColumns[0] = "AutomaticThoughtsID";
            arrColumns[1] = "HotThought";
            arrColumns[2] = "Thought";
            arrColumns[3] = "ThoughtRecordID";

            try
            {
                var thoughtData = _sqlDatabase.Query("AutomaticThoughts", arrColumns, "ThoughtRecordID = " + GlobalData.ThoughtRecordId.ToString(), null, null, null, null);
                if (thoughtData != null)
                {
                    var count = thoughtData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllAutomaticThoughts: Found " + count.ToString() + " items");
                        thoughtData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var thought = new AutomaticThoughts();
                            thought.AutomaticThoughtsId = thoughtData.GetInt(thoughtData.GetColumnIndex("AutomaticThoughtsID"));
                            thought.IsHotThought = thoughtData.GetShort(thoughtData.GetColumnIndex("HotThought")) > 0;
                            thought.Thought = thoughtData.GetString(thoughtData.GetColumnIndex("Thought"));
                            thought.ThoughtRecordId = GlobalData.ThoughtRecordId;
                            thoughtList.Add(thought);
                            thoughtData.MoveToNext();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAllAutomaticThoughts: Exception - " + e.Message);
                thoughtList = null;
            }
            return thoughtList;

        }

        public List<Mood> GetAllMoods()
        {
            List<Mood> moods = new List<Mood>();

            string[] arrColumns = new string[4];

            arrColumns[0] = "MoodsID";
            arrColumns[1] = "MoodListID";
            arrColumns[2] = "MoodRating";
            arrColumns[3] = "ThoughtRecordID";

            try
            {
                var moodData = _sqlDatabase.Query("Mood", arrColumns, null, null, null, null, null);
                if (moodData != null)
                {
                    var count = moodData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllMoods: Found " + count.ToString() + " items");
                        moodData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var mood = new Mood();
                            mood.MoodsId = moodData.GetInt(moodData.GetColumnIndex("MoodsID"));
                            mood.MoodListId = moodData.GetInt(moodData.GetColumnIndex("MoodListID"));
                            mood.MoodRating = moodData.GetInt(moodData.GetColumnIndex("MoodRating"));
                            mood.ThoughtRecordId = GlobalData.ThoughtRecordId;
                            moods.Add(mood);
                            moodData.MoveToNext();
                        }
                    }
                }
                return moods;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAllMoods: Exception - " + e.Message + ": returning null Moods object)");
                moods = null;
            }
            return moods;
        }

        public void GetAllThoughtRecordsForDate(DateTime theDate)
        {
            if (GlobalData.ThoughtRecordsItems == null)
            {
                Log.Info(TAG, "ThoughtRecordItems NULL, re-creating");
                GlobalData.ThoughtRecordsItems = new List<ThoughtRecord>();
            }
            Log.Info(TAG, "Cleared ThoughtRecordItems");
            GlobalData.ThoughtRecordsItems.Clear();

            string[] arrColumns = new string[2];

            arrColumns[0] = "ThoughtRecordID";
            arrColumns[1] = "RecordDate";

            try
            {
                var thoughtRecordsData = _sqlDatabase.Query("ThoughtRecord", arrColumns, "RecordDate = '" + string.Format("{0:yyyy-MM-dd 00:00:00}", theDate) + "'", null, null, null, null);
                if (thoughtRecordsData != null)
                {
                    Log.Info(TAG, "Query returned " + thoughtRecordsData.Count + " items");
                    var count = thoughtRecordsData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllThoughtRecordsForDate: Found " + count.ToString() + " items");
                        thoughtRecordsData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var thoughtRecordID = thoughtRecordsData.GetInt(thoughtRecordsData.GetColumnIndex("ThoughtRecordID"));
                            //Log.Info(TAG, "thoughtRecordID - " + thoughtRecordID.ToString());
                            var thoughtRecord = new ThoughtRecord();
                            thoughtRecord.ThoughtRecordId = thoughtRecordID;
                            thoughtRecord.RecordDateTime = Convert.ToDateTime(thoughtRecordsData.GetString(thoughtRecordsData.GetColumnIndex("RecordDate")));
                            //now we can get the thought record to load up its own data
                            //Log.Info(TAG, "New Thought Record created - Id " + thoughtRecord.ThoughtRecordId.ToString() + ", date " + thoughtRecord.RecordDateTime.ToShortDateString());
                            thoughtRecord.Load(thoughtRecordID, _sqlDatabase);
                            GlobalData.ThoughtRecordsItems.Add(thoughtRecord);
                            thoughtRecordsData.MoveToNext();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAllThoughtRecordsForDate: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Thought Records for Date - " + e.Message, e.InnerException);
            }
        }

        public List<string> GetAllMoodsForAdapter()
        { 
            if(GlobalData.MoodListItems == null)
            {
                Log.Info(TAG, "GetAllMoodsForAdapter: Global List is null - re-creating");
                GlobalData.MoodListItems = new List<MoodList>();
            }

            if(GlobalData.MoodListItems.Count > 0)
            {
                Log.Info(TAG, "GetAllMoodsForAdapter: List is not empty - already retrieved all mood data?");
            }
            List<string> moods = new List<string>();

            string[] arrColumns = new string[4];

            arrColumns[0] = "MoodID";
            arrColumns[1] = "MoodName";
            arrColumns[2] = "IsoCountry";
            arrColumns[3] = "IsDefault";

            try
            {
                if (GlobalData.MoodListItems.Count > 0)
                {
                    foreach(var mood in GlobalData.MoodListItems) { moods.Add(mood.MoodName.Trim()); }
                }
                else
                {
                    var countryAlias = SystemHelper.GetIsoCountryAlias();
                    var moodData = _sqlDatabase.Query("MoodList", arrColumns, "[IsoCountry] = '" + countryAlias + "'", null, null, null, null);
                    if (moodData != null)
                    {
                        var count = moodData.Count;
                        Log.Info(TAG, "GetAllMoodsForAdapter: Found " + count.ToString() + " items");
                        GlobalData.MoodListItems.Clear(); //don't want to repeat ourselves here!!!
                        if (count > 0)
                        {
                            moodData.MoveToFirst();
                            for (var loop = 0; loop < count; loop++)
                            {
                                var mood = new MoodList();
                                mood.MoodId = moodData.GetInt(moodData.GetColumnIndex("MoodID"));
                                mood.MoodName = moodData.GetString(moodData.GetColumnIndex("MoodName"));
                                mood.MoodIsoCountryAlias = moodData.GetString(moodData.GetColumnIndex("IsoCountry"));
                                mood.IsDefault = moodData.GetString(moodData.GetColumnIndex("IsDefault"));
                                GlobalData.MoodListItems.Add(mood);
                                //Log.Info(TAG, "GetAllMoodsForAdapter: Add to Global mood list - " + mood.MoodName + ", with ID " + mood.MoodId.ToString());
                                moods.Add(mood.MoodName);
                                //Log.Info(TAG, "GetAllMoodsForAdapter: Added " + mood.MoodName + " to return value List<string>");
                                moodData.MoveToNext();
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetAllMoodsForAdapter: Query did not return any results!");
                    }
                }
                return moods;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllMoodsForAdapter: Exception - " + e.Message + ": returning null Moods object)");
                moods = null;
            }
            return moods;
        }

        public List<EmergencyNumber> GetAllEmergencyNumbers()
        {
            List<EmergencyNumber> emergencyNumbers = new List<EmergencyNumber>();

            string[] arrColumns = new string[6];

            arrColumns[0] = "EmergencyNumberID";
            arrColumns[1] = "CountryName";
            arrColumns[2] = "PoliceNumber";
            arrColumns[3] = "AmbulanceNumber";
            arrColumns[4] = "FireNumber";
            arrColumns[5] = "Notes";

            try
            {
                var emergencyNumberData = _sqlDatabase.Query("EmergencyNumbers", arrColumns, null, null, null, null, null);
                if (emergencyNumberData != null)
                {
                    var count = emergencyNumberData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllEmergencyNumbers: Found " + count.ToString() + " items");
                        emergencyNumberData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var number = new EmergencyNumber();
                            number.EmergencyNumberID = emergencyNumberData.GetInt(emergencyNumberData.GetColumnIndex("EmergencyNumberID"));
                            number.CountryName = emergencyNumberData.GetString(emergencyNumberData.GetColumnIndex("CountryName"));
                            number.PoliceNumber = emergencyNumberData.GetString(emergencyNumberData.GetColumnIndex("PoliceNumber"));
                            number.AmbulanceNumber = emergencyNumberData.GetString(emergencyNumberData.GetColumnIndex("AmbulanceNumber"));
                            number.FireNumber = emergencyNumberData.GetString(emergencyNumberData.GetColumnIndex("FireNumber"));
                            number.Notes = emergencyNumberData.GetString(emergencyNumberData.GetColumnIndex("Notes"));
                            number.IsNew = true;
                            number.IsDirty = false;
                            //Log.Info(TAG, "GetAllEmergencyNumbers: Retrieved Emergency Number information for " + number.CountryName.Trim());
                            emergencyNumbers.Add(number);
                            emergencyNumberData.MoveToNext();
                        }
                    }
                }
                return emergencyNumbers;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllEmergencyNumbers: Exception - " + e.Message);
                return null;
            }
        }

        public List<Contact> GetAllUsersContacts(Context context)
        {
            List<Contact> usersContacts = new List<Contact>(); 

            string[] arrColumns = new string[9];

            arrColumns[0] = "ID";
            arrColumns[1] = "URI";
            arrColumns[2] = "ContactName";
            arrColumns[3] = "ContactTelephoneNumber";
            arrColumns[4] = "ContactPhotoId";
            arrColumns[5] = "ContactEmail";
            arrColumns[6] = "ContactUseEmergencyCall";
            arrColumns[7] = "ContactUseEmergencySms";
            arrColumns[8] = "ContactUseEmergencyEmail";

            try
            {
                //var addressBook = new AddressBook(context);

                var userContactData = _sqlDatabase.Query("Contacts", arrColumns, null, null, null, null, null);
                if (userContactData != null)
                {
                    var count = userContactData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllUsersContacts: Found " + count.ToString() + " items");
                        userContactData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var contact = new Contact();
                            contact.ID = userContactData.GetInt(userContactData.GetColumnIndex("ID"));
                            contact.ContactUri = userContactData.GetString(userContactData.GetColumnIndex("URI"));
                            contact.ContactName = userContactData.GetString(userContactData.GetColumnIndex("ContactName"));
                            contact.ContactTelephoneNumber = userContactData.GetString(userContactData.GetColumnIndex("ContactTelephoneNumber"));
                            contact.ContactEmail = userContactData.GetString(userContactData.GetColumnIndex("ContactEmail"));
                            //Log.Info(TAG, "GetAllUsersContacts: Attempting string convert ContactUseEmergencyCall to boolean - " + userContactData.GetColumnIndex("ContactUseEmergencyCall"));
                            contact.ContactEmergencyCall = Convert.ToBoolean(userContactData.GetShort(userContactData.GetColumnIndex("ContactUseEmergencyCall")));
                            //Log.Info(TAG, "GetAllUsersContacts: Attempting string convert ContactUseEmergencySms to boolean - " + userContactData.GetColumnIndex("ContactUseEmergencySms"));
                            contact.ContactEmergencySms = Convert.ToBoolean(userContactData.GetShort(userContactData.GetColumnIndex("ContactUseEmergencySms")));
                            //Log.Info(TAG, "GetAllUsersContacts: Attempting string convert ContactUseEmergencyEmail to boolean - " + userContactData.GetColumnIndex("ContactUseEmergencyEmail"));
                            contact.ContactEmergencyEmail = Convert.ToBoolean(userContactData.GetShort(userContactData.GetColumnIndex("ContactUseEmergencyEmail")));

                            var foundContact = GlobalData.ContactItems.Find(item => item.ContactUri == contact.ContactUri);
                            if (foundContact != null)
                            {
                                var thumbNail = foundContact.ContactPhoto;
                                contact.ContactPhoto = thumbNail;
                            }
                            contact.IsNew = false;
                            contact.IsDirty = false;
                            //Log.Info(TAG, "GetAllUsersContacts: Retrieved Contact information for " + contact.ContactName.Trim());
                            usersContacts.Add(contact);
                            userContactData.MoveToNext();
                        }
                    }
                }
                return usersContacts;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllUsersContacts: Exception - " + e.Message);
                return null;
            }
        }

        public Appointments GetAppointmentByID(int appointmentID)
        {
            Appointments appointment = null;
           try
            {
                string[] arrColumns = new string[7];
                arrColumns[0] = "AppointmentID";
                arrColumns[1] = "AppointmentDate";
                arrColumns[2] = "AppointmentType";
                arrColumns[3] = "Location";
                arrColumns[4] = "WithWhom";
                arrColumns[5] = "AppointmentTime";
                arrColumns[6] = "Notes";

                var appointmenttData = _sqlDatabase.Query("Appointments", arrColumns, "AppointmentID = " + appointmentID.ToString(), null, null, null, null);
                if (appointmenttData != null)
                {
                    var count = appointmenttData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAppointmentByID: Found Appointment with ID - " + appointmentID.ToString());
                        appointmenttData.MoveToFirst();
                        appointment = new Appointments();
                        appointment.AppointmentID = appointmentID;
                        var theDate = appointmenttData.GetString(appointmenttData.GetColumnIndex("AppointmentDate"));
                        appointment.AppointmentDate = Convert.ToDateTime(theDate);
                        appointment.AppointmentType = appointmenttData.GetInt(appointmenttData.GetColumnIndex("AppointmentType"));
                        appointment.Location = appointmenttData.GetString(appointmenttData.GetColumnIndex("Location"));
                        appointment.WithWhom = appointmenttData.GetString(appointmenttData.GetColumnIndex("WithWhom"));
                        var theTime = appointmenttData.GetString(appointmenttData.GetColumnIndex("AppointmentTime"));
                        appointment.AppointmentTime = Convert.ToDateTime(theTime);
                        appointment.Notes = appointmenttData.GetString(appointmenttData.GetColumnIndex("Notes"));
                        appointment.IsDirty = false;
                        appointment.IsNew = false;

                        appointment.LoadAppointmentQuestions(_sqlDatabase);
                    }
                }
                return appointment;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAppointmentByID: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Appointment with ID - " + appointmentID.ToString() + ", " + e.Message, e.InnerException);
            }
        }

        public void GetAllAchievementChartItemsForDate(DateTime theDate)
        {
            if (GlobalData.AchievementChartItems == null)
            {
                GlobalData.AchievementChartItems = new List<AchievementChart>();
            }

            string[] arrColumns = new string[3];

            arrColumns[0] = "AchievementID";
            arrColumns[1] = "Achievement";
            arrColumns[2] = "ChuffChartType";

            try
            {
                var achievementChartData = _sqlDatabase.Query("ChuffChart", arrColumns, "AchievementDate = '" + string.Format("{0:yyyy-MM-dd 00:00:00}", theDate) + "'", null, null, null, null);
                if (achievementChartData != null)
                {
                    GlobalData.AchievementChartItems.Clear();
                    var count = achievementChartData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllAchievementChartItemsForDate: Found " + count.ToString() + " Achievement Chart items for date " + theDate.ToShortDateString());
                        achievementChartData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var achievementID = achievementChartData.GetInt(achievementChartData.GetColumnIndex("AchievementID"));
                            var achievementChart = new AchievementChart();
                            achievementChart.AchievementId = achievementID;
                            achievementChart.AchievementDate = theDate;
                            achievementChart.Achievement = achievementChartData.GetString(achievementChartData.GetColumnIndex("Achievement"));
                            achievementChart.AchievementChartType = (AchievementChart.ACHIEVEMENTCHART_TYPE)achievementChartData.GetInt(achievementChartData.GetColumnIndex("ChuffChartType"));
                            achievementChart.IsDirty = false;
                            achievementChart.IsNew = false;
                            GlobalData.AchievementChartItems.Add(achievementChart);
                            achievementChartData.MoveToNext();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllAchievementChartItemsForDate: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Achievements for Date - " + e.Message, e.InnerException);
            }
        }

        public List<TellMyself> GetAllTellMyselfEntries()
        {
            List<TellMyself> tellMyselfEntries = new List<TellMyself>();

            string[] arrColumns = new string[4];

            arrColumns[0] = "ID";
            arrColumns[1] = "TellType";
            arrColumns[2] = "TellText";
            arrColumns[3] = "TellTitle";

            try
            {
                var tellMyselfData = _sqlDatabase.Query("TellMyself", arrColumns, null, null, null, null, null);
                if (tellMyselfData != null)
                {
                    var count = tellMyselfData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllTellMyselfEntries: Found " + count.ToString() + " items");
                        tellMyselfData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var tellMyselfEntry = new TellMyself();
                            tellMyselfEntry.ID = tellMyselfData.GetInt(tellMyselfData.GetColumnIndex("ID"));
                            tellMyselfEntry.TellType = (ConstantsAndTypes.TELL_TYPE)tellMyselfData.GetInt(tellMyselfData.GetColumnIndex("TellType"));
                            tellMyselfEntry.TellText = tellMyselfData.GetString(tellMyselfData.GetColumnIndex("TellText"));
                            tellMyselfEntry.TellTitle = tellMyselfData.GetString(tellMyselfData.GetColumnIndex("TellTitle"));
                            tellMyselfEntries.Add(tellMyselfEntry);
                            //Log.Info(TAG, "GetAllTellMyselfEntries: Retrieved entry type " + (tellMyselfEntry.TellType == ConstantsAndTypes.TELL_TYPE.Audio?"Audio":"Textual") + ", '" + (tellMyselfEntry.TellType == ConstantsAndTypes.TELL_TYPE.Audio?tellMyselfEntry.TellTitle:"'" + tellMyselfEntry.TellText + "'"));
                            tellMyselfData.MoveToNext();
                        }
                    }
                    else
                    {
                        Log.Info(TAG, "GetAllTellMyselfEntries: No entries found!");
                    }
                }
                return tellMyselfEntries;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllTellMyselfEntries: Exception - " + e.Message + ": returning null tellMyselfEntries object)");
                tellMyselfEntries = null;
                return tellMyselfEntries;
            }
        }

        public List<GenericText> GetAllGenericTextEntries()
        {
            List<GenericText> genericTextEntries = new List<GenericText>();

            string[] arrColumns = new string[3];

            arrColumns[0] = "ID";
            arrColumns[1] = "TextType";
            arrColumns[2] = "TextValue";

            try
            {
                var genericTextData = _sqlDatabase.Query("GenericText", arrColumns, null, null, null, null, null);
                if (genericTextData != null)
                {
                    var count = genericTextData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllGenericTextEntries: Found " + count.ToString() + " items");
                        genericTextData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var genericTextEntry = new GenericText();
                            genericTextEntry.ID = genericTextData.GetInt(genericTextData.GetColumnIndex("ID"));
                            genericTextEntry.TextType = (ConstantsAndTypes.GENERIC_TEXT_TYPE)genericTextData.GetInt(genericTextData.GetColumnIndex("TextType"));
                            genericTextEntry.TextValue = genericTextData.GetString(genericTextData.GetColumnIndex("TextValue"));
                            genericTextEntries.Add(genericTextEntry);
                            //Log.Info(TAG, "GetAllGenericTextEntries: Retrieved Generic Text '" + genericTextEntry.TextValue + "'");
                            genericTextData.MoveToNext();
                        }
                    }
                    else
                    {
                        Log.Info(TAG, "GetAllGenericTextEntries: No entries found!");
                    }
                }
                return genericTextEntries;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllGenericTextEntries: Exception - " + e.Message + ": returning null genericTextEntries object)");
                genericTextEntries = null;
            }
            return genericTextEntries;
        }

        public List<SafetyPlanCard> GetAllSafetyPlanCards()
        {
            List<SafetyPlanCard> safetyPlanCards = new List<SafetyPlanCard>();

            string[] arrColumns = new string[5];

            arrColumns[0] = "ID";
            arrColumns[1] = "CalmMyself";
            arrColumns[2] = "TellMyself";
            arrColumns[3] = "WillCall";
            arrColumns[4] = "WillGoTo";

            try
            {
                var safetyPlanCardsData = _sqlDatabase.Query("SafetyPlanCards", arrColumns, null, null, null, null, null);
                if (safetyPlanCardsData != null)
                {
                    var count = safetyPlanCardsData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllSafetyPlanCards: Found " + count.ToString() + " items");
                        safetyPlanCardsData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            SafetyPlanCard safetyPlanCard = new SafetyPlanCard();
                            safetyPlanCard.IsNew = false;
                            safetyPlanCard.IsDirty = false;
                            safetyPlanCard.ID = safetyPlanCardsData.GetInt(safetyPlanCardsData.GetColumnIndex("ID"));
                            safetyPlanCard.CalmMyself = safetyPlanCardsData.GetString(safetyPlanCardsData.GetColumnIndex("CalmMyself"));
                            //Log.Info(TAG, "GetAllSafetyPlanCards: Retrieved CalmMyself - " + safetyPlanCard.CalmMyself);
                            safetyPlanCard.TellMyself = safetyPlanCardsData.GetString(safetyPlanCardsData.GetColumnIndex("TellMyself"));
                            //Log.Info(TAG, "GetAllSafetyPlanCards: Retrieved TellMyself - " + safetyPlanCard.TellMyself);
                            safetyPlanCard.WillCall = safetyPlanCardsData.GetString(safetyPlanCardsData.GetColumnIndex("WillCall"));
                            //Log.Info(TAG, "GetAllSafetyPlanCards: Retrieved WillCall - " + safetyPlanCard.WillCall);
                            safetyPlanCard.WillGoTo = safetyPlanCardsData.GetString(safetyPlanCardsData.GetColumnIndex("WillGoTo"));
                            //Log.Info(TAG, "GetAllSafetyPlanCards: Retrieved WillGoTo - " + safetyPlanCard.WillGoTo);
                            safetyPlanCards.Add(safetyPlanCard);
                            safetyPlanCardsData.MoveToNext();
                        }
                    }
                    else
                    {
                        Log.Info(TAG, "GetAllSafetyPlanCards: No Safety Plan Cards found!");
                    }
                }
                return safetyPlanCards;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllSafetyPlanCards: Exception - " + e.Message + ": returning null SafetyPlanCards object)");
                safetyPlanCards = null;
            }
            return safetyPlanCards;
        }

        public List<Medication> GetAllMedicationItems()
        {
            List<Medication> medications = new List<Medication>();

            string[] medCols = new string[]
            {
                "ID",
                "MedicationName",
                "TotalDailyDosage"
            };

            var medicationList = _sqlDatabase.Query("Medication", medCols, null, null, null, null, null);
            if(medicationList != null && medicationList.Count > 0)
            {
                medicationList.MoveToFirst();
                for(var a = 0; a < medicationList.Count; a++)
                {
                    var medicationItem = new Medication();
                    medicationItem.ID = medicationList.GetInt(medicationList.GetColumnIndex("ID"));
                    medicationItem.MedicationName = medicationList.GetString(medicationList.GetColumnIndex("MedicationName"));
                    medicationItem.TotalDailyDosage = medicationList.GetInt(medicationList.GetColumnIndex("TotalDailyDosage"));
                    medicationItem.LoadMedication();
                    medications.Add(medicationItem);
                    medicationList.MoveToNext();
                }
            }
            return medications;
        }

        public List<Activities> GetAllActivitiesForCurrentWeek()
        {
            var monday = DateHelper.FindDateForBeginningOfWeek();
            var sunday = DateHelper.FindDateForEndOfWeek();

            List<Activities> activities = new List<Activities>();

            string[] actCols = new string[]
            {
                "ID",
                "ActivityDate"
            };

            string whereClause = "[ActivityDate] BETWEEN '" + string.Format("{0:yyyy-MM-dd}", monday) + " 00:00:00' AND '" + string.Format("{0:yyyy-MM-dd}", sunday) + " 23:59:59'";
            Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Attempting to retrieve activities in the date range " + monday.ToShortDateString() + " to " + sunday.ToShortDateString());
            var activityList = _sqlDatabase.Query("Activities", actCols, whereClause, null, null, null, "ActivityDate", null);
            if(activityList != null && activityList.Count > 0)
            {
                Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Found " + activityList.Count.ToString() + " Activities");
                activityList.MoveToFirst();
                for (DateTime activityDay = monday; activityDay <= sunday; activityDay = activityDay.AddDays(1))
                {
                    var theCurrentDate = Convert.ToDateTime(activityList.GetString(activityList.GetColumnIndex("ActivityDate")));
                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Current date before time massage - " + theCurrentDate.ToString());
                    theCurrentDate = theCurrentDate.AddHours(23);
                    theCurrentDate = theCurrentDate.AddMinutes(59);
                    theCurrentDate = theCurrentDate.AddSeconds(59);
                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Creating Activity for " + activityDay.ToShortDateString() + " (theCurrentDate is " + theCurrentDate.ToString() + ")...");
                    var activity = new Activities();
                    if (theCurrentDate.ToShortDateString() == activityDay.ToShortDateString())
                    {
                        activity.ActivityID = activityList.GetInt(activityList.GetColumnIndex("ID"));
                        activity.ActivityDate = theCurrentDate;
                        activity.IsNew = false;

                        string[] timeCols = new string[]
                        {
                            "ActivityTimesID",
                            "ActivityID",
                            "ActivityName",
                            "ActivityTime",
                            "Achievement",
                            "Intimacy",
                            "Pleasure"
                        };
                        whereClause = "[ActivityID] = " + activity.ActivityID;
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Searching for ActivityTimes with ActivityID " + activity.ActivityID.ToString());
                        var activityTimeList = _sqlDatabase.Query("ActivityTimes", timeCols, whereClause, null, null, null, "ActivityTime", null);
                        if (activityTimeList != null && activityTimeList.Count > 0)
                        {
                            //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Found ActivityTimes for ActivityID " + activity.ActivityID.ToString());
                            activityTimeList.MoveToFirst();
                            //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Cycling through Times...");
                            activity.ActivityTimes.Clear();
                            for (
                                ConstantsAndTypes.ACTIVITY_HOURS actHours = ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM;
                                actHours <= ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM;
                                actHours++
                               )
                            {
                                var theHours = (ConstantsAndTypes.ACTIVITY_HOURS)activityTimeList.GetInt(activityTimeList.GetColumnIndex("ActivityTime"));
                                //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Looking for " + StringHelper.ActivityTimeForConstant(theHours) + "...");
                                if (actHours == theHours)
                                {
                                    Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Matched! - Creating...");
                                    var activityTime = new ActivityTime();
                                    activityTime.IsNew = false;
                                    activityTime.ActivityTimeID = activityTimeList.GetInt(activityTimeList.GetColumnIndex("ActivityTimesID"));
                                    activityTime.ActivityID = activity.ActivityID;
                                    activityTime.ActivityName = activityTimeList.GetString(activityTimeList.GetColumnIndex("ActivityName"));
                                    activityTime.ActivityTime = theHours;
                                    activityTime.Achievement = activityTimeList.GetInt(activityTimeList.GetColumnIndex("Achievement"));
                                    activityTime.Intimacy = activityTimeList.GetInt(activityTimeList.GetColumnIndex("Intimacy"));
                                    activityTime.Pleasure = activityTimeList.GetInt(activityTimeList.GetColumnIndex("Pleasure"));
                                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Adding ActivityTime with ID " + activityTime.ActivityTimeID.ToString());
                                    activity.ActivityTimes.Add(activityTime);
                                    if(!activityTimeList.IsLast)
                                        activityTimeList.MoveToNext();
                                }
                                else
                                {
                                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: No match - creating default ActivityTime for " + StringHelper.ActivityTimeForConstant(actHours));
                                    var activityTime = new ActivityTime();
                                    activityTime.ActivityTime = actHours;
                                    activity.ActivityTimes.Add(activityTime);
                                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Added ActivityTime to list");
                                }
                            }
                        }
                        else
                        {
                            //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: No stored ActivityTimes found, creating defaults...");
                            for (
                                ConstantsAndTypes.ACTIVITY_HOURS actHours = ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM;
                                actHours <= ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM;
                                actHours++
                               )
                            {
                                var activityTime = new ActivityTime();
                                activityTime.ActivityTime = actHours;
                                activity.ActivityTimes.Add(activityTime);
                            }
                        }

                        activities.Add(activity);
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Added activity with ID " + activity.ActivityID.ToString() + " to list");
                        if(!activityList.IsLast)
                            activityList.MoveToNext();
                    }
                    else
                    {
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Dates don't match, adding default Activity...");
                        for (
                            ConstantsAndTypes.ACTIVITY_HOURS actHours = ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM;
                            actHours <= ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM;
                            actHours++
                           )
                        {
                            var activityTime = new ActivityTime();
                            activityTime.ActivityTime = actHours;
                            activity.ActivityTimes.Add(activityTime);
                        }
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: activityDay before time massage - " + activityDay.ToString());
                        DateTime massagedActivityDay = activityDay;
                        massagedActivityDay = massagedActivityDay.AddHours(23);
                        massagedActivityDay = massagedActivityDay.AddMinutes(59);
                        massagedActivityDay = massagedActivityDay.AddSeconds(59);
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Massaged activityDay - " + activityDay.ToString());
                        activity.ActivityDate = massagedActivityDay;
                        activities.Add(activity);
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Added default Activity to list");
                    }
                }
            }
            else
            {
                //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: No Activities found - creating blank set...");
                for(DateTime activityDay = monday; activityDay <= sunday; activityDay = activityDay.AddDays(1))
                {
                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Creating default Activity for " + activityDay.ToShortDateString() + "...");
                    var activity = new Activities();
                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: activityDay before time massage - " + activityDay.ToString());
                    DateTime massagedActivityDay = activityDay;
                    massagedActivityDay = massagedActivityDay.AddHours(23);
                    massagedActivityDay = massagedActivityDay.AddMinutes(59);
                    massagedActivityDay = massagedActivityDay.AddSeconds(59);
                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Massaged activityDay - " + activityDay.ToString());
                    activity.ActivityDate = massagedActivityDay;
                    //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Creating default ActivityTimes...");
                    for(
                        ConstantsAndTypes.ACTIVITY_HOURS actHours = ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM; 
                        actHours <= ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM; 
                        actHours++
                       )
                    {
                        //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Creating default Activity Time for " + StringHelper.ActivityTimeForConstant(actHours));
                        var activityTime = new ActivityTime();
                        activityTime.ActivityTime = actHours;
                        activity.ActivityTimes.Add(activityTime);
                    }
                    activities.Add(activity);
                }
            }
            //Log.Info(TAG, "GetAllActivitiesForCurrentWeek: Finished creating Activity entries");
            return activities;
        }

        public void RemovePreviousWeeksActivities()
        {
            DateTime currentDate = DateTime.Now;
            Log.Info(TAG, "RemovePreviousWeeksActivities: Todays date is - " + currentDate.ToShortDateString());

            if(currentDate.DayOfWeek == DayOfWeek.Monday)
            {
                Log.Info(TAG, "RemovePreviousWeeksActivities: Today is a Monday, proceeding to remove last weeks activities...");
                DateTime endOfPreviousWeek = currentDate.AddDays(-1);
                DateTime startOfPreviousWeek = endOfPreviousWeek.AddDays(-6);
                Log.Info(TAG, "RemovePreviousWeeksActivities: Start date of previous week - " + startOfPreviousWeek.ToShortDateString() + ", End date of previous week - " + endOfPreviousWeek.ToShortDateString());
                if (NumberOfActivitiesForPreviousWeek(startOfPreviousWeek, endOfPreviousWeek) > 0)
                {
                    Log.Info(TAG, "RemovePreviousWeeksActivities: Removing activities...");
                    RemoveActivitiesForPreviousWeek(startOfPreviousWeek, endOfPreviousWeek);
                }
            }
        }

        private void RemoveActivitiesForPreviousWeek(DateTime startOfPreviousWeek, DateTime endOfPreviousWeek)
        {
            string whereClause = "[ActivityDate] BETWEEN '" + string.Format("{0:yyyy-MM-dd}", startOfPreviousWeek) + " 00:00:00' AND '" + string.Format("{0:yyyy-MM-dd}", endOfPreviousWeek) + " 23:59:59'";

            Log.Info(TAG, "RemoveActivitiesForPreviousWeek: Removing activities " + whereClause);
            _sqlDatabase.Delete("Activities", whereClause, null);
        }

        private int NumberOfActivitiesForPreviousWeek(DateTime startDate, DateTime endDate)
        {
            string[] actCols = new string[]
            {
                "ID"
            };

            string whereClause = "[ActivityDate] BETWEEN '" + string.Format("{0:yyyy-MM-dd}", startDate) + " 00:00:00' AND '" + string.Format("{0:yyyy-MM-dd}", endDate) + " 23:59:59'";
            Log.Info(TAG, "NumberOfActivitiesForPreviousWeek: Attempting to retrieve number of activities in the date range " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString());
            var activityList = _sqlDatabase.Query("Activities", actCols, whereClause, null, null, null, null, null);

            Log.Info(TAG, "NumberOfActivitiesForPreviousWeek: Number of activities for date range " + startDate.ToShortDateString() + " - " + endDate.ToShortDateString() + " is " + activityList.Count.ToString());
            return activityList.Count;
        }
        public List<Reactions> GetAllReactions()
        {
            List<Reactions> reactions = new List<Reactions>();

            string[] reactCols = new string[]
            {
                "ReactionsID",
                "ToWhat",
                "Strength",
                "Type",
                "Action",
                "ActionOf"
            };

            var reactionList = _sqlDatabase.Query("Reactions", reactCols, null, null, null, null, null);
            if (reactionList != null && reactionList.Count > 0)
            {
                reactionList.MoveToFirst();
                for (var a = 0; a < reactionList.Count; a++)
                {
                    var reactionItem = new Reactions();
                    reactionItem.ReactionsID = reactionList.GetInt(reactionList.GetColumnIndex("ReactionsID"));
                    reactionItem.ToWhat = reactionList.GetString(reactionList.GetColumnIndex("ToWhat"));
                    reactionItem.Strength = reactionList.GetInt(reactionList.GetColumnIndex("Strength"));
                    reactionItem.Type = (ConstantsAndTypes.REACTION_TYPE)reactionList.GetInt(reactionList.GetColumnIndex("Type"));
                    reactionItem.Action = (ConstantsAndTypes.ACTION_TYPE)reactionList.GetInt(reactionList.GetColumnIndex("Action"));
                    reactionItem.ActionOf = reactionList.GetString(reactionList.GetColumnIndex("ActionOf"));
                    reactions.Add(reactionItem);
                    reactionList.MoveToNext();
                }
            }
            return reactions;
        }

        public List<Feelings> GetAllFeelings()
        {
            List<Feelings> feelings = new List<Feelings>();

            string[] feelCols = new string[]
            {
                "FeelingsID",
                "AboutWhat",
                "Strength",
                "Type",
                "Action",
                "ActionOf"
            };

            var feelingList = _sqlDatabase.Query("Feelings", feelCols, null, null, null, null, null);
            if (feelingList != null && feelingList.Count > 0)
            {
                feelingList.MoveToFirst();
                for (var a = 0; a < feelingList.Count; a++)
                {
                    var feelingItem = new Feelings();
                    feelingItem.FeelingsID = feelingList.GetInt(feelingList.GetColumnIndex("FeelingsID"));
                    feelingItem.AboutWhat = feelingList.GetString(feelingList.GetColumnIndex("AboutWhat"));
                    feelingItem.Strength = feelingList.GetInt(feelingList.GetColumnIndex("Strength"));
                    feelingItem.Type = (ConstantsAndTypes.REACTION_TYPE)feelingList.GetInt(feelingList.GetColumnIndex("Type"));
                    feelingItem.Action = (ConstantsAndTypes.ACTION_TYPE)feelingList.GetInt(feelingList.GetColumnIndex("Action"));
                    feelingItem.ActionOf = feelingList.GetString(feelingList.GetColumnIndex("ActionOf"));
                    feelings.Add(feelingItem);
                    feelingList.MoveToNext();
                }
            }
            return feelings;
        }

        public List<Attitudes> GetAllAttitudes()
        {
            List<Attitudes> attitudes = new List<Attitudes>();

            string[] attitudeCols = new string[]
            {
                "AttitudesID",
                "ToWhat",
                "TypeOf",
                "Belief",
                "Feeling",
                "Action",
                "ActionOf"
            };

            var attitudeList = _sqlDatabase.Query("Attitudes", attitudeCols, null, null, null, null, null);
            if (attitudeList != null && attitudeList.Count > 0)
            {
                attitudeList.MoveToFirst();
                for (var a = 0; a < attitudeList.Count; a++)
                {
                    var attitudeItem = new Attitudes();
                    attitudeItem.AttitudesID = attitudeList.GetInt(attitudeList.GetColumnIndex("AttitudesID"));
                    attitudeItem.ToWhat = attitudeList.GetString(attitudeList.GetColumnIndex("ToWhat"));
                    attitudeItem.TypeOf = (ConstantsAndTypes.ATTITUDE_TYPES)attitudeList.GetInt(attitudeList.GetColumnIndex("TypeOf"));
                    attitudeItem.Belief = attitudeList.GetInt(attitudeList.GetColumnIndex("Belief"));
                    attitudeItem.Feeling = attitudeList.GetInt(attitudeList.GetColumnIndex("Feeling"));
                    attitudeItem.Action = (ConstantsAndTypes.ACTION_TYPE)attitudeList.GetInt(attitudeList.GetColumnIndex("Action"));
                    attitudeItem.ActionOf = attitudeList.GetString(attitudeList.GetColumnIndex("ActionOf"));
                    attitudes.Add(attitudeItem);
                    attitudeList.MoveToNext();
                }
            }
            return attitudes;
        }

        public List<Relationships> GetAllRelationships()
        {
            List<Relationships> relationships = new List<Relationships>();

            string[] relationshipCols = new string[]
            {
                "RelationshipsID",
                "WithWhom",
                "Type",
                "Strength",
                "Feeling",
                "Action",
                "ActionOf"
            };

            var relationshipList = _sqlDatabase.Query("Relationships", relationshipCols, null, null, null, null, null);
            if (relationshipList != null && relationshipList.Count > 0)
            {
                relationshipList.MoveToFirst();
                for (var a = 0; a < relationshipList.Count; a++)
                {
                    var relationshipItem = new Relationships();
                    relationshipItem.RelationshipsID = relationshipList.GetInt(relationshipList.GetColumnIndex("RelationshipsID"));
                    relationshipItem.WithWhom = relationshipList.GetString(relationshipList.GetColumnIndex("WithWhom"));
                    relationshipItem.Type = (ConstantsAndTypes.RELATIONSHIP_TYPE)relationshipList.GetInt(relationshipList.GetColumnIndex("Type"));
                    relationshipItem.Strength = relationshipList.GetInt(relationshipList.GetColumnIndex("Strength"));
                    relationshipItem.Feeling = relationshipList.GetInt(relationshipList.GetColumnIndex("Feeling"));
                    relationshipItem.Action = (ConstantsAndTypes.ACTION_TYPE)relationshipList.GetInt(relationshipList.GetColumnIndex("Action"));
                    relationshipItem.ActionOf = relationshipList.GetString(relationshipList.GetColumnIndex("ActionOf"));
                    relationships.Add(relationshipItem);
                    relationshipList.MoveToNext();
                }
            }
            return relationships;
        }

        public List<Health> GetAllHealth()
        {
            List<Health> healths = new List<Health>();

            string[] healthCols = new string[]
            {
                "HealthID",
                "Aspect",
                "Importance",
                "Type",
                "Action",
                "ActionOf"
            };

            var healthList = _sqlDatabase.Query("Health", healthCols, null, null, null, null, null);
            if (healthList != null && healthList.Count > 0)
            {
                healthList.MoveToFirst();
                for (var a = 0; a < healthList.Count; a++)
                {
                    var healthItem = new Health();
                    healthItem.HealthID = healthList.GetInt(healthList.GetColumnIndex("HealthID"));
                    healthItem.Aspect = healthList.GetString(healthList.GetColumnIndex("Aspect"));
                    healthItem.Importance = healthList.GetInt(healthList.GetColumnIndex("Importance"));
                    healthItem.Type = (ConstantsAndTypes.REACTION_TYPE)healthList.GetInt(healthList.GetColumnIndex("Type"));
                    healthItem.Action = (ConstantsAndTypes.ACTION_TYPE)healthList.GetInt(healthList.GetColumnIndex("Action"));
                    healthItem.ActionOf = healthList.GetString(healthList.GetColumnIndex("ActionOf"));
                    healths.Add(healthItem);
                    healthList.MoveToNext();
                }
            }
            return healths;
        }

        public List<Fantasies> GetAllFantasies()
        {
            List<Fantasies> fantasies = new List<Fantasies>();

            string[] fantasyCols = new string[]
            {
                "FantasiesID",
                "OfWhat",
                "Strength",
                "Type",
                "Action",
                "ActionOf"
            };

            var fantasyList = _sqlDatabase.Query("Fantasies", fantasyCols, null, null, null, null, null);
            if (fantasyList != null && fantasyList.Count > 0)
            {
                fantasyList.MoveToFirst();
                for (var a = 0; a < fantasyList.Count; a++)
                {
                    var fantasyItem = new Fantasies();
                    fantasyItem.FantasiesID = fantasyList.GetInt(fantasyList.GetColumnIndex("FantasiesID"));
                    fantasyItem.OfWhat = fantasyList.GetString(fantasyList.GetColumnIndex("OfWhat"));
                    fantasyItem.Strength = fantasyList.GetInt(fantasyList.GetColumnIndex("Strength"));
                    fantasyItem.Type = (ConstantsAndTypes.REACTION_TYPE)fantasyList.GetInt(fantasyList.GetColumnIndex("Type"));
                    fantasyItem.Action = (ConstantsAndTypes.ACTION_TYPE)fantasyList.GetInt(fantasyList.GetColumnIndex("Action"));
                    fantasyItem.ActionOf = fantasyList.GetString(fantasyList.GetColumnIndex("ActionOf"));
                    fantasies.Add(fantasyItem);
                    fantasyList.MoveToNext();
                }
            }
            return fantasies;
        }

        public List<Problem> GetAllProblems()
        {
            List<Problem> problems = new List<Model.Problem>();

            string[] problemCols = new string[]
            {
                "ProblemID",
                "ProblemText"
            };

            var problemList = _sqlDatabase.Query("Problems", problemCols, null, null, null, null, null);
            if(problemList != null && problemList.Count > 0)
            {
                problemList.MoveToFirst();
                for(var a = 0; a < problemList.Count; a++)
                {
                    var problemItem = new Problem();
                    problemItem.ProblemID = problemList.GetInt(problemList.GetColumnIndex("ProblemID"));
                    problemItem.ProblemText = problemList.GetString(problemList.GetColumnIndex("ProblemText"));

                    problemItem.ProblemSteps = GetAllProblemStepsForProblem(problemItem.ProblemID);

                    problemItem.IsNew = false;
                    problemItem.IsDirty = false;
                    problems.Add(problemItem);
                    problemList.MoveToNext();
                }
            }

            return problems;
        }

        public List<ProblemStep> GetAllProblemStepsForProblem(int problemID)
        {
            List<ProblemStep> problemSteps = new List<ProblemStep>();

            string[] problemStepCols = new string[]
            {
                "ProblemStepID",
                "ProblemID",
                "ProblemStep",
                "PriorityOrder"
            };

            string whereClause = "[ProblemID] = " + problemID;

            var problemStepList = _sqlDatabase.Query("ProblemSteps", problemStepCols, whereClause, null, null, null, "PriorityOrder");
            if(problemStepList != null && problemStepList.Count > 0)
            {
                problemStepList.MoveToFirst();
                for(var a = 0; a < problemStepList.Count; a++)
                {
                    var problemStepItem = new ProblemStep();
                    problemStepItem.ProblemStepID = problemStepList.GetInt(problemStepList.GetColumnIndex("ProblemStepID"));
                    problemStepItem.ProblemID = problemStepList.GetInt(problemStepList.GetColumnIndex("ProblemID"));
                    problemStepItem.ProblemStep = problemStepList.GetString(problemStepList.GetColumnIndex("ProblemStep"));
                    problemStepItem.PriorityOrder = problemStepList.GetInt(problemStepList.GetColumnIndex("PriorityOrder"));

                    problemStepItem.ProblemStepIdeas = GetAllProblemIdeasForProblemStep(problemStepItem.ProblemStepID);

                    problemStepItem.IsNew = false;
                    problemStepItem.IsDirty = false;
                    problemSteps.Add(problemStepItem);
                    problemStepList.MoveToNext();
                }
            }

            return problemSteps;
        }

        public List<ProblemIdea> GetAllProblemIdeasForProblemStep(int problemStepID)
        {
            List<ProblemIdea> problemIdeas = new List<ProblemIdea>();

            string[] problemIdeaCols = new string[]
            {
                "ProblemIdeaID",
                "ProblemStepID",
                "ProblemID",
                "ProblemIdeaText"
            };

            string whereClause = "[ProblemStepID] = " + problemStepID;

            var problemIdeaList = _sqlDatabase.Query("ProblemIdeas", problemIdeaCols, whereClause, null, null, null, null);
            if (problemIdeaList != null && problemIdeaList.Count > 0)
            {
                problemIdeaList.MoveToFirst();
                for (var a = 0; a < problemIdeaList.Count; a++)
                {
                    var problemIdeaItem = new ProblemIdea();

                    problemIdeaItem.ProblemIdeaID = problemIdeaList.GetInt(problemIdeaList.GetColumnIndex("ProblemIdeaID"));
                    problemIdeaItem.ProblemStepID = problemIdeaList.GetInt(problemIdeaList.GetColumnIndex("ProblemStepID"));
                    problemIdeaItem.ProblemID = problemIdeaList.GetInt(problemIdeaList.GetColumnIndex("ProblemID"));
                    problemIdeaItem.ProblemIdeaText = problemIdeaList.GetString(problemIdeaList.GetColumnIndex("ProblemIdeaText"));

                    problemIdeaItem.ProsAndCons = GetAllProblemProsAndConsForProblemIdea(problemIdeaItem.ProblemIdeaID);

                    problemIdeaItem.IsNew = false;
                    problemIdeaItem.IsDirty = false;
                    problemIdeas.Add(problemIdeaItem);
                    problemIdeaList.MoveToNext();
                }
            }

            return problemIdeas;
        }

        public List<ProblemProAndCon> GetAllProblemProsAndConsForProblemIdea(int problemIdeaID)
        {
            List<ProblemProAndCon> problemProsAndCons = new List<ProblemProAndCon>();

            string[] problemProsAndConsCols = new string[]
            {
                "ProblemProAndConID",
                "ProblemIdeaID",
                "ProblemStepID",
                "ProblemID",
                "ProblemProAndConText",
                "ProblemProAndConType"
            };

            string whereClause = "[ProblemIdeaID] = " + problemIdeaID;

            var problemProAndConList = _sqlDatabase.Query("ProblemProsAndCons", problemProsAndConsCols, whereClause, null, null, null, "ProblemProAndConType DESC");
            if (problemProAndConList != null && problemProAndConList.Count > 0)
            {
                problemProAndConList.MoveToFirst();
                for (var a = 0; a < problemProAndConList.Count; a++)
                {
                    var problemProAndConItem = new ProblemProAndCon();

                    problemProAndConItem.ProblemProAndConID = problemProAndConList.GetInt(problemProAndConList.GetColumnIndex("ProblemProAndConID"));
                    problemProAndConItem.ProblemIdeaID = problemProAndConList.GetInt(problemProAndConList.GetColumnIndex("ProblemIdeaID"));
                    problemProAndConItem.ProblemStepID = problemProAndConList.GetInt(problemProAndConList.GetColumnIndex("ProblemStepID"));
                    problemProAndConItem.ProblemID = problemProAndConList.GetInt(problemProAndConList.GetColumnIndex("ProblemID"));
                    problemProAndConItem.ProblemProAndConText = problemProAndConList.GetString(problemProAndConList.GetColumnIndex("ProblemProAndConText"));
                    problemProAndConItem.ProblemProAndConType = (ConstantsAndTypes.PROCON_TYPES)problemProAndConList.GetInt(problemProAndConList.GetColumnIndex("ProblemProAndConType"));
                    problemProAndConItem.IsNew = false;
                    problemProAndConItem.IsDirty = false;
                    problemProsAndCons.Add(problemProAndConItem);
                    problemProAndConList.MoveToNext();
                }
            }

            return problemProsAndCons;
        }

        public List<SolutionPlan> GetAllSolutionPlans()
        {
            List<SolutionPlan> solutionPlanSteps = new List<SolutionPlan>();

            string[] solutionPlanStepCols = new string[]
            {
                "SolutionPlanID",
                "ProblemIdeaID",
                "SolutionStep",
                "PriorityOrder"
            };


            var solutionPlanStepList = _sqlDatabase.Query("SolutionPlans", solutionPlanStepCols, null, null, null, null, "PriorityOrder");
            if (solutionPlanStepList != null && solutionPlanStepList.Count > 0)
            {
                solutionPlanStepList.MoveToFirst();
                for (var a = 0; a < solutionPlanStepList.Count; a++)
                {
                    var solutionPlanStepItem = new SolutionPlan();
                    solutionPlanStepItem.SolutionPlanID = solutionPlanStepList.GetInt(solutionPlanStepList.GetColumnIndex("SolutionPlanID"));
                    solutionPlanStepItem.ProblemIdeaID = solutionPlanStepList.GetInt(solutionPlanStepList.GetColumnIndex("ProblemIdeaID"));
                    solutionPlanStepItem.SolutionStep = solutionPlanStepList.GetString(solutionPlanStepList.GetColumnIndex("SolutionStep"));
                    solutionPlanStepItem.PriorityOrder = solutionPlanStepList.GetInt(solutionPlanStepList.GetColumnIndex("PriorityOrder"));

                    solutionPlanStepItem.IsNew = false;
                    solutionPlanStepItem.IsDirty = false;
                    solutionPlanSteps.Add(solutionPlanStepItem);
                    solutionPlanStepList.MoveToNext();
                }
            }

            return solutionPlanSteps;
        }

        public ProblemIdea GetIdea(int ideaID)
        {
            ProblemIdea idea = null;

            string[] ideaCols =
            {
                "ProblemIdeaID",
                "ProblemStepID",
                "ProblemID",
                "ProblemIdeaText"
            };

            var whereClause = "[ProblemIdeaID] = " + ideaID.ToString();

            var ideaItem = _sqlDatabase.Query("ProblemIdeas", ideaCols, whereClause, null, null, null, null);
            if(ideaItem != null && ideaItem.Count > 0)
            {
                ideaItem.MoveToFirst();
                idea = new ProblemIdea();
                idea.IsDirty = false;
                idea.IsNew = false;
                idea.ProblemID = ideaItem.GetInt(ideaItem.GetColumnIndex("ProblemID"));
                idea.ProblemIdeaID = ideaItem.GetInt(ideaItem.GetColumnIndex("ProblemIdeaID"));
                idea.ProblemIdeaText = ideaItem.GetString(ideaItem.GetColumnIndex("ProblemIdeaText"));
            }

            return idea;
        }

        public SolutionReview GetSolutionReviewForIdea(int ideaID)
        {
            SolutionReview review = null;

            string[] reviewCols =
            {
                "SolutionReviewID",
                "ProblemIdeaID",
                "ReviewText",
                "Achieved",
                "AchievedDate"
            };

            var whereClause = "[ProblemIdeaID] = " + ideaID.ToString();

            var reviewItem = _sqlDatabase.Query("SolutionReviews", reviewCols, whereClause, null, null, null, null);
            if(reviewItem != null && reviewItem.Count > 0)
            {
                reviewItem.MoveToFirst();
                review = new SolutionReview();
                review.IsDirty = false;
                review.IsNew = false;
                review.SolutionReviewID = reviewItem.GetInt(reviewItem.GetColumnIndex("SolutionReviewID"));
                review.ProblemIdeaID = reviewItem.GetInt(reviewItem.GetColumnIndex("ProblemIdeaID"));
                review.ReviewText = reviewItem.GetString(reviewItem.GetColumnIndex("ReviewText"));
                review.Achieved = (true && reviewItem.GetInt(reviewItem.GetColumnIndex("Achieved")) > 0);
                review.AchievedDate = Convert.ToDateTime(reviewItem.GetString(reviewItem.GetColumnIndex("AchievedDate")));
            }

            return review;
        }

        public void RemoveSolutionForIdea(int ideaID)
        {
            //removing the solution plan means removing the SolutionPlan and the SolutionReview
            //entries for this idea
            if (_sqlDatabase != null && _sqlDatabase.IsOpen)
            {
                var sql = "";
                try
                {
                    sql = "DELETE FROM [SolutionPlans] WHERE ProblemIdeaID = " + ideaID;
                    _sqlDatabase.ExecSQL(sql);
                }
                catch(Exception e)
                {
                    Log.Error(TAG, "RemoveSolutionForIdea: Attempting to delete from SolutionPlans, Exception - " + e.Message);
                    throw new Exception("Error removing SolutionPlan for idea ID - " + ideaID.ToString() + " : " + e.Message, e);
                }
                try
                {
                    sql = "DELETE FROM [SolutionReviews] WHERE ProblemIdeaID = " + ideaID;
                    _sqlDatabase.ExecSQL(sql);
                }
                catch(Exception e2)
                {
                    Log.Error(TAG, "RemoveSolutionForIdea: Attempting to delete from SolutionReviews, Exception - " + e2.Message);
                    throw new Exception("Error removing SolutionReview for idea ID - " + ideaID.ToString() + " : " + e2.Message, e2);
                }
            }
        }

        public List<Affirmation> GetAllAffirmations()
        {
            List<Affirmation> affirmations = new List<Affirmation>();

            string[] affirmationCols = new string[]
            {
                "AffirmationID",
                "AffirmationText"
            };


            var affirmationList = _sqlDatabase.Query("Affirmations", affirmationCols, null, null, null, null, null);
            if (affirmationList != null && affirmationList.Count > 0)
            {
                affirmationList.MoveToFirst();
                for (var a = 0; a < affirmationList.Count; a++)
                {
                    var affirmationItem = new Affirmation();
                    affirmationItem.AffirmationID = affirmationList.GetInt(affirmationList.GetColumnIndex("AffirmationID"));
                    affirmationItem.AffirmationText = affirmationList.GetString(affirmationList.GetColumnIndex("AffirmationText"));

                    affirmationItem.IsNew = false;
                    affirmationItem.IsDirty = false;
                    affirmations.Add(affirmationItem);
                    affirmationList.MoveToNext();
                }
            }

            return affirmations;
        }

        public List<Imagery> GetAllImages()
        {
            List<Imagery> images = new List<Imagery>();

            string[] imageryCols = new string[]
            {
                "ImageryID",
                "ImageryURI",
                "ImageryComment"
            };

            var imagesList = _sqlDatabase.Query("Imagery", imageryCols, null, null, null, null, null);
            if(imagesList != null && imagesList.Count > 0)
            {
                imagesList.MoveToFirst();
                for(var a = 0; a < imagesList.Count; a++)
                {
                    var imageItem = new Imagery();
                    imageItem.ImageryID = imagesList.GetInt(imagesList.GetColumnIndex("ImageryID"));
                    imageItem.ImageryURI = imagesList.GetString(imagesList.GetColumnIndex("ImageryURI"));
                    imageItem.ImageryComment = imagesList.GetString(imagesList.GetColumnIndex("ImageryComment"));

                    imageItem.IsNew = false;
                    imageItem.IsDirty = false;

                    images.Add(imageItem);

                    imagesList.MoveToNext();
                }
            }

            return images;
        }

        public List<PlayList> GetAllPlayLists()
        {
            List<PlayList> playLists = new List<PlayList>();

            string[] playListsCols = new string[]
            {
                "PlayListID",
                "PlayListName",
                "PlayListTrackCount"
            };

            var playListList = _sqlDatabase.Query("PlayLists", playListsCols, null, null, null, null, null);
            if (playListList != null && playListList.Count > 0)
            {
                playListList.MoveToFirst();
                for (var a = 0; a < playListList.Count; a++)
                {
                    var playListsItem = new PlayList();
                    playListsItem.PlayListID = playListList.GetInt(playListList.GetColumnIndex("PlayListID"));
                    playListsItem.PlayListName = playListList.GetString(playListList.GetColumnIndex("PlayListName"));
                    playListsItem.PlayListTrackCount = playListList.GetInt(playListList.GetColumnIndex("PlayListTrackCount"));

                    //Log.Info(TAG, "GetAllPlayLists: Found playlist - " + playListsItem.PlayListName + ", with ID - " + playListsItem.PlayListID);
                    playListsItem.PlayListTracks = GetAllTracksForPlayList(playListsItem.PlayListID);

                    playListsItem.IsNew = false;
                    playListsItem.IsDirty = false;

                    playLists.Add(playListsItem);

                    playListList.MoveToNext();
                }
            }

            return playLists;
        }

        private List<Track> GetAllTracksForPlayList(int playListID)
        {
            List<Track> tracks = new List<Track>();

            string[] trackCols = new string[]
            {
                "TrackID",
                "PlayListID",
                "TrackName",
                "TrackArtist",
                "TrackDuration",
                "TrackOrderNumber",
                "TrackUri"
            };

            var whereClause = "PlayListID = " + playListID;

            var trackList = _sqlDatabase.Query("Tracks", trackCols, whereClause, null, null, null, null);
            if (trackList != null && trackList.Count > 0)
            {
                trackList.MoveToFirst();
                for (var a = 0; a < trackList.Count; a++)
                {
                    var trackItem = new Track();
                    trackItem.TrackID = trackList.GetInt(trackList.GetColumnIndex("TrackID"));
                    trackItem.PlayListID = trackList.GetInt(trackList.GetColumnIndex("PlayListID"));
                    trackItem.TrackName = trackList.GetString(trackList.GetColumnIndex("TrackName"));
                    trackItem.TrackArtist = trackList.GetString(trackList.GetColumnIndex("TrackArtist"));
                    trackItem.TrackDuration = trackList.GetFloat(trackList.GetColumnIndex("TrackDuration"));
                    trackItem.TrackOrderNumber = trackList.GetInt(trackList.GetColumnIndex("TrackOrderNumber"));
                    trackItem.TrackUri = trackList.GetString(trackList.GetColumnIndex("TrackUri"));

                    trackItem.IsNew = false;
                    trackItem.IsDirty = false;

                    tracks.Add(trackItem);

                    trackList.MoveToNext();
                }
            }

            return tracks;

        }

        public void TempDeleteMedication()
        {
            var sql = "DELETE FROM [Medication];";

            if (_sqlDatabase != null)
            {
                _sqlDatabase.ExecSQL(sql);

                sql = "DELETE FROM [MedicationReminder];";
                _sqlDatabase.ExecSQL(sql);

                sql = "DELETE FROM [MedicationTime];";
                _sqlDatabase.ExecSQL(sql);

                sql = "DELETE FROM [Prescription];";
                _sqlDatabase.ExecSQL(sql);

                sql = "DELETE FROM [MedicationSpread];";
                _sqlDatabase.ExecSQL(sql);
            }
        }

        public void GetAllAppointmentsForDate(DateTime theDate)
        {
            if (GlobalData.Appointments == null)
            {
                GlobalData.Appointments = new List<Appointments>();
            }

            string[] arrColumns = new string[7];

            arrColumns[0] = "AppointmentID";
            arrColumns[1] = "AppointmentDate";
            arrColumns[2] = "AppointmentType";
            arrColumns[3] = "Location";
            arrColumns[4] = "WithWhom";
            arrColumns[5] = "AppointmentTime";
            arrColumns[6] = "Notes";

            try
            {
                var appointmentData = _sqlDatabase.Query("Appointments", arrColumns, "AppointmentDate = '" + string.Format("{0:yyyy-MM-dd 00:00:00}", theDate) + "'", null, null, null, null);
                if (appointmentData != null)
                {
                    GlobalData.Appointments.Clear();
                    var count = appointmentData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllAppointmentsForDate: Found " + count.ToString() + " Appointment items for date " + theDate.ToShortDateString());
                        appointmentData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var appointmentID = appointmentData.GetInt(appointmentData.GetColumnIndex("AppointmentID"));
                            var appointment = new Appointments();
                            appointment.AppointmentID = appointmentID;
                            appointment.AppointmentDate = theDate;
                            appointment.AppointmentType = appointmentData.GetInt(appointmentData.GetColumnIndex("AppointmentType"));
                            appointment.Location = appointmentData.GetString(appointmentData.GetColumnIndex("Location"));
                            appointment.WithWhom = appointmentData.GetString(appointmentData.GetColumnIndex("WithWhom"));
                            appointment.AppointmentTime = Convert.ToDateTime(appointmentData.GetString(appointmentData.GetColumnIndex("AppointmentTime")));
                            appointment.Notes = appointmentData.GetString(appointmentData.GetColumnIndex("Notes"));
                            appointment.IsDirty = false;
                            appointment.IsNew = false;
                            GlobalData.Appointments.Add(appointment);
                            appointmentData.MoveToNext();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllAppointmentsForDate: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Appointments for Date - " + e.Message, e.InnerException);
            }
        }

        public List<Appointments> GetAllAppointmentsForMonth(int year, int month)
        {
            string[] arrColumns = new string[7];

            arrColumns[0] = "AppointmentID";
            arrColumns[1] = "AppointmentDate";
            arrColumns[2] = "AppointmentType";
            arrColumns[3] = "Location";
            arrColumns[4] = "WithWhom";
            arrColumns[5] = "AppointmentTime";
            arrColumns[6] = "Notes";

            List<Appointments> monthAppointments = new List<Appointments>();
            try
            {
                DateTime startDate = new DateTime(year, month, 1);
                var startString = string.Format("{0:yyyy-MM-dd}", startDate);
                DateTime endDate = new DateTime(year, month, DateHelper.GetDaysInMonth(month));
                var endString = string.Format("{0:yyyy-MM-dd}", endDate);

                string whereClause = "[AppointmentDate] BETWEEN '" + startString + " 00:00:00' AND '" + endString + " 23:59:59'";
                var appointmentData = _sqlDatabase.Query("Appointments", arrColumns, whereClause, null, null, null, null);
                if (appointmentData != null)
                {
                    var count = appointmentData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllAppointmentsForMonth: Found " + count.ToString() + " Appointment items for month " + month.ToString());
                        appointmentData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var appointmentID = appointmentData.GetInt(appointmentData.GetColumnIndex("AppointmentID"));
                            var appointment = new Appointments();
                            appointment.AppointmentID = appointmentID;
                            appointment.AppointmentDate = Convert.ToDateTime(appointmentData.GetString(appointmentData.GetColumnIndex("AppointmentDate")));
                            appointment.AppointmentType = appointmentData.GetInt(appointmentData.GetColumnIndex("AppointmentType"));
                            appointment.Location = appointmentData.GetString(appointmentData.GetColumnIndex("Location"));
                            appointment.WithWhom = appointmentData.GetString(appointmentData.GetColumnIndex("WithWhom"));
                            appointment.AppointmentTime = Convert.ToDateTime(appointmentData.GetString(appointmentData.GetColumnIndex("AppointmentTime")));
                            appointment.Notes = appointmentData.GetString(appointmentData.GetColumnIndex("Notes"));
                            appointment.IsDirty = false;
                            appointment.IsNew = false;
                            monthAppointments.Add(appointment);
                            Log.Info(TAG, "GetAllAppointmentsForMonth: Added appointment - \r\n" + 
                                "AppointmentID - " + appointment.AppointmentID + "\r\n" +
                                "AppointmentDate - " + appointment.AppointmentDate.ToShortDateString() + "\r\n" +
                                "AppointmentTime - " + appointment.AppointmentTime.ToShortTimeString() + "\r\n" +
                                "Location - " + appointment.Location + "\r\n" +
                                "With - " + appointment.WithWhom + "\r\n");
                            appointmentData.MoveToNext();
                        }
                    }
                }
                return monthAppointments;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllAppointmentsForMonth: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Appointments for Month - " + e.Message, e.InnerException);
            }
        }

        public void GetAllResourceMedicationTypes()
        {
            if (GlobalData.ResourceMedicationTypes == null)
            {
                GlobalData.ResourceMedicationTypes = new List<ResourceMedicationType>();
            }

            string[] arrColumns = new string[3];

            arrColumns[0] = "ID";
            arrColumns[1] = "MedicationTypeTitle";
            arrColumns[2] = "MedicationTypeDescription";

            try
            {
                var medicationTypeData = _sqlDatabase.Query("ResourceMedicationTypes", arrColumns, null, null, null, null, null);
                if (medicationTypeData != null)
                {
                    GlobalData.ResourceMedicationTypes.Clear();
                    var count = medicationTypeData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllResourceMedicationTypes: Found " + count.ToString() + " Medication types ");
                        medicationTypeData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var medicationTypeID = medicationTypeData.GetInt(medicationTypeData.GetColumnIndex("ID"));
                            var medicationType = new ResourceMedicationType();

                            medicationType.ID = medicationTypeID;
                            medicationType.MedicationTypeTitle = medicationTypeData.GetString(medicationTypeData.GetColumnIndex("MedicationTypeTitle"));
                            medicationType.MedicationTypeDescription = medicationTypeData.GetString(medicationTypeData.GetColumnIndex("MedicationTypeDescription"));
                            medicationType.IsNew = false;
                            medicationType.IsDirty = false;

                            //grab all the items of this type
                            medicationType.MedicationItems = GetItemsForResourceMedicationType(medicationTypeID);

                            GlobalData.ResourceMedicationTypes.Add(medicationType);

                            medicationTypeData.MoveToNext();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllResourceMedicationTypes: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Resource Medication Types", e.InnerException);
            }
        }

        private List<ResourceMedicationItem> GetItemsForResourceMedicationType(int medicationTypeId)
        {
            List<ResourceMedicationItem> medicationItems = new List<ResourceMedicationItem>();

            string[] arrColumns = new string[6];

            arrColumns[0] = "ID";
            arrColumns[1] = "MedicationTypeID";
            arrColumns[2] = "MedicationName";
            arrColumns[3] = "MedicationDescription";
            arrColumns[4] = "SideEffects";
            arrColumns[5] = "Dosage";

            try
            {
                var medicationItemData = _sqlDatabase.Query("ResourceMedicationItem", arrColumns, "[MedicationTypeID] = " + medicationTypeId.ToString(), null, null, null, null);
                if (medicationItemData != null)
                {
                    var count = medicationItemData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetItemsForResourceMedicationType: Found " + count.ToString() + " Medication items for Type Id - " + medicationTypeId.ToString());
                        medicationItemData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var medicationItemID = medicationItemData.GetInt(medicationItemData.GetColumnIndex("ID"));
                            var medicationItem = new ResourceMedicationItem();

                            medicationItem.ID = medicationItemID;
                            medicationItem.MedicationTypeID = medicationItemData.GetInt(medicationItemData.GetColumnIndex("MedicationTypeID"));
                            medicationItem.MedicationItemTitle = medicationItemData.GetString(medicationItemData.GetColumnIndex("MedicationName"));
                            medicationItem.MedicationItemDescription = medicationItemData.GetString(medicationItemData.GetColumnIndex("MedicationDescription"));
                            medicationItem.SideEffects = medicationItemData.GetString(medicationItemData.GetColumnIndex("SideEffects"));
                            medicationItem.Dosage = medicationItemData.GetString(medicationItemData.GetColumnIndex("Dosage"));

                            medicationItem.IsNew = false;
                            medicationItem.IsDirty = false;

                            medicationItems.Add(medicationItem);

                            medicationItemData.MoveToNext();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetItemsForResourceMedicationType: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Resource Medication Items for Id " + medicationTypeId.ToString() + " - " + e.Message, e.InnerException);
            }

            return medicationItems;
        }


        public void OneTimeAddMedicationTypes(Context context)
        {
            int medicationTypeID = -1;

            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //SSRIs
                    //----------------------------------------------------------
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleSSRIs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionSSRIs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted SSRI with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsSSRI(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //SNRIs
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleSNRIs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionSNRIs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted SSRI with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsSNRI(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //SMAS
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleSerotoninmodulatorsandstimulators));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionSerotoninmodulatorsandstimulators));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted SMAS with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsSMAS(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //SARIs
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleSARIs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionSARIs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted SMAS with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsSARI(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //NRIs
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleNRIs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionNRIs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted NRI with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsNRI(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //TCAs
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleTCAs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionTCAs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted TCA with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsTCA(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //TeCAs
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleTeCAs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionTeCAs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted TeCA with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsTeCA(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //MAOIs
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleMAOIs));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionMAOIs));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted MAOI with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsMAOI(context, medicationTypeID);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Atypical antipsychotics
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeTitle", context.GetString(Resource.String.ResourcesMedicationTypeTitleAtypicalantipsychotics));
                    values.Put("MedicationTypeDescription", context.GetString(Resource.String.ResourcesMedicationTypeDescriptionAtypicalantipsychotics));

                    medicationTypeID = (int)_sqlDatabase.Insert("ResourceMedicationTypes", null, values);
                    Log.Info(TAG, "OneTimeAddMedicationTypes: Inserted Atypical antipsychotic with ID " + medicationTypeID.ToString());

                    OneTimeAddMedicationItemsAAP(context, medicationTypeID);
                    //----------------------------------------------------------
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationTypes: Exception - " + e.Message);
                throw new Exception("An error occurred Adding medication types - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsAAP(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Amisulpride
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleAmisulpride));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesAmisulpride));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsAmisulpride));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageAmisulpride));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Lurasidone
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleLurasidone));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesLurasidone));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsLurasidone));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageLurasidone));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Quetiapine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleQuetiapine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesQuetiapine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsQuetiapine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageQuetiapine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsAAP: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsAAP: Exception - " + e.Message);
                throw new Exception("An error occurred Adding AAP medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsMAOI(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Isocarboxazid
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleIsocarboxazid));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesIsocarboxazid));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsIsocarboxazid));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageIsocarboxazid));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Phenelzine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitlePhenelzine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesPhenelzine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsPhenelzine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosagePhenelzine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Tranylcypromine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleTranylcypromine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesTranylcypromine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsTranylcypromine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageTranylcypromine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Moclobemide
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleMoclobemide));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesMoclobemide));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsMoclobemide));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageMoclobemide));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsMAOI: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsMAOI: Exception - " + e.Message);
                throw new Exception("An error occurred Adding MAOI medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsTeCA(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Amoxapine
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleAmoxapine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesAmoxapine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsAmoxapine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageAmoxapine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Maprotiline
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleMaprotiline));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesMaprotiline));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsMaprotiline));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageMaprotiline));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Mianserin
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleMianserin));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesMianserin));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsMianserin));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageMianserin));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Mirtazapine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleMirtazapine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesMirtazapine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsMirtazapine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageMirtazapine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsTeCA: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsTeCA: Exception - " + e.Message);
                throw new Exception("An error occurred Adding TeCA medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsTCA(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Amitriptyline
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleAmitriptyline));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesAmitriptyline));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsAmitriptyline));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageAmitriptyline));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Amitriptylinoxide
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleAmitriptylinoxide));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesAmitriptylinoxide));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsAmitriptylinoxide));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageAmitriptylinoxide));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Clomipramine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleClomipramine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesClomipramine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsClomipramine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageClomipramine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Dibenzepin
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleDibenzepin));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesDibenzepin));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsDibenzepin));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageDibenzepin));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Dosulepin
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleDosulepin));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesDosulepin));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsDosulepin));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageDosulepin));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Doxepin
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleDoxepin));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesDoxepin));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsDoxepin));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageDoxepin));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Imipramine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleImipramine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesImipramine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsImipramine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageImipramine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Lofepramine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleLofepramine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesLofepramine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsLofepramine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageLofepramine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Melitracen
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleMelitracen));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesMelitracen));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsMelitracen));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageMelitracen));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Nortriptyline
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleNortriptyline));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesNortriptyline));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsNortriptyline));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageNortriptyline));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Protriptyline
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleProtriptyline));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesProtriptyline));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsProtriptyline));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageProtriptyline));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Trimipramine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleTrimipramine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesTrimipramine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsTrimipramine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageTrimipramine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsTCA: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsTCA: Exception - " + e.Message);
                throw new Exception("An error occurred Adding TCA medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsNRI(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Reboxetine
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleReboxetine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesReboxetine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsReboxetine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageReboxetine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Viloxazine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleViloxazine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesViloxazine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsViloxazine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageViloxazine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsNRI: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsNRI: Exception - " + e.Message);
                throw new Exception("An error occurred Adding NRI medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsSARI(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Nefazodone
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleNefazodone));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesNefazodone));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsNefazodone));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageNefazodone));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Trazodone
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleTrazodone));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesTrazodone));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsTrazodone));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageTrazodone));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsSARI: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsSARI: Exception - " + e.Message);
                throw new Exception("An error occurred Adding SARI medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsSMAS(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Vilazodone
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleVilazodone));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesVilazodone));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsVilazodone));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageVilazodone));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Vortioxetine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleVortioxetine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesVortioxetine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsVortioxetine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageVortioxetine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsSMAS: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsSMAS: Exception - " + e.Message);
                throw new Exception("An error occurred Adding SMAS medication item - " + e.Message, e.InnerException);
            }
        }

        private void OneTimeAddMedicationItemsSNRI(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Desvenlafaxine
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleDesvenlafaxine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesDesvenlafaxine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsDesvenlafaxine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageDesvenlafaxine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Duloxetine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleDuloxetine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesDuloxetine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsDuloxetine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageDuloxetine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Levomilnacipran
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleLevomilnacipran));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesLevomilnacipran));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsLevomilnacipran));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageLevomilnacipran));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Milnacipran
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleMilnacipran));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesMilnacipran));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsMilnacipran));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageMilnacipran));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Venlafaxine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleVenlafaxine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesVenlafaxine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsVenlafaxine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageVenlafaxine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsSNRI: Database null or not opened");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsSNRI: Exception - " + e.Message);
                throw new Exception("An error occurred Adding SNRI medication item - " + e.Message, e.InnerException);
            }
        }

        public void OneTimeAddMedicationItemsSSRI(Context context, int medicationID)
        {
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    ContentValues values = new ContentValues();

                    //----------------------------------------------------------
                    //Citalopram
                    //----------------------------------------------------------
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleCitalopram));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesCitalopram));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsCitalopram));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageCitalopram));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Escitalopram
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleEscitalopram));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesEscitalopram));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsEscitalopram));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageEscitalopram));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Paroxetine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleParoxetine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesParoxetine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsParoxetine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageParoxetine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Fluoxetine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleFluoxetine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesFluoxetine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsFluoxetine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageFluoxetine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Fluvoxamine
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleFluvoxamine));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesFluvoxamine));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsFluvoxamine));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageFluvoxamine));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------

                    //----------------------------------------------------------
                    //Sertraline
                    //----------------------------------------------------------
                    values = new ContentValues();
                    values.Put("MedicationTypeID", medicationID);
                    values.Put("MedicationName", context.GetString(Resource.String.ResourcesMedicationNameTitleSertraline));
                    values.Put("MedicationDescription", context.GetString(Resource.String.ResourcesMedicationUsesSertraline));
                    values.Put("SideEffects", context.GetString(Resource.String.ResourcesMedicationSideEffectsSertraline));
                    values.Put("Dosage", context.GetString(Resource.String.ResourcesMedicationDosageSertraline));
                    _sqlDatabase.Insert("ResourceMedicationItem", null, values);
                    //----------------------------------------------------------
                }
                else
                {
                    Log.Error(TAG, "OneTimeAddMedicationItemsSSRI: Database null or not opened");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OneTimeAddMedicationItemsSSRI: Exception - " + e.Message);
                throw new Exception("An error occurred Adding SSRI medication item - " + e.Message, e.InnerException);
            }
        }

        public void GetAllResourceConditions()
        {
            if (GlobalData.ResourceConditions == null)
            {
                GlobalData.ResourceConditions = new List<ResourceCondition>();
            }

            string[] arrColumns = new string[4];

            arrColumns[0] = "ID";
            arrColumns[1] = "ConditionTitle";
            arrColumns[2] = "ConditionDescription";
            arrColumns[3] = "ConditionCitation";

            try
            {
                var conditionData = _sqlDatabase.Query("ResourceConditions", arrColumns, null, null, null, null, null);
                if (conditionData != null)
                {
                    GlobalData.ResourceConditions.Clear();
                    var count = conditionData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllResourceConditions: Found " + count.ToString() + " conditions");
                        conditionData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var condition = new ResourceCondition();

                            condition.ConditionId = conditionData.GetInt(conditionData.GetColumnIndex("ID"));
                            condition.ConditionTitle = conditionData.GetString(conditionData.GetColumnIndex("ConditionTitle"));
                            condition.ConditionDescription = conditionData.GetString(conditionData.GetColumnIndex("ConditionDescription"));
                            condition.ConditionCitation = conditionData.GetString(conditionData.GetColumnIndex("ConditionCitation"));
                            condition.IsNew = false;
                            condition.IsDirty = false;

                            GlobalData.ResourceConditions.Add(condition);

                            conditionData.MoveToNext();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllResourceConditions: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Resource Conditions", e.InnerException);
            }
        }

        public void GetAllSettings()
        {
            if (GlobalData.Settings == null)
            {
                GlobalData.Settings = new List<Setting>();
            }

            string[] arrColumns = new string[3];

            arrColumns[0] = "ID";
            arrColumns[1] = "SettingKey";
            arrColumns[2] = "SettingValue";

            try
            {
                var settingData = _sqlDatabase.Query("Settings", arrColumns, null, null, null, null, null);
                if (settingData != null)
                {
                    GlobalData.Settings.Clear();
                    var count = settingData.Count;
                    if (count > 0)
                    {
                        Log.Info(TAG, "GetAllSettings: Found " + count.ToString() + " settings");
                        settingData.MoveToFirst();
                        for (var loop = 0; loop < count; loop++)
                        {
                            var setting = new Setting();

                            setting.SettingId = settingData.GetInt(settingData.GetColumnIndex("ID"));
                            setting.SettingKey = settingData.GetString(settingData.GetColumnIndex("SettingKey"));
                            setting.SettingValue = settingData.GetString(settingData.GetColumnIndex("SettingValue"));
                            setting.IsNew = false;
                            setting.IsDirty = false;

                            GlobalData.Settings.Add(setting);

                            settingData.MoveToNext();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetAllSettings: Exception - " + e.Message);
                throw new Exception("An error occurred Loading Settings", e.InnerException);
            }
        }

        private void AddOneTimeSpanish()
        {
            string sql = "";
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Deprimido', " +
                            "'ESP', " +
                            "'true')";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Deprimido for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Ansioso', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Ansioso for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Enojado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Enojado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Culpable', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Culpable for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Avergonzado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Avergonzado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Triste', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Triste for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Desconcertado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Desconcertado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Emocionado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Emocionado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Asustado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Asustado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Irritado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Irritado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Inseguro', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Inseguro for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Orgulloso', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Orgulloso for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Loca', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Loca for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Lleno de pánico', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Lleno de pánico for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Frustrado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Frustrado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Nervioso', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Nervioso for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Disgustado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Disgustado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Herir', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Herir for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Alegre', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Alegre for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Decepcionado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Decepcionado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Temeroso', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Temeroso for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Contento', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Contento for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Amoroso', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Amoroso for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Humillado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Humillado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Jovial', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Jovial for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Melancolía', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Melancolía for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Coqueto', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Coqueto for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Irritado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Irritado for country code ESP");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Desesperado', " +
                            "'ESP', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeSpanish: Inserted Desesperado for country code ESP");

                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AddOneTimeSpanish: Exception - " + e.Message);
            }
        }

        private void AddOneTimeFrench()
        {
            string sql = "";
            try
            {
                if (_sqlDatabase != null && _sqlDatabase.IsOpen)
                {
                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Déprimé', " +
                            "'FRA', " +
                            "'true')";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Déprimé for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Anxieux', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Anxieux for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'En colère', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted En colère for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Coupable', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Coupable for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Embarrassé', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Embarrassé for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Triste', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Triste for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Perplexe', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Perplexe for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Excité', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Excité for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Effrayé', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Effrayé for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Irritée', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Irritée for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Insécurité', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Insécurité for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Fier', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Fier for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Fou', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Fou for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Plein de panique', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Plein de panique for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Frustré', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Frustré for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Nerveux', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Nerveux for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Dégoûté', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Dégoûté for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Mal', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Mal for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Joyeux', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Joyeux for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Déçu', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Déçu for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Craintif', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Craintif for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Heureux', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Heureux for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Aimer', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Aimer for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Humilié', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Humilié for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Jovial', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Jovial for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Mélancolie', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Mélancolie for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Coquettage', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Coquettage for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Irritée', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Irritée for country code FRA");

                    sql = @"INSERT INTO [MoodList] (" +
                            "[MoodName], " +
                            "[IsoCountry], " +
                            "[IsDefault]) " +
                            "VALUES (" +
                            "'Désespéré', " +
                            "'FRA', " +
                            "'true');";
                    _sqlDatabase.ExecSQL(sql);
                    Log.Info(TAG, "AddOneTimeFrench: Inserted Désespéré for country code FRA");

                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "AddOneTimeFrench: Exception - " + e.Message);
            }
        }
    }
}