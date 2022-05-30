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

        private Hexagrid<HexCellRenderer> field = new Hexagrid<HexCellRenderer>();

        private HexCellRenderer CreateCell(Vector3Int gridPos)
        {
            var sqrRoot3 = Mathf.Sqrt(3) / 4;
            var position = new Vector3(gridPos.x * CellPrefab.OuterSize * 1.5f / 2, 0, (gridPos.y - gridPos.z)  * CellPrefab.OuterSize * sqrRoot3);
            
            HexCellRenderer cell = Instantiate(CellPrefab, position, Quaternion.identity, this.transform);
            cell.gameObject.name = gridPos.ToString();
            return cell;
        }
        
        private void Start()
        {
            field.HexcellConstructor = CreateCell;
            field.GetCellsInRadius(Vector3Int.zero, Radius);
        }
    }
}