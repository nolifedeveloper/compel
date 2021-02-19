using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class RoomManager : MonoBehaviour
{
    [Header("Room Lists")]
    public int ROOM_COUNT = 0;
    public int LAST_ROOM = 19;
    private const int LAST_ROOM_VAL = 19;

    public List<GameObject> ROOM_PREFAB_LIST;
    public List<GameObject> ROOM_LIST;
    public List<GameObject> ACTIVE_ROOMS;

    private bool bossCheckpointReached = false;
    private string PATH = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\nld\compel\";
    private string fullPath => $"{PATH} cp.nld";

    [Header("Special Rooms")]
    public GameObject START_ROOM;
    public GameObject BOSS_ROOM;
    private bool bossRoomHasSpawned = false;


    [Header("Temporary Rooms")]
    public GameObject PREVIOUS_ROOM;
    public GameObject CURRENT_ROOM;
    public GameObject NEXT_ROOM;

    public static RoomManager instance;


    private void ReadCheckpoint()
    {
        if (!Directory.Exists(PATH)) { Directory.CreateDirectory(PATH); }

        if(!File.Exists(fullPath))
        {
            File.WriteAllText(fullPath, bossCheckpointReached.ToString());
        }
        else if(File.Exists(fullPath))
        {
            bossCheckpointReached = Convert.ToBoolean(File.ReadAllText(fullPath));
        }

        LAST_ROOM = bossCheckpointReached ? 0 : LAST_ROOM_VAL;
    }

    public void SetCheckpoint(bool val)
    {
        bossCheckpointReached = val;
        File.WriteAllText(fullPath, bossCheckpointReached.ToString());
    }

    private void Awake()
    {
        InitSingleton();
        ReadCheckpoint();
        InitRooms();
    }

    private void InitSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this)
        {
            Destroy(this);
        }
    }

    //Call on completion
    public void CurrentRoomCleared()
    {
        RoomOfObject(CURRENT_ROOM).ChangeDoor(false);
        InGameUI.Instance.UpdatePathUI();
    }

    //Kamerayı bir sonraki odaya götür, yeni oda spawn'la, kapıları kitle.
    public IEnumerator ActivateRoom()
    {
        float t = 0;
        ROOM_COUNT++;

        if(bossRoomHasSpawned)
        {
            ProjectileManager.instance.PRIEST.CanAct(false);
            RoomOfObject(CURRENT_ROOM).ChangeDoor(true);
            Vector2 bossTarget = (Vector2)Camera.main.transform.parent.position + (Vector2.up * (RoomOfObject(NEXT_ROOM).roomHeight - 0.362548f));
            Camera.main.orthographicSize = 1.07f;

            while (t < 1)
            {
                t += Time.deltaTime * 2.1f;
                Camera.main.transform.parent.position = Vector3.Lerp(Camera.main.transform.parent.position, new Vector3(bossTarget.x, bossTarget.y, Camera.main.transform.parent.position.z), t);
                yield return null;
            }

            yield return StartCoroutine(InGameUI.Instance.SATAN_HIMSELF.UnleashSatan());
            InGameUI.Instance.UpdateUIMode();
            yield break;
        }
        




        Vector2 target = (Vector2)Camera.main.transform.parent.position + (Vector2.up * RoomOfObject(NEXT_ROOM).roomHeight);

        SpawnRoom();

        InGameUI.Instance.UpdateRoomUI();

        RoomOfObject(PREVIOUS_ROOM).ChangeDoor(true);
        RoomOfObject(CURRENT_ROOM).ChangeDoor(true);
        RoomOfObject(NEXT_ROOM).ChangeDoor(true);

        ProjectileManager.instance.PRIEST.CanAct(false);



        while (t < 1)
        {
            t += Time.deltaTime * 2.1f;
            Camera.main.transform.parent.position = Vector3.Lerp(Camera.main.transform.parent.position, new Vector3(target.x, target.y, Camera.main.transform.parent.position.z), t);
            yield return null;
        }


        Room curRoom = RoomOfObject(CURRENT_ROOM);
        curRoom.WakeDemons();
        yield return new WaitForSeconds(curRoom.DEMONS[0].totalParticleDelay);

        ProjectileManager.instance.PRIEST.CanAct(true);
        curRoom.EnableDemonAI();
        UpdateRooms();
    }

    //Eğer şu anki oda 3. oda ya da daha fazlası ise ve başlangıç odası aktif ise, deaktive et.
    private void DeactivateStartRoom()
    {
        if (ROOM_COUNT >= 3 && START_ROOM.activeSelf)
        {
            START_ROOM.SetActive(false);
        }
    }

    //Oyun başladığında, başlangıç odasının kilidini açıp yeni oda spawn et.
    public void StartingRoom()
    {
        NEXT_ROOM = START_ROOM;

        RoomOfObject(NEXT_ROOM).ChangeDoor(false);

        SpawnRoom();

        InGameUI.Instance.UpdateRoomUI();
        InGameUI.Instance.UpdatePathUI();

    }

    //Bir GameObject'ten Room component'ı döndür.
    public Room RoomOfObject(GameObject obj)
    {
        return obj.GetComponent<Room>();
    }


    private void InitRooms()
    {
        for (int i = 0; i < ROOM_PREFAB_LIST.Count; i++)
        {
            GameObject room = Instantiate(ROOM_PREFAB_LIST[i], gameObject.transform);
            room.SetActive(false);
            ROOM_LIST.Add(room);
        }
    }

    //Listeden rastgele oda seç, pozisyonunu ayarla. Önceki, şimdiki ve sonraki odanın referansını güncelle. Odayı aktifleştir.
    [ContextMenu("Spawn room")]
    public void SpawnRoom()
    {
        GameObject room = GetRoomGameObj();
        PREVIOUS_ROOM = CURRENT_ROOM;

        CURRENT_ROOM = NEXT_ROOM;

        NEXT_ROOM = room;

        if(room.name == "BOSS_ROOM")
        {
            room.transform.position = (Vector2)CURRENT_ROOM.transform.position + (RoomOfObject(room).roomHeight * Vector2.up) - (Mathf.Abs(RoomOfObject(room).roomHeight - (RoomOfObject(CURRENT_ROOM).roomHeight + 0.28f)) * Vector2.up);
            room.SetActive(true);
        }
        else
        {
            room.transform.position = (Vector2)CURRENT_ROOM.transform.position + (RoomOfObject(room).roomHeight * Vector2.up) - (Mathf.Abs(RoomOfObject(room).roomHeight - (RoomOfObject(CURRENT_ROOM).roomHeight)) * Vector2.up);
            room.SetActive(true);
        }

    }

    //Oda listesinden aktif olmayan, kullanıma hazır bir oda döndür.
    private GameObject GetRoomGameObj()
    {
        if (ROOM_COUNT < LAST_ROOM)
        {
            if (ROOM_LIST.Count > 0)
            {
                GameObject room = ROOM_LIST[UnityEngine.Random.Range(0, ROOM_LIST.Count)];
                ACTIVE_ROOMS.Add(room);
                ROOM_LIST.Remove(room);
                return room;
            }
            GameObject newRoom = Instantiate(ROOM_PREFAB_LIST[UnityEngine.Random.Range(0, ROOM_PREFAB_LIST.Count)], gameObject.transform);

            newRoom.SetActive(false);

            ACTIVE_ROOMS.Add(newRoom);

            return newRoom;
        }
        else
        {
            GameObject bossRoom = Instantiate(BOSS_ROOM, gameObject.transform);
            bossRoom.name = "BOSS_ROOM";
            bossRoom.SetActive(false);
            bossRoomHasSpawned = true;
            return bossRoom;
        }

    }

    //Oda listesindeki odaları kontrol et, eğer odalar şimdiki, önceki ve sonraki odalardan değil ise, deaktifleştir.
    private void UpdateRooms()
    {
        DeactivateStartRoom();

        for (int i = 0; i < ACTIVE_ROOMS.Count; i++)
        {
            GameObject obj = ACTIVE_ROOMS[i];

            if (obj != NEXT_ROOM && obj != CURRENT_ROOM && obj != PREVIOUS_ROOM)
            {
                obj.SetActive(false);
                ROOM_LIST.Add(obj);
                ACTIVE_ROOMS.Remove(obj);
            }
        }
    }
}
