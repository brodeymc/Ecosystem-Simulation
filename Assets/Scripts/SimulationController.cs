using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public EnvironmentCell[,] grid;
    private Dictionary<(int, int), List<AnimalAgent>> animals;
    private Drought drought;

    public float moisture;
    public float density;

    // steps per second
    public float simulationSpeed = 1f;
    private float stepTimer = 0f;

    private int width;
    private int height;

    [Header("Visualization Settings")]
    public GameObject cellPrefab;
    public GameObject preyPrefab;
    public GameObject predatorPrefab;

    private GameObject[,] visGrid;
    private Dictionary<AnimalAgent, GameObject> visAnimals = new Dictionary<AnimalAgent, GameObject>();

    // initializes new simulation grid
    public void Initialize(int width, int height, int agents)
    {
        this.width = width;
        this.height = height;

        animals = new Dictionary<(int, int), List<AnimalAgent>>();
        grid = new EnvironmentCell[width, height];
        visGrid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new EnvironmentCell(moisture, density);

                Vector3 pos = new Vector3(x, y, 0);
                var cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                visGrid[x, y] = cellObj;
                UpdateCell(x, y);
            }
        }
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

        sr.color = new Color(1f, -cell.density, 1f, 1f - cell.moisture);
    }

    private void Update()
    {
        stepTimer += Time.deltaTime;
        float stepDuration = 1f / simulationSpeed;

        if (stepTimer >= simulationSpeed)
        {
            stepTimer = 0;
            Step();
        }
    }

    // controls simulation steps
    private void Step()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                drought?.UpdateCell(grid[x, y]);
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
