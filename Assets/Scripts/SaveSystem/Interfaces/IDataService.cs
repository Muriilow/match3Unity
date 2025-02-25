using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Persistence
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load(GameData data);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}
