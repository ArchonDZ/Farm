using BayatGames.SaveGameFree;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveData { }

public class SaveSystem : MonoBehaviour
{
    private List<ISaveData> saves;

    void Awake()
    {
        Load();
    }

    void OnDisable()
    {
        Save();
    }

    public bool TryGetSave<T>(out T result) where T : class
    {
        var save = saves.Find(x => x is T);
        if (save != null)
        {
            result = save as T;
            return true;
        }
        result = null;
        return false;
    }

    public void AddSave(ISaveData save)
    {
        if (!saves.Contains(save))
            saves.Add(save);
    }

    private void Load()
    {
        saves = SaveGame.Load<List<ISaveData>>("save_data.dat", false, "FarmOfDmitryZinovsky") ?? new List<ISaveData>();
    }

    private void Save()
    {
        SaveGame.Save("save_data.dat", saves);
    }
}
