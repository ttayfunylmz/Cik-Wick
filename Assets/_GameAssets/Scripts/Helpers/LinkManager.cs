using UnityEngine;
using UnityEngine.UI;

public class LinkManager : MonoBehaviour
{
    [SerializeField] private string _linkUrl;
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start() 
    {
        _button.onClick.AddListener(OpenLink);
    }

    public void OpenLink()
    {
        Application.OpenURL(_linkUrl);
    }
}
