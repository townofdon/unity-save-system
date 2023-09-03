
using System.IO;
using System.Text;
using UnityEngine;

public static class FileRW
{
    private const string EK = "BAA896AA-D894-4088-9313-C084CE07CA1E";

    public static bool Save(string path, object obj, bool append = false, bool useEncryption = false)
    {
        TextWriter writer = null;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var json = JsonUtility.ToJson(obj);
            if (useEncryption)
            {
                json = EncryptDecrypt(json);
            }
            writer = new StreamWriter(path, append);
            writer.Write(json);

            Debug.Log($"[FileRW] successfully saved file to {path}");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FileRW] Failed to save file \"{path}\"");
            Debug.LogError(e);
            return false;
        }
        finally
        {
            writer?.Close();
        }
    }

    public static bool TryLoad<T>(string path, out T loadedData, bool useEncryption = false)
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(path);
            var json = reader.ReadToEnd();
            if (useEncryption)
            {
                json = EncryptDecrypt(json);
            }

            Debug.Log(json);

            loadedData = JsonUtility.FromJson<T>(json);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FileRW] Failed to load file \"{path}\"");
            Debug.LogError(e);
            loadedData = default(T);
            return false;
        }
        finally
        {
            reader?.Close();
        }
    }

    public static void Delete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                // // delete the profile folder and everything within it
                // Directory.Delete(Path.GetDirectoryName(path), true);
                File.Delete(path);
            }
            else
            {
                Debug.LogWarning("[FileRW] Tried to delete profile data, but data was not found at path: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FileRW] Failed to delete file \"{path}\"");
            Debug.LogError(e);
        }
    }

    // the below is a simple implementation of XOR encryption
    static string EncryptDecrypt(string original)
    {
        var modified = new StringBuilder(original.Length);
        // char[] modified = original.ToCharArray();
        for (int i = 0; i < original.Length; i++)
        {
            modified.Append((char)(original[i] ^ EK[i % EK.Length]));
        }
        return modified.ToString();
    }

    // public unsafe static void Clear(this string s)
    // {
    //     fixed (char* ptr = s)
    //     {
    //         for (int i = 0; i < s.Length; i++)
    //         {
    //             ptr[i] = '\0';
    //         }
    //     }
    // }

    public static string GetFullPath(string saveFileName)
    {
        return Path.Combine(Application.persistentDataPath, saveFileName + ".sav");
    }
}
