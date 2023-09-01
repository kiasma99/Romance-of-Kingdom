using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    private int activeElevation;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleInput();
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) EditCell(hexGrid.GetCell(hit.point));
    }

    private void EditCell(HexCell cell)
    {
        cell.color = Color.white;
        cell.Elevation = activeElevation;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
}
