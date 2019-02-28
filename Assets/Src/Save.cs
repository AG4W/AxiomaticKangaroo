using UnityEngine;

using System.Runtime.Serialization;
using System.IO;
using System;

[DataContract]
public class Save
{
    [DataMember]string _name;

    [DataMember]string _build;
    [DataMember]string _datestamp;

    [DataMember]SaveData _data;

    public string name { get { return _name; } }

    public string build { get { return _build; } }
    public string datestamp { get { return _datestamp; } }

    public SaveData data { get { return _data; } }

    public Save(string name, SaveData data)
    {
        _name = name;

        _build = Application.version + "(unity " + Application.unityVersion + ")";
        _datestamp = DateTime.Now.ToString();

        _data = data;
    }

    public void Write()
    {
        DataContractSerializer serializer = new DataContractSerializer(typeof(Save));

        using (FileStream stream = new FileStream(Application.persistentDataPath + "/Saves/" + _name + ".save", FileMode.OpenOrCreate))
            serializer.WriteObject(stream, this);
    }
    public static Save Load(string path)
    {
        Save save;
        DataContractSerializer serializer = new DataContractSerializer(typeof(Save));

        using (FileStream stream = new FileStream(path, FileMode.Open))
            save = serializer.ReadObject(stream) as Save;

        return save;
    }
}
