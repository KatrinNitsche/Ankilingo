using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class IconSelectionWindowUI : MonoBehaviour
{
    [SerializeField] private GameObject iconButtonPrefab;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Sprite[] iconList;
    
    public event Action<Sprite> onIconClicked;
    
    private void Start()
    {
        RemoveIconsFromContainer();
        AddIconButtonsToContainer();
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    private void AddIconButtonsToContainer()
    {
        foreach (Sprite icon in iconList)
        {
            var button = Instantiate(iconButtonPrefab, iconContainer);
            button.GetComponentInChildren<Image>().sprite = icon;
            button.GetComponent<Button>().onClick.AddListener(() => IconClicked(icon));
        }
    }

    private void IconClicked(Sprite icon)
    {
        onIconClicked?.Invoke(icon);
        gameObject.SetActive(false);
    }

    private void RemoveIconsFromContainer()
    {
        foreach (GameObject obj in iconContainer)
        {
            Destroy(obj);
        }
    }
}
