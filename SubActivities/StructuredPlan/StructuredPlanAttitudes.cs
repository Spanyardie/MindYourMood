using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using com.spanyardie.MindYourMood.Adapters;
using com.spanyardie.MindYourMood.Model;
using com.spanyardie.MindYourMood.Model.Interfaces;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Runtime;
using Android.Graphics;
using com.spanyardie.MindYourMood.SubActivities.Help.Treatment.StructuredPlan;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.SubActivities.StructuredPlan
{
    [Activity(Label = "Structured Plan Attitudes")]
    public class StructuredPlanAttitudes : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanAttitudes";

        private Toolbar _toolbar;

        private ListView _attitudesList;
        private LinearLayout _linAttitudesListMain;

        private Button _done;

        private int _selectedItemPosition = -1;

        private ImageLoader _imageLoader = null;

        public StructuredPlanAttitudes()
        {

        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemIndex", _selectedItemPosition);

            base.OnSaveInstanceState(outState); 
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AttitudesMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemPosition = savedInstanceState.GetInt("selectedItemIndex");

                SetContentView(Resource.Layout.StructuredPlanAttitudesLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.attitudesToolbar, Resource.String.StructuredPlanAttitudesActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanattitudespager,
                    new ImageLoadingListener
                    (
                        loadingComplete: (imageUri, view, loadedImage) =>
                        {
                            var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                            ImageLoader_LoadingComplete(null, args);
                        }
                    )
                );

                SetupCallbacks();

                UpdateAdapter();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudesCreate), "StructuredPlanAttitudes.OnCreate");
            }
        }
        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linAttitudesListMain != null)
                _linAttitudesListMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_attitudesList != null)
                {
                    _attitudesList.ItemClick += AttitudesList_ItemClick;
                    _attitudesList.ItemLongClick += AttitudesList_ItemLongClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudesSetCallbacks), "StructuredPlanAttitudes.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void AttitudesList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var attitudeID = GlobalData.StructuredPlanAttitudes[e.Position].AttitudesID;

            Intent intent = new Intent(this, typeof(StructuredPlanAttitudesDialogActivity));
            intent.PutExtra("attitudesID", attitudeID);
            intent.PutExtra("activityTitle", "Edit Attitude");
            StartActivityForResult(intent, ConstantsAndTypes.EDIT_ATTITUDE);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            int attitudesID = -1;
            string toWhat = "";
            ConstantsAndTypes.ATTITUDE_TYPES type = ConstantsAndTypes.ATTITUDE_TYPES.Cautious;
            int belief = 0;
            int feeling = -1;
            ConstantsAndTypes.ACTION_TYPE action = ConstantsAndTypes.ACTION_TYPE.Maintain;
            string actionOf = "";

            if(resultCode == Result.Ok)
            {
                if(data != null)
                {
                    if (data.HasExtra("attitudesID"))
                        attitudesID = data.GetIntExtra("attitudesID", -1);
                    if (data.HasExtra("toWhat"))
                        toWhat = data.GetStringExtra("toWhat");
                    if (data.HasExtra("type"))
                        type = (ConstantsAndTypes.ATTITUDE_TYPES)data.GetIntExtra("type", 0);
                    if (data.HasExtra("belief"))
                        belief = data.GetIntExtra("belief", 0);
                    if (data.HasExtra("feeling"))
                        feeling = data.GetIntExtra("feeling", -1);
                    if (data.HasExtra("action"))
                        action = (ConstantsAndTypes.ACTION_TYPE)data.GetIntExtra("action", -1);
                    if (data.HasExtra("actionOf"))
                        actionOf = data.GetStringExtra("actionOf");
                    try
                    {
                        Attitudes attitudesItem = null;
                        if (attitudesID == -1)
                        {
                            //new item
                            attitudesItem = new Attitudes();
                            attitudesItem.IsNew = true;
                            attitudesItem.IsDirty = false;
                        }
                        else
                        {
                            attitudesItem = GlobalData.StructuredPlanAttitudes.Find(attitude => attitude.AttitudesID == attitudesID);
                            attitudesItem.IsNew = false;
                            attitudesItem.IsDirty = true;
                        }

                        attitudesItem.ToWhat = toWhat;
                        attitudesItem.TypeOf = type;
                        attitudesItem.Belief = belief;
                        attitudesItem.Feeling = feeling;
                        attitudesItem.Action = action;
                        attitudesItem.ActionOf = actionOf;
                        attitudesItem.Save();

                        if (attitudesID == -1)
                            GlobalData.StructuredPlanAttitudes.Add(attitudesItem);

                        UpdateAdapter();
                    }
                    catch (Exception e)
                    {
                        Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                        if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudesConfirm), "StructuredPlanAttitudes.OnActivityResult");
                    }
                }
            }

            if(resultCode == Result.Canceled)
                Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }

        private void Remove()
        {
            if (_selectedItemPosition != -1)
            {
                try
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.StructuredPlanAttitudesRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.StructuredPlanAttitudesRemoveAlertQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "Remove: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanAttitudesRemove), "StructuredPlanAttitudes.Remove");
                }
            }
            else
            {
                string warning = "";
                if (GlobalData.StructuredPlanAttitudes.Count == 0)
                {
                    warning = GetString(Resource.String.StructuredPlanAttitudesWarning1);
                }
                else
                {
                    warning = GetString(Resource.String.StructuredPlanAttitudesWarning2);
                }
                Toast.MakeText(this, warning, ToastLength.Short).Show();
            }
        }

        private void AttitudesList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemPosition = e.Position;
            Log.Info(TAG, "AttitudesList_ItemClick: Selected item at position " + _selectedItemPosition.ToString());
            UpdateAdapter();
            _attitudesList.SetSelection(_selectedItemPosition);
        }

        private void Add()
        {
            try
            {
                if (_attitudesList != null)
                {
                    Intent intent = new Intent(this, typeof(StructuredPlanAttitudesDialogActivity));
                    intent
                        .PutExtra("attitudesID", -1)
                        .PutExtra("activityTitle", "Add Attitude");

                    StartActivityForResult(intent, ConstantsAndTypes.ADD_ATTITUDE);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanAttitudesAdd), "StructuredPlanAttitudes.Add");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.attitudesActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.attitudesActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.attitudesActionHelp);

                switch (screenSize)
                {
                    case ConstantsAndTypes.ScreenSize.Normal:
                        if(itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_24dp);
                        if(itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_24dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_24dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.Large:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_36dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_36dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_36dp);
                        break;
                    case ConstantsAndTypes.ScreenSize.ExtraLarge:
                        if (itemAdd != null)
                            itemAdd.SetIcon(Resource.Drawable.ic_add_circle_outline_white_48dp);
                        if (itemRemove != null)
                            itemRemove.SetIcon(Resource.Drawable.ic_delete_white_48dp);
                        if (itemHelp != null)
                            itemHelp.SetIcon(Resource.Drawable.ic_help_white_48dp);
                        break;
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetActionIcons: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanAttitudes.SetActionIcons");
            }
        }

        private void GoBack()
        {
            Intent intent = new Intent(this, typeof(StructuredPlanActivity));
            Android.Support.V4.App.NavUtils.NavigateUpTo(this, intent);
        }

        private void GetFieldComponents()
        {
            try
            {
                _attitudesList = FindViewById<ListView>(Resource.Id.lstStructuredPlanAttitudes);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linAttitudesListMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanAttitudesList);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudesGetComponents), "StructuredPlanAttitudes.GetFieldComponents");
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
                    case Resource.Id.attitudesActionAdd:
                        Add();
                        return true;

                    case Resource.Id.attitudesActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.attitudesActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanAttitudesHelpActivity));
                        StartActivity(intent);
                        return true;
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        private void UpdateAdapter()
        {
            try
            {
                StructuredPlanAttitudesListAdapter adapter = new StructuredPlanAttitudesListAdapter(this);
                if (_attitudesList != null)
                    _attitudesList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanAttitudesUpdateAdapter), "StructuredPlanAttitudes.UpdateAdapter");
            }
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemPosition;
        }

        public void AlertPositiveButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            try
            {
                var attitudesItem = GlobalData.StructuredPlanAttitudes[_selectedItemPosition];
                attitudesItem.Remove();
                GlobalData.StructuredPlanAttitudes.Remove(attitudesItem);
                UpdateAdapter();
                _selectedItemPosition = -1;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Attitude", "StructuredPlanAttitudes.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }
    }
}