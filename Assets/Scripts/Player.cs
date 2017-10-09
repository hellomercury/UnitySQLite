using UnityEngine;

public enum PlayerEnum
{
    ID,
    Name,
    Level,
    Gold,
    Max
}

public class Player : Base
{
    public static Player Instance
    {
        get
        {
            if(null == instance) instance = new Player();
            return instance;
        }
    }
    private static Player instance;

    [Sync((int)PlayerEnum.ID)]
    public int ID { get; private set; }

    [Sync((int)PlayerEnum.Name)]
    public string Name { get; private set; }

    [Sync((int)PlayerEnum.Level)]
    public int Level { get; private set; }

    [Sync((int)PlayerEnum.Gold)]
    public int Gold { get; private set; }
}
