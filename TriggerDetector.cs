using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace IceWind
{
    public class TriggerDetector : MonoBehaviour
    {
        public IceWindController controller;
        public Collider collider;

        public void Start()
        {
            collider = gameObject.GetComponent<Collider>();
            collider.enabled = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            controller.CollisionDetected(other);
        
        }

        private void OnTriggerExit(Collider other)
        {
            controller.CollisionExit(other);
        }

    }
}
