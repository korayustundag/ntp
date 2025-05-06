# NTP - Network Time Protocol
[![NuGet Version](https://img.shields.io/nuget/v/NetworkTime.svg)](https://www.nuget.org/packages/NetworkTime/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
A fully-featured and lightweight NTP (Network Time Protocol) client for .NET. 

Supports both synchronous and asynchronous APIs, as well as round-trip delay and local time offset correction.

---
## 🌐 Supported Frameworks
- .NET Standard 2.0
- .NET Framework 4.7.2
- .NET Framework 4.8
- .NET 6
- .NET 8

---
## 📦 Install via NuGet
```bash
dotnet add package NetworkTime
```
Or via the NuGet Package Manager:
```powershell
Install-Package NetworkTime
```

---
## 🚀 Features
-   Lightweight and fast UDP-based NTP client
-   Round-trip delay correction
-   Local clock offset calculation
-   Async/await support
-   Easily extendable for custom NTP servers

---
## 🧪 Example Usage
### Simple (Async)
```csharp
var client = new NtpClient();
DateTime localTime = await client.GetNetworkTimeAsync();
Console.WriteLine("NTP Time (Local): " + localTime);
```
### Safe Try Pattern
```csharp
var client = new NtpClient("time.windows.com");
if (client.TryGetNetworkTime(out DateTime ntpTime))
    Console.WriteLine("Time from NTP: " + ntpTime);
else
    Console.WriteLine("Failed to get NTP time.");
```
### Full Result (with delay and offset)
```csharp
var result = await client.GetFullNetworkTimeAsync();
Console.WriteLine($"Server: {result.Server}");
Console.WriteLine($"UTC Time: {result.UtcTime}");
Console.WriteLine($"Round-Trip Delay: {result.RoundTripDelay.TotalMilliseconds} ms");
Console.WriteLine($"Local Offset: {result.LocalOffset.TotalMilliseconds} ms");
```

---
## Class Overview
### `NtpClient`
| Method                                      | Description                                 |
| ------------------------------------------- | ------------------------------------------- |
| `Task<DateTime> GetNetworkTimeAsync()`      | Gets local time from NTP (async)            |
| `DateTime GetNetworkTime()`                 | Gets local time from NTP (sync)             |
| `bool TryGetNetworkTime(out DateTime)`      | Tries to get local NTP time safely          |
| `Task<TimeSpan> GetOffsetFromLocalAsync()`  | Returns offset between system and NTP clock |
| `Task<NtpResult> GetFullNetworkTimeAsync()` | Returns detailed result with delay info     |

### `NtpResult`
Contains:

* UTC time
* Local time (converted)
* Round-trip delay
* Clock offset
* NTP server name

---
## 🧾 License
This project is licensed under the MIT License.

See the [LICENSE](LICENSE) file for details.
