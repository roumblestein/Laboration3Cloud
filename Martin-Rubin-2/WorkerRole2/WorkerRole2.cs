using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace WorkerRole2
{
    public class WorkerRole2 : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string accountName = "laboration3cloud";
        private string accountKey = "8j2BEqc3LY4xRjFM3UxkPjjmuZHZqI714dft0cqEdwaxzJT47xD5sYN38tYCkGJof4exRQdu+HGcUj6WlZbvYg==";    // zPie75n + Wcbwr19brs3LNC05ldiv4sDAPLB6ib4 / eVLsYBc20iSULTvRfVlmI2MXBC2SOf1MCaDHv2cihuu4fw ==";     // zPie75n+Wcbwr19bferrs3LNCdiv4sDAPsdLB6ib4/eVLsYBc20iSULTvRfVlmI2MXBC2SOf1MCaDHv2cihuu4fw";   // Write your Azure storage account key here "YOUR_ACCOUNT_KEY";     
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueue payqueue;
        private CloudQueue transactionqueue;
        private CloudQueueMessage inMessage, outMessage;
        private CloudQueueMessage payMessage;
        private CloudQueueMessage transactionMessage;

        private string connection = "mongodb://localhost:27017/PaymentService?strict=false";



        //the following method is called at the start of the worker role to get instances of incoming and outgoing queues 
        private void initQueue()
        {
            // Retrieve storage account from connection string
            //  CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            //      CloudConfigurationManager.GetSetting("Setting2"));

            creds = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(creds, useHttps: true);

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            inqueue = queueClient.GetQueueReference("hotelrequestqueue");
            payqueue = queueClient.GetQueueReference("mongodatabase");
            transactionqueue = queueClient.GetQueueReference("mongotransaction");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();
            payqueue.CreateIfNotExists();
            transactionqueue.CreateIfNotExists();
            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("hotelresponsequeue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole2 is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            catch (Exception e) {; }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole2 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole2 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole2 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            initQueue();        //call the queue initialization method
            while (!cancellationToken.IsCancellationRequested)
            {
                // Async dequeue (read) the message
                inMessage = await inqueue.GetMessageAsync();    //not an optimal way to retrieve a message from a queue, but works
                payMessage = await payqueue.GetMessageAsync();
                transactionMessage = await transactionqueue.GetMessageAsync();
                if (inMessage != null)
                {
                          //create json object
                    JObject jObject = JObject.Parse(inMessage.AsString);
                    string bigs = calculatePrice(jObject);
                    await inqueue.DeleteMessageAsync(inMessage);

                    // Create a message and add it to the queue.
                    outMessage = new CloudQueueMessage(bigs);
                    outqueue.AddMessage(outMessage);
                }

                //Mongo insertion with queues

                if (payMessage != null)
                {
                    JObject payInfo = JObject.Parse(payMessage.AsString);
                    BsonDocument doc = BsonDocument.Parse(payInfo.ToString());

                    //MONGO insertion
                    MongoClient dbClient = new MongoClient(connection);
                    var database = dbClient.GetDatabase("PaymentService");
                    var getPaymentInformation = database.GetCollection<BsonDocument>("PaymentInformation");
                    //insert
                    await getPaymentInformation.InsertOneAsync(doc);

                    payqueue.DeleteMessage(payMessage);

                }

                if (transactionMessage != null)
                {
                    JObject transInfo = JObject.Parse(transactionMessage.AsString);
                    BsonDocument docu = BsonDocument.Parse(transInfo.ToString());

                    //MONGO insertion
                    MongoClient dbClient = new MongoClient(connection);
                    var database = dbClient.GetDatabase("PaymentService");
                    var getPaymentInformation = database.GetCollection<BsonDocument>("Transactions");
                    //insert
                    await getPaymentInformation.InsertOneAsync(docu);

                    transactionqueue.DeleteMessage(transactionMessage);

                }


                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
        //hotel reservation calculator
        private string calculatePrice(JObject jObject)
        {

            int totalPrice = 0;

            if((bool)jObject.GetValue("singel") == true)
            {
                int travelers = int.Parse(jObject.GetValue("travelers").ToString());
                int seniors = int.Parse(jObject.GetValue("hotelSeniors").ToString());
                int nights = int.Parse(jObject.GetValue("nights").ToString());
                totalPrice += travelers * 600 * nights;
                totalPrice += seniors * 300 * nights;

            }
            else
            {
                int travelers = int.Parse(jObject.GetValue("travelers").ToString());
                int seniors = int.Parse(jObject.GetValue("hotelSeniors").ToString());
                int nights = int.Parse(jObject.GetValue("nights").ToString());
                totalPrice += travelers * 900 * nights;
                totalPrice += seniors * 450 * nights;
            }


            return totalPrice.ToString();
        }

    }
}
