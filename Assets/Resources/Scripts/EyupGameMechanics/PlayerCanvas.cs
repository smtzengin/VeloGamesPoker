using TMPro;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chips, _bet;
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void UpdateBet(int amount)
    {
        UIManager.UpdateChipText(_bet, amount);
    }
    public void UpdateChips(int amount)
    {
        UIManager.UpdateChipText(_chips,amount);
    }
}
