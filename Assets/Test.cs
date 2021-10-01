using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Vector3 posistion;
    private Quaternion rotation;
    [SerializeField] private Vector3 velocity;

    string velocityString;

    private Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        print(rigidBody.drag);
        StartCoroutine(ApplyPhysics());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ApplyPhysics()
    {
        while (true)
        {
            posistion = transform.position;
            rotation = transform.rotation;
            velocity = rigidBody.velocity;
            yield return new WaitForSecondsRealtime(0.1f);
            print("OK");
            transform.position = posistion;
            transform.rotation = rotation;
            rigidBody.velocity = velocity;
        }
    }
}
