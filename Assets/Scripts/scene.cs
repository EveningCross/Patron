using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class scene : MonoBehaviour
{

    public Camera camera;
    public GameObject city;
    public GameObject field;
    public GameObject cursor;
    public InputField daysInput;

    private int total_days = 0;
    private int day = 1;
    private int month = 1;
    private int year = 0;

    int days_to_advance = 0;

    private GameObject[,] tiles = new GameObject[10, 10];
    private GameObject[] cities = new GameObject[1];
    private GameObject pointer;
    // Use this for initialization
    void Start()
    {
        pointer = Instantiate(cursor, new Vector3(0.5F, 0.5F, 0), Quaternion.identity) as GameObject;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                tiles[i, j] = Instantiate(field, new Vector3(i + 0.5F, j + 0.5F, 1), Quaternion.identity) as GameObject;
            }
        }

        cities[0] = Instantiate(city, new Vector3(5.5F, 5.5F, 0), Quaternion.identity) as GameObject;
        cities[0].GetComponent<city>().setLocation(5, 5);
        cities[0].GetComponent<city>().setTileReferenece(tiles);
    }

    // Update is called once per frame
    void Update()
    {
        mouseActions();
        cities[0].GetComponent<city>().manualUpdate(total_days);
    }

    void mouseActions()
    {
        Vector3 vec = Input.mousePosition;
        Vector3 p = camera.ScreenToWorldPoint(new Vector3(vec.x, vec.y, camera.nearClipPlane));
        pointer.GetComponent<cursor>().setPosition((int)Mathf.Floor(p.x), (int)Mathf.Floor(p.y));

        if (Input.GetMouseButtonDown(0))
        {

            if ((int)Mathf.Floor(p.x) >= 0 && (int)Mathf.Floor(p.x) <= 9 && (int)Mathf.Floor(p.y) >= 0 && (int)Mathf.Floor(p.y) <= 9)
            {
                print((int)Mathf.Floor(p.x) + " " + (int)Mathf.Floor(p.y));
                tiles[(int)Mathf.Floor(p.x), (int)Mathf.Floor(p.y)].GetComponent<Tile>().changeSprite(1);
                tiles[(int)Mathf.Floor(p.x), (int)Mathf.Floor(p.y)].GetComponent<Tile>().manualUpdate(total_days);
            }
        }

        if (Input.GetMouseButtonDown(1))
            print("Pressed right click.");

        if (Input.GetMouseButtonDown(2))
            print("Pressed middle click.");
    }

    public void daysToAdvance(string days)
    {
        Int32.TryParse(days, out days_to_advance);
        Debug.Log(days_to_advance);
    }

    public void updateTime()
    {
        Debug.Log(days_to_advance);
        total_days += days_to_advance;
    }

    public int getDay()
    {
        return day;
    }

    public int getMonth()
    {
        return month;
    }

    public int getYear()
    {
        return year;
    }


}
