using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Soul", menuName = "Soul Hunter/Soul")]
public class SoulData : ScriptableObject // Mort
{
    public Color color;
    public int soulPower;

    [SerializeField] GameObject particle;

    public void SpawnParticle(Vector3 position)
    {
        GameObject p = Instantiate(particle, position, Quaternion.identity);
        p.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), 200));
        p.GetComponent<Loot>().soulPower = soulPower;
    }
}
