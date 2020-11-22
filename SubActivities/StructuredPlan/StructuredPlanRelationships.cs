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
    [Activity(Label = "Structured Plan Relationships")]
    public class StructuredPlanRelationships : AppCompatActivity, IAlertCallback
    {
        public const string TAG = "M:StructuredPlanRelationships";

        private Toolbar _toolbar;

        private ListView _relationshipsList;

        private Button _done;

        private LinearLayout _linStructuredPlanRelationshipsList;

        private int _selectedItemPosition = -1;

        private ImageLoader _imageLoader = null;

        public StructuredPlanRelationships()
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
            MenuInflater.Inflate(Resource.Menu.RelationshipsMenu, menu);

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

                SetContentView(Resource.Layout.StructuredPlanRelationshipsLayout);

                GetFieldComponents();

                _toolbar = ToolbarHelper.SetupToolbar(this, Resource.Id.relationshipsToolbar, Resource.String.StructuredPlanRelationshipsActivityTitle, Color.White);

                _imageLoader = ImageLoader.Instance;

                _imageLoader.LoadImage
                (
                    "drawable://" + Resource.Drawable.structuredplanrelationshipspager,
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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsCreate), "StructuredPlanRelationships.OnCreate");
            }
        }
        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linStructuredPlanRelationshipsList != null)
                _linStructuredPlanRelationshipsList.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void SetupCallbacks()
        {
            try
            {
                if (_relationshipsList != null)
                {
                    _relationshipsList.ItemClick += RelationshipsList_ItemClick;
                    _relationshipsList.ItemLongClick += RelationshipsList_ItemLongClick;
                }
                if(_done != null)
                    _done.Click += Done_Click;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "SetupCallbacks: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsSetCallbacks), "StructuredPlanRelationships.SetupCallbacks");
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void RelationshipsList_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var relationshipID = GlobalData.StructuredPlanRelationships[e.Position].RelationshipsID;

            Intent intent = new Intent(this, typeof(StructuredPlanRelationshipsDialogActivity));
            intent
                .PutExtra("relationshipsID", relationshipID)
                .PutExtra("activityTitle", "Edit Relationship");

            StartActivityForResult(intent, ConstantsAndTypes.EDIT_RELATIONSHIP);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            int relationshipsID = -1;
            string withWhom = "";
            ConstantsAndTypes.RELATIONSHIP_TYPE type = ConstantsAndTypes.RELATIONSHIP_TYPE.Aquaintance;
            int strength = -1;
            int feeling = -1;
            ConstantsAndTypes.ACTION_TYPE action = ConstantsAndTypes.ACTION_TYPE.Maintain;
            string actionOf = "";

            base.OnActivityResult(requestCode, resultCode, data);

            if(resultCode == Result.Ok)
            {
                try
                {
                    if (data.HasExtra("relationshipsID"))
                        relationshipsID = data.GetIntExtra("relationshipsID", -1);
                    if (data.HasExtra("withWhom"))
                        withWhom = data.GetStringExtra("withWhom");
                    if (data.HasExtra("type"))
                        type = (ConstantsAndTypes.RELATIONSHIP_TYPE)data.GetIntExtra("type", -1);
                    if (data.HasExtra("strength"))
                        strength = data.GetIntExtra("strength", -1);
                    if (data.HasExtra("feeling"))
                        feeling = data.GetIntExtra("feeling", -1);
                    if (data.HasExtra("action"))
                        action = (ConstantsAndTypes.ACTION_TYPE)data.GetIntExtra("action", -1);
                    if (data.HasExtra("actionOf"))
                        actionOf = data.GetStringExtra("actionOf");

                    Relationships relationshipsItem = null;
                    if (relationshipsID == -1)
                    {
                        //new item
                        relationshipsItem = new Relationships();
                        relationshipsItem.IsNew = true;
                        relationshipsItem.IsDirty = false;
                    }
                    else
                    {
                        relationshipsItem = GlobalData.StructuredPlanRelationships.Find(attitude => attitude.RelationshipsID == relationshipsID);
                        relationshipsItem.IsNew = false;
                        relationshipsItem.IsDirty = true;
                    }

                    relationshipsItem.WithWhom = withWhom;
                    relationshipsItem.Type = type;
                    relationshipsItem.Strength = strength;
                    relationshipsItem.Feeling = feeling;
                    relationshipsItem.Action = action;
                    relationshipsItem.ActionOf = actionOf;
                    relationshipsItem.Save();

                    if (relationshipsID == -1)
                        GlobalData.StructuredPlanRelationships.Add(relationshipsItem);

                    UpdateAdapter();
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "ConfirmPlanItemAddition: Exception - " + e.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsConfirm), "StructuredPlanRelationships.ConfirmPlanItemAddition");
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
                    alertHelper.AlertTitle = GetString(Resource.String.StructuredPlanRelationshipsRemoveAlertTitle);
                    alertHelper.AlertMessage = GetString(Resource.String.StructuredPlanRelationshipsRemoveAlertQuestion);
                    alertHelper.AlertIconResourceID = Resource.Drawable.SymbolStop;
                    alertHelper.AlertPositiveCaption = GetString(Resource.String.ButtonYesCaption);
                    alertHelper.AlertNegativeCaption = GetString(Resource.String.ButtonNoCaption);
                    alertHelper.InstanceId = "remove";
                    alertHelper.ShowAlert();
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, "Remove_Click: Exception - " + ex.Message);
                    if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanRelationshipsRemove), "StructuredPlanRelationships.Remove_Click");
                }
            }
            else
            {
                string warning = "";
                if (GlobalData.StructuredPlanRelationships.Count == 0)
                {
                    warning = GetString(Resource.String.StructuredPlanRelationshipsWarning1);
                }
                else
                {
                    warning = GetString(Resource.String.StructuredPlanRelationshipsWarning2);
                }
                Toast.MakeText(this, warning, ToastLength.Short).Show();
            }
        }

        private void RelationshipsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedItemPosition = e.Position;
            Log.Info(TAG, "RelationshipsList_ItemClick: Selected item at position " + _selectedItemPosition.ToString());
            UpdateAdapter();
            _relationshipsList.SetSelection(_selectedItemPosition);
        }

        private void Add()
        {
            try
            {
                if (_relationshipsList != null)
                {
                    Intent intent = new Intent(this, typeof(StructuredPlanRelationshipsDialogActivity));
                    intent
                        .PutExtra("relationshipsID", -1)
                        .PutExtra("activityTitle", "Add Relationship");

                    StartActivityForResult(intent, ConstantsAndTypes.ADD_RELATIONSHIP);
                }
            }
            catch (Exception ex)
            {
                Log.Error(TAG, "Add: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, GetString(Resource.String.ErrorStructuredPlanRelationshipsAdd), "StructuredPlanRelationships.Add");
            }
        }

        private void SetActionIcons(IMenu menu)
        {
            try
            {
                ConstantsAndTypes.ScreenSize screenSize = SystemHelper.GetScreenSize();

                //get references to each of the menu items
                var itemAdd = menu.FindItem(Resource.Id.relationshipsActionAdd);
                var itemRemove = menu.FindItem(Resource.Id.relationshipsActionRemove);
                var itemHelp = menu.FindItem(Resource.Id.relationshipsActionHelp);

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
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, "Setting Action Icons", "StructuredPlanRelationships.SetActionIcons");
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
                _relationshipsList = FindViewById<ListView>(Resource.Id.lstStructuredPlanRelationships);
                _done = FindViewById<Button>(Resource.Id.btnDone);
                _linStructuredPlanRelationshipsList = FindViewById<LinearLayout>(Resource.Id.linStructuredPlanRelationshipsList);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsGetComponents), "StructuredPlanRelationships.GetFieldComponents");
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
                    case Resource.Id.relationshipsActionAdd:
                        Add();
                        return true;

                    case Resource.Id.relationshipsActionRemove:
                        Remove();
                        return true;

                    case Resource.Id.relationshipsActionHelp:
                        Intent intent = new Intent(this, typeof(StructuredPlanRelationshipsHelpActivity));
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
                StructuredPlanRelationshipsListAdapter adapter = new StructuredPlanRelationshipsListAdapter(this);
                if (_relationshipsList != null)
                    _relationshipsList.Adapter = adapter;
            }
            catch (Exception e)
            {
                Log.Error(TAG, "UpdateAdapter: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, e, GetString(Resource.String.ErrorStructuredPlanRelationshipsUpdateAdapter), "StructuredPlanRelationships.UpdateAdapter");
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
                var relationshipsItem = GlobalData.StructuredPlanRelationships[_selectedItemPosition];
                relationshipsItem.Remove();
                GlobalData.StructuredPlanRelationships.Remove(relationshipsItem);
                UpdateAdapter();
                _selectedItemPosition = -1;
            }
            catch(Exception ex)
            {
                Log.Error(TAG, "AlertPositiveButtonSelect: Exception - " + ex.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(this, ex, "Removing Relationship", "StructuredPlanRelationships.AlertPositiveButtonSelect");
            }
        }

        public void AlertNegativeButtonSelect(object sender, DialogClickEventArgs e, string instanceId)
        {
            Toast.MakeText(this, Resource.String.StructuredplanNoChangesToast, ToastLength.Short).Show();
        }
    }
}