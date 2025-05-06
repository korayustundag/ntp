using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkTime
{
    /// <summary>
    /// Represents a simple NTP (Network Time Protocol) client for retrieving accurate time from an NTP server.
    /// </summary>
    public class NtpClient
    {
        private const string DefaultNtpServer = "pool.ntp.org";
        private const int NtpPort = 123;
        private const byte NtpDataLength = 48;
        private const int Timeout = 3000;

        private readonly string _ntpServer;
        private readonly int _ntpPort;

        /// <summary>
        /// Initializes a new instance of the NtpClient class using the default server and port.
        /// </summary>
        public NtpClient()
        {
            _ntpServer = DefaultNtpServer;
            _ntpPort = NtpPort;
        }

        /// <summary>
        /// Initializes a new instance of the NtpClient class using the specified NTP server.
        /// </summary>
        /// <param name="ntpServer">The hostname or IP address of the NTP server.</param>
        public NtpClient(string ntpServer)
        {
            _ntpServer = ntpServer;
            _ntpPort = NtpPort;
        }

        /// <summary>
        /// Initializes a new instance of the NtpClient class using the specified NTP server and port.
        /// </summary>
        /// <param name="ntpServer">The hostname or IP address of the NTP server.</param>
        /// <param name="ntpPort">The UDP port number of the NTP server (usually 123).</param>
        public NtpClient(string ntpServer, int ntpPort)
        {
            _ntpServer = ntpServer;
            _ntpPort = ntpPort;
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
        }

        /// <summary>
        /// Retrieves the full NTP result with server time, round-trip delay, and offset info.
        /// </summary>
        public async Task<NtpResult> GetFullNetworkTimeAsync()
        {
            byte[] ntpData = new byte[NtpDataLength];
            ntpData[0] = 0b00_011_011; // LI = 0, VN = 3, Mode = 3 (client)

            IPAddress[] addresses = await Dns.GetHostAddressesAsync(_ntpServer);
            IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], _ntpPort);

            using (UdpClient socket = new UdpClient())
            {
                socket.Client.ReceiveTimeout = Timeout;

                var requestTime = DateTime.UtcNow;
                await socket.SendAsync(ntpData, ntpData.Length, ipEndPoint);
                var result = await socket.ReceiveAsync();
                var responseTime = DateTime.UtcNow;

                byte[] response = result.Buffer;

                if (response.Length < NtpDataLength)
                {
                    throw new Exception("NTP response is too short.");
                }

                ulong intPart = BitConverter.ToUInt32(response, 40);
                ulong fractPart = BitConverter.ToUInt32(response, 44);
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);

                ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                DateTime serverTimeUtc = new DateTime(1900, 1, 1).AddMilliseconds((long)milliseconds);

                TimeSpan roundTrip = responseTime - requestTime;
                serverTimeUtc = serverTimeUtc + TimeSpan.FromTicks(roundTrip.Ticks / 2);

                return new NtpResult
                {
                    Server = _ntpServer,
                    UtcTime = serverTimeUtc,
                    RoundTripDelay = roundTrip
                };
            }
        }

        /// <summary>
        /// Returns the current network time as DateTime (Local), with delay correction.
        /// </summary>
        public async Task<DateTime> GetNetworkTimeAsync()
        {
            var result = await GetFullNetworkTimeAsync();
            return result.LocalTime;
        }

        /// <summary>
        /// Returns the current network time as DateTime (Local) synchronously.
        /// </summary>
        public DateTime GetNetworkTime()
        {
            return GetNetworkTimeAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Safely tries to get the current network time. Returns false on failure.
        /// </summary>
        public bool TryGetNetworkTime(out DateTime time)
        {
            try
            {
                time = GetNetworkTime();
                return true;
            }
            catch
            {
                time = DateTime.MinValue;
                return false;
            }
        }

        /// <summary>
        /// Gets the difference between local system time and NTP time.
        /// </summary>
        public async Task<TimeSpan> GetOffsetFromLocalAsync()
        {
            var result = await GetFullNetworkTimeAsync();
            return result.LocalOffset;
        }
    }
}