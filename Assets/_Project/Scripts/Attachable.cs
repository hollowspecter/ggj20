using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attachable : Boopable
{
    protected new Rigidbody rigidbody;
    public Rigidbody Rigidbody { get => rigidbody; set => rigidbody = value; }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Boop(Tongue _tongue)
    {
        base.Boop(_tongue);

        // Attach to tongue.

        _tongue.AttachAttachable(this);
    }
}
