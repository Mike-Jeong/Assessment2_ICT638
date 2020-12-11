using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Android.Support.Design.Widget;
using Android.Support.V7.App;

namespace aaaaa
{
    [Activity(Label = "AgentActivity")]
    public class AgentActivity : Activity
    {
        public EditText ag_name, ag_email, ag_phone, ufn, uln;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            //show agent page
            SetContentView(Resource.Layout.agent_layout);


            /*
            //toolbar
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            */


            //get agent's info
            ag_name = FindViewById<EditText>(Resource.Id.edagname);
            ag_email = FindViewById<EditText>(Resource.Id.edagemail);
            ag_phone = FindViewById<EditText>(Resource.Id.edagphone);

            /*
            string agenturl = "";
            string agentinfo = "";
            var httpWebRequest = new HttpWebRequest(new Uri(agenturl));
            httpWebRequest.ServerCertificateValidationCallback = delegate { return true; };
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "Get";

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                agentinfo = reader.ReadToEnd();
            }

            Agent agent = new Agent();
            agent = Newtonsoft.Json.JsonConvert.DeserializeObject<Agent>(agentinfo);

            ag_ame.Text = agent.Name;
            ag_email.Text = agent.Email;
            ag_phone.Text = agent.Phonenumber;
            */


            //get user's info
            /*
            View v = inflater.Inflate(Resource.Layout.userprofile_layout, container, false); 
            
            ufn = v.FindViewById<EditText>(Resource.Id.userfirstname);
            uln = v.FindViewById<EditText>(Resource.Id.userlastname);

            string userurl = "";
            string userinfo = "";
            var httpWebRequest = new HttpWebRequest(new Uri(userurl));
            httpWebRequest.ServerCertificateValidationCallback = delegate { return true; };
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "Get";

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                userinfo = reader.ReadToEnd();
            }

            User user = new User();
            user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userinfo);

            ufn.Text = user.FirstName;
            uln.Text = user.LastName;
            */


            //map
            var mapFrag = MapFragment.NewInstance();
            ChildFragmentManager.BeginTransaction()
                .Add(Resource.Id.flmap, mapFrag, "map")
                .Commit();

            mapFrag.GetMapAsync(this);


            //set buttons
            Button share = FindViewById<Button>(Resource.Id.btnshare);
            Button email = FindViewById<Button>(Resource.Id.btnemail);

            share.Click += Cshare;
            email.Click += Cemail;
        }

        /*
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.agent_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.itprofile)
            {
                Intent profile = new Intent(this, typeof(ProfileActivity));
                StartActivity(profile);
            }
            else if (id == Resource.Id.itlogout)
            {
                Intent logout = new Intent(this, typeof(LoginActivity));
                StartActivity(logout);
            }

            return base.OnOptionsItemSelected(item);
        }
        */

        public async void OnMapReady(GoogleMap googleMap)
        {
            gmap = googleMap;
            googleMap.MapType = GoogleMap.MapTypeNormal;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;


            //Zoom at agent's office
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(-36.84966, 174.76526));
            builder.Zoom(20);
            builder.Tilt(65);

            CameraPosition cPos = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cPos);
            googleMap.MoveCamera(cameraUpdate);


            //marked
            MarkerOptions office = new MarkerOptions();
            office.SetPosition(new LatLng(-36.84966, 174.76526));
            office.SetTitle("Agent's Office");
            googleMap.AddMarker(office);

            /*
            MarkerOptions house = new MarkerOptions();
            house.SetPosition(new LatLng(house location));
            house.SetTitle("House");
            googleMap.AddMarker(house);
            */

            getCurrentLoc(googleMap);
        }

        public async void getLastLocation(GoogleMap googleMap)
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    Console.WriteLine($"Last Loc - Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    MarkerOptions curLoc = new MarkerOptions();
                    curLoc.SetPosition(new LatLng(location.Latitude, location.Longitude));
                    var address = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = address?.FirstOrDefault();
                    var geocodeAddress = "";
                    if (placemark != null)
                    {
                        geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    }
                    curLoc.SetTitle("You were here" + geocodeAddress);
                    curLoc.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));
                    googleMap.AddMarker(curLoc);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Toast.MakeText(Activity, "Feature Not Supported", ToastLength.Short).Show();
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Toast.MakeText(Activity, "Feature Not Enabled", ToastLength.Short).Show();
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Toast.MakeText(Activity, "Needs more permission", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                // Unable to get location
                Toast.MakeText(Activity, "Unable to get location", ToastLength.Short).Show();
            }
        }

        public async void getCurrentLoc(GoogleMap googleMap)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Console.WriteLine($"current Loc - Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    MarkerOptions curLoc = new MarkerOptions();
                    curLoc = new LatLng(location.Latitude, location.Longitude);
                    curLoc.SetPosition(curLoc);
                    var address = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = address?.FirstOrDefault();
                    var geocodeAddress = "";
                    if (placemark != null)
                    {
                        geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    }
                    curLoc.SetTitle("You are here" + geocodeAddress);
                    curLoc.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));

                    googleMap.AddMarker(curLoc);
                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(location.Latitude, location.Longitude));
                    builder.Zoom(18);
                    builder.Bearing(155);
                    builder.Tilt(65);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                    googleMap.MoveCamera(cameraUpdate);
                }
                else
                {
                    getLastLocation(googleMap);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Toast.MakeText(Activity, "Feature Not Supported", ToastLength.Short);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Toast.MakeText(Activity, "Feature Not Enabled", ToastLength.Short);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Toast.MakeText(Activity, "Needs more permission", ToastLength.Short);
            }
            catch (Exception ex)
            {
                getLastLocation(googleMap);
            }
        }

        private async void Cshare(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = $"Agent's Name is {ag_name.Text}, the email address is {ag_email.Text} and the phone number is {ag_phone.Text}",
                Title = "Agent's details"
            });
        }

        private async void Cemail(object sender, EventArgs e)
        {
            try
            {
                string text = $"Hi, I am {ufn.Text}{uln.Text} saw your details on the Rent-a-go app. Could you please send me details of more houses for rent in the same price range?";
                string recipient = ag_phone.Text;
                var message = new SmsMessage(text, new[] { recipient });
                await Sms.ComposeAsync(message);
            }
            catch (Exception ex)
            {

            }
        }
    }
}