#!/bin/bash

dotnet publish -c Release -f net8.0 -r win-x64 -o publish/win-x64 -p:PublishSingleFile=true
dotnet publish -c Release -f net8.0 -r osx-x64 -o publish/osx-x64 -p:PublishSingleFile=true
dotnet publish -c Release -f net8.0 -r linux-x64 -o publish/linux-x64 -p:PublishSingleFile=true