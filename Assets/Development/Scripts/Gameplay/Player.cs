using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private Transform handTransform = null;

    [SerializeField]
    private Transform checkBoxTransform = null;

    [SerializeField]
    private float moveSpeed = 1;

    private const string HorizontalAxisName = "Horizontal";
    private const string VerticalAxisName = "Vertical";

    [Rpc(SendTo.Server)]
    private void CatchBoxRpc()
    {
        if (handTransform is null)
            throw new System.Exception("HandTranmsform is not init");

        if (checkBoxTransform == null || checkBoxTransform is null)
            return;

        checkBoxTransform.parent = checkBoxTransform.parent is null ? handTransform : null;
    }

    private void Update()
    {
        if (IsServer || !IsOwner) return;

        Vector3 moveDirection = new Vector3(Input.GetAxis(HorizontalAxisName), 0, Input.GetAxis(VerticalAxisName));

        float acceleration = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;

        if (Input.GetKeyDown(KeyCode.E))
        {
            CatchBoxRpc();
        }

        transform.position += moveDirection * moveSpeed * acceleration * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;

        if (checkBoxTransform == null)
        {
            CheckboxInteractable checkbox;
            if (other.TryGetComponent(out checkbox))
            {
                checkBoxTransform = checkbox.transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer)
            return;

        CheckboxInteractable checkbox = null;
        if (other.TryGetComponent(out checkbox))
        {
            if (checkBoxTransform == checkbox.transform)
            {
                checkBoxTransform.parent = null;
                checkBoxTransform = null;
            }
        }
    }
}