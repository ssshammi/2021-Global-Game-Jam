using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    // Root of the armature
    public Joint m_root;

    // End effector
    public Joint m_end;

    public GameObject submarine;
    private Rigidbody2D subrb;

    public InputManager controls;
    public Vector3 mousePos;

    public float m_threshold = 0.05f;
    private float m_rate = 10.0f;
    public int m_steps = 20;

    private bool clawMode = false;

    private void Awake()
    {
        controls = new InputManager();
        controls.Player.Mouse.performed += ctx => TargetMouse(ctx.ReadValue<Vector2>());
        controls.Player.Claw.started += ctx => EnableClawMode();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        subrb = submarine.GetComponent<Rigidbody2D>();        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target;
        if(clawMode == false)
        {
            target = new Vector3(subrb.position.x, subrb.position.y, 1);
        }
        else
        {
            target = mousePos;
        }

        for (int i = 0; i < m_steps; i++)
        {
            if (GetDistance(m_end.transform.position, target) > m_threshold)
            {
                Joint current = m_root;
                while (current != null)
                {
                    float slope = CalculateSlope(current);
                    current.Rotate(-slope * m_rate);
                    current = current.GetChild();
                }

            }
        }
        
    }

    float CalculateSlope(Joint _joint)
    {
        Vector3 target;
        if (clawMode == false)
        {
            target = new Vector3(subrb.position.x, subrb.position.y, 1);
        }
        else
        {
            target = mousePos;
        }

        float deltaTheta = 0.01f;
        float distance1 = GetDistance(m_end.transform.position, target);

        _joint.Rotate(deltaTheta);

        float distance2 = GetDistance(m_end.transform.position, target);

        _joint.Rotate(deltaTheta);

        return (distance2 - distance1) / deltaTheta;
    }

    float GetDistance(Vector2 _point1, Vector2 _point2)
    {
        return Vector2.Distance(_point1, _point2);
    }

    void TargetMouse(Vector2 position)
    {
        Vector2 worldPos;
        worldPos = Camera.main.ScreenToWorldPoint(position);
        mousePos = new Vector3(worldPos.x, worldPos.y, 1);
    }

    void EnableClawMode()
    {
        clawMode = !clawMode;
    }
}
