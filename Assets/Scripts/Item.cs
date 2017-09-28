internal enum  ItemEnum : byte
{
    ID,
    Name,
    Icon,
    Des,
    PileUpperLimit,
    Skill,
    ItemBuyInfo,
    UnlockInfo,
    Max
}

public class Item : Base
{
    [Sync((int)ItemEnum.ID)]
    public int ID { get; private set; }

    [Sync((int)ItemEnum.Name)]
    public int Name { get; private set; }

    [Sync((int)ItemEnum.Icon)]
    public int Icon { get; private set; }

    [Sync((int)ItemEnum.Des)]
    public int Des { get; private set; }

    [Sync((int)ItemEnum.PileUpperLimit)]
    public int PileUpperLimit { get; private set; }

    [Sync((int)ItemEnum.Skill)]
    public int Skill { get; private set; }
    
    [Sync((int)ItemEnum.ItemBuyInfo)]
    public int ItemBuyInfo { get; private set; }

    [Sync((int)ItemEnum.UnlockInfo)]
    public int UnlockInfo { get; private set; }

    public override string ToString()
    {
        return "Item : " + ID + ", " + Name + ", " + Icon
               + ", " + Des + ", " + PileUpperLimit + ", " + Skill
               + ", " + ItemBuyInfo + ", " + UnlockInfo;
    }
}
