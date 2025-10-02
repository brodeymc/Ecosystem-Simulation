using UnityEngine;

public class Drought
{
    // severity of drought, 0-1
    private float droughtLevel = 0f;
    private float growthRate = 0.1f;
    private float decayRate = 0.02f;
    private float carryingCapacity = 1f;

    public void NewDrought(float level)
    {
        droughtLevel = level;
    }

    public void UpdateCell(EnvironmentCell cell)
    {
        cell.moisture -= .01f * droughtLevel;

        if (droughtLevel == 0f)
        {
            cell.moisture += .1f;
            if (cell.moisture > 1f) cell.moisture = 1f;
            return;
        }

        if (cell.moisture < 0f)
        {
            cell.moisture = 0f;
        }

        // limits growth to soil with >20% moisture
        if (cell.moisture > .2f)
        {
            float growth = growthRate * cell.density * (1 - cell.density / carryingCapacity);
            cell.density += growth;
        }
        else
        {
            cell.density -= .01f;
            if (cell.density < 0f) cell.density = 0f;
        }

        droughtLevel -= decayRate;
        if (droughtLevel < 0f) droughtLevel = 0f;
    }
}
