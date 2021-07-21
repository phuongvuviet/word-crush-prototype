using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCross
{
    [CreateAssetMenu(fileName = "LevelData")]
    public class LevelDataModel : ScriptableObject 
    {
        public List<string> Words;
        public string Letters;
        public List<string> BoardLayout;
    }
}