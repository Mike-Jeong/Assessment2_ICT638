using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assessment2_Ict638.Models;
using Android.Util;
using Android.Gms.Maps;
using Xamarin.Essentials;
using Android.Gms.Maps.Model;
using System.Threading.Tasks;

namespace Assessment2_Ict638
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : Activity, IOnMapReadyCallback
    {
        int userid;
        Bundle bundle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_userprofile);
            bundle = Intent.GetBundleExtra("data");
            userid = bundle.GetInt("id");

            EditText et = FindViewById<EditText>(Resource.Id.Pname);
            EditText et2 = FindViewById<EditText>(Resource.Id.PPhonenumber);
            TextView tv1 = FindViewById<TextView>(Resource.Id.Pemail);
            EditText et3 = FindViewById<EditText>(Resource.Id.PCountry);
            TextView tv3 = FindViewById<TextView>(Resource.Id.Pusername);
            TextView tv4 = FindViewById<TextView>(Resource.Id.Ppassword);

            et.Hint = bundle.GetString("name");
            tv3.Text = bundle.GetString("username");
            tv4.Text = bundle.GetString("password");
            et2.Hint = bundle.GetString("phonenumber");
            et3.Hint = bundle.GetString("country");
            tv1.Text = bundle.GetString("email");


            // Delete the logout and current location
            Button btnPModify = FindViewById<Button>(Resource.Id.btnPModify);
            Button btnPSMS = FindViewById<Button>(Resource.Id.btnPSMS);
            Button btnPShare = FindViewById<Button>(Resource.Id.btnPShare);

            btnPModify.Click += BtnPModify_Click;
            btnPShare.Click += BtnPShare_Click;
            btnPSMS.Click += BtnPSMS_Click;



            FrameLayout mapFragContainer = FindViewById<FrameLayout>(Resource.Id.PMapFrgContainer);


            var mapFrag = MapFragment.NewInstance();
            FragmentManager.BeginTransaction().Add(Resource.Id.PMapFrgContainer, mapFrag, "map").Commit();

            mapFrag.GetMapAsync(this);

        }

        private async void BtnPSMS_Click(object sender, EventArgs e)
        {
            string text = "Hi, Please find my contact details as requested.Email: " + bundle.GetString("email") + " Phone Number: " + bundle.GetString("phonenumber");
            string recipient = "0226329826";
            var message = new SmsMessage(text, new[] { recipient });
            await Sms.ComposeAsync(message);
        }

        private async void BtnPShare_Click(object sender, EventArgs e)
        {
            string UDetails = "";
            UDetails = bundle.GetString("name") + bundle.GetString("phonenumber") + bundle.GetString("email");
            await ShareText(UDetails);
        }

        public async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "User Detail Share"
            });
        }


        private void BtnPModify_Click(object sender, EventArgs e)
        {
            EditText et = FindViewById<EditText>(Resource.Id.Pname);
            EditText et2 = FindViewById<EditText>(Resource.Id.PPhonenumber);
            TextView tv1 = FindViewById<TextView>(Resource.Id.Pemail);
            EditText et3 = FindViewById<EditText>(Resource.Id.PCountry);
            TextView tv3 = FindViewById<TextView>(Resource.Id.Pusername);
            TextView tv4 = FindViewById<TextView>(Resource.Id.Ppassword);



            User u = new User();
            u.name = et.Text;
            u.username = tv3.Text;
            u.password = tv4.Text;
            u.email = tv1.Text;
            u.country = et3.Text;
            u.phonenumber = et2.Text;


            ModifyUser(u, userid);
        }
        public string getQuotedString(string str)
        {
            return "\"" + str + "\"";
        }

        public void ModifyUser(User user, int Userid)
        {
            string url = "https://10.0.2.2:5001/api/Users" + "/" + Userid;

            string json =
                "{" +
                getQuotedString("id")+":"+Userid+","+
                getQuotedString("name") + ":" + getQuotedString(user.name) + "," +
                getQuotedString("username") + ":" + getQuotedString(user.username) + "," +
                getQuotedString("password") + ":" + getQuotedString(user.password) + "," +
                getQuotedString("phonenumber") + ":" + getQuotedString(user.phonenumber) + "," +
                getQuotedString("country") + ":" + getQuotedString(user.country) + "," +
                getQuotedString("email") + ":" + getQuotedString(user.email) +
                "}";

            if (APIConnect.Put(url, json))
            {
                Toast.MakeText(this, "User successfully Modified!", ToastLength.Long).Show();
            }
        }


        GoogleMap gMap;

        //May delete the logout in profile

        public void OnMapReady(GoogleMap googleMap)
        {
            //throw new NotImplementedException();
        
            gMap = googleMap;
            googleMap.MapType = GoogleMap.MapTypeNormal;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;

            getCurLocation(googleMap);

            //LatLng loc = new LatLng(lasLoc);
            //CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            //builder.Target(loc);
            //builder.Zoom(20);
            //builder.Tilt(65);

            //CameraPosition cPos = builder.Build();
            //CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cPos);
            //googleMap.MoveCamera(cameraUpdate);

            //MarkerOptions markerOptions = new MarkerOptions();
            //markerOptions.SetPosition(loc);
            //markerOptions.SetTitle("NZSE City Campus");

            //googleMap.AddMarker(markerOptions);

            
        }

        public async void getCurLocation(GoogleMap googleMap)
        {
            Console.WriteLine("Test - CurrentLoc");
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Console.WriteLine($"current Loc - Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    MarkerOptions curLoc = new MarkerOptions();
                    curLoc.SetPosition(new LatLng(location.Latitude, location.Longitude));
                    var address = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = address?.FirstOrDefault();
                    var geocodeAddress = "";
                    if (placemark != null)
                    {
                        geocodeAddress =
                        $"AdminArea: {placemark.AdminArea}\n" +
                        $"CountryCode: {placemark.CountryCode}\n" +
                        $"CountryName: {placemark.CountryName}\n" +
                        $"FeatureName: {placemark.FeatureName}\n" +
                        $"Locality: {placemark.Locality}\n" +
                        $"PostalCode: {placemark.PostalCode}\n" +
                        $"SubAdminArea: {placemark.SubAdminArea}\n" +
                        $"SubLocality: {placemark.SubLocality}\n" +
                        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                        $"Thoroughfare: {placemark.Thoroughfare}\n";

                    }
                    curLoc.SetTitle("You are here" + geocodeAddress);
                    curLoc.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));

                    googleMap.AddMarker(curLoc);

                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(location.Latitude, location.Longitude));
                    builder.Zoom(20);
                    builder.Tilt(65);

                    CameraPosition cPos = builder.Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cPos);
                    googleMap.MoveCamera(cameraUpdate);





                }
                else
                {
                    OnMapReady(googleMap);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                // Toast.MakeText(Activity"Feature Not Supported", ToastLength.Short);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                // Toast.MakeText(Activity, "Feature Not Enabled", ToastLength.Short);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                // Toast.MakeText(Activity, "Needs more permission", ToastLength.Short);
            }
            catch (Exception ex)
            {
                OnMapReady(googleMap);
            }
        }
    }
}