using CliWrap;
using CliWrap.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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
                using (var cli = new Cli("Esptool/esptool.exe"))
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

                    var output = await cli.ExecuteAsync(cmd, bufferHandler: handler);

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
            } catch(Exception ex)
            {
                _outputTextHandler(ex.Message, true);
                _statusUpdateHandler(FirmwareUpdateStatus.Error);
                return FirmwareUpdateStatus.Error;
            }
        }

        public async Task<FirmwareUpdateStatus> UpoloadFirmwareArduinoNano(string comPort, string path)
        {
            try
            {
                _statusUpdateHandler(FirmwareUpdateStatus.Uploading);

                /*
                 Building esptool.exe
                 mkdir build_tmp
                 pyinstaller --onefile --specpath build_tmp --workpath build_tmp/build --distpath build_tmp/dist C:\Users\bramor\.platformio\packages\tool-esptoolpy\esptool.py

                 */
                using (var cli = new Cli("Avrdude/avrdude.exe"))
                {
                    // Execute
                    var cmd = $"-CAvrdude/avrdude.conf -v -patmega328p -carduino -P{comPort} -b57600 -D -Uflash:w:\"{path}\":i ";
                    var handler = new BufferHandler(
                        stdOutLine => _outputTextHandler(stdOutLine, false),
                        stdErrLine => _outputTextHandler(stdErrLine, true)
                    );

                    var output = await cli.ExecuteAsync(cmd, bufferHandler: handler);

                    // Extract output
                    var code = output.ExitCode;
                    var stdOut = output.StandardOutput;
                    var stdErr = output.StandardError;
                    var startTime = output.StartTime;
                    var exitTime = output.ExitTime;
                    var runTime = output.RunTime;
                    //output.ThrowIfError();

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
    }
}
