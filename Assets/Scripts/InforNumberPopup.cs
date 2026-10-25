using UnityEngine;
using UnityEngine.UI;

public class InforNumberPopup : MonoBehaviour
{
    public static InforNumberPopup Instance { get; private set; }
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private float popupLifetime = 2f;
    private void Awake()
    {  
        Instance = this;
    }

    public void ShowDamage(float damage, Transform target, Color color)
    {


    if (target == null)
    {
        Debug.LogError("Target đang bị null!");
        return;
    }

    if (damagePopupPrefab == null)
    {
        Debug.LogError("Chưa gán Damage Popup Prefab!");
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

    Destroy(popup, popupLifetime);
}

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

   
}
