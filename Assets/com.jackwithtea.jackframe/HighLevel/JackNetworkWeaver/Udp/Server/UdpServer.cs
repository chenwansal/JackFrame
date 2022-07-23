using System;
using System.Collections.Generic;
using JackBuffer;

namespace JackFrame.Network {

    public class UdpServer {

        UdpLowLevelServer server;

        SortedDictionary<ushort, Action<int, ArraySegment<byte>>> dic;

        public event Action<int> OnConnectedHandle;
        public event Action<int> OnDisconnectedHandle;

        // 1. 构造
        public UdpServer() {
            server = new UdpLowLevelServer();
            dic = new SortedDictionary<ushort, Action<int, ArraySegment<byte>>>();

            server.OnConnectedHandle += OnConnected;
            server.OnDisconnectedHandle += OnDisconnected;
            server.OnRecvDataHandle += OnRecvData;
        }

        public void Tick() {
            server.Tick();
        }

        public void StartListen(int port) {
            server.StartListen(port);
        }

        public void StopListen() {
            server.Stop();
        }

        public void Send<T>(byte serviceId, byte messageId, int connId, T msg) where T : IJackMessage<T> {
            byte[] data = msg.ToBytes();
            byte[] dst = new byte[data.Length + 2];
            int offset = 0;
            dst[offset] = serviceId;
            offset += 1;
            dst[offset] = messageId;
            offset += 1;
            Buffer.BlockCopy(data, 0, dst, offset, data.Length);
            server.Send(connId, dst);
        }

        public void On<T>(byte serviceId, byte messageId, Func<T> generateHandle, Action<int, T> handle) where T : IJackMessage<T> {

            if (generateHandle == null) {
                PLog.ForceError("未注册: " + nameof(generateHandle));
                return;
            }

            ushort key = (ushort)serviceId;
            key |= (ushort)(messageId << 8);
            if (dic.ContainsKey(key)) {
                PLog.ForceWarning($"已注册 serviceId:{serviceId}, messageId:{messageId}");
                return;
            } else {
                dic.Add(key, (connId, byteData) => {
                    T msg = generateHandle.Invoke();
                    int offset = 2;
                    msg.FromBytes(byteData.Array, ref offset);
                    handle.Invoke(connId, msg);
                });
            }

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

        void OnRecvData(int connId, byte[] data) {
            if (data.Length < 2) {
                PLog.ForceError($"消息长度过短: {data.Length}");
                return;
            }

            byte serviceId = data[0];
            byte messageId = data[1];
            ushort key = (ushort)serviceId;
            key |= (ushort)(messageId << 8);
            dic.TryGetValue(key, out var handle);
            if (handle != null) {
                handle.Invoke(connId, data);
            } else {
                PLog.ForceWarning($"未注册 serviceId:{serviceId}, messageId:{messageId}");
            }

        }

    }

}