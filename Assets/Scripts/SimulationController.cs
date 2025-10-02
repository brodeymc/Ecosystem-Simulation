using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public EnvironmentCell[,] grid;
    private Dictionary<(int, int), List<AnimalAgent>> animals;
    private Drought drought;

    // steps per second
    public float simulationSpeed = 1f;
    private float stepTimer = 0f;

    public float initialPrey = 5f;

    private int width;
    private int height;

    [Header("Drought Settings")]
    public float droughtChance = 0.01f;
    public int droughtMin = 1;
    public int droughtMax = 5;

    [Header("Visualization Settings")]
    public GameObject cellPrefab;
    public GameObject preyPrefab;
    public GameObject predatorPrefab;

    private GameObject[,] visGrid;
    private Dictionary<AnimalAgent, GameObject> visAnimals = new Dictionary<AnimalAgent, GameObject>();

    // avoids running when simulation is inactive
    private bool active = false;

    // initializes new simulation grid
    public void Initialize(int width, int height, float simSpeed, float droughtChance)
    {
        this.width = width;
        this.height = height;
        simulationSpeed = simSpeed;
        this.droughtChance = droughtChance;

        drought = new Drought();

        animals = new Dictionary<(int, int), List<AnimalAgent>>();
        grid = new EnvironmentCell[width, height];
        visGrid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float density = Random.value;
                float moisture = Random.value;
                grid[x, y] = new EnvironmentCell(moisture, density);

                Vector3 pos = new Vector3(x, y, 0);
                var cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                visGrid[x, y] = cellObj;
                UpdateCell(x, y);
            }
        }

        // random prey spawning
        for (int i = 0; i < initialPrey; i++)
        {
            int spawnX = Random.Range(0, width);
            int spawnY = Random.Range(0, height);

            Prey newPrey = new Prey();
            newPrey.energy = Random.Range(5f, 15f); // random starting energy
            newPrey.controller = this;
            newPrey.x = spawnX;
            newPrey.y = spawnY;
            NewAgent(newPrey, spawnX, spawnY);
        }

        active = true;
    }

    // adds new agent to the grid
    public void NewAgent(AnimalAgent agent, int x, int y)
    {
        agent.x = x;
        agent.y = y;

        var key = (x, y);
        if (!animals.ContainsKey(key))
        {
            animals[key] = new List<AnimalAgent>();
        }

        animals[key].Add(agent);

        // predator prefab to be added after predator is implemented
        GameObject prefab = preyPrefab;
        var visual = Instantiate(prefab, new Vector3(x, y, -1), Quaternion.identity, transform);
        visAnimals[agent] = visual;
    }

    // moves agent to new position
    public void MoveAgent(AnimalAgent agent, int x, int y)
    {
        var oldKey = (agent.x, agent.y);
        if (animals.ContainsKey(oldKey))
        {
            animals[oldKey].Remove(agent);
        }

        var newKey = (x, y);
        if (!animals.ContainsKey(newKey))
        {
            animals[newKey] = new List<AnimalAgent>();
        }

        animals[newKey].Add(agent);
        agent.x = x;
        agent.y = y;

        if (visAnimals.TryGetValue(agent, out GameObject visual))
        {
            visual.transform.position = new Vector3(x, y, -1);
        }
    }

    public void Remove(AnimalAgent agent)
    {
        var key = (agent.x, agent.y);
        if (animals.ContainsKey(key))
        {
            animals[key].Remove(agent);
            if (animals[key].Count == 0)
            {
                animals.Remove(key);
            }
        }

        if (visAnimals.TryGetValue(agent, out GameObject vis))
        {
            Destroy(vis);
            visAnimals.Remove(agent);
        }
    }

    // finds animals in cell
    public List<AnimalAgent> GetAnimals(int x, int y)
    {
        var key = (x, y);
        if (animals.ContainsKey(key))
        {
            return animals[key];
        }
        else return new List<AnimalAgent>();
    }

    void UpdateCell(int x, int y)
    {
        EnvironmentCell cell = grid[x, y];  
        var sr = visGrid[x, y].GetComponent<SpriteRenderer>();

        // controls cell color
        float hue = 0.33f; //green
        float saturation = Mathf.Clamp01(cell.density);
        float value = Mathf.Clamp01(cell.moisture);
        Color color = Color.HSVToRGB(hue, saturation, value);
        sr.color = color;
    }

    private void Update()
    {
        stepTimer += Time.deltaTime;
        float stepDuration = 1f / simulationSpeed;

        if (stepTimer >= stepDuration && active)
        {
            stepTimer = 0;
            Step();
        }
    }

    // controls simulation steps
    private void Step()
    {
        if (Random.value < droughtChance)
        {
            int level = Random.Range(droughtMin, droughtMax);
            drought.NewDrought(level);
            Debug.Log("Drought started, level " + level);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                drought.UpdateCell(grid[x, y]);
                UpdateCell(x, y);
            }
        }

        List<AnimalAgent> dead = new List<AnimalAgent>();

        foreach (var anim in animals)
        {
            foreach (var agent in anim.Value)
            {
                if (agent.IsDead())
                {
                    dead.Add(agent);
                }

                agent.TimeStep();
            }
        }

        foreach (var agent in dead)
        {
            Remove(agent);
        }
    }
}
