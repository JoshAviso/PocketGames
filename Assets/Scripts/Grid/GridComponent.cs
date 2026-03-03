
using UnityEditor;
using UnityEngine;

public interface IGridGameObject
{
    public string Name { get; }
};

public class GridComponent : MonoBehaviour
{
    private Grid<IGridGameObject> _grid = new();
    public int XSize => _grid.XSize;
    public int YSize => _grid.YSize;
    public int ZSize => _grid.ZSize;
    public int Size => _grid.Size;
    public Vector3 WorldSize => Vector3.Scale(_cellDimensions, _gridDimensions);

    [Header("Settings")]
    [SerializeField] private Vector3Int _gridDimensions = new(1, 1, 1);
    [SerializeField] private Vector3 _cellDimensions = new(1f, 1f, 1f);
    [SerializeField] private Vector3 _pivotPosition = new(0f, 0f, 0f);
    
    [Header("Debug")]
    [SerializeField] private Color _debugColor = new(1f, 1f, 1f, 0.1f);
    [SerializeField] private bool _showGridlines;
    [SerializeField] private bool _showLabels;

    private Vector3 Pivot => transform.position - Vector3.Scale(_pivotPosition, WorldSize);

    void Awake() {
        CheckDims();
    }

    /// <returns>True if within bounds, false if otherwise</returns>
    public bool GridToWorld(uint x_index, uint y_index, uint z_index, out Vector3 world, Vector3 norm_within_cell = default)
    {
        world = default;
        if(!InBounds(x_index, y_index, z_index)) return false;

        world = Pivot + Vector3.Scale(new(x_index, y_index, z_index), _cellDimensions) + Vector3.Scale(norm_within_cell, _cellDimensions);
        return true;
    }

    /// <returns>True if within bounds, false if otherwise</returns>
    public bool WorldToGrid(Vector3 world, out Vector3Int grid_index)
    {
        grid_index = default;
        
        Vector3 localWorld = world - Pivot;
        int x_coord = Mathf.FloorToInt(localWorld.x / _cellDimensions.x);
        int y_coord = Mathf.FloorToInt(localWorld.y / _cellDimensions.y);
        int z_coord = Mathf.FloorToInt(localWorld.z / _cellDimensions.z);
        
        grid_index = new(x_coord, y_coord, z_coord);
        return InBounds((uint)x_coord, (uint)y_coord, (uint)z_coord);
    }

    public bool InBounds(uint x_index, uint y_index, uint z_index) 
        => !(x_index >= _gridDimensions.x || y_index >= _gridDimensions.y || z_index >= _gridDimensions.z);

    /// <returns>False if no object is present at index, otherwise, true</returns>
    public bool GetObj(uint x_index, uint y_index, uint z_index, out IGridGameObject obj)
        => _grid.GetObj(x_index, y_index, z_index, out obj);
    

    /// <returns>False if not able to set object, otherwise, true</returns>
    public bool SetObj(IGridGameObject obj, uint x_index, uint y_index, uint z_index, bool overwrite = true)
        => _grid.SetObj(obj, x_index, y_index, z_index, overwrite);

    /// <returns>False if no object was removed, otherwise, true</returns>
    public bool RemoveObj(uint x_index, uint y_index, uint z_index, out IGridGameObject obj)
        => _grid.RemoveObj(x_index, y_index, z_index, out obj);

    private void CheckDims()
    {
        if(_gridDimensions.x != XSize || _gridDimensions.y != YSize || _gridDimensions.z != ZSize)
            _grid.SetSize((uint)_gridDimensions.x, (uint)_gridDimensions.y, (uint)_gridDimensions.z, true);
    }

    void OnDrawGizmos()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        if(!_showGridlines && !_showLabels) return;

        for(uint i = 0; i < _gridDimensions.x; i++)
        for(uint j = 0; j < _gridDimensions.y; j++)
        for(uint k = 0; k < _gridDimensions.z; k++)
        {
            _ = GridToWorld(i, j, k, out var center, new(0.5f, 0.5f, 0.5f));
            if (_showGridlines)
            {
                Gizmos.color = _debugColor;
                Gizmos.DrawWireCube(center, _cellDimensions);
            }
            
            if (_showLabels)
            {
                string label = $"{i}, {j}, {k}";
                if(GetObj(i, j, k, out var obj))   
                    label = obj.Name;

                GUIStyle style = new();
                style.normal.textColor = _debugColor;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(center, label);
            }
            
        }
    }
}
