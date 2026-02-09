using UnityEngine;
public enum BulletType
{
    Blue,
    Violet,
}
public class Bullet : MonoBehaviour
{
    [SerializeField]private float speed;
    public BulletType bulletType;
    private Renderer _renderer;
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    public void Shoot(TypeBoid shooterTeam)
    {
        bulletType = (shooterTeam == TypeBoid.BlueTeam) ? BulletType.Blue : BulletType.Violet;
        _renderer.material.color = (bulletType == BulletType.Blue) ? Color.blue : Color.magenta;
        Invoke("DesactivateBullet", 5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            var boid = other.GetComponent<Boid>();
            if (boid == null) return;
            if ((bulletType == BulletType.Blue && boid.typeBoid == TypeBoid.VioletTeam) ||(bulletType == BulletType.Violet && boid.typeBoid == TypeBoid.BlueTeam))
            {
                boid.Life.TakeDamage(10); // ejemplo de daño
            }
            DesactivateBullet();
        }
    }
    void Update()
    {
        this.transform.position += this.transform.forward * speed * Time.deltaTime;
    }
    private void DesactivateBullet()
    {
        this.gameObject.SetActive(false);
    }
}
