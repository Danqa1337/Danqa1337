[System.Serializable]
public struct TileTemplate
{
    public int index;
    public int SolidLayer;
    public int DropLayer;
    public int LiquidLayer;
    public int FloorLayer;
    public int GroundCover;
    public TileState tileState;
    public Biome Biome;
    public TileTemplate(int index)
    {
        this.index = index;
        SolidLayer = -1;
        DropLayer = -1;
        LiquidLayer = -1;
        GroundCover = -1;
        FloorLayer = ItemDataBase.GetObject("RockFloor").id;
        tileState = TileState.Darkness;
        Biome = Biome.Cave;
    }

    public void GenerateObject(ObjectData data, bool save = true)
    {
        SetState(data.staticData.defaultTileState,false);
        switch (data.dynamicData.objectType)
        {
            case ObjectType.Solid:
                this.SolidLayer = data.id;
                break;
            case ObjectType.Drop:
                this.DropLayer = data.id;
                break;
            case ObjectType.Floor:
                this.FloorLayer = data.id;
                break;
            case ObjectType.Liquid:
                this.LiquidLayer = data.id;
                break; 
            case ObjectType.GroundCover:
                this.GroundCover = data.id;
                break;
        }
        if(save) MapGenerator.templateMap[index] = this;
    }
    
    public void Clear()
    {
        SolidLayer = -1;
        DropLayer = -1;
        LiquidLayer = -1;
        FloorLayer = ItemDataBase.GetObject("RockFloor").id;
        GroundCover = -1;
        tileState = TileState.Darkness;
        MapGenerator.templateMap[index] = this;

    }
    public void MakeAbyss()
    {
        SolidLayer = -1;
        DropLayer = -1;
        FloorLayer = -1;
        GroundCover = -1;
        tileState = TileState.Abyss;
        MapGenerator.templateMap[index] = this;

    }
    public void SetState(TileState tileState, bool save = true)
    {
        if (tileState != TileState.Any)
        {
            this.tileState = tileState;
            if(save)MapGenerator.templateMap[index] = this;
        }
    }

    public void SetBiome(Biome biome, bool save = true)
    {
        if (biome != Biome.Any)
        {
            this.Biome = biome;
            if (save) MapGenerator.templateMap[index] = this;
        }
    }
    public void GenerateObject(string name)
    {

        var obj = ItemDataBase.GetObject(name);
        GenerateObject(obj);
    }
    public void GenerateObject(int id)
    {

        var obj = ItemDataBase.GetObject(id);
        GenerateObject(obj);
        

    }

    public bool isFloor()
    {
        return (tileState == TileState.Floor || tileState == TileState.Door);

    }
}