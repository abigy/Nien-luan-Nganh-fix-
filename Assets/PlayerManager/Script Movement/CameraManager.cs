using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;

    public Transform targetTranform;
    public Transform cameraPivot;
    public Transform cameraTransform;
    public LayerMask collsionLayers;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;
    private float defaultPosition;
    public float cameraCollisionRadius = 0.2f;

    public float cameraCollisionOffset = 0.2f; // máy ảnh sẽ nhảy ra khỏi vật thể mà nó va chạm (giá trị càng cao độ nảy càng cao)

    public float minimumCollisionOffset = 0.2f;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;
    public float cameraFollowSpeed = 0.2f; // cameraFollowSpeed đã khởi tạo sẵn giá trị nên khi gọi biến phải sử dụng ref trong hàm có sẵn
    public float lookAngle; //Camera looking up and down
    public float pivotAngle; //Camera looking left and right

    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        targetTranform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleCameraMovement()
    {
        FollowTarget();
        RotataCamera();
        HandleCameraCollision();
    }
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTranform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotataCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle); //Giới hạn giá trị trong khoảng mini - max

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation); // Quaternion.Euler cho phép biểu diễn 3 trục x,y,z vì
                                                                // Quaternion mặc định biểu diễn 4 tham số w,x,y,z (Tham khảo)
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation; 
    }

    private void HandleCameraCollision()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collsionLayers)) //tạo quả cầu vô hình để phát hiện va chạm cho camera
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
