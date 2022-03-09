using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;


    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_jumpInput = false;

    private bool m_isGrounded;

    private List<Collider> m_collisions = new List<Collider>();

    private Camera mainCamera;
    [SerializeField]
    private bool CueStickEnter = false;
    [SerializeField]
    private bool CueStickCatch = false;

    [SerializeField] private GameObject cueStick;

    private Vector3 tableCameraPos = new Vector3(0, 3.5f, 0);
    private Vector3 tableCameraRot = new Vector3(90, 0, 0);

    private Vector3 stageCameraPos = new Vector3(0, 5.4f, -8.5f);
    private Vector3 stageCameraRot = new Vector3(45, 0, 0);

    private Vector3 startPos;
    private Vector3 endPos;

    private Vector3 startRot;
    private Vector3 endRot;

    private float cameraDistance;

    private float cameraTransTime = 1.0f;
    private float cameraTransCurrentTime;

    bool cameraTransState;

    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }


        cameraDistance = Vector3.Distance(stageCameraPos, tableCameraPos);

    }

    private void OnTriggerEnter(Collider other)
    {
        //キュー接触
        if (other.gameObject.tag == "Cue")
        {
            CueStickEnter = true;
        }

        //カメラ遷移
        if(other.gameObject.name == "CameraTransCollider")
        {
            startPos = Camera.main.transform.position;
            endPos = tableCameraPos;

            startRot = Camera.main.transform.rotation.eulerAngles;
            endRot = tableCameraRot;

            cameraTransState = true;
            cameraTransCurrentTime = 0;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        //キュー接触
        if (other.gameObject.tag == "Cue")
        {
            CueStickEnter = false;
        }


        //カメラ遷移
        if (other.gameObject.name == "CameraTransCollider")
        {
            startPos = Camera.main.transform.position;
            endPos = stageCameraPos;

            startRot = Camera.main.transform.rotation.eulerAngles;
            endRot = stageCameraRot;


            cameraTransState = true;
            cameraTransCurrentTime = 0;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }

        
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }

        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }

    }

    private void Update()
    {
        if (!m_jumpInput && Input.GetKey(KeyCode.Space))
        {
            m_jumpInput = true;
        }

        CueUpdate();
        CameraUpdate();
    }

    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

      
        DirectUpdate();

        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }

    
    
    private void CameraUpdate()
    {
        if(cameraTransState)
        {
            float speed = 5f;

            cameraTransCurrentTime += (Time.deltaTime * speed) / cameraTransTime;



            if(cameraTransCurrentTime >= 1.0f)
            {
                cameraTransState = false;
                cameraTransCurrentTime = 0;
                return;
            }

            Vector3 transPos = Vector3.Lerp(startPos, endPos, cameraTransCurrentTime);
            Vector3 transRot = Vector3.Slerp(startRot, endRot, cameraTransCurrentTime);

            Camera.main.transform.position = transPos;
            Camera.main.transform.rotation = Quaternion.Euler(transRot);


        }
    }

    private void CueUpdate()
    {
        //キュー取得


        if (Input.GetMouseButtonDown(0))
        {
            if (CueStickEnter)
            {
                m_animator.SetLayerWeight(2, 1);
                CueStickCatch = true;
                cueStick.transform.parent = this.transform;
                cueStick.transform.position = this.transform.position + new Vector3(0, 0.3f, 0) + this.transform.right * 0.2f;
                cueStick.transform.rotation = this.transform.rotation * Quaternion.Euler(90, -120, -90);
            }
        }
        else if(Input.GetMouseButton(0))
        {
            //if(CueStickCatch)
            //{
                
            //}
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (CueStickCatch)
            {
                m_animator.SetLayerWeight(2, 0);
                CueStickCatch = false;
                cueStick.transform.parent = null;
            }
        }


    }

    private void DirectUpdate()
    {
        //float v = Input.GetAxis("Vertical");
        //float h = Input.GetAxis("Horizontal");

        float v = 0;
        float h = 0;

        if (Input.GetKey(KeyCode.U))
            v = 1;
        else if (Input.GetKey(KeyCode.J))
            v = -1;

        if (Input.GetKey(KeyCode.H))
            h = -1;
        else if (Input.GetKey(KeyCode.K))
            h = 1;


        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }


       

        //JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
}
