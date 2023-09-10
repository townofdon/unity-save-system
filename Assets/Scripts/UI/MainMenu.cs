using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button buttonContinue01;
    [SerializeField] Button buttonContinue02;
    [SerializeField] Button buttonContinue03;
    [SerializeField] Button buttonContinue04;
    [SerializeField] Button buttonNewGame01;
    [SerializeField] Button buttonNewGame02;
    [SerializeField] Button buttonNewGame03;
    [SerializeField] Button buttonNewGame04;
    [SerializeField] Button buttonClearSave01;
    [SerializeField] Button buttonClearSave02;
    [SerializeField] Button buttonClearSave03;
    [SerializeField] Button buttonClearSave04;
    [SerializeField] GameObject saveInfo01;
    [SerializeField] GameObject saveInfo02;
    [SerializeField] GameObject saveInfo03;
    [SerializeField] GameObject saveInfo04;
    [SerializeField] GameObject openSlot01;
    [SerializeField] GameObject openSlot02;
    [SerializeField] GameObject openSlot03;
    [SerializeField] GameObject openSlot04;

    [Space]
    [Space]

    [SerializeField] TextMeshProUGUI fieldTimePlayed01;
    [SerializeField] TextMeshProUGUI fieldTimePlayed02;
    [SerializeField] TextMeshProUGUI fieldTimePlayed03;
    [SerializeField] TextMeshProUGUI fieldTimePlayed04;
    [SerializeField] TextMeshProUGUI fieldLastUpdated01;
    [SerializeField] TextMeshProUGUI fieldLastUpdated02;
    [SerializeField] TextMeshProUGUI fieldLastUpdated03;
    [SerializeField] TextMeshProUGUI fieldLastUpdated04;
    [SerializeField] TextMeshProUGUI fieldLevel01;
    [SerializeField] TextMeshProUGUI fieldLevel02;
    [SerializeField] TextMeshProUGUI fieldLevel03;
    [SerializeField] TextMeshProUGUI fieldLevel04;

    DataPersistor dataPersistor;
    SaveMetadata[] metadatas;

    public void ContinueGame(int slotIndex)
    {
        LoadGame(slotIndex, false);
    }

    public void NewGame(int slotIndex)
    {
        LoadGame(slotIndex, true);
    }

    public void ClearSave(int slotIndex)
    {
        var slot = GetSlotFromIndex(slotIndex);
        dataPersistor.ClearSave(slot);
        dataPersistor.SetSaveSlot(SaveSlot.None);
        LoadMetadata();
        RenderUI();
    }

    void LoadGame(int slotIndex, bool isNewGame)
    {
        var slot = GetSlotFromIndex(slotIndex);
        if (isNewGame) dataPersistor.ClearSave(slot);
        dataPersistor.SetSaveSlot(slot);

        GlobalEvents.OnStartGame?.Invoke();
    }

    SaveSlot GetSlotFromIndex(int index)
    {
        return index switch
        {
            0 => SaveSlot.A,
            1 => SaveSlot.B,
            2 => SaveSlot.C,
            3 => SaveSlot.D,
            _ => throw new Exception($"Invalid save slot index: {index}"),
        };
    }

    void Start()
    {
        dataPersistor = FindObjectOfType<DataPersistor>();
        dataPersistor.SetSaveSlot(SaveSlot.None);

        LoadMetadata();
        RenderUI();
    }

    void LoadMetadata()
    {
        metadatas = dataPersistor.LoadAllSaveFileMetadata();
    }

    void RenderUI()
    {
        RenderSection(metadatas[0], buttonContinue01, buttonNewGame01, buttonClearSave01, saveInfo01, openSlot01);
        RenderSection(metadatas[1], buttonContinue02, buttonNewGame02, buttonClearSave02, saveInfo02, openSlot02);
        RenderSection(metadatas[2], buttonContinue03, buttonNewGame03, buttonClearSave03, saveInfo03, openSlot03);
        RenderSection(metadatas[3], buttonContinue04, buttonNewGame04, buttonClearSave04, saveInfo04, openSlot04);

        RenderInfo(metadatas[0], fieldTimePlayed01, fieldLastUpdated01, fieldLevel01);
        RenderInfo(metadatas[1], fieldTimePlayed02, fieldLastUpdated02, fieldLevel02);
        RenderInfo(metadatas[2], fieldTimePlayed03, fieldLastUpdated03, fieldLevel03);
        RenderInfo(metadatas[3], fieldTimePlayed04, fieldLastUpdated04, fieldLevel04);
    }

    void RenderSection(SaveMetadata metadata, Button buttonContinue, Button buttonNewGame, Button buttonClearSave, GameObject saveInfo, GameObject openSlot)
    {
        if (metadata.timeLastUpdatedBinary == default)
        {
            buttonContinue.gameObject.SetActive(false);
            buttonClearSave.gameObject.SetActive(false);
            buttonNewGame.gameObject.SetActive(true);
            saveInfo.SetActive(false);
            openSlot.SetActive(true);
        }
        else
        {
            buttonContinue.gameObject.SetActive(true);
            buttonClearSave.gameObject.SetActive(true);
            buttonNewGame.gameObject.SetActive(false);
            saveInfo.SetActive(true);
            openSlot.SetActive(false);
        }

    }

    void RenderInfo(SaveMetadata metadata, TextMeshProUGUI fieldTimePlayed, TextMeshProUGUI fieldLastUpdated, TextMeshProUGUI fieldLevel)
    {
        fieldLastUpdated.text = DateTime.FromBinary(metadata.timeLastUpdatedBinary).ToString("MM/dd/yy H:mm:ss");
        // fieldTimePlayed.text = TimeSpan.FromSeconds(metadata.timeSpentPlayingSeconds).ToString(@"hh\:mm\:ss");
        fieldTimePlayed.text = GetTimePlayingDisplay(metadata.timeSpentPlayingSeconds);
        fieldLevel.text = metadata.sceneName;
    }

    string GetTimePlayingDisplay(double timeSpendPlayingSeconds)
    {
        int minutes = (int)(timeSpendPlayingSeconds / 60);
        int seconds = (int)(timeSpendPlayingSeconds % 60);
        return $"{minutes.ToString().PadLeft(2, '0')}:{seconds.ToString().PadLeft(2, '0')}";
    }
}
