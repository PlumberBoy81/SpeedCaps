using System.Collections;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public Rigidbody RB;

    public LayerMask layerMask;

    public Vector3 horizontalVelocity => Vector3.ProjectOnPlane(RB.linearVelocity, RB.transform.up);

    public Vector3 verticalVelocity => Vector3.Project(RB.linearVelocity, RB.transform.up);

    public float verticalSpeed => Vector3.Dot(RB.linearVelocity, RB.transform.up);

    // Update

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Jump();        
    }

    // Jump

    [SerializeField] float jumpForce;

    void Jump()
    {
        if (!ground) return;

        RB.linearVelocity = (Vector3.up * jumpForce)
            + horizontalVelocity;
    }

    // Fixed Update

    void FixedUpdate()
    {
        Move();
        
        if (!ground)
            Gravity();
            
        if (ground && verticalSpeed< RB.sleepThreshold)
            RB.linearVelocity = horizontalVelocity;

        StartCoroutine(LateFixedUpdateRoutine());

        IEnumerator LateFixedUpdateRoutine()
        {
            yield return new WaitForFixedUpdate();

            LateFixedUpdate();
        }
    }

    // Move

    [SerializeField] float speed;

    void Move()
    {
        RB.linearVelocity = Vector3.ProjectOnPlane((Vector3.right * Input.GetAxis("Horizontal") * speed) + (Vector3.forward * Input.GetAxis("Vertical") * speed), normal);
            + verticalVelocity;
    }

    // Gravity
    
    [SerializeField] float gravity;

    void Gravity()
    {
        RB.linearVelocity -= Vector3.up * gravity * Time.deltaTime;
    }

    // Late Fixed Update
    
    void LateFixedUpdate()
    {       
        Ground();

        Snap();

        if (ground)
            RB.linearVelocity = horizontalVelocity;
    }

    // Ground

    [SerializeField] float groundDistance;

    Vector3 point;

    Vector3 normal;

    bool ground;

    void Ground()
    {
        float maxDistance = Mathf.Max(RB.centerOfMass.y, 0) + (RB.sleepThreshold * Time.fixedDeltaTime);
        
        if (ground && verticalSpeed < RB.sleepThreshold)
            maxDistance += groundDistance;
        ground = Physics.Raycast(RB.worldCenterOfMass, -RB.transform.up, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);

        point = ground ? hit.point : RB.transform.position;

        normal = ground ? hit.normal : Vector3.up;
    }

    // Snap

    void Snap()
    {
        RB.transform.up = normal;

        Vector3 goal = point;

        Vector3 difference = goal - RB.transform.position;

        if (RB.SweepTest(difference, out _, difference.magnitude, QueryTriggerInteraction.Ignore)) return;

        RB.transform.position = goal;
    }
}
