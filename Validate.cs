using Google.Cloud.Firestore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Firestore;
using System.Net;
using System.Security.Cryptography;

namespace WebOAuth
{
    public partial class Validate : Form
    {
        public static string utoken,uid;
        public Loading loading1;
        public Validate(string token)
        {
                loading1 = new Loading();
                loading1.Show();
                InitializeComponent();
                utoken = token;
            
            
            /*MessageBox.Show("Validating Token");*/
        }

        private async void Load_Validate()
        {
            string apiEndpoint = "https://weboauthapi.onrender.com/validateutoken";

            try
            {
                // Call the API to get the token with custom headers
                string response = await GetUserIDAsync(apiEndpoint);
                var json = JObject.Parse(response);

                string uid = json["uid"].ToString();
                long time = long.Parse(json["time"].ToString());
                long ctime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000;
                loading1.Hide();
                if (ctime>time)
                {
           

                    MessageBox.Show("Timed OUT Sorry Try again");
                }
                else
                {
                    /*   Firebase get data */
                    setData(uid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }
        public void setData(string uid) {
            try
            {

            loading1.Show();
                
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\vasuk\Downloads\oauthweb-2-firebase-adminsdk-398ue-63df13d8e7.json");

            // Initialize Firestore
            FirestoreDb db = FirestoreDb.Create("oauthweb-2");

            // Example: Retrieve data from a Firestore collection
            DocumentReference docRef = db.Collection("Users").Document(uid);

            // Asynchronously retrieve document snapshot
            DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;

            if (snapshot.Exists)
            {
                Dictionary<string, object> user = snapshot.ToDictionary();
                /*Console.WriteLine($"Document data for user123:");*/
                lblFname.Text = user["fname"].ToString();
                lblLname.Text = user["lname"].ToString();
                lblEmail.Text = user["email"].ToString();
                setProfilePhoto(user["ppurl"].ToString());
                    Properties.Settings.Default.UserUID = uid;
                    Properties.Settings.Default.Save();
                }
            else
            {
                MessageBox.Show("User Data not found !");
            }
            
            loading1.Hide();
            }catch(Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private void setProfilePhoto(string imageUrl)
        {
            try
            {
                WebClient webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(imageUrl);

                // Convert byte array to Image
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    Image img = Image.FromStream(ms);

                    // Assign the image to PictureBox
                    pictureBox1.Image = img;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task<string> GetUserIDAsync(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                // Add custom headers
                client.DefaultRequestHeaders.Add("usertoken", utoken);


                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    // Assume the API returns the token directly as plain text

                    return responseData;
                }
                else
                {
                    // Handle error response
                    throw new Exception($"API call failed with status code: {response.StatusCode}");
                }
            }
        }

        private void Validate_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.UserUID))
            {
                uid = Properties.Settings.Default.UserUID;
                setData(uid);
            }
            else
            {
                Load_Validate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserUID = string.Empty;
            Properties.Settings.Default.Save();
            MessageBox.Show("Logged out!", "Logout Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
