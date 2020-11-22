using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.SubActivities.ProblemSolving;
using Android.Graphics;


namespace com.spanyardie.MindYourMood.Adapters
{
    public class ProblemSolvingListAdapter : BaseAdapter
    {
        public const string TAG = "M:ProblemSolvingListAdapter";

        Activity _activity;

        private List<Problem> _problemList;

        private TextView _problemSolvingListItemLabel;
        private TextView _problemText;
        private TextView _steps;
        private TextView _ideas;
        private TextView _prosAndCons;
        private TextView _problemSolved;

        private ImageView _greenTick;
        private ImageButton _problemDetail;

        private int _position;


        public ProblemSolvingListAdapter(Activity activity)
        {
            _activity = activity;
            _problemList = new List<Problem>();

            GetProblemData();
        }

        private void GetProblemData()
        {
            _problemList = GlobalData.ProblemSolvingItems;
        }

        public override int Count
        {
            get
            {
                return _problemList.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _problemList[position].ProblemID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.ProblemSolvingListItem, parent, false);
                    }
                    else if (parent != null)
                    {
                        LayoutInflater inflater = (LayoutInflater)parent.Context.GetSystemService(Context.LayoutInflaterService);
                        convertView = inflater.Inflate(Resource.Layout.ProblemSolvingListItem, parent, false);
                    }
                    else
                    {
                        return convertView;
                    }
                }

                if(convertView != null)
                {
                    _position = position;

                    GetFieldComponents(convertView);

                    SetupCallbacks();

                    if(_problemText != null)
                    {
                        _problemText.Text = _problemList[position].ProblemText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _problemText is NULL!");
                    }
                    if(_problemDetail != null)
                    {
                        _problemDetail.Tag = _problemList[position].ProblemID.ToString() + ":" + _problemList[position].ProblemText.Trim();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _problemDetail is NULL!");
                    }
                    if(_steps != null)
                    {
                        _steps.Text = GetStepsCountInProblem(position).ToString();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _steps is NULL!");
                    }
                    if (_ideas != null)
                    {
                        _ideas.Text = GetIdeasCountInProblem(position).ToString();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _ideas is NULL!");
                    }
                    if (_prosAndCons != null)
                    {
                        _prosAndCons.Text = GetProsAndConsCountInProblem(position).ToString();
                    }
                    else
                    {
                        Log.Error(TAG, "GetView: _prosAndCons is NULL!");
                    }

                    //has problem been solved?
                    SeeIfProblemWasSolved(convertView, position);

                    if (position == ((ProblemSolvingActivity)_activity).GetSelectedItemIndex())
                    {
                        convertView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));

