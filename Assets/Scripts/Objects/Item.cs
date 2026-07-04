using UnityEngine;
using TMPro;

// requires a TMP component
public class Item : MoveableObject
{
    // Public Fields
    public int Level;
    public VariantType Variant;

    // Private Fields
    private TextMeshPro numberText;

    // Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();

        numberText = GetComponentInChildren<TextMeshPro>();
        numberText.text = $"{Level}";
    }

    // Public Methods
    public void Merge()
    {
        Level++;
        numberText.text = $"{Level}";
    }

    public void SetLevel(int level)
    {
        Level = level;
        numberText.text = $"{Level}";
    }

    // Private Methods
    // you need to add an override to adjust the text's sorting order when the item is moved
    protected override void HandleStartDrag()
    {
        base.HandleStartDrag();

        numberText.sortingOrder = 3;
    }

    protected override void HandleReleased(Vector3 position)
    {
        base.HandleReleased(position);

        numberText.sortingOrder = 2;
    }

    protected override void HandleTap()
    {
        base.HandleTap();

        numberText.sortingOrder = 2;
    }
}
