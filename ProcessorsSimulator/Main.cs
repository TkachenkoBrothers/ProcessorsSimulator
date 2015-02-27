using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ProcessorsSimulator
{
    public partial class Main : Form
    {
        private List<TextBox> processorsCurrentTaskList;
        private List<MaskedTextBox> processorsPowerList;
        private List<TextBox> processorsConditionList;
        private Manager manager;
        private TimeCalculator timeCalculator;
        public Main()
        {
            InitializeComponent();
            manager = new Manager();
            timeCalculator = new TimeCalculator();
            ManageInterface();
            ProcessorsInterface();
        }
      
        private void ManageInterface()
        {
            this.manager.generator.TaskGenerated += new Generator.TaskGeneratedHandler(BlinkWhenNewTaskGenerated);
            this.manager.ProcessorsWorkDone += new EventHandler(OnWorkDone); // enable to start again
            this.manager.QueueModified += new EventHandler(OnQueueModified);
            this.manager.SendTaskToProcessor += new EventHandler(BlinkWhenTaskSended);
            this.maskedTextBoxSleepIndex.Text = manager.generator.indexSleepBetweenTask.ToString("0.0000");
            this.maskedTextBoxScopeFrom.Text = manager.generator.taskComplexityScope[0].ToString("00000");
            this.maskedTextBoxScopeTo.Text = manager.generator.taskComplexityScope[1].ToString("00000");
            this.maskedTextBoxWorkingTime.Text = manager.generator.workingTime.ToString("0000000");
            this.manager.processors.All(p => { p.NewProcessStarted += OnNewProcessStarted; return true; }); // if process any started initialize progress bar
            this.manager.processors.All(p => { p.ProgressChanged += OnProgressChanged; return true; });
            this.manager.processors.All(p => { p.ProcessEnded += OnProcessEnded; return true; });
        }

        private void ProcessorsInterface()
        {
            
            processorsConditionList = new List<TextBox> { };
            processorsPowerList = new List<MaskedTextBox> { };
            processorsCurrentTaskList = new List<TextBox> { };
            
           // var numbersAndWords = numbers.Zip(words, (n, w) => new { Number = n, Word = w });
            var conditions = processorsConditionList.Zip(manager.processors , (c , p) => new { textboxCondition = c, procCondition = p.condition });
            var powers = processorsPowerList.Zip(manager.processors, (c, p) => new { textboxPower = c, procPower = p.power });
            var currentTasks = processorsCurrentTaskList.Zip(manager.processors, (c, p) => new { textboxCurrentTask = c, procCurrentTask = p.currentTask });

            processorsPowerList.Add(maskedTextBoxProcessorPower1);
            processorsPowerList.Add(maskedTextBoxProcessorPower2);
            processorsPowerList.Add(maskedTextBoxProcessorPower3);
            processorsPowerList.Add(maskedTextBoxProcessorPower4);
            processorsPowerList.Add(maskedTextBoxProcessorPower5);

            processorsConditionList.Add(textBoxProcessorCondition1);
            processorsConditionList.Add(textBoxProcessorCondition2);
            processorsConditionList.Add(textBoxProcessorCondition3);
            processorsConditionList.Add(textBoxProcessorCondition4);
            processorsConditionList.Add(textBoxProcessorCondition5);

            processorsCurrentTaskList.Add(textBoxProcessorCurrentTask1);
            processorsCurrentTaskList.Add(textBoxProcessorCurrentTask2);
            processorsCurrentTaskList.Add(textBoxProcessorCurrentTask3);
            processorsCurrentTaskList.Add(textBoxProcessorCurrentTask4);
            processorsCurrentTaskList.Add(textBoxProcessorCurrentTask5);

            foreach (var cond in conditions)
            {
                cond.textboxCondition.Text = cond.procCondition.ToString();
            }
            foreach (var pow in powers)
            {
                pow.textboxPower.Text = pow.procPower.ToString("00000");
            }
            foreach (var cur in currentTasks)
            {
                try
                {
                    cur.textboxCurrentTask.Text = cur.procCurrentTask.ToString();
                }
                catch (NullReferenceException)
                {
                    
                    
                }
            }
        }
        private void OnNewProcessStarted(int id, int maximum, Task currentTask, processor_condition condition)
        {
            switch (id)
            {
                case 0:
                    this.Invoke((MethodInvoker)delegate 
                    {
                        progressBarProcessor1.Value = 0; 
                        progressBarProcessor1.Maximum = maximum; 
                        this.textBoxProcessorCurrentTask1.Text = currentTask.ToString();
                        this.textBoxProcessorCondition1.Text = condition.ToString();
                    });
                    
                    break;
                case 1:
                    this.Invoke((MethodInvoker)delegate 
                    {
                        progressBarProcessor2.Value = 0; 
                        progressBarProcessor2.Maximum = maximum; 
                        this.textBoxProcessorCurrentTask2.Text = currentTask.ToString();
                        this.textBoxProcessorCondition2.Text = condition.ToString();
                    });
                    break;
                case 2:
                    this.Invoke((MethodInvoker)delegate 
                    {
                        progressBarProcessor3.Value = 0; 
                        progressBarProcessor3.Maximum = maximum;
                        this.textBoxProcessorCurrentTask3.Text = currentTask.ToString();
                        this.textBoxProcessorCondition3.Text = condition.ToString();                   
                    });
                    break;
                case 3:
                    this.Invoke((MethodInvoker)delegate 
                    {
                        progressBarProcessor4.Value = 0;
                        progressBarProcessor4.Maximum = maximum; 
                        this.textBoxProcessorCurrentTask4.Text = currentTask.ToString();
                        this.textBoxProcessorCondition4.Text = condition.ToString();
                    });
                    break;
                case 4:
                    this.Invoke((MethodInvoker)delegate 
                    {
                        progressBarProcessor5.Value = 0; 
                        progressBarProcessor5.Maximum = maximum; 
                        this.textBoxProcessorCurrentTask5.Text = currentTask.ToString();
                        this.textBoxProcessorCondition5.Text = condition.ToString();
                    });
                    break;
                default:
                    break;
            }
        }

        private void OnProcessEnded(int id, processor_condition condition)
        {

            switch (id)
            {
                case 0:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.textBoxProcessorCurrentTask1.Text = "";
                        this.textBoxProcessorCondition1.Text = condition.ToString();
                        progressBarProcessor1.Value = 0;
                    });

                    break;
                case 1:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.textBoxProcessorCurrentTask2.Text = "";
                        this.textBoxProcessorCondition2.Text = condition.ToString();
                        progressBarProcessor2.Value = 0;
                    });
                    break;
                case 2:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.textBoxProcessorCurrentTask3.Text = "";
                        this.textBoxProcessorCondition3.Text = condition.ToString();
                        progressBarProcessor3.Value = 0;
                    });
                    break;
                case 3:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.textBoxProcessorCurrentTask4.Text = "";
                        this.textBoxProcessorCondition4.Text = condition.ToString();
                        progressBarProcessor4.Value = 0;
                    });
                    break;
                case 4:
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.textBoxProcessorCurrentTask5.Text = "";
                        this.textBoxProcessorCondition5.Text = condition.ToString();
                        progressBarProcessor5.Value = 0;
                    });
                    break;
                default:
                    break;
            }
        }
        private void OnProgressChanged(int id, int progress) 
        {
            switch (id)
            {
                case 0:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor1.Value = progress; });
                    break;
                case 1:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor2.Value = progress; });
                    break;
                case 2:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor3.Value = progress; });
                    break;
                case 3:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor4.Value = progress; });
                    break;
                case 4:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor5.Value = progress; });
                    break;
                default:
                    break;
            }
        }
        private void OnQueueModified(object sender, EventArgs e)
        {
            string res = "";
            manager.queueMutex.WaitOne();
            if (manager.taskQueue.Count() != 0)
            {
                foreach (Task task in manager.taskQueue)
                {
                    res += String.Format("{0}. Task (operationsAmount={1}, supportedProcessors={2}){3}", 
                                        task.id.ToString(), task.operationsAmont.ToString(), task.getSupportedProcessors(), "\n");
                }
            }
            manager.queueMutex.ReleaseMutex();
            this.Invoke((MethodInvoker)delegate { richTextBoxManagerQueue.Text = res; });
        }
        private void ResetProgressBarsValues()
        {
            this.Invoke((MethodInvoker)delegate { progressBarProcessor1.Value = 0; });
            this.Invoke((MethodInvoker)delegate { progressBarProcessor2.Value = 0; });
            this.Invoke((MethodInvoker)delegate { progressBarProcessor3.Value = 0; });
            this.Invoke((MethodInvoker)delegate { progressBarProcessor4.Value = 0; });
            this.Invoke((MethodInvoker)delegate { progressBarProcessor5.Value = 0; });
        }
        private void OnWorkDone(object sender, EventArgs e)
        {
            //this.buttonStart.Enabled = true;
            this.Invoke((MethodInvoker)delegate { buttonStart.Enabled = true; });
            this.Invoke((MethodInvoker)delegate { buttonGeneratorUpdate.Enabled = true; });
            this.Invoke((MethodInvoker)delegate { this.buttonUpdateProcessor1.Enabled = true; });
            this.Invoke((MethodInvoker)delegate { this.buttonUpdateProcessor2.Enabled = true; });
            this.Invoke((MethodInvoker)delegate { this.buttonUpdateProcessor3.Enabled = true; });
            this.Invoke((MethodInvoker)delegate { this.buttonUpdateProcessor4.Enabled = true; });
            this.Invoke((MethodInvoker)delegate { this.buttonUpdateProcessor5.Enabled = true; });
            ResetProgressBarsValues();
            this.Invoke((MethodInvoker)delegate { timeCalculator.SetSecondTime(); MessageBox.Show("Time : " + timeCalculator.TimeOfWork().ToString()); });
           
            this.manager.processors.All(p => { p.NewProcessStarted += OnNewProcessStarted; return true; }); // reload subscribes (because processors reloaded)
            this.manager.processors.All(p => { p.ProgressChanged += OnProgressChanged; return true; });
            this.manager.processors.All(p => { p.ProcessEnded += OnProcessEnded; return true; });
        }
        private void BlinkWhenTaskSended(object sender, EventArgs e)
        {
            this.pictureBoxManagerIndicator.BackColor = System.Drawing.Color.Gold;
            Application.DoEvents();
            Thread.Sleep(100);
            this.pictureBoxManagerIndicator.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
        }
        private void BlinkWhenNewTaskGenerated(Task task)
        {
            this.pictureBoxGeneratorIndicator.BackColor = System.Drawing.Color.Gold;
            Application.DoEvents();
            Thread.Sleep(100);
            this.pictureBoxGeneratorIndicator.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
        }
        private void buttonGeneratorUpdate_Click(object sender, EventArgs e)
        {
            double sleepIndex = 0;
            int complexityScope0 = 0;
            int complexityScope1 = 0;
            int workingTime = 0;
            try
            {
                sleepIndex = double.Parse(maskedTextBoxSleepIndex.Text);
                complexityScope0 = int.Parse(maskedTextBoxScopeFrom.Text);
                complexityScope1 = int.Parse(maskedTextBoxScopeTo.Text);
                workingTime = int.Parse(maskedTextBoxWorkingTime.Text);
            }
            catch (FormatException )
            {

                MessageBox.Show("Wrong arguments");
            }

            if (sleepIndex > 0 && sleepIndex < 1)
            {
                manager.generator.indexSleepBetweenTask = sleepIndex;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Sleep Index must be from 0 to 1");
                return;
            }
            if (complexityScope0 > 0 && complexityScope1 > 0 && workingTime > 0)
            {
                if (complexityScope0 < complexityScope1)
                {
                    manager.generator.taskComplexityScope[0] = complexityScope0;
                    manager.generator.taskComplexityScope[1] = complexityScope1;
                }
                else
                {
                    MessageBox.Show("Second scope must be greater than first");
                    return;
                }
                
                manager.generator.workingTime = workingTime;
            }
            else
            {
                MessageBox.Show("Wrong argument. They must be greater than zero!");
                return;
            }

            buttonGeneratorUpdate.BackColor = Color.FromName("InactiveCaption");
            this.maskedTextBoxSleepIndex.Text = manager.generator.indexSleepBetweenTask.ToString("0.0000");
            this.maskedTextBoxScopeFrom.Text = manager.generator.taskComplexityScope[0].ToString("00000");
            this.maskedTextBoxScopeTo.Text = manager.generator.taskComplexityScope[1].ToString("00000");
            this.maskedTextBoxWorkingTime.Text = manager.generator.workingTime.ToString("0000000");
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //manager.generator.workingTime = 5000; // reload
            this.buttonStart.Enabled = false;
            this.buttonGeneratorUpdate.Enabled = false;
            this.buttonUpdateProcessor1.Enabled = false;
            this.buttonUpdateProcessor2.Enabled = false;
            this.buttonUpdateProcessor3.Enabled = false;
            this.buttonUpdateProcessor4.Enabled = false;
            this.buttonUpdateProcessor5.Enabled = false;
            manager.Manage();
            timeCalculator.SetFirstTime();
        }

        private void buttonUpdateProcessor1_Click(object sender, EventArgs e)
        {
            manager.processors[0].power = int.Parse(maskedTextBoxProcessorPower1.Text);
            buttonUpdateProcessor1.BackColor = Color.FromName("InactiveCaption");
        }

        private void buttonUpdateProcessor2_Click(object sender, EventArgs e)
        {
            manager.processors[1].power = int.Parse(maskedTextBoxProcessorPower2.Text);
            buttonUpdateProcessor2.BackColor = Color.FromName("InactiveCaption");
        }

        private void buttonUpdateProcessor3_Click(object sender, EventArgs e)
        {
            manager.processors[2].power = int.Parse(maskedTextBoxProcessorPower3.Text);
            buttonUpdateProcessor3.BackColor = Color.FromName("InactiveCaption");
        }

        private void buttonUpdateProcessor4_Click(object sender, EventArgs e)
        {
            manager.processors[3].power = int.Parse(maskedTextBoxProcessorPower4.Text);
            buttonUpdateProcessor4.BackColor = Color.FromName("InactiveCaption");
        }

        private void buttonUpdateProcessor5_Click(object sender, EventArgs e)
        {
            manager.processors[4].power = int.Parse(maskedTextBoxProcessorPower5.Text);
            buttonUpdateProcessor5.BackColor = Color.FromName("InactiveCaption");
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            manager.taskQueue.Clear();
            manager.generator.currrentWorkingTime = -1; // forsly rise WorkDone event
            manager.processors.All(x => { x.condition = processor_condition.waitingForTask; return true; }); // forsly rise ProcessorsWorkDone event        
            //ResetProgressBarsValues();
            this.Invoke((MethodInvoker)delegate { richTextBoxManagerQueue.Text = ""; });
            timeCalculator.SetSecondTime();
        }

      

        private void maskedTextBoxSleepIndex_Click(object sender, EventArgs e)
        {
            buttonGeneratorUpdate.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxScopeFrom_Click(object sender, EventArgs e)
        {
            buttonGeneratorUpdate.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxScopeTo_Click(object sender, EventArgs e)
        {
            buttonGeneratorUpdate.BackColor = Color.FromName("MenuHighlight");
        }

        

        private void maskedTextBoxWorkingTime_Click(object sender, EventArgs e)
        {
            buttonGeneratorUpdate.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxProcessorPower1_Click(object sender, EventArgs e)
        {
            buttonUpdateProcessor1.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxProcessorPower2_Click(object sender, EventArgs e)
        {
            buttonUpdateProcessor2.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxProcessorPower3_Click(object sender, EventArgs e)
        {
            buttonUpdateProcessor3.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxProcessorPower4_Click(object sender, EventArgs e)
        {
            buttonUpdateProcessor4.BackColor = Color.FromName("MenuHighlight");
        }

        private void maskedTextBoxProcessorPower5_Click(object sender, EventArgs e)
        {
            buttonUpdateProcessor5.BackColor = Color.FromName("MenuHighlight");
        }
    }
}
