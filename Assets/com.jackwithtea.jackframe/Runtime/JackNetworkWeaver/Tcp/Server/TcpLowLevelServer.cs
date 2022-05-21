using System;
using Telepathy;
using JackFrame;

namespace JackFrame.Network {

    public class TcpLowLevelServer {

        Server server;
        int port;

        public event Action<int> OnConnectedHandle;
        public event Action<int, ArraySegment<byte>> OnDataHandle;
        public event Action<int> OnDisconnectedHandle;

        public TcpLowLevelServer(int maxMessageSize) {
            server = new Server(maxMessageSize);
            server.OnConnected = OnConnected;
            server.OnDisconnected = OnDisconnected;
            server.OnData = OnData;
        }

        public void StartListen(int port) {
            this.port = port;
            server.Start(port);
        }

        public void RestartListen() {
            if (server.Active) {
                server.Stop();
            }
            StartListen(port);
        }

        public void Tick(int processLimit = 100) {
            server.Tick(processLimit);
        }

        public void StopListen() {
            server.Stop();
        }

        public void Send(int connId, ArraySegment<byte> data) {
            server.Send(connId, data);
        }

        void OnConnected(int connId) {
            if (OnConnectedHandle != null) {
                OnConnectedHandle.Invoke(connId);
            } else {
                PLog.ForceWarning("未注册: " + nameof(OnConnectedHandle));
            }
        }

        void OnDisconnected(int connId) {
            if (OnDisconnectedHandle != null) {
                OnDisconnectedHandle.Invoke(connId);
            } else {
                PLog.ForceWarning("未注册: " + nameof(OnDisconnectedHandle));
            }
        }

        void OnData(int connId, ArraySegment<byte> data) {
            if (OnDataHandle != null) {
                OnDataHandle.Invoke(connId, data);
            } else {
                PLog.ForceWarning("未注册: " + nameof(OnDataHandle));
            }
            // 发送回文
            // PLog.ForceLog("Server 收到长度: " + data.Count);
            // server.Send(connId, data);
        }

    }

}