
enum PersonEnum : byte
{
    ID,
    Name,
    Max
}

public class Person : Base
{
    [Sync((int)PersonEnum.ID)]
    public int ID { get; private set; }

    [Sync((int)PersonEnum.Name)]
    public string Name { get; private set; }

    public override string ToString()
    {
        return "Person : " + ID + ", " + Name;
    }
}
