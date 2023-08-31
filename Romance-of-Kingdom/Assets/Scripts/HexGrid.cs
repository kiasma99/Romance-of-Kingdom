using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
    public TextMeshProUGUI cellLabelPrefab;
    private Canvas gridCanvas;
    private HexMesh hexMesh;

    private HexCell[] cells;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void Start()
    {
        hexMesh.Triangulate(cells);
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position = new Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0f, z * (HexMetrics.outerRadius * 1.5f));

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        if (x > 0) cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0) cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1) cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);

            }
        }

        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

/*    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
    }*/
}
