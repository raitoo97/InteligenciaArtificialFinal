using UnityEngine;
using UnityEngine.UI;
public class Life
{
    private float _life;
    private Slider _slider;
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
    }
    public float GetLife { get => _life; }
}
