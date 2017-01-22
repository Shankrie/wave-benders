using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CreateCircle();

    }

    protected void CreateCircle()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.startLifetime = 10f;
        ParticleSystem.VelocityOverLifetimeModule vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;

        AnimationCurve curveZ = new AnimationCurve();
        float points = 10 * 10f;
        for (int i = 0; i < points; i++)
        {
            float newtime = i / (points - 1);
            float myvalue = 30;

            curveZ.AddKey(newtime, myvalue);
            Debug.Log(newtime);
        }
    }

        // Update is called once per frame
        void Update () {
		
	}
}
