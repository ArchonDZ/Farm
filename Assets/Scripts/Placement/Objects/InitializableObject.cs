using UnityEngine;

public abstract class InitializableObject : MonoBehaviour
{
    // call after Awake
    public abstract void Initialize(InitializableItem initializableItem, PlacebleData placebleData);
}
