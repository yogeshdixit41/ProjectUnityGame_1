using UnityEngine;

/// <summary>
/// Simple Car Controller based on Unity's official tutorial, but cleaned up
/// for my purposes.
/// </summary>
public class CarController1 : MonoBehaviour
{
    public WheelCollider m_leftWheel;
    public WheelCollider m_rightWheel;
    public WheelCollider r_leftWheel;
    public WheelCollider r_rightWheel;
    public float m_maxMotorTorque = 100;
    public float m_maxSteeringAngle = 45;
    public float m_brakeTorque = 100;

    private BoxCollider m_carRoot;
    private Vector3 m_lastPosition;
    private Vector3 m_curPosition;
    private int m_recentWaypointIndex;
    private int m_lapCount;

    /// <summary>
    /// Called once.
    /// </summary>
    public void Awake()
    {
        m_carRoot = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Called once per physics update.
    /// </summary>
    public void FixedUpdate()
    {
        float motor = m_maxMotorTorque * Input.GetAxisRaw("Vertical");
        float steering = m_maxSteeringAngle * Input.GetAxis("Horizontal");
        Debug.Log("Speed:" + motor);
        Debug.Log("streeing:" + steering);
        if (motor == 0)
        {
            m_leftWheel.brakeTorque = m_brakeTorque;
            m_rightWheel.brakeTorque = m_brakeTorque;
            r_leftWheel.brakeTorque = m_brakeTorque;
            r_rightWheel.brakeTorque = m_brakeTorque;
        }
        else
        {
            m_leftWheel.brakeTorque = 0;
            m_rightWheel.brakeTorque = 0;
            r_leftWheel.brakeTorque = 0;
            r_rightWheel.brakeTorque = 0;
        }

        m_leftWheel.steerAngle = steering;
        m_leftWheel.motorTorque = motor;
        _ApplyTransform(m_leftWheel);
        m_rightWheel.steerAngle = steering;
        m_rightWheel.motorTorque = motor;
        _ApplyTransform(m_rightWheel);
        r_leftWheel.steerAngle = 0;
        r_leftWheel.motorTorque = motor;
        _ApplyTransform(r_leftWheel);
        r_rightWheel.steerAngle = 0;
        r_rightWheel.motorTorque = motor;
        _ApplyTransform(r_rightWheel);

        // Prevent the car from flipping over.
        
        Vector3 euler = m_carRoot.transform.localEulerAngles;
        euler.z = 0;
        m_carRoot.transform.localEulerAngles = euler;
        

        // For speed display.
        m_lastPosition = m_curPosition;
        m_curPosition = m_carRoot.transform.position;
    }

    public void OnGUI()
    {
        Vector3 delta = m_lastPosition - m_curPosition;
        delta.y = 0; //< This isn't strictly right, we should project away the car's normal.
        float speedMetersPerSec = delta.magnitude / Time.deltaTime;
        
        GUI.color = Color.black;
        GUI.Label(new Rect(40, 40, 200, 200),
                  string.Format("Speed: {0}", Mathf.FloorToInt(speedMetersPerSec * 2.23694f)));
        GUI.Label(new Rect(40, 60, 200, 200),
                  string.Format("Lap: {0}", m_lapCount));
    }

    public void OnTriggerEnter(Collider other)
    {
        WaypointIndex waypoint = other.GetComponent<WaypointIndex>();
        if (waypoint == null)
        {
            return;
        }

        if (m_recentWaypointIndex + 1 == waypoint.m_index)
        {
            m_recentWaypointIndex = waypoint.m_index;
        }
        else if (m_recentWaypointIndex == WaypointIndex.INDEX_MAX && waypoint.m_index == 0)
        {
            m_recentWaypointIndex = 0;
            ++m_lapCount;
        }
    }

    /// <summary>
    /// Apply the wheel's physics transform to all its children.
    /// </summary>
    // <param name="wheel">Wheel to apply transform to.</param>
    private void _ApplyTransform(WheelCollider wheel)
    {
        Vector3 pos;
        Quaternion rot;
        wheel.GetWorldPose(out pos, out rot);
        foreach (Transform child in wheel.transform)
        {
            child.position = pos;
            child.rotation = rot;
        }
    }
}
