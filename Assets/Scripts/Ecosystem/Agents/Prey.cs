using System.Collections.Generic;
using UnityEngine;

public class Prey : AnimalAgent
{
    // amount of food to be eaten per timestep
    public float consumptionRate = 0.1f;

    public float migrationRate = .5f;

    // how far prey looks for migration
    public int perceptionRadius = 1;

    // migrates to cell with highest density nearby
    public void Migrate(EnvironmentCell[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int currX = (int)position.x;
        int currY = (int)position.y;

        List<(int x, int y)> possibleMoves = new List<(int x, int y)>();

        float maxDensity = grid[currX, currY].density;
        (int bestX, int bestY) = (currX, currY);

        for (int i = -perceptionRadius; i <= perceptionRadius; i++)
        {
            for (int k = -perceptionRadius; k <= perceptionRadius; k++)
            {
                int newX = currX + i;
                int newY = currY + k;

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    float density = grid[newX, newY].density;
                    if (density > maxDensity)
                    {
                        maxDensity = density;
                        bestX = newX;
                        bestY = newY;
                    }
                }
            }
        }

        if ((bestX != currX || bestY != currY) && Random.value < migrationRate)
        {
            Move(bestX, bestY);
        }
    }

    public void Consume(EnvironmentCell[,] grid)
    {
        int x = (int)position.x;
        int y = (int)position.y;
        EnvironmentCell cell = grid[x, y];

        float eaten = Mathf.Min(cell.density, consumptionRate);
        cell.density -= eaten;
        energy += eaten;

        // sets cap on energy
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    public override bool IsDead()
    {
        return (energy <= 0);
    }

    public override void TimeStep()
    {
        int currX = (int)position.x;
        int currY = (int)position.y;
        EnvironmentCell cell = controller.grid[currX, currY];

        float migrationChance = migrationRate * (1f - Mathf.Clamp01(cell.density));
        if (Random.value < migrationChance) { Migrate(controller.grid); }
        
        Consume(controller.grid);
        Reproduce();
    }

    public override void Reproduce()
    {
        if (Random.value < controller.alpha)
        {
            energy /= 2;

            controller.SpawnPrey(x, y);
        }
    }
}
