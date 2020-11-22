using System;
using System.Collections.Generic;
using MikePhil.Charting.Data;
using Android.Util;
using MikePhil.Charting.Components;
using Android.Widget;
using MikePhil.Charting.Charts;

using Android.Views;
using MikePhil.Charting.Interfaces.Datasets;
using Android.App;
using Android.Graphics;

namespace com.spanyardie.MindYourMood.Helpers
{
    public class ProgressChartHelper
    {
        private struct MoodDataPoint
        {
            public float MoodRating;
            public string RecordDate;
        }
        private List<MoodDataPoint> _moodDataPoints = new List<MoodDataPoint>();
        private LinearLayout _progressLineChartContainer = null;
        private LineChart _lineChart = null;
        private Activity _activity = null;

        public const string TAG = "M:ProgressChartHelper";

        public ProgressChartHelper(Activity activity, LinearLayout container, LineChart lineChart)
        {
            _activity = activity;
            _progressLineChartContainer = container;
            _lineChart = lineChart;
        }

        public LinearLayout SetupLineChart()
        {
            try
            {
                if (_progressLineChartContainer != null)
                {
                    _lineChart = new LineChart(_activity);
                    TableLayout.LayoutParams layoutParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                    _lineChart.LayoutParameters = layoutParams;
                    _progressLineChartContainer.AddView(_lineChart);
                    Log.Info(TAG, "SetupLineChart: Success adding chart to container _progressLineChartContainer");
                }
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupLineChart: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting up the Line chart", "ProgressChartHelper.SetupLineChart");
            }

            return _progressLineChartContainer;
        }

