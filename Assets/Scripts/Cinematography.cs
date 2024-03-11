using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cinematography : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI headingToText;

    [SerializeField]
    private TextMeshProUGUI distanceText;

    public Manager manager;

    private Food head;
    public GameObject[] path;
    private float dis;
    GameObject gameFood;
    Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<Manager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        head = manager.foodList.head;
        headingToText.text = (head.headingPathNumber+1).ToString();

        gameFood = manager.FindGameObj(head);
        destination = path[head.headingPathNumber+1].transform.position;

        dis = Vector3.Distance(gameFood.transform.position, destination);

        distanceText.text = dis.ToString();
    }
}
