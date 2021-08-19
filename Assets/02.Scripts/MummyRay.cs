using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MummyRay : Agent
{
    Transform tr;
    Rigidbody rb;
    StageManager stagemanager;
    public float moveSpeed = 1.5f;
    public float turnSpeed = 200f;

    Renderer floorRd;
    public Material goodMt, badMt;
    Material originMt;


    public override void Initialize()
    {
        MaxStep = 5000;
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        stagemanager = tr.parent.GetComponent<StageManager>();
        floorRd = tr.parent.Find("Floor").GetComponent<Renderer>();
        originMt = floorRd.material;
    }

    IEnumerator IEReverMaterial(Material changedMt)
    {
        floorRd.material = changedMt;
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }

    //학습이 시작될 때마다 자동적으로 실행됨
    public override void OnEpisodeBegin()
    {
        stagemanager.InitStage();
        rb.velocity = rb.angularVelocity = Vector3.zero;
        tr.localPosition = new Vector3(Random.Range(-20, 20), 0.05f, Random.Range(-20, 20));
        tr.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        //Debug.Log($"[0]={action[0]},[1] = {action[1]}");

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        //branch 0
        switch (action[0])
        {
            case 1: dir =  tr.forward; break;
            case 2: dir = -tr.forward; break;
            default:
                break;
        }
        //branch 1
        switch (action[1])
        {
            case 1: rot = -tr.up; break; //왼
            case 2: rot =  tr.up; break; //오
            default:
                break;
        }

        tr.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rb.AddForce(dir * moveSpeed, ForceMode.VelocityChange);

        AddReward(-1 / (float)MaxStep);
    }
    
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions.Clear();

        //branch 0 이동 전,후진,정지 0,1,2 : size 3
        if (Input.GetKey(KeyCode.W))
        {
            actions[0] = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            actions[0] = 2;
        }

        //branch 1 회전 정지, 왼,오른쪽 회전 : size 3
        if (Input.GetKey(KeyCode.A))
        {
            actions[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            actions[1] = 2;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("GOOD_ITEM"))
        {
            StartCoroutine("IEReverMaterial", goodMt);
            Destroy(collision.gameObject);
            AddReward(+1.0f);
            rb.velocity = rb.angularVelocity = Vector3.zero;
        }
        if (collision.collider.CompareTag("BAD_ITEM"))
        {
            StartCoroutine("IEReverMaterial", badMt);
            AddReward(-1.0f);
            EndEpisode();
        }
        if (collision.collider.CompareTag("WALL"))
        {
            AddReward(-0.1f);
        }
    }
}
