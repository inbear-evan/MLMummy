using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyAgent : Agent
{
    // 1. observation
    // 2. Policy Action
    // 3. Reward

    Transform tr;
    Transform targetTr;
    Rigidbody rd;
    float speed = 50;
    public Material goodMt, badMt;
    Material originMt;
    new Renderer renderer;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        targetTr = tr.parent.Find("Target").GetComponent<Transform>();
        rd = GetComponent<Rigidbody>();
        renderer = tr.parent.Find("Floor").GetComponent<Renderer>();
        originMt = renderer.material;
    }

    public override void OnEpisodeBegin()
    {
        rd.velocity = Vector3.zero;
        rd.angularVelocity = Vector3.zero;

        tr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.05f, Random.Range(-4.0f, 4.0f));
        targetTr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.55f, Random.Range(-4.0f, 4.0f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition); // x,y,z
        sensor.AddObservation(tr.localPosition);       // x,y,z
        sensor.AddObservation(rd.velocity.x);          // x
        sensor.AddObservation(rd.velocity.z);          // z
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.ContinuousActions;
        Debug.Log($"[0] = {action[0]} , [1] = {action[1]}");

        Vector3 dir = (Vector3.forward * action[0] + Vector3.right * action[1]);
        rd.AddForce(dir.normalized * speed);
        tr.forward = dir.normalized;
        //지속적인 움직임을 위한 패널티
        SetReward(-0.001f);
    }

    // Test
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //연속 Input.GetAxis() -> -1 ~ 1
        //이산 Input.GetAxisRaw() -> -1,0,1

        var action = actionsOut.ContinuousActions;
        action[0] = Input.GetAxis("Vertical");
        action[1] = Input.GetAxis("Horizontal");
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DEAD_ZONE"))
        {
            StartCoroutine("IEReverMaterial", badMt);
            SetReward(-1.0f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("TARGET"))
        {
            StartCoroutine("IEReverMaterial", goodMt);
            SetReward(+1.0f);
            EndEpisode();
        }
    }

    IEnumerator IEReverMaterial(Material changedMt)
    {
        renderer.material = changedMt;
        yield return new WaitForSeconds(0.2f);
        renderer.material = originMt;
    }
}
