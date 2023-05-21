using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MoBuLiveSync
{
    public sealed class UdpReceiver
    {
        private UdpClient _client;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public int Port { get; }
        public bool IsRunning => _task is { IsCanceled: false, IsCompleted: false };

        public ReceivedPacketEventHandler OnReceivedPacket = (_, _, _) => { };
        public ErrorOccuredEventHandler OnUdpStartFailed = _ => { };
        public ErrorOccuredEventHandler OnUdpReceiveFailed = _ => { };
        public ErrorOccuredEventHandler OnUdpReceiveRaiseException = _ => { };

        public delegate void ReceivedPacketEventHandler(byte[] receiveBuffer, int length, EndPoint remoteEp);

        public delegate void ErrorOccuredEventHandler(Exception e);

        public UdpReceiver(int port)
        {
            Port = port;
        }

        ~UdpReceiver()
        {
            StopReceive();
        }

        public void StartReceive()
        {
            StopReceive();
            if (IsRunning) return;

            if (Port == 0) Debug.LogWarning("[UdpReceiver] Port is not set.");

            try
            {
                _client = new UdpClient(Port);
                _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                // _client.Client.Bind(new IPEndPoint(IPAddress.Any, Port));

                _cancellationTokenSource = new CancellationTokenSource();
                _task = Task.Run(() => UdpTaskAsync(_cancellationTokenSource.Token));
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat($"[UdpReceiver] Udp start failed. {e.GetType()} : {e.Message}");
                OnUdpStartFailed?.Invoke(e);
            }
        }

        private async Task UdpTaskAsync(CancellationToken token)
        {
            Debug.Log($"[UdpReceiver] Udp Receive task start. port: {Port}");

            while (!token.IsCancellationRequested && _client != null)
            {
                try
                {
                    IPEndPoint remoteEp = null;
                    var message = _client.Receive(ref remoteEp);

                    var length = message.Length;
                    if (length != 0)
                    {
                        OnReceivedPacket?.Invoke(message, length, remoteEp);
                    }
                }
                catch (Exception e) when (e is SocketException or ObjectDisposedException)
                {
                    Debug.Log($"[UdpReceiver] Udp Receive task failed. {e.Message} : {e.GetType()}");
                    OnUdpReceiveFailed?.Invoke(e);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat($"[UdpReceiver] Udp receive failed. {e.Message} : {e.GetType()}");
                    OnUdpReceiveRaiseException?.Invoke(e);
                }

                await Task.Delay(1, token);
            }
        }

        public void StopReceive()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _task = null;
            }

            if (_client == null) return;
            _client.Close();
            _client = null;
        }
    }
}
