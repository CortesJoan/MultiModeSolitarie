using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(VisualDataBank))]
public class VisualDataBankEditor : Editor
{
    private void OnEnable()
    {
        VisualDataBank visualDataBank = (VisualDataBank)target;
        visualDataBank.Awake();
    }
}
