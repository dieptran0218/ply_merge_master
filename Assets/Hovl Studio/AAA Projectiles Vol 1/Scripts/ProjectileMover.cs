using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public GameObject hit;
    public GameObject flash;
    public GameObject[] Detached;

    [HideInInspector] public Pokemon _target;
    private float scale;
    public void SetupTarget(Pokemon tgr, float scale)
    {
        _target = tgr;
        this.scale = scale;
    }

    void OnEnable()
    {
        if (flash != null)
        {
            var flashInstance = SimplePool.Spawn(flash, transform.position, Quaternion.identity);
            flashInstance.transform.localScale = scale * Vector3.one;
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Timer.Schedule(this, 4f, () =>
                {
                    SimplePool.Despawn(flashInstance);
                });
            }
        }
	}

    //https ://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    public void OnCollision(Vector3 collision)
    {
        //Lock all axes movement and rotation
        var contact = collision;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact);
        Vector3 pos = contact;

        if (hit != null)
        {
            var hitInstance = SimplePool.Spawn(hit, pos, rot);
            hitInstance.transform.localScale = scale * Vector3.one;
            if (UseFirePointRotation) { hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0); }
            else if (rotationOffset != Vector3.zero) { hitInstance.transform.rotation = Quaternion.Euler(rotationOffset); }
            else { hitInstance.transform.LookAt(contact + contact); }

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Timer.Schedule(this, hitPs.main.duration, () =>
                {
                    SimplePool.Despawn(hitInstance);
                });
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Timer.Schedule(this, hitPsParts.main.duration, () =>
                {
                    SimplePool.Despawn(hitInstance);
                });
            }
        }
        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
            }
        }

        SimplePool.Despawn(gameObject);
    }

    public void Destroy()
    {
        SimplePool.Despawn(gameObject);
    }

    public void PreLoad()
    {

    }
}
