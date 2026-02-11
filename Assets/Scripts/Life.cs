using UnityEngine;
using UnityEngine.UI;
public class Life
{
    private float _maxLife;
    private float _currentLife;
    private Slider _slider;
    private GameObject _gameObject;
    public Life(GameObject go,float life, Slider slider)
    {
        _maxLife = life;
        _currentLife = _maxLife;
        _slider = slider;
        _gameObject = go;
        if (_slider != null)
            _slider.maxValue = _maxLife;
        if (_slider != null)
            _slider.value = _currentLife;
    }
    private void CheckLife()
    {
        if (_currentLife <= 0)
        {
            GameObject.Destroy(_gameObject);
        }
    }
    public void TakeDamage(float damage)
    {
        _currentLife -= damage;
        _currentLife = Mathf.Clamp(_currentLife, 0, _maxLife);
        if (_slider != null)
            _slider.value = _currentLife;
        CheckLife();
    }
    public void Heal(float heal)
    {
        _currentLife += heal;
        if (_currentLife > _maxLife)
            _currentLife = Mathf.Clamp(_currentLife, 0, _maxLife);
        _slider.value = _currentLife;
    }
    public float GetLife { get => _currentLife; }
}
