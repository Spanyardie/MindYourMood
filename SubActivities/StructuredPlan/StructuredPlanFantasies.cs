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
    [Activity(Label = "Structured Plan Fantasies")]
    public class StructuredPlanFantasies : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanFantasies";

        private Toolbar _toolbar;

        private ListView _fantasiesList;
        private LinearLayout _linStructuredPlanFantasiesListMain;

        private Button _done;

        private int _selectedItemPosition = -1;

        private ImageLoader _imageLoader = null;

        public StructuredPlanFantasies()
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
            MenuInflater.Inflate(Resource.Menu.FantasiesMenu, menu);

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

                SetContentView(Resource.Layout.StructuredPlanFantasiesLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.fantasiesToolbar, Resource.String.StructuredPlanFantasiesActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanfantasiespager,
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesCreate), "StructuredPlanFantasies.OnCreate");
            }
        }
        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanFantasiesListMain != null)
                _linStructuredPlanFantasiesListMain.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_fantasiesList != null)
                {
                    _fantasiesList.ItemClick += FantasiesList_ItemClick;
                    _fantasiesList.ItemLongClick += FantasiesList_ItemLongClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesSetCallbacks), "StructuredPlanFantasies.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void FantasiesList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var fantasyID = GlobalData.StructuredPlanFantasies[e.Position].FantasiesID;

            Intent intent = new Intent(this, typeof(StructuredPlanFantasiesDialogActivity));
            intent
                .PutExtra("fantasiesID", fantasyID)
                .PutExtra("activityTitle", "Edit Fantasy");

            StartActivityForResult(intent, ConstantsAndTypes.EDIT_FANTASY);
        }

        private void Remove()
        {
            if (_selectedItemPosition != -1)
            {
                try
                {
                    AlertHelper alertHelper = new AlertHelper(this);
                    alertHelper.AlertTitle = GetString(Resource.String.StructuredPlanFantasiesRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.StructuredPlanFantasiesRemoveAlertQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "Remove: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanFantasiesRemove), "StructuredPlanFantasies.Remove");
                }
            }
            else
            {
                string warning = "";
                if (GlobalData.StructuredPlanFantasies.Count == 0)
                {
                    warning = GetString(Resource.String.StructuredPlanFantasiesWarning1);
                }
                else
                {
                    warning = GetString(Resource.String.StructuredPlanFantasiesWarning2);
                }
                Toast.MakeText(this, warning, ToastLength.Short).Show();
            }
        }

        private void FantasiesList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemPosition = e.Position;
            Log.Info(TAG, "FantasiesList_ItemClick: Selected item at position " + _selectedItemPosition.ToString());
            UpdateAdapter();
            _fantasiesList.SetSelection(_selectedItemPosition);
        }

        private void Add()
        {
            try
            {
                if (_fantasiesList != null)
                {
                    Intent intent = new Intent(this, typeof(StructuredPlanFantasiesDialogActivity));
                    intent
                        .PutExtra("fantasiesID", -1)
                        .PutExtra("activityTitle", "Add Fantasy");
                    StartActivityForResult(intent, ConstantsAndTypes.ADD_FANTASY);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add_Click: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanFantasiesAdd), "StructuredPlanFantasies.Add_Click");
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            int fantasiesID = -1;
            string ofWhat = "";
            int strength = -1;
            ConstantsAndTypes.REACTION_TYPE reaction = ConstantsAndTypes.REACTION_TYPE.Ambivalent;
            ConstantsAndTypes.ACTION_TYPE intention = ConstantsAndTypes.ACTION_TYPE.Maintain;
            string actionOf = "";

            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                try
                {
                    if (data.HasExtra("fantasiesID"))
                        fantasiesID = data.GetIntExtra("fantasiesID", -1);
                    if (data.HasExtra("ofWhat"))
                        ofWhat = data.GetStringExtra("ofWhat");
                    if (data.HasExtra("strength"))
                        strength = data.GetIntExtra("strength", -1);
                    if (data.HasExtra("reaction"))
                        reaction = (ConstantsAndTypes.REACTION_TYPE)data.GetIntExtra("reaction", -1);
                    if (data.HasExtra("intention"))
                        intention = (ConstantsAndTypes.ACTION_TYPE)data.GetIntExtra("intention", -1);
                    if (data.HasExtra("actionOf"))
                        actionOf = data.GetStringExtra("actionOf");

                    Fantasies fantasiesItem = null;
                    if (fantasiesID == -1)
                    {
                        //new item
                        fantasiesItem = new Fantasies();
                        fantasiesItem.IsNew = true;
                        fantasiesItem.IsDirty = false;
                    }
                    else
                    {
                        fantasiesItem = GlobalData.StructuredPlanFantasies.Find(fantasiesy => fantasiesy.FantasiesID == fantasiesID);
                        fantasiesItem.IsNew = false;
                        fantasiesItem.IsDirty = true;
                    }

                    fantasiesItem.OfWhat = ofWhat;
                    fantasiesItem.Action = intention;
                    fantasiesItem.ActionOf = actionOf;
                    fantasiesItem.Strength = strength;
                    fantasiesItem.Type = reaction;
                    fantasiesItem.Save();

                    if (fantasiesID == -1)
                        GlobalData.StructuredPlanFantasies.Add(fantasiesItem);

                    UpdateAdapter();
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "OnActivityResult: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesConfirm), "StructuredPlanFantasies.OnActivityResult");
                }
            }

            if(resultCode == Result.Canceled)
                Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();

        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.fantasiesActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.fantasiesActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.fantasiesActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanFantasies.SetActionIcons");
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
                _fantasiesList = FindViewById<ListView>(Resource.Id.lstStructuredPlanFantasies);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanFantasiesListMain = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanFantasiesList);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesGetComponents), "StructuredPlanFantasies.GetFieldComponents");
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
                    case Resource.Id.fantasiesActionAdd:
                        Add();
                        return true;

                    case Resource.Id.fantasiesActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.fantasiesActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanFantasiesHelpActivity));
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
                StructuredPlanFantasiesListAdapter adapter = new StructuredPlanFantasiesListAdapter(this);
                if (_fantasiesList != null)
                    _fantasiesList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanFantasiesUpdateAdapter), "StructuredPlanFantasies.UpdateAdapter");
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
                var fantasiesItem = GlobalData.StructuredPlanFantasies[_selectedItemPosition];
                fantasiesItem.Remove();
                GlobalData.StructuredPlanFantasies.Remove(fantasiesItem);
                UpdateAdapter();
                _selectedItemPosition = -1;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Fantasy", "StructuredPlanFantasies.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }
    }
}