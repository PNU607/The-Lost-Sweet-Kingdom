using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(StageWaveGenerator))]
public class StageWaveGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var generator = (StageWaveGenerator)target;
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Enemy Quick Select", EditorStyles.boldLabel);

        if (generator.allEnemies == null || generator.allEnemies.Count == 0)
        {
            EditorGUILayout.HelpBox("allEnemies 리스트에 EnemyData를 등록하세요.", MessageType.Warning);
            return;
        }

        string[] enemyNames = generator.allEnemies.Select(e => e != null ? e.name : "NULL").ToArray();

        generator.enemyA = DrawEnemyPopup("Enemy A", generator.enemyA, enemyNames, generator.allEnemies);
        generator.enemyB = DrawEnemyPopup("Enemy B", generator.enemyB, enemyNames, generator.allEnemies);
        generator.enemyC = DrawEnemyPopup("Enemy C", generator.enemyC, enemyNames, generator.allEnemies);
        generator.enemyD = DrawEnemyPopup("Enemy D", generator.enemyD, enemyNames, generator.allEnemies);
        generator.boss = DrawEnemyPopup("Boss", generator.boss, enemyNames, generator.allEnemies);

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Generate Wave ScriptableObjects", GUILayout.Height(30)))
        {
            generator.GenerateWaveData();
        }
    }

    private EnemyData DrawEnemyPopup(string label, EnemyData current, string[] names, System.Collections.Generic.List<EnemyData> list)
    {
        int currentIndex = list.IndexOf(current);
        int selectedIndex = EditorGUILayout.Popup(label, Mathf.Max(currentIndex, 0), names);

        if (selectedIndex >= 0 && selectedIndex < list.Count)
            return list[selectedIndex];
        return current;
    }
}
