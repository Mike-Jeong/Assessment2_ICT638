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

namespace Assessment2_Ict638
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_userregister);

            Button btnRRegister = FindViewById<Button>(Resource.Id.btnRRegister);
            btnRRegister.Click += BtnRRegister_Click;



        }

        private void BtnRRegister_Click(object sender, EventArgs e)
        {
            EditText edName = FindViewById<EditText>(Resource.Id.Rname);
            EditText edUserName = FindViewById<EditText>(Resource.Id.Rusername);
            EditText edPassword = FindViewById<EditText>(Resource.Id.Rpassword);
            EditText edphonenumber = FindViewById<EditText>(Resource.Id.RPhonenumber);
            EditText edcountry = FindViewById<EditText>(Resource.Id.RCountry);
            EditText edemail = FindViewById<EditText>(Resource.Id.Remail);


            User u = new User();
            u.name = edName.Text;
            u.username = edUserName.Text;
            u.password = edPassword.Text;
            u.email = edemail.Text;
            u.country = edcountry.Text;
            u.phonenumber = edphonenumber.Text;


            PostUser(u);



        }

        public string getQuotedString(string str)
        {
            return "\"" + str + "\"";
        }
        public void PostUser(User user)
        {
            string url = "https://10.0.2.2:5001/api/Users";

            string json =
                "{" +
                getQuotedString("name") + ":" + getQuotedString(user.name) + "," +
                getQuotedString("username") + ":" + getQuotedString(user.username) + "," +
                getQuotedString("password") + ":" + getQuotedString(user.password) + "," +
                getQuotedString("phonenumber") + ":" + getQuotedString(user.phonenumber) + "," +
                getQuotedString("country") + ":" + getQuotedString(user.country) + "," +
                getQuotedString("email") + ":" + getQuotedString(user.email) +
                "}";

            if (APIConnect.Post(url, json))
            {
                Toast.MakeText(this, "User successfully created!", ToastLength.Long).Show();
            }
        }



    }

}