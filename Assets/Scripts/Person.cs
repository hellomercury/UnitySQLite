
enum PersonEnum : byte
{
    Id,
    Name,
    Surname,
    Age,
    Max
}

public class Person : Base
{
    [Sync((int)PersonEnum.Id)]
    public int Id { get; private set; }

    [Sync((int)PersonEnum.Name)]
    public string Name { get; private set; }

    [Sync((int)PersonEnum.Surname)]
    public string Surname { get; private set; }

    [Sync((int)PersonEnum.Age)]
    public int Age { get; private set; }
}