                        if(_problemSolvingListItemLabel != null)
                            _problemSolvingListItemLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if (_problemText != null)
                            _problemText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_steps != null)
                            _steps.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_ideas != null)
                            _ideas.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_prosAndCons != null)
                            _prosAndCons.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                        if(_problemSolved != null)
                            _problemSolved.SetBackgroundColor(Color.Argb(255, 19, 75, 127));

                        if (_problemDetail != null)
                            _problemDetail.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                    }
                    else
                    {
                        convertView.SetBackgroundDrawable(null);
                        if (_problemSolvingListItemLabel != null)
                            _problemSolvingListItemLabel.SetBackgroundDrawable(null);
                        if (_problemText != null)
                            _problemText.SetBackgroundDrawable(null);
                        if (_steps != null)
                            _steps.SetBackgroundDrawable(null);
                        if (_ideas != null)
                            _ideas.SetBackgroundDrawable(null);
                        if (_prosAndCons != null)
                            _prosAndCons.SetBackgroundDrawable(null);
                        if (_problemSolved != null)
                            _problemSolved.SetBackgroundDrawable(null);

                        if (_problemDetail != null)
                            _problemDetail.SetBackgroundDrawable(null);
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
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingListGetView), "ProblemSolvingListAdapter.GetView");
                return convertView;
            }
        }

        private void SeeIfProblemWasSolved(View view, int position)
        {
            try
            {
                if (view != null)
                {
                    if (_problemList[position].IsProblemSolved())
                    {
                        _greenTick = view.FindViewById<ImageView>(Resource.Id.imgProblemIsSolved);
                        if (_greenTick != null)
                            _greenTick.Visibility = ViewStates.Visible;
                        _problemSolved = view.FindViewById<TextView>(Resource.Id.txtProblemSolved);
                        if (_problemSolved != null)
                            _problemSolved.Visibility = ViewStates.Visible;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SeeIfProblemWasSolved: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Checking to see if problem was solved", "ProblemSolvingListAdapter.SeeIfProblemWasSolved");
            }
        }

        private void GetFieldComponents(View view)
        {
            try
            {
                _problemSolvingListItemLabel = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingListItemLabel);
                _problemText = view.FindViewById<TextView>(Resource.Id.txtProblemSolvingListItemText);
                _steps = view.FindViewById<TextView>(Resource.Id.txtStepsInProblem);
                _ideas = view.FindViewById<TextView>(Resource.Id.txtIdeasInProblem);
                _prosAndCons = view.FindViewById<TextView>(Resource.Id.txtProsAndConsInProblem);
                _problemSolved = view.FindViewById<TextView>(Resource.Id.txtProblemSolved);

                _problemDetail = view.FindViewById<ImageButton>(Resource.Id.imgbtnProblemSolvingListItemDetail);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingListGetComponents), "ProblemSolvingListAdapter.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if(_problemDetail != null)
                {
                    _problemDetail.Click += ProblemDetail_Click;
                }
                else
                {
                    Log.Error(TAG, "SetupCallbacks: _problemDetail is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingListSetCallbacks), "ProblemSolvingListAdapter.SetupCallbacks");
            }
        }

        private void ProblemDetail_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(_activity, typeof(ProblemSolvingStepsActivity));

                string taggedData = (string)((ImageButton)sender).Tag;

                string[] data = taggedData.Split(':');

                intent.PutExtra("problemID", Convert.ToInt32(data[0]));
                intent.PutExtra("problemText", data[1]);

                _activity.StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ProblemDetail_Click: Exception - " + ex.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, "Error viewing Problem detail", "ProblemSolvingListAdapter.ProblemDetail_Click");
            }
        }

        private int GetStepsCountInProblem(int position)
        {
            int stepCount = 0;

            try
            {
                if (_problemList != null)
                {
                    stepCount = _problemList[position].ProblemSteps.Count;
                }
                else
                {
                    Log.Error(TAG, "GetStepsCountInProblem: _problemList is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetStepsCountInProblem: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingListStepsCount), "ProblemSolvingListAdapter.GetStepsCountInProblem");
            }

            return stepCount;
        }

        private int GetIdeasCountInProblem(int position)
        {
            int ideasCount = 0;

            try
            {
                if (_problemList != null)
                {
                    Problem prob = _problemList[position];
                    foreach(ProblemStep step in prob.ProblemSteps)
                    {
                        ideasCount += step.ProblemStepIdeas.Count;
                    }
                }
                else
                {
                    Log.Error(TAG, "GetIdeasCountInProblem: _problemList is NULL!");
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetIdeasCountInProblem: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingListIdeasCount), "ProblemSolvingListAdapter.GetIdeasCountInProblem");
            }

            return ideasCount;
        }

        private int GetProsAndConsCountInProblem(int position)
        {
            int prosAndConsCount = 0;

            try
            {
                if (_problemList != null)
                {
                    Problem prob = _problemList[position];
                    foreach (ProblemStep step in prob.ProblemSteps)
                    {
                        foreach (ProblemIdea idea in step.ProblemStepIdeas)
                        {
                            prosAndConsCount += idea.ProsAndCons.Count;
                        }
                    }
                }
                else
                {
                    Log.Error(TAG, "GetProsAndConsCountInProblem: _problemList is NULL!");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetProsAndConsCountInProblem: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingListProAndConCount), "ProblemSolvingListAdapter.GetProsAndConsCountInProblem");
            }

            return prosAndConsCount;
        }
    }
}