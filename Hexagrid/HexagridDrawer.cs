using System;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
    public class HexagridDrawer : MonoBehaviour
    {
        public int Radius;
        public HexCellRenderer CellPrefab;

        private Hexagrid<HexCellRenderer> field;

        private HexCellRenderer CreateCell(Vector3Int gridPos)
        {
            var position = field.ConvertHexToUnityPosition(gridPos);
            
            HexCellRenderer cell = Instantiate(CellPrefab, position + this.transform.position, Quaternion.identity, this.transform);
            cell.gameObject.name = gridPos.ToString();
            cell.SetPosition(gridPos);
            return cell;
        }
        
        private void Start()
        {
            field = new Hexagrid<HexCellRenderer>(CreateCell, CellPrefab.OuterSize, false);
            field.GetCellsInRadius(Vector3Int.zero, Radius);
            foreach (var cell in field.Cells)
            {
                cell.GetComponent<HexCellRenderer>().SetColor(new Color(0.2f, 1f,0.2f, 1f));
            }
        }
    }
}