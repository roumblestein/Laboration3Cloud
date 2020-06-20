using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private string accountName = "laboration3cloud";
        private string accountKey = "8j2BEqc3LY4xRjFM3UxkPjjmuZHZqI714dft0cqEdwaxzJT47xD5sYN38tYCkGJof4exRQdu+HGcUj6WlZbvYg==";     // zPie75n + Wcbwr19brs3LNC05ldiv4sDAPLB6ib4 / eVLsYBc20iSULTvRfVlmI2MXBC2SOf1MCaDHv2cihuu4fw ==";     // zPie75n+Wcbwr19bferrs3LNCdiv4sDAPsdLB6ib4/eVLsYBc20iSULTvRfVlmI2MXBC2SOf1MCaDHv2cihuu4fw";   // Write your Azure storage account key here "YOUR_ACCOUNT_KEY";     
        private StorageCredentials creds;
        private CloudStorageAccount storageAccount;
        private CloudQueueClient queueClient;
        private CloudQueue inqueue, outqueue;
        private CloudQueueMessage inMessage, outMessage;

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
            inqueue = queueClient.GetQueueReference("flightrequestqueue");

            // Create the queue if it doesn't already exist
            inqueue.CreateIfNotExists();

            // Retrieve a reference to a queue
            outqueue = queueClient.GetQueueReference("flightresponsequeue");

            // Create the queue if it doesn't already exist
            outqueue.CreateIfNotExists();
        }

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

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

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            initQueue();        //call the queue initialization method
            while (!cancellationToken.IsCancellationRequested)
            {       
                // Async dequeue (read) the message
                inMessage = await inqueue.GetMessageAsync();    //not an optimal way to retrieve a message from a queue, but works

                if (inMessage != null)
                {
                    //create json object
                    JObject jObject = JObject.Parse(inMessage.AsString);
                 string bigs = getPrice(jObject).ToString();  
                await inqueue.DeleteMessageAsync(inMessage);

                // Create a message and add it to the queue.
                outMessage = new CloudQueueMessage(bigs);
                outqueue.AddMessage(outMessage);
                }
               
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }



        //Method to send lats and longs to calc
        private string getPrice(JObject jObject)
        {
            double[] coords = new double[5];
            string[] stringMessage = new string[8];

            if (jObject.GetValue("from").ToString().Equals("STO"))
            {
                coords[1] = 59.6519;
                coords[2] = 17.9186;
            }
            if (jObject.GetValue("from").ToString().Equals("CPH"))
            {
                coords[1] = 55.6181;
                coords[2] = 12.6561;
            }
            if (jObject.GetValue("from").ToString().Equals("CDG"))
            {
                coords[1] = 49.0097;
                coords[2] = 2.5478;
            }
            if (jObject.GetValue("from").ToString().Equals("LHR"))
            {
                coords[1] = 31.5497;
                coords[2] = 74.3436;
            }
            if (jObject.GetValue("from").ToString().Equals("FRA"))
            {
                coords[1] = 50.1167;
                coords[2] = 8.6833;
            }


            if (jObject.GetValue("to").ToString().Equals("STO"))
            {
                coords[3] = 59.6519;
                coords[4] = 17.9186;
            }
            if (jObject.GetValue("to").ToString().Equals("CPH"))
            {
                coords[3] = 55.6181;
                coords[4] = 12.6561;
            }
            if (jObject.GetValue("to").ToString().Equals("CDG"))
            {
                coords[3] = 49.0097;
                coords[4] = 2.5478;
            }
            if (jObject.GetValue("to").ToString().Equals("LHR"))
            {
                coords[3] = 31.5497;
                coords[4] = 74.3436;
            }
            if (jObject.GetValue("to").ToString().Equals("FRA"))
            {
                coords[3] = 50.1167;
                coords[4] = 8.6833;
            }


            stringMessage[1] = jObject.GetValue("from").ToString();

            if (jObject.GetValue("infants").ToString() != null)
            {
                stringMessage[3] = jObject.GetValue("infants").ToString();
            }
            else { stringMessage[3] = 0.ToString(); }
            if (jObject.GetValue("children").ToString() != null)
            {
                stringMessage[4] = jObject.GetValue("children").ToString();
            }
            else { stringMessage[4] = 0.ToString(); }
            if (jObject.GetValue("adults").ToString() != null)
            {
                stringMessage[5] = jObject.GetValue("adults").ToString();
            }
            else { stringMessage[5] = 0.ToString(); }
            if (jObject.GetValue("seniors").ToString() != null)
            {
                stringMessage[6] = jObject.GetValue("seniors").ToString();
            }
            else { stringMessage[6] = 0.ToString(); }

            return calculatePrice(coords, stringMessage);
        }



        private string calculatePrice(double[] coords, string[] splitMessage)
        {
            // Distance Calculator
            double lat_1 = coords[1] * (Math.PI / 180);
            double lon_1 = coords[2] * (Math.PI / 180);

            double lat_2 = coords[3] * (Math.PI / 180);
            double lon_2 = coords[4] * (Math.PI / 180);

            const Double r = 6376.5;

            double x_1 = r * Math.Sin(lon_1) * Math.Cos(lat_1);
            double y_1 = r * Math.Sin(lon_1) * Math.Sin(lat_1);
            double z_1 = r * Math.Cos(lon_1);

            double x_2 = r * Math.Sin(lon_2) * Math.Cos(lat_2);
            double y_2 = r * Math.Sin(lon_2) * Math.Sin(lat_2);
            double z_2 = r * Math.Cos(lon_2);

            double flightDistance = Math.Sqrt((x_2 - x_1) * (x_2 - x_1) + (y_2 - y_1) *
                                    (y_2 - y_1) + (z_2 - z_1) * (z_2 - z_1));

            //flight destinations
            double STO = 0.234;
            double CPH = 0.2554;
            double CDG = 0.2255;
            double LHR = 0.2300;
            double FRA = 0.2400;

            double br = 0;
            string brCity = splitMessage[1];

            if (brCity.Equals("STO"))
            {
                br = STO;
            }
            else if (brCity.Equals("CPH"))
            {
                br = CPH;
            }
            else if (brCity.Equals("CDG"))
            {
                br = CDG;
            }
            else if (brCity.Equals("LHR"))
            {
                br = LHR;
            }
            else if (brCity.Equals("FRA"))
            {
                br = FRA;
            }


            

            int numberOfInfants = int.Parse(splitMessage[3]);
            int numberOfChildren = int.Parse(splitMessage[4]);
            int numberOfAdults = int.Parse(splitMessage[5]);
            int numberOfSeniors = int.Parse(splitMessage[6]);

            double infantsPrice = br * flightDistance * (1 - 0.9) * numberOfInfants;
            double childrenPrice = br * flightDistance * (1 - 0.33) * numberOfChildren;
            double adultsPrice = br * flightDistance * (1 - 0) * numberOfAdults;
            double seniorsPrice = br * flightDistance * (1 - 0.25) * numberOfSeniors;

            double fare = infantsPrice + childrenPrice + adultsPrice + seniorsPrice;

            int str = (int)fare;

            return str.ToString();
        }




    }
}
