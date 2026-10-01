# Build firmware and create a `.dsd` package

Run the commands below in PowerShell from the repository root, using the
PlatformIO terminal in VS Code. Both `platformio` and `python` must be available.
If needed, add the installed tools to this terminal's PATH:

```powershell
$env:PATH = "$env:USERPROFILE\.platformio\penv\Scripts;$env:PATH"
```

Set `FW_VERSION` in `src/General_AF3.h` before building. The packaging command
reads this value automatically; version `1.0.6` produces `build/AF3_v1.0.6.dsd`.

## Build both boards

The normal `platformio.ini` enables only Nano Every. For a release, create a
temporary configuration that also builds the classic Nano, without editing the
normal configuration:

```powershell
New-Item -ItemType Directory -Force .pio | Out-Null
@'
[env]
framework = arduino
lib_deps = TMCStepper
    DallasTemperature
    OneWire

[env:nanoatmega328new]
platform = atmelavr@5.3.0
board = nanoatmega328

[env:nano_every]
platform = atmelmegaavr@1.10.0
board = nano_every
'@ | Set-Content -Encoding ascii .pio/af3-package.ini

platformio run --project-dir . --project-conf .pio/af3-package.ini
if ($LASTEXITCODE -ne 0) { throw "Firmware build failed; do not package." }
```

These platform versions successfully built version 1.0.6. The old
`atmelavr@1.12.5` in the commented configuration was unavailable from the package
registry. Keep the bundled libraries in `lib/`, including the modified
TMCStepper 0.6.2 library.

Both environments must report `SUCCESS`. Build them together: switching
PlatformIO configurations can clear previous build output. Package immediately
after this build, before running the normal single-board configuration again.

## Create and verify the package

The [reference AF3 package](https://deepskydad.com/store/software/AF3_v1.0.4.dsd)
is a ZIP archive with exactly these files at its root:

| Archive entry | Build output |
| --- | --- |
| `nano.hex` | `.pio/build/nanoatmega328new/firmware.hex` |
| `nano_every.hex` | `.pio/build/nano_every/firmware.hex` |

No enclosing directory or additional metadata is required. The Control Panel
selects the image for the detected board; both classic Nano bootloaders use
`nano.hex`.

Run this PowerShell block to check the Intel HEX records and embedded version,
create the archive, and verify its contents:

```powershell
@'
from pathlib import Path
from zipfile import ZipFile, ZIP_DEFLATED
import re

version = re.search(r'#define\s+FW_VERSION\s+"([^"]+)"',
                    Path('src/General_AF3.h').read_text()).group(1)
images = {
    'nano.hex': Path('.pio/build/nanoatmega328new/firmware.hex'),
    'nano_every.hex': Path('.pio/build/nano_every/firmware.hex'),
}
for name, path in images.items():
    lines = path.read_text().splitlines()
    data = bytearray()
    for line in lines:
        assert line.startswith(':'), f'{name}: invalid HEX record'
        record = bytes.fromhex(line[1:])
        assert len(record) == record[0] + 5, f'{name}: invalid record length'
        assert sum(record) % 256 == 0, f'{name}: invalid checksum'
        if record[3] == 0:
            data.extend(record[4:-1])
    assert lines[-1] == ':00000001FF', f'{name}: missing EOF'
    assert version.encode() in data, f'{name}: firmware version mismatch'

output = Path('build') / f'AF3_v{version}.dsd'
output.parent.mkdir(exist_ok=True)
with ZipFile(output, 'x', compression=ZIP_DEFLATED, compresslevel=9) as archive:
    for name, path in images.items():
        archive.write(path, name)
with ZipFile(output) as archive:
    assert archive.namelist() == list(images)
    assert archive.testzip() is None
    for name, path in images.items():
        assert archive.read(name) == path.read_bytes()
print(f'Created and verified: {output} ({output.stat().st_size} bytes)')
'@ | python -
if ($LASTEXITCODE -ne 0) { throw "Package creation or verification failed." }

Remove-Item .pio/af3-package.ini
```

The command refuses to overwrite an existing `.dsd`. To replace a release,
move or remove that specific archive first, then rerun packaging.

Build and archive checks do not verify hardware behavior. Test the packaged
firmware on the supported boards and drivers before distributing it.
