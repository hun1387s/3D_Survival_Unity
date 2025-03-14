﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;
    public float delayTime = 0;
    bool enter = false;

    HashSet<IDamagable> things = new HashSet<IDamagable>(); // 중복 방지

    void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    void DealDamage()
    {
        foreach (var thing in things)
        {
            thing.TakePhysicalDamage(damage);
        }
    }

    IEnumerator AddDelay(IDamagable damagable)
    {
        yield return new WaitForSeconds(delayTime);

        if (!things.Contains(damagable) && enter)
        {
            things.Add(damagable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        enter = true;
        if (other.TryGetComponent(out IDamagable damagable))
        {
            StartCoroutine(AddDelay(damagable));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enter = false;
        if (other.TryGetComponent(out IDamagable damagable))
        {
            things.Remove(damagable);
        }
    }
}
