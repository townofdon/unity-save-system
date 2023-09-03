using UnityEngine;

public class test_FileSaver : MonoBehaviour
{

    [SerializeField] string saveFile = "test-save";
    [SerializeField] bool useEncryption;

    [Space]
    [Space]

    [SerializeField] int one;
    [SerializeField] string two;
    [SerializeField] bool three;
    [SerializeField] SerializableDictionary<string, bool> four = new();

    [ContextMenu("Save")]
    void Save()
    {
        string path = FileRW.GetFullPath(saveFile);
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
        FileRW.Save(path, data, useEncryption: useEncryption);
    }

    [ContextMenu("Load")]
    void Load()
    {
        string path = FileRW.GetFullPath(saveFile);
        if (FileRW.TryLoad<TestData>(path, out var data, useEncryption: useEncryption))
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
        string path = FileRW.GetFullPath(saveFile);
        FileRW.Delete(path);
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