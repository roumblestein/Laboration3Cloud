using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure;
using System.Diagnostics;
using System.Data.SqlClient;

namespace WebRole1
{
    public partial class Flight : System.Web.UI.Page
    {
        
        private string accountName = "laboration3cloud";
        private string accountKey = "8j2BEqc3LY4xRjFM3UxkPjjmuZHZqI714dft0cqEdwaxzJT47xD5sYN38tYCkGJof4exRQdu+HGcUj6WlZbvYg==";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            
            if (!IsPostBack) { 
            
            
            if(Session["from"] != null)
            {
                
                fromText.Text = Session["from"].ToString();
                
            }
            if (Session["to"] != null)
            {

                toText.Text = Session["to"].ToString();

            }
            if (Session["month"] != null)
            {

                monthText.Text = Session["month"].ToString();

            }
            if (Session["infants"] != null)
            {

                TextBox1.Text = Session["infants"].ToString();

            }
            if (Session["children"] != null)
            {

                TextBox2.Text = Session["children"].ToString();

            }
            if (Session["adults"] != null)
            {

                TextBox3.Text = Session["adults"].ToString();

            }
            if (Session["seniors"] != null)
            {

                TextBox4.Text = Session["seniors"].ToString();

            }
            if (Session["price"] != null)
            {

                TextBox5.Text = Session["price"].ToString();

            }
            }
        }


        protected void BtnNext_Click(object sender, EventArgs e)
        {
            //Create Session
            Session["from"] = fromText.Text;
            Session["to"] = toText.Text;
            Session["month"] = monthText.Text;
            Session["infants"] = TextBox1.Text;
            Session["children"] = TextBox2.Text;
            Session["adults"] = TextBox3.Text;
            Session["seniors"] = TextBox4.Text;
            Session["price"] = TextBox5.Text;



            if ((bool)Session["checkBox"] == true)
            {
              Response.Redirect("Hotel.aspx");
            }else
            {
                Response.Redirect("Payment.aspx");
            }

            

        }

        protected void BtnPrice_Click(object sender, EventArgs e)
        {


            
            //create json object that sends to que
            JObject jObject = new JObject();
            
            jObject.Add("from", fromText.Text);
            jObject.Add("to", toText.Text);
            jObject.Add("infants", TextBox1.Text);
            jObject.Add("children", TextBox2.Text);
            jObject.Add("adults", TextBox3.Text);
            jObject.Add("seniors", TextBox4.Text);

            try
            {

                StorageCredentials creds = new StorageCredentials(accountName, accountKey);     //Account and key are already initialized
                CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient(); //Create an instance of a Cloud QueueClient object to access your queue in the storage

                // Retrieve a reference to a specific queue
                CloudQueue queue = queueClient.GetQueueReference("flightrequestqueue");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                //remove any existing messages (just in case)
                queue.Clear();

                // Create a message and add it to the queue.
                CloudQueueMessage message = new CloudQueueMessage(jObject.ToString());
                queue.AddMessage(message);


                //Show in the console that some activity is going on in the Web Role
                Debug.WriteLine("Message '" + message + "'stored in Queue");
            }
            catch (Exception ee) {; }



            try
            {
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                CloudQueue responsequeue = queueClient.GetQueueReference("flightresponsequeue");
                try
                {
                    // Create the queue if it doesn't already exist
                    responsequeue.CreateIfNotExists();

                    // retrieve the next message
                    CloudQueueMessage readMessage = responsequeue.GetMessage();


                    // Display message (populate the textbox with the message you just retrieved.
                    TextBox5.Text = readMessage.AsString;

                    //Delete the message just read to avoid reading it over and over again
                    responsequeue.DeleteMessage(responsequeue.GetMessage());


                }
                catch (Exception ee) { Debug.WriteLine("Problem reading from queue"); }
            }
            catch (Exception eee) {; }

        }


       

        protected void BtnBack_Click(object sender, EventArgs e)
        {

            //Create Session
            Session["from"] = fromText.Text;
            Session["to"] = toText.Text;
            Session["month"] = monthText.Text;
            Session["infants"] = TextBox1.Text;
            Session["children"] = TextBox2.Text;
            Session["adults"] = TextBox3.Text;
            Session["seniors"] = TextBox4.Text;
            Session["price"] = TextBox5.Text;
            Session["passenger"] = TextBox4.Text;
            Session["price"] = TextBox5.Text;


            Response.Redirect("Default.aspx");

        }

    }
}