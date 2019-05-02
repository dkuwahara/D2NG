# D2NG
[![CircleCI](https://circleci.com/gh/dkuwahara/D2NG.svg?style=svg&circle-token=911eb9e33fedad65ef3943148fca9b29309cf67f)](https://circleci.com/gh/dkuwahara/D2NG)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/0b90f6cdc4b0445296de25748e066738)](https://www.codacy.com?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=dkuwahara/D2NG&amp;utm_campaign=Badge_Grade)

## Building the project
This project builds with .NET Core 2.1 and can be built by running `dotnet build` on the command line from the root of the Solution.

## Configuring
ConsoleBot expects a `config.yml` file in the same directory as the `.exe`. The `config.yml` should look as follows:
```
classicKey: string
expansionKey: string
realm: string
```
