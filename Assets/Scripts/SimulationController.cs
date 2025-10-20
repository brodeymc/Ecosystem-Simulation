using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public float prey = 100f;
    public float predator = 25f;
    // steps per second
    public float simulationSpeed = 1f;
    private float stepTimer = 0;

    public int initialPrey = 5;
    public int initialPredator = 5;

    [Header("Drought Settings")]
    public float droughtLevel = 0f;
    public float droughtChance = 0.01f;
    public float droughtDecay = 0.99f;


    [Header("Lotka-Volterra Parameters")]
    [Tooltip("Prey birth rate")]
    public float alpha = 0.1f; // prey reproduction rate
    [Tooltip("Predation rate")]
    public float beta = 0.005f; // predation rate
    [Tooltip("Predator reproduction rate")]
    public float delta = 0.005f; // predator reproduction rate
    [Tooltip("Predator death rate")]
    public float gamma = 0.02f; // predator death rate

    public float preyCarryingCapacity = 200f;

    [Header("Resource Settings")]
    public float resources = 100f;
    public float maxResource = 100f;
    public float resourceGrowthRate = 1f;
    public float resourceConsumptionRate = 0.25f;

    // avoids running when simulation is inactive
    private bool active = false;

    private void Update()
    {
       // if (!active) return;

        stepTimer += Time.deltaTime * simulationSpeed; // accumulate fraction of steps

        while (stepTimer >= 1f)
        {
            Step();
            stepTimer -= 1f; // subtract one "step"
        }
    }

    private void Step()
    {
        float dt = 1f / simulationSpeed;

        if (Random.value < droughtChance)
        {
            float newSeverity = Random.Range(0.2f, 1f);

            // only changes drought severity if new drought is more severe
            droughtLevel = Mathf.Max(droughtLevel, newSeverity);
        }
        else
        {
            droughtLevel *= droughtDecay;
        }

        float resourceGrowth = resourceGrowthRate * Mathf.Lerp(1f, 0.2f, droughtLevel);
        resources += resourceGrowth * (1f - resources / maxResource);
        float consumption = prey * resourceConsumptionRate * dt;
        resources -= consumption;
        resources = Mathf.Max(resources, 0f);

        float resourceFactor = Mathf.Clamp01(resources / maxResource);
        float carryingCapacity = preyCarryingCapacity * resourceFactor;

        float logisticGrowth = alpha * prey * (1f - prey / carryingCapacity);

        float dPrey = (logisticGrowth - beta * prey * predator) * dt;
        float dPred = (delta * prey * predator - gamma * predator) * dt;

        prey += dPrey;
        predator += dPred;

        prey = Mathf.Max(prey, 0f);
        predator = Mathf.Max(predator, 0f);

        Debug.Log($"Prey: {prey:F2}, Predator: {predator:F2}, Resource: {resources:F2}, Drought: {droughtLevel:F2}");
    }
}
