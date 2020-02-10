using CliWrap;
using CliWrap.Services;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DeepSkyDad.AF3.ControlPanel.FirmwareUpdateService;
using static DeepSkyDad.AF3.ControlPanel.SerialService;

namespace DeepSkyDad.AF3.ControlPanel
{
    public partial class Form1 : Form
    {
        private static PrivateFontCollection _fonts = new PrivateFontCollection();

        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        private SerialService _serialService;
        private FirmwareUpdateService _firmwareUpdateService;

        private bool _isUploadingFirmware;
        private bool _isTesting;

        ///
        /// Handling the window messages
        ///
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }

        public Form1()
        {
            InitializeComponent();

            //var test = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            AddFontFromResource("DeepSkyDad.AF3.ControlPanel.EmbeddedFiles.TitilliumWeb-Light.ttf");
            AddFontFromResource("DeepSkyDad.AF3.ControlPanel.EmbeddedFiles.AlegreyaSansSC-Bold.ttf");
            //_fonts.AddFontFile("TitilliumWeb-Light.ttf");
            //_fonts.AddFontFile("AlegreyaSansSC-Bold.ttf");

            this.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("DeepSkyDad.AF3.ControlPanel.EmbeddedFiles.favicon.ico"));

            this.ControlBox = false;
            this.Text = String.Empty;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Font = new Font(_fonts.Families[1], 9, FontStyle.Regular);
            this.ForeColor = Color.White;
            this.BackColor = Color.FromArgb(24, 24, 24);

            this.labelTitle.Font = new Font(_fonts.Families[0], 12, FontStyle.Bold);
            this.labelTitle.Left = (this.ClientSize.Width - this.labelTitle.Width) / 2;
            this.labelTitle.Top = 16;
            this.labelTitle.Text = $"DSD AF3 Control Panel v{Application.ProductVersion}";

            this.comboComPort.DropDown += comPortCombo_DropDown;

