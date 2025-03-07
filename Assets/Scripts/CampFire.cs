using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;
    public float delayTime = 0;

    List<IDamagable> things = new List<IDamagable>();

    void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);
    }

    void DealDamage()
    {
        for (int i = 0; i < things.Count; i++)
        {
            things[i].TakePhysicalDamage(damage);
        }
    }

    IEnumerator AddDelay(IDamagable damagable)
    {
        // 딜레이 시간 적용
        yield return new WaitForSeconds(delayTime);

        // 대기 중 범위 벗어나면 적용 X
        if (things.Contains(damagable) == false)
        {
            things.Add(damagable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            StartCoroutine(AddDelay(damagable));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            things.Remove(damagable);
        }
    }

}
