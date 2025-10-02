using UnityEngine;

public abstract class AnimalAgent : MonoBehaviour
{
    public Vector2 position;
    public float energy;
    public float maxEnergy = 20f;
    public int x;
    public int y;
    public bool prey;
    protected SimulationController controller;

    public virtual bool CanReproduce()
    {
        return energy > 10f;
    }

    public abstract bool IsDead();

    protected void Move(int newX, int newY)
    {
        controller.MoveAgent(this, newX, newY);
    }

    public abstract void TimeStep();
}
