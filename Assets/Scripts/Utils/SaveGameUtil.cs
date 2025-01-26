using BayatGames.SaveGameFree;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class SaveGameUtil
{
    [MenuItem("Farm/Saves/DeleteAll")]
    public static void DeleteAll()
    {
        SaveGame.DeleteAll();
        Debug.Log("All saves have been deleted");
    }

    [MenuItem("Farm/Saves/DeleteCollectible")]
    public static void DeleteCollectible()
    {
        SaveGame.Delete("save_collectible.dat");
        Debug.Log("All collectibles have been deleted");
    }

    [MenuItem("Farm/Saves/DeletePlaceable")]
    public static void DeletePlaceable()
    {
        SaveGame.Delete("save_placeable.dat");
        Debug.Log("All placeables have been deleted");
    }
}
#endif
