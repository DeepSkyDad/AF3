namespace DeepSkyDad.AF3.ControlPanel
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
            this.comboComPort = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.richTextboxOutput = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxSyncScroll = new System.Windows.Forms.CheckBox();
            this.btnCopyOutputToClipboard = new System.Windows.Forms.Button();
            this.btnClearOutput = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.currentHoldMultiplierNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.currentMoveMultiplierNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSpeedMode = new System.Windows.Forms.ComboBox();
            this.checkBoxReverseDirection = new System.Windows.Forms.CheckBox();
            this.comboBoxStepMode = new System.Windows.Forms.ComboBox();
            this.labelTemperature = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.labelPosition = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericMoveAbsoluteSteps = new System.Windows.Forms.NumericUpDown();
            this.btnMoveAbsoluteGo = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.numericMoveRelativeSteps = new System.Windows.Forms.NumericUpDown();
            this.btnMoveRelativeMinus = new System.Windows.Forms.Button();
            this.btnMoveRelativePlus = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.btnMinus90 = new System.Windows.Forms.Button();
            this.btnMinus180 = new System.Windows.Forms.Button();
            this.btnPlus180 = new System.Windows.Forms.Button();
            this.btnPlus90 = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.motorTestStartBtn = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.motorTestDurationNumeric = new System.Windows.Forms.NumericUpDown();
            this.motorTestStepsNumeric = new System.Windows.Forms.NumericUpDown();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabControl = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.btnMinus360 = new System.Windows.Forms.Button();
            this.btnPlus360 = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.numericMaxMove = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.numericMaximumPosition = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tabFwUpgrade = new System.Windows.Forms.TabPage();
            this.btnChooseFirmware = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.textBoxFirmwareFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentHoldMultiplierNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentMoveMultiplierNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMoveAbsoluteSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMoveRelativeSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.motorTestDurationNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.motorTestStepsNumeric)).BeginInit();
            this.tabs.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaximumPosition)).BeginInit();
            this.tabFwUpgrade.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboComPort
            // 
            this.comboComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboComPort.FormattingEnabled = true;
            this.comboComPort.Location = new System.Drawing.Point(74, 9);
            this.comboComPort.Name = "comboComPort";
            this.comboComPort.Size = new System.Drawing.Size(75, 21);
            this.comboComPort.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "COM port";
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(242, -164);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(172, 13);
            this.labelTitle.TabIndex = 9;
            this.labelTitle.Text = "DSD AF3 Configurator v1.0.0";
            // 
            // btnExit
            // 
            this.btnExit.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnExit.Location = new System.Drawing.Point(586, 9);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(74, 22);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // richTextboxOutput
            // 
            this.richTextboxOutput.Location = new System.Drawing.Point(0, 45);
            this.richTextboxOutput.Name = "richTextboxOutput";
            this.richTextboxOutput.ReadOnly = true;
            this.richTextboxOutput.Size = new System.Drawing.Size(329, 385);
            this.richTextboxOutput.TabIndex = 12;
            this.richTextboxOutput.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxSyncScroll);
            this.groupBox2.Controls.Add(this.btnCopyOutputToClipboard);
            this.groupBox2.Controls.Add(this.btnClearOutput);
            this.groupBox2.Controls.Add(this.richTextboxOutput);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Window;
            this.groupBox2.Location = new System.Drawing.Point(330, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 430);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OUTPUT";
            // 
            // checkBoxSyncScroll
            // 
            this.checkBoxSyncScroll.AutoSize = true;
            this.checkBoxSyncScroll.Checked = true;
            this.checkBoxSyncScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSyncScroll.Location = new System.Drawing.Point(6, 19);
            this.checkBoxSyncScroll.Name = "checkBoxSyncScroll";
            this.checkBoxSyncScroll.Size = new System.Drawing.Size(77, 17);
            this.checkBoxSyncScroll.TabIndex = 133;
            this.checkBoxSyncScroll.Text = "Sync scroll";
            this.checkBoxSyncScroll.UseVisualStyleBackColor = true;
            // 
            // btnCopyOutputToClipboard
            // 
            this.btnCopyOutputToClipboard.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnCopyOutputToClipboard.Location = new System.Drawing.Point(148, 15);
            this.btnCopyOutputToClipboard.Name = "btnCopyOutputToClipboard";
            this.btnCopyOutputToClipboard.Size = new System.Drawing.Size(102, 22);
            this.btnCopyOutputToClipboard.TabIndex = 13;
            this.btnCopyOutputToClipboard.Text = "Copy to clipboard";
            this.btnCopyOutputToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyOutputToClipboard.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnClearOutput
            // 
            this.btnClearOutput.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnClearOutput.Location = new System.Drawing.Point(256, 15);
            this.btnClearOutput.Name = "btnClearOutput";
            this.btnClearOutput.Size = new System.Drawing.Size(66, 22);
            this.btnClearOutput.TabIndex = 11;
            this.btnClearOutput.Text = "Clear";
            this.btnClearOutput.UseVisualStyleBackColor = true;
            this.btnClearOutput.Click += new System.EventHandler(this.button1_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(24, 226);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 13);
            this.label10.TabIndex = 138;
            this.label10.Text = "C. hold multiplier (%)";
            // 
            // currentHoldMultiplierNumeric
            // 
            this.currentHoldMultiplierNumeric.Location = new System.Drawing.Point(136, 224);
            this.currentHoldMultiplierNumeric.Name = "currentHoldMultiplierNumeric";
            this.currentHoldMultiplierNumeric.Size = new System.Drawing.Size(100, 20);
            this.currentHoldMultiplierNumeric.TabIndex = 137;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 136;
            this.label4.Text = "C. move multiplier (%)";
            // 
            // currentMoveMultiplierNumeric
            // 
            this.currentMoveMultiplierNumeric.Location = new System.Drawing.Point(136, 251);
            this.currentMoveMultiplierNumeric.Name = "currentMoveMultiplierNumeric";
            this.currentMoveMultiplierNumeric.Size = new System.Drawing.Size(100, 20);
            this.currentMoveMultiplierNumeric.TabIndex = 135;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 134;
            this.label3.Text = "Speed";
            // 
            // comboBoxSpeedMode
            // 
            this.comboBoxSpeedMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpeedMode.FormattingEnabled = true;
            this.comboBoxSpeedMode.Items.AddRange(new object[] {
            "Very slow",
            "Slow",
            "Medium",
            "Fast",
            "Very fast"});
            this.comboBoxSpeedMode.Location = new System.Drawing.Point(136, 100);
            this.comboBoxSpeedMode.Name = "comboBoxSpeedMode";
            this.comboBoxSpeedMode.Size = new System.Drawing.Size(100, 21);
            this.comboBoxSpeedMode.TabIndex = 133;
            // 
            // checkBoxReverseDirection
            // 
            this.checkBoxReverseDirection.AutoSize = true;
            this.checkBoxReverseDirection.Location = new System.Drawing.Point(136, 53);
            this.checkBoxReverseDirection.Name = "checkBoxReverseDirection";
            this.checkBoxReverseDirection.Size = new System.Drawing.Size(15, 14);
            this.checkBoxReverseDirection.TabIndex = 130;
            this.checkBoxReverseDirection.UseVisualStyleBackColor = true;
            // 
            // comboBoxStepMode
            // 
            this.comboBoxStepMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStepMode.FormattingEnabled = true;
            this.comboBoxStepMode.Items.AddRange(new object[] {
            "1",
            "1/2",
            "1/4",
            "1/8",
            "1/16",
            "1/32",
            "1/64",
            "1/128",
            "1/256"});
            this.comboBoxStepMode.Location = new System.Drawing.Point(136, 73);
            this.comboBoxStepMode.Name = "comboBoxStepMode";
            this.comboBoxStepMode.Size = new System.Drawing.Size(100, 21);
            this.comboBoxStepMode.TabIndex = 82;
            // 
            // labelTemperature
            // 
            this.labelTemperature.AutoSize = true;
            this.labelTemperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTemperature.Location = new System.Drawing.Point(120, 41);
            this.labelTemperature.Name = "labelTemperature";
            this.labelTemperature.Size = new System.Drawing.Size(13, 17);
            this.labelTemperature.TabIndex = 125;
            this.labelTemperature.Text = "-";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(14, 41);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(101, 17);
            this.label25.TabIndex = 124;
            this.label25.Text = "Temperature";
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.DarkRed;
            this.btnStop.Location = new System.Drawing.Point(226, 14);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(62, 44);
            this.btnStop.TabIndex = 98;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // labelPosition
            // 
            this.labelPosition.AutoSize = true;
            this.labelPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPosition.Location = new System.Drawing.Point(120, 14);
            this.labelPosition.Name = "labelPosition";
            this.labelPosition.Size = new System.Drawing.Size(13, 17);
            this.labelPosition.TabIndex = 97;
            this.labelPosition.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 17);
            this.label7.TabIndex = 96;
            this.label7.Text = "Position";
            // 
            // numericMoveAbsoluteSteps
            // 
            this.numericMoveAbsoluteSteps.Location = new System.Drawing.Point(23, 276);
            this.numericMoveAbsoluteSteps.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericMoveAbsoluteSteps.Name = "numericMoveAbsoluteSteps";
            this.numericMoveAbsoluteSteps.Size = new System.Drawing.Size(196, 20);
            this.numericMoveAbsoluteSteps.TabIndex = 95;
            // 
            // btnMoveAbsoluteGo
            // 
            this.btnMoveAbsoluteGo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnMoveAbsoluteGo.Location = new System.Drawing.Point(225, 276);
            this.btnMoveAbsoluteGo.Name = "btnMoveAbsoluteGo";
            this.btnMoveAbsoluteGo.Size = new System.Drawing.Size(63, 23);
            this.btnMoveAbsoluteGo.TabIndex = 94;
            this.btnMoveAbsoluteGo.Text = "GO";
            this.btnMoveAbsoluteGo.UseVisualStyleBackColor = true;
            this.btnMoveAbsoluteGo.Click += new System.EventHandler(this.btnMoveAbsoluteGo_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(20, 252);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 93;
            this.label6.Text = "Move - absolute";
            // 
            // numericMoveRelativeSteps
            // 
            this.numericMoveRelativeSteps.Location = new System.Drawing.Point(96, 218);
            this.numericMoveRelativeSteps.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericMoveRelativeSteps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericMoveRelativeSteps.Name = "numericMoveRelativeSteps";
            this.numericMoveRelativeSteps.Size = new System.Drawing.Size(123, 20);
            this.numericMoveRelativeSteps.TabIndex = 92;
            this.numericMoveRelativeSteps.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // btnMoveRelativeMinus
            // 
            this.btnMoveRelativeMinus.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnMoveRelativeMinus.Location = new System.Drawing.Point(225, 215);
            this.btnMoveRelativeMinus.Name = "btnMoveRelativeMinus";
            this.btnMoveRelativeMinus.Size = new System.Drawing.Size(63, 23);
            this.btnMoveRelativeMinus.TabIndex = 91;
            this.btnMoveRelativeMinus.Text = "-";
            this.btnMoveRelativeMinus.UseVisualStyleBackColor = true;
            this.btnMoveRelativeMinus.Click += new System.EventHandler(this.btnMoveRelativeMinus_Click);
            // 
            // btnMoveRelativePlus
            // 
            this.btnMoveRelativePlus.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnMoveRelativePlus.Location = new System.Drawing.Point(23, 218);
            this.btnMoveRelativePlus.Name = "btnMoveRelativePlus";
            this.btnMoveRelativePlus.Size = new System.Drawing.Size(67, 23);
            this.btnMoveRelativePlus.TabIndex = 90;
            this.btnMoveRelativePlus.Text = "+";
            this.btnMoveRelativePlus.UseVisualStyleBackColor = true;
            this.btnMoveRelativePlus.Click += new System.EventHandler(this.btnMoveRelativePlus_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(20, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 89;
            this.label5.Text = "Move - relative";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(13, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 13);
            this.label14.TabIndex = 88;
            this.label14.Text = "Move - fixed angle";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(24, 76);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(29, 13);
            this.label19.TabIndex = 87;
            this.label19.Text = "Step";
            // 
            // btnMinus90
            // 
            this.btnMinus90.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnMinus90.Location = new System.Drawing.Point(162, 99);
            this.btnMinus90.Name = "btnMinus90";
            this.btnMinus90.Size = new System.Drawing.Size(126, 23);
            this.btnMinus90.TabIndex = 82;
            this.btnMinus90.Text = "-90  deg";
            this.btnMinus90.UseVisualStyleBackColor = true;
            this.btnMinus90.Click += new System.EventHandler(this.btnMinus90_Click);
            // 
            // btnMinus180
            // 
            this.btnMinus180.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnMinus180.Location = new System.Drawing.Point(162, 128);
            this.btnMinus180.Name = "btnMinus180";
            this.btnMinus180.Size = new System.Drawing.Size(126, 23);
            this.btnMinus180.TabIndex = 81;
            this.btnMinus180.Text = "-180 deg";
            this.btnMinus180.UseVisualStyleBackColor = true;
            this.btnMinus180.Click += new System.EventHandler(this.btnMinus180_Click);
            // 
            // btnPlus180
            // 
            this.btnPlus180.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnPlus180.Location = new System.Drawing.Point(16, 128);
            this.btnPlus180.Name = "btnPlus180";
            this.btnPlus180.Size = new System.Drawing.Size(140, 23);
            this.btnPlus180.TabIndex = 80;
            this.btnPlus180.Text = "+180  deg";
            this.btnPlus180.UseVisualStyleBackColor = true;
            this.btnPlus180.Click += new System.EventHandler(this.btnPlus180_Click);
            // 
            // btnPlus90
            // 
            this.btnPlus90.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnPlus90.Location = new System.Drawing.Point(14, 99);
            this.btnPlus90.Name = "btnPlus90";
            this.btnPlus90.Size = new System.Drawing.Size(142, 23);
            this.btnPlus90.TabIndex = 78;
            this.btnPlus90.Text = "+90 deg";
            this.btnPlus90.UseVisualStyleBackColor = true;
            this.btnPlus90.Click += new System.EventHandler(this.btnPlus90_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnConnect.Location = new System.Drawing.Point(155, 8);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(65, 23);
            this.btnConnect.TabIndex = 79;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // motorTestStartBtn
            // 
            this.motorTestStartBtn.ForeColor = System.Drawing.SystemColors.InfoText;
            this.motorTestStartBtn.Location = new System.Drawing.Point(229, 331);
            this.motorTestStartBtn.Name = "motorTestStartBtn";
            this.motorTestStartBtn.Size = new System.Drawing.Size(59, 54);
            this.motorTestStartBtn.TabIndex = 142;
            this.motorTestStartBtn.Text = "START";
            this.motorTestStartBtn.UseVisualStyleBackColor = true;
            this.motorTestStartBtn.Click += new System.EventHandler(this.motorTestStartBtn_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 367);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(34, 13);
            this.label12.TabIndex = 141;
            this.label12.Text = "Steps";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 333);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 13);
            this.label11.TabIndex = 139;
            this.label11.Text = "Duration (min)";
            // 
            // motorTestDurationNumeric
            // 
            this.motorTestDurationNumeric.Location = new System.Drawing.Point(99, 332);
            this.motorTestDurationNumeric.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.motorTestDurationNumeric.Name = "motorTestDurationNumeric";
            this.motorTestDurationNumeric.Size = new System.Drawing.Size(124, 20);
            this.motorTestDurationNumeric.TabIndex = 140;
            this.motorTestDurationNumeric.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // motorTestStepsNumeric
            // 
            this.motorTestStepsNumeric.Location = new System.Drawing.Point(99, 365);
            this.motorTestStepsNumeric.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.motorTestStepsNumeric.Name = "motorTestStepsNumeric";
            this.motorTestStepsNumeric.Size = new System.Drawing.Size(124, 20);
            this.motorTestStepsNumeric.TabIndex = 139;
            this.motorTestStepsNumeric.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabControl);
            this.tabs.Controls.Add(this.tabSettings);
            this.tabs.Controls.Add(this.tabFwUpgrade);
            this.tabs.Location = new System.Drawing.Point(12, 42);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(310, 430);
            this.tabs.TabIndex = 141;
            // 
            // tabControl
            // 
            this.tabControl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabControl.Controls.Add(this.label8);
            this.tabControl.Controls.Add(this.labelTemperature);
            this.tabControl.Controls.Add(this.btnMinus360);
            this.tabControl.Controls.Add(this.btnPlus360);
            this.tabControl.Controls.Add(this.label25);
            this.tabControl.Controls.Add(this.motorTestStartBtn);
            this.tabControl.Controls.Add(this.labelPosition);
            this.tabControl.Controls.Add(this.btnStop);
            this.tabControl.Controls.Add(this.label7);
            this.tabControl.Controls.Add(this.btnPlus90);
            this.tabControl.Controls.Add(this.label12);
            this.tabControl.Controls.Add(this.numericMoveAbsoluteSteps);
            this.tabControl.Controls.Add(this.label11);
            this.tabControl.Controls.Add(this.btnMoveAbsoluteGo);
            this.tabControl.Controls.Add(this.motorTestDurationNumeric);
            this.tabControl.Controls.Add(this.btnPlus180);
            this.tabControl.Controls.Add(this.motorTestStepsNumeric);
            this.tabControl.Controls.Add(this.label6);
            this.tabControl.Controls.Add(this.btnMinus180);
            this.tabControl.Controls.Add(this.numericMoveRelativeSteps);
            this.tabControl.Controls.Add(this.btnMinus90);
            this.tabControl.Controls.Add(this.btnMoveRelativeMinus);
            this.tabControl.Controls.Add(this.label14);
            this.tabControl.Controls.Add(this.btnMoveRelativePlus);
            this.tabControl.Controls.Add(this.label5);
            this.tabControl.Location = new System.Drawing.Point(4, 22);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Windows.Forms.Padding(3);
            this.tabControl.Size = new System.Drawing.Size(302, 404);
            this.tabControl.TabIndex = 0;
            this.tabControl.Text = "Control";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(20, 306);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(136, 13);
            this.label8.TabIndex = 145;
            this.label8.Text = "Continuous motion test";
            // 
            // btnMinus360
            // 
            this.btnMinus360.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnMinus360.Location = new System.Drawing.Point(162, 157);
            this.btnMinus360.Name = "btnMinus360";
            this.btnMinus360.Size = new System.Drawing.Size(126, 23);
            this.btnMinus360.TabIndex = 144;
            this.btnMinus360.Text = "+360  deg";
            this.btnMinus360.UseVisualStyleBackColor = true;
            this.btnMinus360.Click += new System.EventHandler(this.btnMinus360_Click);
            // 
            // btnPlus360
            // 
            this.btnPlus360.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnPlus360.Location = new System.Drawing.Point(16, 157);
            this.btnPlus360.Name = "btnPlus360";
            this.btnPlus360.Size = new System.Drawing.Size(140, 23);
            this.btnPlus360.TabIndex = 143;
            this.btnPlus360.Text = "+360  deg";
            this.btnPlus360.UseVisualStyleBackColor = true;
            this.btnPlus360.Click += new System.EventHandler(this.btnPlus360_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabSettings.Controls.Add(this.button2);
            this.tabSettings.Controls.Add(this.label17);
            this.tabSettings.Controls.Add(this.label16);
            this.tabSettings.Controls.Add(this.numericMaxMove);
            this.tabSettings.Controls.Add(this.label15);
            this.tabSettings.Controls.Add(this.numericMaximumPosition);
            this.tabSettings.Controls.Add(this.label13);
            this.tabSettings.Controls.Add(this.label9);
            this.tabSettings.Controls.Add(this.checkBoxReverseDirection);
            this.tabSettings.Controls.Add(this.comboBoxStepMode);
            this.tabSettings.Controls.Add(this.label19);
            this.tabSettings.Controls.Add(this.label10);
            this.tabSettings.Controls.Add(this.comboBoxSpeedMode);
            this.tabSettings.Controls.Add(this.currentHoldMultiplierNumeric);
            this.tabSettings.Controls.Add(this.label3);
            this.tabSettings.Controls.Add(this.label4);
            this.tabSettings.Controls.Add(this.currentMoveMultiplierNumeric);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(302, 404);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.SystemColors.InfoText;
            this.button2.Location = new System.Drawing.Point(117, 369);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(59, 29);
            this.button2.TabIndex = 146;
            this.button2.Text = "SAVE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(24, 195);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(77, 13);
            this.label17.TabIndex = 145;
            this.label17.Text = "Coils control";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(24, 155);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(80, 13);
            this.label16.TabIndex = 144;
            this.label16.Text = "Maximum move";
            // 
            // numericMaxMove
            // 
            this.numericMaxMove.Location = new System.Drawing.Point(136, 153);
            this.numericMaxMove.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.numericMaxMove.Name = "numericMaxMove";
            this.numericMaxMove.Size = new System.Drawing.Size(100, 20);
            this.numericMaxMove.TabIndex = 143;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 129);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(90, 13);
            this.label15.TabIndex = 142;
            this.label15.Text = "Maximum position";
            // 
            // numericMaximumPosition
            // 
            this.numericMaximumPosition.Location = new System.Drawing.Point(136, 127);
            this.numericMaximumPosition.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericMaximumPosition.Name = "numericMaximumPosition";
            this.numericMaximumPosition.Size = new System.Drawing.Size(100, 20);
            this.numericMaximumPosition.TabIndex = 141;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(24, 53);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(90, 13);
            this.label13.TabIndex = 140;
            this.label13.Text = "Reverse direction";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(24, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 139;
            this.label9.Text = "Movement";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // tabFwUpgrade
            // 
            this.tabFwUpgrade.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabFwUpgrade.Controls.Add(this.btnChooseFirmware);
            this.tabFwUpgrade.Controls.Add(this.btnUpload);
            this.tabFwUpgrade.Controls.Add(this.textBoxFirmwareFile);
            this.tabFwUpgrade.Controls.Add(this.label2);
            this.tabFwUpgrade.Location = new System.Drawing.Point(4, 22);
            this.tabFwUpgrade.Name = "tabFwUpgrade";
            this.tabFwUpgrade.Padding = new System.Windows.Forms.Padding(3);
            this.tabFwUpgrade.Size = new System.Drawing.Size(302, 404);
            this.tabFwUpgrade.TabIndex = 2;
            this.tabFwUpgrade.Text = "Firmware upgrade";
            // 
            // btnChooseFirmware
            // 
            this.btnChooseFirmware.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnChooseFirmware.Location = new System.Drawing.Point(20, 49);
            this.btnChooseFirmware.Name = "btnChooseFirmware";
            this.btnChooseFirmware.Size = new System.Drawing.Size(129, 23);
            this.btnChooseFirmware.TabIndex = 1;
            this.btnChooseFirmware.Text = "Browse";
            this.btnChooseFirmware.UseVisualStyleBackColor = true;
            this.btnChooseFirmware.Click += new System.EventHandler(this.btnChooseFirmware_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpload.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnUpload.Location = new System.Drawing.Point(155, 49);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(130, 23);
            this.btnUpload.TabIndex = 3;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // textBoxFirmwareFile
            // 
            this.textBoxFirmwareFile.Location = new System.Drawing.Point(71, 23);
            this.textBoxFirmwareFile.Name = "textBoxFirmwareFile";
            this.textBoxFirmwareFile.ReadOnly = true;
            this.textBoxFirmwareFile.Size = new System.Drawing.Size(214, 20);
            this.textBoxFirmwareFile.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "DSD file";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(675, 487);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.comboComPort);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConnect);
            this.ForeColor = System.Drawing.SystemColors.Window;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentHoldMultiplierNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentMoveMultiplierNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMoveAbsoluteSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMoveRelativeSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.motorTestDurationNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.motorTestStepsNumeric)).EndInit();
            this.tabs.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabControl.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaximumPosition)).EndInit();
            this.tabFwUpgrade.ResumeLayout(false);
            this.tabFwUpgrade.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboComPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.RichTextBox richTextboxOutput;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCopyOutputToClipboard;
        private System.Windows.Forms.Button btnClearOutput;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericMoveAbsoluteSteps;
        private System.Windows.Forms.Button btnMoveAbsoluteGo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericMoveRelativeSteps;
        private System.Windows.Forms.Button btnMoveRelativeMinus;
        private System.Windows.Forms.Button btnMoveRelativePlus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnMinus90;
        private System.Windows.Forms.Button btnMinus180;
        private System.Windows.Forms.Button btnPlus180;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnPlus90;
        private System.Windows.Forms.Label labelTemperature;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.ComboBox comboBoxStepMode;
        private System.Windows.Forms.CheckBox checkBoxReverseDirection;
        private System.Windows.Forms.CheckBox checkBoxSyncScroll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxSpeedMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown currentMoveMultiplierNumeric;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown currentHoldMultiplierNumeric;
        private System.Windows.Forms.Button motorTestStartBtn;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown motorTestDurationNumeric;
        private System.Windows.Forms.NumericUpDown motorTestStepsNumeric;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabControl;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabFwUpgrade;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TextBox textBoxFirmwareFile;
        private System.Windows.Forms.Button btnChooseFirmware;
        private System.Windows.Forms.Button btnMinus360;
        private System.Windows.Forms.Button btnPlus360;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown numericMaxMove;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown numericMaximumPosition;
        private System.Windows.Forms.Label label13;
    }
}

