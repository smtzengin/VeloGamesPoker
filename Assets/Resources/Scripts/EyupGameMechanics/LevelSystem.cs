using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    private int _nextXP, _currentXP;
    private int _level;
    void Awake()
    {
        _level = 0; //Get level from database
        _currentXP = 0; //Get _currentXP from database

        _nextXP = 100 + (_level * 50);
    }
    private void Start()
    {
        UpdateCanvas();
    }
    public void GainXP(int gainedChips)
    {
        int xp = CalculateXP(gainedChips);
        _currentXP += xp;
        if(_currentXP >= _nextXP)
        {
            _currentXP -= _nextXP;
            _level++;
            _nextXP = 100 + (_level * 50);
        }

        //Send CurrentXP and level to the database.

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
}
