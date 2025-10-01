using UnityEngine;

public abstract class AnimalAgent : MonoBehaviour
{
    public Vector2 position;
    public float energy;

    public abstract void Move(EnvironmentCell[,] grid);
    public abstract void Consume(EnvironmentCell[,] grid);
    public virtual bool CanReproduce()
    {
        return energy > 10f;
    }
    public abstract bool IsDead();
}
