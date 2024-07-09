using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CellVisualize : NetworkBehaviour
{
    [SerializeField]
    private Renderer cellRenderer;

    private enum ICellVisualizeState
    {
        unactive,
        active
    }

    [Rpc(SendTo.NotOwner)]
    private void SyncTheVisualRpc(bool state)
    {
        if (cellRenderer is null)
            throw new System.Exception("ActiveMaterial is not init");

        cellRenderer.material.SetInt("_CellState", (int)(state ? ICellVisualizeState.active : ICellVisualizeState.unactive));
    }

    public void ChangeState(bool state)
    {
        if (cellRenderer is null)
            throw new System.Exception("ActiveMaterial is not init");

        cellRenderer.material.SetInt("_CellState", (int)(state ? ICellVisualizeState.active : ICellVisualizeState.unactive));

        SyncTheVisualRpc(state);
    }
}