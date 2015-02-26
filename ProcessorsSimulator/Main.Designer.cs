namespace ProcessorsSimulator
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxGenerator = new System.Windows.Forms.PictureBox();
            this.labelGenerator = new System.Windows.Forms.Label();
            this.labelSleepIndex = new System.Windows.Forms.Label();
            this.labelScopeFrom = new System.Windows.Forms.Label();
            this.labelScopeTo = new System.Windows.Forms.Label();
            this.labelWorkingTime = new System.Windows.Forms.Label();
            this.labelGeneratorIndicator = new System.Windows.Forms.Label();
            this.pictureBoxGeneratorIndicator = new System.Windows.Forms.PictureBox();
            this.buttonGeneratorUpdate = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.pictureBoxManagerIndicator = new System.Windows.Forms.PictureBox();
            this.labelManagerIndicator = new System.Windows.Forms.Label();
            this.labelManager = new System.Windows.Forms.Label();
            this.pictureBoxManager = new System.Windows.Forms.PictureBox();
            this.richTextBoxManagerQueue = new System.Windows.Forms.RichTextBox();
            this.hArrow1 = new PriborControlLibrary.HArrow();
            this.maskedTextBoxSleepIndex = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxScopeFrom = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxScopeTo = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBoxWorkingTime = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGenerator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGeneratorIndicator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxManagerIndicator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxManager)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxGenerator
            // 
            this.pictureBoxGenerator.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBoxGenerator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxGenerator.Location = new System.Drawing.Point(200, 12);
            this.pictureBoxGenerator.Name = "pictureBoxGenerator";
            this.pictureBoxGenerator.Size = new System.Drawing.Size(198, 202);
            this.pictureBoxGenerator.TabIndex = 0;
            this.pictureBoxGenerator.TabStop = false;
            // 
            // labelGenerator
            // 
            this.labelGenerator.AutoSize = true;
            this.labelGenerator.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelGenerator.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelGenerator.Location = new System.Drawing.Point(259, 23);
            this.labelGenerator.Name = "labelGenerator";
            this.labelGenerator.Size = new System.Drawing.Size(84, 18);
            this.labelGenerator.TabIndex = 1;
            this.labelGenerator.Text = "Generator";
            // 
            // labelSleepIndex
            // 
            this.labelSleepIndex.AutoSize = true;
            this.labelSleepIndex.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelSleepIndex.Location = new System.Drawing.Point(213, 54);
            this.labelSleepIndex.Name = "labelSleepIndex";
            this.labelSleepIndex.Size = new System.Drawing.Size(62, 13);
            this.labelSleepIndex.TabIndex = 2;
            this.labelSleepIndex.Text = "Sleep index";
            // 
            // labelScopeFrom
            // 
            this.labelScopeFrom.AutoSize = true;
            this.labelScopeFrom.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelScopeFrom.Location = new System.Drawing.Point(213, 84);
            this.labelScopeFrom.Name = "labelScopeFrom";
            this.labelScopeFrom.Size = new System.Drawing.Size(61, 13);
            this.labelScopeFrom.TabIndex = 3;
            this.labelScopeFrom.Text = "Scope from";
            // 
            // labelScopeTo
            // 
            this.labelScopeTo.AutoSize = true;
            this.labelScopeTo.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelScopeTo.Location = new System.Drawing.Point(319, 84);
            this.labelScopeTo.Name = "labelScopeTo";
            this.labelScopeTo.Size = new System.Drawing.Size(16, 13);
            this.labelScopeTo.TabIndex = 4;
            this.labelScopeTo.Text = "to";
            // 
            // labelWorkingTime
            // 
            this.labelWorkingTime.AutoSize = true;
            this.labelWorkingTime.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelWorkingTime.Location = new System.Drawing.Point(213, 113);
            this.labelWorkingTime.Name = "labelWorkingTime";
            this.labelWorkingTime.Size = new System.Drawing.Size(69, 13);
            this.labelWorkingTime.TabIndex = 5;
            this.labelWorkingTime.Text = "Working time";
            // 
            // labelGeneratorIndicator
            // 
            this.labelGeneratorIndicator.AutoSize = true;
            this.labelGeneratorIndicator.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelGeneratorIndicator.Location = new System.Drawing.Point(275, 153);
            this.labelGeneratorIndicator.Name = "labelGeneratorIndicator";
            this.labelGeneratorIndicator.Size = new System.Drawing.Size(48, 13);
            this.labelGeneratorIndicator.TabIndex = 10;
            this.labelGeneratorIndicator.Text = "Indicator";
            // 
            // pictureBoxGeneratorIndicator
            // 
            this.pictureBoxGeneratorIndicator.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pictureBoxGeneratorIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxGeneratorIndicator.Location = new System.Drawing.Point(280, 166);
            this.pictureBoxGeneratorIndicator.Name = "pictureBoxGeneratorIndicator";
            this.pictureBoxGeneratorIndicator.Size = new System.Drawing.Size(37, 35);
            this.pictureBoxGeneratorIndicator.TabIndex = 11;
            this.pictureBoxGeneratorIndicator.TabStop = false;
            // 
            // buttonGeneratorUpdate
            // 
            this.buttonGeneratorUpdate.Location = new System.Drawing.Point(340, 178);
            this.buttonGeneratorUpdate.Name = "buttonGeneratorUpdate";
            this.buttonGeneratorUpdate.Size = new System.Drawing.Size(48, 23);
            this.buttonGeneratorUpdate.TabIndex = 12;
            this.buttonGeneratorUpdate.Text = "update";
            this.buttonGeneratorUpdate.UseVisualStyleBackColor = true;
            this.buttonGeneratorUpdate.Click += new System.EventHandler(this.buttonGeneratorUpdate_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(771, 230);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 13;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // pictureBoxManagerIndicator
            // 
            this.pictureBoxManagerIndicator.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.pictureBoxManagerIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxManagerIndicator.Location = new System.Drawing.Point(500, 218);
            this.pictureBoxManagerIndicator.Name = "pictureBoxManagerIndicator";
            this.pictureBoxManagerIndicator.Size = new System.Drawing.Size(37, 35);
            this.pictureBoxManagerIndicator.TabIndex = 25;
            this.pictureBoxManagerIndicator.TabStop = false;
            // 
            // labelManagerIndicator
            // 
            this.labelManagerIndicator.AutoSize = true;
            this.labelManagerIndicator.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelManagerIndicator.Location = new System.Drawing.Point(495, 205);
            this.labelManagerIndicator.Name = "labelManagerIndicator";
            this.labelManagerIndicator.Size = new System.Drawing.Size(48, 13);
            this.labelManagerIndicator.TabIndex = 24;
            this.labelManagerIndicator.Text = "Indicator";
            // 
            // labelManager
            // 
            this.labelManager.AutoSize = true;
            this.labelManager.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.labelManager.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelManager.Location = new System.Drawing.Point(640, 23);
            this.labelManager.Name = "labelManager";
            this.labelManager.Size = new System.Drawing.Size(73, 18);
            this.labelManager.TabIndex = 15;
            this.labelManager.Text = "Manager";
            // 
            // pictureBoxManager
            // 
            this.pictureBoxManager.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBoxManager.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxManager.Location = new System.Drawing.Point(478, 12);
            this.pictureBoxManager.Name = "pictureBoxManager";
            this.pictureBoxManager.Size = new System.Drawing.Size(387, 257);
            this.pictureBoxManager.TabIndex = 14;
            this.pictureBoxManager.TabStop = false;
            // 
            // richTextBoxManagerQueue
            // 
            this.richTextBoxManagerQueue.BackColor = System.Drawing.SystemColors.HotTrack;
            this.richTextBoxManagerQueue.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.richTextBoxManagerQueue.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBoxManagerQueue.ForeColor = System.Drawing.SystemColors.Info;
            this.richTextBoxManagerQueue.Location = new System.Drawing.Point(500, 51);
            this.richTextBoxManagerQueue.Name = "richTextBoxManagerQueue";
            this.richTextBoxManagerQueue.ReadOnly = true;
            this.richTextBoxManagerQueue.Size = new System.Drawing.Size(346, 151);
            this.richTextBoxManagerQueue.TabIndex = 26;
            this.richTextBoxManagerQueue.Text = "";
            // 
            // hArrow1
            // 
            this.hArrow1.ArrowType = ArrowT.none;
            this.hArrow1.Location = new System.Drawing.Point(397, 110);
            this.hArrow1.Margin = new System.Windows.Forms.Padding(0);
            this.hArrow1.Name = "hArrow1";
            this.hArrow1.Size = new System.Drawing.Size(81, 10);
            this.hArrow1.TabIndex = 27;
            this.hArrow1.WidthType = WidthT.default_width;
            // 
            // maskedTextBoxSleepIndex
            // 
            this.maskedTextBoxSleepIndex.Location = new System.Drawing.Point(281, 49);
            this.maskedTextBoxSleepIndex.Mask = "0.0000";
            this.maskedTextBoxSleepIndex.Name = "maskedTextBoxSleepIndex";
            this.maskedTextBoxSleepIndex.Size = new System.Drawing.Size(42, 20);
            this.maskedTextBoxSleepIndex.TabIndex = 28;
            // 
            // maskedTextBoxScopeFrom
            // 
            this.maskedTextBoxScopeFrom.Location = new System.Drawing.Point(276, 81);
            this.maskedTextBoxScopeFrom.Mask = "00000";
            this.maskedTextBoxScopeFrom.Name = "maskedTextBoxScopeFrom";
            this.maskedTextBoxScopeFrom.Size = new System.Drawing.Size(41, 20);
            this.maskedTextBoxScopeFrom.TabIndex = 29;
            this.maskedTextBoxScopeFrom.ValidatingType = typeof(int);
            // 
            // maskedTextBoxScopeTo
            // 
            this.maskedTextBoxScopeTo.Location = new System.Drawing.Point(333, 81);
            this.maskedTextBoxScopeTo.Mask = "00000";
            this.maskedTextBoxScopeTo.Name = "maskedTextBoxScopeTo";
            this.maskedTextBoxScopeTo.Size = new System.Drawing.Size(41, 20);
            this.maskedTextBoxScopeTo.TabIndex = 30;
            this.maskedTextBoxScopeTo.ValidatingType = typeof(int);
            // 
            // maskedTextBoxWorkingTime
            // 
            this.maskedTextBoxWorkingTime.Location = new System.Drawing.Point(288, 107);
            this.maskedTextBoxWorkingTime.Mask = "0000000";
            this.maskedTextBoxWorkingTime.Name = "maskedTextBoxWorkingTime";
            this.maskedTextBoxWorkingTime.Size = new System.Drawing.Size(55, 20);
            this.maskedTextBoxWorkingTime.TabIndex = 31;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1148, 631);
            this.Controls.Add(this.maskedTextBoxWorkingTime);
            this.Controls.Add(this.maskedTextBoxScopeTo);
            this.Controls.Add(this.maskedTextBoxScopeFrom);
            this.Controls.Add(this.maskedTextBoxSleepIndex);
            this.Controls.Add(this.hArrow1);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.richTextBoxManagerQueue);
            this.Controls.Add(this.pictureBoxManagerIndicator);
            this.Controls.Add(this.labelManagerIndicator);
            this.Controls.Add(this.labelManager);
            this.Controls.Add(this.pictureBoxManager);
            this.Controls.Add(this.buttonGeneratorUpdate);
            this.Controls.Add(this.pictureBoxGeneratorIndicator);
            this.Controls.Add(this.labelGeneratorIndicator);
            this.Controls.Add(this.labelWorkingTime);
            this.Controls.Add(this.labelScopeTo);
            this.Controls.Add(this.labelScopeFrom);
            this.Controls.Add(this.labelSleepIndex);
            this.Controls.Add(this.labelGenerator);
            this.Controls.Add(this.pictureBoxGenerator);
            this.Name = "Main";
            this.Text = "Main";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGenerator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGeneratorIndicator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxManagerIndicator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxGenerator;
        private System.Windows.Forms.Label labelGenerator;
        private System.Windows.Forms.Label labelSleepIndex;
        private System.Windows.Forms.Label labelScopeFrom;
        private System.Windows.Forms.Label labelScopeTo;
        private System.Windows.Forms.Label labelWorkingTime;
        private System.Windows.Forms.Label labelGeneratorIndicator;
        private System.Windows.Forms.PictureBox pictureBoxGeneratorIndicator;
        private System.Windows.Forms.Button buttonGeneratorUpdate;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.PictureBox pictureBoxManagerIndicator;
        private System.Windows.Forms.Label labelManagerIndicator;
        private System.Windows.Forms.Label labelManager;
        private System.Windows.Forms.PictureBox pictureBoxManager;
        private System.Windows.Forms.RichTextBox richTextBoxManagerQueue;
        private PriborControlLibrary.HArrow hArrow1;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxSleepIndex;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxScopeFrom;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxScopeTo;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxWorkingTime;

        
    }
}

