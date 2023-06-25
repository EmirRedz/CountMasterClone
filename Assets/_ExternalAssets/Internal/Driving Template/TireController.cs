using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace LaynarGames
{
    public class TireController : MonoBehaviour
    {
        public Transform[] frontTireContainers;
        public Transform[] actualWheels;
        float turningRate = 300;
        public bool autoTurnWheels;

        [SerializeField] private PlayerTopDownDrivingController playerDrivingController;
        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < frontTireContainers.Length; i++)
            {
                float t = Mathf.InverseLerp(-90, 90, -playerDrivingController.steeringAngle);
                float newRotY = Mathf.Lerp(-30, 30, t);
                Vector3 newEuler = frontTireContainers[i].transform.localEulerAngles;
                newEuler.y = newRotY;

                Quaternion targetRot = Quaternion.Euler(newEuler);
                frontTireContainers[i].localRotation = Quaternion.RotateTowards(frontTireContainers[i].localRotation, targetRot, turningRate * Time.deltaTime);

               
            }

            for (int i = 0; i < actualWheels.Length; i++)
            {
                if (autoTurnWheels)
                {
                    actualWheels[i].transform.Rotate(new Vector3(10, 0, 0));
                }
                else
                {
                    actualWheels[i].transform.Rotate(new Vector3(playerDrivingController.forwardVelocity, 0, 0));
                }
            }

        }
    }
}