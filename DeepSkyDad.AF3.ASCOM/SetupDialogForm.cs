using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ASCOM.Utilities;

namespace ASCOM.DeepSkyDad.AF3
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        private FocuserTemplate _f;

        public SetupDialogForm(FocuserTemplate f)
        {
            _f = f;
            InitializeComponent();
            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here
            // Update the state variables with results from the dialogue
            FocuserTemplate.comPort = (string)comboBoxComPort.SelectedItem;
            FocuserTemplate.maxPosition = (int)numericUpMaxPosition.Value;
            FocuserTemplate.maxMovement = (int)numericUpMaxMovement.Value;
            FocuserTemplate.stepSize = (string)comboBoxStepSize.SelectedItem;
            FocuserTemplate.speedMode = (string)comboBoxSpeedMode.SelectedItem;
            FocuserTemplate.traceState = chkTrace.Checked;
            FocuserTemplate.resetOnConnect = chkResetOnConnect.Checked;
            FocuserTemplate.setPositonOnConnect = chkSetPositionOnConnect.Checked;
            if(FocuserTemplate.setPositonOnConnect)
                FocuserTemplate.setPositionOnConnectValue = (int)numericSetPositionOnConnectValue.Value;
            FocuserTemplate.motorMoveCurrentMultiplier = (int)moveCurrentMultiplierNumeric.Value;
            FocuserTemplate.motorHoldCurrentMultiplier = (int)holdCurrentMultiplierNumeric.Value;
            FocuserTemplate.reverseDirection = chkReverseDirection.Checked;
            FocuserTemplate.settleBuffer = (int)numericUpDownSettleBuffer.Value;
            FocuserTemplate.temperatureCompensation = chkTmpComp.Checked;
        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {
            this.Size = new Size(this.Size.Width - advancedPanel.Width, this.Height);

            chkTrace.Checked = FocuserTemplate.traceState;
            numericUpMaxPosition.Value = FocuserTemplate.maxPosition;
            numericUpMaxMovement.Value = FocuserTemplate.maxMovement;
            comboBoxStepSize.Text = FocuserTemplate.stepSize;
            comboBoxSpeedMode.Text = FocuserTemplate.speedMode;
            chkResetOnConnect.Checked = FocuserTemplate.resetOnConnect;
            chkSetPositionOnConnect.Checked = FocuserTemplate.setPositonOnConnect;
            numericSetPositionOnConnectValue.Value = FocuserTemplate.setPositionOnConnectValue;
            numericSetPositionOnConnectValue.Visible = FocuserTemplate.setPositonOnConnect;
            chkReverseDirection.Checked = FocuserTemplate.reverseDirection;
            numericUpDownSettleBuffer.Value = FocuserTemplate.settleBuffer;
            // set the list of com ports to those that are currently available
            comboBoxComPort.Items.Clear();
            comboBoxComPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());      // use System.IO because it's static
            // select the current port if possible
            if (comboBoxComPort.Items.Contains(FocuserTemplate.comPort))
            {
                comboBoxComPort.SelectedItem = FocuserTemplate.comPort;
            }
            chkTmpComp.Checked = FocuserTemplate.temperatureCompensation;
            moveCurrentMultiplierNumeric.Value = FocuserTemplate.motorMoveCurrentMultiplier;
            holdCurrentMultiplierNumeric.Value = FocuserTemplate.motorHoldCurrentMultiplier;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chkAlwaysOn_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkResetOnConnet_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkTrace_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonFirmwareInfo_Click(object sender, EventArgs e)
        {
            var comPort = (string)comboBoxComPort.SelectedItem;
            if (string.IsNullOrWhiteSpace(comPort))
            {
                MessageBox.Show("Please select COM port", "Error");
                return;
            }

            try
            {
                ShowNonBlockingMessageBox($"Required: {_f.GetFirmwareVersion()}.X\r\nInstalled: {_f.GetInstalledFirmwareVersion()}", "Firmware version");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Firmware version check failed ({ex.Message})", "Error");
            }
        }

        private void ShowNonBlockingMessageBox(string text, string caption)
        {
            Thread t = new Thread(() => MessageBox.Show(text, caption));
            t.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void chkSetPositionOnConnect_CheckedChanged(object sender, EventArgs e)
        {
            numericSetPositionOnConnectValue.Visible = chkSetPositionOnConnect.Checked;
        }

        private void buttonReboot_Click(object sender, EventArgs e)
        {
            var comPort = (string)comboBoxComPort.SelectedItem;
            if (string.IsNullOrWhiteSpace(comPort))
            {
                MessageBox.Show("Please select COM port", "Error");
                return;
            }

            try
            {
                FocuserTemplate.comPort = comPort;
                _f.Connected = true;
                _f.CommandBlind("RBOT");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection to the focuser failed ({ex.Message})", "Error");
            }
            finally
            {
                if (_f.Connected)
                    _f.Disconnect();
            }

            MessageBox.Show("Reboot successful!");
        }

        private void showAdvancedBtn_Click(object sender, EventArgs e)
        {
            if(this.advancedPanel.Visible)
            {
                this.showAdvancedBtn.Text = "Advanced >>";
                this.advancedPanel.Visible = false;
                this.Size = new Size(this.Width - this.advancedPanel.Width, this.Height);
            } else
            {
                this.showAdvancedBtn.Text = "Advanced <<";
                this.advancedPanel.Visible = true;
                this.Size = new Size(this.Size.Width + advancedPanel.Width, this.Height);
               
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxSpeedMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
