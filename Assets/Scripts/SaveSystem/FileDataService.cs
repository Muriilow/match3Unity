using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Unity.VisualScripting;

namespace Systems.Persistence
{
    public class FileDataService : IDataService
    {
        ISerializer serializer;
        string dataPath;
        string fileExtesion;

        public FileDataService(ISerializer serializer)
        {
            this.dataPath = Application.persistentDataPath;
            this.fileExtesion = "json";
            this.serializer = serializer;
            Debug.Log($"FileDataService initialized with path: {dataPath}");
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtesion));
        }
        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            if(!overwrite && File.Exists(fileLocation)) 
            {
                throw new IOException($"The file '{data.Name}.{fileExtesion}' already exists and cannot be overwritten.");
            }

            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);

            if(!File.Exists(fileLocation))
            {
                return null;
                //throw new ArgumentException($"No persisted GameData with name '{name}'");
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }
        
        //PlaceHolders 
        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        { 
            throw new NotImplementedException();
        }

        public IEnumerable<string> ListSaves()
        {
            throw new NotImplementedException();
        }
    }
}
