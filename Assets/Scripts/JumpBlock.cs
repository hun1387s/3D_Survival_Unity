using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBlock : MonoBehaviour
{
    public float jumpForce = 10f;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("점프대 진입");
        if (other.CompareTag("Player"))
        {
            Debug.Log("캐릭터 진입");
            Rigidbody _rigidbody = other.GetComponent<Rigidbody>();
            if (_rigidbody != null)
            {
                // 위쪽으로 힘을 가한다.
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
                _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

        }
    }

}
