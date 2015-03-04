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
            power = 50; // default
            condition = processor_condition.waitingForTask;
            currentTask = null;
            executedTasks = new List<Task>();
        }

        public Processor(int pow)
        {
            power = pow; // default
            condition = processor_condition.waitingForTask;
            currentTask = null;
            executedTasks = new List<Task>();
        }
        public int id;
        public int power { get; set; } // n operations per milisecond
        public processor_condition condition { get; set; }
        public Task currentTask { get; set; }

        public delegate void ProcessEndedHandler(int id , processor_condition cond, int operationsAmount);
        public event ProcessEndedHandler ProcessEnded;
        public delegate void NewProcessStartedHandler(int id, int maximum, Task currentTask, processor_condition cond);
        public event NewProcessStartedHandler NewProcessStarted;
        public delegate void ProgressChangedHandler(int id, int progress);
        public event ProgressChangedHandler ProgressChanged;
        private List<Task> executedTasks;

        public void DoWork()
        {
            while(true)
            {
                if (condition == processor_condition.processing && currentTask != null)
                {
                    if (executedTasks.Contains(currentTask))
                        Debug.Print("THIS TASK ALREADY EXECUTED!! WTF");
                    else
                        executedTasks.Add(currentTask);
                    double processingTime = currentTask.operationsAmont / power;
                    int maximumTime = (int)Math.Round(processingTime, MidpointRounding.ToEven);
                    if (NewProcessStarted != null) NewProcessStarted(this.id, maximumTime, currentTask, condition);

                    Debug.Print("Processing task (operationsAmount=" + currentTask.operationsAmont.ToString() + 
                                ", supportedProcessors=" + currentTask.getSupportedProcessors() + ")");
                    for (int i = 0; i < maximumTime; i += 1) // TODO
                    {
                        if (ProgressChanged != null) ProgressChanged(this.id, i);
                        Thread.Sleep(20);
                    }
                    condition = processor_condition.waitingForTask; // work done, processor is free
                    if (ProcessEnded != null)
                    {
                        ProcessEnded(this.id , condition, currentTask.operationsAmont);
                    }
                }
                else
                {
                    Thread.Sleep(20);
                }
                    
            } 
        }

      
    }
}
