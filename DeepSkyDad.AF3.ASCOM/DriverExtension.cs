using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace ASCOM.DeepSkyDad.AF1
{
    public partial class Focuser
    {
        //private const long commandTimeoutMs = 130;
        private Version minimumFirmwareVerson = new Version("5.60");

        //for locking serial communication
        private Mutex serialMutex = null;
        private bool createdNew;
        private bool hasHandle;

        private void CheckVersion()
        {
            CheckConnected("CheckVersion");
            //var currentVersion = new Version(Regex.Matches(CommandString("YV"), "PULSAR V([\\d.]+)\\w.*")[0].Groups[1].Value);
            //if (minimumFirmwareVerson > currentVersion)
            //    throw new DriverException(string.Format("Please upgrade firmware before using {0} driver (minimum version: {1})", driverID, minimumFirmwareVerson.ToString()));
        }

        public string CommandString(string command)
        {
            return CommandString(command, false, false);
        }

        private bool CommandBool(string command)
        {
            return CommandBool(command, false);
        }

        private long? CommandLong(string command)
        {
            var resultStr = CommandString(command);
            long resultLong;

            if(Int64.TryParse(resultStr, out resultLong))
            {
                return resultLong;
            } else
            {
                return null;
            }
        }

        public void CommandBlind(string command)
        {
            CheckConnected("CommandBlind");
            CommandString(command, false, true);
        }

        /// <summary>
        /// it's a good idea to put all the low level communication with the device here - then all communication calls this function.
        /// </summary>
        /// <param name="command">given command</param>
        /// <param name="raw">if true, skip formatting. If false, format command text</param>
        /// <param name="async">if true, do not wait for response</param>
        /// <returns>response text, in case of empty command or async empty string</returns>
        private string CommandString(string command, bool raw, bool async)
        {
            //init
            if (serialMutex == null)
            {
                //set up security for multi-user usage + add support for localized systems (don't use just "Everyone") 
                MutexAccessRule allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
                MutexSecurity securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);
                serialMutex = new Mutex(false, driverID, out createdNew, securitySettings);
            }

            var response = string.Empty;

            //if command is empty, return immediately
            if (string.IsNullOrWhiteSpace(command))
                return response;

            //format command if not raw
            if (!raw)
                command = Helper.FormatCommand(command);

            try
            {
                try
                {
                    //mutex lock to ensure that only one command is in progress at a time (http://www.ascom-standards.org/Help/Developer/html/caf6b21d-755a-4f1c-891f-ce971a9a2f79.htm)
                    //note, we want to time out here instead of waiting forever
                    hasHandle = serialMutex.WaitOne(commandTimeout, false);
                    if (hasHandle == false)
                        throw new TimeoutException("Timeout waiting for exclusive access");
                    //tl.LogMessage("CommandString mutex", "mutex acquired");
                }
                catch (AbandonedMutexException)
                {
                    //log the fact that the mutex was abandoned in another process, it will still get acquired
                    //tl.LogMessage("CommandString mutex", "mutex acquired via abandon");
                    hasHandle = true;
                }

                //serial communication
                var watch = Stopwatch.StartNew();

                if (async)
                {
                    tl.LogMessage("CommandString async", string.Format("Sending command {0}", command));
                    tls.LogMessage("Request async", command);
                    serial.ClearBuffers();
                    serial.Transmit(command); //async message - do not wait for response
                }
                else
                {
                    tls.LogMessage("Request", command);
                    serial.ClearBuffers();
                    serial.Transmit(command);
                    response = serial.ReceiveTerminated(")"); //wait until termination character

                    if (response.StartsWith("!"))
                        throw new ApplicationException($"Command failed, response: {response}");

                    tls.LogMessage("Response", response);
                    tl.LogMessage("CommandString sync", $"Response for {command} received: {response}");

                    response = response.TrimStart('(').TrimEnd(')');
                }

                //maximum frequency is XYHz so execution must take at least XYms
                //watch.Stop();
                //if (watch.ElapsedMilliseconds < commandTimeoutMs)
                //utilities.WaitForMilliseconds((int)(commandTimeoutMs - watch.ElapsedMilliseconds));
            }
            catch (Exception e)
            {
                tl.LogMessage("CommandString error", $"Command: {command}, Message: {e.Message}, StackTrace: {e.StackTrace}");
                throw;
            }
            finally
            {
                if (hasHandle)
                {
                    serialMutex.ReleaseMutex();
                    //tl.LogMessage("CommandString mutex", "mutex released");
                }
            }

            return response;
        }
    }
}
