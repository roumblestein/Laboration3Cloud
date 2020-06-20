using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole1
{
    public partial class Payment : System.Web.UI.Page
    {
        //queue connection data
        private string accountName = "laboration3cloud";
        private string accountKey = "8j2BEqc3LY4xRjFM3UxkPjjmuZHZqI714dft0cqEdwaxzJT47xD5sYN38tYCkGJof4exRQdu+HGcUj6WlZbvYg==";
        private SqlConnection conn;
        SqlCommand cmd;

        protected void Page_Load(object sender, EventArgs e)
        {   //SQL connection string
            {
                //           conn = new SqlConnection("Server=tcp:dawitest.database.windows.net;Database=mynewdb;User ID=dawitmen;Password=DA376c2020;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                conn = new SqlConnection("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = newdatabase; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
            }

            if (!IsPostBack)
            {
                //display total price
          //      totalCost.Text = (int.Parse(Session["price"].ToString()) + int.Parse(Session["hotelPrice"].ToString())).ToString();

                if (Session["creditCard"] != null)
                {

                    cardNumber.Text = Session["creditCard"].ToString();

                }
                if (Session["payName"] != null)
                {

                    nameText.Text = Session["payName"].ToString();

                }
                
            }
        }

        protected void BtnPay_Click(object sender, EventArgs e)
        {
            //Create data for database
            DateTime date = DateTime.Now;
            string sqlFormattedDate = date.ToString("yyyy-MM-dd HH:mm:ss.fff");


            JObject jObject = new JObject();

            jObject.Add("cardnumber", cardNumber.Text );
            jObject.Add("cardholder", nameText.Text);
            jObject.Add("expiredate", expires.Text );
            jObject.Add("date", date);
            jObject.Add("price", totalCost.Text);

            JObject transObject = new JObject();

            transObject.Add("date", date);
            transObject.Add("cardnumber", cardNumber.Text);
            transObject.Add("price", totalCost.Text);


            try
            {

                StorageCredentials creds = new StorageCredentials(accountName, accountKey);     //Account and key are already initialized
                CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient(); //Create an instance of a Cloud QueueClient object to access your queue in the storage

                // Retrieve a reference to a specific queue
                //Create ques for backend database storage
                CloudQueue queue = queueClient.GetQueueReference("mongodatabase");
                CloudQueue transqueue = queueClient.GetQueueReference("mongotransaction");
                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();
                transqueue.CreateIfNotExists();
                //remove any existing messages (just in case)
                queue.Clear();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage(jObject.ToString());
                queue.AddMessage(message);

                CloudQueueMessage transmessage = new CloudQueueMessage(transObject.ToString());
                transqueue.AddMessage(transmessage);
                //Show in the console that some activity is going on in the Web Role
                Debug.WriteLine("Message '" + message + "'stored in Queue");
            }
            catch (Exception ee) {; }






            //insertion into the SQL database flight info
            conn.Open();
            cmd = new SqlCommand(@"INSERT INTO dbo.Flights (passenger, passport, date, price)
                                  VALUES (@passenger, @passport, @date, @price)", conn);
            cmd.Parameters.Add(new SqlParameter("passenger", nameText.Text));
            cmd.Parameters.Add(new SqlParameter("passport", passportNr.Text));
            cmd.Parameters.Add(new SqlParameter("date", date));
            cmd.Parameters.Add(new SqlParameter("price", Session["price"].ToString()));
            cmd.ExecuteNonQuery();
            conn.Close();


            //remove session
            Session.Abandon();
            Response.Redirect("Default.aspx");

        }


        protected void BtnBack_Click(object sender, EventArgs e)
        {
            Session["creditCard"] = cardNumber.Text;
            Session["payName"] = nameText.Text;
            if ((bool)Session["checkBox"] == true)
            {
                Response.Redirect("Hotel.aspx");
            }
            else
            {
                Response.Redirect("Flight.aspx");
            }


        }
    }
}