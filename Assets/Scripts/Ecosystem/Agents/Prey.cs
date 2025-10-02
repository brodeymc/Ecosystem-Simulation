using UnityEngine;

public class Prey : AnimalAgent
{
    // amount of food to be eaten per timestep
    public float consumptionRate = 0.1f;

    public float migrationRate = 0.25f;

    public void Migrate(EnvironmentCell[,] grid)
    {
        // random movement for now
        int x = Random.Range(0, grid.GetLength(0));
        int y = Random.Range(0, grid.GetLength(1));
        Move(x, y);
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
        if (IsDead()) { controller.Remove(this); }

        if (Random.Range(0, 1) < migrationRate) { Migrate(controller.grid); }
        
        Consume(controller.grid);
    }
}
