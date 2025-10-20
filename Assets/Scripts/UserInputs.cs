using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UserInputs : MonoBehaviour
{
    public TMP_InputField speedInput;
    public TMP_InputField gridInput;
    public TMP_InputField droughtInput;
    public Button start;
    public SimulationController simulationController;

    private void Start()
    {
        start.onClick.AddListener(StartSim);
    }

    private void StartSim()
    {
        float speed = float.Parse(speedInput.text);
        int gridSize = int.Parse(gridInput.text);
        float droughtChance = float.Parse(droughtInput.text);

        //simulationController.Initialize(gridSize, gridSize, speed, droughtChance);
    }
}
