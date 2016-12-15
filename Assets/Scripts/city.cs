using UnityEngine;
using System.Collections;
using System;
public class city : MonoBehaviour {




    public GameObject[,] tiles;

    private int last_day = 0;

    private int x = 0;
    private int y = 0;
    private int pop_cap = 100000;
    private float growth_rate = 0.0F;

    private int labor_pool = 0;

    private int unemployed_index = 0;
    private int wheat_farmer_index = 1;
    private int baker_index = 2;


    /*
 * index
 * 0 = wheat farmer
 * 1 = baker
*/

    private struct job
    {
        //population
        public int workers;
        //money
        public int wealth;
        // amount of owned food
        public int food;
        // how much a worker is able to make relative to normal levels
        public float efficiency;
        // satisfaction with current status
        public float happiness;
        //index of the goods needed for production
        public int production_index1;
        public int production_index2;
        public int production_index3;
        // stockpiled resources for production
        public int production_item1;
        public int production_item2;
        public int production_item3;
        public int index;
    }



    //these are market stockpiles, not personal belongings
    private struct item
    {
        // amount on the market for purchase
        public int stock;
        // the current market value
        public int price;
        // these 2 are used to determine pricing
        // how much of the good was sold in the cycle
        public int sold;
        // how much was produced in the cycle
        public int produced;

        // how fast the good is consumed, 
        //100% means 1 person destroys 1 item on use
        //50% means 2 people destroy 1 item on use
        public float decay;
        // the index of which profession produced the good, used to deliver payment
        public int job_index;
    }

    static int number_of_professions = 3;

    private job[] jobs = new job[number_of_professions];
    private int[] payment = new int[number_of_professions];
    private item[] food = new item[1];
    private item[] goods = new item[1];


    //GUI
    private string day_text;


    void Start () {
        growth_rate = 0.03F;

        // unemployment pool
        jobs[unemployed_index] = new job()
        {
            workers = 0,
            wealth = 0,
            food = 100,
            efficiency = 0F,
            happiness = 1F,
            production_index1 = -1,
            production_index2 = -1,
            production_index3 = -1,
            production_item1 = 0,
            production_item2 = 0,
            production_item3 = 0,
            index = unemployed_index
        };

        // wheat farmer
        jobs[wheat_farmer_index] = new job() {
            workers = 100,
            wealth = 10000,
            food = 100,
            efficiency = 1,
            happiness = .5F,
            production_index1 = -1,
            production_index2 = -1,
            production_index3 = -1,
            production_item1 = 0,
            production_item2 = 0,
            production_item3 = 0,
            index = wheat_farmer_index };

        // bakers
        jobs[baker_index] = new job() {
            workers = 100,
            wealth = 10000,
            food = 100,
            efficiency = 2F,
            happiness = .5F,
            production_index1 = 0,
            production_index2 = -1,
            production_index3 = -1,
            production_item1 = 0,
            production_item2 = 0,
            production_item3 = 0,
            index = baker_index };



        Array.Sort(jobs, (a, b) => a.index.CompareTo(b.index));

        //bread
        food[0] = new item() {
            stock = 0,
            price = 10,
            sold = 0,
            produced = 0,
            decay = 1,
            job_index = baker_index };

        //wheat
        goods[0] = new item() {
            stock = 0,
            price = 5,
            sold = 0,
            produced = 0,
            decay = 1,
            job_index = wheat_farmer_index };
    }





    public void manualUpdate(int total_days)
    {
        int elapsed_time = total_days - last_day;

        for (int i = 0; i < elapsed_time; i++)
        {
 

            //generate raw resources from the land
            gatherResources();
            //sort the jobs from richest to poorest so they rich spend money first
            Array.Sort(jobs, (a, b) => b.wealth.CompareTo(a.wealth));
            //cycle through buying food
            purchaseFood();
            //cycle through purchasing production resources
            purchaseSupplies();
            //purchase luxuries
            //TODO
            //switch to index to distribute payment
            Array.Sort(jobs, (a, b) => a.index.CompareTo(b.index));
            // distribute payment from purchases
            distributePayment();
            //adjust prices based on sales
            setPrices();
            //use the stuff purchased
            consumeGoods();
            //leave jobs
            leaveJobs();
            //join jobs
            joinJobs();



            //foreign purchases and market wealth
            //TODO

            //and the tariffs and taxes
            //TODO

            last_day = total_days;
            
            day_text = last_day.ToString();
        }
    }

