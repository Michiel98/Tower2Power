using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerARProjection : MonoBehaviour
{
    public GameObject playerChild;
    public GameObject redTower;
    public GameObject blueTower;

    [Header("red AR tower")]
    public GameObject redARTower;
    public GameObject red_2;
    public GameObject red_3;
    public GameObject red_4;


    [Header("blue AR tower")]
    public GameObject blueARTower;
    public GameObject blue_2;
    public GameObject blue_3;
    public GameObject blue_4;


    private string playerName;
    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        playerName = playerChild.transform.parent.gameObject.name;
        setTower();

    }

    public void Update()
    {
        playerName = playerChild.transform.parent.gameObject.name;
        if (playerName == "Red Team")
        {
            currentLevel = blueTower.GetComponent<Tower>().currentLevel;
            setLevelTower(currentLevel);
        }
        else if (playerName == "Blue Team")
        {
            currentLevel = redTower.GetComponent<Tower>().currentLevel;
            setLevelTower(currentLevel);
            
        }

    }


    void setTower()
    {
        if(playerName == "Red Team")
        {
            blueARTower.SetActive(true);
            redARTower.SetActive(false);
            blue_2.SetActive(false);
            blue_3.SetActive(false);
            blue_4.SetActive(false);

            //currentLevel = blueTower.currentLevel();
            //levelTower(blueTower, currentLevel);

        }else if(playerName == "Blue Team")
        {
            blueARTower.SetActive(false);
            redARTower.SetActive(true);
            red_2.SetActive(false);
            red_3.SetActive(false);
            red_4.SetActive(false);

            //currentLevel = GameObject.Find("Red Tower").GetComponent<Tower>().currentLevel;
            //levelRedTower(currentLevel);
        }

    }

    void setLevelTower(int level)
    {
        if (level == 1)
        {
            red_2.SetActive(true);

            blue_2.SetActive(true);

        }else if (level == 2){
            red_2.SetActive(true);
            red_3.SetActive(true);

            blue_2.SetActive(true);
            blue_3.SetActive(true);

        }else if (level == 3){
            red_2.SetActive(true);
            red_3.SetActive(true);
            red_4.SetActive(true);

            blue_2.SetActive(true);
            blue_3.SetActive(true);
            blue_4.SetActive(true);
        }
        
    }

}
