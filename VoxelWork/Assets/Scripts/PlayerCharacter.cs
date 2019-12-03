using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RequireComponent(typeof(Rigidbody))]
public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]private float _movementSpeed = 100f;
    private Rigidbody rb;
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;
        

        pos += vert * _movementSpeed * Time.deltaTime * transform.forward;
        pos += horz * _movementSpeed * Time.deltaTime * transform.right;
        transform.position = pos;

        transform.position += pos + new Vector3(0, -2, 0);

        if (Input.GetKey(KeyCode.Space))
        {
            //rb.AddForce(new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), ForceMode.Force);
            pos.y = pos.y + 10 * Time.deltaTime;
        }
    }
   
}
