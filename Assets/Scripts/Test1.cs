using UnityEngine;

public class Test1 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Player.Instance.RegisterPropertyChanged(
            PlayerEnum.Name.ToString(),
            (Obj, Name, Value, OldValue) =>
            {
                //Debug.LogError(OldValue + " > " + Value);
            });

        Customer.Instance.RegisterPropertyChanged(
            "Name",
            (currentValue, oldValue) =>
            {
                //Debug.LogError(oldValue + " > " + currentValue);
            });
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            for (int i = 0; i < 10000; ++i)
            {
                Player.Instance.OnSyncOne(PlayerEnum.Name.GetHashCode(), string.Empty);
            }
        }

        if (Input.GetKeyUp(KeyCode.F2))
        {
            for (int i = 0; i < 10000; ++i)
            {
                Customer.Instance.Name = string.Empty;
            }
        }
    }
}
