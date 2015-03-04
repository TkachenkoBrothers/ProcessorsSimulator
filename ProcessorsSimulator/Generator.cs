using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ProcessorsSimulator
{ 
    class Generator
    {
        public Generator()
        {
            sleepTime = 200; //default
            taskComplexityScope = new int[2] { 3000, 10000 }; //default
            workingTime = 10000;
        }

        public Generator(int tasksAmount ,  int Scope1, int Scope2, int _workingTime )
        {
            tasksAmount = tasksAmount;
            taskComplexityScope = new int[2] { Scope1, Scope2 };
            workingTime = _workingTime;
        }
        public int sleepTime { get; set; }
        public int workingTime { get; set; }
        public int currrentWorkingTime { get; set; }
        public int[] taskComplexityScope { get; set; }
        public delegate void TaskGeneratedHandler(Task task);
        public event TaskGeneratedHandler TaskGenerated;
        public event EventHandler WorkDone;
        public void GenerateTasks()
        {
            currrentWorkingTime = workingTime;
            Random random = new Random();
            int id = 0;

            while (currrentWorkingTime > 0)
            {
                Debug.Print("Working time: " + currrentWorkingTime.ToString());
                Thread.Sleep(sleepTime); // simulate waiting for task (create task every n miliseconds)
                currrentWorkingTime -= sleepTime;
                Task currentTask = new Task();

                currentTask.id = id++;
                currentTask.operationsAmont = random.Next(taskComplexityScope[0], taskComplexityScope[1] + 1); // creates random in my scope range
                int randomProcessorsAmount = random.Next(1, 6); // random processors amount 1..5 
                //currentTask.supportedProcessors = new int[] { 1, 2, 3, 4, 5 };
                currentTask.supportedProcessors = new int[randomProcessorsAmount];
                for (int i = 0; i < randomProcessorsAmount; i++)
                {
                    int processorNumber = random.Next(1, 6);
                    if (currentTask.supportedProcessors != null || currentTask.supportedProcessors.Length != 0) // if array isn`t empty
                        while (currentTask.supportedProcessors.Contains(processorNumber)) // prevent number dublication
                        {
                            processorNumber = random.Next(1, 6);
                        }
                    currentTask.supportedProcessors[i] = processorNumber; // random processor number 
                }
                if (TaskGenerated != null) TaskGenerated(currentTask); // call GenerateTask event if smb subscribed 
            }
            if (WorkDone != null) WorkDone(this, null);
        }
    }
}
