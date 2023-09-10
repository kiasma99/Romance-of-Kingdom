using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    private int activeElevation;
    private bool editMode;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleInput();
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if(editMode) EditCell(currentCell);
            else hexGrid.FindDistancesTo(currentCell);
        }
    }

    public void SetEditMode(bool toggle)
    {
        editMode = toggle;
        hexGrid.ShowUI(!toggle);
    }

    private void EditCell(HexCell cell)
    {
        cell.Elevation = activeElevation;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
}
