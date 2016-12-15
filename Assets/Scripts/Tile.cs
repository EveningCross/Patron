using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    public Sprite field;
    public Sprite forest;


    private int last_day = 0;

    private float cattle_growth = 0.0F;
    private int cattle_cap = 100000;
    private float cattle = 0;

    private int edible_plants_growth = 0;
    private int edible_plants_cap = 100000;
    private int edible_plants = 0;

    //agriculture
    private int cultivatable_land = 0;

    //crop key
    /*
     * 0. wheat
     * 
     * 
     * 
     * 
     */
    private float[] crop_fertility = new float[1]; 

	// Use this for initialization
	void Start () {
        cattle_growth = 0.50F;
        cattle = 100000.0F;
        edible_plants_growth = 1000;
        edible_plants = 100000;
        cultivatable_land = 5000;
        crop_fertility[0] = 1.0F;
	}

    public void manualUpdate(int total_days)
    {
        int elapsed_time =  total_days - last_day;
        if (elapsed_time > 0)
        {
            float previous_cattle = cattle;
            cattle = Mathf.Clamp((cattle * Mathf.Exp(cattle_growth * (((float) elapsed_time) / 300.0F))), 0, cattle_cap);
            //print(day + ". " + month + ". " + year + " " + cattle);

            float previous_edible_plants = edible_plants;
            edible_plants = Mathf.Clamp((edible_plants + edible_plants_growth * elapsed_time), 0, edible_plants_cap);

            
            last_day = total_days;
        }
    }

    public void changeSprite(int code)
    {
        switch (code)
        {
            case 1:
                GetComponent<SpriteRenderer>().sprite = forest;
                break;
        }
    }

    public int takeAnimals(int hunters, int x, int y)
    {
        int amount_taken = hunters * 1;
        if (amount_taken < cattle)
        {
            cattle -= amount_taken;
        }
        else
        {
            amount_taken = (int) cattle;
            cattle = 0;
        }
        //print(cattle);
        return amount_taken;
    }

    public int takeEdiblePlants(int gatherers, int x, int y)
    {
        int amount_taken = gatherers * 3;
        if (amount_taken < edible_plants)
        {
            edible_plants -= amount_taken;
        }
        else
        {
            amount_taken = edible_plants;
            edible_plants = 0;
        }
        return amount_taken;
    }

    //fields give a return once a month
    public int work_fields(int fertilizer, int workers, int crop_key)
    {
        return (int) (workers * crop_fertility[crop_key] * (1 + .2 * fertilizer / workers));
    }

    public int get_land()
    {
        return cultivatable_land;
    }
}
