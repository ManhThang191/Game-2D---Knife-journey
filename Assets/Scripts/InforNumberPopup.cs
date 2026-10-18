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

    public void ShowDamage(float damage,Transform target, Color color)
    {
        if (damagePopupPrefab == null)
        {
            Debug.LogWarning("Chưa gán Damage Popup Prefab!");
            return;
        }
        Vector3 popupPosition = target.transform.position + new Vector3(0f, 1f, 0f);

        GameObject popup = Instantiate(
            damagePopupPrefab,
            popupPosition,
            Quaternion.identity
        );

        // Tìm TextMeshPro trong các object con của popup
        Text text = popup.GetComponentInChildren<Text>();

        if (text != null)
        {
            text.text = damage.ToString("0");
            text.color = color;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy TextMeshPro trong Damage Popup!");
        }

        Destroy(popup, popupLifetime);
        
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

   
}
