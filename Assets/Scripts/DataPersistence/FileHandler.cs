
using System.IO;
using System.Text;
using UnityEngine;

public enum SaveSlot
{
    None = 0,
    A,
    B,
    C,
    D,
}

public class FileHandler
{
    private const bool DEBUG = true;
    private const string EK = "BAA896AA-D894-4088-9313-C084CE07CA1E";
    private const string DEFAULT_SAVEFILE_NAME = "gamedata";
    private const string EXTENSION = ".sav";
    private const string SAVE_SLOT_DIR_A = "01";
    private const string SAVE_SLOT_DIR_B = "02";
    private const string SAVE_SLOT_DIR_C = "03";
    private const string SAVE_SLOT_DIR_D = "04";
    private readonly string saveFileName;

    public FileHandler(string saveFileName = DEFAULT_SAVEFILE_NAME)
    {
        if (string.IsNullOrWhiteSpace(saveFileName))
        {
            throw new System.Exception("[FileHandler] saveFileName cannot be blank");
        }
        this.saveFileName = saveFileName;
    }

    string GetFullPath(SaveSlot slot)
    {
        return Path.Combine(Application.persistentDataPath, GetDirectoryFromSaveSlot(slot), BuildString(saveFileName, EXTENSION));
    }

    string GetSaveSlotPath(SaveSlot slot)
    {
        return Path.Combine(Application.persistentDataPath, GetDirectoryFromSaveSlot(slot));
    }

    string GetDirectoryFromSaveSlot(SaveSlot slot)
    {
        return slot switch
        {
            SaveSlot.A => SAVE_SLOT_DIR_A,
            SaveSlot.B => SAVE_SLOT_DIR_B,
            SaveSlot.C => SAVE_SLOT_DIR_C,
            SaveSlot.D => SAVE_SLOT_DIR_D,
            _ => throw new System.Exception($"Invalid save slot: {slot}"),
        };
    }

    public bool Save(SaveSlot slot, object obj, bool append = false, bool useEncryption = false)
    {
        TextWriter writer = null;
        var path = GetFullPath(slot);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var json = JsonUtility.ToJson(obj);
            if (useEncryption)
            {
                json = EncryptDecrypt(json);
            }
            using (writer = new StreamWriter(path, append))
            {
                writer.Write(json);
            }

            if (DEBUG) Debug.Log($"[FileRW] successfully saved file to {path}");

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

    public bool TryLoad<T>(SaveSlot slot, out T loadedData, bool useEncryption = false)
    {
        TextReader reader = null;
        var path = GetFullPath(slot);
        try
        {
            using (reader = new StreamReader(path))
            {
                var json = reader.ReadToEnd();
                if (useEncryption)
                {
                    json = EncryptDecrypt(json);
                }

                if (DEBUG) Debug.Log(json);

                loadedData = JsonUtility.FromJson<T>(json);
                return true;
            }

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

    public void Delete(SaveSlot slot)
    {
        var path = GetSaveSlotPath(slot);
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                if (DEBUG) Debug.LogWarning("[FileRW][Delete] No data found at path: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FileRW] Failed to delete file \"{path}\"");
            Debug.LogError(e);
        }
    }

    string BuildString(params string[] parts)
    {
        var modified = new StringBuilder(parts.Length);
        for (int i = 0; i < parts.Length; i++)
        {
            modified.Append(parts[i]);
        }
        return modified.ToString();
    }

    // simple implementation of XOR encryption - NOT a super secure way to encrypt a save file
    string EncryptDecrypt(string original)
    {
        var modified = new StringBuilder(original.Length);
        for (int i = 0; i < original.Length; i++)
        {
            modified.Append((char)(original[i] ^ EK[i % EK.Length]));
        }
        return modified.ToString();
    }
}
