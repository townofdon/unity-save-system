using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void NewGame(SaveSlot saveSlot)
    {
        if (saveSlot == SaveSlot.None) throw new System.Exception("[DataPersistor][NewGame]: valid save slot requried");
        this.saveSlot = saveSlot;
        gameState.Clear();
        fileHandler.Delete(saveSlot);
    }

    public void LoadGame()
    {
        if (saveSlot == SaveSlot.None)
        {
            Debug.LogWarning("[DataPersistor][LoadGame]: No save slot has been set");
            return;
        }

        if (disableDataPersistence)
            return;

        if (!fileHandler.TryLoad<GameData>(saveSlot, out var data, useEncryption))
        {
            return;
        }

        gameState.SetData(data);

        var saveables = FindAllDataPersistenceObjects();

        Debug.Log($"[DataPersistor] found {saveables.Length} saveables");

        if (saveables == null)
            return;

        for (int i = 0; i < saveables.Length; i++)
        {
            Debug.Log($"[DataPersistor] loading {saveables[i].name}");
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
            return;

        var saveables = FindAllDataPersistenceObjects();

        if (saveables == null)
            return;

        for (int i = 0; i < saveables.Length; i++)
        {
            saveables[i].OnGameSave(ref gameState);
        }

        gameState.OnSave(sceneIndex: SceneManager.GetActiveScene().buildIndex);
        fileHandler.Save(saveSlot, gameState.GetData(), useEncryption: useEncryption);
        metadataHandler.Save(saveSlot, gameState.GetMetadata(), useEncryption: useEncryption);
    }

    void OnApplicationQuit()
    {
        gameState.OnSave(sceneIndex: -1);
        metadataHandler.Save(saveSlot, gameState.GetMetadata(), useEncryption: useEncryption);
    }

    MonoSaveable[] FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoSaveable>(true);
    }
}