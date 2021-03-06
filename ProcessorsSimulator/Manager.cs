﻿using System;
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
            taskList = new List<Task>();
            InitializeTaskQueues();
            processors = new List<Processor>();
            processorsThreads = new Thread[5];
            CreateProcessors();
            CreateProcessorsThreads();
            CreateGenerator();
            CreateGeneratorThread();
            //CreateManageProcessors();
            generator.WorkDone += new EventHandler(OnWorkDone);
            this.ProcessorsWorkDone += OnProcessorsWorkDone;
            listMutex = new Mutex();
            Debug.Print("Manager initialized");
            method = "";
        }

        public Manager(int pow1, int pow2, int pow3, int pow4, int pow5, int sleepTime, int Scope1, int Scope2, int workingTime, string _method)
        {
            method = _method;
            taskList = new List<Task>();
            InitializeTaskQueues();
            processors = new List<Processor>(){new Processor(pow1), new Processor(pow2), new Processor(pow3),
            new Processor(pow4), new Processor(pow5)};
            processorsThreads = new Thread[5];
            CreateProcessorsThreads();
            //CreateManageProcessors();
            generator = new Generator(sleepTime, Scope1, Scope2,  workingTime);
            CreateGeneratorThread();
            generator.TaskGenerated += new Generator.TaskGeneratedHandler(GetTask);
            generator.WorkDone += new EventHandler(OnWorkDone);
            this.ProcessorsWorkDone += OnProcessorsWorkDone;
            listMutex = new Mutex();
            Debug.Print("Manager initialized");
            
        }

        
        private Queue<Task>[] devisionQueue;
        public Mutex listMutex;
        public conditionManager condition;
        public List<Task> taskList; // using for second method where specific task need to be removed from the list of tasks (can`t remove specific task from queue)
        public Generator generator;
        public List<Processor> processors;
        public Thread generatorThread;
        private Thread processorManager;
        private Thread[] processorsThreads;
        public string method;

        public event EventHandler ProcessorsWorkDone;
        public event EventHandler ListModified;
        public event EventHandler SendTaskToProcessor;

        public void Manage(string method)
        {
            this.method = method;
            CreateManageProcessors();
            StartGenerator();
            StartProcessors();
            StartManageProcessors();
            
        }
        private void GetTask(Task task)
        {
            listMutex.WaitOne();
            taskList.Add(task);
            Debug.Print(String.Format("Task (id={0}, operationsAmount={1}, supportedProcessors={2}) is added to list", 
                                task.id.ToString(), task.operationsAmont.ToString(), task.getSupportedProcessors()));
            if (method == "Second") devideTasks();
            if (method == "Third")
            {
                if (devisionQueue.Any(x => x.Count <= 3) || devisionQueue.All(x => x.Count <= 3))
                    devideTasks();
                else
                {

                }
            }
            if (generator.currrentWorkingTime == 0)
            {
                devideTasks();
            }
            if (ListModified != null) ListModified(this, null);
            listMutex.ReleaseMutex();
        }

        private void InitializeTaskQueues() 
        {
            devisionQueue = new Queue<Task>[5] { new Queue<Task>(), new Queue<Task>(), new Queue<Task>(), new Queue<Task>(), new Queue<Task>() };
        }

        private void OnWorkDone(object sender, EventArgs e) //generator work done
        {
            //generatorThread.Abort();
            //processorManager.Abort();
            //AbortProcessors();
            Debug.Print("Generator work is done."); // must execute BEFORE Starting SmartManageProcessors or devideTasks
            CreateGeneratorThread(); // removes Start procedure for generator (100% stops calling taskQueue)
            //devideTasks();
        }
        //
        // reload threads when they finish work !!Attention: generator might finish his work before processors
        //
        private void OnProcessorsWorkDone(object sender, EventArgs e) 
        {
            listMutex.WaitOne();
            Debug.Print("Processors work is done.");
            Debug.Print("List count = " + taskList.Count().ToString());
            taskList = new List<Task>(); // reload
            listMutex.ReleaseMutex();
            //processors = new List<Processor>() { new Processor(processors[0].power), new Processor(processors[1].power), new Processor(processors[2].power),
            //new Processor(processors[3].power), new Processor(processors[4].power)};
            //processorsThreads = new Thread[5];
            //CreateProcessorsThreads();
            //CreateManageProcessors();
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
            }
        }
        private void CreateProcessorsThreads()
        {
            for (int i = 0; i < processors.Count; i++)
            {
                //processors[i].condition = processor_condition.waitingForTask;
                //processors[i].currentTask = null;
                Thread currentThread = new Thread(new ThreadStart(processors[i].DoWork));
                currentThread.Name = "Processor" + (i).ToString();
                if (currentThread != null)
                    processorsThreads[i] = currentThread;
            }
        }
        private void StartProcessors()
        {
            for (int i = 0; i < processorsThreads.Count(); i++)
            {
                processorsThreads[i].Start();
                Debug.Print("Processor" + (i).ToString() + " thread started");
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
            if (method == "FIFO")
            {
                processorManager = new Thread(new
                ThreadStart(ManageProcessors));
            }
            else if (method == "Second" || method == "Third")
            {
                processorManager = new Thread(new
                ThreadStart(ManageProcessorsSecond));
            }
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
                listMutex.WaitOne();
                if (taskList.Count != 0)
                {
                    Thread.Sleep(50); // extra time for displaying queue (only for those tasks, which is send to processors immediately)
                    Task currentTask = new Task();
                    if (taskList.Count != 0)
                        currentTask = taskList.First(); // peeks first elem from List
                    for(int i = 0; i < processors.Count; i++)
                    {
                        bool supported = currentTask.supportedProcessors.Contains(processors[i].id + 1);
                        if (supported)
                            if (processors[i].condition == processor_condition.waitingForTask && taskList.Count != 0)
                            {
                                taskList.RemoveAt(0); // delete first elem
                                if (ListModified != null) ListModified(this, null);
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
                                listMutex.ReleaseMutex();
                                break;
                            }
                            else Thread.Sleep(30); 
                    }         
                }
                listMutex.ReleaseMutex();
            }
        }

        private void ManageProcessorsSecond()
        {
            while (true)
            {
                listMutex.WaitOne();
                if (taskList.Count != 0)
                {
                    Task currentTask = new Task();
                    for (int i = 0; i < devisionQueue.Length; i++)
                    {
                        if (devisionQueue[i].Count != 0)
                        {
                            if (processors[i].condition == processor_condition.waitingForTask && taskList.Count != 0)
                            {
                                currentTask = devisionQueue[i].Dequeue();
                                taskList.RemoveAll(x => x.id == currentTask.id); // removes current task from main list
                                if (ListModified != null) ListModified(this, null);
                                processors[i].currentTask = currentTask;
                                Debug.Print(String.Format("Manager sends task (id={0}, operationsAmount={1}, supportedProcessors={2}) to Processor{3}",
                                                        currentTask.id.ToString(), currentTask.operationsAmont.ToString(),
                                                        currentTask.getSupportedProcessors(), (i + 1).ToString()));
                                if (SendTaskToProcessor != null) SendTaskToProcessor(this, null);
                                processors[i].condition = processor_condition.processing;
                            }
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
                                listMutex.ReleaseMutex();
                                break;
                            }
                            else Thread.Sleep(30);
                    }
                }
                listMutex.ReleaseMutex();
            }
        }

        private void FormOneProcTaskQueues()
        {
            foreach (var i in taskList)
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
            foreach (var i in taskList)
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

            foreach (var i in taskList)
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

            foreach (var i in taskList)
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
            foreach (var i in taskList)
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
                            devisionQueue[4].Enqueue(i);
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
          
            listMutex.WaitOne();
                foreach (var item in devisionQueue)
                {
                    item.Clear();
                }
                FormOneProcTaskQueues();
                FormTwoProcTaskQueues();
                FormThreeProcTaskQueues();
                FormFourProcTaskQueues();
                FormFiveProcTaskQueues();
               
            listMutex.ReleaseMutex();

          
        }
        private void SmartManageProcessors()
        {
            devideTasks();
        }
    }
}
