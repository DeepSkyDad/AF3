using CliWrap;
using CliWrap.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeepSkyDad.AF3.ControlPanel
{
    public class FirmwareUpdateService
    {
        Action<FirmwareUpdateStatus> _statusUpdateHandler;
        Action<string, bool> _outputTextHandler;

        public enum FirmwareUpdateStatus
        {
            Uploading,
            Successful,
            Error
        }

        public FirmwareUpdateService(Action<FirmwareUpdateStatus> statusUpdateHandler, Action<string, bool> outputTextHandler)
        {
            _statusUpdateHandler = statusUpdateHandler;
            _outputTextHandler = outputTextHandler;
        }

        public async Task<FirmwareUpdateStatus> UpoloadFirmwareEsp32(string comPort, string path)
        {
            try
            {
                _statusUpdateHandler(FirmwareUpdateStatus.Uploading);

                if (Directory.Exists("tmp"))
                    Directory.Delete("tmp", true);

                Directory.CreateDirectory("tmp");

                ZipFile.ExtractToDirectory(path, "tmp");

                /*
                 Building esptool.exe
                 mkdir build_tmp
                 pyinstaller --onefile --specpath build_tmp --workpath build_tmp/build --distpath build_tmp/dist C:\Users\bramor\.platformio\packages\tool-esptoolpy\esptool.py

                 */
                using (Program.CurrentCli = new Cli("Esptool/esptool.exe"))
                {
                    // Execute

                    /*
                     # Name,   Type, SubType, Offset,  Size, Flags
                        nvs,      data, nvs,     0x9000,  0x5000,
                        otadata,  data, ota,     0xe000,  0x2000,
                        app0,     app,  ota_0,   0x10000, 0x140000,
                        app1,     app,  ota_1,   0x150000,0x140000,
                        eeprom,   data, 0x99,    0x290000,0x1000,
                        spiffs,   data, spiffs,   0x291000,0x16F000
                     */
                    var cmd = $"--chip esp32 --port \"{comPort}\" --baud 921600 --before default_reset --after hard_reset write_flash -z --flash_mode dio --flash_freq 40m --flash_size detect 0x1000 .\\tmp\\bootloader_dio_40m.bin 0x8000 .\\tmp\\partitions.bin 0xe000 .\\tmp\\boot_app0.bin 0x10000 .\\tmp\\firmware.bin 0x291000 .\\tmp\\spiffs.bin";
                    var handler = new BufferHandler(
                        stdOutLine => _outputTextHandler(stdOutLine, false),
                        stdErrLine => _outputTextHandler(stdErrLine, true)
                    );

                    var output = await Program.CurrentCli.ExecuteAsync(cmd, bufferHandler: handler);

                    // Extract output
                    var code = output.ExitCode;
                    var stdOut = output.StandardOutput;
                    var stdErr = output.StandardError;
                    var startTime = output.StartTime;
                    var exitTime = output.ExitTime;
                    var runTime = output.RunTime;
                    //output.ThrowIfError();

                    Directory.Delete("tmp", true);

                    if (output.ExitCode != 0)
                    {
                        _statusUpdateHandler(FirmwareUpdateStatus.Error);
                        return FirmwareUpdateStatus.Error;
                    }
                    else
                    {
                        _statusUpdateHandler(FirmwareUpdateStatus.Successful);
                        return FirmwareUpdateStatus.Successful;
                    }
                }
            }
            catch (Exception ex)
            {
                _outputTextHandler(ex.Message, true);
                _statusUpdateHandler(FirmwareUpdateStatus.Error);
                return FirmwareUpdateStatus.Error;
            }
        }

        public async Task<FirmwareUpdateStatus> UploadFirmwareArduinoNano(string comPort, string path)
        {
            try
            {
                _statusUpdateHandler(FirmwareUpdateStatus.Uploading);

                #region Arduino Nano - old bootloader

                _outputTextHandler("", false);
                _outputTextHandler("/***************************************************/", false);
                _outputTextHandler("/******** Arduino Nano - old bootloader *********/", false);
                _outputTextHandler("/***************************************************/", false);
                _outputTextHandler("", false);

                using (Program.CurrentCli = new Cli("Avrdude/avrdude.exe"))
                {
                    var cmd = $"-CAvrdude/avrdude.conf -v -patmega328p -carduino -P{comPort} -b57600 -D -Uflash:w:\"{path}\":i ";
                    var handler = new BufferHandler(
                        stdOutLine => {
                            if (stdOutLine.Contains("not responding"))
                            {
                                Program.CurrentCli.CancelAll();
                            }
                            else
                            {
                                _outputTextHandler(stdOutLine, false);
                            }
                        },
                        stdErrLine => {
                            if (stdErrLine.Contains("not responding"))
                            {
                                Program.CurrentCli.CancelAll();
                            }
                            else
                            {
                                _outputTextHandler(stdErrLine, true);
                            }
                        }
                    );

                    try
                    {
                        var output = await Program.CurrentCli.ExecuteAsync(cmd, bufferHandler: handler);
                        if (output.ExitCode == 0)
                        {
                            _statusUpdateHandler(FirmwareUpdateStatus.Successful);
                            return FirmwareUpdateStatus.Successful;
                        }
                    } catch(Exception e)
                    {
                        _outputTextHandler(e.Message, true);
                    }
                }

                Program.CurrentCli = null;

                _outputTextHandler("Arduino Nano - old bootloader failed", true);

                #endregion

                #region Arduino Nano - new bootloader

                _outputTextHandler("", false);
                _outputTextHandler("/***************************************************/", false);
                _outputTextHandler("/******** Arduino Nano - new bootloader ********/", false);
                _outputTextHandler("/***************************************************/", false);
                _outputTextHandler("", false);

                using (Program.CurrentCli = new Cli("Avrdude/avrdude.exe"))
                {
                    var cmd = $"-CAvrdude/avrdude.conf -v -patmega328p -carduino -P{comPort} -b115200 -D -Uflash:w:\"{path}\":i ";
                    var handler = new BufferHandler(
                        stdOutLine => {
                            if (stdOutLine.Contains("not responding"))
                            {
                                Program.CurrentCli.CancelAll();
                            }
                            else
                            {
                                _outputTextHandler(stdOutLine, false);
                            }
                        },
                        stdErrLine => {
                            if (stdErrLine.Contains("not responding"))
                            {
                                Program.CurrentCli.CancelAll();
                            }
                            else
                            {
                                _outputTextHandler(stdErrLine, true);
                            }
                        }
                    );

                    try
                    {
                        var output = await Program.CurrentCli.ExecuteAsync(cmd, bufferHandler: handler);
                        if (output.ExitCode == 0)
                        {
                            _statusUpdateHandler(FirmwareUpdateStatus.Successful);
                            return FirmwareUpdateStatus.Successful;
                        }
                    }
                    catch (Exception e)
                    {
                        _outputTextHandler(e.Message, true);
                    }
                }

                Program.CurrentCli = null;

                _outputTextHandler("Arduino Nano - new bootloader failed", true);

                #endregion

                #region Arduino Nano Every

                _outputTextHandler("", false);
                _outputTextHandler("/***************************************************/", false);
                _outputTextHandler("/************* Arduino Nano Every ***************/", false);
                _outputTextHandler("/***************************************************/", false);
                _outputTextHandler("", false);

                _outputTextHandler("Switch to 1200 baud rate, open/close to force reset", false);

                using (Program.CurrentCli = new Cli("Avrdude/putty.exe"))
                {
                    var cmd = $"-serial {comPort} -sercfg 1200";

                    Program.CurrentCli.ExecuteAsync(cmd);

                    Thread.Sleep(500);

                    Program.CurrentCli.CancelAll();
                }

                Program.CurrentCli = null;

                using (Program.CurrentCli = new Cli("Avrdude/avrdude.exe"))
                {
                    // Execute
                    var cmd = $"-CAvrdude/avrdude.conf -v -p atmega4809 -c jtag2updi -D -V -b 115200 -e -P{comPort} -Uflash:w:\"{path}\":i -Ufuse2:w:0x01:m -Ufuse5:w:0xC9:m -Ufuse8:w:0x00:m";
                    var handler = new BufferHandler(
                        stdOutLine => {
                            if (stdOutLine.Contains("status -1"))
                            {
                                Program.CurrentCli.CancelAll();
                            }
                            else
                            {
                                _outputTextHandler(stdOutLine, false);
                            }
                        },
                        stdErrLine => {

                            if (stdErrLine.Contains("status -1"))
                            {
                                Program.CurrentCli.CancelAll();
                            }
                            else
                            {
                                _outputTextHandler(stdErrLine, true);
                            }
                        }
                    );

                    try
                    {
                        var output = await Program.CurrentCli.ExecuteAsync(cmd, bufferHandler: handler);
                        _statusUpdateHandler(FirmwareUpdateStatus.Successful);
                        return FirmwareUpdateStatus.Successful;
                    }
                    catch (Exception e)
                    {
                        _outputTextHandler(e.Message, true);
                    }
                }

                Program.CurrentCli = null;

                _outputTextHandler("Arduino Nano Every falied", true);

                #endregion
            }
            catch (Exception ex)
            {
                _outputTextHandler(ex.Message, true);
                _statusUpdateHandler(FirmwareUpdateStatus.Error);
                return FirmwareUpdateStatus.Error;
            }

            _statusUpdateHandler(FirmwareUpdateStatus.Error);
            return FirmwareUpdateStatus.Error;
        }
    }
}
;