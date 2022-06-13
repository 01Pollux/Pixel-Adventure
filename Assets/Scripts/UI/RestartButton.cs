using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RestartButton : MonoBehaviour
    {
        private void Start()
        {
            Button btn = GetComponent<Button>();
            LevelManager levelmgr = GetComponentInParent<LevelManager>();
            btn.onClick.AddListener(() => { levelmgr.GoTo(levelmgr.CurrentLevel); });
        }

    }
}