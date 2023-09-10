using UnityEngine;

public class DataPersistor : MonoBehaviour
{
    const string METADATA_SAVE_FILE_NAME = "metadata";

    [SerializeField] SaveSlot saveSlot;
    [SerializeField] bool useEncryption;
    [SerializeField] bool disableDataPersistence = false;

    [Space]
    [Space]

    [SerializeField] GameState gameState;

    // MonoSaveable[] saveables;
    FileHandler fileHandler;
    FileHandler metadataHandler;

    void OnValidate()
    {
        if (Application.isPlaying) return;
        if (gameState == null) gameState = State.game;
    }

    void Awake()
    {
        fileHandler = new FileHandler();
        metadataHandler = new FileHandler(METADATA_SAVE_FILE_NAME);
    }

    public void SetSaveSlot(SaveSlot slot)
    {
        this.saveSlot = slot;
    }

    public void ClearSave(SaveSlot slot)
    {
        if (slot == SaveSlot.None) throw new System.Exception("[DataPersistor][NewGame]: valid save slot requried");
        gameState.Clear();
        // this also deletes metadata since the whole save slot directory is deleted
        fileHandler.Delete(slot);
    }

    // public void NewGame(SaveSlot slot)
    // {
    //     if (slot == SaveSlot.None) throw new System.Exception("[DataPersistor][NewGame]: valid save slot requried");
    //     this.saveSlot = slot;
    //     ClearSave(slot);
    //     gameState.OnSave(saveSceneData: true);
    //     SaveGame();
    // }

    public void LoadGame()
    {
        if (saveSlot == SaveSlot.None)
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: No save slot has been set");
            return;
        }

        if (disableDataPersistence)
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: data persistence disabled");
            return;
        }


        if (!fileHandler.TryLoad<GameData>(saveSlot, out var data, useEncryption))
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: could not load data");
            return;
        }

        gameState.SetData(data);
    }

    public void NotifyLoaded()
    {
        if (disableDataPersistence)
            return;

        var saveables = FindAllDataPersistenceObjects();

        for (int i = 0; i < saveables.Length; i++)
        {
            saveables[i].OnGameLoad(gameState);
        }
    }

    public void SaveGame()
    {
        if (saveSlot == SaveSlot.None)
        {
            Debug.LogWarning("[DataPersistor][SaveGame]: No save slot has been set");
            return;
        }

        if (disableDataPersistence)
        {
            Debug.LogWarning("[DataPersistor][SaveGame]: Data persistence disabled");
            return;
        }

        var saveables = FindAllDataPersistenceObjects();

        for (int i = 0; i < saveables.Length; i++)
        {
            saveables[i].OnGameSave(ref gameState);
        }

        gameState.OnSave(saveSceneData: true);
        fileHandler.Save(saveSlot, gameState.GetData(), useEncryption: useEncryption);
        metadataHandler.Save(saveSlot, gameState.GetMetadata(), useEncryption: useEncryption);
    }

    public void LoadMetadata()
    {
        if (saveSlot == SaveSlot.None)
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: No save slot has been set");
            return;
        }

        if (disableDataPersistence)
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: data persistence disabled");
            return;
        }


        if (!metadataHandler.TryLoad<SaveMetadata>(saveSlot, out var data, useEncryption))
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: could not load data");
            return;
        }

        gameState.SetMetadata(data);
    }

    public void SaveMetadata()
    {
        if (saveSlot == SaveSlot.None) return;

        gameState.OnSave(saveSceneData: false);
        metadataHandler.Save(saveSlot, gameState.GetMetadata(), useEncryption: useEncryption);
    }

    public SaveMetadata[] LoadAllSaveFileMetadata()
    {
        SaveMetadata[] metadatas = new SaveMetadata[4];

        metadataHandler.TryLoad(SaveSlot.A, out metadatas[0], useEncryption: useEncryption);
        metadataHandler.TryLoad(SaveSlot.B, out metadatas[1], useEncryption: useEncryption);
        metadataHandler.TryLoad(SaveSlot.C, out metadatas[2], useEncryption: useEncryption);
        metadataHandler.TryLoad(SaveSlot.D, out metadatas[3], useEncryption: useEncryption);

        return metadatas;
    }

    MonoSaveable[] FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoSaveable>(true);
    }

    void OnApplicationQuit()
    {
        SaveMetadata();
    }
}
