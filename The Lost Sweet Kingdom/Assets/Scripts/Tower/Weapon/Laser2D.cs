using UnityEngine;

public class Laser2D : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer laserBody;   // ��ü (Draw Mode = Tiled)
    public SpriteRenderer startGlow;   // ������ Glow
    public SpriteRenderer endGlow;     // ���� Glow

    [Header("Settings")]
    public float laserWidth = 0.2f;    // ������ �β�
    public float scrollSpeed = 2f;     // �ؽ�ó �帣�� �ӵ�

    private Material bodyMat;

    void Start()
    {
        if (laserBody != null)
        {
            // ��Ƽ���� �ν��Ͻ�ȭ (�ؽ�ó Offset ���� �����)
            bodyMat = Instantiate(laserBody.material);
            laserBody.material = bodyMat;
        }
    }

    /// <summary>
    /// �������� ���� (start �� end����)
    /// </summary>
    public void UpdateLaser(Vector2 startPos, Vector2 endPos)
    {
        Vector2 dir = endPos - startPos;
        float length = dir.magnitude;

        // ��ü ��ġ/���� ����
        laserBody.transform.position = startPos;
        laserBody.transform.right = dir.normalized; // ȸ��
        laserBody.size = new Vector2(length, laserWidth); // ����/����

        // Glow ��ġ
        if (startGlow != null) startGlow.transform.position = startPos;
        if (endGlow != null) endGlow.transform.position = endPos;

        // UV ��ũ�� (�����̴� ����)
        if (bodyMat != null)
        {
            Vector2 offset = bodyMat.mainTextureOffset;
            offset.x -= Time.deltaTime * scrollSpeed;
            bodyMat.mainTextureOffset = offset;
        }
    }
}
