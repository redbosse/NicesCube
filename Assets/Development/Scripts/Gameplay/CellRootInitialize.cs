using System.Runtime;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct JsonCellInfo
{
    public CellInfo[] cells;
}

public class CellRootInitialize : NetworkBehaviour
{
    [SerializeField]
    private int numberOfGeneratedCells = 3;

    private CellInfo[] cells = null;

    [SerializeField]
    private CellMainPool mainPool;

    [SerializeField]
    private CellReferencePool referencePool;

    public CellMainPool MainPool { get => mainPool; set => mainPool = value; }
    public CellReferencePool ReferencePool { get => referencePool; set => referencePool = value; }

    private void Init()
    {
        var mainPool = GetComponentInChildren<CellMainPool>();
        var refPool = GetComponentInChildren<CellReferencePool>();

        if (mainPool is not null)
        {
            MainPool = mainPool;

            MainPool.Init();
        }
        else
        {
            throw new System.Exception($"{TypeVariable.GetTypeName(mainPool)} is not Initialize!");
        }

        if (refPool is not null)
        {
            ReferencePool = refPool;

            ReferencePool.Init();
        }
        else
        {
            throw new System.Exception($"{TypeVariable.GetTypeName(ReferencePool)} is not Initialize!");
        }
    }

    private void Generator()
    {
        int maxCells = MainPool.ChildCells.Count;

        if (maxCells == 0)
            throw new System.Exception("Cells is not initialized!");

        CellInfo[] cellInfos = new CellInfo[maxCells];

        for (int i = 0; i < cellInfos.Length; i++)
        {
            cellInfos[i] = new CellInfo { State = false };
        }

        int cellCounter = 0;

        int Index = 0;

        while (true)
        {
            if (cellCounter >= numberOfGeneratedCells) break;

            if (Index > maxCells - 1)
                Index = 0;

            if (!cellInfos[Index].State)
            {
                if (Random.Range(0, 32000) > 16000)
                {
                    cellCounter++;

                    cellInfos[Index].State = true;
                }
            }

            Index++;
        }

        cells = cellInfos;

        MainPool.SetCells(cells);
        ReferencePool.SetCells(cells);
    }

    private void Start()
    {
        Init();
    }

    [Rpc(SendTo.Server)]
    private void GetStateRpc()
    {
        JsonCellInfo info = new JsonCellInfo { cells = cells };

        ShareStateRpc(JsonUtility.ToJson(info));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShareStateRpc(string network_cells)
    {
        cells = JsonUtility.FromJson<JsonCellInfo>(network_cells).cells;

        MainPool.SetCells(cells);
        ReferencePool.SetCells(cells);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            Generator();

        if (IsClient)
            GetStateRpc();
    }
}