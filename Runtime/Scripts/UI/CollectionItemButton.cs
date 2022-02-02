using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CollectionItemButton : MonoBehaviour
{
    private Button button;
    private ICollectionCallback[] callbacks;

    [SerializeField]
    private IndexRetrievalStrategy indexRetrieval = IndexRetrievalStrategy.This; 
    
    public int CustomIndex { get; set; }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
        
        InitializeCallbacks();
    }

    private void InitializeCallbacks()
    {
        callbacks = GetComponentsInParent<ICollectionCallback>();
    }

    public void Click()
    {
        if(callbacks == null)
            InitializeCallbacks();

        int index;
        switch (indexRetrieval)
        {
            case IndexRetrievalStrategy.This:
                index = transform.GetSiblingIndex();
                break;
            case IndexRetrievalStrategy.Parent:
                index = transform.parent.GetSiblingIndex();
                break;
            case IndexRetrievalStrategy.ParentOfParent:
                index = transform.parent.parent.GetSiblingIndex();
                break;
            case IndexRetrievalStrategy.Custom:
                index = CustomIndex;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        foreach (var callback in callbacks)
        {
            callback.OnItemClicked(index);
        }
    }
}

public enum IndexRetrievalStrategy
{
    This,
    Parent,
    ParentOfParent,
    Custom
}