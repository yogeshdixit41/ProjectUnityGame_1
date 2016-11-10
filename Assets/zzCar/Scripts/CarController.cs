using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple Car Controller based on Unity's official tutorial, but cleaned up
/// for my purposes.
/// </summary>
public class CarController : MonoBehaviour
{
    public WheelCollider m_leftWheel;
    public WheelCollider m_rightWheel;
    public float m_maxMotorTorque = 2000;
    public float m_maxSteeringAngle = 45;
    public float m_brakeTorque = 100;
    public GameObject optionA;
    public GameObject optionB;
    public GameObject optionC;
    public GameObject optionD;
    public Text questionTextObject;
    

    private BoxCollider m_carRoot;
    private Vector3 m_lastPosition;
    private Vector3 m_curPosition;
    private int m_recentWaypointIndex;
    private int m_lapCount;

    private ArrayList availablePositionsArray;
    private ArrayList questionList;
    private int playerHealth;
    private int questionNumber;
    private string currentAnswer;



    /// <summary>
    /// Called once.
    /// </summary>
    public void Awake()
    {
        m_carRoot = GetComponent<BoxCollider>();

        // add 2 available positions on the scene for character popping

        availablePositionsArray = new ArrayList();
        questionList = new ArrayList();
        playerHealth = 100;
        questionNumber = 0;

        availablePositionsArray.Add(new Vector3(-14, 0.206f, 5.98f));
        availablePositionsArray.Add(new Vector3(-12.264f, 0.206f, -12.48f));

        //Debug.Log(availablePositionsArray.Count);

        createQuestion();
        displayQuestion();
    }

    /// <summary>
    /// Called once per physics update.
    /// </summary>
    public void FixedUpdate()
    {
        float motor = m_maxMotorTorque * Input.GetAxisRaw("Vertical");
        float steering = m_maxSteeringAngle * Input.GetAxis("Horizontal");

        if (motor == 0)
        {
            m_leftWheel.brakeTorque = m_brakeTorque;
            m_rightWheel.brakeTorque = m_brakeTorque;
        }
        else
        {
            m_leftWheel.brakeTorque = 0;
            m_rightWheel.brakeTorque = 0;
        }

        m_leftWheel.steerAngle = steering;
        m_leftWheel.motorTorque = motor;
        _ApplyTransform(m_leftWheel);
        m_rightWheel.steerAngle = steering;
        m_rightWheel.motorTorque = motor;
        _ApplyTransform(m_rightWheel);

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
        //Vector3 delta = m_lastPosition - m_curPosition;
        //delta.y = 0; //< This isn't strictly right, we should project away the car's normal.
        //float speedMetersPerSec = delta.magnitude / Time.deltaTime;
        
        GUI.color = Color.black;
        GUI.Label(new Rect(40, 40, 200, 200),
                  string.Format("Health: {0}", playerHealth));
        //GUI.Label(new Rect(40, 60, 200, 200),
                  //string.Format("Lap: {0}", m_lapCount));
    }

    public void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log(other.name);

        //Create new instance of gameobject
        if (other.name != "Magic Box")
        {
            
            GameObject newInstance = new GameObject();
            Vector3 randomVectorPosition = (Vector3)availablePositionsArray[0];
            availablePositionsArray.RemoveAt(0);

            Debug.Log("Random: " + randomVectorPosition);
            newInstance = (GameObject)Instantiate(other.gameObject, randomVectorPosition, Quaternion.identity);

            switch (other.name)
            {
                case "OptionA":
                    checkAnswer("A");
                    displayQuestion();
                    //optionA.PlayAnimation();

                    availablePositionsArray.Add(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z));
                    Destroy(other.gameObject);
                    newInstance.name = "OptionA";
                    newInstance.SetActive(true);
                    break;
                case "OptionB":
                    checkAnswer("B");
                    displayQuestion();
                    //optionA.PlayAnimation();
                    availablePositionsArray.Add(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z));
                    Destroy(other.gameObject);
                    newInstance.name = "OptionB";
                    newInstance.SetActive(true);

