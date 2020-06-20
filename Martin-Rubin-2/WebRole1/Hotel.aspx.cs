using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebRole1
{
    public partial class Hotel : System.Web.UI.Page
    {
        private string accountName = "laboration3cloud";
        private string accountKey = "8j2BEqc3LY4xRjFM3UxkPjjmuZHZqI714dft0cqEdwaxzJT47xD5sYN38tYCkGJof4exRQdu+HGcUj6WlZbvYg==";



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


                if (Session["travelers"] != null)
                {

                    TextBox1.Text = Session["travelers"].ToString();

                }
                if (Session["nights"] != null)
                {

                    TextBox2.Text = Session["nights"].ToString();

                }
                if (Session["hotelSeniors"] != null)
                {

                    TextBox3.Text = Session["hotelSeniors"].ToString();

                }
                if (Session["name"] != null)
                {

                    TextBox4.Text = Session["name"].ToString();

                }
                if (Session["single"] != null)
                {

                    singelRoom.Checked = (bool)Session["single"];

                }
                if (Session["double"] != null)
                {

                    doubleRoom.Checked = (bool)Session["double"];

                }
                if (Session["hotelprice"] != null)
                {

                    TextBox5.Text = Session["hotelprice"].ToString();

                }
            }
        }

        protected void BtnNext_Click(object sender, EventArgs e)
        {


            Session["travelers"] = TextBox1.Text;
            Session["nights"] = TextBox2.Text;
            Session["hotelSeniors"] = TextBox3.Text;
            Session["name"] = TextBox4.Text;
            Session["single"] = singelRoom.Checked;
            Session["double"] = doubleRoom.Checked;
            Session["hotelprice"] = TextBox5.Text;



            Response.Redirect("Payment.aspx");

        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            Session["travelers"] = TextBox1.Text;
            Session["nights"] = TextBox2.Text;
            Session["hotelSeniors"] = TextBox3.Text;
            Session["name"] = TextBox4.Text;
            Session["single"] = singelRoom.Checked;
            Session["double"] = doubleRoom.Checked;
            Session["hotelprice"] = TextBox5.Text;

            Response.Redirect("Flight.aspx");

        }

        protected void BtnPost_Click(object sender, EventArgs e)
        {

            JObject jObject = new JObject();

            jObject.Add("singel", singelRoom.Checked);
            jObject.Add("double", doubleRoom.Checked);
            jObject.Add("travelers", TextBox1.Text);
            jObject.Add("nights", TextBox2.Text);
            jObject.Add("hotelSeniors", TextBox3.Text);
            jObject.Add("name", TextBox4.Text);

            try
            {

                StorageCredentials creds = new StorageCredentials(accountName, accountKey);     //Account and key are already initialized
                CloudStorageAccount storageAccount = new CloudStorageAccount(creds, useHttps: true);

                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient(); //Create an instance of a Cloud QueueClient object to access your queue in the storage

                // Retrieve a reference to a specific queue
                CloudQueue queue = queueClient.GetQueueReference("hotelrequestqueue");

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
                CloudQueue responsequeue = queueClient.GetQueueReference("hotelresponsequeue");
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
    }
}