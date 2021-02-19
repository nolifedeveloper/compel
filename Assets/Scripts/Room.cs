
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //-ON ENTRANCE -Lock the doors move the camera , spawn enemies one by one and take control from the player.
    [Header("Room Pathfinding Limits")]
    [SerializeField] private Vector2 minRange = new Vector2(-0.53f, -0.62f);
    [SerializeField] private Vector2 maxRange = new Vector2(1.4f, 0.15f);

    public Vector2 RandomPosition
    {
        get
        {
            return new Vector2(Random.Range(minRange.x,maxRange.x), Random.Range(minRange.y,maxRange.y));
        }
    }


    [Header("Raycast")]
    public Transform raycastTransform;
    public GameObject topPoint, bottomPoint;
    private LayerMask PRIEST_LAYER = 9;
    public float roomHeight => Vector2.Distance(topPoint.transform.localPosition,bottomPoint.transform.localPosition);
    [Header("Door")]
    public GameObject DOOR;
    private bool hasEntered = false;
    [Header("Enemy Stuff")]
    public List<Demon> DEMONS;

    private void OnDisable()
    {
        SetRoomDefaultState();
    }

    private void Update()
    {
        CastRay();
    }

    private void CastRay()
    {
        RaycastHit2D raycasthit = Physics2D.Raycast(raycastTransform.position, Vector2.right,Mathf.Infinity);

        if(raycasthit.collider != null && hasEntered == false)
        {
            if(raycasthit.collider.gameObject.layer == PRIEST_LAYER)
            {
                StartCoroutine(RoomManager.instance.ActivateRoom());
                hasEntered = true;
            }
        }
    }

    //Eğer bütün demon'lar deaktif ise, bir sonraki odayı spawnla.
    public void CheckDemons()
    {
        for(int i = 0; i < DEMONS.Count; i++)
        {
            if(DEMONS[i].gameObject.activeSelf)
            {
                return;
            }
        }
        RoomManager.instance.CurrentRoomCleared();
    }

    public void WakeDemons()
    {
        for (int i = 0; i < DEMONS.Count; i++)
        {
            DEMONS[i].gameObject.SetActive(true);
            DEMONS[i].StartCoroutine(DEMONS[i].OnSpawn());
        }
    }

    public void EnableDemonAI()
    {
        for (int i = 0; i < DEMONS.Count; i++)
        {
            DEMONS[i].CanAct(true);
        }
    }

    private void SetRoomDefaultState()
    {
        ChangeDoor(false);
        hasEntered = false;
        //Reset Enemies etc.
    }

    public void ChangeDoor(bool val)
    {
        DOOR.SetActive(val);
    }
}
