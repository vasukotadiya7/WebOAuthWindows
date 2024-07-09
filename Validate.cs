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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
