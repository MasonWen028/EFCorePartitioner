using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Utils
{
    public static class SnowflakeIdGenerator
    {
        private static long _lastTimestamp = -1L;
        private static long _sequence = 0L;
        private const int MachineIdBits = 5; // The number of digits occupied by the machine ID
        private const int SequenceBits = 12; // The number of digits occupied by the serial number

        private static long _machineId = -1; // Cache machine ID
        private static long _maxSequence = (1 << SequenceBits) - 1; // Maximum value of serial number

        private static readonly object LockObject = new object(); // Lock object, used for synchronization

        /// <summary>
        /// Generates a new ID based on the Snowflake algorithm.
        /// </summary>
        /// <returns>The generated unique ID.</returns>
        /// <exception cref="Exception">Thrown when the system clock goes backwards.</exception>
        public static long GenerateId()
        {
            lock (LockObject)
            {
                var timestamp = GetCurrentTimestamp();
                if (timestamp < _lastTimestamp)
                {
                    throw new Exception("Clock moved backwards. Refusing to generate id");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & _maxSequence;
                    if (_sequence == 0) // Sequence overflow, wait for next millisecond
                    {
                        timestamp = WaitForNextMillisecond(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0; // Reset sequence for new millisecond
                }

                _lastTimestamp = timestamp;

                if (_machineId == -1)
                {
                    _machineId = GetMachineId(); // Lazy load machine ID
                }

                return ((timestamp << (MachineIdBits + SequenceBits))
                        | (_machineId << SequenceBits)
                        | _sequence);
            }
        }

        /// <summary>
        /// Reverse the generated time from the Snowflake ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DateTime GetTimestampFromId(long id)
        {
            var timestamp = (id >> (MachineIdBits + SequenceBits)); // Shift right to drop sequence and machine ID bits
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
        }


        /// <summary>
        /// Gets the current Unix timestamp in milliseconds.
        /// </summary>
        /// <returns>The current Unix timestamp.</returns>
        private static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Waits until the next millisecond.
        /// </summary>
        /// <param name="lastTimestamp">The timestamp of the last generated ID.</param>
        /// <returns>The next millisecond's timestamp.</returns>
        private static long WaitForNextMillisecond(long lastTimestamp)
        {
            var timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }

        /// <summary>
        /// Retrieves a unique machine ID using the local IP address.
        /// </summary>
        /// <returns>The machine ID derived from the IP address.</returns>
        public static long GetMachineId()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        byte[] bytes = ip.GetAddressBytes();
                        return ((long)bytes[2] << 8) + bytes[3]; // Use the last two bytes of the IP address
                    }
                }
            }
            catch
            {
                return 0;
            }
            return 0;
        }
    }

}
