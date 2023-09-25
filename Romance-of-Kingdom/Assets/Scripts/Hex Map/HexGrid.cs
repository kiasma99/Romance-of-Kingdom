using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4, chunkCountZ = 3;
    private int cellCountX, cellCountZ;

    public HexCell cellPrefab;
    public TextMeshProUGUI cellLabelPrefab;
    public HexGridChunk chunkPrefab;

    private HexCell[] cells;
    private HexGridChunk[] chunks;

    private void Awake()
    {
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for(int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for(int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }        
        }
    }

    private void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountZ];

        for(int z = 0, i = 0; z < cellCountZ; z++)
        {
            for(int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position = new Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0f, z * (HexMetrics.outerRadius * 1.5f));

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        if (x > 0) cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0) cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1) cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);

            }
        }

        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        cell.uiRect = label.rectTransform;
        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public void FindDistancesTo(HexCell cell)
    {
        StopAllCoroutines();
        StartCoroutine(Search(cell));
    }

    IEnumerator Search(HexCell cell)
    { 
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }

        List<HexCell> frontier = new List<HexCell> ();
        cell.Distance = 0;
        frontier.Add(cell);
        while(frontier.Count > 0)
        {
            yield return null;
            HexCell current = frontier[0];
            frontier.RemoveAt(0);
            for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);

                // �̵��Ұ����� �����߰�
                if (neighbor == null || neighbor.Distance != int.MaxValue) continue;
                if (neighbor.Elevation > 1) continue;

                neighbor.Distance = current.Distance + 1;
                frontier.Add(neighbor);
            }
        }
    }

    public void ShowUI(bool visible)
    {
        for(int i = 0; i < chunks.Length; i++) chunks[i].ShowUI(visible);
    }
}