    public void setTileReferenece(GameObject[,] tile_reference)
    {
        tiles = tile_reference;
    }	
    public void setLocation(int x_coord, int y_coord)
    {
        x = x_coord;
        y = y_coord;
    }
    

    private void gatherResources()
    {
        growWheat();
        bakeBread();
    }

    private void growWheat()
    {
        goods[0].produced = tiles[x, y].GetComponent<Tile>().work_fields(0, jobs[1].workers, 0);
        goods[0].stock += goods[0].produced;
    }

    private void bakeBread()
    {
        if (jobs[2].workers < jobs[2].production_item1)
        {
            jobs[2].production_item1 -= (jobs[2].workers);
            food[0].stock += (int) ((jobs[2].workers) * jobs[2].efficiency);
            food[0].produced = (int)((jobs[2].workers) * jobs[2].efficiency);
        }
        else
        {
            food[0].stock += (int) (jobs[2].production_item1 * jobs[2].efficiency);
            food[0].produced = (int)(jobs[2].production_item1 * jobs[2].efficiency);
            jobs[2].production_item1 = 0;
        }
    }

    private void purchaseFood()
    {
        for (int i = 0; i < jobs.Length; i++)
        {
            if (jobs[i].food < jobs[i].workers * 3)
            {
                int desired_food = jobs[i].workers * 3 - jobs[i].food;
                int total_purchasable_amount = jobs[i].wealth / food[0].price;
                int purchase_amount = Mathf.Min(Mathf.Min(desired_food, total_purchasable_amount), food[0].stock);
               

                food[0].stock -= purchase_amount;
                food[0].sold += purchase_amount;
                jobs[i].food += purchase_amount;
                jobs[i].wealth -= purchase_amount * food[0].price;
                payment[food[0].job_index] += purchase_amount * food[0].price;
            }
        }
    }

    private void purchaseSupplies()
    {
        for (int i = 0; i < jobs.Length; i++)
        {

            if (jobs[i].production_index1 != -1 && jobs[i].production_item1 < jobs[i].workers * 3)
            {
                int desired_stock = jobs[i].workers * 3 - jobs[i].production_item1;
                int total_purchasable_amount = jobs[i].wealth / goods[jobs[i].production_index1].price;
                int purchase_amount = Mathf.Min(Mathf.Min(desired_stock, total_purchasable_amount), goods[jobs[i].production_index1].stock);
                goods[jobs[i].production_index1].stock -= purchase_amount;
                goods[jobs[i].production_index1].sold += purchase_amount;
                jobs[i].production_item1 += purchase_amount;
                jobs[i].wealth -= purchase_amount * goods[jobs[i].production_index1].price;
                payment[goods[jobs[i].production_index1].job_index] += purchase_amount * goods[jobs[i].production_index1].price;
            }
            if (jobs[i].production_index2 != -1 && jobs[i].production_item2 < jobs[i].workers * 3)
            {
                int desired_stock = jobs[i].workers * 3 - jobs[i].production_item2;
                int total_purchasable_amount = jobs[i].wealth / goods[jobs[i].production_index2].price;
                int purchase_amount = Mathf.Min(Mathf.Min(desired_stock, total_purchasable_amount), goods[jobs[i].production_index2].stock);
                goods[jobs[i].production_index2].stock -= purchase_amount;
                goods[jobs[i].production_index2].sold += purchase_amount;
                jobs[i].production_item2 += purchase_amount;
                jobs[i].wealth -= purchase_amount * goods[jobs[i].production_index2].price;
                payment[goods[jobs[i].production_index2].job_index] += purchase_amount * goods[jobs[i].production_index2].price;
            }
            if (jobs[i].production_index3 != -1 && jobs[i].production_item3 < jobs[i].workers * 3)
            {
                int desired_stock = jobs[i].workers * 3 - jobs[i].production_item3;
                int total_purchasable_amount = jobs[i].wealth / goods[jobs[i].production_index3].price;
                int purchase_amount = Mathf.Min(Mathf.Min(desired_stock, total_purchasable_amount), goods[jobs[i].production_index3].stock);
                goods[jobs[i].production_index3].stock -= purchase_amount;
                goods[jobs[i].production_index3].sold += purchase_amount;
                jobs[i].production_item3 += purchase_amount;
                jobs[i].wealth -= purchase_amount * goods[jobs[i].production_index3].price;
                payment[goods[jobs[i].production_index3].job_index] += purchase_amount * goods[jobs[i].production_index3].price;
            }
        }
    }

