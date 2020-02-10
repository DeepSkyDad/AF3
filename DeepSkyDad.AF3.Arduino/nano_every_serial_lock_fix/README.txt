Hi @technoblogy , thanks for reporting. I investigated the problem and indeed it lies in the USB stack for the D11 (muxto).
I patched the binaries here (ee0a9bd) but I would need your help to validate them.

Could you follow the guide at https://www.arduino.cc/en/Guide/NANOEvery (Firmware for SAMD11D14A chapter), trigger the D11 bootloader, extract the attached zip and execute this command?
./bossac --port=$portname -U true -i -e -w -v MuxTO.bin -R
update_muxto.zip
Thanks a lot