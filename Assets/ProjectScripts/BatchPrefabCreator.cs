using UnityEngine;
using UnityEditor;

public class BatchPrefabCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Food Prefabs (FoodSprites2)")]
    static void CreatePrefabs()
    {
        string sourceFolder = "Assets/FoodSprites2";
        string saveFolder = "Assets/Resources/Foods2";

        // Ensure save folder exists
        if (!AssetDatabase.IsValidFolder(saveFolder))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Foods2");
        }

        // Find all sprites in FoodSprites2
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { sourceFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null) continue;

            // Create GameObject
            GameObject go = new GameObject(sprite.name);

            // Add components
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            BoxCollider col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;

            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            go.AddComponent<FoodItem>();
            go.AddComponent<FoodMover>();

            // Save prefab
            string prefabPath = saveFolder + "/" + sprite.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

            GameObject.DestroyImmediate(go);
        }

        AssetDatabase.Refresh();
        Debug.Log("Prefabs created ONLY from FoodSprites2!");
    }
}