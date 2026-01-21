using UnityEngine;
using UnityEngine.Splines;

public class ObjectAlongSpline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject[] prefabsToSpawn; 

    public int objectCount = 10;
    public float spacing = 2f;
    public bool useFixedCount = true;

    public bool orientToPath = true;
    public Vector3 rotationOffset;

    public float minScale = 1.5f;
    public float maxScale = 2.5f;

    public float minRotationY = 0f;
    public float maxRotationY = 360f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.EditorApplication.delayCall += () => {
            if (this != null) Spawn();
        };
    }
#endif

    [ContextMenu("Spawn Objects")]
    public void Spawn()
    {
        if (splineContainer == null || prefabsToSpawn == null || prefabsToSpawn.Length == 0) return;
        if (splineContainer.Spline == null) return;

        ClearObjects();

        float length = splineContainer.CalculateLength();
        int finalCount = useFixedCount ? objectCount : Mathf.FloorToInt(length / spacing);
        if (finalCount <= 1) finalCount = 2; 

        for (int i = 0; i < finalCount; i++)
        {
            float t = (float)i / (finalCount - 1);
            
            Vector3 position = (Vector3)splineContainer.EvaluatePosition(t);
            Vector3 forward = (Vector3)splineContainer.EvaluateTangent(t);
            Vector3 up = (Vector3)splineContainer.EvaluateUpVector(t);

            GameObject randomPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
            if (randomPrefab == null) continue;

            GameObject spawned = Instantiate(randomPrefab, position, Quaternion.identity, transform);

            if (orientToPath)
            {
                spawned.transform.rotation = Quaternion.LookRotation(forward, up) * Quaternion.Euler(rotationOffset);
            }

            float randomY = Random.Range(minRotationY, maxRotationY);
            spawned.transform.Rotate(Vector3.up, randomY, Space.Self);

            float randomS = Random.Range(minScale, maxScale);
            spawned.transform.localScale = Vector3.one * randomS;
        }
    }

    public void ClearObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}