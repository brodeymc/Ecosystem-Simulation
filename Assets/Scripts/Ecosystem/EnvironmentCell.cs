using UnityEngine;

public class EnvironmentCell : MonoBehaviour
{
    // Soil moisture which affects growth rate, decreases during drought periods
    public float moisture;

    // Density of the vegetation
    public float density;

    public EnvironmentCell(float moisture, float density)
    {
        this.moisture = moisture;
        this.density = density;
    }
}
