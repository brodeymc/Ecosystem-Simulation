using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInputs : MonoBehaviour
{
    public SimulationController simulationController;
    private SimulationController currentSim;

    [Header("Inputs")]
    public TMP_InputField alphaInput;
    public TMP_InputField betaInput;
    public TMP_InputField deltaInput;
    public TMP_InputField gammaInput;
    public TMP_InputField droughtChanceInput;
    public TMP_InputField runIDInput;
    public TMP_InputField preyCountInput;
    public TMP_InputField predCountInput;
    public TMP_InputField resourceCountInput;
    public TMP_InputField preyCarryingCapacityInput;
    public TMP_InputField randomSeedInput;

    public Button startButton;
    public Button stopButton;

    [Header("Text")]
    public TMP_Text status;
    public TMP_Text predCountText;
    public TMP_Text preyCountText;
    public TMP_Text resourcesCountText;
    public TMP_Text droughtSeverityText;
    public TMP_Text warningText;

    private void Start()
    {
        startButton.onClick.AddListener(StartSim);
        stopButton.onClick.AddListener(Stop);

        alphaInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        betaInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        deltaInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        gammaInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        droughtChanceInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        preyCountInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        predCountInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        resourceCountInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        preyCarryingCapacityInput.onValueChanged.AddListener(delegate { ValidateInputs(); });
        randomSeedInput.onValueChanged.AddListener(delegate { ValidateInputs(); });

        ValidateInputs();
    }

    // limits inputs to valid values
    private void ValidateInputs()
    {
        warningText.text = "";
        startButton.interactable = true;

        if (!float.TryParse(alphaInput.text, out float alpha) || alpha <= 0)
            SetInvalid("Alpha must be a positive number");

        else if (!float.TryParse(betaInput.text, out float beta) || beta <= 0)
            SetInvalid("Beta must be a positive number");

        else if (!float.TryParse(deltaInput.text, out float delta) || delta <= 0)
            SetInvalid("Delta must be a positive number");

        else if (!float.TryParse(gammaInput.text, out float gamma) || gamma <= 0)
            SetInvalid("Gamma must be a positive number");

        else if (!float.TryParse(droughtChanceInput.text, out float droughtChance) || droughtChance < 0 || droughtChance > 1)
            SetInvalid("Drought chance must be between 0 and 1");

        else if (!float.TryParse(preyCountInput.text, out float preyCount) || preyCount <= 0)
            SetInvalid("Initial prey count must be positive");

        else if (!float.TryParse(predCountInput.text, out float predCount) || predCount < 0)
            SetInvalid("Initial predator count cannot be negative");

        else if (!float.TryParse(resourceCountInput.text, out float resourceCount) || resourceCount <= 0)
            SetInvalid("Initial resource amount must be positive");

        else if (!float.TryParse(preyCarryingCapacityInput.text, out float preyCap) || preyCap <= 0)
            SetInvalid("Prey carrying capacity must be positive");

        else if (string.IsNullOrWhiteSpace(runIDInput.text))
            SetInvalid("Run ID cannot be empty");

        else if (!int.TryParse(preyCarryingCapacityInput.text, out int seed) || seed <= 0)
            SetInvalid("Seed must be a positive integer");
    }

    // doesnt allow simulation to start without valid input
    private void SetInvalid(string message)
    {
        startButton.interactable = false;
        warningText.text = message;
    }

    // starts simulation on button press & reads values from unity inputs
    private void StartSim()
    {
        if (currentSim == null)
        {
            currentSim = Instantiate(simulationController);
            if (float.TryParse(alphaInput.text, out float alpha)) currentSim.alpha = alpha;
            if (float.TryParse(betaInput.text, out float beta)) currentSim.beta = beta;
            if (float.TryParse(deltaInput.text, out float delta)) currentSim.delta = delta;
            if (float.TryParse(gammaInput.text, out float gamma)) currentSim.gamma = gamma;
            if (float.TryParse(droughtChanceInput.text, out float droughtChance)) currentSim.droughtChance = droughtChance;
            if (float.TryParse(preyCountInput.text, out float preyCount)) currentSim.prey = preyCount;
            if (float.TryParse(predCountInput.text, out float predCount)) currentSim.predator = predCount;
            if (float.TryParse(resourceCountInput.text, out float resourceCount)) 
            { 
                currentSim.resources = resourceCount; 
                currentSim.maxResource = resourceCount; 
            }
            if (float.TryParse(preyCarryingCapacityInput.text, out float preyCarryingCapacity)) currentSim.preyCarryingCapacity = preyCarryingCapacity;
            

            currentSim.runID = runIDInput.text.Trim();

            currentSim.StartSim();

            status.text = $"Simulation {currentSim.runID} started";
        }
    }

    private void Stop()
    {
        if (currentSim != null)
        {
            currentSim.EndRun();
            status.text = $"Simulation {currentSim.runID} stopped";
        }
    }

    // displays values
    private void Update()
    {
        if (currentSim != null)
        {
            preyCountText.text = currentSim.prey.ToString("F3");
            predCountText.text = currentSim.predator.ToString("F3");
            resourcesCountText.text = currentSim.resources.ToString("F3");
            droughtSeverityText.text = currentSim.droughtLevel.ToString("F3");
        }
    }
}
