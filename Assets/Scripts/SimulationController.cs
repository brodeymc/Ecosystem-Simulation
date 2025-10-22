using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public float prey = 100f;
    public float predator = 25f;
    // steps per second
    public float simulationSpeed = 1f;
    private float stepTimer = 0;
    private float simulationTime = 0f;
    public float duration = 60f;

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
    public float resourceGrowthRate = 0.1f;
    public float resourceConsumptionRate = 0.02f;

    [Header("Data")]
    public string runID = "run_001";
    public int randomSeed = 0;
    private string runFolder;
    private StreamWriter csvWriter;
    private Stopwatch stopwatch;

    // avoids running when simulation is inactive
    private bool active = false;

    private string baseFolder;

    public void StartSim()
    {
        if (active) return;

        //creates files for new runs
        baseFolder = Path.Combine(Application.dataPath, "SimulationRuns");
        Directory.CreateDirectory(baseFolder);

        Setup();
        active = true;
    }

    private void Update()
    {
       if (!active) return;

       if (simulationTime >= duration)
       {
            EndRun();
            active = false;
            return;
       }

        stepTimer += Time.deltaTime * simulationSpeed;
        simulationTime += Time.deltaTime;

        while (stepTimer >= 1f)
        {
            Step();
            stepTimer -= 1f; 
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

        csvWriter.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F2},{simulationTime:F2},{prey:F2},{predator:F2},{resources:F2},{droughtLevel:F2}");
    }

    private void Setup()
    {
        // seed value for randomization
        UnityEngine.Random.InitState(randomSeed);

        runFolder = Path.Combine(baseFolder, runID);

        Directory.CreateDirectory(runFolder);

        string configFile = JsonUtility.ToJson(this, this);
        File.WriteAllText(Path.Combine(runFolder, "config.json"), configFile);

        string csvPath = Path.Combine(runFolder, "timeseries.csv");
        csvWriter = new StreamWriter(csvPath);
        csvWriter.WriteLine("timestamp,simTime,prey,predator,resources,drought");

        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    // serializable class for summary file
    [System.Serializable]
    public class SimSummary
    {
        public string runID;
        public float time;
        public float duration;
        public float finalPrey;
        public float finalPred;
        public float finalResource;
        public float droughtLevel;
    }

    // collects data into files and destroys gameObject
    public void EndRun()
    {
        if (!active) return;

        active = false;
        stopwatch.Stop();

        csvWriter?.Flush();
        csvWriter?.Close();
        csvWriter?.Dispose();
        csvWriter = null;

        var summary = new SimSummary
        {
            runID = runID,
            time = (float)stopwatch.Elapsed.TotalSeconds,
            duration = duration,
            finalPrey = prey,
            finalPred = predator,
            finalResource = resources,
            droughtLevel = droughtLevel
        };

        File.WriteAllText(Path.Combine(runFolder, "summary.json"), JsonUtility.ToJson(summary, true));
        string indexPath = Path.Combine(baseFolder, "index.csv");
        bool writeHeader = !File.Exists(indexPath);
        using (var writer = new StreamWriter(indexPath, append: true))
        {
            if (writeHeader)
                writer.WriteLine("runID,realTime,simDuration,finalPrey,finalPredator,finalResources,drought");

            writer.WriteLine($"{runID},{summary.time:F2},{summary.duration:F2},{summary.finalPrey:F2},{summary.finalPred:F2},{summary.finalResource:F2},{summary.droughtLevel:F2}");
        }

        Destroy(gameObject);
    }
}
