#!/bin/bash

LOCAL_SOURCE="${NUGET_PACKAGES_LOCAL:-$HOME/.nuget/local}"

if [[ ! -d $LOCAL_SOURCE ]]; then
    echo "Creating local nuget source: $LOCAL_SOURCE"
    mkdir -p $LOCAL_SOURCE
fi

echo "Publishing nuget packages to: $LOCAL_SOURCE"
dotnet pack -o $LOCAL_SOURCE
