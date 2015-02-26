using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

enum processor_condition {waitingForTask, processing}

namespace ProcessorsSimulator
{
    class Processor
    {
        public Processor()
        {
            power = 100; // default
            condition = processor_condition.waitingForTask;
            currentTask = null; 
        }
        public int id;
        public int power { get; set; } // n operations per milisecond
        public processor_condition condition { get; set; }
        public Task currentTask { get; set; }

        public delegate void NewProcessStartedHandler(int id, int maximum);
        public event NewProcessStartedHandler NewProcessStarted;
        public delegate void ProgressChangedHandler(int id, int progress);
        public event ProgressChangedHandler ProgressChanged;

        public void DoWork()
        {
            while(true)
            {
                if (condition == processor_condition.processing && currentTask != null)
                {
                    double processingTime = 10000;
                    if (NewProcessStarted != null) NewProcessStarted(this.id, (int)Math.Round(processingTime, MidpointRounding.AwayFromZero));

                    Debug.Print("Processing task (operationsAmount=" + currentTask.operationsAmont.ToString() + 
                                ", supportedProcessors=" + currentTask.getSupportedProcessors() + ")");
                    for (int i = 0; i < processingTime; i += (int) Math.Round(processingTime / 1000, MidpointRounding.AwayFromZero)) // TODO
                    {
                        if (ProgressChanged != null) ProgressChanged(this.id, i);
                        Thread.Sleep(10);
                    }
                    condition = processor_condition.waitingForTask; // work done, processor is free
                }
                else
                {
                    Thread.Sleep(50);
                }
                    
            } 
        }
    }
}
