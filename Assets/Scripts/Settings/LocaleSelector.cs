using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class LocaleSelector : MonoBehaviour, IBind<LanguageData>
{
    private string _id = "langMnger";
    private LanguageData _data;
    private int _localizationId;
    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public void Bind(LanguageData data)
    {
        _data = data;
        _localizationId = data.localeID;
    }
}

[Serializable]
public class LanguageData : ISaveable
{
    [field: SerializeField] public string Id { get; set; }
    public bool isNew;
    public int localeID;
}
