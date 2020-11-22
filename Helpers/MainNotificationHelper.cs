using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Model;
using Android.App;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class MainNotificationHelper
    {
        public const string TAG = "M:MainNotificationHelper";

        private Activity _activity = null;

        private LinearLayout _notificationBody = null;
        private ImageButton _notificationImage = null;
        private TextView _notificationText = null;

        public MainNotificationHelper(Activity activity)
        {
            _activity = activity;
        }

        public LinearLayout GetRandomNotification()
        {
            LinearLayout randomNotify = null;
            try
            {
                if (_activity != null)
                {
                    randomNotify = (LinearLayout)_activity.LayoutInflater.Inflate(Resource.Layout.MainNotificationItem, null);
                    if(randomNotify != null)
                    {
                        _notificationBody = randomNotify;
                        GetFieldComponents();
                        PerformRandomSelection();
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomNotification: _notificationBody is NULL");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomNotification: _activity is NULL");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetRandomNotification: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMainNotHelpGetRandom), "MainNotificationHelper.GetRandomNotification");
            }
            return _notificationBody;
        }

        private void PerformRandomSelection()
        {
            List<ConstantsAndTypes.NotificationCategories> categoryList = new List<ConstantsAndTypes.NotificationCategories>();
            bool didGet = false;
            const int MAX_CATEGORIES = 10;

            try
            {
                Random randomNotify = new Random();

                do
                {
                    ConstantsAndTypes.NotificationCategories category = (ConstantsAndTypes.NotificationCategories)randomNotify.Next((int)ConstantsAndTypes.NotificationCategories.Affirmation + 1);

                    if (!categoryList.Contains(category))
                    {
                        switch (category)
                        {
                            case ConstantsAndTypes.NotificationCategories.Achievement:
                                didGet = GetRandomAchievement();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Activity:
                                didGet = GetUpComingActivity();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Affirmation:
                                didGet = GetRandomAffirmation();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Attitudes:
                                didGet = GetRandomAttitude();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Fantasy:
                                didGet = GetRandomFantasy();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Feelings:
                                didGet = GetRandomFeelings();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Health:
                                didGet = GetRandomHealth();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Medication:
                                didGet = GetNextMedicationTime();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Reactions:
                                didGet = GetRandomReaction();
                                break;
                            case ConstantsAndTypes.NotificationCategories.Relationships:
                                didGet = GetRandomRelationship();
                                break;
                        }
                        if (!didGet)
                            categoryList.Add(category);
                    }
                }
                while (categoryList.Count < MAX_CATEGORIES && !didGet);
                if(!didGet)
                {
                    //provide a default
                    GetDefaultNotification();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "PerformRandomSelection: Execption - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMainNotHelpPerformRandom), "MainNotificationHelper.PerformRandomSelection");
            }
        }

        private void GetDefaultNotification()
        {
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Default);
                    }
                    else
                    {
                        Log.Error(TAG, "GetDefaultNotification: _notificationImage is NULL!");
                    }
                    if (_notificationText != null)
                    {
                        _notificationText.Text = _activity.GetString(Resource.String.MainNotificationHelperDefaultText);
                    }
                    else
                    {
                        Log.Error(TAG, "GetDefaultNotification: _notificationText is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetDefaultNotification: _notificationBody is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetDefaultNotification: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomAchievement), "MainNotificationHelper.GetDefaultNotification");
            }
        }

        private void GetFieldComponents()
        {
            try
            {
                if (_notificationBody != null)
                {
                    _notificationImage = _notificationBody.FindViewById<ImageButton>(Resource.Id.imgbtnMainNotificationItem);
                    _notificationText = _notificationBody.FindViewById<TextView>(Resource.Id.txtMainNotificationItem);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: _notificationBody is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMainNotHelpGetComponents), "MainNotificationHelper.GetFieldComponents");
            }
        }

        private bool GetRandomAchievement()
        {
            bool didGet = false;
            try
            {
                List<AchievementChart> achievementsWeek = new List<AchievementChart>();
                if(_notificationBody != null)
                {
                    if(_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Achievement);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomAchievement: _notificationImage is NULL!");
                    }
                    if(_notificationText != null)
                    {
                        if(GlobalData.AchievementChartItems == null)
                            GlobalData.AchievementChartItems = new List<AchievementChart>();
                        DateTime start = DateTime.Now.AddDays(-7);
                        for (DateTime theDay = start; theDay <= DateTime.Now; theDay = theDay.AddDays(1))
                        {
                            GlobalData.GetAchievementChartItemsForDate(theDay);
                            if (GlobalData.AchievementChartItems.Count > 0)
                            {
                                foreach(var item in GlobalData.AchievementChartItems)
                                {
                                    achievementsWeek.Add(item);
                                }
                            }
                        }
                        //got a selection from the last week, now pick a random one from this list
                        if (achievementsWeek.Count > 0)
                        {
                            Random randomAchieve = new Random();
                            Int32 index = randomAchieve.Next(achievementsWeek.Count);
                            AchievementChart achievement = achievementsWeek[index];
                            _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpAchievementText1) + " " + achievement.AchievementDate.ToShortDateString() + ", " + achievement.Achievement.Trim();
                            didGet = true;
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomAchievement: _notificationText is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomAchievement: _notificationBody is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetRandomAchievement: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomAchievement), "MainNotificationHelper.GetRandomAchievement");
            }
            return didGet;
        }

        private bool GetUpComingActivity()
        {
            Globals dbHelp = null;
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Activity);
                    }
                    else
                    {
                        Log.Error(TAG, "GetUpComingActivity: _notificationImage is NULL!");
                    }
                    if (_notificationText != null)
                    {
                        dbHelp = new Globals();
                        dbHelp.OpenDatabase();
                        if(dbHelp.GetSQLiteDatabase().IsOpen)
                        {
                            DateTime startDate = DateTime.Now;
                            if(GlobalData.ActivitiesForWeek != null)
                            {
                                if(GlobalData.ActivitiesForWeek.Count > 0)
                                {
                                    var laterActivities =
                                        (from itemActivity in GlobalData.ActivitiesForWeek
                                         where itemActivity.ActivityDate >= startDate
                                         select itemActivity).ToList();

                                    foreach (Activities activities in laterActivities)
                                    {
                                        //we need to find if there are any activities in this item
                                        if(activities.GetTotalNumberOfActivities() > 0)
                                        {
                                            bool getNextActivity = false;
                                            foreach(ActivityTime activityTime in activities.ActivityTimes)
                                            {
                                                if (activities.ActivityDate > startDate)
                                                    getNextActivity = true;
                                                if(!string.IsNullOrEmpty(activityTime.ActivityName.Trim()) && getNextActivity)
                                                {
                                                    _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpNextActivity) + " " + activities.ActivityDate.ToShortDateString() + " " + StringHelper.ActivityTimeForConstant(activityTime.ActivityTime) + ",\n" + activityTime.ActivityName;
                                                    didGet = true;
                                                    break;
                                                }
                                                ConstantsAndTypes.NumericComparator comparator = DateHelper.CompareSpecifiedTimeWithActivityTimeRange(startDate, activityTime.ActivityTime);
                                                if(comparator == ConstantsAndTypes.NumericComparator.EqualTo)
                                                {
                                                    if (!string.IsNullOrEmpty(activityTime.ActivityName.Trim()))
                                                    {
                                                        _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpNextActivity) + " " + activities.ActivityDate.ToShortDateString() + " " + StringHelper.ActivityTimeForConstant(activityTime.ActivityTime) + ",\n" + activityTime.ActivityName;
                                                        didGet = true;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        getNextActivity = true;
                                                    }
                                                }
                                                if (comparator == ConstantsAndTypes.NumericComparator.LessThan)
                                                {
                                                    //we will cycle thru from here to the next activity
                                                    getNextActivity = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetUpComingActivity: _notificationText is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetUpComingActivity: _notificationBody is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetUpComingActivity: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomActivity), "MainNotificationHelper.GetUpComingActivity");
            }
            return didGet;
        }

        private bool GetNextMedicationTime()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Medication);
                    }
                    else
                    {
                        Log.Error(TAG, "GetNextMedicationTime: _notificationImage is NULL!");
                    }
                    if (_notificationText != null)
                    {
                        if(GlobalData.MedicationItems != null)
                        {
                            string medicationList = "";
                            List<MedicationSpread> medSpread = new List<MedicationSpread>();
                            DateTime start = DateTime.Now;
                            foreach(Medication medication in GlobalData.MedicationItems)
                            {
                                medication.MedicationSpread.Sort(delegate (MedicationSpread A, MedicationSpread B)
                               {
                                   return A.MedicationTakeTime.TakenTime.CompareTo(B.MedicationTakeTime.TakenTime);

                               });
                                foreach(MedicationSpread spread in medication.MedicationSpread)
                                {
                                    if(start.TimeOfDay < spread.MedicationTakeTime.TakenTime.TimeOfDay)
                                    {
                                        spread.Tag = medication.MedicationName.Trim();
                                        medSpread.Add(spread);
                                        break;
                                    }
                                }
                            }
                            if (medSpread.Count > 0)
                            {
                                didGet = true;
                                medSpread.Sort(delegate (MedicationSpread a, MedicationSpread b)
                                {
                                    return a.MedicationTakeTime.TakenTime.CompareTo(b.MedicationTakeTime.TakenTime);
                                });
                                foreach (MedicationSpread remSpread in medSpread)
                                {
                                    medicationList += remSpread.Dosage.ToString() + "mg " + _activity.GetString(Resource.String.WordOfLabel) + " " + remSpread.Tag.Trim() + " " + _activity.GetString(Resource.String.WordAt) + " " + remSpread.MedicationTakeTime.TakenTime.ToShortTimeString() + "\n";
                                }
                                _notificationText.Text = medicationList.Trim();
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetNextMedicationTime: _notificationText is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetNextMedicationTime: _notificationBody is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetNextMedicationTime: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomMedicationTime), "MainNotificationHelper.GetNextMedicationTime");
            }

            return didGet;
        }

        private bool GetRandomFeelings()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Feelings);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomFeelings");
                    }
                    if (_notificationText != null)
                    {
                        if(GlobalData.StructuredPlanFeelings != null)
                        {
                            if(GlobalData.StructuredPlanFeelings.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.StructuredPlanFeelings.Count);
                                var feeling = GlobalData.StructuredPlanFeelings[index];
                                _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpFeelingsAbout) + " " + feeling.AboutWhat.Trim() + "\n" + _activity.GetString(Resource.String.MainNotHelpTryTo) + " " + StringHelper.ActionTypeForConstant(feeling.Action) + " " + _activity.GetString(Resource.String.WordOfLabel) + " " + feeling.ActionOf.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomFeelings: _notificationText is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomFeelings: _notificationBody is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomFeelings");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomFeelings), "MainNotificationHelper.GetRandomFeelings");
            }

            return didGet;
        }

        private bool GetRandomReaction()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Reactions);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomReaction");
                    }
                    if (_notificationText != null)
                    {
                        if (GlobalData.StructuredPlanReactions != null)
                        {
                            if (GlobalData.StructuredPlanReactions.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.StructuredPlanReactions.Count);
                                var react = GlobalData.StructuredPlanReactions[index];
                                _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpReactionTo) + " " + react.ToWhat.Trim() + "\n" + _activity.GetString(Resource.String.MainNotHelpTryTo) + " " + StringHelper.ActionTypeForConstant(react.Action) + " " + _activity.GetString(Resource.String.WordOfLabel) + " " + react.ActionOf.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomReaction: _notificationText is NULL!");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomReaction: _notificationBody is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomReaction");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomReaction), "MainNotificationHelper.GetRandomReaction");
            }

            return didGet;
        }

        private bool GetRandomAttitude()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Attitudes);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomAttitude");
                    }
                    if (_notificationText != null)
                    {
                        if (GlobalData.StructuredPlanAttitudes != null)
                        {
                            if (GlobalData.StructuredPlanAttitudes.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.StructuredPlanAttitudes.Count);
                                var attit = GlobalData.StructuredPlanAttitudes[index];
                                _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpAttitudeTowards) + " " + attit.ToWhat.Trim() + "\n" + _activity.GetString(Resource.String.MainNotHelpTryTo) + " " + StringHelper.ActionTypeForConstant(attit.Action) + " " + _activity.GetString(Resource.String.WordOfLabel) + " " + attit.ActionOf.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomAttitude");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomAttitude");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomAttitude");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomAttitude), "MainNotificationHelper.GetRandomAttitude");
            }

            return didGet;
        }

        private bool GetRandomRelationship()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Relationships);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomRelationship");
                    }
                    if (_notificationText != null)
                    {
                        if (GlobalData.StructuredPlanRelationships != null)
                        {
                            if (GlobalData.StructuredPlanRelationships.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.StructuredPlanRelationships.Count);
                                var relat = GlobalData.StructuredPlanRelationships[index];
                                _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpRelationshipWith) + " " + relat.WithWhom.Trim() + " (" + StringHelper.RelationshipTypeForConstant(relat.Type) + ")\n" + _activity.GetString(Resource.String.MainNotHelpTryTo) + " " + StringHelper.ActionTypeForConstant(relat.Action) + " " + _activity.GetString(Resource.String.WordOfLabel) + " " + relat.ActionOf.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomRelationship");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomRelationship");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomRelationship");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomRelationship), "MainNotificationHelper.GetRandomRelationship");
            }

            return didGet;
        }

        private bool GetRandomHealth()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Health);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomHealth");
                    }
                    if (_notificationText != null)
                    {
                        if (GlobalData.StructuredPlanHealth != null)
                        {
                            if (GlobalData.StructuredPlanHealth.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.StructuredPlanHealth.Count);
                                var health = GlobalData.StructuredPlanHealth[index];
                                _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpHealthAspect) + " " + health.Aspect.Trim() + "\n" + _activity.GetString(Resource.String.MainNotHelpTryTo) + " " + StringHelper.ActionTypeForConstant(health.Action) + " " + _activity.GetString(Resource.String.WordOfLabel) + " " + health.ActionOf.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomHealth");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomHealth");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomHealth");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomHealth), "MainNotificationHelper.GetRandomHealth");
            }

            return didGet;
        }

        private bool GetRandomFantasy()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Fantasy);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomFantasy");
                    }
                    if (_notificationText != null)
                    {
                        if (GlobalData.StructuredPlanFantasies != null)
                        {
                            if (GlobalData.StructuredPlanFantasies.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.StructuredPlanFantasies.Count);
                                var fanta = GlobalData.StructuredPlanFantasies[index];
                                _notificationText.Text = _activity.GetString(Resource.String.MainNotHelpFantasyAbout) + " " + fanta.OfWhat.Trim() + "\n" + _activity.GetString(Resource.String.MainNotHelpTryTo) + " " + StringHelper.ActionTypeForConstant(fanta.Action) + " " + _activity.GetString(Resource.String.WordOfLabel) + " " + fanta.ActionOf.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomFantasy");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomFantasy");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomFantasy");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomFantasy), "MainNotificationHelper.GetRandomFantasy");
            }

            return didGet;
        }

        private bool GetRandomAffirmation()
        {
            bool didGet = false;
            try
            {
                if (_notificationBody != null)
                {
                    if (_notificationImage != null)
                    {
                        ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories.Affirmation);
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomAffirmation");
                    }
                    if (_notificationText != null)
                    {
                        if (GlobalData.AffirmationListItems != null)
                        {
                            if (GlobalData.AffirmationListItems.Count > 0)
                            {
                                Random random = new Random();
                                var index = random.Next(GlobalData.AffirmationListItems.Count);
                                var affirm = GlobalData.AffirmationListItems[index];
                                _notificationText.Text = affirm.AffirmationText.Trim();
                                didGet = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Error(TAG, "GetRandomAffirmation");
                    }
                }
                else
                {
                    Log.Error(TAG, "GetRandomAffirmation");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetRandomAffirmation");
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorGetRandomAffirmation), "MainNotificationHelper.GetRandomAffirmation");
            }

            return didGet;
        }

        private void ApplyRelevantImageResource(ConstantsAndTypes.NotificationCategories category)
        {
            try
            {
                if(_notificationImage != null)
                {
                    switch(category)
                    {
                        case ConstantsAndTypes.NotificationCategories.Achievement:
                            _notificationImage.SetImageResource(Resource.Drawable.chuffed);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Activity:
                            _notificationImage.SetImageResource(Resource.Drawable.activity);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Affirmation:
                            _notificationImage.SetImageResource(Resource.Drawable.affirmation);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Attitudes:
                            _notificationImage.SetImageResource(Resource.Drawable.attitudes);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Fantasy:
                            _notificationImage.SetImageResource(Resource.Drawable.fantasies);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Feelings:
                            _notificationImage.SetImageResource(Resource.Drawable.feelings);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Health:
                            _notificationImage.SetImageResource(Resource.Drawable.health);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Medication:
                            _notificationImage.SetImageResource(Resource.Drawable.tablets);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Reactions:
                            _notificationImage.SetImageResource(Resource.Drawable.reactions);
                            break;
                        case ConstantsAndTypes.NotificationCategories.Relationships:
                            _notificationImage.SetImageResource(Resource.Drawable.relationships);
                            break;
                        default:
                            _notificationImage.SetImageResource(Resource.Drawable.SymbolInformation);
                            break;
                    }
                }
                else
                {
                    Log.Error(TAG, "ApplyRelevantImageResource: _notificationImage is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "ApplyRelevantImageResource: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorMainNotHelpApplyImgResource), "MainNotificationHelper.ApplyRelevantImageResource");
            }
        }
    }
}