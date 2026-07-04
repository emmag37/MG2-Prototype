using UnityEngine;
using System;

public class Powerup : MoveableObject
{
    // on double tap, disappear and spawn an item

    // Events
    public event Action<Item> ItemSpawned;

    // Inspector Fields
    [SerializeField] private GameObject spawnableItem;
    [SerializeField] private GameObject itemParent;
    [SerializeField] private VariantType variant;
    [SerializeField] private int level;

    // private fields
    private float doubleTapWindow = 0.3f;
    private float lastTapTime = -1f;    // sets so no initial double tap on single tap

    // checks for the double tap
    protected override void HandleTap()
    {
        base.HandleTap();

        float timeElapsed = Time.time - lastTapTime;

        if (timeElapsed < doubleTapWindow)
        {
            // execute the double tap code
            Debug.Log("double tap");

            SpawnItem();
            Destroy(gameObject);
        }
        else
        {
            lastTapTime = Time.time;
        }
    }
    
    private void SpawnItem()
    {
        // create the item on top of the parent
        Item newItem = Instantiate(spawnableItem, transform.position, Quaternion.identity, itemParent.transform).GetComponent<Item>();
        newItem.GetComponent<SpriteRenderer>().sortingOrder = 2;
        newItem.Index = Index;
        newItem.Variant = variant;
        newItem.SetLevel(level);

        // move item to the board
        ItemSpawned?.Invoke(newItem);
    }
}
