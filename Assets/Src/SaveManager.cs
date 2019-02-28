using UnityEngine;

using System.IO;
using System.Collections.Generic;

public static class SaveManager
{
    static List<Save> _saves = new List<Save>();

    public static List<Save> saves { get { return _saves; } }
    public static bool hasSaves { get { return _saves.Count > 0; } }

    public static void Initialize()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
        {
            Debug.Log("Save directory doesn't exist, creating...");
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }

        //create debug save
        //for (int i = 0; i < 3; i++)
        //    new Save("debug save #" + i);

        string[] files = Directory.GetFiles(Application.persistentDataPath + "/Saves/", "*.save", SearchOption.TopDirectoryOnly);

        if (files.Length == 0)
        {
            Debug.Log("No saves found! :(");
            return;
        }

        for (int i = 0; i < files.Length; i++)
            _saves.Add(Save.Load(files[i]));
    }

    public static void Delete(Save save)
    {
        _saves.Remove(save);

        if(File.Exists(Application.persistentDataPath + "/Saves/" + save.name + ".save"))
            File.Delete(Application.persistentDataPath + "/Saves/" + save.name + ".save");
    }
}
