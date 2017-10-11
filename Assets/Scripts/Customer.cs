using System.Collections.Generic;

public class Customer
{
    private static Customer instance;

    public static Customer Instance
    {
        get
        {
            if (null == instance) instance = new Customer();
            return instance;
        }
    }

    public delegate void DlgtOnPropertyChanged(object InCurrentValue, object InOldValue);

    private Dictionary<string, DlgtOnPropertyChanged> dlgtOnPropertyChangedDict;

    public void RegisterPropertyChanged(string InName, DlgtOnPropertyChanged InFunc)
    {
        if (null == dlgtOnPropertyChangedDict) dlgtOnPropertyChangedDict = new Dictionary<string, DlgtOnPropertyChanged>(5);

        if (dlgtOnPropertyChangedDict.ContainsKey(InName)) dlgtOnPropertyChangedDict[InName] += InFunc;
        else dlgtOnPropertyChangedDict[InName] = InFunc;
    }

    public void UnRegisterPropertyChanged(string InName, DlgtOnPropertyChanged InFunc)
    {
        if (dlgtOnPropertyChangedDict.ContainsKey(InName)) dlgtOnPropertyChangedDict[InName] -= InFunc;
    }

    private void OnPropertyChanged(string InName, object InCurrentValue, object InOldValue)
    {
        if (!InCurrentValue.Equals(InOldValue))
        {
            DlgtOnPropertyChanged func;
            if (dlgtOnPropertyChangedDict.TryGetValue(InName, out func)) func(InCurrentValue, InOldValue);
        }
    }

    private int id;
    public int Id
    {
        get { return id; }
        set
        {
            OnPropertyChanged("Id", value, id);
            id = value;
        }
    }

    private string name;

    public string Name
    {
        get { return name; }
        set
        {
            OnPropertyChanged("Name", value, name);
            name = value;
        }
    }

    private string surname;
    public string Surname
    {
        get { return surname; }
        set
        {
            OnPropertyChanged("Surname", value, surname);
            surname = value;
        }
    }

    private int age;
    public int Age
    {
        get { return age; }
        set
        {
            OnPropertyChanged("Age", value, age);
            age = value;
        }
    }
}
