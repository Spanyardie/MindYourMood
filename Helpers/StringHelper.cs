using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace com.spanyardie.MindYourMood.Helpers
{
    public static class StringHelper
    {
        public static string[] DaysOfTheWeek()
        {
            string[] daysOfWeek = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    daysOfWeek = new string[7]
                    {
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                        "Friday",
                        "Saturday",
                        "Sunday"
                    };
                    break;
                case "spa":
                    daysOfWeek = new string[7]
                    {
                        "Lunes",
                        "Martes",
                        "Miércoles",
                        "Jueves",
                        "Viernes",
                        "Sábado",
                        "Domingo"
                    };
                    break;
                case "fra":
                    daysOfWeek = new string[7]
                    {
                        "Lundi",
                        "Mardi",
                        "Mercredi",
                        "Jeudi",
                        "Vendredi",
                        "Samedi",
                        "Dimanche"
                    };
                    break;
            }

            return daysOfWeek;
        }

        public static string DayStringForConstant(ConstantsAndTypes.DAYS_OF_THE_WEEK dayOfWeek)
        {
            string theDay = "Monday";

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (dayOfWeek)
                    {
                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday:
                            theDay = "Monday";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Tuesday:
                            theDay = "Tuesday";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Wednesday:
                            theDay = "Wednesday";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Thursday:
                            theDay = "Thursday";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Friday:
                            theDay = "Friday";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Saturday:
                            theDay = "Saturday";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Sunday:
                            theDay = "Sunday";
                            break;
                    }
                    break;

                case "spa":
                    switch (dayOfWeek)
                    {
                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday:
                            theDay = "Lunes";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Tuesday:
                            theDay = "Martes";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Wednesday:
                            theDay = "Miércoles";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Thursday:
                            theDay = "Jueves";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Friday:
                            theDay = "Viernes";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Saturday:
                            theDay = "Sábado";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Sunday:
                            theDay = "Domingo";
                            break;
                    }
                    break;

                case "fra":
                    switch (dayOfWeek)
                    {
                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Monday:
                            theDay = "Lundi";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Tuesday:
                            theDay = "Mardi";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Wednesday:
                            theDay = "Mercredi";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Thursday:
                            theDay = "Jeudi";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Friday:
                            theDay = "Vendredi";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Saturday:
                            theDay = "Samedi";
                            break;

                        case ConstantsAndTypes.DAYS_OF_THE_WEEK.Sunday:
                            theDay = "Dimanche";
                            break;
                    }
                    break;
            }


            return theDay;
        }

        public static string[] PrescriptionTypes()
        {
            string[] prescType = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    prescType = new string[2]
                    {
                        "Daily",
                        "Weekly"
                    };
                    break;

                case "spa":
                    prescType = new string[2]
                    {
                        "Diariamente",
                        "Semanal"
                    };
                    break;
                case "fra":
                    prescType = new string[2]
                    {
                        "Du quotidien",
                        "Hebdomadaire"
                    };
                    break;
            }


            return prescType;
        }

        public static string PrescriptionStringForConstant(ConstantsAndTypes.PRESCRIPTION_TYPE prescriptionType)
        {
            string type = "Daily";

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (prescriptionType)
                    {
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                            type = "Daily";
                            break;

                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                            type = "Weekly";
                            break;

                        //case ConstantsAndTypes.PRESCRIPTION_TYPE.Monthly:
                        //    type = "Monthly";
                        //    break;
                    }
                    break;

                case "spa":
                    switch (prescriptionType)
                    {
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                            type = "Diariamente";
                            break;

                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                            type = "Semanal";
                            break;

                        //case ConstantsAndTypes.PRESCRIPTION_TYPE.Monthly:
                        //    type = "Mensual";
                        //    break;
                    }
                    break;

                case "fra":
                    switch (prescriptionType)
                    {
                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Daily:
                            type = "Du quotidien";
                            break;

                        case ConstantsAndTypes.PRESCRIPTION_TYPE.Weekly:
                            type = "Hebdomadaire";
                            break;

                            //case ConstantsAndTypes.PRESCRIPTION_TYPE.Monthly:
                            //    type = "Mensual";
                            //    break;
                    }
                    break;
            }
            return type;
        }

        public static string[] MedicationTimes()
        {
            string[] medTimes = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    medTimes = new string[4]
                    {
                        "Morning",
                        "Lunch Time",
                        "Dinner Time",
                        "Evening"
                    };
                    break;

                case "spa":
                    medTimes = new string[4]
                    {
                        "Mañana",
                        "Almuerzo",
                        "Cena",
                        "Noche"
                    };
                    break;

                case "fra":
                    medTimes = new string[4]
                    {
                        "Matin",
                        "L\'heure du déjeuner",
                        "L\'heure du dîner",
                        "Soir"
                    };
                    break;
            }
            return medTimes;
        }

        public static string MedicationTimeForConstant(ConstantsAndTypes.MEDICATION_TIME medicationTime)
        {
            string medTime = "Morning";

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (medicationTime)
                    {
                        case ConstantsAndTypes.MEDICATION_TIME.Morning:
                            medTime = "Morning";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.LunchTime:
                            medTime = "Lunch Time";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.DinnerTime:
                            medTime = "Dinner Time";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.Evening:
                            medTime = "Evening";
                            break;
                    }
                    break;

                case "spa":
                    switch (medicationTime)
                    {
                        case ConstantsAndTypes.MEDICATION_TIME.Morning:
                            medTime = "Mañana";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.LunchTime:
                            medTime = "Almuerzo";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.DinnerTime:
                            medTime = "Cena";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.Evening:
                            medTime = "Noche";
                            break;
                    }
                    break;

                case "fra":
                    switch (medicationTime)
                    {
                        case ConstantsAndTypes.MEDICATION_TIME.Morning:
                            medTime = "Matin";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.LunchTime:
                            medTime = "L\'heure du déjeuner";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.DinnerTime:
                            medTime = "L\'heure du dîner";
                            break;

                        case ConstantsAndTypes.MEDICATION_TIME.Evening:
                            medTime = "Soir";
                            break;
                    }
                    break;
            }
            return medTime;
        }

        public static string[] MedicationFoodTimes()
        {
            string[] foodTimes = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    foodTimes = new string[4]
                    {
                        "Before",
                        "After",
                        "With",
                        "Doesn't Matter"
                    };
                    break;

                case "spa":
                    foodTimes = new string[4]
                    {
                        "Antes de",
                        "Después",
                        "Con",
                        "No importa"
                    };
                    break;

                case "fra":
                    foodTimes = new string[4]
                    {
                        "Avant",
                        "Après",
                        "Avec",
                        "Ne compte pas"
                    };
                    break;
            }
            return foodTimes;
        }

        public static string MedicationFoodForConstant(ConstantsAndTypes.MEDICATION_FOOD medicationFood)
        {
            string medFood = "Before";

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (medicationFood)
                    {
                        case ConstantsAndTypes.MEDICATION_FOOD.Before:
                            medFood = "Before";
                            break;

                        case ConstantsAndTypes.MEDICATION_FOOD.After:
                            medFood = "After";
                            break;

                        case ConstantsAndTypes.MEDICATION_FOOD.With:
                            medFood = "With";
                            break;

                        case ConstantsAndTypes.MEDICATION_FOOD.DoesntMatter:
                            medFood = "Doesn't Matter";
                            break;
                    }
                    break;

                case "spa":
                    switch (medicationFood)
                    {
                        case ConstantsAndTypes.MEDICATION_FOOD.Before:
                            medFood = "Antes de";
                            break;

                        case ConstantsAndTypes.MEDICATION_FOOD.After:
                            medFood = "Después";
                            break;

                        case ConstantsAndTypes.MEDICATION_FOOD.With:
                            medFood = "Con";
                            break;

                        case ConstantsAndTypes.MEDICATION_FOOD.DoesntMatter:
                            medFood = "No importa";
                            break;
                    }
                    break;
            }
            return medFood;
        }

        public static string TimePickerContextString(ConstantsAndTypes.TIMEPICKER_CONTEXT context)
        {
            string timeContext = "";

            switch(context)
            {
                case ConstantsAndTypes.TIMEPICKER_CONTEXT.MedicationReminder:
                    timeContext = "Medication Reminder";
                    break;
                case ConstantsAndTypes.TIMEPICKER_CONTEXT.MedicationTime:
                    timeContext = "Medication Time";
                    break;
            }

            return timeContext;
        }

        public static string ActivityTimeForConstant(ConstantsAndTypes.ACTIVITY_HOURS activityHours)
        {
            string theTimes = "6am - 8am";

            switch(activityHours)
            {
                case ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM:
                    theTimes = "6am - 8am";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.EightAMToTenAM:
                    theTimes = "8am - 10am";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TenAMToTwelvePM:
                    theTimes = "10am - 12pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TwelvePMToTwoPM:
                    theTimes = "12pm - 2pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TwoPMToFourPM:
                    theTimes = "2pm - 4pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.FourPMToSixPM:
                    theTimes = "4pm - 6pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.SixPMToEightPM:
                    theTimes = "6pm - 8pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.EightPMToTenPM:
                    theTimes = "8pm - 10pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM:
                    theTimes = "10pm - 12am";
                    break;
                default:
                    theTimes = "6am - 8am";
                    break;
            }

            return theTimes;
        }

        public static void ActivityTimeBeginEndForConstant(ConstantsAndTypes.ACTIVITY_HOURS activityHours, out string startHour, out string endHour)
        {
            startHour = "6am";
            endHour = "8am";

            switch(activityHours)
            {
                case ConstantsAndTypes.ACTIVITY_HOURS.SixAMToEightAM:
                    startHour = "6am"; endHour = "8am";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.EightAMToTenAM:
                    startHour = "8am"; endHour = "10am";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TenAMToTwelvePM:
                    startHour = "10am"; endHour = "12pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TwelvePMToTwoPM:
                    startHour = "12pm"; endHour = "2pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TwoPMToFourPM:
                    startHour = "2pm"; endHour = "4pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.FourPMToSixPM:
                    startHour = "4pm"; endHour = "6pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.SixPMToEightPM:
                    startHour = "6pm"; endHour = "8pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.EightPMToTenPM:
                    startHour = "8pm"; endHour = "10pm";
                    break;
                case ConstantsAndTypes.ACTIVITY_HOURS.TenPMToTwelveAM:
                    startHour = "10pm"; endHour = "12am";
                    break;
                default:
                    startHour = "6am"; endHour = "8am";
                    break;
            }
        }

        public static string AttitudeTypeForConstant(ConstantsAndTypes.ATTITUDE_TYPES attitudeType)
        {
            string attitude = "";

            switch (GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (attitudeType)
                    {
                        case ConstantsAndTypes.ATTITUDE_TYPES.Optimism:
                            attitude = "Optimism";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Pessimism:
                            attitude = "Pessimism";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Confident:
                            attitude = "Confident";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Interested:
                            attitude = "Interested";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Independent:
                            attitude = "Independent";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Jealous:
                            attitude = "Jealous";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Courteous:
                            attitude = "Courteous";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Cooperative:
                            attitude = "Cooperative";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Considerate:
                            attitude = "Considerate";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Inferior:
                            attitude = "Inferior";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Happy:
                            attitude = "Happy";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Frank:
                            attitude = "Frank";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Respectful:
                            attitude = "Respectful";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Authoritative:
                            attitude = "Authoritative";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Sincere:
                            attitude = "Sincere";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Persistent:
                            attitude = "Persistent";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Honest:
                            attitude = "Honest";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Sympathetic:
                            attitude = "Sympathetic";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Realistic:
                            attitude = "Realistic";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Faithful:
                            attitude = "Faithful";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Flexible:
                            attitude = "Flexible";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Decisive:
                            attitude = "Decisive";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Trusting:
                            attitude = "Trusting";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Thoughtful:
                            attitude = "Thoughtful";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Determined:
                            attitude = "Determined";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Loving:
                            attitude = "Loving";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Hostile:
                            attitude = "Hostile";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Modest:
                            attitude = "Modest";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Reliable:
                            attitude = "Reliable";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Tolerant:
                            attitude = "Tolerant";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Humble:
                            attitude = "Humble";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Cautious:
                            attitude = "Cautious";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Sarcastic:
                            attitude = "Sarcastic";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Helping:
                            attitude = "Helping";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.HardWorking:
                            attitude = "Hard Working";
                            break;
                        default:
                            attitude = "";
                            break;
                    }
                    break;

                case "spa":
                    switch (attitudeType)
                    {
                        case ConstantsAndTypes.ATTITUDE_TYPES.Optimism:
                            attitude = "Optimismo";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Pessimism:
                            attitude = "Pesimismo";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Confident:
                            attitude = "Confidente";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Interested:
                            attitude = "Interesado";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Independent:
                            attitude = "Independiente";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Jealous:
                            attitude = "Celoso";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Courteous:
                            attitude = "Cortés";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Cooperative:
                            attitude = "Cooperativa";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Considerate:
                            attitude = "Considerado";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Inferior:
                            attitude = "Inferior";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Happy:
                            attitude = "Contento";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Frank:
                            attitude = "Franco";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Respectful:
                            attitude = "Respetuoso";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Authoritative:
                            attitude = "Autoritario";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Sincere:
                            attitude = "Sincero";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Persistent:
                            attitude = "Persistente";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Honest:
                            attitude = "Honesto";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Sympathetic:
                            attitude = "Simpático";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Realistic:
                            attitude = "Realista";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Faithful:
                            attitude = "Fiel";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Flexible:
                            attitude = "Flexible";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Decisive:
                            attitude = "Decisivo";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Trusting:
                            attitude = "Confiar";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Thoughtful:
                            attitude = "Pensativo";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Determined:
                            attitude = "Determinado";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Loving:
                            attitude = "Amoroso";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Hostile:
                            attitude = "Hostil";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Modest:
                            attitude = "Modesto";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Reliable:
                            attitude = "De confianza";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Tolerant:
                            attitude = "Tolerante";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Humble:
                            attitude = "Humilde";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Cautious:
                            attitude = "Cauteloso";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Sarcastic:
                            attitude = "Sarcástico";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.Helping:
                            attitude = "Ración";
                            break;
                        case ConstantsAndTypes.ATTITUDE_TYPES.HardWorking:
                            attitude = "Trabajo duro";
                            break;
                        default:
                            attitude = "";
                            break;
                    }
                    break;
            }

            return attitude;
        }

        public static string ReactionTypeForConstant(ConstantsAndTypes.REACTION_TYPE reaction)
        {
            string reactionType = "";

            switch (GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (reaction)
                    {
                        case ConstantsAndTypes.REACTION_TYPE.Positive:
                            reactionType = "Positive";
                            break;
                        case ConstantsAndTypes.REACTION_TYPE.Negative:
                            reactionType = "Negative";
                            break;
                        case ConstantsAndTypes.REACTION_TYPE.Ambivalent:
                            reactionType = "Ambivalent";
                            break;
                    }
                    break;
                case "spa":
                    switch (reaction)
                    {
                        case ConstantsAndTypes.REACTION_TYPE.Positive:
                            reactionType = "Positivo";
                            break;
                        case ConstantsAndTypes.REACTION_TYPE.Negative:
                            reactionType = "Negativo";
                            break;
                        case ConstantsAndTypes.REACTION_TYPE.Ambivalent:
                            reactionType = "Abivalente";
                            break;
                    }
                    break;
            }

            return reactionType;
        }

        public static string[] ReactionList()
        {
            string[] reactions = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    reactions = new string[3]
                    {
                        "Positive",
                        "Negative",
                        "Ambivalent"
                    };
                    break;

                case "spa":
                    reactions = new string[3]
                    {
                        "Positivo",
                        "Negativo",
                        "Ambivalente"
                    };
                    break;
            }

            return reactions;
        }

        public static string[] ActionList()
        {
            string[] actions = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    actions = new string[3]
                    {
                        "Do More",
                        "Do Less",
                        "Maintain"
                    };
                    break;
                case "spa":
                    actions = new string[3]
                    {
                        "Hacer más",
                        "Haz menos",
                        "Mantener"
                    };
                    break;
            }

            return actions;
        }

        public static string[] RelationshipList()
        {
            string[] relationships = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    relationships = new string[19]
                    {
                        "Mother",
                        "Father",
                        "Sister",
                        "Brother",
                        "Son",
                        "Daughter",
                        "Aunt",
                        "Uncle",
                        "Grandmother",
                        "Grandfather",
                        "Wife",
                        "Husband",
                        "Partner",
                        "Niece",
                        "Nephew",
                        "Relative",
                        "Friend",
                        "Aquaintance",
                        "Work Colleague"
                    };
                    break;
                case "spa":
                    relationships = new string[19]
                    {
                        "Madre",
                        "Padre",
                        "Hermana",
                        "Hermano",
                        "Hijo",
                        "Hija",
                        "Tía",
                        "Tío",
                        "Abuela",
                        "Abuelo",
                        "Esposa",
                        "Marido",
                        "Compañero",
                        "Sobrina",
                        "Sobrino",
                        "Relativo",
                        "Amigo",
                        "Aquaintance",
                        "Compañero de trabajo"
                    };
                    break;
            }

            return relationships;
        }

        public static string RelationshipTypeForConstant(ConstantsAndTypes.RELATIONSHIP_TYPE relationship)
        {
            string relation = "";
            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch(relationship)
                    {
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Aquaintance:
                            relation = "Aquaintance";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Aunt:
                            relation = "Aunt";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Brother:
                            relation = "Brother";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Daughter:
                            relation = "Daughter";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Father:
                            relation = "Father";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Friend:
                            relation = "Friend";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Grandfather:
                            relation = "Grandfather";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Grandmother:
                            relation = "Grandmother";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Husband:
                            relation = "Husband";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Mother:
                            relation = "Mother";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Nephew:
                            relation = "Nephew";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Niece:
                            relation = "Niece";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Partner:
                            relation = "Partner";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Relative:
                            relation = "Relative";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Sister:
                            relation = "Sister";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Son:
                            relation = "Son";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Uncle:
                            relation = "Uncle";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Wife:
                            relation = "Wife";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.WorkColleague:
                            relation = "Work Colleague";
                            break;
                        default:
                            relation = "";
                            break;
                    }
                    break;
                case "spa":
                    switch (relationship)
                    {
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Aquaintance:
                            relation = "Aquaintance";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Aunt:
                            relation = "Tía";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Brother:
                            relation = "Hermano";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Daughter:
                            relation = "Hija";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Father:
                            relation = "Padre";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Friend:
                            relation = "Amigo";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Grandfather:
                            relation = "Abuelo";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Grandmother:
                            relation = "Abuela";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Husband:
                            relation = "Marido";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Mother:
                            relation = "Madre";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Nephew:
                            relation = "Sobrino";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Niece:
                            relation = "Sobrina";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Partner:
                            relation = "Compañero";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Relative:
                            relation = "Relativo";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Sister:
                            relation = "Hermana";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Son:
                            relation = "Hijo";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Uncle:
                            relation = "Tío";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.Wife:
                            relation = "Esposa";
                            break;
                        case ConstantsAndTypes.RELATIONSHIP_TYPE.WorkColleague:
                            relation = "Compañero de trabajo";
                            break;
                        default:
                            relation = "";
                            break;
                    }
                    break;
            }

            return relation;
        }

        public static string[] AttitudeList()
        {
            string[] attitudes = null;

            switch(GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    attitudes = new string[35]
                    {
                        "Optimism",
                        "Pessimism",
                        "Confident",
                        "Interested",
                        "Independent",
                        "Jealous",
                        "Courteous",
                        "Cooperative",
                        "Considerate",
                        "Inferior",
                        "Happy",
                        "Frank",
                        "Respectful",
                        "Authoritative",
                        "Sincere",
                        "Persistent",
                        "Honest",
                        "Sympathetic",
                        "Realistic",
                        "Faithful",
                        "Flexible",
                        "Decisive",
                        "Trusting",
                        "Thoughtful",
                        "Determined",
                        "Loving",
                        "Hostile",
                        "Modest",
                        "Reliable",
                        "Tolerant",
                        "Humble",
                        "Cautious",
                        "Sarcastic",
                        "Helping",
                        "Hard Working"
                    };
                    break;
                case "spa":
                    attitudes = new string[35]
                    {
                        "Optimismo",
                        "Pesimismo",
                        "Confidente",
                        "Interesado",
                        "Independiente",
                        "Celoso",
                        "Cortés",
                        "Cooperativa",
                        "Considerado",
                        "Inferior",
                        "Contento",
                        "Franco",
                        "Respetuoso",
                        "Autoritario",
                        "Sincero",
                        "Persistente",
                        "Honesto",
                        "Simpático",
                        "Realista",
                        "Fiel",
                        "Flexible",
                        "Decisivo",
                        "Fiable",
                        "Pensativo",
                        "Determinado",
                        "Amoroso",
                        "Hostil",
                        "Modesto",
                        "De confianza",
                        "Tolerante",
                        "Humilde",
                        "Cauteloso",
                        "Sarcástico",
                        "Ración",
                        "Trabajo duro"
                    };
                    break;
            }

            return attitudes;
        }

        public static string ProConTypeForConstant(ConstantsAndTypes.PROCON_TYPES proConType)
        {
            string proCon = "";

            switch(proConType)
            {
                case ConstantsAndTypes.PROCON_TYPES.Con:
                    proCon = "Con";
                    break;
                case ConstantsAndTypes.PROCON_TYPES.Pro:
                    proCon = "Pro";
                    break;
            }

            return proCon;
        }

        public static string ActionTypeForConstant(ConstantsAndTypes.ACTION_TYPE actionType)
        {
            string action = "";

            switch (GlobalData.CurrentIsoLanguageCode.ToLower())
            {
                case "eng":
                    switch (actionType)
                    {
                        case ConstantsAndTypes.ACTION_TYPE.DoLess:
                            action = "do less";
                            break;
                        case ConstantsAndTypes.ACTION_TYPE.DoMore:
                            action = "do more";
                            break;
                        case ConstantsAndTypes.ACTION_TYPE.Maintain:
                            action = "maintain";
                            break;
                    }
                    break;

                case "spa":
                    switch (actionType)
                    {
                        case ConstantsAndTypes.ACTION_TYPE.DoLess:
                            action = "hacer más";
                            break;
                        case ConstantsAndTypes.ACTION_TYPE.DoMore:
                            action = "haz menos";
                            break;
                        case ConstantsAndTypes.ACTION_TYPE.Maintain:
                            action = "mantener";
                            break;
                    }
                    break;
            }
            return action;
        }

        public static int GetNumberFromText(string textNumber)
        {
            var retVal = 0;

            string lower = textNumber.ToLower();

            switch(lower)
            {
                case "one":
                    retVal = 1;
                    break;
                case "two":
                    retVal = 2;
                    break;
                case "three":
                    retVal = 3;
                    break;
                case "four":
                    retVal = 4;
                    break;
                case "five":
                    retVal = 5;
                    break;
                case "ten":
                    retVal = 10;
                    break;
                case "fifteen":
                    retVal = 15;
                    break;
                case "twenty":
                    retVal = 20;
                    break;
                case "twenty five":
                    retVal = 25;
                    break;
                case "thirty":
                    retVal = 30;
                    break;
                case "thirty five":
                    retVal = 35;
                    break;
                case "forty":
                    retVal = 40;
                    break;
                case "forty five":
                    retVal = 45;
                    break;
                case "fifty":
                    retVal = 50;
                    break;
                case "fifty five":
                    retVal = 55;
                    break;
                case "sixty":
                    retVal = 60;
                    break;
                case "sixty five":
                    retVal = 65;
                    break;
                case "seventy":
                    retVal = 70;
                    break;
                case "seventy five":
                    retVal = 75;
                    break;
                case "eighty":
                    retVal = 80;
                    break;
                case "eighty five":
                    retVal = 85;
                    break;
                case "ninety":
                    retVal = 90;
                    break;
                case "ninety five":
                    retVal = 95;
                    break;
                case "one hundred":
                    retVal = 100;
                    break;
                case "one hundred and five":
                    retVal = 105;
                    break;
                case "one hundred and ten":
                    retVal = 110;
                    break;
                case "one hundred and fifteen":
                    retVal = 115;
                    break;
                case "one hundred and twenty":
                    retVal = 120;
                    break;
                case "one hundred and twenty five":
                    retVal = 125;
                    break;
                case "one hundred and thirty":
                    retVal = 130;
                    break;
                case "one hundred and thirty five":
                    retVal = 135;
                    break;
                case "one hundred and forty":
                    retVal = 140;
                    break;
                case "one hundred and forty five":
                    retVal = 145;
                    break;
                case "one hundred and fifty":
                    retVal = 150;
                    break;
                case "one hundred and fifty five":
                    retVal = 155;
                    break;
                case "one hundred and sixty":
                    retVal = 160;
                    break;
                case "one hundred and sixty five":
                    retVal = 165;
                    break;
                case "one hundred and seventy":
                    retVal = 170;
                    break;
                case "one hundred and seventy five":
                    retVal = 175;
                    break;
                case "one hundred and eighty":
                    retVal = 180;
                    break;
                case "one hundred and eighty five":
                    retVal = 185;
                    break;
                case "one hundred and ninety":
                    retVal = 190;
                    break;
                case "one hundred and ninety five":
                    retVal = 195;
                    break;
                case "two hundred":
                    retVal = 200;
                    break;
                case "two hundred and five":
                    retVal = 205;
                    break;
                case "two hundred and ten":
                    retVal = 210;
                    break;
                case "two hundred and fifteen":
                    retVal = 215;
                    break;
                case "two hundred and twenty":
                    retVal = 220;
                    break;
                case "two hundred and twenty five":
                    retVal = 225;
                    break;
                case "two hundred and thirty":
                    retVal = 230;
                    break;
                case "two hundred and thirty five":
                    retVal = 235;
                    break;
                case "two hundred and forty":
                    retVal = 240;
                    break;
                case "two hundred and forty five":
                    retVal = 245;
                    break;
                case "two hundred and fifty":
                    retVal = 250;
                    break;
                case "two hundred and fifty five":
                    retVal = 255;
                    break;
                case "two hundred and sixty":
                    retVal = 260;
                    break;
                case "two hundred and sixty five":
                    retVal = 265;
                    break;
                case "two hundred and seventy":
                    retVal = 270;
                    break;
                case "two hundred and seventy five":
                    retVal = 275;
                    break;
                case "two hundred and eighty":
                    retVal = 280;
                    break;
                case "two hundred and eighty five":
                    retVal = 285;
                    break;
                case "two hundred and ninety":
                    retVal = 290;
                    break;
                case "two hundred and ninety five":
                    retVal = 295;
                    break;
                case "three hundred":
                    retVal = 300;
                    break;
                case "three hundred and five":
                    retVal = 305;
                    break;
                case "three hundred and ten":
                    retVal = 310;
                    break;
                case "three hundred and fifteen":
                    retVal = 315;
                    break;
                case "three hundred and twenty":
                    retVal = 320;
                    break;
                case "three hundred and twenty five":
                    retVal = 325;
                    break;
                case "three hundred and thirty":
                    retVal = 330;
                    break;
                case "three hundred and thirty five":
                    retVal = 335;
                    break;
                case "three hundred and forty":
                    retVal = 340;
                    break;
                case "three hundred and forty five":
                    retVal = 345;
                    break;
                case "three hundred and fifty":
                    retVal = 350;
                    break;
                case "three hundred and fifty five":
                    retVal = 355;
                    break;
                case "three hundred and sixty":
                    retVal = 360;
                    break;
                case "three hundred and sixty five":
                    retVal = 365;
                    break;
                case "three hundred and seventy":
                    retVal = 370;
                    break;
                case "three hundred and seventy five":
                    retVal = 375;
                    break;
                case "three hundred and eighty":
                    retVal = 380;
                    break;
                case "three hundred and eighty five":
                    retVal = 385;
                    break;
                case "three hundred and ninety":
                    retVal = 390;
                    break;
                case "three hundred and ninety five":
                    retVal = 395;
                    break;
                case "four hundred":
                    retVal = 400;
                    break;
                case "four hundred and five":
                    retVal = 405;
                    break;
                case "four hundred and ten":
                    retVal = 410;
                    break;
                case "four hundred and fifteen":
                    retVal = 415;
                    break;
                case "four hundred and twenty":
                    retVal = 420;
                    break;
                case "four hundred and twenty five":
                    retVal = 425;
                    break;
                case "four hundred and thirty":
                    retVal = 430;
                    break;
                case "four hundred and thirty five":
                    retVal = 435;
                    break;
                case "four hundred and forty":
                    retVal = 440;
                    break;
                case "four hundred and forty five":
                    retVal = 445;
                    break;
                case "four hundred and fifty":
                    retVal = 450;
                    break;
                case "four hundred and fifty five":
                    retVal = 455;
                    break;
                case "four hundred and sixty":
                    retVal = 460;
                    break;
                case "four hundred and sixty five":
                    retVal = 465;
                    break;
                case "four hundred and seventy":
                    retVal = 470;
                    break;
                case "four hundred and seventy five":
                    retVal = 475;
                    break;
                case "four hundred and eighty":
                    retVal = 480;
                    break;
                case "four hundred and eighty five":
                    retVal = 485;
                    break;
                case "four hundred and ninety":
                    retVal = 490;
                    break;
                case "four hundred and ninety five":
                    retVal = 495;
                    break;
                case "five hundred":
                    retVal = 500;
                    break;
                case "five hundred and five":
                    retVal = 505;
                    break;
                case "five hundred and ten":
                    retVal = 510;
                    break;
                case "five hundred and fifteen":
                    retVal = 515;
                    break;
                case "five hundred and twenty":
                    retVal = 520;
                    break;
                case "five hundred and twenty five":
                    retVal = 525;
                    break;
                case "five hundred and thirty":
                    retVal = 530;
                    break;
                case "five hundred and thirty five":
                    retVal = 535;
                    break;
                case "five hundred and forty":
                    retVal = 540;
                    break;
                case "five hundred and forty five":
                    retVal = 545;
                    break;
                case "five hundred and fifty":
                    retVal = 550;
                    break;
                case "five hundred and fifty five":
                    retVal = 555;
                    break;
                case "five hundred and sixty":
                    retVal = 560;
                    break;
                case "five hundred and sixty five":
                    retVal = 565;
                    break;
                case "five hundred and seventy":
                    retVal = 570;
                    break;
                case "five hundred and seventy five":
                    retVal = 575;
                    break;
                case "five hundred and eighty":
                    retVal = 580;
                    break;
                case "five hundred and eighty five":
                    retVal = 585;
                    break;
                case "five hundred and ninety":
                    retVal = 590;
                    break;
                case "five hundred and ninety five":
                    retVal = 595;
                    break;
                case "six hundred":
                    retVal = 600;
                    break;
                case "six hundred and five":
                    retVal = 605;
                    break;
                case "six hundred and ten":
                    retVal = 610;
                    break;
                case "six hundred and fifteen":
                    retVal = 615;
                    break;
                case "six hundred and twenty":
                    retVal = 620;
                    break;
                case "six hundred and twenty five":
                    retVal = 625;
                    break;
                case "six hundred and thirty":
                    retVal = 630;
                    break;
                case "six hundred and thirty five":
                    retVal = 635;
                    break;
                case "six hundred and forty":
                    retVal = 640;
                    break;
                case "six hundred and forty five":
                    retVal = 645;
                    break;
                case "six hundred and fifty":
                    retVal = 650;
                    break;
                case "six hundred and fifty five":
                    retVal = 655;
                    break;
                case "six hundred and sixty":
                    retVal = 660;
                    break;
                case "six hundred and sixty five":
                    retVal = 665;
                    break;
                case "six hundred and seventy":
                    retVal = 670;
                    break;
                case "six hundred and seventy five":
                    retVal = 675;
                    break;
                case "six hundred and eighty":
                    retVal = 680;
                    break;
                case "six hundred and eighty five":
                    retVal = 685;
                    break;
                case "six hundred and ninety":
                    retVal = 690;
                    break;
                case "six hundred and ninety five":
                    retVal = 695;
                    break;
                case "seven hundred":
                    retVal = 700;
                    break;
                case "seven hundred and five":
                    retVal = 705;
                    break;
                case "seven hundred and ten":
                    retVal = 710;
                    break;
                case "seven hundred and fifteen":
                    retVal = 715;
                    break;
                case "seven hundred and twenty":
                    retVal = 720;
                    break;
                case "seven hundred and twenty five":
                    retVal = 725;
                    break;
                case "seven hundred and thirty":
                    retVal = 730;
                    break;
                case "seven hundred and thirty five":
                    retVal = 735;
                    break;
                case "seven hundred and forty":
                    retVal = 740;
                    break;
                case "seven hundred and forty five":
                    retVal = 745;
                    break;
                case "seven hundred and fifty":
                    retVal = 750;
                    break;
                case "seven hundred and fifty five":
                    retVal = 755;
                    break;
                case "seven hundred and sixty":
                    retVal = 760;
                    break;
                case "seven hundred and sixty five":
                    retVal = 765;
                    break;
                case "seven hundred and seventy":
                    retVal = 770;
                    break;
                case "seven hundred and seventy five":
                    retVal = 775;
                    break;
                case "seven hundred and eighty":
                    retVal = 780;
                    break;
                case "seven hundred and eighty five":
                    retVal = 785;
                    break;
                case "seven hundred and ninety":
                    retVal = 790;
                    break;
                case "seven hundred and ninety five":
                    retVal = 795;
                    break;
                case "eight hundred":
                    retVal = 800;
                    break;
                case "eight hundred and five":
                    retVal = 805;
                    break;
                case "eight hundred and ten":
                    retVal = 810;
                    break;
                case "eight hundred and fifteen":
                    retVal = 815;
                    break;
                case "eight hundred and twenty":
                    retVal = 820;
                    break;
                case "eight hundred and twenty five":
                    retVal = 825;
                    break;
                case "eight hundred and thirty":
                    retVal = 830;
                    break;
                case "eight hundred and thirty five":
                    retVal = 835;
                    break;
                case "eight hundred and forty":
                    retVal = 840;
                    break;
                case "eight hundred and forty five":
                    retVal = 845;
                    break;
                case "eight hundred and fifty":
                    retVal = 850;
                    break;
                case "eight hundred and fifty five":
                    retVal = 855;
                    break;
                case "eight hundred and sixty":
                    retVal = 860;
                    break;
                case "eight hundred and sixty five":
                    retVal = 865;
                    break;
                case "eight hundred and seventy":
                    retVal = 870;
                    break;
                case "eight hundred and seventy five":
                    retVal = 875;
                    break;
                case "eight hundred and eighty":
                    retVal = 880;
                    break;
                case "eight hundred and eighty five":
                    retVal = 885;
                    break;
                case "eight hundred and ninety":
                    retVal = 890;
                    break;
                case "eight hundred and ninety five":
                    retVal = 895;
                    break;
                case "nine hundred":
                    retVal = 900;
                    break;
                case "nine hundred and five":
                    retVal = 905;
                    break;
                case "nine hundred and ten":
                    retVal = 910;
                    break;
                case "nine hundred and fifteen":
                    retVal = 915;
                    break;
                case "nine hundred and twenty":
                    retVal = 920;
                    break;
                case "nine hundred and twenty five":
                    retVal = 925;
                    break;
                case "nine hundred and thirty":
                    retVal = 930;
                    break;
                case "nine hundred and thirty five":
                    retVal = 935;
                    break;
                case "nine hundred and forty":
                    retVal = 940;
                    break;
                case "nine hundred and forty five":
                    retVal = 945;
                    break;
                case "nine hundred and fifty":
                    retVal = 950;
                    break;
                case "nine hundred and fifty five":
                    retVal = 955;
                    break;
                case "nine hundred and sixty":
                    retVal = 960;
                    break;
                case "nine hundred and sixty five":
                    retVal = 965;
                    break;
                case "nine hundred and seventy":
                    retVal = 970;
                    break;
                case "nine hundred and seventy five":
                    retVal = 975;
                    break;
                case "nine hundred and eighty":
                    retVal = 980;
                    break;
                case "nine hundred and eighty five":
                    retVal = 985;
                    break;
                case "nine hundred and ninety":
                    retVal = 990;
                    break;
                case "nine hundred and ninety five":
                    retVal = 995;
                    break;
            }

            return retVal;
        }

        public static string[] AlarmNotificationTypes()
        {
            string[] alarmTypes = new string[7];

            // I've matched up the value with the array index, just makes it easier really as we only have to single out index 0 as being -1
            alarmTypes[0] = "All";                  // -1
            alarmTypes[1] = "Sound";                // 1
            alarmTypes[2] = "Vibrate";              // 2
            alarmTypes[3] = "SoundAndVibrate";      // 3
            alarmTypes[4] = "Lights";               // 4
            alarmTypes[5] = "LightsAndSound";       // 5
            alarmTypes[6] = "LightsAndVibrate";     // 6

            return alarmTypes;
        }

        //returning a string array because RequestPermission requires it
        public static string[] GetPermissionStringForEnum(ConstantsAndTypes.AppPermission permission)
        {
            string[] perm = new string[1];

            switch(permission)
            {
                case ConstantsAndTypes.AppPermission.MakeCalls:
                    perm[0] = ConstantsAndTypes.MAKE_CALLS;
                    break;
                case ConstantsAndTypes.AppPermission.ModifyAudioSettings:
                    perm[0] = ConstantsAndTypes.MODIFY_AUDIO_SETTINGS;
                    break;
                case ConstantsAndTypes.AppPermission.ReadContacts:
                    perm[0] = ConstantsAndTypes.READ_CONTACTS;
                    break;
                case ConstantsAndTypes.AppPermission.ReadPhoneState:
                    perm[0] = ConstantsAndTypes.READ_PHONE_STATE;
                    break;
                case ConstantsAndTypes.AppPermission.ReadProfile:
                    perm[0] = ConstantsAndTypes.READ_PROFILE;
                    break;
                case ConstantsAndTypes.AppPermission.ReceiveBootCompleted:
                    perm[0] = ConstantsAndTypes.RECEIVE_BOOT_COMPLETED;
                    break;
                case ConstantsAndTypes.AppPermission.SendSms:
                    perm[0] = ConstantsAndTypes.SEND_SMS;
                    break;
                case ConstantsAndTypes.AppPermission.SetAlarm:
                    perm[0] = ConstantsAndTypes.SET_ALARM;
                    break;
                case ConstantsAndTypes.AppPermission.UseMicrophone:
                    perm[0] = ConstantsAndTypes.USE_MICROPHONE;
                    break;
                case ConstantsAndTypes.AppPermission.WakeLock:
                    perm[0] = ConstantsAndTypes.WAKE_LOCK;
                    break;
                case ConstantsAndTypes.AppPermission.WriteSms:
                    perm[0] = ConstantsAndTypes.WRITE_SMS;
                    break;
                case ConstantsAndTypes.AppPermission.ReadExternalStorage:
                    perm[0] = ConstantsAndTypes.READ_EXTERNAL_STORAGE;
                    break;
                default:
                    return null;
            }

            return perm;
        }
    }
}