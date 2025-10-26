using UnityEngine;
using System.Collections.Generic;

namespace ATD
{
    public class BuildingPlacer : MonoBehaviour
    {
        [Header("설치 불가 대상 오브젝트 배열")]
        public GameObject[] collisionObjects; // 인스펙터에서 지정

        [Header("표시 색상 (반투명)")]
        public Color validColor = new Color(0f, 1f, 0f, 0.35f);
        public Color invalidColor = new Color(1f, 0f, 0f, 0.35f);

        private GameObject previewObject;
        private Renderer overlayRenderer;
        private bool canPlace = false;
        private float rotationY = 0f;
        private GameObject currentPrefab;
        private float fixedY;

        private HashSet<Collider> targetColliders = new HashSet<Collider>();

        // 설치 데이터
        private int refundGold;
        private int timeCost;
        public DayMode dayModeRef;

        void Awake()
        {
            // 배열에 지정된 오브젝트들의 자신+자식 콜라이더 수집
            foreach (var obj in collisionObjects)
            {
                if (obj == null) continue;
                foreach (var col in obj.GetComponentsInChildren<Collider>())
                {
                    targetColliders.Add(col);
                }
            }
        }

        /// <summary>
        /// 설치 모드 시작
        /// </summary>
        public void StartPlacement(GameObject prefab, int goldCost, int timeCost, DayMode dayMode)
        {
            refundGold = goldCost;
            this.timeCost = timeCost;
            dayModeRef = dayMode;

            currentPrefab = prefab;

            CreatePreview();
            gameObject.SetActive(true);
            enabled = true;
        }

        void Update()
        {
            if (previewObject == null) return;

            FollowMousePosition();
            HandlePlacement();
            HandleCancel();
        }

        void CreatePreview()
        {
            if (previewObject != null) Destroy(previewObject);

            previewObject = Instantiate(currentPrefab);
            previewObject.name = "Preview_" + currentPrefab.name;

            foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            {
                col.enabled = true;
                col.isTrigger = true;
            }

            CreateOverlay();
            UpdateOverlayColor(invalidColor);

            fixedY = previewObject.transform.position.y;
        }

        void CreateOverlay()
        {
            Bounds b;
            var renderers = previewObject.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length > 0)
            {
                b = new Bounds(renderers[0].bounds.center, Vector3.zero);
                foreach (var r in renderers) b.Encapsulate(r.bounds);
            }
            else
            {
                var cols = previewObject.GetComponentsInChildren<Collider>();
                b = new Bounds(cols[0].bounds.center, Vector3.zero);
                foreach (var c in cols) b.Encapsulate(c.bounds);
            }

            var overlayGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlayGO.name = "_PlacementOverlay";
            overlayGO.transform.SetParent(previewObject.transform, worldPositionStays: true);
            Destroy(overlayGO.GetComponent<Collider>());

            Material mat = CreateUnlitTransparentMaterial();
            overlayRenderer = overlayGO.GetComponent<Renderer>();
            overlayRenderer.sharedMaterial = mat;
            overlayRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            overlayRenderer.receiveShadows = false;

            overlayGO.transform.position = b.center + new Vector3(0, 0.5f, 0);
            overlayGO.transform.localScale = new Vector3(b.size.x, 0.05f, b.size.z);
        }

        public Material temp;
        Material CreateUnlitTransparentMaterial()
        {
            //Shader s =
            //    Shader.Find("Universal Render Pipeline/Unlit") ??
            //    Shader.Find("Unlit/Color") ??
            //    Shader.Find("Standard");

            //var mat = new Material(s);

            //if (s.name == "Standard")
            //{
            //    mat.SetFloat("_Mode", 3);
            //    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //    mat.SetInt("_ZWrite", 0);
            //    mat.DisableKeyword("_ALPHATEST_ON");
            //    mat.EnableKeyword("_ALPHABLEND_ON");
            //    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //    mat.renderQueue = 3000;
            //}

            return Instantiate(temp);
            //return mat;
        }

        void FollowMousePosition()
        {
            var cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                Vector3 newPos = new Vector3(hit.point.x, fixedY, hit.point.z);
                previewObject.transform.position = newPos;
                previewObject.transform.rotation = Quaternion.Euler(0, rotationY, 0);

                bool prev = canPlace;
                canPlace = !IsCollidingWithOthers();

                if (canPlace != prev)
                {
                    UpdateOverlayColor(canPlace ? validColor : invalidColor);
                }
            }
        }

        bool IsCollidingWithOthers()
        {
            var previewCols = previewObject.GetComponentsInChildren<Collider>();

            foreach (var col in previewCols)
            {
                Bounds bounds = col.bounds;
                Vector3 center = bounds.center;
                Vector3 halfExtents = bounds.extents;

                var hits = Physics.OverlapBox(center, halfExtents, col.transform.rotation);
                foreach (var h in hits)
                {
                    if (h == null) continue;
                    if (h.transform.IsChildOf(previewObject.transform)) continue;

                    if (targetColliders.Contains(h)) return true;
                    if (h.CompareTag("PlacedPrefab")) return true;
                }
            }
            return false;
        }

        void HandlePlacement()
        {
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                // 좌표 유효성 검사
                Vector3 pos = previewObject.transform.position;
                if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z) ||
                    float.IsInfinity(pos.x) || float.IsInfinity(pos.y) || float.IsInfinity(pos.z))
                {
                    Debug.LogError("[BuildingPlacer] 설치 위치 값이 잘못되었습니다. 설치를 취소합니다.");
                    CancelPlacement();
                    return;
                }

                Transform parent = null;
                var gm = GameObject.Find("GameManager");
                if (gm != null) parent = gm.transform;

                var placed = Instantiate(currentPrefab, pos, previewObject.transform.rotation, parent);

                foreach (Collider col in placed.GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                    col.isTrigger = true;
                }

                placed.tag = "PlacedPrefab";

                // ✅ 실제 설치 후 시간 차감
                if (dayModeRef != null)
                {
                    dayModeRef.Consume(timeCost);
                }

                Destroy(previewObject);
                overlayRenderer = null;
                enabled = false;
            }
        }

        void HandleCancel()
        {
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }

        void CancelPlacement()
        {
            if (dayModeRef != null)
            {
                GameManager.Instance.gold += refundGold;
                dayModeRef.Refresh();
            }

            if (previewObject) Destroy(previewObject);
            overlayRenderer = null;
            enabled = false;
        }

        void UpdateOverlayColor(Color color)
        {
            if (overlayRenderer == null) return;
            var mat = overlayRenderer.material;

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
        }
    }
}
