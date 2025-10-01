using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private EnvironmentCell[,] grid;
    private List<AnimalAgent> animals;
    private Drought drought;

    public float moisture;
    public float density;

    public SimulationController(int width, int height, int agents)
    {
        grid = new EnvironmentCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new EnvironmentCell(moisture, density);
            }
        }
    }

}
