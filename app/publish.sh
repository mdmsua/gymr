#!/usr/bin/env sh

rm -rf ./publish

dotnet publish -c Release -o ./publish

cd ./publish

zip -r publish.zip .

az webapp deployment source config-zip -g gymr -n gymr --src publish.zip

cd -