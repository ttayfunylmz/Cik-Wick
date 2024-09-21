using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace OccaSoftware.OutlineObjects.Demo
{
    [AddComponentMenu("OccaSoftware/Outline Objects/Demo/")]
    public class FloatAndSpin : MonoBehaviour
    {
        Vector3 startPos;

        [SerializeField]
        float rotSpeed = 30f;

        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            float yOffset = (Mathf.PerlinNoise(transform.position.x, Time.time * 0.1f) - 0.5f) * 2.0f;
            transform.position = new Vector3(transform.position.x, startPos.y + yOffset, transform.position.z);
            yOffset *= rotSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(yOffset, yOffset * 0.5f, yOffset * 1.3f));
        }
    }
}
