using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetShadowArea : MonoBehaviour
{
    public Transform targetLight; // 타깃이 되는 Point 광원
    public GameObject shadowAreaPt; // 그림자 외곽선 포인트 프리팹
    public GameObject idMaker; // 인덱스 순번을 매겨줄 게임 오브젝트
    List<Vector3> shadowArea = new List<Vector3>(); // 그림자 영역
    List<int> colID = new List<int>(); // 충돌선을 이어줄 순서
    Vector3 centerPos; // 오브젝트 중점
    float rotateCount = 0;
    bool isCheckPoints = false;

    void Start()
    {
        Mesh mesh;
        mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertex; // 3D모델의 꼭짓점을 담을 배열
        Vector3[] targetAngle; // 광원에서부터 오브젝트 꼭짓점으로 향하는 방향벡터를 담을 배열
        Vector3 targetCenterAngle; // 오브젝트 중점을 향하는 방향벡터를 담을 변수
        vertex = new Vector3[mesh.vertexCount / 3];
        targetAngle = new Vector3[vertex.Length];

        int obsLayer; // 장애물 전용 레이어 마스크 값
        int wallLayer; // 벽 전용 레이어 마스크 값
        int layerMask; // 레이어 마스크
        int centerLayerMask; // 중점 전용 레이어 마스크
        obsLayer = LayerMask.NameToLayer("OBSTACLE");
        wallLayer = LayerMask.NameToLayer("WALL");
        layerMask = 1 << wallLayer | 1 << obsLayer;
        centerLayerMask = 1 << wallLayer;

        RaycastHit hit;
        centerPos = mesh.bounds.center + transform.position;
        targetCenterAngle = (centerPos - targetLight.position).normalized;
        for (int i = 0; i < vertex.Length; i++)
        {
            vertex[i] = mesh.vertices[i] + transform.position;
            targetAngle[i] = (vertex[i] - targetLight.position).normalized;

            if (Physics.Raycast(targetLight.position, targetLight.transform.TransformDirection(targetAngle[i]), out hit, Mathf.Infinity, layerMask)) // 오브젝트 꼭짓점 위치로 레이 발사
            {
                Debug.DrawRay(targetLight.position, targetLight.transform.TransformDirection(targetAngle[i]) * hit.distance, Color.yellow);
                Debug.Log("Did Hit!");
                shadowArea.Add(hit.point);
                Instantiate(shadowAreaPt, shadowArea[shadowArea.Count - 1], Quaternion.identity, gameObject.transform.GetChild(1).transform);
            }
        }
        if (Physics.Raycast(targetLight.position, targetLight.transform.TransformDirection(targetCenterAngle), out hit, Mathf.Infinity, centerLayerMask)) // 오브젝트 센터 위치로 레이 발사
        {
            Debug.DrawRay(targetLight.position, targetLight.transform.TransformDirection(targetCenterAngle) * hit.distance, Color.red);
            Debug.Log("Center Ray Hit!");
            centerPos = hit.point;
        }

        float idMakerLength = 0f;
        for (int i = 0; i < vertex.Length; i++)
        {
            if (i == 0)
                idMakerLength = Vector3.Distance(vertex[i], centerPos);
            else if (idMakerLength < Vector3.Distance(vertex[i], centerPos))
                idMakerLength = Vector3.Distance(vertex[i], centerPos);
        }
        //idMaker.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal); // 충돌 지점으로부터 법선 벡터
        Instantiate(idMaker, centerPos, Quaternion.FromToRotation(-Vector3.forward, hit.normal), gameObject.transform.GetChild(1).transform);
        idMaker.GetComponent<BoxCollider>().size = new Vector3(idMakerLength * 2.0f, 0, 0);
        idMaker.GetComponent<BoxCollider>().center = new Vector3(idMaker.GetComponent<BoxCollider>().size.x * 0.5f, 0, 0);

        Debug.Log(vertex.Length);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < shadowArea.Count; i++)
        {
            Gizmos.DrawWireSphere(shadowArea[i], 0.1f);
        }
        Gizmos.DrawWireSphere(centerPos, 0.1f);
    }

    IEnumerator RotateCheck()
    {
        while(true)
        {
            
            yield return null;
        }
    }
    /*
    public Transform targetLight; // 타깃이 되는 Point 광원
    public GameObject shadowAreaPt; // 그림자 외곽선 포인트 지점
    public GameObject idMaker; // 인덱스 순번을 매겨줄 게임 오브젝트
    List<Vector3> shadowArea = new List<Vector3>(); // 그림자 영역
    List<int> colID = new List<int>(); // 충돌선을 이어줄 순서
    Vector3 centerPos; // 오브젝트 중점
    float rotateCount = 0;
    bool isCheckPoints = false;

    void Start()
    {
        Mesh mesh;
        mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertex; // 3D모델의 꼭짓점을 담을 배열
        Vector3[] targetAngle; // 광원에서부터 오브젝트 꼭짓점으로 향하는 방향벡터를 담을 배열
        Vector3 targetCenterAngle; // 오브젝트 중점을 향하는 방향벡터를 담을 변수
        vertex = new Vector3[mesh.vertexCount / 3];
        targetAngle = new Vector3[vertex.Length];

        int obsLayer; // 장애물 전용 레이어 마스크 값
        int wallLayer; // 벽 전용 레이어 마스크 값
        int layerMask; // 레이어 마스크
        int centerLayerMask; // 중점 전용 레이어 마스크
        obsLayer = LayerMask.NameToLayer("OBSTACLE");
        wallLayer = LayerMask.NameToLayer("WALL");
        layerMask = 1 << wallLayer | 1 << obsLayer;
        centerLayerMask = 1 << wallLayer;

        RaycastHit hit;
        centerPos = mesh.bounds.center + transform.position;
        targetCenterAngle = (centerPos - targetLight.position).normalized;
        for (int i = 0; i < vertex.Length; i++)
        {
            vertex[i] = mesh.vertices[i] + transform.position;
            targetAngle[i] = (vertex[i] - targetLight.position).normalized;

            if (Physics.Raycast(targetLight.position, targetLight.transform.TransformDirection(targetAngle[i]), out hit, Mathf.Infinity, layerMask)) // 오브젝트 꼭짓점 위치로 레이 발사
            {
                Debug.DrawRay(targetLight.position, targetLight.transform.TransformDirection(targetAngle[i]) * hit.distance, Color.yellow);
                Debug.Log("Did Hit!");
                shadowArea.Add(hit.point);
                Instantiate(shadowAreaPt, shadowArea[shadowArea.Count - 1], Quaternion.identity, gameObject.transform.GetChild(1).transform);
            }
        }
        if (Physics.Raycast(targetLight.position, targetLight.transform.TransformDirection(targetCenterAngle), out hit, Mathf.Infinity, centerLayerMask)) // 오브젝트 센터 위치로 레이 발사
        {
            Debug.DrawRay(targetLight.position, targetLight.transform.TransformDirection(targetCenterAngle) * hit.distance, Color.red);
            Debug.Log("Center Ray Hit!");
            centerPos = hit.point;
        }

        float idMakerLength = 0f;
        for (int i = 0; i < vertex.Length; i++)
        {
            if (i == 0)
                idMakerLength = Vector3.Distance(vertex[i], centerPos);
            else if (idMakerLength < Vector3.Distance(vertex[i], centerPos))
                idMakerLength = Vector3.Distance(vertex[i], centerPos);
        }
        //idMaker.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal); // 충돌 지점으로부터 법선 벡터
        Instantiate(idMaker, centerPos, Quaternion.FromToRotation(-Vector3.forward, hit.normal), gameObject.transform.GetChild(1).transform);
        idMaker.GetComponent<BoxCollider>().size = new Vector3(idMakerLength * 2.0f, 0, 0);
        idMaker.GetComponent<BoxCollider>().center = new Vector3(idMaker.GetComponent<BoxCollider>().size.x * 0.5f, 0, 0);

        Debug.Log(vertex.Length);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < shadowArea.Count; i++)
        {
            Gizmos.DrawWireSphere(shadowArea[i], 0.1f);
        }
        Gizmos.DrawWireSphere(centerPos, 0.1f);
    }

    IEnumerator RotateCheck()
    {
        while(true)
        {
            
            yield return null;
        }
    }
    */
}