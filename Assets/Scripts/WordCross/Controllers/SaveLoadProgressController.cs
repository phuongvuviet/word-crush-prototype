using UnityEngine;

namespace WCross
{
    public class SaveLoadProgressController
    {
        const string currentLevelKey = "WC_CurrentLevel";
        private const string levelDataPath = "WC/LevelData";
        int _currentLevel = -1;

        public int CurrentLevel
        {
            get
            {
                if (_currentLevel == -1)
                {
                    _currentLevel = GetCurrentLevelIndex();
                }

                return _currentLevel;
            }
            set
            {
                _currentLevel = value;
                SaveCurrentLevelIndex(_currentLevel);
            }
        }
        
        public LevelDataModel LoadLevelData(int level)
        {
            LevelDataModel levelData = Resources.Load<LevelDataModel>(levelDataPath + level);
            if (levelData == null)
            {
                Debug.LogError("Cann't load level dataaaa: " + level);
            }

            return levelData;
        }
        
        public int GetCurrentLevelIndex()
        {
            return PlayerPrefs.GetInt(currentLevelKey, 1);
        }
        
        void SaveCurrentLevelIndex(int level)
        {
            PlayerPrefs.SetInt(currentLevelKey, level);
        }
    }
}