            _serialService = new SerialService(
                (SerialServiceStatus status) =>
                {
                    if (status == SerialServiceStatus.Connected)
                    {
                        if(_serialService.SendCommand("[GPOS]", true, false).Result == "(ERROR)")
                        {
                            richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { "AF3 connection failed", true });
                            _serialService.Disconnect();
                            return;
                        }

                        btnConnect.Text = "Disconnect";
                        richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { "AF3 Connected", false });
                        RefreshUI();
                        ReadFocuserState();
                    }
                    else
                    {
                        btnConnect.Text = "Connect";
                        richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { "AF3 Disconnected", false });
                        RefreshUI();
                    }
                },
                (string serialDataReceived, bool isError) =>
                {
                    richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { $"{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")}: {serialDataReceived}", isError });
                }
            );

            _firmwareUpdateService = new FirmwareUpdateService(
                (FirmwareUpdateStatus status) =>
                {
                    _isUploadingFirmware = false;
                    if (status == FirmwareUpdateStatus.Uploading)
                    {
                        _isUploadingFirmware = true;
                        richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { "UPLOADING...", false });
                    }
                    else if (status == FirmwareUpdateStatus.Successful)
                    {
                        richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { "UPLOAD SUCCESSFUL!", false });
                    }
                    else if (status == FirmwareUpdateStatus.Error)
                    {
                        richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { "FIRMWARE UPLOAD FAILED!", true});
                    }

                    RefreshUI();
                },
                (string text, bool isError) =>
                {
                    richTextboxOutput.Invoke(new AppendOutputTextDelegate(AppendOutputText), new Object[] { text, isError });
                }
            );

            richTextboxOutput.TextChanged += (object sender, EventArgs e) =>
            {
                if (!checkBoxSyncScroll.Checked)
                    return;
                richTextboxOutput.SelectionStart = richTextboxOutput.Text.Length;
                richTextboxOutput.ScrollToCaret();
            };

            ClearUI();
            RefreshUI();

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = (2000);
            timer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                if (_isTesting)
                    return;
                var result = ReadPositionAndTemperature(false, false).Result;
            });
            timer.Start();
        }

        private void comPortCombo_DropDown(object sender, EventArgs e)
        {
            this.comboComPort.Items.Clear();
            var i = 0;
            foreach (var port in SerialPort.GetPortNames())
            {
                this.comboComPort.Items.Insert(i, port);
                i++;
            }
        }

        private void btnChooseFirmware_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Choose firmware .dsd file";
            theDialog.Filter = "DSD files|*.dsd";
            theDialog.InitialDirectory = KnownFolders.Downloads.Path;
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxFirmwareFile.Text = theDialog.FileName.ToString();
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.comboComPort.Text))
            {
                AppendOutputText("Please, select COM port", true);
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.textBoxFirmwareFile.Text))
            {
                AppendOutputText("Please, select firmare DSD file", true);
                return;
            }

            await _firmwareUpdateService.UploadFirmwareArduinoNano(this.comboComPort.Text, this.textBoxFirmwareFile.Text);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void AddFontFromResource(string fontResourceName)
        {
            var fontBytes = GetFontResourceBytes(fontResourceName);
            var fontData = Marshal.AllocCoTaskMem(fontBytes.Length);
            Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length);
            _fonts.AddMemoryFont(fontData, fontBytes.Length);
            Marshal.FreeCoTaskMem(fontData);
        }

        private static byte[] GetFontResourceBytes(string fontResourceName)
        {
            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fontResourceName);
            if (resourceStream == null)
                throw new Exception(string.Format("Unable to find font '{0}' in embedded resources.", fontResourceName));
            var fontBytes = new byte[resourceStream.Length];
            resourceStream.Read(fontBytes, 0, (int)resourceStream.Length);
            resourceStream.Close();
            return fontBytes;
        }

        private void RefreshUI()
        {
            comboComPort.Enabled = !_serialService.isConnected() && !_isUploadingFirmware;
            btnExit.Enabled = !_isUploadingFirmware;

            foreach (Control c in tabControl.Controls)
            {
                if (c.GetType() == typeof(Label))
                    continue;
                c.Enabled = _serialService.isConnected() && !_isTesting;
            }

            foreach (Control c in tabSettings.Controls)
            {
                if (c.GetType() == typeof(Label))
                    continue;
                c.Enabled = _serialService.isConnected() && !_isTesting;
            }

            foreach (Control c in tabFwUpgrade.Controls)
            {
                if (c.GetType() == typeof(Label))
                    continue;
                c.Enabled = !_isUploadingFirmware && !_serialService.isConnected();
            }

            btnStop.Enabled = _serialService.isConnected();
        }

        private async void ChangeMaxMoveIfNeccessary(int steps)
        {
            if (steps > numericMaxMove.Value && steps <= numericMaxMove.Maximum)
            {
                numericMaxMove.Value = steps;
                await _serialService.SendCommand($"[SMXM{steps}]");
            }
        }

        private void ClearUI()
        {
            labelPosition.Text = "-";
            labelTemperature.Text = "-";
            comboBoxStepMode.Text = "1/2";
            currentHoldMultiplierNumeric.Value = 0;
            currentMoveMultiplierNumeric.Value = 0;
            numericMoveAbsoluteSteps.Value = 0;
        }

        private async void ReadFocuserState()
        {
            await ReadPositionAndTemperature();

            checkBoxReverseDirection.Checked = await _serialService.SendCommand("[GREV]") == "1";

            var stepMode = await _serialService.SendCommand("[GSTP]");
            switch (stepMode)
            {
                case "1":
                    stepMode = "1";
                    break;
                case "2":
                    stepMode = "1/2";
                    break;
                case "4":
                    stepMode = "1/4";
                    break;
                case "8":
                    stepMode = "1/8";
                    break;
                case "16":
                    stepMode = "1/16";
                    break;
                case "32":
                    stepMode = "1/32";
                    break;
                case "64":
                    stepMode = "1/64";
                    break;
                case "128":
                    stepMode = "1/128";
                    break;
                case "256":
                    stepMode = "1/256";
                    break;
                default:
                    throw new Exception("Invalid step size: " + stepMode);
            }
            comboBoxStepMode.Text = stepMode;

            var speedMode = await _serialService.SendCommand("[GSPD]");
            switch (speedMode)
            {
                case "1":
                    speedMode = "Very slow";
                    break;
                case "2":
                    speedMode = "Slow";
                    break;
                case "3":
                    speedMode = "Medium";
                    break;
                case "4":
                    speedMode = "Fast";
                    break;
                case "5":
                    speedMode = "Very fast";
                    break;
                default:
                    speedMode = "Slow";
                    break;
            }
            comboBoxSpeedMode.Text = speedMode;

            numericMaximumPosition.Value = Convert.ToInt32(await _serialService.SendCommand("[GMXP]"));
            numericMaxMove.Value = Convert.ToInt32(await _serialService.SendCommand("[GMXM]"));
            currentMoveMultiplierNumeric.Value = Convert.ToInt32(await _serialService.SendCommand("[GMMM]"));
            currentHoldMultiplierNumeric.Value = Convert.ToInt32(await _serialService.SendCommand("[GMHM]"));
        }

        private async Task<bool> ReadPositionAndTemperature(bool isRefreshAbsolutePositionField = true, bool isOutputSerial = true)
        {
            if (!_serialService.isConnected())
                return false;

            var position = await _serialService.SendCommand("[GPOS]", true, isOutputSerial);
            var tmpC = await _serialService.SendCommand("[GTMC]", true, isOutputSerial);

            if (!_serialService.isConnected())
                return false;

            labelPosition.Text = position;
            if(isRefreshAbsolutePositionField)
                numericMoveAbsoluteSteps.Value = Convert.ToInt32(position);
         
            labelTemperature.Text = tmpC == "-127.00" ? "Disconnected" : tmpC;
            return true;
        }

        private void MoveForAngle(int angle, string stepMode)
        {
            int steps;
            switch (stepMode)
            {
                case "1":
                    steps = 200;
                    break;
                case "1/2":
                    steps = 400;
                    break;
                case "1/4":
                    steps = 800;
                    break;
                case "1/8":
                    steps = 1600;
                    break;
                case "1/16":
                    steps = 3100;
                    break;
                case "1/32":
                    steps = 6400;
                    break;
                case "1/64":
                    steps = 12800;
                    break;
                case "1/128":
                    steps = 25600;
                    break;
                case "1/256":
                    steps = 51200;
                    break;
                default:
                    throw new Exception("Invalid step size: " + stepMode);
            }

            MoveForSteps(steps / (360 / angle));
        }

        private async void MoveForSteps(int steps)
        {
            ChangeMaxMoveIfNeccessary(steps);
            var pos = Convert.ToInt32(await _serialService.SendCommand("[GPOS]"));
            var target = pos + steps;

            await _serialService.SendCommand($"[STRG{target}]");
            await _serialService.SendCommand("[SMOV]");
        }

        public delegate void AppendOutputTextDelegate(string text, bool isError);

        public void AppendOutputText(string text)
        {
            AppendOutputText(text, false);
        }

        public void AppendOutputText(String text, bool isError)
        {
            richTextboxOutput.AppendText($"{text}\r\n", isError ? Color.Red : Color.Green);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextboxOutput.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextboxOutput.Text = string.Empty;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.comboComPort.Text))
            {
                AppendOutputText("Please choose COM port", true);
                return;
            }

            try
            {
                if (_serialService.isConnected())
                {
                    _serialService.Disconnect();
                    ClearUI();
                }
                else
                {
                    _serialService.Connect(this.comboComPort.Text);
                }
            }
            catch (Exception ex)
            {
                AppendOutputText(ex.Message, true);
            }

            RefreshUI();
        }

        private void btnPlus90_Click(object sender, EventArgs e)
        {
            var stepMode = comboBoxStepMode.Text;
            MoveForAngle(90, stepMode);
        }

        private void btnMinus90_Click(object sender, EventArgs e)
        {
            var stepMode = comboBoxStepMode.Text;
            MoveForAngle(-90, stepMode);
        }

        private void btnPlus180_Click(object sender, EventArgs e)
        {
            var stepMode = comboBoxStepMode.Text;
            MoveForAngle(180, stepMode);
        }

        private void btnMinus180_Click(object sender, EventArgs e)
        {
            var stepMode = comboBoxStepMode.Text;
            MoveForAngle(-180, stepMode);
        }

        private void btnMoveRelativePlus_Click(object sender, EventArgs e)
        {
            MoveForSteps((int)numericMoveRelativeSteps.Value);
        }

        private void btnMoveRelativeMinus_Click(object sender, EventArgs e)
        {
            MoveForSteps(-1*(int)numericMoveRelativeSteps.Value);
        }

        private async void btnMoveAbsoluteGo_Click(object sender, EventArgs e)
        {
            ChangeMaxMoveIfNeccessary(Math.Abs(Convert.ToInt32(labelPosition.Text) - (int)numericMoveAbsoluteSteps.Value));
            await _serialService.SendCommand($"[STRG{(int)numericMoveAbsoluteSteps.Value}]");
            await _serialService.SendCommand("[SMOV]");
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            _isTesting = false;
            await _serialService.SendCommand("[STOP]");
            RefreshUI();
        }

        private async void motorTestStartBtn_Click(object sender, EventArgs e)
        {
            var moveSteps = (int)motorTestStepsNumeric.Value;
            ChangeMaxMoveIfNeccessary(moveSteps);
            _isTesting = true;
            RefreshUI();
            var pos = Convert.ToInt32(await _serialService.SendCommand("[GPOS]")) + moveSteps;
            var factor = -1;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (_isTesting)
            { 
                var result = await _serialService.SendCommand($"[STRG{pos}]");
                if (result == "101")
                    break;
                await _serialService.SendCommand("[SMOV]");

                while(await _serialService.SendCommand("[GMOV]") == "1")
                {
                    await Task.Delay(2000);
                }

                if(!_serialService.isConnected() || sw.Elapsed.TotalMinutes >= (double)motorTestDurationNumeric.Value)
                {
                    _isTesting = false;
                    RefreshUI();
                }

                pos = pos + (moveSteps * factor);
                factor *= -1;
                await Task.Delay(1000);
            }
        }

        private void motorTestStopBtn_Click(object sender, EventArgs e)
        {
            _isTesting = false;
            RefreshUI();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void btnPlus360_Click(object sender, EventArgs e)
        {
            var stepMode = comboBoxStepMode.Text;
            MoveForAngle(360, stepMode);
        }

        private void btnMinus360_Click(object sender, EventArgs e)
        {
            var stepMode = comboBoxStepMode.Text;
            MoveForAngle(-360, stepMode);
        }

        private async void button2_Click_1(object sender, EventArgs e)
        {
            string stepMode;
            switch (comboBoxStepMode.Text)
            {
                case "1":
                    stepMode = "0";
                    break;
                case "1/2":
                    stepMode = "2";
                    break;
                case "1/4":
                    stepMode = "4";
                    break;
                case "1/8":
                    stepMode = "8";
                    break;
                case "1/16":
                    stepMode = "16";
                    break;
                case "1/32":
                    stepMode = "32";
                    break;
                case "1/64":
                    stepMode = "64";
                    break;
                case "1/128":
                    stepMode = "128";
                    break;
                case "1/256":
                    stepMode = "256";
                    break;
                default:
                    throw new Exception("Invalid step size: " + comboBoxStepMode.Text);
            }

            await _serialService.SendCommand($"[SSTP{stepMode}]");

            string speedMode;
            switch (comboBoxSpeedMode.Text)
            {
                case "Very slow":
                    speedMode = "1";
                    break;
                case "Slow":
                    speedMode = "2";
                    break;
                case "Medium":
                    speedMode = "3";
                    break;
                case "Fast":
                    speedMode = "4";
                    break;
                case "Very fast":
                    speedMode = "5";
                    break;
                default:
                    speedMode = "1";
                    break;
            }

            await _serialService.SendCommand($"[SSPD{speedMode}]");
            await _serialService.SendCommand($"[SSPD{speedMode}]");
            await _serialService.SendCommand($"[SMXP{numericMaximumPosition.Value}]");
            await _serialService.SendCommand($"[SMXM{numericMaxMove.Value}]");
            await _serialService.SendCommand($"[SREV{(checkBoxReverseDirection.Checked == true ? 1 : 0)}]");
            await _serialService.SendCommand($"[SMMM{currentMoveMultiplierNumeric.Value}]");
            await _serialService.SendCommand($"[SMHM{currentHoldMultiplierNumeric.Value}]");
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
