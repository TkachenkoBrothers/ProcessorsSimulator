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
        private Manager manager;
        public Main()
        {
            InitializeComponent();
            manager = new Manager();
            ManageInterface();
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
        }
        private void OnNewProcessStarted(int id, int maximum)
        {
            switch (id)
            {
                case 0:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor1.Value = 0; progressBarProcessor1.Maximum = maximum;});
                    break;
                case 1:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor2.Value = 0; progressBarProcessor2.Maximum = maximum; });
                    break;
                case 2:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor3.Value = 0; progressBarProcessor3.Maximum = maximum; });
                    break;
                case 3:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor4.Value = 0; progressBarProcessor4.Maximum = maximum; });
                    break;
                case 4:
                    this.Invoke((MethodInvoker)delegate { progressBarProcessor5.Value = 0; progressBarProcessor5.Maximum = maximum; });
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
            ResetProgressBarsValues();
            this.manager.processors.All(p => { p.NewProcessStarted += OnNewProcessStarted; return true; }); // reload subscribes (because processors reloaded)
            this.manager.processors.All(p => { p.ProgressChanged += OnProgressChanged; return true; });
        }
        private void BlinkWhenTaskSended(object sender, EventArgs e)
        {
            this.pictureBoxManagerIndicator.BackColor = System.Drawing.SystemColors.Highlight;
            Application.DoEvents();
            Thread.Sleep(100);
            this.pictureBoxManagerIndicator.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
        }
        private void BlinkWhenNewTaskGenerated(Task task)
        {
            this.pictureBoxGeneratorIndicator.BackColor = System.Drawing.SystemColors.Highlight;
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
                MessageBox.Show("working time" + manager.generator.workingTime.ToString());
            }
            else
            {
                MessageBox.Show("Wrong argument. They must be greater than zero!");
                return;
            }
            
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
            manager.Manage();
        }
    }
}
