using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    public enum EBGMKey {
        BGM_Gameplay
    }

    public enum ESEKey {
        SE_Success,
        SE_Failure,
        SE_ThunderAttack,
    }

    [System.Serializable]
    public class SoundData
    {
        [SerializeField] public string key;
        [SerializeField] public AudioClip clip;
    }

    [CreateAssetMenu(fileName = "AudioClipSheet.asset", menuName = "AudioClipSheet")]
    public class AudioSheet : ScriptableObject
    {
        [SerializeField] public List<SoundData> bgmDict;
        [SerializeField] public List<SoundData> seDict;
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(AudioSheet))]
    public class AudioSheetEditor : Editor
    {
        SerializedProperty _bgmProp;
        SerializedProperty _seProp;

        public void OnEnable()
        {
            _bgmProp = serializedObject.FindProperty("bgmDict");
            _bgmProp.arraySize = Enum.GetNames(typeof(EBGMKey)).Length;

            _seProp = serializedObject.FindProperty("seDict");
            _seProp.arraySize = Enum.GetNames(typeof(ESEKey)).Length;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("BGM List");
            EditorGUILayout.BeginVertical(GUI.skin.box);
            foreach (var eBgm in Enum.GetValues(typeof(EBGMKey)) as EBGMKey[]) {
                var soundDataProp = _bgmProp.GetArrayElementAtIndex((int)eBgm);
                var keyProp = soundDataProp.FindPropertyRelative("key");
                keyProp.stringValue = eBgm.ToString();

                EditorGUILayout.PropertyField(soundDataProp.FindPropertyRelative("clip"), new GUIContent(eBgm.ToString()));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("SE List");
            EditorGUILayout.BeginVertical(GUI.skin.box);
            foreach (var eSe in Enum.GetValues(typeof(ESEKey)) as ESEKey[]) {
                var soundDataProp = _seProp.GetArrayElementAtIndex((int)eSe);
                var keyProp = soundDataProp.FindPropertyRelative("key");
                keyProp.stringValue = eSe.ToString();

                EditorGUILayout.PropertyField(soundDataProp.FindPropertyRelative("clip"), new GUIContent(eSe.ToString()));
            }
            EditorGUILayout.EndVertical();

            if (!EditorGUI.EndChangeCheck()) return;

            // Set object dirty, this will make it be saved after saving the project.
            EditorUtility.SetDirty(serializedObject.targetObject);

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}
