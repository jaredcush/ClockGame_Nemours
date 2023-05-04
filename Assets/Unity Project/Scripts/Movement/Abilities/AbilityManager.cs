using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages a list of equipped abilities and allows the CharacterController
/// to use and manipulate certain ability scripts.
/// </summary>
public class AbilityManager : MonoBehaviour
{
    [SerializeField]
    private int m_CurrAbilityIndex = 0;

    public Ability CurrentAbility { get; private set; }
    public PendulumAbility PendulumAbility { get; private set; }
    public HandsAbility HandsAbility { get; private set; }

    public List<Ability> TotalAbilities = new();

    private void Awake()
    {
        // Get individual components
        PendulumAbility = GetComponent<PendulumAbility>();
        HandsAbility = GetComponent<HandsAbility>();

        // Find and Equip Abilities
        TotalAbilities.AddRange(GetComponents<Ability>());
        if (TotalAbilities.Count > 0)
        {
            CurrentAbility = TotalAbilities[0];
        }
    }

    // + + + + | Functions | + + + + 

    // TODO: Test these fools

    public void NextAbility()
    {
        // Map ability index
        m_CurrAbilityIndex = (int) Mathf.Repeat(m_CurrAbilityIndex + 1, TotalAbilities.Count);
        CurrentAbility = TotalAbilities[m_CurrAbilityIndex];
        Debug.Log($"NextAbility! CurrentAbility is {CurrentAbility}");
    }

    public void PreviousAbility()
    {
        // Map ability index
        m_CurrAbilityIndex = (int)Mathf.Repeat(m_CurrAbilityIndex - 1, TotalAbilities.Count);
        CurrentAbility = TotalAbilities[m_CurrAbilityIndex];
        Debug.Log($"PreviousAbility! CurrentAbility is {CurrentAbility}");
    }
}
