using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;

using Android.Graphics;
using Android.App;
using com.spanyardie.MindYourMood.SubActivities.ProblemSolving;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ProblemSolvingReviewHelper
    {
        public const string TAG = "M:ProblemSolvingReviewHelper";

        public enum ClearView
        {
            False = 0,
            True
        }

        private Context _context;
        private Activity _activity;

        private int _problemID = -1;

        private Problem _problem;

        public ProblemSolvingReviewHelper(Activity activity, Context context)
        {
            try
            {
                _activity = activity;
                _context = context;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "Constructor: Exception - " + e.Message);
                throw;
            }
        }

        private void GetProblem()
        {
            try
            {
                if (_problemID != -1)
                {
                    if (GlobalData.ProblemSolvingItems.Count > 0)
                    {
                        _problem = GlobalData.ProblemSolvingItems.Find(prob => prob.ProblemID == _problemID);
                        if (_problem == null)
                            throw new Exception("Unable to find problem with ID " + _problemID.ToString() + " in global cache");
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetProblem: Exception - " + e.Message);
                throw;
            }
        }

        public LinearLayout GetProblemDisplay(int problemID)
        {
            //We have a problem, so this is how we want to return data for review:
            //
            //  Problem: A problem
            //      Step: First step
            //          Idea: First idea for first step
            //          Pros and cons: For first idea
            //              Pro
            //              Pro
            //              Con
            //              Con
            //          Idea: Second idea for first step
            //          Pros and cons: For second idea
            //              Pro
            //              Pro
            //              Con
            //              Con
            //          ... etc for all ideas for first step
            //          Select Solution Plan
            //          Provide Review of step - date achieved - stopped etc
            //
            //      Step: Second step
            //          Idea: First idea for second step
            //          Pros and cons: For first idea
            //              Pro
            //              Pro
            //              Con
            //              Con
            //          ... etc for all ideas for second step
            //      ... etc for all steps for A problem
            //---------------------------------------------------------------------
            //  Problem: A second problem...
            //
            //*********************************************************************
            LayoutInflater inflater = null;
            LinearLayout problemMainLayout = null;

            try
            {
                _problemID = problemID;
                GetProblem();
            }
            catch(Exception eg)
            {
                Log.Error(TAG, "GetProblemDisplay: Exception - " + eg.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, eg, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperGetProblem), "ProblemSolvingReviewHelper.GetProblemDisplay");
            }

            var screenSize = SystemHelper.GetScreenSize();
            int verticalPadding;
            int horizontalPadding;
            switch(screenSize)
            {
                case ConstantsAndTypes.ScreenSize.Normal:
                    verticalPadding = (int)_context.Resources.GetDimension(Resource.Dimension.mindyourmood_padding_vertical_normal);
                    break;
                case ConstantsAndTypes.ScreenSize.Large:
                    verticalPadding = (int)_context.Resources.GetDimension(Resource.Dimension.mindyourmood_padding_vertical_large);
                    break;
                case ConstantsAndTypes.ScreenSize.ExtraLarge:
                    verticalPadding = (int)_context.Resources.GetDimension(Resource.Dimension.mindyourmood_padding_vertical_xlarge);
                    break;
                default:
                    verticalPadding = (int)_context.Resources.GetDimension(Resource.Dimension.mindyourmood_padding_vertical_normal);
                    break;
            }

            horizontalPadding = (int)_context.Resources.GetDimension(Resource.Dimension.mindyourmood_padding_horizontal);

            try
            {
                //Create the main viewgroup to be passed back to the ProblemSolvingReviewActivity
                problemMainLayout = new LinearLayout(_context);

                problemMainLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                problemMainLayout.Orientation = Orientation.Vertical;

                //Grab the inflater
                inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);

                //The main heading will be the problem itself (but we are going to lop off each image from the list item view
                //as it is doubling as our review item)
                View problemView = inflater.Inflate(Resource.Layout.ProblemSolvingListItem, null);

                problemView.SetPadding(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);

                ImageButton btn = problemView.FindViewById<ImageButton>(Resource.Id.imgbtnProblemSolvingListItemDetail);
                btn.Visibility = ViewStates.Gone;

                ImageView imgSteps = problemView.FindViewById<ImageView>(Resource.Id.imgStepsInProblem);
                imgSteps.Visibility = ViewStates.Gone;

                ImageView imgIdeas = problemView.FindViewById<ImageView>(Resource.Id.imgIdeasInProblem);
                imgIdeas.Visibility = ViewStates.Gone;

                ImageView imgProsAndCons = problemView.FindViewById<ImageView>(Resource.Id.imgProsAndConsInProblem);
                imgProsAndCons.Visibility = ViewStates.Gone;

                TextView txtSteps = problemView.FindViewById<TextView>(Resource.Id.txtStepsInProblem);
                txtSteps.Visibility = ViewStates.Gone;

                TextView txtIdeas = problemView.FindViewById<TextView>(Resource.Id.txtIdeasInProblem);
                txtIdeas.Visibility = ViewStates.Gone;

                TextView txtProsAndCons = problemView.FindViewById<TextView>(Resource.Id.txtProsAndConsInProblem);
                txtProsAndCons.Visibility = ViewStates.Gone;

                LinearLayout counts = problemView.FindViewById<LinearLayout>(Resource.Id.linCounts);
                counts.Visibility = ViewStates.Gone;

                TextView problemLabel = problemView.FindViewById<TextView>(Resource.Id.txtProblemSolvingListItemLabel);
                problemLabel.SetBackgroundColor(Color.Argb(255, 19, 75, 127));

                TextView probText = problemView.FindViewById<TextView>(Resource.Id.txtProblemSolvingListItemText);
                probText.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                probText.Text = _problem.ProblemText.Trim();

                problemView.SetBackgroundColor(Color.Argb(255, 19, 75, 127));
                problemMainLayout.AddView(problemView);
            }
            catch(Exception em)
            {
                Log.Error(TAG, "GetProblemDisplay: Exception - " + em.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, em, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperCreateView), "ProblemSolvingReviewHelper.GetProblemDisplay");
            }

            try
            {
                foreach (ProblemStep step in _problem.ProblemSteps)
                {
                    View stepView = inflater.Inflate(Resource.Layout.ProblemSolvingStepsListItem, null);
                    stepView.SetPadding(horizontalPadding, 0, horizontalPadding, 0);

                    stepView.SetBackgroundColor(new Color(250, 250, 221, 255));

                    ImageButton stepBtn = stepView.FindViewById<ImageButton>(Resource.Id.imgbtnProblemStepsListItemToIdeas);
                    stepBtn.Visibility = ViewStates.Invisible;

                    TextView priorityLabel = stepView.FindViewById<TextView>(Resource.Id.txtProblemStepsItemTitlePriority);
                    priorityLabel.SetTextColor(new Color(54, 93, 109, 255));
                    priorityLabel.SetBackgroundColor(new Color(250, 250, 221, 255));

                    TextView priorityText = stepView.FindViewById<TextView>(Resource.Id.txtProblemStepsItemDataPriority);
                    priorityText.SetTextColor(new Color(54, 93, 109, 255));
                    priorityText.SetBackgroundColor(new Color(250, 250, 221, 255));
                    priorityText.Text = step.PriorityOrder.ToString();

                    TextView stepLabel = stepView.FindViewById<TextView>(Resource.Id.txtProblemStepsItemTitleStep);
                    stepLabel.SetTextColor(new Color(54, 93, 109, 255));
                    stepLabel.SetBackgroundColor(new Color(250, 250, 221, 255));

                    TextView stepText = stepView.FindViewById<TextView>(Resource.Id.txtProblemStepsItemDataStep);
                    stepText.SetTextColor(new Color(54, 93, 109, 255));
                    stepText.SetBackgroundColor(new Color(250, 250, 221, 255));
                    stepText.Text = step.ProblemStep.Trim();

                    ImageButton toIdeas = stepView.FindViewById<ImageButton>(Resource.Id.imgbtnProblemStepsListItemToIdeas);
                    toIdeas.Visibility = ViewStates.Gone;

                    ImageView reviewBar = stepView.FindViewById<ImageView>(Resource.Id.imgProblemStepsItemReviewImage);
                    reviewBar.Visibility = ViewStates.Visible;
                    var newParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                    newParams.RightMargin = 16;
                    newParams.Width = 48;
                    newParams.Height = 48;
                    newParams.Gravity = GravityFlags.CenterVertical;

                    reviewBar.LayoutParameters = newParams;

                    problemMainLayout.AddView(stepView);

                    try
                    {
                        foreach (ProblemIdea idea in step.ProblemStepIdeas)
                        {
                            View ideaView = inflater.Inflate(Resource.Layout.ProblemSolvingIdeasListItem, null);
                            ideaView.SetPadding(horizontalPadding, 0, horizontalPadding, 0);

                            ImageButton ideaBtn = ideaView.FindViewById<ImageButton>(Resource.Id.imgbtnProblemSolvingIdeasToProsAndCons);
                            ideaBtn.Visibility = ViewStates.Gone;

                            TextView ideaText = ideaView.FindViewById<TextView>(Resource.Id.txtProblemSolvingIdeasText);
                            ideaText.Text = idea.ProblemIdeaText.Trim();

                            ImageView ideaBar = ideaView.FindViewById<ImageView>(Resource.Id.imgProblemSolvingIdeasReviewIndicator);
                            ideaBar.Visibility = ViewStates.Visible;

                            var ideaParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                            ideaParams.RightMargin = 16;
                            ideaParams.LeftMargin = 10;
                            ideaParams.Width = 48;
                            ideaParams.Height = 48;
                            ideaParams.Gravity = GravityFlags.CenterVertical;

                            ideaBar.LayoutParameters = newParams;

                            if (idea.ProblemIdeaID == step.ProblemStepIdeas[step.ProblemStepIdeas.Count - 1].ProblemIdeaID)
                            {
                                ideaView.SetPadding(horizontalPadding, 0, horizontalPadding, verticalPadding);
                            }

                            problemMainLayout.AddView(ideaView);

                            try
                            {
                                foreach (ProblemProAndCon proCon in idea.ProsAndCons)
                                {
                                    View proConView = inflater.Inflate(Resource.Layout.ProblemSolvingProsAndConsListItem, null);
                                    proConView.SetPadding(horizontalPadding, 0, horizontalPadding, 0);

                                    TextView typeText = proConView.FindViewById<TextView>(Resource.Id.txtProblemSolvingProsAndConsTypeText);
                                    typeText.Text = StringHelper.ProConTypeForConstant(proCon.ProblemProAndConType);

                                    TextView proConText = proConView.FindViewById<TextView>(Resource.Id.txtProblemSolvingProsAndConsText);
                                    proConText.Text = proCon.ProblemProAndConText.Trim();

                                    Space proConBar = proConView.FindViewById<Space>(Resource.Id.spcprosAndConsPlaceholder);
                                    proConBar.Visibility = ViewStates.Visible;

                                    var proConParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.MatchParent);
                                    proConParams.RightMargin = 16;
                                    proConParams.LeftMargin = 16;
                                    proConParams.Width = 48;

                                    proConBar.LayoutParameters = newParams;

                                    problemMainLayout.AddView(proConView);
                                }
                            }
                            catch (Exception ep)
                            {
                                Log.Error(TAG, "GetProblemDisplay: Exception - " + ep.Message);
                                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ep, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperCreateProConView), "ProblemSolvingReviewHelper.GetProblemDisplay");
                            }

                            if (idea.ProsAndCons.Count > 0)
                            {
                                //finally, the step review action buttons
                                View reviewAct = inflater.Inflate(Resource.Layout.ProblemSolvingReviewStepActions, null);
                                ImageButton buttonSolution = reviewAct.FindViewById<ImageButton>(Resource.Id.imgbtnReviewStepActionsSolutionPlan);
                                ImageButton buttonReviews = reviewAct.FindViewById<ImageButton>(Resource.Id.imgbtnReviewStepActionsStepReviews);
                                //tag each one with the idea ID
                                if (buttonSolution != null)
                                {
                                    buttonSolution.Tag = idea.ProblemIdeaID + ":" + idea.ProblemIdeaText.Trim();
                                    buttonSolution.Click += ButtonSolution_Click;
                                }
                                else
                                {
                                    Log.Error(TAG, "GetProblemDisplay: buttonSolution is NULL!");
                                }
                                if (buttonReviews != null)
                                {
                                    buttonReviews.Tag = idea.ProblemIdeaID;
                                    buttonReviews.Click += ButtonReviews_Click;
                                    CheckIfThisIdeaHasReview(idea.ProblemIdeaID, reviewAct);
                                }
                                else
                                {
                                    Log.Error(TAG, "GetProblemDisplay: buttonReviews is NULL!");
                                }
                                problemMainLayout.AddView(reviewAct);
                            }
                        }
                    }
                    catch (Exception ei)
                    {
                        Log.Error(TAG, "GetProblemDisplay: Exception - " + ei.Message);
                        if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ei, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperCreateIdeaview), "ProblemSolvingReviewHelper.GetProblemDisplay");
                    }
                }
            }
            catch (Exception es)
            {
                Log.Error(TAG, "GetProblemDisplay: Exception - " + es.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, es, "Creating Steps Views", "ProblemSolvingReviewHelper.GetProblemDisplay");
            }

            return problemMainLayout;
        }

        private void CheckIfThisIdeaHasReview(int problemIdeaID, View view)
        {
            Globals dbHelp = new Globals();
            try
            {
                dbHelp.OpenDatabase();
                if(dbHelp.GetSQLiteDatabase().IsOpen)
                {
                    var review = dbHelp.GetSolutionReviewForIdea(problemIdeaID);
                    if(review != null)
                    {
                        if(view != null)
                        {
                            ImageView tick = view.FindViewById<ImageView>(Resource.Id.imgChosenIdea);
                            TextView chosenText = view.FindViewById<TextView>(Resource.Id.txtChosenIdea);
                            if (tick != null)
                            {
                                tick.Visibility = ViewStates.Visible;
                            }
                            if(chosenText != null)
                            {
                                chosenText.Visibility = ViewStates.Visible;
                            }
                        }
                    }
                    dbHelp.CloseDatabase();
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "CheckIfThisIdeaHasReview: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperCheckIdea), "ProblemSolvingReviewHelper.CheckIfThisIdeaHasReview");
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
            }
        }

        private void ButtonReviews_Click(object sender, EventArgs e)
        {
            try
            {
                int ideaID = (int)((ImageButton)sender).Tag;
                Intent intent = new Intent(_activity, typeof(SolutionReviewActivity));
                intent.PutExtra("problemIdeaID", ideaID);
                _activity.StartActivity(intent);
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "ButtonReviews_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperNavReview), "ProblemSolvingReviewHelper.ButtonReviews_Click");
            }
        }

        private void ButtonSolution_Click(object sender, EventArgs e)
        {
            try
            {
                string solutionTag = (string)((ImageButton)sender).Tag;
                string[] splits = solutionTag.Split(':');

                Intent intent = new Intent(_activity, typeof(SolutionPlanActivity));
                intent.PutExtra("problemIdeaID", Convert.ToInt32(splits[0]));
                intent.PutExtra("problemIdeaText", splits[1]);
                _activity.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "ButtonSolution_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, ex, _activity.GetString(Resource.String.ErrorProblemSolvingReviewHelperNavSolution), "ProblemSolvingReviewHelper.ButtonSolution_Click");
            }
        }
    }
}