                    break;
                case "OptionC":
                    checkAnswer("C");
                    displayQuestion();
                    availablePositionsArray.Add(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z));
                    Destroy(other.gameObject);
                    newInstance.name = "OptionC";
                    newInstance.SetActive(true);
                    break;
                case "OptionD":
                    checkAnswer("D");
                    displayQuestion();
                    availablePositionsArray.Add(new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z));
                    Debug.Log("Before destroy");
                    Destroy(other.gameObject);
                    newInstance.name = "OptionD";
                    newInstance.SetActive(true);
                    break;
                default:
                    Debug.Log("Before destroy" + other.name);
                    break;
            }
        }

        else if (other.name == "Magic Box")
        {
            this.gameObject.transform.position = new Vector3(-0.15f, 2.59f, 3.71f);
        }

            
        /*WaypointIndex waypoint = other.GetComponent<WaypointIndex>();
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
        }*/
    }

    /// <summary>
    /// Apply the wheel's physics transform to all its children.
    /// </summary>
    /// <param name="wheel">Wheel to apply transform to.</param>
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

    public void createQuestion()
    {
        Question newQuestion = new Question();
        newQuestion.questionText = "Which of these fruits is mentioned in PPAP song?";
        newQuestion.answer = "D";
        newQuestion.optionA = "Banana";
        newQuestion.optionB = "Watermelon";
        newQuestion.optionC = "Grapes";
        newQuestion.optionD = "Apple";

        Question newQuestion1 = new Question();
        newQuestion1.questionText = "100 + 109 ?";
        newQuestion1.answer = "C";
        newQuestion1.optionA = "210";
        newQuestion1.optionB = "208";
        newQuestion1.optionC = "Two hundred and nine";
        newQuestion1.optionD = "Two hundre and five";

        Question newQuestion2 = new Question();
        newQuestion2.questionText = "333 * 33 ?";
        newQuestion2.answer = "C";
        newQuestion2.optionA = "9999";
        newQuestion2.optionB = "1089";
        newQuestion2.optionC = "10989";
        newQuestion2.optionD = "None";

        Question newQuestion3 = new Question();
        newQuestion3.questionText = "Best pizza place ?";
        newQuestion3.answer = "D";
        newQuestion3.optionA = "Pizza Hut";
        newQuestion3.optionB = "Papa John's";
        newQuestion3.optionC = "Pizza My Heart";
        newQuestion3.optionD = "Domino's";

        Question newQuestion4 = new Question();
        newQuestion4.questionText = "What's the color of your car?";
        newQuestion4.answer = "B";
        newQuestion4.optionA = "Black";
        newQuestion4.optionB = "Red";
        newQuestion4.optionC = "White";
        newQuestion4.optionD = "Green";

		Question newQuestion5 = new Question();
        newQuestion5.questionText = "Who was elected as the President of the USA in 2016?";
        newQuestion5.answer = "D";
        newQuestion5.optionA = "Hillary Clinton";
        newQuestion5.optionB = "Ted Cruz";
        newQuestion5.optionC = "Bernie Sanders";
        newQuestion5.optionD = "Donald Trump";

        Question newQuestion6 = new Question();
        newQuestion6.questionText = "What do you get when you heat a Barbie over a grill?";
        newQuestion6.answer = "A";
        newQuestion6.optionA = "Barbeque";
        newQuestion6.optionB = "Ken";
        newQuestion6.optionC = "Plastic";
        newQuestion6.optionD = "None of the above";

        Question newQuestion7 = new Question();
        newQuestion7.questionText = "What Game Engine was used to construct this game?";
        newQuestion7.answer = "C";
        newQuestion7.optionA = "Unreal";
        newQuestion7.optionB = "Frostbite";
        newQuestion7.optionC = "Unity";
        newQuestion7.optionD = "CryEngine";

        questionList.Add(newQuestion);
        questionList.Add(newQuestion1);
        questionList.Add(newQuestion2);
        questionList.Add(newQuestion3);
        questionList.Add(newQuestion4);
        questionList.Add(newQuestion5);
        questionList.Add(newQuestion6);
        questionList.Add(newQuestion7);
    }

    public void displayQuestion()
    {
        string formattedText = "";
        if (questionNumber < questionList.Count)
        {
            questionTextObject.text = "";
            Question questionObject = (Question)questionList[questionNumber];
            questionNumber++;
            formattedText = formattedText + questionObject.questionText + "\n";
            formattedText = formattedText + "A : " + questionObject.optionA + "  ";
            formattedText = formattedText + "B : " + questionObject.optionB + "\n  ";
            formattedText = formattedText + "C : " + questionObject.optionC + "  ";
            formattedText = formattedText + "D : " + questionObject.optionD;

            Debug.Log(formattedText);

            questionTextObject.text = formattedText;
            currentAnswer = questionObject.answer;
        }
        else
        {
            //next level
            questionTextObject.text = "Nice Job. Next Level";
            //load next scene
        }
        
    }

    private void checkAnswer(string playersAnswer)
    {
        if (!playersAnswer.Equals(currentAnswer))
        {
            if (playerHealth > 0)
            {
                playerHealth = playerHealth - 50;
                if (playerHealth <= 0)
                { 
                    Debug.Log("Gameover");
                    questionTextObject.text = "Game Over";
                    Destroy(this.gameObject);
                    SceneManager.LoadScene(0);
                }

            }
                
            //Debug.Log("Wrong answer");
        }
        else
        {
            questionTextObject.text = "Right Answer";
            Debug.Log("Right answer");
        }
    }



    private class Question
    {
        public string questionText;
        public string optionA;
        public string optionB;
        public string optionC;
        public string optionD;
        public string answer;

    }
}
