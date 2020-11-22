using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using com.spanyardie.MindYourMood.Model.LowLevel;
using Android.Util;

namespace com.spanyardie.MindYourMood.Model
{
    public class ThoughtRecord : ThoughtRecordBase
    {
        public static string TAG = "M:ThoughtRecord";

        public ThoughtRecord()
        {
            Log.Info(TAG, "Constructor: initialising Lists");
            InitialiseLists();

            IsNew = true;
        }

        private void InitialiseLists()
        {
            try
            {
                Moods = new List<Mood>();
                AutomaticThoughtsList = new List<AutomaticThoughts>();
                EvidenceForHotThoughtList = new List<EvidenceForHotThought>();
                EvidenceAgainstHotThoughtList = new List<EvidenceAgainstHotThought>();
                AlternativeThoughtsList = new List<AlternativeThoughts>();
                RerateMoodList = new List<RerateMood>();
                Log.Info(TAG, "InitialiseLists: Completed Initialising Lists successfully");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "InitialiseLists: Error occurred Initialising data lists - " + e.Message);
            }
        }

        public void AddSituation(Situation newSituation)
        {
            if (newSituation != null)
            {
                Situation = newSituation;
                IsDirty = true;
            }
        }

        public void AddMood(Mood newMood)
        {
            try
            {
                if (newMood != null)
                {
                    Moods.Add(newMood);
                    IsDirty = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to add Mood failed - " + e.Message);
            }
        }

        public void AddAutomaticThought(AutomaticThoughts newAutomaticThought)
        {
            try
            {
                if (newAutomaticThought != null)
                {
                    AutomaticThoughtsList.Add(newAutomaticThought);
                    IsDirty = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to add Automatic Thought failed - " + e.Message);
            }
        }

        public void AddEvidenceForHotThought(EvidenceForHotThought newEvidenceForHotThought)
        {
            try
            {
                if (newEvidenceForHotThought != null)
                {
                    EvidenceForHotThoughtList.Add(newEvidenceForHotThought);
                    IsDirty = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to add Evidence for Hot Thought failed - " + e.Message);
            }
        }

        public void AddEvidenceAgainstHotThought(EvidenceAgainstHotThought newEvidenceAgainstHotThought)
        {
            try
            {
                if (newEvidenceAgainstHotThought != null)
                {
                    EvidenceAgainstHotThoughtList.Add(newEvidenceAgainstHotThought);
                    IsDirty = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to add Evidence Against Hot Thought failed - " + e.Message);
            }
        }

        public void AddAlternativeThought(AlternativeThoughts newAlternativeThought)
        {
            try
            {
                if (newAlternativeThought != null)
                {
                    AlternativeThoughtsList.Add(newAlternativeThought);
                    IsDirty = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to add Alternative Thought failed - " + e.Message);
            }
        }

        public void AddRerateMood(RerateMood newRerateMood)
        {
            try
            {
                if (newRerateMood != null)
                {
                    RerateMoodList.Add(newRerateMood);
                    IsDirty = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to add Rerate Mood failed - " + e.Message);
            }
        }

        public void RemoveMood(Mood mood, SQLiteDatabase sqlDatabase)
        {
            try
            {
                //caller will have to warn User that any Rerated mood based on this will also be deleted
                if (mood != null)
                {
                    //Find and Remove any re-rated mood
                    foreach (var reRatedMood in RerateMoodList)
                    {
                        if (reRatedMood.MoodsId == mood.MoodsId)
                        {
                            RerateMoodList.Remove(reRatedMood);
                            break;
                        }
                    }

                    mood.Remove(sqlDatabase);
                    Moods.Remove(mood);
                }
            }
            catch(Exception e)
            {
                throw new Exception("Attempt to Remove Mood failed - " + e.Message);
            }
        }

        public void RemoveAutomaticThought(AutomaticThoughts automaticThought, SQLiteDatabase sqlDatabase)
        {
            try
            {
                //caller will have to warn User that any EvidenceForHotThought and EvidenceAgainstHotThought based on this will also be removed
                if (automaticThought != null)
                {
                    //find and remove any EvidenceForHotThought with this ID
                    foreach(var evidenceForHotThought in EvidenceForHotThoughtList)
                    {
                        if (evidenceForHotThought.AutomaticThoughtsId == automaticThought.AutomaticThoughtsId)
                        {
                            EvidenceForHotThoughtList.Remove(evidenceForHotThought);
                            evidenceForHotThought.Remove(sqlDatabase);
                            break;
                        }
                    }
                    //find and remove any EvidenceAgainstHotThought with this ID
                    foreach(var evidenceAgainstHotThought in EvidenceAgainstHotThoughtList)
                    {
                        if (evidenceAgainstHotThought.AutomaticThoughtsId == automaticThought.AutomaticThoughtsId)
                        {
                            EvidenceAgainstHotThoughtList.Remove(evidenceAgainstHotThought);
                            evidenceAgainstHotThought.Remove(sqlDatabase);
                            break;
                        }
                    }

                    automaticThought.Remove(sqlDatabase);
                    AutomaticThoughtsList.Remove(automaticThought);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to remove Automatic Thought failed - " + e.Message);
            }
        }

        public void RemoveEvidenceForHotThought(EvidenceForHotThought evidenceForHotThought)
        {
            try
            {
                if (evidenceForHotThought != null)
                {
                    EvidenceForHotThoughtList.Remove(evidenceForHotThought);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to remove Evidence for Hot Thought failed - " + e.Message);
            }
        }

        public void RemoveEvidenceAgainstHotThought(EvidenceAgainstHotThought evidenceAgainstHotThought)
        {
            try
            {
                if (evidenceAgainstHotThought != null)
                {
                    EvidenceAgainstHotThoughtList.Remove(evidenceAgainstHotThought);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to remove Evidence against Hot Thought failed - " + e.Message);
            }
        }

        public void RemoveAlternative(AlternativeThoughts alternativeThought)
        {
            try
            {
                if (alternativeThought != null)
                {
                    AlternativeThoughtsList.Remove(alternativeThought);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to remove Alternative Thought failed - " + e.Message);
            }
        }

        public void RemoveRerateMood(RerateMood reratedMood)
        {
            try
            {
                if (reratedMood != null)
                {
                    RerateMoodList.Remove(reratedMood);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to remove Rerate Mood failed - " + e.Message);
            }
        }

        public void RemoveThisThoughtRecord(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                try
                {
                    string commandText = "DELETE FROM ThoughtRecord WHERE [ThoughtRecordId] = " + ThoughtRecordId;
                    sqLiteDatabase.ExecSQL(commandText);
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "RemoveThisThoughtRecord: Exception - " + e.Message);
                    throw new Exception("Command to remove this thought record failed - " + e.Message);
                }
            }
        }

        public void RemoveThoughtRecord(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                //Remove the thought record and you want to remove everything really
                ClearAlternativeThoughts(sqLiteDatabase);
                ClearEvidenceForHotThought(sqLiteDatabase);
                ClearEvidenceAgainstHotThought(sqLiteDatabase);
                ClearAutomaticThoughts(sqLiteDatabase);
                ClearReratedMoods(sqLiteDatabase);
                ClearMoods(sqLiteDatabase);

                Situation.Remove(sqLiteDatabase);

                //remove this record
                RemoveThisThoughtRecord(sqLiteDatabase);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RemoveThoughtRecord: Exception - " + e.Message);
                throw new Exception("Attempt to remove Thought Record failed - " + e.Message);
            }
        }

        private void ClearAlternativeThoughts(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (AlternativeThoughtsList.Count > 0)
                {
                    foreach(var alternativeThought in AlternativeThoughtsList)
                    {
                        if (!alternativeThought.IsNew)
                        {
                            alternativeThought.Remove(sqLiteDatabase);
                        }
                    }
                    AlternativeThoughtsList.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to clear Alternative Thoughts failed - " + e.Message);
            }
        }

        private void ClearAutomaticThoughts(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (AutomaticThoughtsList.Count > 0)
                {
                    foreach(var automaticThought in AutomaticThoughtsList)
                    {
                        if (!automaticThought.IsNew)
                        {
                            automaticThought.Remove(sqLiteDatabase);
                        }
                    }
                    AutomaticThoughtsList.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to clear Automatic Thoughts failed - " + e.Message);
            }
        }

        private void ClearEvidenceForHotThought(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (EvidenceForHotThoughtList.Count > 0)
                {
                    foreach(var evidenceForHotThought in EvidenceForHotThoughtList)
                    {
                        if (!evidenceForHotThought.IsNew)
                        {
                            evidenceForHotThought.Remove(sqLiteDatabase);
                        }
                    }
                    EvidenceForHotThoughtList.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to clear Evidence for Hot Thoughts failed - " + e.Message);
            }
        }

        private void ClearEvidenceAgainstHotThought(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (EvidenceAgainstHotThoughtList.Count > 0)
                {
                    foreach(var evidenceAgainstHotThought in EvidenceAgainstHotThoughtList)
                    {
                        if (!evidenceAgainstHotThought.IsNew)
                        {
                            evidenceAgainstHotThought.Remove(sqLiteDatabase);
                        }
                    }
                    EvidenceAgainstHotThoughtList.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to clear Evidence against Hot Thoughts failed - " + e.Message);
            }
        }

        private void ClearReratedMoods(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (RerateMoodList.Count > 0)
                {
                    foreach(var reratedMood in RerateMoodList)
                    {
                        if (!reratedMood.IsNew)
                        {
                            reratedMood.Remove(sqLiteDatabase);
                        }
                    }
                    RerateMoodList.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to clear Rerate Moods failed - " + e.Message);
            }
        }

        private void ClearMoods(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (Moods.Count > 0)
                {
                    foreach(var mood in Moods)
                    {
                        if (!mood.IsNew)
                        {
                            mood.Remove(sqLiteDatabase);
                        }
                    }
                    Moods.Clear();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Attempt to clear Moods failed - " + e.Message);
            }
        }

        public void Save(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                if (IsNew)
                {
                    Log.Info(TAG, "Save: New record being saved");
                    SaveThoughtRecord(sqLiteDatabase);
                }

                if (IsDirty)
                {
                    UpdateThoughtRecord(sqLiteDatabase);
                }
                if (Situation != null)
                {
                    SaveSituation(sqLiteDatabase);
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Save: Excpetion - " + e.Message);
                throw new Exception("ThoughtRecord encountered an error during Save\r\n" + e.Message);
            }
        }

        private void SaveSituation(SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                Situation.ThoughtRecordId = ThoughtRecordId;
                Situation.Save(sqLiteDatabase);
            }
            catch (Exception e)
            {
                throw new Exception("ThoughtRecord failed to save Situation\r\n" + e.Message);
            }
        }

        private void SaveThoughtRecord(SQLiteDatabase sqLiteDatabase)
        {
            if (sqLiteDatabase.IsOpen)
            {
                Log.Info(TAG, "SaveThoughtRecord: Database is Open");
                if (IsNew)
                {
                    Log.Info(TAG, "SaveThoughtRecord: New record");
                    try
                    {
                        //string commandText = "INSERT INTO ThoughtRecord([RecordDate]) VALUES (CDateTime(#" + RecordDateTime + "#))";
                        string commandText = "INSERT INTO ThoughtRecord([RecordDate]) VALUES ('" + RecordDateTime.ToString() + "')";
                        Log.Info(TAG, "SaveThoughtRecord: Command text is '" + commandText + "'");
                        ContentValues values = new ContentValues();
                        values.Put("RecordDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", RecordDateTime));

                        var retVal = sqLiteDatabase.Insert("ThoughtRecord", null, values);
                        GlobalData.ThoughtRecordId = retVal;
                        ThoughtRecordId = retVal;
                        Log.Info(TAG, "SaveThoughtRecord: retVal assigned to global thoughtrecordId - " + retVal.ToString());
                        Log.Info(TAG, "SaveThoughtRecord: Internal ThoughtRecord Id is " + ThoughtRecordId.ToString());
                        IsNew = false;
                        IsDirty = false;
                    }
                    catch (Exception e)
                    {
                        Log.Error(TAG, "SaveThoughtRecord: Exception - " + e.Message);
                        throw new Exception("Attempt to save Thought Record failed - " + e.Message);
                    }
                }
            }
        }

        private void UpdateThoughtRecord(SQLiteDatabase sqlDatabase)
        {
            try
            {
                if (Situation != null)
                {
                    Situation.Save(sqlDatabase);
                }
            }
            catch (Exception eSituation)
            {
                Log.Error(TAG, "UpdateThoughtRecord: Update of Situation failed - " + eSituation.Message);
                throw new Exception("Attempt to save Situation during update of Thought Record failed - " + eSituation.Message);
            }

            //update entails saving all stuff not currently saved
            try
            {
                if (Moods != null)
                {
                    Log.Info(TAG, "UpdateThoughtRecord: " + Moods.Count.ToString() + " moods to save");
                    foreach (var mood in Moods)
                    {
                        mood.Save(sqlDatabase);
                        Log.Info(TAG, "UpdateThoughtRecord: Mood " + mood.MoodsId.ToString() + " saved");
                    }
                }
            }
            catch (Exception eMoods)
            {
                Log.Error(TAG, "UpdateThoughtRecord: Exception - " + eMoods.Message);
                throw new Exception("Attempt to save Moods during update of Thought Record failed - " + eMoods.Message);
            }

            try
            {
                if (AutomaticThoughtsList != null)
                {
                    Log.Info(TAG, "UpdateThoughtRecord: " + AutomaticThoughtsList.Count.ToString() + " automatic thoughts to save");
                    foreach (var autoThought in AutomaticThoughtsList)
                    {
                        autoThought.Save(sqlDatabase);
                    }
                }
            }
            catch (Exception eAutomaticThoughts)
            {
                Log.Error(TAG, "UpdateThoughtRecord: Exception - " + eAutomaticThoughts.Message);
                throw new Exception("Attempt to save Automatic Thoughts during update of Thought Record failed - " + eAutomaticThoughts.Message);
            }

            try
            {
                if (EvidenceForHotThoughtList != null)
                {
                    foreach(var forHot in EvidenceForHotThoughtList)
                    {
                        forHot.AutomaticThoughtsId = GetAutomaticThoughtIdForHotThought();
                        forHot.Save(sqlDatabase);
                    }
                }
            }
            catch (Exception eEvidenceFor)
            {
                throw new Exception("Attempt to save Evidence for Hot Thought during update of Thought Record failed - " + eEvidenceFor.Message);
            }

            try
            {
                if (EvidenceAgainstHotThoughtList != null)
                {
                    foreach(var againstHot in EvidenceAgainstHotThoughtList)
                    {
                        againstHot.AutomaticThoughtsId = GetAutomaticThoughtIdForHotThought();
                        againstHot.Save(sqlDatabase);
                    }
                }
            }
            catch (Exception eEvidenceAgainst)
            {
                throw new Exception("Attempt to save Evidence against Hot Thought during update of Thought Record failed - " + eEvidenceAgainst.Message);
            }

            try
            {
                if (AlternativeThoughtsList != null)
                {
                    foreach(var altThought in AlternativeThoughtsList)
                    {
                        altThought.Save(sqlDatabase);
                    }
                }
            }
            catch (Exception eAlternative)
            {
                throw new Exception("Attempt to save Alternative Thoughts during update of Thought Record failed - " + eAlternative.Message);
            }

            try
            {
                if (RerateMoodList != null)
                {
                    foreach(var rerate in RerateMoodList)
                    {
                        rerate.MoodsId = GetMoodsIdForMoodListItem(rerate.MoodListId);
                        rerate.Save(sqlDatabase);
                    }
                }
            }
            catch (Exception eRerate)
            {
                throw new Exception("Attempt to save Rerate Moods during update of Thought Record failed - " + eRerate.Message);
            }
        }

        private int GetAutomaticThoughtIdForHotThought()
        {
            try
            {
                if (AutomaticThoughtsList.Count > 0)
                {
                    foreach(var autoThought in AutomaticThoughtsList)
                    {
                        if (autoThought.IsHotThought)
                        {
                            return autoThought.AutomaticThoughtsId;
                        }
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                throw new Exception("Getting Automatic Thought Id for Hot Thought failed - " + e.Message);
            }
        }

        private long GetMoodsIdForMoodListItem(long moodsListId)
        {
            try
            {
                if (Moods.Count > 0)
                {
                    foreach(var mood in Moods)
                    {
                        if (mood.MoodListId == moodsListId)
                        {
                            return mood.MoodsId;
                        }
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                throw new Exception("Getting Moods ID for Mood List Item failed - " + e.Message);
            }
        }

        public void Load(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                IsNew = false;
                IsDirty = false;
                Log.Info(TAG, "Loading...");
                LoadThoughtRecord(id, sqLiteDatabase);
                LoadSituation(id, sqLiteDatabase);
                LoadMoods(id, sqLiteDatabase);
                LoadAutomaticThoughts(id, sqLiteDatabase);
                LoadEvidenceForHotThought(id, sqLiteDatabase);
                LoadEvidenceAgainstHotThought(id, sqLiteDatabase);
                LoadAlternativeThoughts(id, sqLiteDatabase);
                LoadReratedMoods(id, sqLiteDatabase);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Exception - " + e.Message);
                throw new Exception("Unable to Load Thought Record - " + e.Message);
            }
        }

        private void LoadThoughtRecord(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                string commandText = "SELECT [RecordDate] FROM ThoughtRecord WHERE [ThoughtRecordID] = " + id;
                Log.Info(TAG, "command text - " + commandText);
                if (sqLiteDatabase.IsOpen)
                {
                    Log.Info(TAG, "Database is open, running raw query");
                    var data = sqLiteDatabase.RawQuery(commandText, null);

                    if (data != null)
                    {
                        Log.Info(TAG, "Found " + data.Count.ToString() + " items" + (data.Count>1?" found more than 1 item!":""));
                        if (data.MoveToNext())
                        {
                            do
                            {
                                var stringDate = Convert.ToDateTime(data.GetString(0));
                                RecordDateTime = stringDate;
                                ThoughtRecordId = id;
                                Log.Info(TAG, "Retrieved! Date " + RecordDateTime.ToShortDateString() + ", Id " + ThoughtRecordId.ToString());
                            }
                            while (data.MoveToNext());
                        }
                        data.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Exception - " + e.Message);
            }
        }

        private void LoadSituation(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                Situation = new Situation();
                string commandText = "SELECT [SituationID], [Who], [What], [When], [Where] FROM [Situation] WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {
                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToNext())
                        {
                            do
                            {
                                Situation.SituationId = data.GetInt(0);
                                Situation.ThoughtRecordId = id;
                                Situation.Who = data.GetString(1).Trim();
                                Situation.What = data.GetString(2).Trim();
                                Situation.When = data.GetString(3).Trim();
                                Situation.Where = data.GetString(4).Trim();
                                Situation.IsDirty = false;
                                Situation.IsNew = false;
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Situation failed - " + e.Message);
            }
        }

        private void LoadMoods(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                Mood mood = null;
                string commandText = "SELECT [MoodsID], [MoodListID], [MoodRating] FROM Mood WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {
                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToNext())
                        {
                            do
                            {
                                mood = new Mood();
                                mood.MoodsId = data.GetInt(0);
                                mood.ThoughtRecordId = id;
                                mood.MoodListId = data.GetInt(1);
                                mood.MoodRating = data.GetInt(2);
                                mood.IsNew = false;
                                mood.IsDirty = false;
                                Moods.Add(mood);
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Moods failed - " + e.Message);
            }
        }

        private void LoadAutomaticThoughts(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                AutomaticThoughts autoThought = new AutomaticThoughts();
                string commandText = "SELECT [AutomaticThoughtsID], [Thought], [HotThought] FROM AutomaticThoughts WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {
                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToNext())
                        {
                            do
                            {
                                autoThought = new AutomaticThoughts();
                                autoThought.AutomaticThoughtsId = data.GetInt(0);
                                autoThought.ThoughtRecordId = id;
                                autoThought.Thought = data.GetString(1).Trim();
                                autoThought.IsHotThought = Convert.ToBoolean(data.GetShort(2));
                                autoThought.IsNew = false;
                                autoThought.IsDirty = false;
                                AutomaticThoughtsList.Add(autoThought);
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Automatic Thoughts failed - " + e.Message);
            }
        }

        private void LoadEvidenceForHotThought(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                EvidenceForHotThought forHot = new EvidenceForHotThought();
                string commandText = "SELECT [EvidenceForHotThoughtID], [AutomaticThoughtsID], [Evidence] FROM EvidenceForHotThought WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {
                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToNext())
                        {
                            do
                            {
                                forHot = new EvidenceForHotThought();
                                forHot.EvidenceForHotThoughtId = data.GetInt(0);
                                forHot.ThoughtRecordId = id;
                                forHot.AutomaticThoughtsId = data.GetInt(1);
                                forHot.Evidence = data.GetString(2).Trim();
                                forHot.IsNew = false;
                                forHot.IsDirty = false;
                                EvidenceForHotThoughtList.Add(forHot);
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Evidence for Hot Thought failed - " + e.Message);
            }
        }

        private void LoadEvidenceAgainstHotThought(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                EvidenceAgainstHotThought againstHot = new EvidenceAgainstHotThought();
                string commandText = "SELECT [EvidenceAgainstHotThoughtID], [AutomaticThoughtsID], [Evidence] FROM EvidenceAgainstHotThought WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {
                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToNext())
                        {
                            do
                            {
                                againstHot = new EvidenceAgainstHotThought();
                                againstHot.EvidenceAgainstHotThoughtId = data.GetInt(0);
                                againstHot.ThoughtRecordId = id;
                                againstHot.AutomaticThoughtsId = data.GetInt(1);
                                againstHot.Evidence = data.GetString(2).Trim();
                                againstHot.IsNew = false;
                                againstHot.IsDirty = false;
                                EvidenceAgainstHotThoughtList.Add(againstHot);
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Evidence against Hot Thought failed - " + e.Message);
            }
        }

        private void LoadAlternativeThoughts(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                AlternativeThoughts altThought = new AlternativeThoughts();
                string commandText = "SELECT [AlternativeThoughtsID], [Alternative], [BeliefRating] FROM AlternativeThoughts WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {
                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToNext())
                        {
                            do
                            {
                                altThought = new AlternativeThoughts();
                                altThought.AlternativeThoughtsId = data.GetInt(0);
                                altThought.ThoughtRecordId = id;
                                altThought.Alternative = data.GetString(1).Trim();
                                altThought.BeliefRating = data.GetInt(2);
                                altThought.IsNew = false;
                                altThought.IsDirty = false;
                                AlternativeThoughtsList.Add(altThought);
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Alternative Thoughts failed - " + e.Message);
            }
        }

        private void LoadReratedMoods(int id, SQLiteDatabase sqLiteDatabase)
        {
            try
            {
                RerateMood reratedMood = new RerateMood();
                string commandText = "SELECT [RerateMoodID], [MoodsID], [MoodListID], [MoodRating] FROM RerateMood WHERE [ThoughtRecordID] = " + id;
                if (sqLiteDatabase.IsOpen)
                {



                    var data = sqLiteDatabase.RawQuery(commandText, null);
                    if (data != null)
                    {
                        if (data.MoveToFirst())
                        {
                            do
                            {
                                reratedMood = new RerateMood();
                                reratedMood.RerateMoodId = data.GetInt(0);
                                reratedMood.ThoughtRecordId = id;
                                reratedMood.MoodsId = data.GetInt(1);
                                reratedMood.MoodListId = data.GetInt(2);
                                reratedMood.MoodRating = data.GetInt(3);
                                reratedMood.IsNew = false;
                                reratedMood.IsDirty = false;
                                RerateMoodList.Add(reratedMood);
                            }
                            while (data.MoveToNext());
                        }
                    }
                    data.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Load of Rerate Moods failed - " + e.Message);
            }
        }

        public string MoodsString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            foreach(var mood in Moods)
            {
                ret += mood.toString(sqlDatabase) + "\r\n";
            }

            return ret;
        }

        public string AutomaticThoughtsString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            foreach(var auto in AutomaticThoughtsList)
            {
                ret += auto.toString() + "\r\n";
            }

            return ret;
        }

        public string EvidenceForHotThoughtString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            foreach(var forHot in EvidenceForHotThoughtList)
            {
                ret += forHot.toString() + "\r\n";
            }

            return ret;
        }

        public string EvidenceAgainstHotThoughtString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            foreach(var againstHot in EvidenceAgainstHotThoughtList)
            {
                ret += againstHot.toString() + "\r\n";
            }

            return ret;
        }

        public string AlternativeThoughtString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            foreach(var altThought in AlternativeThoughtsList)
            {
                ret += altThought.toString() + "\r\n";
            }

            return ret;
        }

        public string RerateMoodsString(SQLiteDatabase sqlDatabase)
        {
            string ret = "";

            foreach(var mood in RerateMoodList)
            {
                ret += mood.toString(sqlDatabase) + "\r\n";
            }

            return ret;
        }

    }
}