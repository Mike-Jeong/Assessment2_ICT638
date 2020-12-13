using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Assessment2_Ict638.Models;
using Android.Gms.Common;

namespace Assessment2_Ict638
{/// <summary>
/// //
/// </summary>
    public class HousedetailFragment : Fragment, IOnMapReadyCallback
    {
        GoogleMap gMap;
        
       int photonum;
       int id;
       int hid;
       List<Data> hList;
       List<Agency> agencies;

        // string h;
        // string nr;
        // string nt;
        // string r;
        // string l;
        // string an;
        // string d;
        // string anum;
        //LatLng curLocation;




        public HousedetailFragment(List<Data> l, List<Agency> A) 
        {
            hList = l;
            agencies = A;

        }


       /* public HousedetailFragment(string heading, string numberofroom, string numberoftoilet, string rentfee, string location, string agencyname, string description) //, string agencyphonenumber)
        {
            h = heading; nr = numberofroom; nt = numberoftoilet; r = rentfee; l = location; an = agencyname; d = description; // anum = agencyphonenumber;

        }*/



        public void getph(int n)
        {
            photonum = n;
            hid = n;

            switch (photonum)
            {
                case 1:
                    id = Resource.Drawable.P1;

                    break;

                case 2:
                    id = Resource.Drawable.P2;

                    break;

                case 3:
                    id = Resource.Drawable.P3;

                    break;

                case 4:
                    id = Resource.Drawable.P4;

                    break;

                case 5:
                    id = Resource.Drawable.P5;

                    break;

                case 6:
                    id = Resource.Drawable.P6;

                    break;

                case 7:
                    id = Resource.Drawable.P7;

                    break;

                case 8:
                    id = Resource.Drawable.P8;

                    break;

                case 9:
                    id = Resource.Drawable.P9;

                    break;

                case 10:
                    id = Resource.Drawable.P10;

                    break;

                default:
                    break;
            }

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
       
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            // Use this to return your custom view for this Fragment
            View v = inflater.Inflate(Resource.Layout.fragment_housedetail, container, false);

            TextView Heading = v.FindViewById<TextView>(Resource.Id.HtvHeading);
            TextView Numberofroom = v.FindViewById<TextView>(Resource.Id.HtvNumberofroom);
            TextView Numberoftoilet = v.FindViewById<TextView>(Resource.Id.HtvNumberoftoilet);
            TextView Rentfee = v.FindViewById<TextView>(Resource.Id.HtvRentfee);
            TextView nAgency = v.FindViewById<TextView>(Resource.Id.HtvAgency);
            TextView ALocation = v.FindViewById<TextView>(Resource.Id.HtvLocation);
            TextView Description = v.FindViewById<TextView>(Resource.Id.HtvDescription);

            // heading = "House name : " + data.GetString("heading");
            // numberofroom = data.GetString("numberofroom");
            //numberoftoilet = data.GetString("numberoftoilet");
            // rentfee = data.GetString("rentfee");
            // location = data.GetString("location");
            // agencyname = "Agency name : " + data.GetString("agencyname");
            // description = "Description " + data.GetString("description");

            Heading.Text = "House name : " + hList[hid-1].heading;
            Numberofroom.Text = hList[hid-1].numberofroom;
            Numberoftoilet.Text = hList[hid-1].numberoftoilet;
            Rentfee.Text = hList[hid-1].rentfee;
            nAgency.Text = "Agency name : " + agencies[0].agencyname;
            ALocation.Text = hList[hid-1].location;
            Description.Text = "Description " + hList[hid-1].description;



            ImageView imgMapFrag = v.FindViewById<ImageView>(Resource.Id.imgHMapFrag);
            imgMapFrag.SetImageResource(id);

            FrameLayout mapFragContainer = v.FindViewById<FrameLayout>(Resource.Id.mapFrgContainer);

            var mapFrag = MapFragment.NewInstance();
            ChildFragmentManager.BeginTransaction()
                .Add(Resource.Id.mapFrgContainer, mapFrag, "map")
                .Commit();

            mapFrag.GetMapAsync(this);

      




            Button btnShare = v.FindViewById<Button>(Resource.Id.btnShare);
            Button btnSMS = v.FindViewById<Button>(Resource.Id.btnSMS);

            btnShare.Click += BtnShare_Click;
            btnSMS.Click += BtnSMS_Click;


            return v;
        }


        private async void BtnSMS_Click(object sender, EventArgs e)
        {
            //To use this, delete async
            //throw new NotImplementedException();


            try
            {
                string text = "Hi, I am interested in the house at" + hList[hid-1].location + "you have posted for rent. Could I please have more details?";
                string recipient = agencies[0].agencyphonenumber;
                var message = new SmsMessage(text, new[] { recipient });
                await Sms.ComposeAsync(message);
            }
            catch (Exception exp)
            {
                Toast.MakeText(Activity, "Exception Found", ToastLength.Long).Show();
            }
        }
        private async void BtnShare_Click(object sender, EventArgs e)
        {
            string locDetails = "House name : " + hList[hid - 1].heading + ", " + hList[hid-1].numberofroom + ", " + hList[hid-1].numberoftoilet + ", " + hList[hid-1].rentfee + ", " + hList[hid-1].location;
            await ShareText(locDetails);
        }

     


        public async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "House Detail Share"
            }
            );
        }


        public async void OnMapReady(GoogleMap googleMap)
        {
        
           

            googleMap.MapType = GoogleMap.MapTypeNormal;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;

           

            var location = (await Geocoding.GetLocationsAsync(string.Format("$\"{0}\"", hList[hid - 1].location))).FirstOrDefault();

            if (location == null) return;
            LatLng loc = new LatLng(location.Latitude, location.Longitude);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(loc);
            builder.Zoom(20);
            builder.Tilt(65);

            CameraPosition cPos = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cPos);
            googleMap.MoveCamera(cameraUpdate);

            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(loc);
            markerOptions.SetTitle("NZSE City Campus");


            
            googleMap.AddMarker(markerOptions);

            gMap = googleMap;
            addingmark();



        }


     

        public void addingmark()
        {
         //   getHouseLocation(gMap);
            getCurLocation(gMap);
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
                    curLoc.SetTitle("You were here" + geocodeAddress);
                    curLoc.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));
                    googleMap.AddMarker(curLoc);
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
                // Unable to get location
                Toast.MakeText(Activity, "Unable to get location", ToastLength.Short);
            }
        }



        



    }
}