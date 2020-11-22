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
    public class ProblemSolvingProsAndConsListAdapter : BaseAdapter
    {
        public const string TAG = "M:ProblemSolvingProsAndConsListAdapter";

        Activity _activity;

        private List<ProblemProAndCon> _problemProAndConList;

        private int _problemID;
        private int _problemStepID;
        private int _problemIdeaID;

        private TextView _problemSolvingProsAndConsTypeLabel;
        private TextView _proAndConType;
        private TextView _proAndConText;

        public ProblemSolvingProsAndConsListAdapter(Activity activity, int problemID, int problemStepID, int problemIdeaID)
        {
            _activity = activity;
            _problemID = problemID;
            _problemStepID = problemStepID;
            _problemIdeaID = problemIdeaID;

            _problemProAndConList = new List<ProblemProAndCon>();

            GetAllProblemProAndConData();
        }

        private void GetAllProblemProAndConData()
        {
            try
            {
                var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
                if (problem != null)
                {
                    var problemStep = problem.ProblemSteps.Find(step => step.ProblemStepID == _problemStepID);
                    if (problemStep != null)
                    {
                        var problemIdea = problemStep.ProblemStepIdeas.Find(idea => idea.ProblemIdeaID == _problemIdeaID);
                        if (problemIdea != null)
                        {
                            _problemProAndConList = problemIdea.ProsAndCons;
                        }
                        else
                        {
                            Log.Info(TAG, "GetAllProblemProAndConData: problem idea could not be located in the problem step");
                        }
                    }
                    else
                    {
                        Log.Info(TAG, "GetAllProblemProAndConData: problem step could not be located in the problem");
                    }
                }
                else
                {
                    Log.Info(TAG, "GetAllProblemProAndConData: problem could not be located in Global cache");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetAllProblemProAndConData: Exception: " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingProsAndConsGetData), "ProblemSolvingProsAndConsListAdapter.GetAllProblemProAndConData");
            }
        }

        public override int Count
        {
            get
            {
                if (_problemProAndConList != null)
                    return _problemProAndConList.Count;
                return 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_problemProAndConList != null)
                return _problemProAndConList[position].ProblemProAndConID;

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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ProblemSolvingProsAndConsListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.ProblemSolvingProsAndConsListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if (convertView != null)
                {
                    GetFieldComponents(convertView);

                    if (_proAndConText != null)
                    {
                        _proAndConText.Text = _problemProAndConList[position].ProblemProAndConText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _proAndConText is NULL!");
                    }
                    if(_proAndConType != null)
                    {
                        _proAndConType.Text = StringHelper.ProConTypeForConstant(_problemProAndConList[position].ProblemProAndConType);
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _proAndConType is NULL!");
                    }
                    if (position == ((ProblemSolvingProsAndConsActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_problemSolvingProsAndConsTypeLabel != null)
                            _problemSolvingProsAndConsTypeLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_proAndConText != null)
                            _proAndConText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_proAndConType != null)
                            _proAndConType.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_problemSolvingProsAndConsTypeLabel != null)
                            _problemSolvingProsAndConsTypeLabel.SetBackgroundDrawable(null);
                        if (_proAndConText != null)
                            _proAndConText.SetBackgroundDrawable(null);
                        if (_proAndConType != null)
                            _proAndConType.SetBackgroundDrawable(null);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingProsAndConsGetView), "ProblemSolvingProsAndConsListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _problemSolvingProsAndConsTypeLabel = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingProsAndConsTypeLabel);
                    _proAndConText = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingProsAndConsText);
                    _proAndConType = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingProsAndConsTypeText);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingProsAndConsGetComponents), "ProblemSolvingProsAndConsListAdapter.GetFieldComponents");
            }
        }
    }
}