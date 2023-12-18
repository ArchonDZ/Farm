using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] private Plant plant;

    public void Plant(Vector3 pos)
    {
        Instantiate(plant, pos, Quaternion.identity);
    }
}
