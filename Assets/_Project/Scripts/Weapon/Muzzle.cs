using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class Muzzle : MonoBehaviour
    {
        private float PlayOneTime = 0.2f;
        
        public void PlayOne()
        {
            gameObject.SetActive(true);
            StartCoroutine(Stop(PlayOneTime));
        }

        private IEnumerator Stop(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}