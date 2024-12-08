using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class Crosshair : MonoBehaviour
    {
        [FormerlySerializedAs("crosshair")] public GameObject crosshairImgObject;
        private RectTransform crosshairRectTransorm;
        public float defaultSize = 5f;
        public float walkingSize = 10f;
        [FormerlySerializedAs("player")] [FormerlySerializedAs("playerMovement")] public global::GamePlayer gamePlayer;
        public float speedThreshold = 0.1f; // Порог скорости для определения ходьбы
        public Vector3 prevPosition;

        private void OnEnable()
        {
            crosshairRectTransorm = crosshairImgObject.GetComponent<RectTransform>();
        }

        private void Update()
        {
           HandleResizeOnMove();
           HandleAim();
        }

        private void HandleResizeOnMove()
        {
            Vector3 currentPosition = gamePlayer.transform.position;
            float speed = Vector3.Distance(currentPosition, prevPosition) / Time.deltaTime;
            prevPosition = currentPosition;

            // Изменение размера курсора прицела в зависимости от скорости движения
            if (speed > speedThreshold)
            {
                crosshairRectTransorm.sizeDelta = new Vector2(walkingSize, walkingSize);
            }
            else
            {
                crosshairRectTransorm.sizeDelta = new Vector2(defaultSize, defaultSize);
            }
        }

        private void HandleAim()
        {
            if (Input.GetButtonDown("Fire2")) // Правая кнопка мыши для прицеливания
            {
               crosshairImgObject.SetActive(!crosshairImgObject.activeSelf);
            }
        }
    }
}