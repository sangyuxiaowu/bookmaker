#!/bin/bash

dotnet publish -c Release -f net7.0 -r win-x64 -p:PublishSingleFile=true