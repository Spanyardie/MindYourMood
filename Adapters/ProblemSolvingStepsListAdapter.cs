using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.SubActivities.ProblemSolving;
using Android.Graphics;
using System;
using Android.Content;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class ProblemSolvingStepsListAdapter : BaseAdapter
    {
        public const string TAG = "M:ProblemSolvingStepsListAdapter";

        Activity _activity;

        private List<ProblemStep> _problemStepList;

        private int _problemID;

        private TextView _problemStepsItemTitlePriority;
        private TextView _problemStepsItemTitleStep;
        private TextView _priority;
        private TextView _problemStep;
        private ImageButton _toIdeas;
        

        public ProblemSolvingStepsListAdapter(Activity activity, int problemID)
        {
            _activity = activity;
            _problemID = problemID;

            _problemStepList = new List<ProblemStep>();

            GetAllProblemStepsData();
        }

        private void GetAllProblemStepsData()
        {
            var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
            if(problem != null)
            {
                _problemStepList = problem.ProblemSteps;
            }
            else
            {
                Log.Info(TAG, "GetAllProblemStepsData: problem could not be located in Global cache");
            }
        }

        public override int Count
        {
            get
            {
                if(_problemStepList != null)
                    return _problemStepList.Count;
                return 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if(_problemStepList != null)
                return _problemStepList[position].ProblemStepID;

            return -1;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ProblemSolvingStepsListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.ProblemSolvingStepsListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if (convertView != null)
                {
                    GetFieldComponents(convertView);

                    SetupCallbacks();

                    if (_priority != null)
                    {
                        _priority.Text = _problemStepList[position].PriorityOrder.ToString();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _priority is NULL!");
                    }
                    if(_problemStep != null)
                    {
                        _problemStep.Text = _problemStepList[position].ProblemStep.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _problemStep is NULL!");
                    }
                    if(_toIdeas != null)
                    {
                        _toIdeas.Tag = _problemID.ToString() + ":" + _problemStepList[position].ProblemStepID.ToString() + ":" + _problemStep.Text.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _toIdeas is NULL!");
                    }
                    if (position == ((ProblemSolvingStepsActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_problemStepsItemTitlePriority != null)
                            _problemStepsItemTitlePriority.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_problemStepsItemTitleStep != null)
                            _problemStepsItemTitleStep.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_priority != null)
                            _priority.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_problemStep != null)
                            _problemStep.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_problemStepsItemTitlePriority != null)
                            _problemStepsItemTitlePriority.SetBackgroundDrawable(null);
                        if (_problemStepsItemTitleStep != null)
                            _problemStepsItemTitleStep.SetBackgroundDrawable(null);
                        if (_priority != null)
                            _priority.SetBackgroundDrawable(null);
                        if (_problemStep != null)
                            _problemStep.SetBackgroundDrawable(null);
                    }
                }
                else
                {
                    Log.Error(TAG, "GetView: view is NULL!");
                }

                return convertView;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception: " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingStepsListGetView), "ProblemSolvingStepsListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _problemStepsItemTitlePriority = view.FindViewById<TextView>(Resource.Id.txtProblemStepsItemTitlePriority);
                    _problemStepsItemTitleStep = view.FindViewById<TextView>(Resource.Id.txtProblemStepsItemTitleStep);
                    _priority = view.FindViewById<TextView>(Resource.Id.txtProblemStepsItemDataPriority);
                    _problemStep = view.FindViewById<TextView>(Resource.Id.txtProblemStepsItemDataStep);
                    _toIdeas = view.FindViewById<ImageButton>(Resource.Id.imgbtnProblemStepsListItemToIdeas);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingStepsListGetComponents), "ProblemSolvingStepsListAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_toIdeas != null)
                {
                    _toIdeas.Click += ToIdeas_Click;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingStepsListSetCallbacks), "ProblemSolvingStepsListAdapter.SetupCallbacks");
            }
        }

        private void ToIdeas_Click(object sender, EventArgs e)
        {
            try
            {
                string taggedData = (string)((ImageButton)sender).Tag;

                string[] idData = taggedData.Split(':');

                var problemID = Convert.ToInt32(idData[0]);
                var problemStepID = Convert.ToInt32(idData[1]);
                var problemStepText = idData[2];

                //now load the Ideas activity
                Intent intent = new Intent(_activity, typeof(ProblemSolvingIdeasActivity));
                intent.PutExtra("problemID", problemID);
                intent.PutExtra("problemStepID", problemStepID);
                intent.PutExtra("problemStepText", problemStepText);

                _activity.StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ToIdeas_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Error trying to view Ideas", "ProblemSolvingStepsListAdapter.ToIdeas_Click");
            }
        }
    }
}