using UnityEngine;
public class Life
{
    private float _life;
    public Life(float life)
    {
        _life = life;
    }
    private void CheckLife()
    {
        if (_life <= 0)
        {
            Debug.Log("Muerto");
        }
    }
    public void TakeDamage(float damage)
    {
        _life -= damage;
        CheckLife();
        Debug.Log($"Vida actual: {_life}");
    }
    public float GetLife { get => _life; }
}
