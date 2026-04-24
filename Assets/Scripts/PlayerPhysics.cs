using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public Rigidbody RB;

    public LayerMask layerMask;

    public Vector3 horizontalVelocity => Vector3.ProjectOnPlane(RB.linearVelocity, RB.transform.up);

    public Vector3 verticalVelocity => Vector3.Project(RB.linearVelocity, RB.transform.up);

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
        Ground();

        Snap();

        Move();
        
        if (!ground)
            Gravity();
    }

    // Move

    [SerializeField] float speed;

    void Move()
    {
        RB.linearVelocity = (Vector3.right * Input.GetAxis("Horizontal") * speed) + (Vector3.forward * Input.GetAxis("Vertical") * speed)
            + verticalVelocity;
    }

    // Gravity
    
    [SerializeField] float gravity;

    void Gravity()
    {
        RB.linearVelocity -= Vector3.up * gravity * Time.deltaTime;
    }

    // Ground

    [SerializeField] float groundDistance;

    Vector3 normal;

    bool ground;

    void Ground()
    {
        ground = Physics.Raycast(RB.worldCenterOfMass, -RB.transform.up, out RaycastHit hit, groundDistance, layerMask, QueryTriggerInteraction.Ignore);

        normal = ground ? hit.normal : Vector3.up;
    }

    // Snap

    void Snap()
    {
        transform.up = normal;
    }
}
