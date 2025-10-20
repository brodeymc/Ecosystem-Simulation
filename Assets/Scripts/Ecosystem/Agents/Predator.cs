using UnityEngine;

public class Predator : AnimalAgent
{
    // amount of energy gained on consumption
    public float consumptionGain = 0.3f;

    // distance the predator can see prey
    public float perceptionRadius = 2f;

    // chance of moving per timestep
    public float migrationRate = 0.25f;

    // base chance to reproduce
    public float reproductionChance = 0.05f;

    public void Hunt()
    {
        
    }

    public void Consume(EnvironmentCell[,] grid)
    {

    }

    public override bool IsDead()
    {
        return (energy <= 0);
    }

    public override void TimeStep()
    {
        Hunt();
 
        //Consume(controller.grid);
    }

    public override void Reproduce()
    {

    }
}
