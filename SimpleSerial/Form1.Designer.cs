namespace SimpleSerial
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Timer tempTimer;
            this.musicProgBarLeft = new System.Windows.Forms.ProgressBar();
            this.musicTimer = new System.Windows.Forms.Timer(this.components);
            this.sensivityScrollBar = new System.Windows.Forms.HScrollBar();
            this.sensivityLabel = new System.Windows.Forms.Label();
            this.musicProgBarRight = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.saveSetupBtn = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.musicControllChkBox = new System.Windows.Forms.CheckBox();
            this.customColorBtn = new System.Windows.Forms.Button();
            this.maxPeakLabel = new System.Windows.Forms.Label();
            this.autoSensivityTimer = new System.Windows.Forms.Timer(this.components);
            this.checkBoxAutoSensivity = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.timerPeaksCounterReset = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblPeakPerQuantum = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnTest2 = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.hScrollBarBrgihtness = new System.Windows.Forms.HScrollBar();
            this.label8 = new System.Windows.Forms.Label();
            this.labelBrightness = new System.Windows.Forms.Label();
            tempTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // musicProgBarLeft
            // 
            this.musicProgBarLeft.Location = new System.Drawing.Point(84, 27);
            this.musicProgBarLeft.Margin = new System.Windows.Forms.Padding(4);
            this.musicProgBarLeft.Name = "musicProgBarLeft";
            this.musicProgBarLeft.Size = new System.Drawing.Size(432, 16);
            this.musicProgBarLeft.TabIndex = 16;
            // 
            // musicTimer
            // 
            this.musicTimer.Enabled = true;
            this.musicTimer.Interval = 30;
            this.musicTimer.Tick += new System.EventHandler(this.musicTimer_Tick);
            // 
            // sensivityScrollBar
            // 
            this.sensivityScrollBar.LargeChange = 5;
            this.sensivityScrollBar.Location = new System.Drawing.Point(83, 85);
            this.sensivityScrollBar.Maximum = 101;
            this.sensivityScrollBar.Name = "sensivityScrollBar";
            this.sensivityScrollBar.Size = new System.Drawing.Size(431, 13);
            this.sensivityScrollBar.TabIndex = 17;
            this.sensivityScrollBar.Value = 50;
            this.sensivityScrollBar.ValueChanged += new System.EventHandler(this.sesivityScroll_Change);
            // 
            // sensivityLabel
            // 
            this.sensivityLabel.AutoSize = true;
            this.sensivityLabel.Location = new System.Drawing.Point(528, 85);
            this.sensivityLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sensivityLabel.Name = "sensivityLabel";
            this.sensivityLabel.Size = new System.Drawing.Size(24, 17);
            this.sensivityLabel.TabIndex = 18;
            this.sensivityLabel.Text = "50";
            // 
            // musicProgBarRight
            // 
            this.musicProgBarRight.Location = new System.Drawing.Point(84, 51);
            this.musicProgBarRight.Margin = new System.Windows.Forms.Padding(4);
            this.musicProgBarRight.Name = "musicProgBarRight";
            this.musicProgBarRight.Size = new System.Drawing.Size(432, 16);
            this.musicProgBarRight.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 86);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 21;
            this.label3.Text = "Sensivity:";
            // 
            // saveSetupBtn
            // 
            this.saveSetupBtn.Location = new System.Drawing.Point(13, 639);
            this.saveSetupBtn.Margin = new System.Windows.Forms.Padding(4);
            this.saveSetupBtn.Name = "saveSetupBtn";
            this.saveSetupBtn.Size = new System.Drawing.Size(141, 28);
            this.saveSetupBtn.TabIndex = 22;
            this.saveSetupBtn.Text = "Save COM setting";
            this.saveSetupBtn.UseVisualStyleBackColor = true;
            this.saveSetupBtn.Click += new System.EventHandler(this.saveSetupBtn_Click);
            // 
            // musicControllChkBox
            // 
            this.musicControllChkBox.AutoSize = true;
            this.musicControllChkBox.Location = new System.Drawing.Point(553, 26);
            this.musicControllChkBox.Margin = new System.Windows.Forms.Padding(4);
            this.musicControllChkBox.Name = "musicControllChkBox";
            this.musicControllChkBox.Size = new System.Drawing.Size(115, 21);
            this.musicControllChkBox.TabIndex = 10;
            this.musicControllChkBox.Text = "Music Control";
            this.musicControllChkBox.UseVisualStyleBackColor = true;
            // 
            // customColorBtn
            // 
            this.customColorBtn.Location = new System.Drawing.Point(674, 639);
            this.customColorBtn.Margin = new System.Windows.Forms.Padding(4);
            this.customColorBtn.Name = "customColorBtn";
            this.customColorBtn.Size = new System.Drawing.Size(141, 28);
            this.customColorBtn.TabIndex = 23;
            this.customColorBtn.Text = "Custom Color";
            this.customColorBtn.UseVisualStyleBackColor = true;
            this.customColorBtn.Click += new System.EventHandler(this.customColorBtn_Click);
            // 
            // maxPeakLabel
            // 
            this.maxPeakLabel.AutoSize = true;
            this.maxPeakLabel.Location = new System.Drawing.Point(145, 32);
            this.maxPeakLabel.Name = "maxPeakLabel";
            this.maxPeakLabel.Size = new System.Drawing.Size(16, 17);
            this.maxPeakLabel.TabIndex = 26;
            this.maxPeakLabel.Text = "0";
            // 
            // autoSensivityTimer
            // 
            this.autoSensivityTimer.Enabled = true;
            this.autoSensivityTimer.Tick += new System.EventHandler(this.autoSensivityTimer_Tick);
            // 
            // checkBoxAutoSensivity
            // 
            this.checkBoxAutoSensivity.AutoSize = true;
            this.checkBoxAutoSensivity.Checked = true;
            this.checkBoxAutoSensivity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoSensivity.Location = new System.Drawing.Point(553, 55);
            this.checkBoxAutoSensivity.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxAutoSensivity.Name = "checkBoxAutoSensivity";
            this.checkBoxAutoSensivity.Size = new System.Drawing.Size(119, 21);
            this.checkBoxAutoSensivity.TabIndex = 10;
            this.checkBoxAutoSensivity.Text = "Auto Sensivity";
            this.checkBoxAutoSensivity.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(826, 640);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(109, 28);
            this.btnTest.TabIndex = 27;
            this.btnTest.Text = "TEST";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // timerPeaksCounterReset
            // 
            this.timerPeaksCounterReset.Enabled = true;
            this.timerPeaksCounterReset.Interval = 1000;
            this.timerPeaksCounterReset.Tick += new System.EventHandler(this.timerPeaksCounterReset_Tick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 17);
            this.label5.TabIndex = 26;
            this.label5.Text = "Current max peak:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 17);
            this.label6.TabIndex = 26;
            this.label6.Text = "Peaks per second:";
            // 
            // lblPeakPerQuantum
            // 
            this.lblPeakPerQuantum.AutoSize = true;
            this.lblPeakPerQuantum.Location = new System.Drawing.Point(145, 56);
            this.lblPeakPerQuantum.Name = "lblPeakPerQuantum";
            this.lblPeakPerQuantum.Size = new System.Drawing.Size(16, 17);
            this.lblPeakPerQuantum.TabIndex = 26;
            this.lblPeakPerQuantum.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 17);
            this.label4.TabIndex = 21;
            this.label4.Text = "Left:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 54);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 17);
            this.label7.TabIndex = 21;
            this.label7.Text = "Right:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.maxPeakLabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblPeakPerQuantum);
            this.groupBox1.Location = new System.Drawing.Point(774, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 153);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Information";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.musicProgBarLeft);
            this.groupBox2.Controls.Add(this.musicControllChkBox);
            this.groupBox2.Controls.Add(this.checkBoxAutoSensivity);
            this.groupBox2.Controls.Add(this.musicProgBarRight);
            this.groupBox2.Controls.Add(this.sensivityScrollBar);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.sensivityLabel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(724, 153);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sound";
            // 
            // btnTest2
            // 
            this.btnTest2.Location = new System.Drawing.Point(958, 640);
            this.btnTest2.Name = "btnTest2";
            this.btnTest2.Size = new System.Drawing.Size(109, 27);
            this.btnTest2.TabIndex = 32;
            this.btnTest2.Text = "TEST2";
            this.btnTest2.UseVisualStyleBackColor = true;
            this.btnTest2.Click += new System.EventHandler(this.btnTest2_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.hScrollBarBrgihtness);
            this.groupBoxSettings.Controls.Add(this.label8);
            this.groupBoxSettings.Controls.Add(this.labelBrightness);
            this.groupBoxSettings.Location = new System.Drawing.Point(17, 507);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(719, 88);
            this.groupBoxSettings.TabIndex = 33;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // hScrollBarBrgihtness
            // 
            this.hScrollBarBrgihtness.LargeChange = 5;
            this.hScrollBarBrgihtness.Location = new System.Drawing.Point(87, 36);
            this.hScrollBarBrgihtness.Maximum = 104;
            this.hScrollBarBrgihtness.Name = "hScrollBarBrgihtness";
            this.hScrollBarBrgihtness.Size = new System.Drawing.Size(431, 17);
            this.hScrollBarBrgihtness.TabIndex = 17;
            this.hScrollBarBrgihtness.Value = 20;
            this.hScrollBarBrgihtness.ValueChanged += new System.EventHandler(this.hScrollBarBrgihtness_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 36);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "Brightness";
            // 
            // labelBrightness
            // 
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(532, 36);
            this.labelBrightness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(24, 17);
            this.labelBrightness.TabIndex = 18;
            this.labelBrightness.Text = "20";
            // 
            // tempTimer
            // 
            tempTimer.Enabled = true;
            tempTimer.Tick += new System.EventHandler(this.tempTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 680);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.btnTest2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.customColorBtn);
            this.Controls.Add(this.saveSetupBtn);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Led Controller By MaorT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar musicProgBarLeft;
        private System.Windows.Forms.Timer musicTimer;
        private System.Windows.Forms.HScrollBar sensivityScrollBar;
        private System.Windows.Forms.Label sensivityLabel;
        private System.Windows.Forms.ProgressBar musicProgBarRight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button saveSetupBtn;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox musicControllChkBox;
        private System.Windows.Forms.Button customColorBtn;
        private System.Windows.Forms.Label maxPeakLabel;
        private System.Windows.Forms.Timer autoSensivityTimer;
        private System.Windows.Forms.CheckBox checkBoxAutoSensivity;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblPeakPerQuantum;
        private System.Windows.Forms.Timer timerPeaksCounterReset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnTest2;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.HScrollBar hScrollBarBrgihtness;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelBrightness;
    }
}

