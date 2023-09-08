using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistor : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool disableDataPersistence = false;
    [SerializeField] bool initializeDataIfNull = false;
    [SerializeField] bool overrideSelectedProfileId = false;
    // [SerializeField] string testSelectedProfileId = "test";

    [Header("File Storage Config")]
    [SerializeField] string saveFile;
    [SerializeField] bool useEncryption;

    // [Header("Auto Saving Configuration")]
    // [SerializeField] float autoSaveTimeSeconds = 60f;

    GameData gameData;
    ISaveable[] dataPersistenceObjects;
    // FileDataHandler dataHandler;

    string selectedProfileId = "";

    string currentSavePath;

    Coroutine autoSaveCoroutine;

    static DataPersistor instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("[DataPersistor] Found more than one DataPersistor in the scene. Destroying the newest one.");
            gameObject.SetActive(false);
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        if (disableDataPersistence)
        {
            Debug.LogWarning("[DataPersistor] save/load is currently disabled");
        }

        // this.dataHandler = new FileDataHandler(Application.persistentDataPath, saveFile, useEncryption);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        // LoadGame();

        // // start up the auto saving coroutine
        // if (autoSaveCoroutine != null)
        // {
        //     StopCoroutine(autoSaveCoroutine);
        // }
        // autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    // public void ChangeSelectedProfileId(string newProfileId)
    // {
    //     this.selectedProfileId = newProfileId;
    // }

    // public void DeleteProfileData(string profileId)
    // {
    //     // delete the data for this profile id
    //     dataHandler.Delete(profileId);
    //     // initialize the selected profile id
    //     InitializeSelectedProfileId();
    //     // reload the game so that our data matches the newly selected profile id
    //     LoadGame();
    // }

    public void NewGame()
    {
        // this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // // return right away if data persistence is disabled
        // if (disableDataPersistence)
        // {
        //     return;
        // }

        // // load any saved data from a file using the data handler
        // this.gameData = dataHandler.Load(selectedProfileId);

        // // start a new game if the data is null and we're configured to initialize data for debugging purposes
        // if (this.gameData == null && initializeDataIfNull)
        // {
        //     NewGame();
        // }

        // // if no data can be loaded, don't continue
        // if (this.gameData == null)
        // {
        //     Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
        //     return;
        // }

        // // push the loaded data to all other scripts that need it
        // foreach (ISaveable dataPersistenceObj in dataPersistenceObjects)
        // {
        //     dataPersistenceObj.LoadData(gameData);
        // }
    }

    public void SaveGame()
    {
        // // return right away if data persistence is disabled
        // if (disableDataPersistence)
        // {
        //     return;
        // }

        // // if we don't have any data to save, log a warning here
        // if (this.gameData == null)
        // {
        //     Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
        //     return;
        // }

        // // pass the data to other scripts so they can update it
        // foreach (ISaveable dataPersistenceObj in dataPersistenceObjects)
        // {
        //     dataPersistenceObj.SaveData(gameData);
        // }

        // // timestamp the data so we know when it was last saved
        // gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // // save that data to a file using the data handler
        // dataHandler.Save(gameData, selectedProfileId);
    }

    void OnApplicationQuit()
    {
        // SaveGame();
    }

    ISaveable[] FindAllDataPersistenceObjects()
    {
        return null;
        // return FindObjectsOfType<ISaveable>(true);

        // // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        // IEnumerable<ISaveable> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
        //     .OfType<ISaveable>();

        // return new List<ISaveable>(dataPersistenceObjects);
    }

    // public bool HasGameData()
    // {
    //     return gameData != null;
    // }

    // public Dictionary<string, GameData> GetAllProfilesGameData()
    // {
    //     return dataHandler.LoadAllProfiles();
    // }

    // IEnumerator AutoSave()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(autoSaveTimeSeconds);
    //         SaveGame();
    //         Debug.Log("Auto Saved Game");
    //     }
    // }
}