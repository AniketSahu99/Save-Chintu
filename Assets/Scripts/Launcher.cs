using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Mathematics;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    private GameObject plate;

    [SerializeField]
    private GameObject LaunchSpawn;

    [SerializeField]
    private float offset = 90;

    [SerializeField]
    private Manager manager;

    [SerializeField]
    private float ForceMagnitude = 100f;

    [SerializeField]
    private float minDistanceAfterLaunch = 3f;

    [SerializeField]
    private int ShootLayer = 7;

    private Vector3 lookDirection;
    private float lookAngle;
    private Transform child;
    private int k;

    int maxSize;
    // Start is called before the first frame update
    void Start()
    {
        child = transform;
        manager = GameObject.Find("Game Manager").GetComponent<Manager>();
        maxSize = manager.noOfFoodsToBeSpawned;
        SpawnLaunch();
        k = maxSize + 1;
    }

    private void SpawnLaunch()
    {
        if (manager.noOfFoodsToBeSpawned < maxSize)
        {
            int nF = manager.foodList.GenerateRandom(manager.Collection.Length);
            Food newFood = new Food(nF);

            GameObject prefabObj = manager.Collection[nF];

            GameObject newObj = new GameObject(prefabObj.name + "_" + ++k);

            manager.CopyComponent(prefabObj, ref newObj, true);

            newObj.layer = ShootLayer;
            newObj.transform.position = LaunchSpawn.transform.position;
            newObj.AddComponent<Rigidbody2D>();
            
            newObj.GetComponent<CircleCollider2D>().isTrigger = true;
            newObj.transform.parent = plate.transform;

            manager.foodDict[newObj] = newFood;
        }

    }
    // Update is called once per frame
    void Update()
    {
        DirectionUpdate();
        if(plate.transform.childCount == 1 && Vector3.Distance(plate.transform.position, child.transform.position) > minDistanceAfterLaunch)
        {
            SpawnLaunch();
        }
        else if(plate.transform.childCount == 2)
        {
            if(Input.GetMouseButtonDown(0))
            { 
                child = plate.transform.GetChild(1);
                Rigidbody2D rb = plate.GetComponentInChildren<Rigidbody2D>();

                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - plate.transform.position;
                rb.AddForce(direction.normalized * ForceMagnitude);
                child.SetParent(null);

            }
        }

    }

    private void DirectionUpdate()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = lookDirection - plate.transform.position;

        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - offset;

        plate.transform.rotation = Quaternion.Euler(0, 0, lookAngle);
    }
}
