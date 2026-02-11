using UnityEngine;
using UnityEngine.UI;
public class Life
{
    private float _life;
    private Slider _slider;
    private GameObject _gameObject;
    public Life(GameObject go,float life, Slider slider)
    {
        _life = life;
        _slider = slider;
        _gameObject = go;
    }
    private void CheckLife()
    {
        if (_life <= 0)
        {
            GameObject.Destroy(_gameObject);
        }
    }
    public void TakeDamage(float damage)
    {
        _life -= damage;
        _slider.value = _life;
        CheckLife();
    }
    public void Heal(float heal)
    {
        _life += heal;
        if (_life > _slider.maxValue)
            _life = _slider.maxValue;
        _slider.value = _life;
    }
    public float GetLife { get => _life; }
}
