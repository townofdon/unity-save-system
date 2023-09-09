using UnityEngine;

public class test_FileSaver : MonoBehaviour
{
    [SerializeField] SaveSlot saveSlot;
    [SerializeField] bool useEncryption;
    [SerializeField] string saveFile = "";

    [Space]
    [Space]

    [SerializeField] int one;
    [SerializeField] string two;
    [SerializeField] bool three;
    [SerializeField] SerializableDictionary<string, bool> four = new();

    FileHandler fileHandler;

    static test_FileSaver instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [ContextMenu("Save")]
    void Save()
    {
        fileHandler ??= new FileHandler(saveFile);
        four["aaa"] = true;
        four["bbb"] = false;
        four["ccc"] = true;
        four["ddd"] = false;
        var data = new TestData
        {
            one = one,
            two = two,
            three = three,
            four = four,
        };
        fileHandler.Save(saveSlot, data, useEncryption: useEncryption);
    }

    [ContextMenu("Load")]
    void Load()
    {
        fileHandler ??= new FileHandler(saveFile);
        if (fileHandler.TryLoad<TestData>(saveSlot, out var data, useEncryption: useEncryption))
        {
            one = data.one;
            two = data.two;
            three = data.three;
            four = data.four;
            Debug.Log("Loaded data successfully");
        }
    }

    [ContextMenu("Delete")]
    void Delete()
    {
        fileHandler ??= new FileHandler(saveFile);
        fileHandler.Delete(saveSlot);
        one = 0;
        two = "";
        three = false;
        four.Clear();
    }
}

[System.Serializable]
struct TestData
{
    public int one;
    public string two;
    public bool three;
    public SerializableDictionary<string, bool> four;
}