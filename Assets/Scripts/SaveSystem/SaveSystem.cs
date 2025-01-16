using UnityEngine;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Persistence
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public FastData fastData;
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
            //if (scene.name == "Menu")
                //return;
            Bind<GameManagerFast, FastData>(gameData.fastData);
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
            
        }
        public void SaveGame()
        {
            dataService.Save(gameData);
        }

        public void LoadGame(string gameName)
        {
            if (gameName == null || dataService.Load(gameName) == null)
            {
                gameData = NewGame();
                gameData = dataService.Load(gameData.Name);
                return;
            }
            
            gameData = dataService.Load(gameName);

            //TODO: If the data is null, handle it. 
        }

        public GameData NewGame()
        {
            gameData = new GameData {
                Name = "Game",
            };
            
            return gameData;
        }
        public void DeleteGame(string gameName)
        {
            dataService.Delete(gameName);
        }

        public void ReloadGame()
        {
            LoadGame(gameData.Name);
        }
    }
}
