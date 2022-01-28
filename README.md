EventListener is a .NET console application compatible with Linux distributions that is capable of subscribing to a SNS topic
and displaying messages send to the given topic. It works with localstack and goaws.

We're planning to dockerize this app for easier use.

## Usage

```
dotnet restore
dotnet run <command> <Optional: options>
```

commands:
  - ListTopics     Lists all topics

  - Subscribe      Subscribes to the give topic to receive events,
                   Option (required) [--topic-arn] topic arn

  - Listen         Listens to the events, [ctrl] + [c] to close the program
                   Option (optional) [--pretty] displays prettified json event

  - Unsubscribe    Unsbucribes topic
                   Option (required) [--topic-arn] => topic arn

  - Help           Displays help


options:
  --topic-arn     => arn of the SNS topic, required for Subscribe and Unsubscribe commands
  --endpoint-url  => AWS endpoint url
  --pretty

## Envirionment

.NET SDK (reflecting any global.json):
 - Version:   6.0.101
 - Commit:    ef49f6213a

Runtime Environment:
 - OS Name:     debian
 - OS Version:  11
 - OS Platform: Linux
 - RID:         debian.11-x64
 - Base Path:   /usr/share/dotnet/sdk/6.0.101/

Host (useful for support):
  - Version: 6.0.1
  - Commit:  3a25a7f1cc

.NET SDKs installed:
  - 3.1.416 [/usr/share/dotnet/sdk]
  - 6.0.101 [/usr/share/dotnet/sdk]

.NET runtimes installed:
  - Microsoft.AspNetCore.App 3.1.22 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
  - Microsoft.AspNetCore.App 6.0.1 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
  - Microsoft.NETCore.App 3.1.22 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
  - Microsoft.NETCore.App 6.0.1 [/usr/share/dotnet/shared/Microsoft.NETCore.App]

To install additional .NET runtimes or SDKs:
  https://aka.ms/dotnet-download


## Development
- [Installation](https://docs.microsoft.com/en-us/dotnet/core/install/linux)