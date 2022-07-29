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
        mrenderer = GetComponent<MeshRenderer>();    //�ʱ�ȭ����
    }

    private void Update()
    {
        //���͸����� offset �����ϱ�
        offset += Time.deltaTime * speed;   //offset�� deltaTime �̿��ؼ� ��� ����������
        mrenderer.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
