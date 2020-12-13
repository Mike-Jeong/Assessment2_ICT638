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
using Newtonsoft.Json;
using Assessment2_Ict638.Models;


namespace Assessment2_Ict638
{
    [Activity(Label = "LoginActivity", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Button btnlogin = FindViewById<Button>(Resource.Id.btnlogin);
            btnlogin.Click += Btnlogin_Click;

            Button btnnewregister = FindViewById<Button>(Resource.Id.btnnewregister);
            btnnewregister.Click += Btnnewregister_Click;

            
        }

        private void Btnnewregister_Click(object sender, EventArgs e)
        {
            Intent newActivity = new Intent(this, typeof(RegisterActivity));
            StartActivity(newActivity);
        }
        private static EditText edUserName;
        private static EditText edPassword;
        public static void clearInput()       
        {
            //clear input
            edUserName.Text = "";
            edPassword.Text = "";
            
        }

        private void Btnlogin_Click(object sender, EventArgs e)
        {

            edUserName = FindViewById<EditText>(Resource.Id.et_id);
            edPassword = FindViewById<EditText>(Resource.Id.et_password);

            string uname = edUserName.Text.Trim();
            string pass = edPassword.Text.Trim();
            string name;
            int id;
            
            if (checkLogin(uname, pass, out id, out name, out string username, out string password, out string phonenumber, out string country, out string email))
            {
                Intent homeActivity = new Intent(this, typeof(HomeActivity));
                Bundle bundle = new Bundle();
                bundle.PutString("name", name);
                bundle.PutInt("id", id);
                bundle.PutString("username", username);
                bundle.PutString("password", password);
                bundle.PutString("phonenumber", phonenumber);
                bundle.PutString("country", country);
                bundle.PutString("email", email);
                homeActivity.PutExtra("data", bundle);
                clearInput();
                StartActivity(homeActivity);

                
            }
            else
            {
                Toast.MakeText(this, "Invalid username or password", ToastLength.Long).Show();
            }
            


            /*  Intent homeActivity = new Intent(this, typeof(HomeActivity));
            StartActivity(homeActivity);*/

        }

        private bool checkLogin(string uname, string pass, out int id, out string name, out string username, out string password, out string phonenumber, out string country, out string email)
        {
            bool staus = false;
            string url = "https://10.0.2.2:5001/api/Users";
            string response = APIConnect.Get(url);
            List<User> users = JsonConvert.DeserializeObject<List<User>>(response);
            id = 0;
            name = "";
            username = "";
            password = "";
            phonenumber = "";
            country = "";
            email = "";
    
            foreach (User user in users)
            {
                if (user.username == uname && user.password == pass)
                {
                    staus = true;
                    id = user.id;
                    name = user.name;
                    username = user.username;
                    password = user.password;
                    phonenumber = user.phonenumber;
                    country = user.country;
                    email = user.email;
                    break;
                }
            }
            
            return staus;

        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            //return base.OnCreateOptionsMenu(menu);
            return true;
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {//menu
            switch (item.ItemId)
            {
                case Resource.Id.Logout:
                    {
                        Intent newActivity = new Intent(this, typeof(HomeActivity));
                        StartActivity(newActivity);
                        return true;
                    }
                case Resource.Id.EditProfile:
                    {
                        //data or user 
                        Intent newActivity = new Intent(this, typeof(ProfileActivity));
                        Bundle bundle = Intent.GetBundleExtra("user");
                        newActivity.PutExtra("user", bundle);


                        StartActivity(newActivity);
                        return true;
                    }

            }

            return base.OnOptionsItemSelected(item);
        }


    }
}