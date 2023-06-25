using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

//using HomaGames.HomaBelly;

namespace LaynarGames.IdleArcadeTemplate
{
    public class IdleArcadeManager : MonoBehaviour
    {
        [SerializeField] int cashCollected;
        public float currentCollectRadius = 10;
        
        private void Awake()
        {
            if (PlayerPrefs.HasKey("Cash"))
            {
                cashCollected = PlayerPrefs.GetInt("Cash");
            }
        }

        private void Start()
        {
            //HomaBelly.Instance.TrackProgressionEvent(ProgressionStatus.Start, "Level 1", 0);
        }

        [ContextMenu("Add IDs")]
        public void AddIdentifiersToAllBuySections()
        {
            Unlock_Tile[] unlock_Tiles = GameObject.FindObjectsOfType<Unlock_Tile>(true);

            for (int i = 0; i < unlock_Tiles.Length; i++)
            {
                unlock_Tiles[i].ID = SceneManager.GetActiveScene().name + i;
#if UNITY_EDITOR
            EditorUtility.SetDirty(unlock_Tiles[i]);
#endif
            }
        }
    }
}

