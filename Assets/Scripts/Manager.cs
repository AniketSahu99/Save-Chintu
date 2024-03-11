using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Manager : MonoBehaviour
{
    //Spawn Components
    [SerializeField]
    public GameObject[] Collection;

    [SerializeField]
    private AudioSource eatSfx;

    [SerializeField]
    private float WaitTime = 0.6f;
    
    [SerializeField]
    public int noOfFoodsToBeSpawned = 5;
    private int spawnedNumber;

    [SerializeField]
    private GameObject SpawnPoint;

    [SerializeField]
    private float speed = 2f;

    [SerializeField]
    public bool isUnlimited = false;

    [SerializeField]
    private List<GameObject> path;

    [SerializeField]
    public GameObject[] Lifes;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    public GameObject newLevels;

    private float ScoreText = 0;

    public int noOfLivesLeft = 3;

    public bool moveBack = false;
    
    public Dictionary<GameObject, Food> foodDict = new Dictionary<GameObject, Food>();

    public FoodList foodList = new FoodList();

    Food tail = null;
    Food last = null;
    void Start()
    {
        noOfLivesLeft = Lifes.Length;
        spawnedNumber = noOfFoodsToBeSpawned;
        StartCoroutine(SpawnFoods());
    }

    void Update()
    {
        if (!moveBack)
        {
            MoveForward();
            CheckRumba();
        }
        else
        {
            if(tail != null && last != null && tail != last)
                MoveBackward(tail,last);
            else
                moveBack = false;
        }

        ManageScore();

    }

    private void ManageScore()
    {
        if (isUnlimited)
        {
            ScoreText += 0.002f;
            if(scoreText != null)
                scoreText.text = Math.Floor(ScoreText).ToString();
        }
    }
    private void MoveBackward(Food tail, Food last)
    {
        while(Vector3.Distance(FindGameObj(tail).transform.position, FindGameObj(last).transform.position) > 1.3f) 
        {
            GameObject temp = FindGameObj(tail);

            while (tail != null)
            {
                GameObject gameFood = FindGameObj(tail);
                Vector3 destination = path[tail.headingPathNumber - 1].transform.position;

                gameFood.transform.position = Vector3.MoveTowards(gameFood.transform.position, destination, speed * Time.deltaTime);

                float distance = Vector3.Distance(gameFood.transform.position, destination);
                if (distance <= 0.1)
                {
                    tail.headingPathNumber--;
                }
                tail = tail.nextFood;
            }
        }
        moveBack = false;
        
    }
    private void MoveForward()
    {
        Food head = foodList.head;
        GameObject temp = FindGameObj(head);

        if (foodList != null)
        {
            while (head != null)
            {
                MovementUpdateHead(head);

                head = head.prevFood;
            }
        }
    }
    private void CheckRumba()
    {
        Food temp = foodList.head;
        while (temp != null)
        {
            if(temp.prevFood != null && temp.prevFood.prevFood != null && temp.foodNumber == temp.prevFood.foodNumber && 
                                                            temp.foodNumber == temp.prevFood.prevFood.foodNumber)
            {
                Food stored = temp.prevFood.prevFood.prevFood;

                if (temp.nextFood != null)
                {
                    tail = temp.nextFood;
                    moveBack = true;
                    last = stored;
                }


                Destroy(FindGameObj(temp.prevFood.prevFood));
                Destroy(FindGameObj(temp.prevFood));
                Destroy(FindGameObj(temp));

                eatSfx.Play();

                if(temp.prevFood.prevFood.prevFood != null && temp.prevFood.prevFood.prevFood.foodNumber == temp.foodNumber)
                {
                    Destroy(FindGameObj(temp.prevFood.prevFood.prevFood));
                    foodList.DeleteFood(temp.prevFood.prevFood.prevFood);
                    stored = stored.prevFood;
                }

                foodList.DeleteFood(temp.prevFood.prevFood);
                foodList.DeleteFood(temp.prevFood);
                foodList.DeleteFood(temp);

                ScoreText += 5;

                CheckWin();
                
                temp = stored;

            }
            else
            {
                temp = temp.prevFood;
            }
            
        }
    }

    public void CheckWin()
    {
        if(foodList.head == null && noOfFoodsToBeSpawned == 0)
        {
            if(isUnlimited)
            {
                noOfFoodsToBeSpawned = spawnedNumber + 1;
                spawnedNumber++;
                StartCoroutine(SpawnFoods());

            }
            else
            {
                if (SceneManager.GetActiveScene().name != "Level_3")
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                else
                    newLevels.SetActive(true);
            }

        }
    }
    private void MovementUpdateHead(Food thisFood)
    {
        GameObject gameFood = FindGameObj(thisFood);
        Vector3 destination = path[thisFood.headingPathNumber].transform.position;

        gameFood.transform.position = Vector3.MoveTowards(gameFood.transform.position, destination, speed * Time.deltaTime);

        float distance = Vector3.Distance(gameFood.transform.position, destination);
        if (distance <= 0.1)
        {
            thisFood.headingPathNumber++;
        }
    }
    
    public GameObject FindGameObj(Food food)
    {
        foreach(var f in foodDict)
        {
            if (f.Value == food)
                return f.Key;
        }
        return null;
    }

    public IEnumerator SpawnFoods()
    {
        while(noOfFoodsToBeSpawned > 0)
        {
            Food newFood = foodList.AddFoodBack(Collection.Length);

            GameObject prefabObj = Collection[newFood.foodNumber];

            GameObject newObj = new GameObject(prefabObj.name + "_" + noOfFoodsToBeSpawned);

            CopyComponent(prefabObj, ref newObj, false);

            newObj.transform.position = SpawnPoint.transform.position;

            foodDict[newObj] = newFood;
            noOfFoodsToBeSpawned--;
            yield return new WaitForSeconds(WaitTime);
        }
    }

    public void CopyComponent(GameObject fromObj, ref GameObject toObj, bool isLauncher)
    {
        toObj.AddComponent<SpriteRenderer>();
        toObj.GetComponent<SpriteRenderer>().sprite = fromObj.GetComponent<SpriteRenderer>().sprite;
        toObj.GetComponent<SpriteRenderer>().sortingLayerName = fromObj.GetComponent<SpriteRenderer>().sortingLayerName;
        toObj.GetComponent<SpriteRenderer>().sortingOrder = fromObj.GetComponent<SpriteRenderer>().sortingOrder;

        toObj.AddComponent<CircleCollider2D>();
        toObj.GetComponent<CircleCollider2D>().radius = fromObj.GetComponent<CircleCollider2D>().radius;

        if (isLauncher)
        {
            toObj.AddComponent<CollisionControl>();
            toObj.GetComponent<CollisionControl>().DefaultLayer = fromObj.GetComponent<CollisionControl>().DefaultLayer;
            toObj.GetComponent<CollisionControl>().manager = fromObj.GetComponent<CollisionControl>().manager;
        }

        toObj.transform.localScale = fromObj.transform.localScale;
    }

    public void UpdateLocationOfSpawned(GameObject spawnedObj, GameObject collidedObj)
    {
        float diff = spawnedObj.transform.position.x - collidedObj.transform.position.x;
        float direction = path[foodDict[collidedObj].headingPathNumber].transform.position.x - collidedObj.transform.position.x;
        
        if (direction > 0) //Head going Right
        {
            if(diff > 0)
            {
                AddNext(spawnedObj, collidedObj);
            }
            else
            {
                AddPrev(spawnedObj, collidedObj);
            }
        }
        else if(direction < 0)//Head going Left
        {
            if (diff > 0)
            {
                AddPrev(spawnedObj, collidedObj);
            }
            else
            {
                AddNext(spawnedObj, collidedObj);
            }
        }
        else
        {
            Debug.Log("Nahi Hoga");
        }
    }

    private void AddPrev(GameObject spawnedObj, GameObject collidedObj)
    {
        Food spawnedFood = foodDict[collidedObj];

        while (spawnedFood.prevFood != null)
        {
            GameObject spObj = FindGameObj(spawnedFood.prevFood);
            Vector3 dir = (path[foodDict[spObj].headingPathNumber - 2].transform.position - spObj.transform.position).normalized;

            spObj.transform.position = new Vector3(dir.x * 1.2f + spObj.transform.position.x,
                                                                dir.y * 1.2f + spObj.transform.position.y, 0);
            spawnedFood = spawnedFood.prevFood;
        }

        Vector3 direction = (path[foodDict[collidedObj].headingPathNumber - 2].transform.position - collidedObj.transform.position).normalized;
        spawnedObj.transform.position = new Vector3(direction.x * 1.2f + collidedObj.transform.position.x,
                                                                    direction.y * 1.2f + collidedObj.transform.position.y, 0);

        spawnedObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        AddFoodBack(spawnedObj, collidedObj);
    }
    private void AddFoodBack(GameObject toAdd, GameObject Obj)
    {
        Food objFood = foodDict[Obj];
        Food toAddFood = foodDict[toAdd];

        Food prevFood = objFood.prevFood;

        toAddFood.nextFood = objFood;
        objFood.prevFood = toAddFood;

        if (Vector3.Dot(path[foodDict[Obj].headingPathNumber].transform.position - toAdd.transform.position,
                        path[foodDict[Obj].headingPathNumber - 1].transform.position - toAdd.transform.position) < 0)
            toAddFood.headingPathNumber = foodDict[Obj].headingPathNumber;
        else
            toAddFood.headingPathNumber = foodDict[Obj].headingPathNumber - 1;

        toAddFood.prevFood = prevFood;
        toAddFood.isAttachedToList = true;

        if(prevFood != null)
            prevFood.nextFood = toAddFood;
    }

    private void AddNext(GameObject spawnedObj, GameObject collidedObj)
    {
        Food spawnedFood = foodDict[collidedObj];
        while(spawnedFood.nextFood != null)
        {
            GameObject spObj = FindGameObj(spawnedFood.nextFood);
            Vector3 dir = (path[foodDict[spObj].headingPathNumber + 1].transform.position - spObj.transform.position).normalized;

            spObj.transform.position = new Vector3(dir.x * 1.2f + spObj.transform.position.x,
                                                                dir.y * 1.2f + spObj.transform.position.y, 0);

            spawnedFood.nextFood.headingPathNumber += 1;
            spawnedFood = spawnedFood.nextFood;
        }
        Vector3 direction = (path[foodDict[collidedObj].headingPathNumber + 1].transform.position - collidedObj.transform.position).normalized;
        spawnedObj.transform.position = new Vector3(direction.x * 1.2f + collidedObj.transform.position.x, 
                                                                    direction.y * 1.2f + collidedObj.transform.position.y, 0);

        spawnedObj.transform.rotation = Quaternion.Euler(0,0,0);
        AddFoodFront(spawnedObj, collidedObj);
    }


    public void AddFoodFront(GameObject toAdd, GameObject Obj)
    {
        Food objFood = foodDict[Obj];
        Food toAddFood = foodDict[toAdd];

        Food nextFood = objFood.nextFood;

        objFood.nextFood = toAddFood;
        toAddFood.prevFood = objFood;

        if (Vector3.Dot(path[foodDict[Obj].headingPathNumber].transform.position - Obj.transform.position, 
                        path[foodDict[Obj].headingPathNumber].transform.position - toAdd.transform.position) > 0)
            toAddFood.headingPathNumber = foodDict[Obj].headingPathNumber;
        else
            toAddFood.headingPathNumber = foodDict[Obj].headingPathNumber + 1;

        toAddFood.nextFood = nextFood;
        toAddFood.isAttachedToList = true;

        if (nextFood == null)
        {
            foodList.head = toAddFood;
        }
        else
            nextFood.prevFood = toAddFood;
    }

    private void printFoodList()
    {
        Food head = foodList.head;
        while(head != null)
        {
            Debug.Log(FindGameObj(head));
            head = head.prevFood;
        }
    }
}
