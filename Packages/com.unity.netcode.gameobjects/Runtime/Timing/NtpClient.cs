using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Represents an NTP (Network Time Protocol) client with RTT correction.
/// </summary>
public class NtpClient : IDisposable
{
    private readonly string _server;
    private Socket _socket;
    private bool disposedValue;

    public static DateTime baseTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public NtpClient()
    {
        _server = "pool.ntp.org";
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    public NtpClient(string server)
    {
        _server = server;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    private uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
    }

    /// <summary>
    /// Retrieves the current network time from the NTP server with RTT correction.
    /// </summary>
    public DateTime GetNetworkTime()
    {
        try
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            IPAddress[] addresses = Dns.GetHostEntry(_server).AddressList;
            IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
            if(!_socket.Connected)
            {
                _socket.Connect(ipEndPoint);
            }
            _socket.ReceiveTimeout = 3000;

            DateTime t1 = DateTime.UtcNow;

            _socket.Send(ntpData);
            _socket.Receive(ntpData);

            DateTime t4 = DateTime.UtcNow;


            // 서버 응답 시간 추출 (T3 - Transmit Timestamp)
            const byte serverReplyTime = 40;
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            ulong milliseconds = (intPart * 1000) + (fractPart * 1000 / 0x100000000L);
            DateTime t3 = baseTime.AddMilliseconds((long)milliseconds);

            TimeSpan rtt = t4 - t1;
            DateTime correctedTime = t3.AddMilliseconds(rtt.TotalMilliseconds / 2.0);

            return correctedTime;
        }
        catch (Exception ex)
        {
            InvalidOperationException IVOPEX = new InvalidOperationException("Failed to get network time.", ex);
            Debug.LogException(IVOPEX);
            throw IVOPEX;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        _socket?.Close();
        if (!disposedValue)
        {
            if (disposing)
            {
                _socket?.Dispose();
            }
            _socket = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
