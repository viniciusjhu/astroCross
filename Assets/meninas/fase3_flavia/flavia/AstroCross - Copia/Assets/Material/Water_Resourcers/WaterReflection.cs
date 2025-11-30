using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterReflection : MonoBehaviour
{
    Camera mainCamera;
    Camera reflectionCamera;

    [Tooltip("The plane where the camera will be reflected, the water plane or any object with the same position and rotation")]
    public Transform reflectionPlane;
    [Tooltip("The texture used by the Water shader to display the reflection")]
    public RenderTexture outputTexture;

    public bool copyCameraParamerers;
    public float verticalOffset;
    private bool isReady;

    private Transform mainCamTransform;
    private Transform reflectionCamTransform;

    // Inicializa referências das câmeras e valida componentes.
    public void Awake()
    {
        mainCamera = Camera.main;

        reflectionCamera = GetComponent<Camera>();

        Validate();
    }

    // Renderiza a reflexão se o sistema estiver pronto.
    private void Update()
    {
        if (isReady)
            RenderReflection();
    }

    // Calcula a posição e rotação da câmera de reflexão baseada no plano da água.
    private void RenderReflection()
    {
        Vector3 cameraDirectionWorldSpace = mainCamTransform.forward;
        Vector3 cameraUpWorldSpace = mainCamTransform.up;
        Vector3 cameraPositionWorldSpace = mainCamTransform.position;

        cameraPositionWorldSpace.y += verticalOffset;

        Vector3 cameraDirectionPlaneSpace = reflectionPlane.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = reflectionPlane.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = reflectionPlane.InverseTransformPoint(cameraPositionWorldSpace);

        cameraDirectionPlaneSpace.y *= -1;
        cameraUpPlaneSpace.y *= -1;
        cameraPositionPlaneSpace.y *= -1;

        cameraDirectionWorldSpace = reflectionPlane.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = reflectionPlane.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = reflectionPlane.TransformPoint(cameraPositionPlaneSpace);

        reflectionCamTransform.position = cameraPositionWorldSpace;
        reflectionCamTransform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);
    }

    // Verifica se todos os componentes necessários estão atribuídos.
    private void Validate()
    {
        if (mainCamera != null)
        {
            mainCamTransform = mainCamera.transform;
            isReady = true;
        }
        else
            isReady = false;

        if (reflectionCamera != null)
        {
            reflectionCamTransform = reflectionCamera.transform;
            isReady = true;
        }
        else
            isReady = false;

        if (isReady && copyCameraParamerers)
        {
            copyCameraParamerers = !copyCameraParamerers;
            reflectionCamera.CopyFrom(mainCamera);

            reflectionCamera.targetTexture = outputTexture;
        }
    }
}