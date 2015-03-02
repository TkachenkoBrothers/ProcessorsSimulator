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
            InitializeTaskQueues();
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

        
        private Queue<Task>[] devisionQueue;
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

        private void InitializeTaskQueues() 
        {
            devisionQueue = new Queue<Task>[5];
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
                    Task currentTask = new Task();
                    if (taskQueue.Count != 0)
                        currentTask = taskQueue.Peek(); // peeks first elem in queue
                    for(int i = 0; i < processors.Count; i++)
                    {
                        bool supported = currentTask.supportedProcessors.Contains(processors[i].id + 1);
                        if (supported)
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


        private void FormOneProcTaskQueues()
        {
            foreach (var i in taskQueue)
            {
                if (i.supportedProcessors.Length == 1)
                {
                    switch (i.supportedProcessors[0])
                    {
                        case 1:
                            devisionQueue[0].Enqueue(i);
                            break;
                        case 2:
                            devisionQueue[1].Enqueue(i);
                            break;
                        case 3:
                            devisionQueue[2].Enqueue(i);
                            break;
                        case 4:
                            devisionQueue[3].Enqueue(i);
                            break;
                        case 5:
                            devisionQueue[4].Enqueue(i);
                            break;
                        default:
                            break;
                    }
                }   
            }
           
        }


        private void FormTwoProcTaskQueues() 
        {
            List<Task> t12 = new List<Task>();
            List<Task> t13 = new List<Task>();
            List<Task> t14 = new List<Task>();
            List<Task> t15 = new List<Task>();
            List<Task> t23 = new List<Task>();
            List<Task> t24 = new List<Task>();
            List<Task> t25 = new List<Task>();
            List<Task> t34 = new List<Task>();
            List<Task> t35 = new List<Task>();
            List<Task> t45 = new List<Task>();
            foreach (var i in taskQueue)
            {
                if (i.supportedProcessors.Length == 2)
                {
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2))
                    {
                        t12.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 3))
                    {
                        t13.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t14.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t15.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 3))
                    {
                        t23.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t24.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t25.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t34.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t35.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t45.Add(i);
                    }
                }
            }
                t12.Sort(); t13.Sort(); t14.Sort(); t15.Sort(); t23.Sort(); t24.Sort(); t25.Sort(); t34.Sort(); t35.Sort(); t45.Sort();
                t12.Reverse(); t13.Reverse(); t14.Reverse(); t15.Reverse(); t23.Reverse();
                t24.Reverse(); t25.Reverse(); t34.Reverse(); t35.Reverse(); t45.Reverse();

                foreach (var item in t12)
                {
                    if (devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power < devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power)
                    {
                        devisionQueue[0].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[1].Enqueue(item);
                    }
                }

                foreach (var item in t13)
                {
                    if (devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power < devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power)
                    {
                        devisionQueue[0].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[2].Enqueue(item);
                    }
                }

                foreach (var item in t14)
                {
                    if (devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power < devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power)
                    {
                        devisionQueue[0].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[3].Enqueue(item);
                    }
                }

                foreach (var item in t15)
                {
                    if (devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power < devisionQueue[4].Sum(p => p.operationsAmont) / processors[1].power)
                    {
                        devisionQueue[0].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[4].Enqueue(item);
                    }
                }


                foreach (var item in t23)
                {
                    if (devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power < devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power)
                    {
                        devisionQueue[1].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[2].Enqueue(item);
                    }
                }

                foreach (var item in t24)
                {
                    if (devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power < devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power)
                    {
                        devisionQueue[1].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[3].Enqueue(item);
                    }
                }

                foreach (var item in t25)
                {
                    if (devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power < devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power)
                    {
                        devisionQueue[1].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[4].Enqueue(item);
                    }
                }

                foreach (var item in t34)
                {
                    if (devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power < devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power)
                    {
                        devisionQueue[2].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[3].Enqueue(item);
                    }
                }

                foreach (var item in t35)
                {
                    if (devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power < devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power)
                    {
                        devisionQueue[2].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[4].Enqueue(item);
                    }
                }

                foreach (var item in t45)
                {
                    if (devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power < devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power)
                    {
                        devisionQueue[3].Enqueue(item);
                    }
                    else
                    {
                        devisionQueue[4].Enqueue(item);
                    }
                }
                
            
        }


        private void FormThreeProcTaskQueues()
        {
            List<Task> t123 = new List<Task>();
            List<Task> t124 = new List<Task>();
            List<Task> t125 = new List<Task>();
            List<Task> t234 = new List<Task>();
            List<Task> t235 = new List<Task>();
            List<Task> t345 = new List<Task>();
            List<Task> t134 = new List<Task>();
            List<Task> t135 = new List<Task>();
            List<Task> t245 = new List<Task>();
            List<Task> t145 = new List<Task>();

            foreach (var i in taskQueue)
            {
                if (i.supportedProcessors.Length == 3)
                {
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 3))
                    {
                        t123.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t124.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t125.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t234.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t235.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t345.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t134.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t135.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t245.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t145.Add(i);
                    }
                }
            }
            t123.Sort(); t124.Sort(); t125.Sort(); t234.Sort(); t235.Sort(); t345.Sort(); t134.Sort(); t135.Sort(); t245.Sort(); t145.Sort();
            t123.Reverse(); t124.Reverse(); t125.Reverse(); t234.Reverse(); t235.Reverse(); t345.Reverse(); t134.Reverse(); t135.Reverse(); t245.Reverse(); t145.Reverse();

            foreach (var item in t123)
            {
                switch (MinimumFrom3vars (devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power , devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                    devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[2].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t124)
            {
                switch (MinimumFrom3vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                    devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[3].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t125)
            {
                switch (MinimumFrom3vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                    devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t234)
            {
                switch (MinimumFrom3vars(devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power, devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power,
                    devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power))
                {
                    case 1:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[3].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t235)
            {
                switch (MinimumFrom3vars(devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power, devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power,
                    devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t345)
            {
                switch (MinimumFrom3vars(devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power, devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power,
                    devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[3].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t134)
            {
                switch (MinimumFrom3vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power,
                    devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[3].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t135)
            {
                switch (MinimumFrom3vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power,
                    devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t245)
            {
                switch (MinimumFrom3vars(devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power, devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power,
                    devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[3].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t145)
            {
                switch (MinimumFrom3vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power,
                    devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[3].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

        }


        private void FormFourProcTaskQueues()
        {
            List<Task> t1234 = new List<Task>();
            List<Task> t2345 = new List<Task>();
            List<Task> t1345 = new List<Task>();
            List<Task> t1235 = new List<Task>();
            List<Task> t1245 = new List<Task>();

            foreach (var i in taskQueue)
            {
                if (i.supportedProcessors.Length == 4)
                {
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2)
                        && i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 4))
                    {
                        t1234.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 2) && i.supportedProcessors.Any(p => p == 3)
                        && i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t2345.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 3)
                        && i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t1345.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2)
                        && i.supportedProcessors.Any(p => p == 3) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t1235.Add(i);
                    }
                    if (i.supportedProcessors.Any(p => p == 1) && i.supportedProcessors.Any(p => p == 2)
                        && i.supportedProcessors.Any(p => p == 4) && i.supportedProcessors.Any(p => p == 5))
                    {
                        t1245.Add(i);
                    }
                  
                }
            }
            t1234.Sort(); t2345.Sort(); t1345.Sort(); t1235.Sort(); t1245.Sort();
            t1234.Reverse(); t2345.Reverse(); t1345.Reverse(); t1235.Reverse(); t1245.Reverse();

            foreach (var item in t1234)
            {
                switch (MinimumFrom4vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                    devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power, devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 4:
                        devisionQueue[3].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t2345)
            {
                switch (MinimumFrom4vars(devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power, devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power,
                    devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power, devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[3].Enqueue(item);
                        break;
                    case 4:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t1345)
            {
                switch (MinimumFrom4vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power,
                    devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power, devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[3].Enqueue(item);
                        break;
                    case 4:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t1235)
            {
                switch (MinimumFrom4vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                    devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power, devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[2].Enqueue(item);
                        break;
                    case 4:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in t1245)
            {
                switch (MinimumFrom4vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                    devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power, devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                {
                    case 1:
                        devisionQueue[0].Enqueue(item);
                        break;
                    case 2:
                        devisionQueue[1].Enqueue(item);
                        break;
                    case 3:
                        devisionQueue[3].Enqueue(item);
                        break;
                    case 4:
                        devisionQueue[4].Enqueue(item);
                        break;
                    default:
                        break;
                }
            }

        }


        private void FormFiveProcTaskQueues()
        {
            foreach (var i in taskQueue)
            {
                if (i.supportedProcessors.Length == 5)
                {
                    switch (MinimumFrom5vars(devisionQueue[0].Sum(p => p.operationsAmont) / processors[0].power, devisionQueue[1].Sum(p => p.operationsAmont) / processors[1].power,
                   devisionQueue[2].Sum(p => p.operationsAmont) / processors[2].power, devisionQueue[3].Sum(p => p.operationsAmont) / processors[3].power,
                   devisionQueue[4].Sum(p => p.operationsAmont) / processors[4].power))
                    {
                        case 1:
                            devisionQueue[0].Enqueue(i);
                            break;
                        case 2:
                            devisionQueue[1].Enqueue(i);
                            break;
                        case 3:
                            devisionQueue[2].Enqueue(i);
                            break;
                        case 4:
                            devisionQueue[3].Enqueue(i);
                            break;
                        case 5:
                            devisionQueue[3].Enqueue(i);
                            break;
                        default:
                            break;
                    }   
                }
            }
        }
        //returns 1 if v1 < v2 and < v3;  2 if v2 is min
        private int MinimumFrom3vars( decimal v1, decimal v2, decimal v3 )
        {
            if (v1 <= v2 && v1 <= v3)
            {
                return 1;
            }
            if (v2 <= v1 && v2 <= v3)
            {
                return 2;
            }
            if (v3 <= v2 && v3 <= v1)
            {
                return 3;
            }
            return 0;
        }

        private int MinimumFrom4vars(decimal v1, decimal v2, decimal v3, decimal v4)
        {
            if (v1 <= v2 && v1 <= v3 && v1 <= v4)
            {
                return 1;
            }
            if (v2 <= v1 && v2 <= v3 && v2 <= v4)
            {
                return 2;
            }
            if (v3 <= v2 && v3 <= v1 && v3 <= v4)
            {
                return 3;
            }
            if (v4 <= v1 && v4 <= v2 && v4 <= v3)
            {
                return 4;
            }
            return 0;
        }

        private int MinimumFrom5vars(decimal v1, decimal v2, decimal v3, decimal v4, decimal v5)
        {
            if (v1 <= v2 && v1 <= v3 && v1 <= v4 && v1<= v5)
            {
                return 1;
            }
            if (v2 <= v1 && v2 <= v3 && v2 <= v4 && v2 <= v5)
            {
                return 2;
            }
            if (v3 <= v2 && v3 <= v1 && v3 <= v4 && v3 <= v5)
            {
                return 3;
            }
            if (v4 <= v1 && v4 <= v2 && v4 <= v3 && v4 <= v5)
            {
                return 4;
            }
            if (v5 <= v1 && v5 <= v2 && v5 <= v3 && v5 <= v4)
            {
                return 5;
            }
            return 0;
        }

        private void devideTasks()
        {
          

            queueMutex.WaitOne();
               
                    FormOneProcTaskQueues();
                    FormTwoProcTaskQueues();
                    FormThreeProcTaskQueues();
               
            queueMutex.ReleaseMutex();

          
        }
        private void SmartManageProcessors()
        {
            
        }
    }
}
