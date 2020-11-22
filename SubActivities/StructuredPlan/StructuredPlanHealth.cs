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
    [Activity(Label = "Structured Plan Health")]
    public class StructuredPlanHealth : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanHealth";

        private Toolbar _toolbar;

        private ListView _healthList;
        private LinearLayout _linStructuredPlanHealthList;

        private Button _done;

        private int _selectedItemPosition = -1;

        private ImageLoader _imageLoader = null;

        public StructuredPlanHealth()
        {

        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (outState != null)
                outState.PutInt("selectedItemPosition", _selectedItemPosition);

            base.OnSaveInstanceState(outState);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.HealthMenu, menu);

            SetActionIcons(menu);

            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                if (savedInstanceState != null)
                    _selectedItemPosition = savedInstanceState.GetInt("selectedItemPosition");

                SetContentView(Resource.Layout.StructuredPlanHealthLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.healthToolbar, Resource.String.StructuredPlanHealthActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanhealthpager,
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthCreate), "StructuredPlanHealth.OnCreate");
            }
        }
        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanHealthList != null)
                _linStructuredPlanHealthList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_healthList != null)
                {
                    _healthList.ItemClick += HealthList_ItemClick;
                    _healthList.ItemLongClick += HealthList_ItemLongClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthSetCallbacks), "StructuredPlanHealth.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void HealthList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var healthID = GlobalData.StructuredPlanHealth[e.Position].HealthID;

            Intent intent = new Intent(this, typeof(StructuredPlanHealthDialogActivity));
            intent
                .PutExtra("healthID", healthID)
                .PutExtra("activityTitle", "Edit Health item");

            StartActivityForResult(intent, ConstantsAndTypes.EDIT_HEALTH);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            int healthID = -1;
            string aspect = "";
            int importance = -1;
            ConstantsAndTypes.REACTION_TYPE reaction = ConstantsAndTypes.REACTION_TYPE.Positive;
            ConstantsAndTypes.ACTION_TYPE intention = ConstantsAndTypes.ACTION_TYPE.Maintain;
            string actionOf = "";

            base.OnActivityResult(requestCode, resultCode, data);

            if(resultCode == Result.Ok)
            {
                try
                {
                    if (data.HasExtra("healthID"))
                        healthID = data.GetIntExtra("healthID", -1);
                    if (data.HasExtra("aspect"))
                        aspect = data.GetStringExtra("aspect");
                    if (data.HasExtra("importance"))
                        importance = data.GetIntExtra("importance", -1);
                    if (data.HasExtra("reaction"))
                        reaction = (ConstantsAndTypes.REACTION_TYPE)data.GetIntExtra("reaction", -1);
                    if (data.HasExtra("intention"))
                        intention = (ConstantsAndTypes.ACTION_TYPE)data.GetIntExtra("intention", -1);
                    if (data.HasExtra("actionOf"))
                        actionOf = data.GetStringExtra("actionOf");

                    Health healthItem = null;
                    if (healthID == -1)
                    {
                        //new item
                        healthItem = new Health();
                        healthItem.IsNew = true;
                        healthItem.IsDirty = false;
                    }
                    else
                    {
                        healthItem = GlobalData.StructuredPlanHealth.Find(healthy => healthy.HealthID == healthID);
                        healthItem.IsNew = false;
                        healthItem.IsDirty = true;
                    }

                    healthItem.Aspect = aspect;
                    healthItem.Action = intention;
                    healthItem.ActionOf = actionOf;
                    healthItem.Importance = importance;
                    healthItem.Type = reaction;
                    healthItem.Save();

                    if (healthID == -1)
                        GlobalData.StructuredPlanHealth.Add(healthItem);

                    UpdateAdapter();
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthConfirm), "StructuredPlanHealth.OnActivityResult");
                }
            }

            if (resultCode == Result.Canceled)
            {
                Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
            }
        }

        private void Remove()
        {
            if (_selectedItemPosition != -1)
            {
                try
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.StructuredPlanHealthRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.StructuredPlanHealthRemoveAlertQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanHealthRemove), "StructuredPlanHealth.Remove_Click");
                }
            }
            else
            {
                string warning = "";
                if (GlobalData.StructuredPlanHealth.Count == 0)
                {
                    warning = GetString(Resource.String.StructuredPlanHealthWarning1);
                }
                else
                {
                    warning = GetString(Resource.String.StructuredPlanHealthWarning2);
                }
                Toast.MakeText(this, warning, ToastLength.Short).Show();
            }
        }

        private void HealthList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemPosition = e.Position;
            Log.Info(TAG, "HealthList_ItemClick: Selected item at position " + _selectedItemPosition.ToString());
            UpdateAdapter();
            _healthList.SetSelection(_selectedItemPosition);
        }

        private void Add()
        {
            try
            {
                if (_healthList != null)
                {
                    Intent intent = new Intent(this, typeof(StructuredPlanHealthDialogActivity));
                    intent
                        .PutExtra("healthID", -1)
                        .PutExtra("activityTitle", "Add Health item");

                    StartActivityForResult(intent, ConstantsAndTypes.ADD_HEALTH);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanHealthAdd), "StructuredPlanHealth.Add");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.healthActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.healthActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.healthActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanHealth.SetActionIcons");
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
                _healthList = FindViewById<ListView>(Resource.Id.lstStructuredPlanHealth);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanHealthList = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanHealthList);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthGetComponents), "StructuredPlanHealth.GetFieldComponents");
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
                    case Resource.Id.healthActionAdd:
                        Add();
                        return true;

                    case Resource.Id.healthActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.healthActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanHealthHelpActivity));
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
                StructuredPlanHealthListAdapter adapter = new StructuredPlanHealthListAdapter(this);
                if (_healthList != null)
                    _healthList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanHealthUpdateAdapter), "StructuredPlanHealth.UpdateAdapter");
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
                var healthItem = GlobalData.StructuredPlanHealth[_selectedItemPosition];
                healthItem.Remove();
                GlobalData.StructuredPlanHealth.Remove(healthItem);
                UpdateAdapter();
                _selectedItemPosition = -1;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Health", "StructuredPlanHealth.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }
    }
}