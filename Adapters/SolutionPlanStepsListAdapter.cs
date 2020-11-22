using System.Collections.Generic;
using System.Linq;
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
    public class SolutionPlanStepsListAdapter : BaseAdapter
    {
        public const string TAG = "M:SolutionPlanStepsListAdapter";

        Activity _activity;

        private List<SolutionPlan> _solutionStepList;

        private int _problemIdeaID;

        private TextView _priority;
        private TextView _solutionStep;

        private TextView _priorityLabel;
        private TextView _stepLabel;

        public SolutionPlanStepsListAdapter(Activity activity, int problemIdeaID)
        {
            _activity = activity;
            _problemIdeaID = problemIdeaID;

            _solutionStepList = new List<SolutionPlan>();

            GetAllSolutionStepsData();
        }

        private void GetAllSolutionStepsData()
        {
            _solutionStepList =
                (from eachStep in GlobalData.SolutionPlansItems
                 where eachStep.ProblemIdeaID == _problemIdeaID
                 select eachStep).ToList();
        }

        public override int Count
        {
            get
            {
                if (_solutionStepList != null)
                    return _solutionStepList.Count;
                return 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_solutionStepList != null)
                return _solutionStepList[position].SolutionPlanID;

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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.SolutionPlanStepListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.SolutionPlanStepListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if (convertView != null)
                {
                    GetFieldComponents(convertView);

                    if (_priority != null)
                    {
                        _priority.Text = _solutionStepList[position].PriorityOrder.ToString();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _priority is NULL!");
                    }
                    if (_solutionStep != null)
                    {
                        _solutionStep.Text = _solutionStepList[position].SolutionStep.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _solutionStep is NULL!");
                    }

                    if (position == ((SolutionPlanActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_priority != null)
                            _priority.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_priorityLabel != null)
                            _priorityLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));

                        if (_solutionStep != null)
                            _solutionStep.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_stepLabel != null)
                            _stepLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_priority != null)
                            _priority.SetBackgroundDrawable(null);
                        if (_priorityLabel != null)
                            _priorityLabel.SetBackgroundDrawable(null);

                        if (_solutionStep != null)
                            _solutionStep.SetBackgroundDrawable(null);
                        if (_stepLabel != null)
                            _stepLabel.SetBackgroundDrawable(null);
                    }
                }
                else
                {
                    Log.Error(TAG, "GetView: view is NULL!");
                }

                return convertView;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetView: Exception: " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorSolutionPlanStepsListGetView), "SolutionPlanStepsListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _priority = view.FindViewById<TextView>(Resource.Id.txtSolutionStepsItemDataPriority);
                    _solutionStep = view.FindViewById<TextView>(Resource.Id.txtSolutionStepsItemDataStep);
                    _priorityLabel = view.FindViewById<TextView>(Resource.Id.txtSolutionStepsItemTitlePriority);
                    _stepLabel = view.FindViewById<TextView>(Resource.Id.txtSolutionStepsItemTitleStep);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorSolutionPlanStepsListGetComponents), "SolutionPlanStepsListAdapter.GetFieldComponents");
            }
        }
    }
}