using UnityEngine;
using UnityEngine.UI;

public class DamePopup : MonoBehaviour
{
    public static DamePopup Instance { get; private set; }
    [SerializeField] private GameObject damagePopupPrefab;

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
        
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

   
}
