using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CellInfo
{
    [SerializeField]
    private bool state;

    public bool State { get => state; set => state = value; }
}

public class CellPool : MonoBehaviour
{
    private List<ICell> childCells = new List<ICell>();

    private CellInfo[] cellMap;

    public List<ICell> ChildCells { get => childCells; set => childCells = value; }
    public CellInfo[] CellMap { get => cellMap; set => cellMap = value; }

    private bool IsEmpty(Array array)
    {
        return (array is null) || (array.Length == 0);
    }

    public void Init()
    {
        var cells = gameObject.GetComponentsInChildren<CellBase>();

        if (IsEmpty(cells))
            throw new Exception("Cells in the child objects are not matched!");

        childCells = new List<ICell>(cells);
    }

    public void SetCells(CellInfo[] cells)
    {
        CellMap = new CellInfo[cells.Length];

        for (int i = 0; i < CellMap.Length; i++)
        {
            CellMap[i] = cells[i];
        }

        LoadToCells();
    }

    private void LoadToCells()
    {
        if (CellMap.Length != childCells.Count)
            throw new Exception("The number of cells does not match the generated code");

        for (int i = 0; i < childCells.Count; i++)
        {
            CellInitialize(childCells[i], i, CellMap[i].State && HasVisualize(), HasVisualize());
        }
    }

    private void CellInitialize(ICell cell, int cellIndex, bool isActivate, bool isReference)
    {
        cell.Deactivation();

        if (isReference)
            cell.Reference();

        if (isActivate)
        {
            cell.Activation();
        }

        cell.SetIndex(cellIndex);
        cell.SetParentPool(this);
    }

    protected virtual bool HasVisualize()
    {
        return false;
    }
}