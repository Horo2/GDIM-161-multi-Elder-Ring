using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Horo
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] Transform cameraPivotTransform;

        // Change thses to tweak camera performance
        [Header("Camera Settings")]       
        private float cameraSmoothSpeed = 1; // The bigger this number , the longer for the camera to reach its position during movement
        [SerializeField] float leftAndRightLookSpeed = 220;
        [SerializeField] float upAndDownRotationSpeed = 220;
        [SerializeField] float minmumPivot = -30; // the lowest point you are able to look down
        [SerializeField] float maxmumPivot = 60; // the highest point you are able to look up
        [SerializeField] float cameraCollisionRadius = 0.2f;
        [SerializeField] LayerMask collideWithLayers;

        [Header("Camera Values")]
        private Vector3 cameraVelociity;
        private Vector3 cameraObjectPosition; //Used for camera Collisions(Moves the camera object this position upon colliding)
        [SerializeField] float leftAndRightLookAngle;
        [SerializeField] float upAndDownLookAngle;
        private float CameraZPosition; //Values used for camera collisions
        private float targetCameraZPosition; //Values used for camera collisions

        private void Awake()
        {
            if(instance == null )
            {
                instance = this;
            }
             else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            CameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if(player != null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }

        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelociity, cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            // If locked on, force rotation towards target
            // else rotate regularly

            //normal rotations
            // Roatate left and right based on Horizontal movement on the right joystick or mouse
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightLookSpeed) * Time.deltaTime;
            // Rotate up and down based on Vertical movement on the right joystick or mouse
            upAndDownLookAngle += (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            // Clamp the up and down look angle bteween a min and max value
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minmumPivot, maxmumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;
            // Rotate this gameobejct left and right
            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            // Rotate this Pivot gameobject up and down
            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = CameraZPosition;
            RaycastHit hit;
            // ��������������㵽���������ķ����������������׼��
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position; 
            direction.Normalize();

            // ִ���������߼�⣬����Ƿ����κζ����������·����
            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                // ����ж������ж��뾶�ϣ����������㵽��ײ��ľ���
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                // ���������Ŀ��Zλ��Ϊ��ײ������ȥ��ײ�뾶�ĸ�ֵ
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            // ���������Ŀ��λ�õ�ֵС����ײ�뾶�����Ǽ�ȥ��ײ�뾶(ʹ�䵯��)
            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            // �������������Zλ�ã�ʹ��ƽ�����ɵ��µ�Ŀ��Zλ��
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z , targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}
