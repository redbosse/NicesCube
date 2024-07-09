using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class CellBase : NetworkBehaviour, ICell
{
    [SerializeField]
    private Collider cellCollider;

    [SerializeField]
    private UnityEvent<bool> changeStateEventHandler;

    private CellPool parentPool;

    private bool isLoaded = false;

    private int index = 0;

    private CheckboxInteractable saveCheckbox;

    public CellPool ParentPool { get => parentPool; set => parentPool = value; }

    [Rpc(SendTo.NotServer)]
    private void ChangeRpc(bool state)
    {
        Change(state);
    }

    [Rpc(SendTo.Server)]
    private void GetStateFromServerRpc()
    {
        ChangeRpc(isLoaded);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
            GetStateFromServerRpc();

        ActivateCollider();
    }

    private void ActivateCollider()
    {
        if (cellCollider is not null)
            cellCollider.enabled = IsServer;
    }

    private void Change(bool state)
    {
        isLoaded = state;

        changeStateEventHandler?.Invoke(state);

        LoadStateToParent(state);

        if (IsServer)
            ChangeRpc(state);
    }

    public void LoadStateToParent(bool state)
    {
        if (ParentPool is not null && index < ParentPool.CellMap.Length)
        {
            ParentPool.CellMap[index].State = state;
        }
    }

    public void SetIndex(int inputIndex)
    {
        index = inputIndex;
    }

    public void Activation()
    {
        Change(true);
    }

    public void Deactivation()
    {
        Change(false);
    }

    public void Reference()
    {
        if (cellCollider is not null)
            cellCollider.enabled = false;
    }

    public void RestoreDefault()
    {
        cellCollider.enabled = true;
        Deactivation();
    }

    public void SetParentPool(CellPool pool)
    {
        ParentPool = pool;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (saveCheckbox != null) return;

        CheckboxInteractable checkbox = null;
        if (other.TryGetComponent(out checkbox))
        {
            saveCheckbox = checkbox;
            Activation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        CheckboxInteractable checkbox = null;
        if (other.TryGetComponent(out checkbox))
        {
            if (saveCheckbox != null && saveCheckbox.transform == checkbox.transform)
            {
                saveCheckbox = null;
                Deactivation();
            }
        }
    }
}