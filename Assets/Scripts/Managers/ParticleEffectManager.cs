using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectManager : MonoBehaviour
{
    public GameObject summonEffectPrefab;
    public GameObject summonEnemyEffectPrefab;

    private static ParticleEffectManager instance;

    public static ParticleEffectManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetSummonEffect()
    {
        return summonEffectPrefab;
    }

    public GameObject GetEnemySummonEffect()
    {
        return summonEnemyEffectPrefab;
    }
}
