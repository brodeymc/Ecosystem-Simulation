using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public EnvironmentCell[,] grid;
    private Dictionary<(int, int), List<AnimalAgent>> animals;
    private Drought drought;

    public float moisture;
    public float density;

    private int width;
    private int height;

    // initializes new simulation grid
    public void Initialize(int width, int height, int agents)
    {
        this.width = width;
        this.height = height;

        grid = new EnvironmentCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new EnvironmentCell(moisture, density);
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
}
