using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(GameManager gameManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.candy";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameManagerData data = new GameManagerData(gameManager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameManagerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/save.candy";

        if(File.Exists(path)) 
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            GameManagerData data = formatter.Deserialize(stream) as GameManagerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
