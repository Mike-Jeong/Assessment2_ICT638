﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Android.Gms.Common;

namespace Assessment2_ICT638
{
    [Activity(Label = "AgentActivity")]
    public class AgentActivity : Activity, IOnMapReadyCallback
    {
        public EditText ag_name, ag_email, ag_phone, un, ag_house;

        GoogleMap gmap;
        LatLng curLocation;

        public class Agency
        {
            public string agencyname { get; set; }
            public string agencyemail { get; set; }
            public string agencyphonenumber { get; set; }
            public string agencylocation { get; set; }

        }



        public class User
        {
            public int id { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string phonenumber { get; set; }
            public string country { get; set; }
            public string email { get; set; }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            //show agent page
            SetContentView(Resource.Layout.agent_layout);


            //get agent's info
            ag_name = FindViewById<EditText>(Resource.Id.edagname);
            ag_email = FindViewById<EditText>(Resource.Id.edagemail);
            ag_phone = FindViewById<EditText>(Resource.Id.edagphone);

            
            string agenturl = "https://10.0.2.2:5001/api/Agent";
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

            Agency agent = new Agency();
            agent = Newtonsoft.Json.JsonConvert.DeserializeObject<Agency>(agentinfo);

            ag_name.Text = agent.agencyname;
            ag_email.Text = agent.agencyemail;
            ag_phone.Text = agent.agencyphonenumber;
            ag_house.Text = agent.agencylocation;
            
        

            //map
            var mapFrag = MapFragment.NewInstance();
            this.FragmentManager.BeginTransaction()
                                    .Add(Resource.Id.flmap, mapFrag, "map_fragment")
                                    .Commit();

            mapFrag.GetMapAsync(this);


            //set buttons
            Button share = FindViewById<Button>(Resource.Id.btnshare);
            Button email = FindViewById<Button>(Resource.Id.btnemail);

            share.Click += Cshare;
            email.Click += Cemail;
        }


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
                Intent logout = new Intent(this, typeof(MainActivity));
                StartActivity(logout);
            }

            return base.OnOptionsItemSelected(item);
        }
        

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


            //office's marked
            MarkerOptions office = new MarkerOptions();
            office.SetPosition(new LatLng(-36.84966, 174.76526));
            office.SetTitle("Agent's Office");
            googleMap.AddMarker(office);

            //marked of house list
            var housesadd = ag_house.Text;
            var locations = await Geocoding.GetLocationsAsync(housesadd);
            var location = locations?.FirstOrDefault();

            MarkerOptions house = new MarkerOptions();
            house.SetPosition(new LatLng(location.Latitude, location.Longitude));
            house.SetTitle("House");
            googleMap.AddMarker(house);
            

            getCurrentLoc(googleMap);
        }

        public async void getLastLocation(GoogleMap googleMap)
        {
            Console.WriteLine("Test - LastLoc");
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

            }
        }

        public async void getCurrentLoc(GoogleMap googleMap)
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
                    curLocation = new LatLng(location.Latitude, location.Longitude);
                    curLoc.SetPosition(curLocation);
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
            string userurl = "https://10.0.2.2:5001/api/User";
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

            un.Text = user.name;

            try
            {
                string text = $"Hi, I am {un.Text} saw your details on the Rent-a-go app. Could you please send me details of more houses for rent in the same price range?";
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