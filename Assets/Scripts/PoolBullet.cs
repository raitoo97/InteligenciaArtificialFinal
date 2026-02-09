using System.Collections.Generic;
using UnityEngine;
public class PoolBullet : MonoBehaviour
{
    public static PoolBullet instance;
    private List<GameObject> bulletsPool = new List<GameObject>();
    public GameObject bulletPrefab;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
    public void CompleteList(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var bullet = Instantiate(bulletPrefab, this.transform);
            bullet.SetActive(false);
            bulletsPool.Add(bullet);
        }
    }
    public GameObject GetBullet()
    {
        foreach (var bullet in bulletsPool)
        {
            if (!bullet.activeSelf)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        CompleteList(1);
        var auxBullet = bulletsPool[bulletsPool.Count - 1];
        auxBullet.SetActive(true);
        return auxBullet;
    }
}
