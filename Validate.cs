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

namespace WebOAuth
{
    public partial class Validate : Form
    {
        public static string utoken;
        public Validate(string token)
        {
            InitializeComponent();
            utoken = token;
            MessageBox.Show("Validating Token");
        }

        private async void Validate_Load(object sender, EventArgs e)
        {
            string apiEndpoint = "http://localhost:5000/validateutoken";

            try
            {
                // Call the API to get the token with custom headers
                string response = await GetUserIDAsync(apiEndpoint);
                var json = JObject.Parse(response);

                string uid = json["uid"].ToString();
                long time = long.Parse(json["time"].ToString());
                long ctime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000;

                if (ctime>time)
                {
           

                    MessageBox.Show("Timed OUT Sorry Try again");
                }
                else
                {
                    /*   Firebase get data */

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
                        foreach (var kvp in user)
                        {
                        /*MessageBox.Show($"{kvp.Key}: {kvp.Value}");*/
                            if (kvp.Key == "ppurl")
                            {
                                setProfilePhoto(kvp.Value.ToString());
                            }
                            TextBox txtKey = new TextBox();
                            txtKey.ReadOnly = true;
                            txtKey.Text = kvp.Key;
                            txtKey.Location = new System.Drawing.Point(20, 20 + 30 * tableLayoutPanel1.Controls.Count); // Adjust Y position

                            TextBox txtValue = new TextBox();
                            txtValue.ReadOnly = true;
                            if (kvp.Key == "password")
                            {
                                txtValue.Text = "**********";
                            }
                            else
                            {

                            txtValue.Text = kvp.Value.ToString();
                            }
                            txtValue.Location = new System.Drawing.Point(120, 20 + 30 * tableLayoutPanel1.Controls.Count); // Adjust Y position

                            tableLayoutPanel1.Controls.Add(txtKey);
                            tableLayoutPanel1.Controls.Add(txtValue);

                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("User Data not found !");
                    }
                }
            }
            catch (Exception ex)
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

    }
}