    private void distributePayment()
    {
        for (int i = 0; i < jobs.Length; i++)
        {
            jobs[i].wealth += payment[i];
            payment[i] = 0;
        }
    }

    private void setPrices()
    {
        if (food[0].sold >= food[0].produced)
        {
            food[0].price = (int) (1.01 * food[0].price) + 1;
            food[0].sold = 0;
        }
        else
        {
            food[0].price = Mathf.Max((int)(.99 * food[0].price) - 1, 1) ;
            food[0].sold = 0;
        }

        if (goods[0].sold >= goods[0].produced)
        {
            goods[0].price = (int)(1.01 * goods[0].price) + 1;
            goods[0].sold = 0;
        }
        else
        {
            goods[0].price = Mathf.Max((int)(.99 * goods[0].price) - 1, 1);
            goods[0].sold = 0;
        }
    }

    private void consumeGoods()
    {
        // happiness will be determined here
        //fix
        for (int i = 0; i < jobs.Length; i++)
        {
            if (jobs[i].workers <= jobs[i].food)
            {
                jobs[i].food -= jobs[i].workers;
            }
            else
            {
                jobs[i].food = 0;
                //fix add penalty
            }
        }
    }

    private void leaveJobs()
    {
        //switching jobs is dependent on happiness
        //jobs with less happiness are more liable to switching jobs
        //fix
        for (int i = 1; i < jobs.Length; i++)
        {
            int people_quitting = (int)(jobs[i].workers * (1-jobs[i].happiness));
            int wealth_leaving = (int)(jobs[i].wealth * (1-jobs[i].happiness));
            jobs[i].workers -= people_quitting;
            jobs[i].wealth -= wealth_leaving;
            jobs[0].workers += people_quitting;
            jobs[0].wealth += wealth_leaving;
        }
    }

    private void joinJobs()
    {
        //FIX
        // unemployed should only join professions actually happier than
        // the unemployment pool
        int labor_distribution = jobs[0].workers / jobs.Length;
        int wealth_distribution = jobs[0].wealth / jobs.Length;
        for (int i = 1; i < jobs.Length; i++)
        {
            jobs[0].workers -= labor_distribution;
            jobs[0].wealth -= wealth_distribution;
            jobs[i].workers += labor_distribution;
            jobs[i].wealth += wealth_distribution;
        }
    }

    void OnGUI()
    {
        int space = 25;
        GUI.Label(new Rect(10, space, 200, 20),  "days:             " + day_text);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20),  "wheat farmers:    " + jobs[1].workers);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20),  "wheat wealth:     " + jobs[1].wealth);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20),  "wheat:            " + goods[0].stock);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20),  "wheat price:      " + goods[0].price);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "wheat produced:   " + goods[0].produced);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "bakers:           " + jobs[2].workers);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "bakers wealth:    " + jobs[2].wealth);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "bread:            " + food[0].stock);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "bread price:      " + food[0].price);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "bread produced:   " + food[0].produced);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "wheat stock:      " + jobs[2].production_item1);
        space += 15;
        GUI.Label(new Rect(10, space, 200, 20), "unemployed:       " + jobs[0].workers);
        //if (GUI.Button(new Rect(10, 20, 150, 100), "I am a button"))
        //    print("You clicked the button!");
    }
}
