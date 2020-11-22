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
    public class ProblemSolvingIdeasListAdapter : BaseAdapter
    {
        public const string TAG = "M:ProblemSolvingIdeasListAdapter";

        Activity _activity;

        private List<ProblemIdea> _problemIdeaList;

        private int _problemID;
        private int _problemStepID;

        private TextView _problemIdea;
        private ImageButton _toProsAndCons;

        public ProblemSolvingIdeasListAdapter(Activity activity, int problemID, int problemStepID)
        {
            _activity = activity;
            _problemID = problemID;
            _problemStepID = problemStepID;

            _problemIdeaList = new List<ProblemIdea>();

            GetAllProblemIdeasData();
        }

        private void GetAllProblemIdeasData()
        {
            var problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
            if (problem != null)
            {
                var problemStep = problem.ProblemSteps.Find(step => step.ProblemStepID == _problemStepID);
                if(problemStep != null)
                {
                    _problemIdeaList = problemStep.ProblemStepIdeas;
                }
                else
                {
                    Log.Info(TAG, "GetAllProblemIdeasData: problem step could not be located in the problem");
                }
            }
            else
            {
                Log.Info(TAG, "GetAllProblemIdeasData: problem could not be located in Global cache");
            }
        }

        public override int Count
        {
            get
            {
                if (_problemIdeaList != null)
                    return _problemIdeaList.Count;
                return 0;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            if (_problemIdeaList != null)
                return _problemIdeaList[position].ProblemIdeaID;

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
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ProblemSolvingIdeasListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.ProblemSolvingIdeasListItem, parent, false);
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

                    if (_problemIdea != null)
                    {
                        _problemIdea.Text = _problemIdeaList[position].ProblemIdeaText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _problemIdea is NULL!");
                    }
                    if (_toProsAndCons != null)
                    {
                        _toProsAndCons.Tag = _problemID.ToString() + ":" + _problemStepID.ToString() + ":" + _problemIdeaList[position].ProblemIdeaID.ToString() + ":" + _problemIdeaList[position].ProblemIdeaText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _toProsAndCons is NULL!");
                    }
                    if (position == ((ProblemSolvingIdeasActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_problemIdea != null)
                            _problemIdea.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_problemIdea != null)
                            _problemIdea.SetBackgroundDrawable(null);
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingIdeasListGetView), "ProblemSolvingIdeasListAdapter.GetView");
                return convertView;
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                if (view != null)
                {
                    _problemIdea = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingIdeasText);
                    _toProsAndCons = view.FindViewById<ImageButton>(Resource.Id.imgbtnProblemSolvingIdeasToProsAndCons);
                }
                else
                {
                    Log.Error(TAG, "GetFieldComponents: view is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingIdeasListGetComponents), "ProblemSolvingIdeasListAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_toProsAndCons != null)
                {
                    _toProsAndCons.Click += ToProsAndCons_Click;
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingIdeasListSetCallbacks), "ProblemSolvingIdeasListAdapter.SetupCallbacks");
            }
        }

        private void ToProsAndCons_Click(object sender, EventArgs e)
        {
            try
            {
                string taggedData = (string)((ImageButton)sender).Tag;

                string[] idData = taggedData.Split(':');

                var problemID = Convert.ToInt32(idData[0]);
                var problemStepID = Convert.ToInt32(idData[1]);
                var problemIdeaID = Convert.ToInt32(idData[2]);
                var problemIdeaText = idData[3];

                //now load the Pros and Cons activity
                Intent intent = new Intent(_activity, typeof(ProblemSolvingProsAndConsActivity));
                intent.PutExtra("problemID", problemID);
                intent.PutExtra("problemStepID", problemStepID);
                intent.PutExtra("problemIdeaID", problemIdeaID);
                intent.PutExtra("problemIdeaText", problemIdeaText);

                _activity.StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ToProsAndCons_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, _activity.GetString(Resource.String.ErrorProblemSolvingIdeasListNavToProsAndCons), "ProblemSolvingIdeasListAdapter.ToProsAndCons_Click");
            }
        }
    }
}