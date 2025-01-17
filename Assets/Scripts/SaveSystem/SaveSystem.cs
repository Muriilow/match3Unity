using UnityEngine;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Persistence
{
    public class SaveSystem : PersistentSingleton<SaveSystem>
    {
        [SerializeField] public GameData gameData;

        IDataService dataService;

        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; 
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LoadGame();
            if (scene.name != "NormalGame" && scene.name != "FastGame")
                return;
                
            Bind<GameManagerFast, FastData>(gameData.fastData);
            Bind<GameManagerNormal, NormalData>(gameData.normalData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity == null)
                return;
            
            if (data == null)
            {
                data = new TData { Id = entity.Id };
            }
            entity.Bind(data);
           Debug.Log(data.Id); 
        }
        public void SaveGame(bool overwrite = true)
        {
            dataService.Save(gameData, overwrite);
        }

        public void LoadGame()
        {
            gameData = dataService.Load(gameData);
            
            //TODO: If the data is null, handle it. 
        }

        public void NewGame()
        {
            gameData = new GameData {
                Name = "Candy",
            };
        }
        public void DeleteGame(string gameName)
        {
            dataService.Delete(gameName);
        }
    }
    [Serializable]
    public class GameData
    {
        public string Name = "Candy";
        public FastData fastData;
        public NormalData normalData;
    }
    public interface ISaveable
    {
        string Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        string Id { get; set; }
        void Bind(TData data);
    }
}
