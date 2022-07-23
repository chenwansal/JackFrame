using System;
using System.Collections.Generic;
using JackBuffer;

namespace JackFrame.Network {

    public class UdpClient {

        UdpLowLevelClient client;

        SortedDictionary<ushort, Action<ArraySegment<byte>>> dic;

        public event Action OnConnectedHandle;
        public event Action OnDisconnectedHandle;

        public UdpClient() {
            client = new UdpLowLevelClient();
            dic = new SortedDictionary<ushort, Action<ArraySegment<byte>>>();

            client.OnConnectedHandle += OnConnected;
            client.OnRecvDataHandle += OnRecvData;
            client.OnDisconnectedHandle += OnDisconnected;
        }

        public void Tick() {
            client.Tick();
        }

        public void Connect(string host, int port) {
            client.Connect(host, port, "");
        }

        public void Disconnect() {
            client.Stop();
        }

        public void Send<T>(byte serviceId, byte messageId, T msg) where T : IJackMessage<T> {
            byte[] buffer = msg.ToBytes();
            byte[] dst = new byte[buffer.Length + 2];
            int offset = 0;
            dst[offset] = serviceId;
            offset += 1;
            dst[offset] = messageId;
            offset += 1;
            Buffer.BlockCopy(buffer, 0, dst, offset, buffer.Length);
            client.Send(dst);
        }

        public void On<T>(byte serviceId, byte messageId, Func<T> generateHandle, Action<T> handle) where T : IJackMessage<T> {
            
            if (generateHandle == null) {
                PLog.ForceWarning($"未注册: " + nameof(generateHandle));
                return;
            }

            ushort key = (ushort)serviceId;
            key |= (ushort)(messageId << 8);

            if (dic.ContainsKey(key)) {
                PLog.ForceWarning($"已注册 serviceId:{serviceId}, messageId:{messageId}");
                return;
            } else {
                dic.Add(key, (byteData) => {
                    T msg = generateHandle.Invoke();
                    int offset = 2;
                    msg.FromBytes(byteData.Array, ref offset);
                    handle.Invoke(msg);
                });
            }
        }

        void OnConnected() {
            if (OnConnectedHandle != null) {
                OnConnectedHandle.Invoke();
            } else {
                PLog.ForceWarning("未注册: " + nameof(OnConnectedHandle));
            }
        }

        void OnDisconnected() {
            if (OnDisconnectedHandle != null) {
                OnDisconnectedHandle.Invoke();
            } else {
                PLog.ForceWarning("未注册: " + nameof(OnDisconnectedHandle));
            }
        }

        void OnRecvData(byte[] data) {
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
                handle.Invoke(data);
            } else {
                PLog.ForceWarning($"未注册 serviceId:{serviceId}, messageId:{messageId}");
            }
        }

    }

}