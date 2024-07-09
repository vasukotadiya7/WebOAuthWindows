using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace WebOAuth
{
    public partial class Form1 : Form
    {
        private Label lblToken;
        public Form1(string token)
        {
            InitializeComponent();
            lblToken = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(13, 13),
                Name = "lblToken",
                Size = new System.Drawing.Size(35, 13),
                TabIndex = 0,
                Text = "Token: " + token
            };
            this.Controls.Add(lblToken);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            string apiEndpoint = "http://localhost:5000/getdata";

            try
            {
                // Call the API to get the token with custom headers
                string response = await GetAuthTokenAsync(apiEndpoint);
                var json = JObject.Parse(response);

                string accessToken = json["accessToken"].ToString();
               /* string weboauthurl = json["weboauth_url"].ToString();*/
                string weboauthurl = "http://localhost:5173";

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Construct the URL with the token as a parameter
                    string urlWithToken = $"{weboauthurl}/{accessToken}";

                    // Open the URL in the default browser
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = urlWithToken,
                        UseShellExecute = true
                    });

                    MessageBox.Show("Browser opened with token.");
                }
                else
                {
                    MessageBox.Show("Failed to retrieve token.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task<string> GetAuthTokenAsync(string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                // Add custom headers
                client.DefaultRequestHeaders.Add("redirect-url", "myapp://callback");
               

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

        private void button2_Click(object sender, EventArgs e)
        {
           
            
        }
        public void UpdateToken(string token)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateToken), token);
                return;
            }
            lblToken.Text = "Token: " + token;

            this.Hide();
            Validate validate = new Validate(token);
            validate.Show();
        }
    }
}
