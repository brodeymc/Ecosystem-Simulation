using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Header("Simulation Settings")]
    public float prey = 100f;
    public float predator = 25f;
    // steps per second
    private float timeStep = 0.1f;
    private int simulationTime = 0;
    public float extinctionThreshold = 0.1f;
    public float maxSteps = 10000;

    [Header("Drought Settings")]
    public float droughtLevel = 0f;
    public float droughtChance = 0.01f;
    public float droughtDecay = 0.99f;

    [Header("Lotka-Volterra Parameters")]
    [Tooltip("Prey birth rate")]
    public float alpha = 0.1f; // prey reproduction rate
    [Tooltip("Predation rate")]
    public float beta = 0.001f; // predation rate
    [Tooltip("Predator reproduction rate")]
    public float delta = 0.005f; // predator reproduction rate
    [Tooltip("Predator death rate")]
    public float gamma = 0.02f; // predator death rate

    public float preyCarryingCapacity = 200f;

    [Header("Resource Settings")]
    public float resources = 100f;
    public float maxResource = 100f;
    public float resourceGrowthRate = 0.1f;
    public float resourceConsumptionRate = 0.2f;

    [Header("Data")]
    public string runID = "run_001";
    public int randomSeed = 0;
    private string runFolder;
    private StreamWriter csvWriter;
    private Stopwatch stopwatch;
    private float preySum = 0f;
    private float resourceSum = 0f;
    private float predatorSum = 0f;
    private float droughtSum = 0f;

    // avoids running when simulation is inactive
    private bool active = false;

    private string baseFolder;

    public void StartSim()
    {
        if (active) return;

        //creates files for new runs
        #if UNITY_EDITOR
        baseFolder = Path.Combine(Application.dataPath, "SimulationRuns");
        #else
        baseFolder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "SimulationRuns");
        #endif
        Directory.CreateDirectory(baseFolder);

        Setup();
        active = true;
    }

    private void Update()
    {
        if (!active) return;

        // simulation ends when prey & predator are extinct or after set time
        if ((prey <= extinctionThreshold && predator <= extinctionThreshold) || simulationTime >= maxSteps)
        {
            EndRun();
            return;
        }

        Step();
    }

    private void Step()
    {
        float dt = timeStep;
        simulationTime += 1;

        if (Random.value < droughtChance)
        {
            droughtLevel = Mathf.Max(droughtLevel, Random.Range(0.1f, 0.99f));
        }
        else
        {
            droughtLevel *= droughtDecay;
        }

        // drought reduces resource growht rate as well as carrying capacity
        float currGrowthRate = resourceGrowthRate * (1f - droughtLevel);
        float currMaxResource = maxResource * (1f - droughtLevel);

        resources += (currGrowthRate * resources * (1f - resources / currMaxResource)
             - (resourceConsumptionRate * prey)) * dt;

        // drought decreases prey birth rate and increases predator death rate
        float preyBirthRate = alpha * (1f - droughtLevel);
        float predDeathRate = gamma * (1f + droughtLevel);

        float resourceFactor = Mathf.Clamp01(resources / currMaxResource);
        float carryingCapacity = preyCarryingCapacity * resourceFactor;

        float logisticGrowth = preyBirthRate * prey * (1f - prey / carryingCapacity);

        float dPrey = (logisticGrowth - beta * prey * predator) * dt;
        float dPred = (delta * prey * predator - predDeathRate * predator) * dt;

        prey += dPrey;
        predator += dPred;

        prey = Mathf.Max(prey, 0f);
        predator = Mathf.Max(predator, 0f);

        float memoryMB = System.GC.GetTotalMemory(false) / (1024f * 1024f);
        csvWriter.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F3},{simulationTime},{prey:F2},{predator:F2},{resources:F2},{droughtLevel:F2},{memoryMB:F2}");
        preySum += prey;
        predatorSum += predator;
        resourceSum += resources;
        droughtSum += droughtLevel;
    }

    private void Setup()
    {
        // seed value for randomization
        UnityEngine.Random.InitState(randomSeed);

        runFolder = Path.Combine(baseFolder, runID);

        Directory.CreateDirectory(runFolder);

        string configFilePath = Path.Combine(runFolder, $"{runID}_config.json");
        string configFile = JsonUtility.ToJson(this, true);
        File.WriteAllText(configFilePath, configFile);

        string csvPath = Path.Combine(runFolder, $"{runID}_timeseries.csv");
        csvWriter = new StreamWriter(csvPath);
        csvWriter.WriteLine("timestamp,simTime,prey,predator,resources,drought,memoryUsedMB");

        // start stopwatch
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    // serializable class for summary file
    [System.Serializable]
    public class SimSummary
    {
        public string runID;
        public float time;
        public float avgPrey;
        public float avgPred;
        public float avgResource;
        public float avgDroughtLevel;
        public int randomSeed;
        //performance stats
        public long memoryUsedBytes;
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

        long memoryUsed = System.GC.GetTotalMemory(false);

        var summary = new SimSummary
        {
            runID = runID,
            time = (float)stopwatch.Elapsed.TotalSeconds,
            avgPrey = preySum / simulationTime,
            avgPred = predatorSum / simulationTime,
            avgResource = resourceSum / simulationTime,
            avgDroughtLevel = droughtSum / simulationTime,
            randomSeed = randomSeed,
            memoryUsedBytes = memoryUsed
        };

        File.WriteAllText(Path.Combine(runFolder, "summary.json"), JsonUtility.ToJson(summary, true));
        string indexPath = Path.Combine(baseFolder, "index.csv");
        bool writeHeader = !File.Exists(indexPath);
        using (var writer = new StreamWriter(indexPath, append: true))
        {
            if (writeHeader)
                writer.WriteLine("runID,realTime,avgPrey,avgPredator,avgResources,avgDrought,randomSeed,memoryUsedBytes");

            writer.WriteLine
                ($"{runID},{summary.time:F2},{summary.avgPrey:F2}," +
                $"{summary.avgPred:F2},{summary.avgResource:F2},{summary.avgDroughtLevel:F2}," +
                $"{summary.randomSeed},{summary.memoryUsedBytes}");
        }

        Destroy(gameObject);
    }
}