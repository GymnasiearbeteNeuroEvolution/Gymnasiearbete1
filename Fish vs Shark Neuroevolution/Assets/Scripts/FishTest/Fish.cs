using System;
using SharpNeat.Phenomes;
using UnityEngine;

public class Fish : UnitController
{
    private GameObject shark;
    private float _fitness = 0;
    private bool isHit = false;
    private bool isGrounded = false;
    bool IsRunning;
    private IBlackBox box;
    RaycastHit hit;
    public bool StopCalc = false;
    private int WallHits = 0;
    Rigidbody rigid;
    bool jump = false;
    int timesJumped = 0;

    #region Constructors
    protected Fish()  {  }
    #endregion
    #region Properties

    private bool IsWithinBounds()
    {
        return CameraManager.IsWithinCameraBounds(this.gameObject.transform.position, FishSettings.Instance.FishHorizontalPadding, FishSettings.Instance.FishVerticalPadding);
    }
    public float Fitness
    {
        get { return _fitness; }
    }
    #endregion

    void Awake()
    {
        //shark = GameObject.FindGameObjectWithTag("Shark");
        rigid = gameObject.GetComponent<Rigidbody>();
    }
    void Start()
    {

    }

    void FixedUpdate()
    {
        if (IsRunning)
        {
            float rightSensor = 0;
            float topRightSensor = 0;
            float bottomRightSensor = 0;
            float topSensor = 0;
            float bottomSensor = 0;
            // Front sensor
            if (HitsTag("Wall", transform.position + new Vector3(1, 0, 0)))
            {
                rightSensor = 1 - hit.distance / FishSettings.Instance.SensorRange;
            }
            if (HitsTag("Wall", transform.position + new Vector3(1, 0.5f, 0)))
            {
                topRightSensor = 1 - hit.distance / FishSettings.Instance.SensorRange;
            }
            if (HitsTag("Wall", transform.position + new Vector3(1,-0.5f, 0)))
            {
                bottomRightSensor = 1 - hit.distance / FishSettings.Instance.SensorRange;
            }
            if (HitsTag("Wall", transform.position + new Vector3(0, 1, 0)))
            {
                topSensor = 1 - hit.distance / FishSettings.Instance.SensorRange;
            }
            if (HitsTag("Wall", transform.position + new Vector3(0, -1, 0)))
            {
                bottomSensor = 1 - hit.distance / FishSettings.Instance.SensorRange;
            }


            ISignalArray inputArr = box.InputSignalArray;
            inputArr[0] = rightSensor;
            inputArr[1] = topRightSensor;
            inputArr[2] = bottomRightSensor;
            //inputArr[3] = topSensor;
            inputArr[3] = bottomSensor;
            //inputArr[5] = transform.position.x;
            //inputArr[6] = transform.position.y;

            box.Activate();

            ISignalArray outputArr = box.OutputSignalArray;

            //var turn = (float)outputArr[0] * 2 - 1;
            var moveX = (float)outputArr[0];
            var jumpValue = (float)outputArr[1];
            if (jumpValue > 0.99)
                jump = true;
            else
                jump = false;
            //var angle = turn * FishSettings.Instance.TurnSpeed * Time.deltaTime;
            if(isGrounded)
                rigid.AddForce(moveX * FishSettings.Instance.FishSpeed ,0 ,0);
            if (jump == true && isGrounded)
            {
                rigid.velocity = new Vector3(moveX * FishSettings.Instance.FishSpeed * Time.deltaTime, FishSettings.Instance.JumpSpeed, 0);
                timesJumped++;
            }
            //transform.Rotate(new Vector3(0, 0, turn));
            //transform.Translate(new Vector3(1,1,0) * move);
            /*if(!isHit && IsWithinBounds())
            {
                _fitness += Time.deltaTime;
            }
            else
            {
                StopCalc = true;
            }*/

        }
    }

    private bool HitsTag(string tag, Vector3 v)
    {
        if (Physics.Raycast(transform.position + transform.right * 1.1f, transform.TransformDirection(v.normalized), out hit, FishSettings.Instance.SensorRange))
        {
            if (hit.collider.tag.Equals(tag))
            {
                return true;
            }
        }
        return false;
    }

    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        IsRunning = true;
    }

    public override float GetFitness()
    {
        if (gameObject.transform.position.x < 0)
            return 0;
        _fitness = (gameObject.transform.position.x/10)- (WallHits*5);
        if (_fitness < 0)
            return 0;
        return (float)Math.Round(_fitness, 2);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            WallHits++;
        }
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
    }


    //consider when character is jumping .. it will exit collision.
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = false;
        }
    }
}
    