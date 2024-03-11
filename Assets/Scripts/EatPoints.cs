using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EatPoints : MonoBehaviour
{
    [SerializeField]
    private Manager manager;

    [SerializeField]
    private GameObject LostCanvas;

    public GameObject newLevels;

    private AudioSource eatsfx;

    void Start()
    {
        manager = GameObject.Find("Game Manager").GetComponent<Manager>();
        eatsfx = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int foodNo = manager.foodDict[collision.gameObject].foodNumber;
        manager.foodList.DeleteFood(manager.foodDict[collision.gameObject]);
        eatsfx.Play();
        Destroy(collision.gameObject);
        if(manager.isUnlimited)
        {
            if(foodNo % 2 == 0) //Bad
            {
                manager.Lifes[manager.noOfLivesLeft-- - 1].SetActive(false);
                if(manager.noOfLivesLeft == 0)
                {
                    Time.timeScale = 0;
                    LostCanvas.SetActive(true);
                }
            }
            else                //Good
            {
                if(manager.noOfLivesLeft < manager.Lifes.Length)
                    manager.Lifes[manager.noOfLivesLeft++].SetActive(true);
            }

            manager.CheckWin();
        }
        else
        {
            /*manager.Lifes[manager.noOfLivesLeft-- - 1].SetActive(false);
            if (manager.noOfLivesLeft == 0)
            {*/
            Time.timeScale = 0;
            LostCanvas.SetActive(true);
            /*}
            else if(manager.foodList.head == null)
            {
                if (SceneManager.GetActiveScene().name != "Level_3")
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                else
                    newLevels.SetActive(true);
            }*/
        }
    }
}
