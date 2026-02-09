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
    private void OnEnable()
    {
        Invoke("DesactivateBullet", 5);
        var renderer = GetComponent<Renderer>();
        if (bulletType == BulletType.Blue)
            renderer.material.color = Color.blue;
        else
            renderer.material.color = Color.magenta;
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
