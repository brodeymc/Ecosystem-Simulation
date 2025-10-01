using UnityEngine;

public class Drought : MonoBehaviour
{
    // severity of drought, 0-1
    private float droughtLevel;
    private float growthRate = 0.1f;
    private float carryingCapacity = 1f;

    public Drought(float level)
    {
        droughtLevel = level;
    }

    public void UpdateCell(EnvironmentCell cell)
    {
        cell.moisture -= .01f * droughtLevel;

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
        }
    }
}
