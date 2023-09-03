using UnityEngine;

public class test_FileSaver : MonoBehaviour
{
    [SerializeField] int one;
    [SerializeField] string two;
    [SerializeField] bool three;
    [SerializeField] SerializableDictionary<string, bool> four = new();

    [ContextMenu("Save")]
    void Save()
    {
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
        FileSaver.Save("test-file", data);
    }

    [ContextMenu("Load")]
    void Load()
    {
        if (FileSaver.TryLoad<TestData>("test-file", out var data))
        {
            one = data.one;
            two = data.two;
            three = data.three;
            four = data.four;
            Debug.Log("Loaded data successfully");
        }
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