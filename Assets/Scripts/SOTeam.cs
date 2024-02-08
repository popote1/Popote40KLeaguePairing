using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeam", menuName = "SO/NewTeam")]
public class SOTeam : ScriptableObject {
    public string TeamName;
    public List<string> Factions;

    public void LoadDataFromSerializeData(SerializeSOTeam data) {
        TeamName = data.TeamName.ToString();
        Factions = SerializeSOTeam.GetStringListFromFixedStringArray(data.FactionIndex);
    }

    public string GetDescription() {
        string text = "";

        for (int i = 0; i < Factions.Count; i++) {
            if (i != 0) text += "\n";
            text +="- "+ Factions[i].ToUpper();
        }
        return text;
    }
}

public static class Exstention {
    public static SerializeSOTeam CreateSerializationData(this SOTeam team) {
        SerializeSOTeam data = new SerializeSOTeam();
        data.TeamName = team.TeamName;
        data.FactionIndex = SerializeSOTeam.GetSerializeArrayFromStringList(team.Factions);
        return data;
    }
}

public class SerializeSOTeam : INetworkSerializable{
    public FixedString64Bytes TeamName;
    public FixedString64Bytes[] FactionIndex;

    
    
    public static SerializeSOTeam CreateSerializationData(SOTeam so) {
        SerializeSOTeam data = new SerializeSOTeam();
        data.TeamName = so.TeamName;
        data.FactionIndex = GetSerializeArrayFromStringList(so.Factions);
        return data;
    }

    public static FixedString64Bytes[] GetSerializeArrayFromStringList(List<String> list) {
        int lenght = list.Count;
        FixedString64Bytes[] array = new FixedString64Bytes[lenght];
        for (int i = 0; i < lenght; i++) array[i] = list[i];
        return array;
    }

    public static List<String> GetStringListFromFixedStringArray(FixedString64Bytes[] array) {
        List<string> list = new List<string>();
        foreach (var fixedString in array) list.Add(fixedString.ToString());
        return list;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref TeamName);

        FixedString64Bytes[] array;
        int length = 0;

        if (!serializer.IsReader) {
            array = FactionIndex;
            length = FactionIndex.Length;
        }
        else { 
            array = new FixedString64Bytes[length];
        }
        
        serializer.SerializeValue(ref length);
        
        if (serializer.IsReader) array = new FixedString64Bytes[length];
        for (int i = 0; i < length; i++) serializer.SerializeValue(ref array[i]);
        if (serializer.IsReader) FactionIndex = array;
        
       


    }
}