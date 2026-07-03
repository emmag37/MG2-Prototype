using UnityEngine;

// requires moveable object component
public class Spawner : MonoBehaviour
{
    // this needs to spawn items when tapped/clicked

    // Private Fields
    private MoveableObject moveable;

    // Unity Lifecycle
    void Awake()
    {
        moveable = GetComponent<MoveableObject>();

        moveable.MovObjTapped += HandleTap;
    }

    void OnDestroy()
    {
        moveable.MovObjTapped -= HandleTap;
    }

    // Event Handlers
    private void HandleTap()
    {
        Debug.Log("spawner tapped");
    }
}
