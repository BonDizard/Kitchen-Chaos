/*
 * Author: Bharath Kumar S
 * Date: 30-09-2024
 * Description: Player data
 */

using System;
using System.Drawing;
using System.Runtime.Serialization;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
    public ulong clientId;
    public int colorId;

    public bool Equals(PlayerData other) {
        return clientId == other.clientId && colorId == other.colorId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
    }
}