
using System.IO;
using System.Text;
using UnityEngine;

public static class FileSaver
{
    public static bool Save(string saveFilePath, object obj, bool append = false)
    {
        TextWriter writer = null;
        try
        {
            string fullPath = GetFullPath(saveFilePath);
            var json = JsonUtility.ToJson(obj);
            writer = new StreamWriter(fullPath, append);
            writer.Write(json);

            Debug.Log($"successfully saved file to {fullPath}");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        finally
        {
            writer?.Close();
        }

        // string json = JsonUtility.ToJson(obj);
        // var bytes = Encoding.UTF8.GetBytes(json);
        // File.WriteAllBytes(GetFullPath(saveFilePath), bytes);
    }

    public static bool TryLoad<T>(string saveFilePath, out T loadedData)
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(GetFullPath(saveFilePath));
            var json = reader.ReadToEnd();

            Debug.Log(json);

            loadedData = JsonUtility.FromJson<T>(json);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            loadedData = default(T);
            return false;
        }
        finally
        {
            reader?.Close();
        }

        // string json = File.ReadAllText(GetFullPath(saveFilePath));
        // object obj = JsonUtility.FromJson<object>(json);
        // return obj;
    }

    static string GetFullPath(string path)
    {
        return Application.persistentDataPath + Path.DirectorySeparatorChar + path + ".sav";
    }
}
