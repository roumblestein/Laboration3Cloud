using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebRole1
{
    public partial class Report : System.Web.UI.Page
    {

        //MONGO connection string
        private string connection = "mongodb://localhost:27017/PaymentService?strict=false";


        protected  void Page_Load (object sender, EventArgs e)
        {

            

            //MONGO client setup
            MongoClient dbClient = new MongoClient(connection);
            var database = dbClient.GetDatabase("PaymentService");
            var getPaymentInformation = database.GetCollection<BsonDocument>("PaymentInformation");

            var documents = getPaymentInformation.AsQueryable();

            //Create stringbuilder for html display
           StringBuilder builder = new StringBuilder();
            int i = 0;
           foreach (BsonDocument doc in documents)
            {
                //Retrieve every object for customer data
                builder.Append("<br/><br/>Iteration: " + i);
                builder.Append("<br/>Card Holder: " + doc.GetElement("cardholder").Value.ToString());
                builder.Append("<br/>Card Number: " + doc.GetElement("cardnumber").Value.ToString());
                builder.Append("<br/>Card Experation Date: " + doc.GetElement("expiredate").Value.ToString());
                builder.Append("<br/>balance: " + "1000");

                i++;
            }

            IterationText.Text = builder.ToString();


            StringBuilder onebuild = new StringBuilder();

            //retrieve one specific object for customer data
            foreach (BsonDocument doc in documents)
            { 
                if (doc.GetElement("cardholder").Value.ToString().Equals("martin")){

                    onebuild.Append("<br/><br/>Specific Card Holder");
                    onebuild.Append("<br/>Card Holder: " + doc.GetElement("cardholder").Value.ToString());
                    onebuild.Append("<br/>Card Number: " + doc.GetElement("cardnumber").Value.ToString());
                    onebuild.Append("<br/>Card Experation Date: " + doc.GetElement("expiredate").Value.ToString());
                    onebuild.Append("<br/>balance: " + "1000");
                }
                
            }

            specificholder.Text = onebuild.ToString();

        }
    }
}