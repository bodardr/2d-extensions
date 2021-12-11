using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CollectionItemButton : MonoBehaviour
{
    private Button button;
    private ICollectionCallback[] callbacks;

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

        int index = transform.GetSiblingIndex();
        foreach (var callback in callbacks)
        {
            callback.OnClicked(index);
        }
    }
}