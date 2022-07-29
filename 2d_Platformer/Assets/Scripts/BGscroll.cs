using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGscroll : MonoBehaviour
{
    MeshRenderer mrenderer;

    public float speed;
    float offset;

    private void Awake()
    {
        mrenderer = GetComponent<MeshRenderer>();    //초기화해줌
    }

    private void Update()
    {
        //머터리얼의 offset 변경하기
        offset += Time.deltaTime * speed;   //offset을 deltaTime 이용해서 계속 증가시켜줌
        mrenderer.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
