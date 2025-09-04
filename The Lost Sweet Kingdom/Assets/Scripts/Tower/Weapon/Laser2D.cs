using UnityEngine;

public class Laser2D : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer laserBody;   // 본체 (Draw Mode = Tiled)
    public SpriteRenderer startGlow;   // 시작점 Glow
    public SpriteRenderer endGlow;     // 끝점 Glow

    [Header("Settings")]
    public float laserWidth = 0.2f;    // 레이저 두께
    public float scrollSpeed = 2f;     // 텍스처 흐르는 속도

    private Material bodyMat;

    void Start()
    {
        if (laserBody != null)
        {
            // 머티리얼 인스턴스화 (텍스처 Offset 개별 제어용)
            bodyMat = Instantiate(laserBody.material);
            laserBody.material = bodyMat;
        }
    }

    /// <summary>
    /// 레이저를 갱신 (start → end까지)
    /// </summary>
    public void UpdateLaser(Vector2 startPos, Vector2 endPos)
    {
        Vector2 dir = endPos - startPos;
        float length = dir.magnitude;

        // 본체 위치/길이 조정
        laserBody.transform.position = startPos;
        laserBody.transform.right = dir.normalized; // 회전
        laserBody.size = new Vector2(length, laserWidth); // 길이/굵기

        // Glow 위치
        if (startGlow != null) startGlow.transform.position = startPos;
        if (endGlow != null) endGlow.transform.position = endPos;

        // UV 스크롤 (움직이는 느낌)
        if (bodyMat != null)
        {
            Vector2 offset = bodyMat.mainTextureOffset;
            offset.x -= Time.deltaTime * scrollSpeed;
            bodyMat.mainTextureOffset = offset;
        }
    }
}
