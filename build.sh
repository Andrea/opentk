#!/bin/bash

mono --runtime=v4.0 tools/NuGet/nuget.exe install FAKE -Version 3.30.2
mono --runtime=v4.0 packages/FAKE.3.30.2/tools/FAKE.exe build.fsx $@