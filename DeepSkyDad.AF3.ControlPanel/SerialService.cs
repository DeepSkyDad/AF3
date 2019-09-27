using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeepSkyDad.AF3.ControlPanel
{
    public class SerialService
    {
        Action<SerialServiceStatus> _statusUpdateHandler;
        Action<string, bool> _outputTextHandler;
        bool _isCallOutputTextHandler;
        static object _lockObj = new object();

        public enum SerialServiceStatus
        {
            Disconnected,
            Connected
        }

        private SerialPort _port = null;
        private bool _portIsConnected = false;
        private string _currentResponse;

        public SerialService(Action<SerialServiceStatus> statusUpdateHandler, Action<string, bool> outputTextHandler)
        {
            _statusUpdateHandler = statusUpdateHandler;
            _outputTextHandler = outputTextHandler;
            _isCallOutputTextHandler = true;
        }

        public async void Connect(string comPort)
        {
            try
            {
                if (_portIsConnected)
                    return;

                if (_port == null)
                {
                    _port = new SerialPort();
                    _port.BaudRate = 115200;
                    _port.ReadTimeout = 3000;
                    _port.WriteTimeout = 3000;
                    _port.Encoding = Encoding.ASCII;
                    _port.DtrEnable = false;
                    _port.RtsEnable = false;
                    _port.Handshake = Handshake.None;
                    _port.Parity = Parity.None;
                    _port.DataBits = 8;
                    _port.StopBits = StopBits.One;
                    _port.DataReceived += new SerialDataReceivedEventHandler(PortDataReceived);
                }

                _port.PortName = comPort;
                _port.Open();

                await Task.Delay(2000);

                _portIsConnected = true;
                _statusUpdateHandler(SerialServiceStatus.Connected);
            }
            catch(Exception ex)
            {
                _outputTextHandler(ex.Message, true);
            } 
        }

        public void Disconnect()
        {
            if (!_portIsConnected)
                return;
        
            _portIsConnected = false;
            _statusUpdateHandler(SerialServiceStatus.Disconnected);
            _port.DiscardOutBuffer();
            _port.DiscardInBuffer();
            _port.Close();
            _port.Dispose();
            _port = null;
        }

        public async Task<string> SendCommand(string cmd, bool waitResponse = true, bool isOutputSerial = true)
        {
            try
            {
                if (!_portIsConnected)
                    return null;

                lock(_lockObj)
                {
                    _port.DiscardOutBuffer();
                    _port.DiscardInBuffer();

                    try
                    {
                        _currentResponse = string.Empty;
                        _port.Write(cmd);
                        if (isOutputSerial && _isCallOutputTextHandler)
                            _outputTextHandler(cmd, false);
                    }
                    catch
                    {
                        _port.DiscardOutBuffer();
                        _port.DiscardInBuffer();
                        throw;
                    }

                    if (waitResponse)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        while (_currentResponse?.EndsWith(")") != true)
                        {
                            if (sw.ElapsedMilliseconds > _port.ReadTimeout)
                            {
                                _port.DiscardOutBuffer();
                                _port.DiscardInBuffer();
                                throw new Exception($"Timed out while waiting for command {cmd} response");
                            }
                            else
                            {
                                Thread.Sleep(1);
                            }
                        }

                        if (isOutputSerial && _isCallOutputTextHandler)
                            _outputTextHandler(_currentResponse, _currentResponse.StartsWith("(!"));

                        return _currentResponse.Substring(1, _currentResponse.Length - 2);
                    }
                }

                return null;
            } catch(Exception ex)
            {
                if (_isCallOutputTextHandler)
                    _outputTextHandler($"Command execution failed: {ex.Message}", true);
                return "(ERROR)";
            }
         
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _currentResponse += _port.ReadExisting();
        }
    }
}
