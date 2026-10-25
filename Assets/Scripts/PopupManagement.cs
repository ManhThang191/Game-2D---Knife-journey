using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupManagement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static PopupManagement Instance {get; private set;}
    [Header("Dame Popup")]
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private float damePopupLifetime = 2f;
    
    [Header("Infor Popup")]
    [SerializeField] private GameObject inforPopupPrefab;
    [SerializeField] private float inforPopupLifetime = 2f;
    
    private void Awake()
    {  
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDamage(float damage, Transform target, Color color)
    {
        if (target == null)
        {
            // Debug.LogError("Target đang bị null!");
            return;
        }

        if (damagePopupPrefab == null)
        {
            // Debug.LogError("Chưa gán Damage Popup Prefab!");
            return;
        }

        Vector3 popupPosition = target.position + Vector3.up;

        GameObject popup = Instantiate(
            damagePopupPrefab,
            popupPosition,
            Quaternion.identity
        );

        Debug.Log($"Popup đã tạo: {popup.name}, vị trí: {popup.transform.position}");

        Text text = popup.GetComponentInChildren<Text>(true);

        if (text == null)
        {
            Debug.LogError("Không tìm thấy Unity UI Text trong prefab!");
            return;
        }

        text.text = damage.ToString("0");
        text.color = color;
        text.enabled = true;

        Debug.Log($"Text: {text.text}, active: {text.gameObject.activeInHierarchy}");

        Destroy(popup, damePopupLifetime);
    }

    public void ShowInfo(string infoContent, Transform target, Color color)
{
    if (target == null)
    {
        Debug.LogError("Target đang bị null!");
        return;
    }

    if (string.IsNullOrWhiteSpace(infoContent))
    {
        Debug.LogWarning("Chưa có nội dung thông báo!");
        return;
    }

    if (inforPopupPrefab == null)
    {
        Debug.LogError("Chưa gán Info Popup Prefab!");
        return;
    }

    Vector3 popupPosition = target.position;

    GameObject popup = Instantiate(
        inforPopupPrefab,
        popupPosition,
        Quaternion.identity
    );

    Text text = popup.GetComponentInChildren<Text>(true);

    if (text == null)
    {
        Debug.LogError("Không tìm thấy Unity UI Text trong Info Popup Prefab!");
        Destroy(popup);
        return;
    }

    text.text = infoContent;
    text.color = color;
    text.enabled = true;

    Destroy(popup, inforPopupLifetime);
}
}
