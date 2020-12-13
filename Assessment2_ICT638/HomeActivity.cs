using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Assessment2_Ict638.Models;
using Android.Util;
using Android.Gms.Common;
using Android.Support.V7.App;
using AlertDialog = Android.App.AlertDialog;







namespace Assessment2_Ict638
{/// <summary>
/// //
/// </summary>
    [Activity(Label = "Rent A Go")]
    public class HomeActivity : AppCompatActivity //, Android.Support.V7.Widget.Toolbar.IOnMenuItemClickListener
    {
        public const string TAG = "HomeActivity";
        internal static readonly string CHANNEL_ID = "ict638assessment_notification_channel";

        RecyclerView mRecycleView;
        RecyclerView.LayoutManager mLayoutManager;
        PhotoAlbum mPhotoAlbum;
        PhotoAdapter mAdapter;
        List<Data> dList = new List<Data>();
        Bundle bundle;

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Log.Debug(TAG, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(TAG, "This device is not supported");
                    Finish();
                }
                return false;
            }

            Log.Debug(TAG, "Google Play Services is available.");
            return true;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channelName = CHANNEL_ID;
            var channelDescription = string.Empty;
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

        }

        private void putData()
        {
           /* dList.Add(new Data("Database", "This is the database description"));
            dList.Add(new Data("Programming", "This is the programming description"));
            dList.Add(new Data("GitHub", "This is the github description"));
            dList.Add(new Data("Math", "This is Math description"));
            dList.Add(new Data("MongoDB", "This is MongoDB description"));
            dList.Add(new Data("C#", "This is a C# description"));*/

            string url = "https://10.0.2.2:5001/api/Data";
            string response = APIConnect.Get(url);
            dList = JsonConvert.DeserializeObject<List<Data>>(response);

        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.content_home);
            bundle = Intent.GetBundleExtra("data");


            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key != null)
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    }
                }
            }

            IsPlayServicesAvailable();
            CreateNotificationChannel();

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
      
            //toolbar.SetOnMenuItemClickListener(this);
            //SetSupportActionBar(toolbar);

            mPhotoAlbum = new PhotoAlbum();
            putData();
            mLayoutManager = new LinearLayoutManager(this);

            mAdapter = new PhotoAdapter(mPhotoAlbum, dList);
            mAdapter.ItemClick += MAdapter_ItemClick;

            mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mRecycleView.SetLayoutManager(mLayoutManager);
            mRecycleView.SetAdapter(mAdapter);


        }

        // delete button profile

        private void MAdapter_ItemClick(object sender, int e)
        {
            int photoNum = e + 1;
            Toast.MakeText(this, "This is House number " + photoNum, ToastLength.Short).Show();

            Intent i = new Intent(this, typeof(NavigationActivity));
            string uname = bundle.GetString("username");
            string uphone = bundle.GetString("phonenumber");
            
            Bundle bundle2 = new Bundle();
            i.PutExtra("ListItem", JsonConvert.SerializeObject(dList));

            bundle2.PutInt("photoid", photoNum);
            bundle2.PutString("uname", uname);
            bundle2.PutString("uphone", uphone);
           

            /* bundle2.PutString("heading", dList[e].heading);
             bundle2.PutString("description", dList[e].description);
             bundle2.PutString("numberofroom", dList[e].numberofroom);
             bundle2.PutString("numberoftoilet", dList[e].numberoftoilet);
             bundle2.PutString("rentfee", dList[e].rentfee);
             bundle2.PutString("location", dList[e].location);
             bundle2.PutString("agencyname", dList[e].agencyname);*/


             i.PutExtra("data", bundle2);

            StartActivity(i);


        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            //return base.OnCreateOptionsMenu(menu);
            return true;
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.EditProfile:
                    Intent newActivity = new Intent(this, typeof(ProfileActivity));
                     newActivity.PutExtra("data", bundle);


                    StartActivity(newActivity);
                    return true;
                case Resource.Id.Logout:
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);

                    builder.SetTitle("Logout?");
                    builder.SetMessage("Are you sure you want to log out of the app?\n(Go to the Login page after the logout.)");
                    builder.SetPositiveButton("OK", (c, ev) =>
                    {

                        Intent LoginActivity = new Intent(this, typeof(LoginActivity));
                        StartActivity(LoginActivity);
                        FinishAffinity();

                    });
                    builder.SetNegativeButton("Cancel", (c, ev) =>
                    {
                        builder.Dispose();
                    });
                    builder.Create().Show();
                    return true;


            }

            return base.OnOptionsItemSelected(item);
        }

       /* public bool OnMenuItemClick(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.EditProfile:
                    Intent newActivity = new Intent(this, typeof(ProfileActivity));
                    Bundle bundle = Intent.GetBundleExtra("data");
                    newActivity.PutExtra("data", bundle);


                    StartActivity(newActivity);
                    return true;
                case Resource.Id.Logout:
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);

                    builder.SetTitle("Logout?");
                    builder.SetMessage("Are you sure you want to log out of the app?\n(Go to the Login page after the logout.)");
                    builder.SetPositiveButton("OK", (c, ev) =>
                    {

                        Intent LoginActivity = new Intent(this, typeof(LoginActivity));
                        StartActivity(LoginActivity);
                        FinishAffinity();

                    });
                    builder.SetNegativeButton("Cancel", (c, ev) =>
                    {
                        builder.Dispose();
                    });
                    builder.Create().Show();
                    return true;


            }
            return false;
        }*/

    }
}