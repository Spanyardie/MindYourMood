using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.SubActivities.Resources;
using Android.Graphics;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Model.Interfaces;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class AppointmentQuestionListAdapter : BaseAdapter
    {
        public const string TAG = "M:AppointmentQuestionListAdapter";

        private List<AppointmentQuestion> _questions = null;

        private Activity _activity = null;
        private TextView _question = null;
        private ImageButton _answerQuestion = null;

        public AppointmentQuestionListAdapter(Activity activity, List<AppointmentQuestion> questions)
        {
            _activity = activity;
            _questions = questions;
        }

        public override int Count
        {
            get
            {
                return _questions.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _questions[position].QuestionsID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                int selectedItemIndex = -1;
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.AppointmentQuestionListItem, parent, false);
                    }
                }

                GetFieldComponents(convertView, position);
                SetupCallbacks();

                if (_question != null)
                    _question.Text = _questions?[position].Question;

                if (_activity != null)
                    selectedItemIndex = ((ResourcesAppointmentItemActivity)_activity).GetSelectedItemIndex();

                if (position == selectedItemIndex)
                {
                    convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    if (_question != null)
                        _question.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                }
                else
                {
                    convertView.SetBackgroundDrawable(null);
                    if (_question != null)
                        _question.SetBackgroundDrawable(null);
                }

                if (_answerQuestion != null)
                    _answerQuestion.Tag = position;

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting view", "AppointmentQuestionListAdapter.GetView");
                return null;
            }
        }
        private void GetFieldComponents(View convertView, int position)
        {
            try
            {
                if (convertView != null)
                {
                    _question = convertView.FindViewById<TextView>(Resource.Id.txtAppointmentQuestion);
                    _answerQuestion = convertView.FindViewById<ImageButton>(Resource.Id.imgbtnAnswerQuestion);
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting field components", "AppointmentQuestionListAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_answerQuestion != null)
                    _answerQuestion.Click += AnswerQuestion_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting up callbacks", "AppointmentQuestionListAdapter.SetupCallbacks");
            }
        }

        private void AnswerQuestion_Click(object sender, EventArgs e)
        {
            try
            {
                int position = Convert.ToInt32(((ImageButton)sender).Tag);
                if (_activity != null)
                {
                    Log.Info(TAG, "AnswerQuestion_Click: Calling to parent activity InitiateQuestionAnswer");
                    ((IAnswerQuestion)_activity).InitiateQuestionAnswer(position);
                    Log.Info(TAG, "AnswerQuestion_Click: Returned from call to InitiateQuestionAnswer");
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AnswerQuestion_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Answering question", "AppointmentQuestionListAdapter.AnswerQuestion_Click");
            }
        }
    }
}