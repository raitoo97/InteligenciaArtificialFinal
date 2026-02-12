using UnityEngine;
public enum HealingZoneTeam
{
    Blue,
    Violet
}
public class HealingZone : MonoBehaviour
{
    [SerializeField] private HealingZoneTeam zoneType;
    [SerializeField] private float healAmount = 60f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            var boid = other.GetComponent<Boid>();
            if (boid == null) return;
            if (IsCorrectTeam(boid.typeBoid))
            {
                boid.Life.Heal(healAmount);
            }
        }
        if (other.gameObject.layer == 10)
        {
            var leader = other.GetComponent<Leader>();
            if (leader == null) return;
            if (IsCorrectLeaderTeam(leader))
            {
                leader.Life.Heal(healAmount);
            }
        }
    }
    private bool IsCorrectTeam(TypeBoid boidType)
    {
        if (zoneType == HealingZoneTeam.Blue && boidType == TypeBoid.BlueTeam)
            return true;
        if (zoneType == HealingZoneTeam.Violet && boidType == TypeBoid.VioletTeam)
            return true;
        return false;
    }
    private bool IsCorrectLeaderTeam(Leader leaderType)
    {
        if(zoneType == HealingZoneTeam.Violet && leaderType.IsVioletLeader)
            return true;
        if(zoneType == HealingZoneTeam.Blue && !leaderType.IsVioletLeader)
            return true;
        return false;
    }
}
