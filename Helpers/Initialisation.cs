using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Xamarin.Contacts;
using System;
using Android.Util;
using Android.Graphics;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class Initialisation
    {
        public const string TAG = "M:Initialisation";

        public string ErrorDescription { get; set; }
        public string ErrorFunction { get; set; }

        public void CreateMedicationTypes(Context context)
        {
            if(context != null)
            {
                string[] medicationType;
                GlobalData.MedicationTypes = new string[9, 3];

                medicationType = context.GetString(Resource.String.MedicationTypeATAP).Split(';');
                GlobalData.MedicationTypes[0, 0] = medicationType[0];
                GlobalData.MedicationTypes[0, 1] = medicationType[1];
                GlobalData.MedicationTypes[0, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeMAOI).Split(';');
                GlobalData.MedicationTypes[1, 0] = medicationType[0];
                GlobalData.MedicationTypes[1, 1] = medicationType[1];
                GlobalData.MedicationTypes[1, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeNRI).Split(';');
                GlobalData.MedicationTypes[2, 0] = medicationType[0];
                GlobalData.MedicationTypes[2, 1] = medicationType[1];
                GlobalData.MedicationTypes[2, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeSARI).Split(';');
                GlobalData.MedicationTypes[3, 0] = medicationType[0];
                GlobalData.MedicationTypes[3, 1] = medicationType[1];
                GlobalData.MedicationTypes[3, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeSMAS).Split(';');
                GlobalData.MedicationTypes[4, 0] = medicationType[0];
                GlobalData.MedicationTypes[4, 1] = medicationType[1];
                GlobalData.MedicationTypes[4, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeSNRI).Split(';');
                GlobalData.MedicationTypes[5, 0] = medicationType[0];
                GlobalData.MedicationTypes[5, 1] = medicationType[1];
                GlobalData.MedicationTypes[5, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeSSRI).Split(';');
                GlobalData.MedicationTypes[6, 0] = medicationType[0];
                GlobalData.MedicationTypes[6, 1] = medicationType[1];
                GlobalData.MedicationTypes[6, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeTCA).Split(';');
                GlobalData.MedicationTypes[7, 0] = medicationType[0];
                GlobalData.MedicationTypes[7, 1] = medicationType[1];
                GlobalData.MedicationTypes[7, 2] = medicationType[2];

                medicationType = context.GetString(Resource.String.MedicationTypeTeCA).Split(';');
                GlobalData.MedicationTypes[8, 0] = medicationType[0];
                GlobalData.MedicationTypes[8, 1] = medicationType[1];
                GlobalData.MedicationTypes[8, 2] = medicationType[2];
            }
        }

        public void CreateMedicationList(Context context)
        {
            if(context != null)
            {
                string[] medicationName;
                GlobalData.MedicationList = new string[52, 3];

                medicationName = context.GetString(Resource.String.MedicationAmisulpride).Split(';');
                GlobalData.MedicationList[0, 0] = medicationName[0];
                GlobalData.MedicationList[0, 1] = medicationName[1];
                GlobalData.MedicationList[0, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationAmitriptyline).Split(';');
                GlobalData.MedicationList[1, 0] = medicationName[0];
                GlobalData.MedicationList[1, 1] = medicationName[1];
                GlobalData.MedicationList[1, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationAmitriptylinoxide).Split(';');
                GlobalData.MedicationList[2, 0] = medicationName[0];
                GlobalData.MedicationList[2, 1] = medicationName[1];
                GlobalData.MedicationList[2, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationAmoxapine).Split(';');
                GlobalData.MedicationList[3, 0] = medicationName[0];
                GlobalData.MedicationList[3, 1] = medicationName[1];
                GlobalData.MedicationList[3, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationCitalopram).Split(';');
                GlobalData.MedicationList[4, 0] = medicationName[0];
                GlobalData.MedicationList[4, 1] = medicationName[1];
                GlobalData.MedicationList[4, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationClomipramine).Split(';');
                GlobalData.MedicationList[5, 0] = medicationName[0];
                GlobalData.MedicationList[5, 1] = medicationName[1];
                GlobalData.MedicationList[5, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDesipramine).Split(';');
                GlobalData.MedicationList[6, 0] = medicationName[0];
                GlobalData.MedicationList[6, 1] = medicationName[1];
                GlobalData.MedicationList[6, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDesvenlafaxine).Split(';');
                GlobalData.MedicationList[7, 0] = medicationName[0];
                GlobalData.MedicationList[7, 1] = medicationName[1];
                GlobalData.MedicationList[7, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDibenzepin).Split(';');
                GlobalData.MedicationList[8, 0] = medicationName[0];
                GlobalData.MedicationList[8, 1] = medicationName[1];
                GlobalData.MedicationList[8, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDimetacrine).Split(';');
                GlobalData.MedicationList[9, 0] = medicationName[0];
                GlobalData.MedicationList[9, 1] = medicationName[1];
                GlobalData.MedicationList[9, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDosulepin).Split(';');
                GlobalData.MedicationList[10, 0] = medicationName[0];
                GlobalData.MedicationList[10, 1] = medicationName[1];
                GlobalData.MedicationList[10, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDoxepin).Split(';');
                GlobalData.MedicationList[11, 0] = medicationName[0];
                GlobalData.MedicationList[11, 1] = medicationName[1];
                GlobalData.MedicationList[11, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationDuloxetine).Split(';');
                GlobalData.MedicationList[12, 0] = medicationName[0];
                GlobalData.MedicationList[12, 1] = medicationName[1];
                GlobalData.MedicationList[12, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationEscitalopram).Split(';');
                GlobalData.MedicationList[13, 0] = medicationName[0];
                GlobalData.MedicationList[13, 1] = medicationName[1];
                GlobalData.MedicationList[13, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationEtoperidone).Split(';');
                GlobalData.MedicationList[14, 0] = medicationName[0];
                GlobalData.MedicationList[14, 1] = medicationName[1];
                GlobalData.MedicationList[14, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationFluoxetine).Split(';');
                GlobalData.MedicationList[15, 0] = medicationName[0];
                GlobalData.MedicationList[15, 1] = medicationName[1];
                GlobalData.MedicationList[15, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationFluvoxamine).Split(';');
                GlobalData.MedicationList[16, 0] = medicationName[0];
                GlobalData.MedicationList[16, 1] = medicationName[1];
                GlobalData.MedicationList[16, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationImipramine).Split(';');
                GlobalData.MedicationList[17, 0] = medicationName[0];
                GlobalData.MedicationList[17, 1] = medicationName[1];
                GlobalData.MedicationList[17, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationIsocarboxazid).Split(';');
                GlobalData.MedicationList[18, 0] = medicationName[0];
                GlobalData.MedicationList[18, 1] = medicationName[1];
                GlobalData.MedicationList[18, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationLevomilnacipran).Split(';');
                GlobalData.MedicationList[19, 0] = medicationName[0];
                GlobalData.MedicationList[19, 1] = medicationName[1];
                GlobalData.MedicationList[19, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationLofepramine).Split(';');
                GlobalData.MedicationList[20, 0] = medicationName[0];
                GlobalData.MedicationList[20, 1] = medicationName[1];
                GlobalData.MedicationList[20, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationLurasidone).Split(';');
                GlobalData.MedicationList[21, 0] = medicationName[0];
                GlobalData.MedicationList[21, 1] = medicationName[1];
                GlobalData.MedicationList[21, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMaprotiline).Split(';');
                GlobalData.MedicationList[22, 0] = medicationName[0];
                GlobalData.MedicationList[22, 1] = medicationName[1];
                GlobalData.MedicationList[22, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMelitracen).Split(';');
                GlobalData.MedicationList[23, 0] = medicationName[0];
                GlobalData.MedicationList[23, 1] = medicationName[1];
                GlobalData.MedicationList[23, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMetralindole).Split(';');
                GlobalData.MedicationList[24, 0] = medicationName[0];
                GlobalData.MedicationList[24, 1] = medicationName[1];
                GlobalData.MedicationList[24, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMianserin).Split(';');
                GlobalData.MedicationList[25, 0] = medicationName[0];
                GlobalData.MedicationList[25, 1] = medicationName[1];
                GlobalData.MedicationList[25, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMilnacipran).Split(';');
                GlobalData.MedicationList[26, 0] = medicationName[0];
                GlobalData.MedicationList[26, 1] = medicationName[1];
                GlobalData.MedicationList[26, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMirtazapine).Split(';');
                GlobalData.MedicationList[27, 0] = medicationName[0];
                GlobalData.MedicationList[27, 1] = medicationName[1];
                GlobalData.MedicationList[27, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationMoclobemide).Split(';');
                GlobalData.MedicationList[28, 0] = medicationName[0];
                GlobalData.MedicationList[28, 1] = medicationName[1];
                GlobalData.MedicationList[28, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationNefazodone).Split(';');
                GlobalData.MedicationList[29, 0] = medicationName[0];
                GlobalData.MedicationList[29, 1] = medicationName[1];
                GlobalData.MedicationList[29, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationNitroxazepine).Split(';');
                GlobalData.MedicationList[30, 0] = medicationName[0];
                GlobalData.MedicationList[30, 1] = medicationName[1];
                GlobalData.MedicationList[30, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationNortriptyline).Split(';');
                GlobalData.MedicationList[31, 0] = medicationName[0];
                GlobalData.MedicationList[31, 1] = medicationName[1];
                GlobalData.MedicationList[31, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationNoxiptiline).Split(';');
                GlobalData.MedicationList[32, 0] = medicationName[0];
                GlobalData.MedicationList[32, 1] = medicationName[1];
                GlobalData.MedicationList[32, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationParoxetine).Split(';');
                GlobalData.MedicationList[33, 0] = medicationName[0];
                GlobalData.MedicationList[33, 1] = medicationName[1];
                GlobalData.MedicationList[33, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationPhenelzine).Split(';');
                GlobalData.MedicationList[34, 0] = medicationName[0];
                GlobalData.MedicationList[34, 1] = medicationName[1];
                GlobalData.MedicationList[34, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationPipofezine).Split(';');
                GlobalData.MedicationList[35, 0] = medicationName[0];
                GlobalData.MedicationList[35, 1] = medicationName[1];
                GlobalData.MedicationList[35, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationPirlindole).Split(';');
                GlobalData.MedicationList[36, 0] = medicationName[0];
                GlobalData.MedicationList[36, 1] = medicationName[1];
                GlobalData.MedicationList[36, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationProtriptyline).Split(';');
                GlobalData.MedicationList[37, 0] = medicationName[0];
                GlobalData.MedicationList[37, 1] = medicationName[1];
                GlobalData.MedicationList[37, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationQuetiapine).Split(';');
                GlobalData.MedicationList[38, 0] = medicationName[0];
                GlobalData.MedicationList[38, 1] = medicationName[1];
                GlobalData.MedicationList[38, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationReboxetine).Split(';');
                GlobalData.MedicationList[39, 0] = medicationName[0];
                GlobalData.MedicationList[39, 1] = medicationName[1];
                GlobalData.MedicationList[39, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationSelegiline).Split(';');
                GlobalData.MedicationList[40, 0] = medicationName[0];
                GlobalData.MedicationList[40, 1] = medicationName[1];
                GlobalData.MedicationList[40, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationSertraline).Split(';');
                GlobalData.MedicationList[41, 0] = medicationName[0];
                GlobalData.MedicationList[41, 1] = medicationName[1];
                GlobalData.MedicationList[41, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationSetiptiline).Split(';');
                GlobalData.MedicationList[42, 0] = medicationName[0];
                GlobalData.MedicationList[42, 1] = medicationName[1];
                GlobalData.MedicationList[42, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationTofenacin).Split(';');
                GlobalData.MedicationList[43, 0] = medicationName[0];
                GlobalData.MedicationList[43, 1] = medicationName[1];
                GlobalData.MedicationList[43, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationToloxatone).Split(';');
                GlobalData.MedicationList[44, 0] = medicationName[0];
                GlobalData.MedicationList[44, 1] = medicationName[1];
                GlobalData.MedicationList[44, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationTranylcypromine).Split(';');
                GlobalData.MedicationList[45, 0] = medicationName[0];
                GlobalData.MedicationList[45, 1] = medicationName[1];
                GlobalData.MedicationList[45, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationTrazodone).Split(';');
                GlobalData.MedicationList[46, 0] = medicationName[0];
                GlobalData.MedicationList[46, 1] = medicationName[1];
                GlobalData.MedicationList[46, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationTrimipramine).Split(';');
                GlobalData.MedicationList[47, 0] = medicationName[0];
                GlobalData.MedicationList[47, 1] = medicationName[1];
                GlobalData.MedicationList[47, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationVenlafaxine).Split(';');
                GlobalData.MedicationList[48, 0] = medicationName[0];
                GlobalData.MedicationList[48, 1] = medicationName[1];
                GlobalData.MedicationList[48, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationVilazodone).Split(';');
                GlobalData.MedicationList[49, 0] = medicationName[0];
                GlobalData.MedicationList[49, 1] = medicationName[1];
                GlobalData.MedicationList[49, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationViloxazine).Split(';');
                GlobalData.MedicationList[50, 0] = medicationName[0];
                GlobalData.MedicationList[50, 1] = medicationName[1];
                GlobalData.MedicationList[50, 2] = medicationName[2];

                medicationName = context.GetString(Resource.String.MedicationVortioxetine).Split(';');
                GlobalData.MedicationList[51, 0] = medicationName[0];
                GlobalData.MedicationList[51, 1] = medicationName[1];
                GlobalData.MedicationList[51, 2] = medicationName[2];
            }
        }
        public void CreateMoodList()
        {
            try
            {
                Globals dbHelp = new Globals();
                dbHelp.OpenDatabase();
                dbHelp.GetAllMoodsForAdapter();
                dbHelp.CloseDatabase();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "CreateMoodList: Exception - " + e.Message);
            }
        }

        public void CreateAppointmentTypes(Context context)
        {
            if (context != null)
            {
                GlobalData.AppointmentTypes = new string[]
                {
                    context.GetString(Resource.String.AppointmentTypeCounsellor),
                    context.GetString(Resource.String.AppointmentTypeConsultantPsychiatrist),
                    context.GetString(Resource.String.AppointmentTypeMedicationReview),
                    context.GetString(Resource.String.AppointmentTypeCrisisTeam),
                    context.GetString(Resource.String.AppointmentTypeGroupTherapy),
                    context.GetString(Resource.String.AppointmentTypeCognitiveBehaviouralTherapy),
                    context.GetString(Resource.String.AppointmentTypeConsultation),
                    context.GetString(Resource.String.AppointmentTypeCommunityPsychiactricNurse),
                    context.GetString(Resource.String.AppointmentTypeHospitalMentalHealth),
                    context.GetString(Resource.String.AppointmentTypeHospitalConsultation),
                    context.GetString(Resource.String.AppointmentTypeHospitalScan),
                    context.GetString(Resource.String.AppointmentTypeHospitalOutpatient),
                    context.GetString(Resource.String.AppointmentTypeHospitalOperation),
                    context.GetString(Resource.String.AppointmentTypeGeneralPractitioner),
                    context.GetString(Resource.String.AppointmentTypepractiseNurse),
                    context.GetString(Resource.String.AppointmentTypeBloodTest),
                    context.GetString(Resource.String.AppointmentTypeFluJab),
                    context.GetString(Resource.String.AppointmentTypeWorkTeamMeeting),
                    context.GetString(Resource.String.AppointmentTypeWorkHumanResources),
                    context.GetString(Resource.String.AppointmentTypeWorkAppraisal),
                    context.GetString(Resource.String.AppointmentTypeWorkGeneralMeeting),
                    context.GetString(Resource.String.AppointmentTypeWorkProjectPlanning),
                    context.GetString(Resource.String.AppointmentTypeWorkTraining),
                    context.GetString(Resource.String.AppointmentTypeJobInterview)
                };
            }
        }

        public void CreateAchievementChartTypes(Context context)
        {
            if (context != null)
            {
                GlobalData.AchievementChartTypes = new string[]
                {
                    context.GetString(Resource.String.AchievementTypesGeneral),
                    context.GetString(Resource.String.AchievementTypesLife),
                    context.GetString(Resource.String.AchievementTypesWork),
                    context.GetString(Resource.String.AchievementTypesFamily),
                    context.GetString(Resource.String.AchievementTypesRelationships),
                    context.GetString(Resource.String.AchievementTypesHealth),
                    context.GetString(Resource.String.AchievementTypesFinancial),
                    context.GetString(Resource.String.AchievementTypesAffirmation),
                    context.GetString(Resource.String.AchievementTypesGoal)
                };
            }
            else
            {
                GlobalData.AchievementChartTypes = new string[]
                {
                    "General",
                    "Life",
                    "Work",
                    "Family",
                    "Relationships",
                    "Health",
                    "Financial",
                    "Affirmation",
                    "Goal"
                };
            }
        }

        public void CreateMoodListArray(Context context)
        {
            //*****************************************************************************************************
            //
            //  Note: If you add to this list remember to update the following in ConstantsAndTypes:
            //
            //  public const int MAX_NUMBER_OF_MOODLIST_ITEMS = 29; <-- increase this number
            //
            //*****************************************************************************************************

            if (context != null)
            {
                GlobalData.MoodlistArray = new string[]
                {
                    context.GetString(Resource.String.MoodListDepressed),
                    context.GetString(Resource.String.MoodListAnxious),
                    context.GetString(Resource.String.MoodListAngry),
                    context.GetString(Resource.String.MoodListGuilty),
                    context.GetString(Resource.String.MoodListAshamed),
                    context.GetString(Resource.String.MoodListSad),
                    context.GetString(Resource.String.MoodListEmbarrassed),
                    context.GetString(Resource.String.MoodListExcited),
                    context.GetString(Resource.String.MoodListFrightened),
                    context.GetString(Resource.String.MoodListIrritated),
                    context.GetString(Resource.String.MoodListInsecure),
                    context.GetString(Resource.String.MoodListProud),
                    context.GetString(Resource.String.MoodListMad),
                    context.GetString(Resource.String.MoodListPanicky),
                    context.GetString(Resource.String.MoodListFrustrated),
                    context.GetString(Resource.String.MoodListNervous),
                    context.GetString(Resource.String.MoodListDisgusted),
                    context.GetString(Resource.String.MoodListHurt),
                    context.GetString(Resource.String.MoodListCheerful),
                    context.GetString(Resource.String.MoodListDisappointed),
                    context.GetString(Resource.String.MoodListScared),
                    context.GetString(Resource.String.MoodListHappy),
                    context.GetString(Resource.String.MoodListLoving),
                    context.GetString(Resource.String.MoodListHumiliated),
                    context.GetString(Resource.String.MoodListJovial),
                    context.GetString(Resource.String.MoodListMelancholy),
                    context.GetString(Resource.String.MoodListFlirtatious),
                    context.GetString(Resource.String.MoodListAnnoyed),
                    context.GetString(Resource.String.MoodListDespairing)
                };
            }
            else
            {
                GlobalData.MoodlistArray = new string[]
                {
                    "Depressed",
                    "Anxious",
                    "Angry",
                    "Guilty",
                    "Ashamed",
                    "Sad",
                    "Embarrassed",
                    "Excited",
                    "Frightened",
                    "Irritated",
                    "Insecure",
                    "Proud",
                    "Mad",
                    "Panicky",
                    "Frustrated",
                    "Nervous",
                    "Disgusted",
                    "Hurt",
                    "Cheerful",
                    "Disappointed",
                    "Scared",
                    "Happy",
                    "Loving",
                    "Humiliated",
                    "Jovial",
                    "Melancholy",
                    "Flirtatious",
                    "Annoyed",
                    "Despairing"
                };
            }
        }

        public void CreateColourList()
        {
            GlobalData.ColourList = new int[126];
            GlobalData.ColourList[0] = Color.Blue.ToArgb();
            GlobalData.ColourList[1] = Color.Black.ToArgb();
            GlobalData.ColourList[2] = Color.Green.ToArgb();
            GlobalData.ColourList[3] = Color.Red.ToArgb();
            GlobalData.ColourList[4] = Color.Orange.ToArgb();
            GlobalData.ColourList[5] = Color.Brown.ToArgb();
            GlobalData.ColourList[6] = Color.DarkBlue.ToArgb();
            GlobalData.ColourList[7] = Color.Maroon.ToArgb();
            GlobalData.ColourList[8] = Color.Magenta.ToArgb();
            GlobalData.ColourList[9] = Color.DarkCyan.ToArgb();
            GlobalData.ColourList[10] = Color.HotPink.ToArgb();
            GlobalData.ColourList[11] = Color.CadetBlue.ToArgb();
            GlobalData.ColourList[12] = Color.DarkGreen.ToArgb();
            GlobalData.ColourList[13] = Color.DeepSkyBlue.ToArgb();
            GlobalData.ColourList[14] = Color.DarkRed.ToArgb();
            GlobalData.ColourList[15] = Color.DeepPink.ToArgb();
            GlobalData.ColourList[16] = Color.Yellow.ToArgb();
            GlobalData.ColourList[17] = Color.DarkOrange.ToArgb();
            GlobalData.ColourList[18] = Color.DarkSlateGray.ToArgb();
            GlobalData.ColourList[19] = Color.DarkViolet.ToArgb();
            GlobalData.ColourList[20] = Color.Bisque.ToArgb();
            GlobalData.ColourList[21] = Color.Crimson.ToArgb();
            GlobalData.ColourList[22] = Color.Pink.ToArgb();
            GlobalData.ColourList[23] = Color.DarkGoldenrod.ToArgb();
            GlobalData.ColourList[24] = Color.DarkGray.ToArgb();
            GlobalData.ColourList[25] = Color.Cyan.ToArgb();
            GlobalData.ColourList[26] = Color.DarkKhaki.ToArgb();
            GlobalData.ColourList[27] = Color.DarkMagenta.ToArgb();
            GlobalData.ColourList[28] = Color.DarkOliveGreen.ToArgb();
            GlobalData.ColourList[29] = Color.CornflowerBlue.ToArgb();
            GlobalData.ColourList[30] = Color.DarkOrchid.ToArgb();
            GlobalData.ColourList[31] = Color.Chartreuse.ToArgb();
            GlobalData.ColourList[32] = Color.DarkSalmon.ToArgb();
            GlobalData.ColourList[33] = Color.DarkSeaGreen.ToArgb();
            GlobalData.ColourList[34] = Color.DarkSlateBlue.ToArgb();
            GlobalData.ColourList[35] = Color.Cornsilk.ToArgb();
            GlobalData.ColourList[36] = Color.DarkTurquoise.ToArgb();
            GlobalData.ColourList[37] = Color.Beige.ToArgb();
            GlobalData.ColourList[38] = Color.Chocolate.ToArgb();
            GlobalData.ColourList[39] = Color.Coral.ToArgb();
            GlobalData.ColourList[40] = Color.DimGray.ToArgb();
            GlobalData.ColourList[41] = Color.DodgerBlue.ToArgb();
            GlobalData.ColourList[42] = Color.Firebrick.ToArgb();
            GlobalData.ColourList[43] = Color.ForestGreen.ToArgb();
            GlobalData.ColourList[44] = Color.Fuchsia.ToArgb();
            GlobalData.ColourList[45] = Color.Gainsboro.ToArgb();
            GlobalData.ColourList[46] = Color.Goldenrod.ToArgb();
            GlobalData.ColourList[47] = Color.Gray.ToArgb();
            GlobalData.ColourList[48] = Color.Aqua.ToArgb();
            GlobalData.ColourList[49] = Color.GreenYellow.ToArgb();
            GlobalData.ColourList[50] = Color.Honeydew.ToArgb();
            GlobalData.ColourList[51] = Color.BurlyWood.ToArgb();
            GlobalData.ColourList[52] = Color.IndianRed.ToArgb();
            GlobalData.ColourList[53] = Color.Indigo.ToArgb();
            GlobalData.ColourList[54] = Color.Khaki.ToArgb();
            GlobalData.ColourList[55] = Color.Lavender.ToArgb();
            GlobalData.ColourList[56] = Color.LavenderBlush.ToArgb();
            GlobalData.ColourList[57] = Color.LawnGreen.ToArgb();
            GlobalData.ColourList[58] = Color.LemonChiffon.ToArgb();
            GlobalData.ColourList[59] = Color.LightBlue.ToArgb();
            GlobalData.ColourList[60] = Color.LightCoral.ToArgb();
            GlobalData.ColourList[61] = Color.LightCyan.ToArgb();
            GlobalData.ColourList[62] = Color.LightGoldenrodYellow.ToArgb();
            GlobalData.ColourList[63] = Color.LightGray.ToArgb();
            GlobalData.ColourList[64] = Color.LightGreen.ToArgb();
            GlobalData.ColourList[65] = Color.LightPink.ToArgb();
            GlobalData.ColourList[66] = Color.LightSalmon.ToArgb();
            GlobalData.ColourList[67] = Color.LightSeaGreen.ToArgb();
            GlobalData.ColourList[68] = Color.LightSkyBlue.ToArgb();
            GlobalData.ColourList[69] = Color.LightSlateGray.ToArgb();
            GlobalData.ColourList[70] = Color.LightSteelBlue.ToArgb();
            GlobalData.ColourList[71] = Color.LightYellow.ToArgb();
            GlobalData.ColourList[72] = Color.Lime.ToArgb();
            GlobalData.ColourList[73] = Color.LimeGreen.ToArgb();
            GlobalData.ColourList[74] = Color.Linen.ToArgb();
            GlobalData.ColourList[75] = Color.BlanchedAlmond.ToArgb();
            GlobalData.ColourList[76] = Color.YellowGreen.ToArgb();
            GlobalData.ColourList[77] = Color.MediumAquamarine.ToArgb();
            GlobalData.ColourList[78] = Color.MediumBlue.ToArgb();
            GlobalData.ColourList[79] = Color.MediumOrchid.ToArgb();
            GlobalData.ColourList[80] = Color.MediumPurple.ToArgb();
            GlobalData.ColourList[81] = Color.MediumSeaGreen.ToArgb();
            GlobalData.ColourList[82] = Color.MediumSlateBlue.ToArgb();
            GlobalData.ColourList[83] = Color.MediumSpringGreen.ToArgb();
            GlobalData.ColourList[84] = Color.MediumTurquoise.ToArgb();
            GlobalData.ColourList[85] = Color.MediumVioletRed.ToArgb();
            GlobalData.ColourList[86] = Color.MidnightBlue.ToArgb();
            GlobalData.ColourList[87] = Color.MistyRose.ToArgb();
            GlobalData.ColourList[88] = Color.Moccasin.ToArgb();
            GlobalData.ColourList[89] = Color.Navy.ToArgb();
            GlobalData.ColourList[90] = Color.Olive.ToArgb();
            GlobalData.ColourList[91] = Color.OliveDrab.ToArgb();
            GlobalData.ColourList[92] = Color.AliceBlue.ToArgb();
            GlobalData.ColourList[93] = Color.OrangeRed.ToArgb();
            GlobalData.ColourList[94] = Color.Orchid.ToArgb();
            GlobalData.ColourList[95] = Color.PaleGoldenrod.ToArgb();
            GlobalData.ColourList[96] = Color.PaleGreen.ToArgb();
            GlobalData.ColourList[97] = Color.PaleTurquoise.ToArgb();
            GlobalData.ColourList[98] = Color.PaleVioletRed.ToArgb();
            GlobalData.ColourList[99] = Color.PapayaWhip.ToArgb();
            GlobalData.ColourList[100] = Color.PeachPuff.ToArgb();
            GlobalData.ColourList[101] = Color.Peru.ToArgb();
            GlobalData.ColourList[102] = Color.BlueViolet.ToArgb();
            GlobalData.ColourList[103] = Color.Plum.ToArgb();
            GlobalData.ColourList[104] = Color.PowderBlue.ToArgb();
            GlobalData.ColourList[105] = Color.Purple.ToArgb();
            GlobalData.ColourList[106] = Color.Azure.ToArgb();
            GlobalData.ColourList[107] = Color.RosyBrown.ToArgb();
            GlobalData.ColourList[108] = Color.RoyalBlue.ToArgb();
            GlobalData.ColourList[109] = Color.SaddleBrown.ToArgb();
            GlobalData.ColourList[110] = Color.Salmon.ToArgb();
            GlobalData.ColourList[111] = Color.SandyBrown.ToArgb();
            GlobalData.ColourList[112] = Color.SeaGreen.ToArgb();
            GlobalData.ColourList[113] = Color.SeaShell.ToArgb();
            GlobalData.ColourList[114] = Color.Sienna.ToArgb();
            GlobalData.ColourList[115] = Color.SkyBlue.ToArgb();
            GlobalData.ColourList[116] = Color.SlateBlue.ToArgb();
            GlobalData.ColourList[117] = Color.SlateGray.ToArgb();
            GlobalData.ColourList[118] = Color.SpringGreen.ToArgb();
            GlobalData.ColourList[119] = Color.SteelBlue.ToArgb();
            GlobalData.ColourList[120] = Color.Tan.ToArgb();
            GlobalData.ColourList[121] = Color.Thistle.ToArgb();
            GlobalData.ColourList[122] = Color.Tomato.ToArgb();
            GlobalData.ColourList[123] = Color.Turquoise.ToArgb();
            GlobalData.ColourList[124] = Color.Violet.ToArgb();
            GlobalData.ColourList[125] = Color.Aquamarine.ToArgb();
        }

        public void RetrieveEmergencyContacts(Context context)
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.ContactsUserItems == null)
                    GlobalData.ContactsUserItems = new List<Model.Contact>();
                //set the contacts main object here to prevent errors
                if (GlobalData.ContactItems == null)
                    GlobalData.ContactItems = new List<Model.Contact>();
                dbHelp.OpenDatabase();
                GlobalData.ContactsUserItems = dbHelp.GetAllUsersContacts(context);
                dbHelp.CloseDatabase();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveEmergencyContacts: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllTellMyselfEntries()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.TellMyselfItemsList == null)
                    GlobalData.TellMyselfItemsList = new List<Model.TellMyself>();
                dbHelp.OpenDatabase();
                GlobalData.TellMyselfItemsList = dbHelp.GetAllTellMyselfEntries();
                Log.Info(TAG, "RetrieveAllTellMyselfEntries: Database retrieval found " + GlobalData.TellMyselfItemsList.Count.ToString() + " items");
                dbHelp.CloseDatabase();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllTellMyselfEntries: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllGenericTextEntries()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.GenericTextItemsList == null)
                    GlobalData.GenericTextItemsList = new List<Model.GenericText>();
                dbHelp.OpenDatabase();
                GlobalData.GenericTextItemsList = dbHelp.GetAllGenericTextEntries();
                dbHelp.CloseDatabase();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllGenericTextEntries: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public List<Model.Contact> RetrieveAllContacts(Context context)
        {
            try
            {
                List<Model.Contact> contactsList = new List<Model.Contact>();
                var xamContacts = new AddressBook(context);

                IEnumerable<Contact> sortedContacts = xamContacts.AsEnumerable().OrderBy(contact => contact.FirstName);

                foreach (Contact contact in sortedContacts)
                {
                    var myContact = new Model.Contact();
                    myContact.ContactUri = contact.Id;
                    myContact.ContactName = contact.DisplayName;
                    foreach (var phone in contact.Phones)
                    {
                        string phoneNumber = "";
                        if (!string.IsNullOrEmpty(phone.Number))
                            phoneNumber = phone.Number.Trim();
                        if (phone.Type == PhoneType.Mobile)
                        {
                            myContact.ContactTelephoneNumber = phone.Number;
                            break;
                        }
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            myContact.ContactTelephoneNumber = phoneNumber;
                        }
                        else
                        {
                            myContact.ContactTelephoneNumber = "";
                        }
                    }
                    foreach(var email in contact.Emails)
                    {
                        string emailAddress = "";
                        if (!string.IsNullOrEmpty(email.Address))
                            emailAddress = email.Address.Trim();
                        if(email.Type == EmailType.Home)
                        {
                            myContact.ContactEmail = email.Address.Trim();
                            break;
                        }
                        if(!string.IsNullOrEmpty(emailAddress))
                        {
                            myContact.ContactEmail = emailAddress.Trim();
                        }
                        else
                        {
                            myContact.ContactEmail = "";
                        }
                    }
                    myContact.ContactPhoto = contact.GetThumbnail();
                    contactsList.Add(myContact);
                }
                return contactsList;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllContacts: Exception - " + e.Message);
                return null;
            }
        }
        public void RetrieveAllSafetyPlanCardEntries()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.SafetyPlanCardsItems == null)
                {
                    Log.Info(TAG, "RetrieveAllSafetyPlanCardEntries: Items is null, creating...");
                    GlobalData.SafetyPlanCardsItems = new List<Model.SafetyPlanCard>();
                }
                dbHelp.OpenDatabase();
                GlobalData.SafetyPlanCardsItems = dbHelp.GetAllSafetyPlanCards();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllSafetyPlanCardEntries: Successfully retrieved " + GlobalData.SafetyPlanCardsItems.Count.ToString() + " Safety Plan Card Items");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllSafetyPlanCardEntries: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveMedicationList()
        {
            Globals dbHelp = new Globals();
            try
            {
                if(GlobalData.MedicationItems == null)
                {
                    Log.Info(TAG, "RetrieveMedicationList: Items is null, creating...");
                    GlobalData.MedicationItems = new List<Model.Medication>();
                }
                dbHelp.OpenDatabase();
                GlobalData.MedicationItems = dbHelp.GetAllMedicationItems();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveMedicationList: Successfully retrieved " + GlobalData.MedicationItems.Count.ToString() + " Medication items");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveMedicationList: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveActivitesForCurrentWeek()
        {
            Globals dbHelp = new Globals();
            try
            {
                if(GlobalData.ActivitiesForWeek == null)
                {
                    Log.Info(TAG, "RetrieveActivitesForCurrentWeek: Activities is NULL, creating...");
                    GlobalData.ActivitiesForWeek = new List<Model.Activities>();
                }

                dbHelp.OpenDatabase();

                //if today is Monday, and there are activities from the previous week, we want to remove the activities from last week
                dbHelp.RemovePreviousWeeksActivities();

                GlobalData.ActivitiesForWeek = dbHelp.GetAllActivitiesForCurrentWeek();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveActivitesForCurrentWeek: Retrieved " + GlobalData.ActivitiesForWeek.Count.ToString() + " Activity items");
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveActivitesForCurrentWeek: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllReactions()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.StructuredPlanReactions == null)
                {
                    Log.Info(TAG, "RetrieveAllReactions: StructuredPlanReactions is NULL, creating...");
                    GlobalData.StructuredPlanReactions = new List<Model.Reactions>();
                }

                dbHelp.OpenDatabase();
                GlobalData.StructuredPlanReactions = dbHelp.GetAllReactions();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllReactions: Retrieved " + GlobalData.StructuredPlanReactions.Count.ToString() + " Reaction items");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllReactions: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllFeelings()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.StructuredPlanFeelings == null)
                {
                    Log.Info(TAG, "RetrieveAllFeelings: StructuredPlanFeelings is NULL, creating...");
                    GlobalData.StructuredPlanFeelings = new List<Model.Feelings>();
                }

                dbHelp.OpenDatabase();
                GlobalData.StructuredPlanFeelings = dbHelp.GetAllFeelings();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllFeelings: Retrieved " + GlobalData.StructuredPlanFeelings.Count.ToString() + " Feeling items");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllFeelings: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllAttitudes()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.StructuredPlanAttitudes == null)
                {
                    Log.Info(TAG, "RetrieveAllAttitudes: StructuredPlanAttitudes is NULL, creating...");
                    GlobalData.StructuredPlanAttitudes = new List<Model.Attitudes>();
                }

                dbHelp.OpenDatabase();
                GlobalData.StructuredPlanAttitudes = dbHelp.GetAllAttitudes();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllAttitudes: Retrieved " + GlobalData.StructuredPlanAttitudes.Count.ToString() + " Attitude items");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllAttitudes: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllRelationships()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.StructuredPlanRelationships == null)
                {
                    Log.Info(TAG, "RetrieveAllRelationships: StructuredPlanRelationships is NULL, creating...");
                    GlobalData.StructuredPlanRelationships = new List<Model.Relationships>();
                }

                dbHelp.OpenDatabase();
                GlobalData.StructuredPlanRelationships = dbHelp.GetAllRelationships();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllRelationships: Retrieved " + GlobalData.StructuredPlanRelationships.Count.ToString() + " Relationship items");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllRelationships: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllHealth()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.StructuredPlanHealth == null)
                {
                    Log.Info(TAG, "RetrieveAllHealth: StructuredPlanHealth is NULL, creating...");
                    GlobalData.StructuredPlanHealth = new List<Model.Health>();
                }

                dbHelp.OpenDatabase();
                GlobalData.StructuredPlanHealth = dbHelp.GetAllHealth();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllHealth: Retrieved " + GlobalData.StructuredPlanHealth.Count.ToString() + " Health items");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllHealth: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllFantasies()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.StructuredPlanFantasies == null)
                {
                    Log.Info(TAG, "RetrieveAllFantasies: StructuredPlanFantasies is NULL, creating...");
                    GlobalData.StructuredPlanFantasies = new List<Model.Fantasies>();
                }

                dbHelp.OpenDatabase();
                GlobalData.StructuredPlanFantasies = dbHelp.GetAllFantasies();
                dbHelp.CloseDatabase();
                Log.Info(TAG, "RetrieveAllFantasies: Retrieved " + GlobalData.StructuredPlanFantasies.Count.ToString() + " Fantasy items");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllFantasies: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllProblems()
        {
            Globals dbHelp = new Globals();
            try
            {
                if(GlobalData.ProblemSolvingItems == null)
                {
                    Log.Info(TAG, "RetrieveAllProblems: ProblemSolvingItems is NULL, creating...");
                    GlobalData.ProblemSolvingItems = new List<Model.Problem>();
                }

                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    GlobalData.ProblemSolvingItems = dbHelp.GetAllProblems();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllProblems: Retrieved " + GlobalData.ProblemSolvingItems.Count.ToString() + " problem items");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllProblems: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllSolutionPlans()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.SolutionPlansItems == null)
                {
                    Log.Info(TAG, "RetrieveAllSolutionPlans: SolutionPlansItems is NULL, creating...");
                    GlobalData.SolutionPlansItems = new List<Model.SolutionPlan>();
                }

                dbHelp.OpenDatabase();
                if (dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    GlobalData.SolutionPlansItems = dbHelp.GetAllSolutionPlans();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllSolutionPlans: Retrieved " + GlobalData.SolutionPlansItems.Count.ToString() + " solution plan items");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllSolutionPlans: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllAffirmations()
        {
            Globals dbHelp = new Globals();
            try
            {
                if(GlobalData.AffirmationListItems == null)
                {
                    Log.Info(TAG, "RetrieveAllAffirmations: AffirmationListItems is NULL, creating...");
                    GlobalData.AffirmationListItems = new List<Model.Affirmation>();
                }

                dbHelp.OpenDatabase();
                if (dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    GlobalData.AffirmationListItems = dbHelp.GetAllAffirmations();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllAffirmations: Retrieved " + GlobalData.AffirmationListItems.Count.ToString() + " affirmation items");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllAffirmations: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllImages()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.ImageListItems == null)
                {
                    Log.Info(TAG, "RetrieveAllImages: ImageListItems is NULL, creating...");
                    GlobalData.ImageListItems = new List<Model.Imagery>();
                }

                dbHelp.OpenDatabase();
                if (dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    GlobalData.ImageListItems = dbHelp.GetAllImages();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllImages: Retrieved " + GlobalData.ImageListItems.Count.ToString() + " Image items");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllImages: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllPlayLists()
        {
            Globals dbHelp = new Globals();
            try
            {
                if (GlobalData.PlayListItems == null)
                {
                    Log.Info(TAG, "RetrieveAllPlayLists: PlayListItems is NULL, creating...");
                    GlobalData.PlayListItems = new List<Model.PlayList>();
                }

                dbHelp.OpenDatabase();
                if (dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    GlobalData.PlayListItems = dbHelp.GetAllPlayLists();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllPlayLists: Retrieved " + GlobalData.PlayListItems.Count.ToString() + " PlayList items");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "RetrieveAllPlayLists: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllResourceMedicationTypes()
        {
            Globals dbHelp = new Globals();

            try
            {
                if(GlobalData.ResourceMedicationTypes == null)
                {
                    GlobalData.ResourceMedicationTypes = new List<Model.ResourceMedicationType>();
                }

                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.GetAllResourceMedicationTypes();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllResourceMedicationTypes: Retrieved " + GlobalData.ResourceMedicationTypes.Count.ToString() + " Medication Types");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllResourceMedicationTypes: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllResourceConditions()
        {
            Globals dbHelp = new Globals();

            try
            {
                if (GlobalData.ResourceConditions == null)
                    GlobalData.ResourceConditions = new List<Model.ResourceCondition>();

                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.GetAllResourceConditions();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllResourceConditions: Retrieved " + GlobalData.ResourceConditions.Count.ToString() + " Conditions");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllResourceConditions: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void RetrieveAllSettings()
        {
            Globals dbHelp = new Globals();

            try
            {
                if (GlobalData.Settings == null)
                    GlobalData.Settings = new List<Model.Setting>();

                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    dbHelp.GetAllSettings();
                    dbHelp.CloseDatabase();
                    Log.Info(TAG, "RetrieveAllSettings: Retrieved " + GlobalData.Settings.Count.ToString() + " Settings");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "RetrieveAllSettings: Exception - " + e.Message);
                if (dbHelp.GetSQLiteDatabase() != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        public void GetErrorAlertShowSetting()
        {
            try
            {
                if(GlobalData.Settings != null)
                {
                    var setting = GlobalData.Settings.Find(set => set.SettingKey == "ShowErrorDialog");
                    if(setting != null)
                    {
                        GlobalData.ShowErrorDialog = (setting.SettingValue == "True" ? true : false);
                    }
                    else
                    {
                        //default to showing errors
                        GlobalData.ShowErrorDialog = true;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetErrorAlertShowSetting: Exception - " + e.Message);
            }
        }
    }
}