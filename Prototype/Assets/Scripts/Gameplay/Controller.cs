using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    private Vector2 Input;

    public float Speed = 5f;

    private float Gravity;
    private float GravityConstant = 9.5f;

    public float TurnSmoothTime = .1f;
    private float TurnSmoothVelocity;

    public Transform cam;

    private CharacterController cc;

    [SerializeField]
    GameManager Manager;

    [SerializeField]
    Animator Anim;

    public float attackTime = 4f;
    private float currentAttackTime = 0f;

    public GameObject ActivatePowerUp;
    public TextMeshProUGUI TimeRemaining;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        TimeRemaining.text = ((int)Mathf.Clamp(Manager.startTime - Time.time, 0, 65)).ToString() + " Seconds Remaining.";

        Vector3 dir = new Vector3(Input.x, 0, Input.y);

        if (dir.magnitude >= .1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            dir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        if (Anim)
            Anim.SetFloat("WalkSpeed", dir.magnitude);

        if (!cc.isGrounded)
        {
            Gravity += GravityConstant;
        }
        else
        {
            Gravity = GravityConstant;
        }

        dir -= new Vector3(0, Gravity, 0);

        dir.Normalize();

        cc.Move(dir * Speed * Time.deltaTime);

        if (Manager.isPlaying && gameObject.tag != "Target" && Time.time > currentAttackTime)
        {
            gameObject.tag = "Target";
            ActivatePowerUp.SetActive(false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Input = context.ReadValue<Vector2>().normalized;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!Manager.isPlaying) return;

        if (other.tag == "Boid")
        {
            Manager.Died();
        }
        else if (other.tag == "Vicsek")
        {
            Manager.Died();
        }
        else if (other.tag == "Control")
        {
            Manager.Died();
        }
        else if (other.tag == "Upgrade")
        {
            ActivatePowerUp.SetActive(true);
            Destroy(other.gameObject);

            gameObject.tag = "Predator";
            currentAttackTime = Time.time + attackTime;
        }
    }
}
