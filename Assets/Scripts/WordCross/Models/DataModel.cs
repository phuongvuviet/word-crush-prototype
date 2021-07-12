using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCross{
    [CreateAssetMenu(fileName = "LevelData")]
    public class DataModel : ScriptableObject 
    {
        public List<string> Words;
        public string Letters;
    }
}