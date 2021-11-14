# pbrain-Whatateam

An artificial intelligence for [Piskvork](http://gomocup.org/piskvork/) Gomoku software. *pbrain-Whatateam* uses a homemade algorithm.

## Requirements

 - [Windows](https://www.microsoft.com/en-us/windows/)
 - [.NET Framework](https://www.microsoft.com/net/download/windows)
 - [Piskvork](http://gomocup.org/piskvork/)

## How to re-compile the AI?

A copy of pbrain-Whatateam's last version is located at the root of the repository: `pbrain-Whatateam.exe`.

If you want to compile `pbrain-Whatateam.exe` by yourself, here's how.

### IDE

You can use you favorite IDE to load the .NET solution and build `pbrain-Whatateam.exe`.

### Command-line

 - Open Windows' Command Prompt (`cmd`).
 - You can find .NET's `MSBuild.exe` under one of the subfolders of `C:\Windows\Microsoft.Net\Framework`.
 - Navigate to the root of the repository & execute your `MSBuild.exe`, passing `pbrain-Whatateam.sln` as an argument.
 - Here is an example with the .NET Frameword 4.7.1:
```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe pbrain-Whatateam.sln
```
