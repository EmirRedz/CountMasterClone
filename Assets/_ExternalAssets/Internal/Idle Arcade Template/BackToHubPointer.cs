using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LaynarGames.IdleArcadeTemplate
{
    public class BackToHubPointer : MonoBehaviour
    {
        public Transform targetPosition;
        [SerializeField] RectTransform pointerRectTransform;
        [SerializeField] RectTransform arrow;
        [SerializeField] Image arrowImage;
        [SerializeField] Image homeImage;
        //[SerializeField] BoolScriptableData inHubArea;
        [SerializeField] string targetName;
        [SerializeField] bool doesntMatterIfInHub;
        public bool getTargetByName;
        [SerializeField] Image[] additionalImagesToDisable;

        bool arrowEnabled = true;
        private bool ISBannerUp;

        public float borderSize = 100;
        [SerializeField] private TextMeshProUGUI exclamationText;
        private void Start()
        {
            if (getTargetByName)
            {
                targetPosition = GameObject.Find(targetName).transform;
            }
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 targetPositionScreenPoint = WorldToScreenPointProjected(Camera.main, targetPosition.position);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSize  || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            if (isOffScreen)
            {
                RotatePointerTowardsTargetPosition();


                pointerRectTransform.gameObject.SetActive(true);

                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;

                if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
                if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;


                if (ISBannerUp)
                {
                    if (cappedTargetScreenPosition.y <= borderSize * 3) cappedTargetScreenPosition.y = borderSize * 3;
                }
                else
                {
                    if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
                }

                if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;


                pointerRectTransform.position = cappedTargetScreenPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);


                if (!arrowEnabled)
                {
                    arrowEnabled = true;
                    arrowImage.enabled = true;
                    
                    if (homeImage != null)
                    {
                        homeImage.enabled = true;
                    }

                    if (exclamationText != null)
                    {
                        exclamationText.enabled = true;
                    }

                    for (int i = 0; i < additionalImagesToDisable.Length; i++)
                    {
                        additionalImagesToDisable[i].gameObject.SetActive(true);
                    }
                }

            } else
            {
                if (arrowEnabled)
                {
                    Hide();
                }
            }
        }

        private void OnDisable()
        {
            Hide();
        }

        void Hide()
        {
            arrowEnabled = false;
            arrowImage.enabled = false;

            if (homeImage != null)
            {
                homeImage.enabled = false;
            }
            
            if (exclamationText != null)
            {
                exclamationText.enabled = false;
            }

            for (int i = 0; i < additionalImagesToDisable.Length; i++)
            {
                additionalImagesToDisable[i].gameObject.SetActive(false);
            }
        }

        public static Vector2 WorldToScreenPointProjected(Camera camera, Vector3 worldPos)
        {
            Vector3 camNormal = camera.transform.forward;
            Vector3 vectorFromCam = worldPos - camera.transform.position;
            float camNormDot = Vector3.Dot(camNormal, vectorFromCam);
            if (camNormDot <= 0)
            {
                // we are behind the camera forward facing plane, project the position in front of the plane
                Vector3 proj = (camNormal * camNormDot * 1.01f);
                worldPos = camera.transform.position + (vectorFromCam - proj);
            }

            return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
        }

        private void RotatePointerTowardsTargetPosition()
        {
            Vector3 toPosition = targetPosition.position;
            Vector3 fromPosition = Camera.main.transform.position;
            fromPosition.z = 0f;
            Vector3 dir = (toPosition - fromPosition).normalized;

            float angle = GetAngleFromVectorFloat(dir);
            arrow.localEulerAngles = new Vector3(0, 0, angle);
        }

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            return angle;
        }
    }
}