        public LineChart SetupChartData(DateTime startDate, DateTime endDate)
        {
            //when first entered into this activity the default is the past weeks data, so we need to get that
            Globals dbHelp = null;
            try
            {
                if (_lineChart != null)
                {
                    Log.Info(TAG, "SetupChartData: Found _progressChart");
                    DateTime loopDate = startDate;
                    //this next array will contain the string labels for the X axis, which is the date of the point
                    List<string> pointsDates = new List<string>();

                    if (GlobalData.MoodListItems != null)
                    {
                        dbHelp = new Globals();
                        dbHelp.OpenDatabase();
                        LineDataSet moodDataSet = null;
                        List<ILineDataSet> moodLineDataSets = new List<ILineDataSet>();
                        var daysDifference = endDate.Subtract(startDate).Days;
                        Log.Info(TAG, "SetupChartData: Attempting to get " + daysDifference.ToString() + " days data from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString());
                        for (var a = 0; a <= daysDifference; a++)
                        {
                            pointsDates.Add(loopDate.ToShortDateString());
                            Log.Info(TAG, "SetupChartData: Added '" + loopDate.ToShortDateString() + " to date list");
                            loopDate = loopDate.AddDays(1);
                        }

                        Log.Info(TAG, "SetupChartData: MoodListItems global list contains " + GlobalData.MoodListItems.Count.ToString() + " items");
                        foreach (var item in GlobalData.MoodListItems)
                        {
                            Log.Info(TAG, "SetupChartData: Attempting to retrieve data points for " + item.MoodName.Trim());
                            moodDataSet = GetDataPoints(item.MoodId, startDate, endDate, item.MoodName);
                            if (moodDataSet != null)
                            {
                                int[] colorList = new int[1] { GlobalData.ColourList[item.MoodId - 1] };
                                moodDataSet.SetColors(colorList);
                                moodLineDataSets.Add(moodDataSet);
                                Log.Info(TAG, "SetupChartData: Added " + item.MoodName + " to data sets list");
                            }
                            else
                            {
                                Log.Info(TAG, "SetupChartData: No data set returned for " + item.MoodName + "!");
                            }
                        }
                        LineData moodLineData = new LineData(pointsDates, moodLineDataSets);
                        Log.Info(TAG, "SetupChartData: Created LineData containing date list and mood data sets");
                        _lineChart.Data = moodLineData;
                        _lineChart.Legend.WordWrapEnabled = true;
                        _lineChart.SetDescriptionColor(Color.White);
                        _lineChart.SetDescription(_activity.GetString(Resource.String.MyProgressGraphDescription) + " " + startDate.ToShortDateString() + " - " + endDate.ToShortDateString());
                        _lineChart.XAxis.TextColor = Color.White;
                        _lineChart.XAxis.AxisLineColor = Color.White;
                        _lineChart.XAxis.GridColor = Color.White;
                        _lineChart.AxisLeft.AxisLineColor = Color.White;
                        _lineChart.AxisRight.AxisLineColor = Color.White;
                        _lineChart.AxisLeft.GridColor = Color.White;
                        _lineChart.AxisRight.GridColor = Color.White;
                        _lineChart.AxisLeft.TextColor = Color.White;
                        
                        _lineChart.AxisRight.TextColor = Color.White;
                        _lineChart.AxisLeft.TextSize = 10f;
                        _lineChart.AxisRight.TextSize = 10f;
                        var yaxis1 = _lineChart.GetAxis(YAxis.AxisDependency.Left);
                        yaxis1.AxisLineColor = Color.White;
                        yaxis1.GridColor = Color.White;
                        yaxis1.TextColor = Color.White;
                        yaxis1.TextSize = 10f;
                        var yaxis2 = _lineChart.GetAxis(YAxis.AxisDependency.Right);
                        yaxis2.AxisLineColor = Color.White;
                        yaxis2.GridColor = Color.White;
                        yaxis2.TextColor = Color.White;
                        yaxis1.TextSize = 10f;
                        _lineChart.SetBorderColor(Color.White);
                        moodLineData.SetValueTextColor(Color.White);
                        moodLineData.SetValueTextSize(10f);
                        
                        _lineChart.Invalidate();
                        dbHelp.CloseDatabase();
                        Log.Info(TAG, "SetupChartData: Finished retrieving chart data");
                    }
                }
            }
            catch (Exception e)
            {
                if(dbHelp != null)
                {
                    if (dbHelp.GetSQLiteDatabase().IsOpen)
                        dbHelp.CloseDatabase();
                }
                Log.Error(TAG, "SetupChartData: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Setting data for chart", "ProgressChartHelper.SetupChartData");
            }

            return _lineChart;
        }

        public LineDataSet GetDataPoints(int moodListID, DateTime startDate, DateTime endDate, string moodName)
        {
            //data points for a particular Mood
            List<Entry> moodPoints = new List<Entry>();
            DateTime previousDate = new DateTime();
            float runningPointTotal = 0;
            int sameDayEntryCount = 0;
            DateTime currentPassDate = new DateTime();
            int theXIndex = 0;

            Globals dbHelp = new Globals();
            dbHelp.OpenDatabase();
            var sqlDatabase = dbHelp.GetSQLiteDatabase();
            if(sqlDatabase == null)
            {
                Log.Error(TAG, "GetDataPoints: Uable to retrieve connection to database");
                return null;
            }

            try
            {
                var sql = "SELECT [MoodRating], [RecordDate] FROM vwCompleteMoodsRating WHERE ([RecordDate] BETWEEN '" + string.Format("{0:yyyy-MM-dd}", startDate) + " 00:00:00' AND '" + string.Format("{0:yyyy-MM-dd}", endDate) + " 23:59:59') AND ([MoodListID] = " + moodListID + ") ORDER BY [RecordDate];";

                if (sqlDatabase != null && sqlDatabase.IsOpen)
                {
                    var moodDataPoints = sqlDatabase.RawQuery(sql, null);
                    if (moodDataPoints.Count > 0)
                    {
                        PadMissingEntriesForMoodInDateRange(startDate, endDate, moodDataPoints);
                        //If there is more than 1 data point for this mood for a particular date, then we need to average it
                        //rather than add more than 1 data point (this will cause an exception otherwise)
                        previousDate = Convert.ToDateTime(_moodDataPoints[0].RecordDate);
                        foreach(var a in _moodDataPoints)
                        {
                            try
                            {
                                Log.Info(TAG, "GetDataPoints: Previous date on iteration " + a.ToString() + " is " + previousDate.ToShortDateString());
                                currentPassDate = Convert.ToDateTime(a.RecordDate);
                                Log.Info(TAG, "GetDataPoints: Current pass date is " + currentPassDate.ToShortDateString());
                                if (previousDate != currentPassDate)
                                {
                                    //unequal means that we can save a point
                                    Log.Info(TAG, "GetDataPoints: Dates don't match - creating new data point from Running Total " + runningPointTotal.ToString() + ", number of entries for this date - " + sameDayEntryCount.ToString());
                                    var theYPoint = runningPointTotal / sameDayEntryCount;
                                    if (theYPoint != 0)
                                    {
                                        Entry thePoint = new Entry(theYPoint, theXIndex);
                                        moodPoints.Add(thePoint);
                                    }
                                    Log.Info(TAG, "GetDataPoints: Added data point " + theYPoint.ToString() + " for date " + previousDate.ToShortDateString());
                                    runningPointTotal = a.MoodRating;
                                    sameDayEntryCount = 1;
                                    theXIndex++;
                                    Log.Info(TAG, "GetDataPoints: X index is now " + theXIndex.ToString());
                                }
                                else
                                {
                                    //current Pass date and previous date are equal, meaning there is more than 1 data point
                                    //we will store these points and take an average before we finalise the value for an entry

                                    //Note: Since we are using the '1' value as a padding value, if there is padding required AFTER the date
                                    //of a data point, they act as additional entries for the date last processed with a value.
                                    //The upshot of this is explained as follows:
                                    //Say we have a last data point for the date range of 29
                                    //The following day does not have an entry, so a 'padding' entry of '1' is created
                                    //This is then added erroneously to the current data point, giving 2 data points with total of 29 + 1 = 30
                                    //This is then averaged by dividing the total by the number of entries, giving 30 / 2 = 15 - INCORRECT
                                    //We will therefore ignore values of '1' as padding until a fix can be found
                                    var grabPoint = a.MoodRating;
                                    //This conditional is the workaround for the padding bug
                                    if (grabPoint > 0)
                                    {
                                        runningPointTotal += grabPoint;
                                        sameDayEntryCount++;
                                        Log.Info(TAG, "GetDataPoints: Current date same as previous date - storing Running total of " + runningPointTotal.ToString() + ", number of entries for this date - " + sameDayEntryCount.ToString());
                                    }
                                }
                            }
                            finally
                            {
                                previousDate = currentPassDate;
                                Log.Info(TAG, "GetDataPoints: Previous date set to " + previousDate.ToShortDateString());
                            }
                        }
                        //there will always be one data point left over to add
                        var theLastYPoint = runningPointTotal / sameDayEntryCount;
                        if (theLastYPoint != 0)
                        {
                            Entry thePoint = new Entry(theLastYPoint, theXIndex);
                            moodPoints.Add(thePoint);
                        }
                        Log.Info(TAG, "GetDataPoints: Added final point of " + theLastYPoint.ToString() + " at x index of " + theXIndex.ToString());

                        //now create the line dataset object to hold our points
                        LineDataSet moodDataSet = new LineDataSet(moodPoints, moodName);
                        //and the axis to plot against
                        moodDataSet.AxisDependency = YAxis.AxisDependency.Left;

                        Log.Info(TAG, "GetDataPoints: Returning " + moodPoints.Count.ToString() + " data points");
                        if (sqlDatabase.IsOpen)
                            sqlDatabase.Close();
                        return moodDataSet;
                    }
                }
                if (sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                return null;
            }
            catch (Exception e)
            {
                if (sqlDatabase != null && sqlDatabase.IsOpen)
                    sqlDatabase.Close();
                Log.Error(TAG, "GetDataPoints: Exception - " + e.Message);
                throw new Exception("Unable to get Data Point for Chart - " + e.Message, e);
            }
        }

        // There is an issue with the chart component whereby if there are missing entries for a particular date
        // instead of making the assumption it is zero, it will start the line draw from date-x where x is the
        // number of days missing from the range.  This is totally no good, as it skews the line chart and
        // doesn't then reflect the true dates for a particular mood. Additionally, it will ignore '0' entries
        // This method, then, will 'pad' out the missing days with '1' entries for the rating
        private void PadMissingEntriesForMoodInDateRange(DateTime fromDate, DateTime toDate, Android.Database.ICursor databasePoints)
        {
            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: FromDate - " + fromDate.ToShortDateString() + ", ToDate - " + toDate.ToShortDateString());

            try
            {
                if (_moodDataPoints == null)
                    _moodDataPoints = new List<MoodDataPoint>();
                _moodDataPoints.Clear();

                if (databasePoints != null && databasePoints.Count > 0)
                {
                    Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: DataPoints count is " + databasePoints.Count.ToString());

                    var theDate = Convert.ToDateTime( fromDate.ToShortDateString() + " 00:00:00" );
                    
                    var previousDate = theDate;
                    Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: theDate - " + theDate.ToShortDateString() + ", previousDate - " + previousDate.ToShortDateString());

                    databasePoints.MoveToFirst();
                    for (var a = 0; a < databasePoints.Count; a++)
                    {
                        //if theDate is less than the current pass date, add a zero entry
                        int recordDateIndex = databasePoints.GetColumnIndex("[RecordDate]");
                        if(recordDateIndex == -1)
                        {
                            //try without square brackets - fucking Android!!
                            recordDateIndex = databasePoints.GetColumnIndex("RecordDate");
                        }
                        string recordDate = databasePoints.GetString(recordDateIndex);
                        var pointDate = Convert.ToDateTime(recordDate);
                        Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Iteration " + a.ToString() + ", pointDate - " + pointDate.ToShortDateString());
                        var thePreviousDateDiff = previousDate.Subtract(pointDate).Days;
                        Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: thePreviousDateDiff - " + thePreviousDateDiff.ToString());
                        if (thePreviousDateDiff < -1)
                        {
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Padding required, performing...");
                            do
                            {
                                var newZeroPoint = new MoodDataPoint();
                                newZeroPoint.RecordDate = theDate.ToShortDateString();
                                newZeroPoint.MoodRating = 0.001f;
                                _moodDataPoints.Add(newZeroPoint);
                                Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Added padding for date - " + theDate.ToShortDateString());
                                theDate = theDate.AddDays(1);
                                Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Added 1 to theDate, now " + theDate.ToShortDateString());
                            }
                            while (theDate.Subtract(pointDate).Days < 0);
                            MoodDataPoint newDataPoint = new MoodDataPoint();
                            newDataPoint.RecordDate = pointDate.ToShortDateString();
                            int moodRatingIndex = databasePoints.GetColumnIndex("[MoodRating]");
                            if(moodRatingIndex == -1)
                            {
                                //gaaahh!!
                                moodRatingIndex = databasePoints.GetColumnIndex("MoodRating");
                            }
                            newDataPoint.MoodRating = databasePoints.GetFloat(moodRatingIndex);
                            _moodDataPoints.Add(newDataPoint);
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Added point from data for date - " + pointDate.ToShortDateString());
                            previousDate = pointDate; // theDate;
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: PreviousDate set to - " + previousDate.ToShortDateString());
                            databasePoints.MoveToNext();
                        }
                        else
                        {
                            MoodDataPoint newDataPoint = new MoodDataPoint();
                            newDataPoint.RecordDate = pointDate.ToShortDateString();
                            int moodRatingIndex = databasePoints.GetColumnIndex("[MoodRating]");
                            if (moodRatingIndex == -1)
                            {
                                //gaaahh!!
                                moodRatingIndex = databasePoints.GetColumnIndex("MoodRating");
                            }
                            newDataPoint.MoodRating = databasePoints.GetFloat(moodRatingIndex);
                            _moodDataPoints.Add(newDataPoint);
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Added point from data for date  - " + pointDate.ToShortDateString());
                            previousDate = Convert.ToDateTime(newDataPoint.RecordDate);
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: PreviousDate set to - " + previousDate.ToShortDateString());
                            if (thePreviousDateDiff == -1)
                            {
                                theDate = theDate.AddDays(1);
                                Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: One day difference, adding 1 to theDate, now - " + theDate.ToShortDateString());
                            }
                            databasePoints.MoveToNext();
                        }
                    }
                    //are there any left over for padding?
                    if (toDate.Subtract(previousDate).Days > 0)
                    {
                        theDate = previousDate.AddDays(1);
                        for (var a = 0; a < toDate.Subtract(previousDate).Days; a++)
                        {
                            var newZeroPoint = new MoodDataPoint();
                            newZeroPoint.RecordDate = theDate.ToShortDateString();
                            newZeroPoint.MoodRating = 0.001f;
                            _moodDataPoints.Add(newZeroPoint);
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Added left over padding for date - " + theDate.ToShortDateString());
                            theDate = theDate.AddDays(1);
                            Log.Info(TAG, "PadMissingEntriesForMoodInDateRange: Added 1 to theDate, now " + theDate.ToShortDateString());
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "PadMissingEntriesForMoodInDateRange: Exception - " + e.Message);
                throw new Exception("Error occurred padding missing dates in chart point data", e);
            }
        }
    }
}