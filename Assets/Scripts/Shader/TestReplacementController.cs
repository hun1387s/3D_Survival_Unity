using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReplacementController : MonoBehaviour
{
    // 대체할 셰이더 (Inspector에서 설정 가능)
    public Shader m_replacementShader;

    private void OnEnable()
    {
        if (m_replacementShader != null)
        {
            // 카메라가 씬(Scene) 내 모든 셰이더를 대체함
            // RenderType이 일치하는 셰이더만 교체됨
            GetComponent<Camera>().SetReplacementShader(m_replacementShader, "RenderType");
        }
    }

    private void OnDisable()
    {
        // 원래 셰이더로 복원
        GetComponent<Camera>().ResetReplacementShader();
    }
}
