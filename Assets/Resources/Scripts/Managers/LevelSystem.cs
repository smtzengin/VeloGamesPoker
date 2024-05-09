using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private int _nextXP, _currentXP;
    [SerializeField] private int _level;

    private void Start()
    {
        GetUserLevelAndExp();
    }
    public async void GainXP(int gainedChips)
    {
        int xp = CalculateXP(gainedChips);
        _currentXP += xp;

        // Eğer kazanılan deneyim, bir sonraki seviyeye geçmeye yeterliyse
        if (_currentXP >= _nextXP)
        {
            // Yeni seviye hesaplama işlemi
            int newLevel = _level + 1;
            _currentXP -= _nextXP;
            _nextXP = 100 + (newLevel * 50);
            _level = newLevel;

            // Veritabanını güncelleme işlemi
            await DatabaseManager.Instance.UpdateExp(_currentXP);
            await DatabaseManager.Instance.UpdateLevel(_level);
        }
        else
        {
            // Eğer yeni seviyeye geçilmediyse, sadece deneyim puanını güncelleme işlemi
            await DatabaseManager.Instance.UpdateExp(_currentXP);
        }

        UpdateCanvas();
    }
    private int CalculateXP(int gainedChips)
    {
        return Mathf.CeilToInt(gainedChips / 10f);
    }

    public void UpdateCanvas()
    {
        UIManager.UpdateLevel(_level, _nextXP,_currentXP);
    }

    public async void GetUserLevelAndExp()
    {
        _level = await FirebaseManager.Instance.GetUserIntData("Level");
        _currentXP = await FirebaseManager.Instance.GetUserIntData("Exp");
        
        _nextXP = 100 + (_level * 50);

        UpdateCanvas();
    }
}
