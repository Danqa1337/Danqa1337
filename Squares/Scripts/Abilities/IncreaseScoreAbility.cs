using UnityEngine;

public class IncreaseScoreAbility : MonoBehaviour, IAbilityOnTarget
{
    public void Apply(GameObject target)
    {
        if (target.GetComponent<Player>() != null)
        {
            RuntimeScoreCounter.IncreaseScore();
        }
    }
}