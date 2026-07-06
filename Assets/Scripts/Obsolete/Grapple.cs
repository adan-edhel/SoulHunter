using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrappleState
{
    Inactive,
    Aiming,
    Yank,
    Climb,
    Throw,
    Thrown,
    Retrieve,
    Hooked
}

public class Grapple : MonoBehaviour
{
    Rigidbody2D _rb;
    Collider2D _Coll;

    public GrappleState g_State;

    [SerializeField] private GameObject Reticle;
    [SerializeField] private GameObject Player;
    [SerializeField] private int throwStrength;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _Coll = GetComponent<Collider2D>();
        g_State = GrappleState.Inactive;
    }
    void Update()
    {
        GrappleSwitch();
    }

    void GrappleSwitch()
    {
        switch (g_State)
        {
            case GrappleState.Inactive:
                transform.position = Player.transform.position;
                _Coll.enabled = false;
                _rb.gravityScale = 0;
                break;
            case GrappleState.Aiming:
                transform.position = Player.transform.position;

                break;
            case GrappleState.Throw:
                //Throw Hook
                _rb.AddForce((Reticle.transform.position - transform.position) * throwStrength);
                _rb.gravityScale = 1;
                _Coll.enabled = true;
                g_State = GrappleState.Thrown;
                break;
            case GrappleState.Thrown:

                break;
            case GrappleState.Retrieve:
                _rb.constraints = RigidbodyConstraints2D.None;
                _rb.velocity = Vector3.zero;
                g_State = GrappleState.Inactive;
                break;
            case GrappleState.Hooked:
                _rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                _rb.velocity = Vector3.zero;
                break;
            case GrappleState.Climb:

                break;
            case GrappleState.Yank:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Environment")
        {
            g_State = GrappleState.Hooked;
        }
    }
}
