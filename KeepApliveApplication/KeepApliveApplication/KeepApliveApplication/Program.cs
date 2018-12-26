using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace KeepApliveApplication
{
    class Program
    {

        private string interval;
        private string appname;
        private bool connection = false;

        private System.Timers.Timer aTimer;

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            Console.WriteLine("SetTimer");
            int intervalLong = int.Parse(interval);
            intervalLong /= 2;
            aTimer = new System.Timers.Timer(intervalLong);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            new Task(() => { ExecuteApplication(appname, interval); }).Start();
        }

        public Program(string appname, string interval)
        {
            this.appname = appname;
            this.interval = interval;
            //Start connection
            new Task(() => { RegistrateApplication(appname, interval);}).Start();
            while (!connection)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine("");

            SetTimer();

            /*
            aTimer.Stop();
            aTimer.Dispose();*/
            
            while(true)
            {
                Thread.Sleep(2000);
                Console.Write(".");
            }
        }

        async Task ExecuteApplication(string appname, string interval)
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(10000);
            string url = "https://licencesmanager.herokuapp.com/keep_alive/" + this.appname + "/keep_alive";
            await client.GetStringAsync(url);
            Console.WriteLine();
            Console.WriteLine("Processed");
        }

        async Task RegistrateApplication(string appname, string interval)
        {
            HttpClient client = new HttpClient();
            string url = "https://licencesmanager.herokuapp.com/keep_alive/" + appname + "/keep_alive?interval=" + interval;
            await client.GetStringAsync(url);
            connection = true;
        }

        static void Main(string[] args)
        {
            /*
             * Params:
             * application.exe --inteval 20 --appname <App Name>
             */
            string interval = "30000";
            string appname = "app1";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--interval")
                {
                    interval = args[i + 1];
                }
                if (args[i] == "--appname")
                {
                    appname = args[i + 1];
                }
            }
            Console.WriteLine("Interval: " + interval);
            Console.WriteLine("Appname: " + appname);
            Console.Write("Starting appication process");

            Program mainProgram = new Program(appname, interval);
        }
    }
}
