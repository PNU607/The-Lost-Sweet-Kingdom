using UnityEngine;

[ExecuteAlways] // 에디터에서도 즉시 갱신
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

    private void Awake()
    {
        InitMaterial();
    }

    private void Start()
    {
        InitMaterial();
    }

    private void InitMaterial()
    {
        if (laserBody != null && (bodyMat == null || Application.isEditor && !Application.isPlaying))
        {
            // 머티리얼 인스턴스화 (Editor에서도 각기 다른 Offset)
            bodyMat = Instantiate(laserBody.sharedMaterial);
            laserBody.sharedMaterial = bodyMat;
        }
    }

    /// <summary>
    /// 레이저를 startPos → endPos로 갱신
    /// </summary>
    public void UpdateLaser(Vector2 startPos, Vector2 endPos)
    {
        if (laserBody == null) return;

        Vector2 dir = endPos - startPos;
        float length = dir.magnitude;

        // 방향 회전 설정
        Vector3 direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 본체 중앙이 아닌 "시작점 → 끝점"에 정확히 맞게 위치 보정
        laserBody.transform.position = (Vector3)startPos + (Vector3)(direction * length * 0.5f);
        laserBody.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Draw Mode = Tiled 일 때만 size 적용 가능
        laserBody.drawMode = SpriteDrawMode.Tiled;
        laserBody.size = new Vector2(length, laserWidth);

        // Glow 위치
        if (startGlow != null)
        {
            startGlow.transform.position = startPos;
            startGlow.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (endGlow != null)
        {
            endGlow.transform.position = endPos;
            endGlow.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // UV 스크롤 (움직이는 느낌)
        if (bodyMat != null)
        {
            Vector2 offset = bodyMat.mainTextureOffset;
            offset.x -= Time.deltaTime * scrollSpeed;
            bodyMat.mainTextureOffset = offset;
        }

#if UNITY_EDITOR
        // SceneView에서 바로 반영되도록
        if (!Application.isPlaying)
            UnityEditor.SceneView.RepaintAll();
#endif
    }

    private void OnDisable()
    {
        // 편집 중 레이저가 끊길 때 offset 초기화
        if (bodyMat != null)
            bodyMat.mainTextureOffset = Vector2.zero;
    }
}
