using UnityEngine;
using System;

// requires moveable object component - maybe change to base class
// spawns items when clicked
public class Spawner : MoveableObject
{
    // Events
    public event Action<Item> ItemSpawned;

    // Inspector Fields
    [SerializeField] GameObject spawnableItem;  // item prefab to be spawned
    [SerializeField] GameObject itemParent; // parent for the item prefabs


    // Event Handlers
    protected override void HandleTap()
    {
        Debug.Log("spawner tapped");

        SpawnItem();
    }

    // Private Methods
    // spawns item on top of parent then animates to spot on the board
    private void SpawnItem()
    {
        Debug.Log("spawn item");

        // create the item on top of the parent
        GameObject newItem = Instantiate(spawnableItem, transform.position, Quaternion.identity, itemParent.transform);
        newItem.GetComponent<SpriteRenderer>().sortingOrder = 2;

        // move item to the board
        ItemSpawned?.Invoke(newItem.GetComponent<Item>());
    }
}
