using UnityEngine;
using System.Collections;
using DlhSoft.ChartLibrary.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace DlhSoft.ChartLibrary
{
    public class BarChartBehavior : MonoBehaviour
    {
        public ChartData Data;

        // Use this for initialization
        void Start()
        {
            Update();
        }

        private List<List<GameObject>> cubes;

        // Update is called once per frame
        void Update()
        {
            if (Data == null)
                return;
            var rows = Data.Rows;
            if (rows == null)
                return;
            var rowCount = rows.Length;
            var columnNames = Data.ColumnNames;
            var columnCount = Math.Max(columnNames != null ? columnNames.Length : 0, rows.Any() ? rows.Max(r => r.Values != null ? r.Values.Length : 0) : 0);
            var values = rows.SelectMany(r => r.Values ?? new float[0]);
            var maxValue = values.Any() ? values.Max() : 0;
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                for (var j = 0; j < columnCount; j++)
                {
                    var rowValues = row.Values;
                    if (rowValues.Length <= j)
                        continue;
                    var value = rowValues[j];
                    var cube = GetCube(i, j);
                    var rowName = row.Name;
                    rowName = !string.IsNullOrEmpty(rowName) ? rowName : string.Format(CultureInfo.InvariantCulture, "[{0}]", i);
                    var columnName = columnNames != null && columnNames.Length > j ? columnNames[j] : null;
                    columnName = !string.IsNullOrEmpty(columnName) ? columnName : string.Format(CultureInfo.InvariantCulture, "[{0}]", j);
                    cube.name = string.Format(CultureInfo.InvariantCulture, "Bar-{0}-{1}", rowName, columnName);
                    cube.transform.parent = gameObject.transform;
                    cube.transform.localPosition = new Vector3((i + 0.5f) / rowCount, (value / 2f) / maxValue, (j + 0.5f) / columnCount);
                    cube.transform.localScale = new Vector3(0.25f / rowCount, value / maxValue, 0.25f / columnCount);
                    var cubeRenderer = cube.GetComponent<Renderer>();
                    cubeRenderer.material = row.Material;
                }
                RemoveCubes(i, columnCount);
            }
            RemoveCubes(rowCount);
        }

        private GameObject GetCube(int i, int j)
        {
            if (cubes == null)
                cubes = new List<List<GameObject>>();
            while (cubes.Count <= i)
                cubes.Add(new List<GameObject>());
            var rowCubes = cubes[i];
            while (rowCubes.Count <= j)
                rowCubes.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            var cube = rowCubes[j];
            return cube;
        }

        private void RemoveCubes(int i, int jStart)
        {
            if (cubes == null || cubes.Count <= i)
                return;
            var rowCubes = cubes[i];
            for (var j = rowCubes.Count; j-- > jStart;)
            {
                var cube = rowCubes[j];
                Destroy(cube);
                rowCubes.RemoveAt(j);
            }
        }
        private void RemoveCubes(int iStart)
        {
            if (cubes == null)
                return;
            for (var i = cubes.Count; i-- > iStart;)
            {
                RemoveCubes(i, 0);
                cubes.RemoveAt(i);
            }
        }
    }
}
