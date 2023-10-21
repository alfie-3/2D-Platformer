using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer spriteRenderer;
    WalkerAudioHandler audioHandler;

    //Input
    PlayerInput input;

    [Header("Movement Variables")]
    [SerializeField] float movementDamping;
    [SerializeField] float movementSpeed;
    public bool MovementEnabled { get; private set; } = true;
    Vector2 inputVector;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpTime;
    [Space]
    [SerializeField] float fallGravityMultiplier;
    [Space]
    [SerializeField] float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private bool isJumping;
    private bool jumpReady;
    private float jumpTimeCounter;
    private float gravityScale;

    [Header("Forces")]
    [SerializeField] float forceDecayRate;
    Vector2 outsideForces;

    [Header("Ground Check")]
    [SerializeField] Vector3 groundCheckAdjustment;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayers;

    // Start is called before the first frame update
    void Awake()
    {
        audioHandler = GetComponent<WalkerAudioHandler>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        gravityScale = rb.gravityScale;

        UpdateAnim();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    public void SetMovementEnabled(bool value) => MovementEnabled = value;

    void Update()
    {
        DecayOutsideForce();

        UpdateAnim();

        inputVector.x = input.actions["Movement"].ReadValue<Vector2>().normalized.x;

        #region coyote time
        if (IsGrounded())
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;
        #endregion

        #region Fall Gravity Multiplier

        if (rb.velocity.y < 0)
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        else
            rb.gravityScale = gravityScale;

        #endregion

        Jump();
    }

    private void Move()
    {
        if (!MovementEnabled)
            inputVector.x = 0;

        inputVector.y = 0;

        rb.velocity = new Vector2(inputVector.normalized.x * movementSpeed, rb.velocity.y) + outsideForces;
    }

    public void AddForce(Vector2 force) => outsideForces += force;
    public void ResetAllForces() => outsideForces = Vector2.zero;

    private void DecayOutsideForce()
    {
        outsideForces = Vector3.Lerp(outsideForces, Vector3.zero, forceDecayRate * Time.deltaTime);
    }

    void Jump()
    {
        bool jumpKeyHeld = input.actions["Jump"].IsPressed();

        if (!MovementEnabled)
            return;

        if (coyoteTimeCounter > 0 == true && !isJumping && jumpKeyHeld && jumpReady)
        {
            anim.SetTrigger("Jump");
            isJumping = true; jumpReady = false;
            jumpTimeCounter = jumpTime;
            rb.velocity = (Vector2.up * jumpForce);
            audioHandler.PlayJumpSound();
        }

        //makes you jump higher when you hold down space
        if (jumpKeyHeld && isJumping == true)
        {

            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }

        }
        else if (!jumpKeyHeld && isJumping)
        {
            isJumping = false;
            coyoteTimeCounter = 0;
        }

        if (IsGrounded() && !jumpKeyHeld)
            jumpReady = true;
    }

    void UpdateAnim()
    {
        anim.SetBool("HasInput", input.actions["Movement"].ReadValue<Vector2>().x != 0);
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("Falling", coyoteTimeCounter < 0);
        anim.SetBool("Grounded", IsGrounded());

        if (inputVector.x > 0)
            spriteRenderer.flipX = false;
        else if (inputVector.x < 0)
            spriteRenderer.flipX = true;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position - groundCheckAdjustment, checkRadius, groundLayers);
    }

    private void OnDrawGizmos()
    {
        if (Physics2D.OverlapCircle(transform.position - groundCheckAdjustment, checkRadius, groundLayers))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position - groundCheckAdjustment, checkRadius);
    }
}
