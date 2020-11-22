using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Helpers;
using Android.Util;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.ThoughtsHelp;

namespace com.spanyardie.MindYourMood
{
    [Activity]
    public class ViewThoughtsActivity : AppCompatActivity
    {
        public const string TAG = "M:ViewThoughtsActivity";

        private Toolbar _toolbar;
        private ListView _thoughtRecordList;
        private DateTime _listDate;

        private TextView _selectedDateText;

        private Button _done;

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutString("listDate", _listDate.ToString());
            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ViewThoughtsMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                {
                    _listDate = Convert.ToDateTime(savedInstanceState.GetString("listDate"));
                }
                else
                {
                    if (Intent.HasExtra("RecordDate"))
                    {
                        _listDate = Convert.ToDateTime(Intent.Extras.GetString("RecordDate"));
                    }
                    else
                    {
                        _listDate = DateTime.Now;
                    }
                }

                SetContentView(Resource.Layout.ViewThoughtRecordsLayout);

                GetFieldComponents();

                SetupCallbacks();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.viewThoughtRecordsToolbar, Resource.String.ViewThoughRecordsActionBarTitle, Color.White);

                if (_selectedDateText != null)
                    _selectedDateText.Text = _listDate.ToShortDateString();

                LoadThoughtRecords();
            }
            catch(Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorCreateViewThoughtsActivity), "ViewThoughtsActivity.OnCreate");
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                if (item.ItemId == Android.Resource.Id.Home)
                {
                    GoBack();
                    return true;
                }

                switch (item.ItemId)
                {
                    case Resource.Id.viewthoughtsSelectDate:
                        SelectDate();
                        return true;
                    case Resource.Id.viewThoughtsActionHelp:
                        Intent intent = new Intent(this, typeof(ViewThoughtsHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void GetFieldComponents()
        {
            try
            {
                _toolbar = (Toolbar)FindViewById(Resource.Id.viewThoughtRecordsToolbar);
                _thoughtRecordList = FindViewById<ListView>(Resource.Id.lstThoughtRecords);

                _selectedDateText = FindViewById<TextView>(Resource.Id.txtViewThoughtRecordsSelectedDate);

                _done = FindViewById<Button>(Resource.Id.btnDone);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorViewThoughtsGetComponents), "ViewThoughtsActivity.GetFieldComponents");
            }
        }

        private void SetupCallbacks()
        {
            if(_done != null)
                _done.Click += Done_Click;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SelectDate()
        {
            try
            {
                DatePickerFragment datePicker = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    _selectedDateText.Text = time.ToShortDateString();
                    _listDate = time;
                    LoadThoughtRecords();
                });
                Bundle bundle = new Bundle();
                bundle.PutString("defaultDate", _selectedDateText.Text.Trim());
                datePicker.Arguments = bundle;
                datePicker.Show(FragmentManager, DatePickerFragment.TAG);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "SelectDate_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorCreatingDatePicker), "ViewThoughtsActivity.SelectDate_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemDate = menu.FindItem(Resource.Id.viewthoughtsSelectDate);
                var itemHelp = menu.FindItem(Resource.Id.viewThoughtsActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemDate != null)
                            itemDate.SetIcon(Resource.Drawable.ic_date_range_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemDate != null)
                            itemDate.SetIcon(Resource.Drawable.ic_date_range_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemDate != null)
                            itemDate.SetIcon(Resource.Drawable.ic_date_range_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "ViewThoughtsActivity.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(ThoughtRecordsActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
            Finish();
        }

        private void LoadThoughtRecords()
        {
            Globals dbHelp = new Globals();

            try
            {
                dbHelp.OpenDatabase();
                dbHelp.GetAllThoughtRecordsForDate(_listDate);
                dbHelp.GetAllMoodsForAdapter();
                dbHelp.CloseDatabase();

                ViewThoughtRecordsAdapter adapter = new ViewThoughtRecordsAdapter(this);
                if (_thoughtRecordList != null)
                {
                    _thoughtRecordList.Adapter = adapter;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "LoadThoughtRecords: Exception - " + e.Message);
                if (dbHelp != null && dbHelp.GetSQLiteDatabase().IsOpen)
                    dbHelp.CloseDatabase();
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorLoadThoughtRecords), "ViewThoughtsActivity.LoadThoughtRecords");
            }
        }
    }
}