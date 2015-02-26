using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
enum conditionManager {waiting_for_task, waiting_for_proc, stopped}



namespace ProcessorsSimulator
{
    class Manager
    {
        public Manager()
        {
            taskQueue = new Queue<Task>();
            processors = new List<Processor>();
            processorsThreads = new Thread[5];
            CreateProcessors();
            CreateGenerator();
            CreateGeneratorThread();
            CreateManageProcessors();
            generator.WorkDone += new EventHandler(OnWorkDone);
            this.ProcessorsWorkDone += OnProcessorsWorkDone;
            queueMutex = new Mutex();
            Debug.Print("Manager initialized");
        }

        public Mutex queueMutex;
        public conditionManager condition;
        public Queue<Task> taskQueue;
        public Generator generator;
        public List<Processor> processors;
        public Thread generatorThread;
        private Thread processorManager;
        private Thread[] processorsThreads;

        public event EventHandler ProcessorsWorkDone;
        public event EventHandler QueueModified;
        public event EventHandler SendTaskToProcessor;

        public void Manage()
        {
            StartProcessors();
            StartGenerator();
            StartManageProcessors();    
        }
        private void GetTask(Task task)
        {
            queueMutex.WaitOne();
            taskQueue.Enqueue(task);
            Debug.Print(String.Format("Task (id={0}, operationsAmount={1}, supportedProcessors={2}) is added to queue", 
                                task.id.ToString(), task.operationsAmont.ToString(), task.getSupportedProcessors()));
            if (QueueModified != null) QueueModified(this, null);
            queueMutex.ReleaseMutex();
        }
        private void OnWorkDone(object sender, EventArgs e)
        {
            //generatorThread.Abort();
            //processorManager.Abort();
            //AbortProcessors();
            Debug.Print("Generator work is done.");        
            CreateGeneratorThread();   
        }
        //
        // reload threads when they finish work !!Attention: generator might finish his work before processors
        //
        private void OnProcessorsWorkDone(object sender, EventArgs e) 
        {
            queueMutex.WaitOne();
            Debug.Print("Processors work is done.");
            Debug.Print("Queue count = " + taskQueue.Count().ToString());
            taskQueue = new Queue<Task>(); // reload
            queueMutex.ReleaseMutex();
            processors = new List<Processor>();
            processorsThreads = new Thread[5];
            CreateProcessors(); 
            CreateManageProcessors();
        }
        private void CreateGenerator()
        {
            generator = new Generator();
            generator.TaskGenerated += new Generator.TaskGeneratedHandler(GetTask);
        }
        public void CreateGeneratorThread()
        {
            generatorThread = new Thread(new
                ThreadStart(generator.GenerateTasks));
            generatorThread.Name = "Generator";
        }
        private void StartGenerator()
        {
            generatorThread.Start();
            Debug.Print("Generator thread started");
        }
        private void CreateProcessors() // filling processors
        {
            for(int i = 0; i < 5; i++)
            {
                Processor currentProc = new Processor();
                currentProc.id = i;
                processors.Add(currentProc);
                Thread currentThread = new Thread(new ThreadStart(currentProc.DoWork));
                currentThread.Name = "Processor" + (i + 1).ToString();
                if (currentThread != null)
                    processorsThreads[i] = currentThread;
            }
        }
        private void StartProcessors()
        {
            for (int i = 0; i < processorsThreads.Count(); i++)
            {
                processorsThreads[i].Start();
                Debug.Print("Processor" + (i + 1).ToString() + " thread started");
            }
        }
        private void AbortProcessors()
        {
            for (int i = 0; i < processorsThreads.Count(); i++)
            {
                processorsThreads[i].Abort();
            }
        }
        private void CreateManageProcessors()
        {
            processorManager = new Thread(new
                ThreadStart(ManageProcessors));
            processorManager.Name = "Manager";
        }
        private void StartManageProcessors()
        {
            processorManager.Start(); // start managing
            Debug.Print("Manager thread started");
        }
        private void ManageProcessors() // if task_queue has tasks - give them to processors
        {
            while(true)
            {
                queueMutex.WaitOne();
                if (taskQueue.Count != 0)
                {
                    Thread.Sleep(50); // extra time for displaying queue (only for those tasks, which is send to processors immediately)
                    Task currentTask = taskQueue.Peek(); // peeks first elem in queue
                    for(int i = 0; i < processors.Count; i++)
                    {
                        if (processors[i].condition == processor_condition.waitingForTask && taskQueue.Count != 0)
                        {
                            currentTask = taskQueue.Dequeue(); // return first elem and delete it
                            if (QueueModified != null) QueueModified(this, null);
                            processors[i].currentTask = currentTask;
                            Debug.Print(String.Format("Manager sends task (id={0}, operationsAmount={1}, supportedProcessors={2}) to Processor{3}",
                                                    currentTask.id.ToString(), currentTask.operationsAmont.ToString(), 
                                                    currentTask.getSupportedProcessors(), (i + 1).ToString()));
                            if (SendTaskToProcessor != null) SendTaskToProcessor(this, null);
                            processors[i].condition = processor_condition.processing;
                        }             
                    }
                }
                else // queue is empty
                {
                    if (generator.currrentWorkingTime <= 0) // if generator stops working
                    {
                        if (processors.All(x => x.condition == processor_condition.waitingForTask)) // checking for all tasks processed
                            if (ProcessorsWorkDone != null)
                            {
                                ProcessorsWorkDone(this, null);
                                queueMutex.ReleaseMutex();
                                break;
                            }
                            else Thread.Sleep(30); 
                    }         
                }
                queueMutex.ReleaseMutex();
            }
        }
    }
}
