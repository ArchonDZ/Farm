using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleLoadConfig", menuName = "Farm/Configs/CollectibleLoadConfig")]
public class CollectibleLoadConfig : ScriptableObject
{
    public CollectibleType FullDatabase;
    public List<int> IDDistinct = new List<int>();
    public List<SerializableRange> IDRanges = new List<SerializableRange>();
}
