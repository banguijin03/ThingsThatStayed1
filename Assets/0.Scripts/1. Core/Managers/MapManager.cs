using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    None,
    Forest,
    WaterFall
}

public class MapManager : ManagerBase
{
    [Header("맵 목록")]
    [SerializeField] List<MapBase> maps = new();

    MapBase currentMap;

    public MapBase CurrentMap => currentMap;

    public MapType CurrentMapType =>
        currentMap != null ? currentMap.MapType : MapType.None;


    protected override IEnumerator OnConnected(GameManager newManager)
    {
        yield return null;
    }


    protected override void OnDisconnected()
    {
        if (currentMap != null)
        {
            Destroy(currentMap.gameObject);
            currentMap = null;
        }
    }


    public void LoadMap(MapType mapType)
    {
        MapBase mapPrefab = maps.Find(map => map != null && map.MapType == mapType);
        if (mapPrefab == null) return;

        if (currentMap != null)
        {
            Destroy(currentMap.gameObject);
            currentMap = null;
        }

        currentMap = Instantiate(mapPrefab);
        ObjectManager.RegistrationObject(currentMap.gameObject);
    }
}