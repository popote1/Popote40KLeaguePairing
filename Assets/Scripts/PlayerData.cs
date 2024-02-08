using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData> , INetworkSerializable {
    
    public ulong ClientId;
    public FixedString64Bytes PlayerName;
    public bool IsReady;
    
    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId
               && PlayerName == other.PlayerName
               && IsReady == other.IsReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref IsReady);
    }

    public static List<FixedString64Bytes> GetListOfPlayerNamesFromPlayerDataNetWorkList(
        NetworkList<PlayerData> playerDatas) {
        List<FixedString64Bytes> playersNames = new List<FixedString64Bytes>();
        foreach (var playerData in playerDatas) {
            playersNames.Add(playerData.PlayerName);
        }
        return playersNames;
    }
}