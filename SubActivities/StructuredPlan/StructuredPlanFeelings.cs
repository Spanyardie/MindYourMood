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
using V7Sup = Android.Support.V7.App;
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
    [Activity(Label = "Structured Plan Feelings")]
    public class StructuredPlanFeelings : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanFeelings";

        private Toolbar _toolbar;

        private ListView _feelingsList;

        private Button _done;
        private LinearLayout _linStructuredPlanFeelingsList;

        private int _selectedItemPosition = -1;

        private ImageLoader _imageLoader = null;

        public StructuredPlanFeelings()
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
            MenuInflater.Inflate(Resource.Menu.FeelingsMenu, menu);

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

                SetContentView(Resource.Layout.StructuredPlanFeelingsLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.feelingsToolbar, Resource.String.StructuredPlanFeelingsActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanfeelingspager,
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsCreate), "StructuredPlanFeelings.OnCreate");
            }
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanFeelingsList != null)
                _linStructuredPlanFeelingsList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            int feelingsID = -1;
            string about = "";
            int strength = -1;
            ConstantsAndTypes.REACTION_TYPE reaction = ConstantsAndTypes.REACTION_TYPE.Positive;
            ConstantsAndTypes.ACTION_TYPE intention = ConstantsAndTypes.ACTION_TYPE.Maintain;
            string actionOf = "";

            base.OnActivityResult(requestCode, resultCode, data);

            if(resultCode == Result.Ok)
            {
                try
                {
                    if (data.HasExtra("feelingsID"))
                        feelingsID = data.GetIntExtra("feelingsID", -1);
                    if (data.HasExtra("about"))
                        about = data.GetStringExtra("about");
                    if (data.HasExtra("strength"))
                        strength = data.GetIntExtra("strength", -1);
                    if (data.HasExtra("reaction"))
                        reaction = (ConstantsAndTypes.REACTION_TYPE)data.GetIntExtra("reaction", -1);
                    if (data.HasExtra("intention"))
                        intention = (ConstantsAndTypes.ACTION_TYPE)data.GetIntExtra("intention", -1);
                    if (data.HasExtra("actionOf"))
                        actionOf = data.GetStringExtra("actionOf");

                    Feelings feeling = null;
                    if (feelingsID == -1)
                    {
                        //new item
                        feeling = new Feelings();
                        feeling.IsNew = true;
                        feeling.IsDirty = false;
                    }
                    else
                    {
                        feeling = GlobalData.StructuredPlanFeelings.Find(feel => feel.FeelingsID == feelingsID);
                        feeling.IsNew = false;
                        feeling.IsDirty = true;
                    }

                    feeling.AboutWhat = about;
                    feeling.Action = intention;
                    feeling.ActionOf = actionOf;
                    feeling.Strength = strength;
                    feeling.Type = reaction;
                    feeling.Save();

                    if (feelingsID == -1)
                        GlobalData.StructuredPlanFeelings.Add(feeling);

                    UpdateAdapter();
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "ConfirmPlanItemAddition: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsConfirm), "StructuredPlanFeelings.ConfirmPlanItemAddition");
                }
            }

            if (resultCode == Result.Canceled)
            {
                Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
            }
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_feelingsList != null)
                {
                    _feelingsList.ItemClick += FeelingsList_ItemClick;
                    _feelingsList.ItemLongClick += FeelingsList_ItemLongClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch(Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsSetCallbacks), "StructuredPlanFeelings.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void FeelingsList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var feelingID = GlobalData.StructuredPlanFeelings[e.Position].FeelingsID;

            Intent intent = new Intent(this, typeof(StructuredPlanFeelingsDialogActivity));
            intent
                .PutExtra("feelingsID", feelingID)
                .PutExtra("activityTitle", "Edit Feeling");

            StartActivityForResult(intent, ConstantsAndTypes.EDIT_FEELING);
        }

        private void Remove()
        {
            if(_selectedItemPosition != -1)
            {
                try
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.StructuredPlanFeelingsRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.StructuredPlanFeelingsRemoveAlertQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                catch (Exception ex)
                { 
                    Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanFeelingsRemove), "StructuredPlanFeelings.Remove_Click");
                }
            }
            else
            {
                string warning = "";
                if (GlobalData.StructuredPlanFeelings.Count == 0)
                {
                    warning = GetString(Resource.String.StructuredPlanFeelingsWarning1);
                }
                else
                {
                    warning = GetString(Resource.String.StructuredPlanFeelingsWarning2);
                }
                Toast.MakeText(this, warning, ToastLength.Short).Show();
            }
        }

        private void FeelingsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemPosition = e.Position;
            Log.Info(TAG, "FeelingsList_ItemClick: Selected item at position " + _selectedItemPosition.ToString());
            UpdateAdapter();
            _feelingsList.SetSelection(_selectedItemPosition);
        }

        private void Add()
        {
            try
            {
                if (_feelingsList != null)
                {
                    Intent intent = new Intent(this, typeof(StructuredPlanFeelingsDialogActivity));
                    intent
                        .PutExtra("feelingsID", -1)
                        .PutExtra("activityTitle", "Add Feeling");

                    StartActivityForResult(intent, ConstantsAndTypes.ADD_FEELING);
                }
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanFeelingsAdd), "StructuredPlanFeelings.Add_Click");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.feelingsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.feelingsActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.feelingsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanFeelings.SetActionIcons");
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
                _feelingsList = FindViewById<ListView>(Resource.Id.lstStructuredPlanFeelings);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanFeelingsList = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanFeelingsList);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsGetComponents), "StructuredPlanFeelings.GetFieldComponents");
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
                    case Resource.Id.feelingsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.feelingsActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.feelingsActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanFeelingsHelpActivity));
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
                StructuredPlanFeelingsListAdapter adapter = new StructuredPlanFeelingsListAdapter(this);
                if (_feelingsList != null)
                    _feelingsList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFeelingsUpdateAdapter), "StructuredPlanFeelings.UpdateAdapter");
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
                var feeling = GlobalData.StructuredPlanFeelings[_selectedItemPosition];
                feeling.Remove();
                GlobalData.StructuredPlanFeelings.Remove(feeling);
                UpdateAdapter();
                _selectedItemPosition = -1;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Feeling", "StructuredPlanFeelings.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }
    }
}