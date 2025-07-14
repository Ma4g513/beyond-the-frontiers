using UnityEngine;

public class invetaire : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel; // glisse ici ton Panel dans l’Inspector
    private bool isOpen = false;

    void Awake()
    {
        inventoryPanel.SetActive(isOpen);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen);
        }
    }
}
