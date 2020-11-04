using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aircraft
{

    public class Rotate : MonoBehaviour
    {

        [Tooltip("Speed at wich to rotate")]
        public Vector3 rotateSpeed;

        public bool randomized;

        // Start is called before the first frame update
        void Start()
        {
            if (randomized) transform.Rotate(rotateSpeed * Random.Range(0f, 350f));
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);
        }
    }

}