
public class Grid<TGridObject>
{
    private TGridObject[,,] _objects = new TGridObject[1, 1, 1];
    public int XSize => _objects.GetLength(0);
    public int YSize => _objects.GetLength(1);
    public int ZSize => _objects.GetLength(2);
    public int Size => _objects.Length;
    
    public bool InBounds(uint x_index, uint y_index, uint z_index) 
        => !(x_index >= XSize || y_index >= YSize || z_index >= ZSize);

    /// <returns>False if no object is present at index, otherwise, true</returns>
    public bool GetObj(uint x_index, uint y_index, uint z_index, out TGridObject obj)
    {
        obj = default;
        if(!InBounds(x_index, y_index, z_index)) return false;
        obj = _objects[x_index, y_index, z_index];
        return !Equals(obj, default);
    }

    /// <returns>False if not able to set object, otherwise, true</returns>
    public bool SetObj(TGridObject obj, uint x_index, uint y_index, uint z_index, bool overwrite = true)
    {
        if(!InBounds(x_index, y_index, z_index)) return false;
        if(!overwrite && GetObj(x_index, y_index, z_index, out _)) return false;
        _objects[x_index, y_index, z_index] = obj;
        return true;
    }

    /// <returns>False if no object was removed, otherwise, true</returns>
    public bool RemoveObj(uint x_index, uint y_index, uint z_index, out TGridObject obj)
    {
        obj = default;
        if(!GetObj(x_index, y_index, z_index, out obj)) return false;
        obj = _objects[x_index, y_index, z_index];
        _objects[x_index, y_index, z_index] = default;
        return true;
    }

    public void SetSize(uint x_size, uint y_size, uint z_size, bool preserveObjects = true)
    {
        if(_objects == null || !preserveObjects)
        {
            _objects = new TGridObject[x_size, y_size, z_size];
            return;
        }

        TGridObject[,,] temp = new TGridObject[x_size, y_size, z_size];
        for(int i = 0; i < XSize && i < x_size; i++)
        for(int j = 0; j < YSize && j < y_size; j++)
        for(int k = 0; k < ZSize && k < z_size; k++)
            temp[i, j, k] = _objects[i, j, k];
        
        _objects = temp;
    }
}
