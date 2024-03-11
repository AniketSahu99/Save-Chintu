using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionControl : MonoBehaviour
{
    [SerializeField]
    public Manager manager;

    [SerializeField]
    public int DefaultLayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<Manager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (manager.foodDict[this.gameObject].isAttachedToList == false)
        {
            Destroy(this.gameObject.GetComponent<Rigidbody2D>());
            this.gameObject.layer = DefaultLayer;
            manager.UpdateLocationOfSpawned(this.gameObject, collision.gameObject);
        }
    }

}
