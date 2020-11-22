using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using com.spanyardie.MindYourMood.Model;
using Android.Util;
using com.spanyardie.MindYourMood.Helpers;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Listener;
using Android.Graphics.Drawables;

namespace com.spanyardie.MindYourMood.Adapters
{
    public class ResourceMedicationTypeAdapter : BaseAdapter
    {
        public const string TAG = "M:ResourceMedicationTypeAdapter";

        private List<ResourceMedicationType> _medicationTypes;
        private Activity _activity;

        private TextView _typeTitle;
        private TextView _typeDescription;
        private LinearLayout _typeItemsContainer;
        private LinearLayout _linMedicationTypes;

        private ImageLoader _imageLoader = null;

        public ResourceMedicationTypeAdapter(Activity activity)
        {
            _medicationTypes = GlobalData.ResourceMedicationTypes;
            _activity = activity;
        }

        public override int Count
        {
            get
            {
                return _medicationTypes.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return _medicationTypes[position].ID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            try
            {
                if (convertView == null)
                {
                    if (_activity != null)
                    {
                        convertView = _activity.LayoutInflater.Inflate(Resource.Layout.MedicationTypeItemBody, null);
                    }

                }
                if (convertView != null)
                {
                    GetFieldComponents(convertView);
                    if (_typeTitle != null)
                        _typeTitle.Text = _medicationTypes[position].MedicationTypeTitle;
                    if (_typeDescription != null)
                        _typeDescription.Text = _medicationTypes[position].MedicationTypeDescription;
                    AddTypeItems(position);
                    _imageLoader = ImageLoader.Instance;

                    _imageLoader.LoadImage
                    (
                        "drawable://" + Resource.Drawable.variouspills,
                        new ImageLoadingListener
                        (
                            loadingComplete: (imageUri, view, loadedImage) =>
                            {
                                var args = new LoadingCompleteEventArgs(imageUri, view, loadedImage);
                                ImageLoader_LoadingComplete(null, args);
                            }
                        )
                    );

                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetView: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting Medication view", "ResourceMedicationTypeAdapter.GetView");
            }
            return convertView;
        }

        private void ImageLoader_LoadingComplete(object sender, LoadingCompleteEventArgs e)
        {
            var bitmap = e.LoadedImage;

            if (_linMedicationTypes != null)
                _linMedicationTypes.SetBackgroundDrawable(new BitmapDrawable(bitmap));
        }

        private void AddTypeItems(int position)
        {
            try
            {
                if (_typeItemsContainer != null)
                {
                    _typeItemsContainer.RemoveAllViews();
                    View item;
                    TextView itemTitle = null;
                    TextView itemDescription = null;
                    TextView itemSideEffects = null;
                    TextView itemDosage = null;

                    foreach (var med in _medicationTypes[position].MedicationItems)
                    {
                        item = _activity.LayoutInflater.Inflate(Resource.Layout.ResourceMedicationItemDetail, null);
                        if (item != null)
                        {
                            itemTitle = item.FindViewById<TextView>(Resource.Id.txtItemTitle);
                            itemDescription = item.FindViewById<TextView>(Resource.Id.txtItemDescription);
                            itemSideEffects = item.FindViewById<TextView>(Resource.Id.txtItemSideEffects);
                            itemDosage = item.FindViewById<TextView>(Resource.Id.txtItemDosage);

                            if (itemTitle != null)
                                itemTitle.Text = med.MedicationItemTitle;
                            if (itemDescription != null)
                                itemDescription.Text = med.MedicationItemDescription;
                            if (itemSideEffects != null)
                                itemSideEffects.Text = med.SideEffects;
                            if (itemDosage != null)
                                itemDosage.Text = med.Dosage;

                            _typeItemsContainer.AddView(item);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Error(TAG, "AddTypeItems: Exception - " + e.Message);
                if (GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Adding type items", "ResourceMedicationTypeAdapter.AddTypeItems");
            }
        }

        private void GetFieldComponents(View convertView)
        {
            try
            {
                _typeTitle = convertView.FindViewById<TextView>(Resource.Id.txtMedicationTypeListHeaderTitle);
                _typeDescription = convertView.FindViewById<TextView>(Resource.Id.txtMedicationTypeListDescription);
                _typeItemsContainer = convertView.FindViewById<LinearLayout>(Resource.Id.linTypeItems);
                _linMedicationTypes = convertView.FindViewById<LinearLayout>(Resource.Id.linMedicationTypeHeader);
            }
            catch(Exception e)
            {
                Log.Error(TAG, "GetFieldComponents: Exception - " + e.Message);
                if(GlobalData.ShowErrorDialog) ErrorDisplay.ShowErrorAlert(_activity, e, "Getting adapter field components", "ResourceMedicationTypeAdapter.GetFieldComponents");
            }
        }
    }
}