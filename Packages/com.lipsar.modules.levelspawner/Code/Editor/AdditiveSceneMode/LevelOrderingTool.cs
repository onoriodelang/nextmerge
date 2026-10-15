using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Modules.LevelSpawner.Editor.LevelOrderingTool
{
    public class LevelOrderingTool : EditorWindow
    {
        private string[] _scenesByIndex; // warning: with offset = _tagetConfig.FirstLevelSceneIndex
        private Dictionary<string, int> _indexByNames;
        private AdditiveSceneMode.AdditiveSceneModeSpawnConfig _targetConfig; 
        private bool _scenesInfoCollected;
        private Vector2 _scrollPos;

        private int currentSelectorSceneIndex;

        [MenuItem("Modules/LevelSpawner/LevelOrderingTool")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            LevelOrderingTool window = (LevelOrderingTool)EditorWindow.GetWindow(typeof(LevelOrderingTool));
            window.titleContent = new GUIContent("Level ordering tool");
            window.Show();
        }

        private void OnEnable()
        {
            _scenesInfoCollected = false;
        }

        private void OnDisable()
        {
            _targetConfig = null;
            _scenesInfoCollected = false;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            _targetConfig = (AdditiveSceneMode.AdditiveSceneModeSpawnConfig) EditorGUILayout.ObjectField("Target config", _targetConfig, typeof(AdditiveSceneMode.AdditiveSceneModeSpawnConfig), true);

            if(_targetConfig == null)
            {
                EditorGUILayout.HelpBox("Select target config", MessageType.Info);
                _scenesInfoCollected = false;
                return;
            }

            if(!_scenesInfoCollected || (UnityEditor.SceneManagement.EditorSceneManager.sceneCountInBuildSettings) != _scenesByIndex.Length)
            {
                CollectScenesInfo();
            }

            EditorGUILayout.Space();

            DrawLevelsList();

        } 

        private void CollectScenesInfo()
        {
            _scenesByIndex = new string[UnityEditor.SceneManagement.EditorSceneManager.sceneCountInBuildSettings];
            _indexByNames = new Dictionary<string, int>();

            for (int i = 0; i < _scenesByIndex.Length; i++)
            {
                _scenesByIndex[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
                _indexByNames[_scenesByIndex[i]] = i;
            }
        }

        private void DrawLevelsList()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(position.width * 0.8f),GUILayout.Height(position.height * 0.7f));
            for (int i = 0; i < _targetConfig.SceneNames.Count; i++)
            {
                HorizontalLine(new Color(0,0,0,0.4f));
                DrawLevelEntry(_indexByNames[_targetConfig.SceneNames[i]], i);
            }

            HorizontalLine(new Color(0,0,0,0.4f));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("+", GUILayout.Width(position.width * 0.75f)))
            {
                AddEntryToTarget();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            HorizontalLine(new Color(0,0,0,0.4f));
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Generate string for remote config", GUILayout.Width(position.width * 0.8f)))
            {
                ShowRCStringPopUp();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
        }

        private void DrawLevelEntry(int sceneIndex , int order)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"[{order+1}]");
            GUILayout.Space(15);
            currentSelectorSceneIndex = sceneIndex;
            currentSelectorSceneIndex = EditorGUILayout.Popup(currentSelectorSceneIndex, _scenesByIndex);
            if(currentSelectorSceneIndex != (sceneIndex))
            {
                UpdateConcreteIndex(currentSelectorSceneIndex, order);
            }
            GUILayout.Space(15);
            if(GUILayout.Button("↑"))
            {
                SwapEntriesAtTarget(order, order-1);
            }
            if(GUILayout.Button("↓"))
            {
                SwapEntriesAtTarget(order, order+1);
            }
            GUILayout.Space(15);
            if(GUILayout.Button("X"))
            {
                RemoveEntryFromTarget(order);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void UpdateConcreteIndex(int newIndex, int order)
        {
            _targetConfig.SceneNames[order] = _scenesByIndex[newIndex];
            EditorUtility.SetDirty(_targetConfig);
        }

        private void AddEntryToTarget()
        {
            if(_targetConfig.SceneNames==null || _targetConfig.SceneNames.Count == 0)
            {
                _targetConfig.SceneNames = new List<string>(){_scenesByIndex[Mathf.Min(2, _scenesByIndex.Length-1)]};
            }else
            {
                _targetConfig.SceneNames.Add(_targetConfig.SceneNames[_targetConfig.SceneNames.Count-1]);
            }
            EditorUtility.SetDirty(_targetConfig);
        }

        private void RemoveEntryFromTarget(int entryId)
        {
            _targetConfig.SceneNames.RemoveAt(entryId);
            EditorUtility.SetDirty(_targetConfig);
        }
        private void SwapEntriesAtTarget(int entryIDA, int entryIDB)
        {
            if(!IsLevelOrderIndexValid(entryIDA) || !IsLevelOrderIndexValid(entryIDB))
            {
                return;
            }

            string temp = _targetConfig.SceneNames[entryIDA];
            _targetConfig.SceneNames[entryIDA] = _targetConfig.SceneNames[entryIDB];
            _targetConfig.SceneNames[entryIDB] = temp;
            EditorUtility.SetDirty(_targetConfig);
        }

        private bool IsLevelOrderIndexValid(int orderIndex)
        {
            return orderIndex >= 0 && orderIndex < _targetConfig.SceneNames.Count;
        }

        public static void HorizontalLine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        private void ShowRCStringPopUp()
        {
            EditorUtility.DisplayDialog("RC string", _targetConfig.ToConfigString(), "ok");
        }
    }
}