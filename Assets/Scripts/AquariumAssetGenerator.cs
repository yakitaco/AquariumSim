using UnityEngine;

public class AquariumAssetGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public Material baseMaterial; // 空のMaterial（Standardシェーダー）をセットしてください（なくても自動生成します）

    // Inspector上でスクリプト名を右クリックして実行できるメニューを追加
    [ContextMenu("Generate All Aquarium Assets")]
    public void GenerateAll()
    {
        CreateTank();
        CreateNeonTetra();
        CreateGoldfish();
        CreateWaterPlant();
        CreateRock();
        CreateFood();
        CreateMoss();
        
        Debug.Log("すべてのアセットを生成しました。ヒエラルキーからProjectウィンドウにドラッグ＆ドロップしてPrefab化してください。");
    }

    [ContextMenu("Generate Tank Only")]
    public void CreateTank()
    {
        GameObject tankRoot = new GameObject("Pref_AquariumTank");

        // 砂（底面）
        GameObject sand = CreatePrimitive(PrimitiveType.Cube, "Sand", tankRoot.transform, new Vector3(0, -2.5f, 0), new Vector3(10f, 0.5f, 5f), new Color(0.9f, 0.8f, 0.6f));
        
        // ガラス面（透明）
        Color glassColor = new Color(0.2f, 0.5f, 1f, 0.2f);
        GameObject backGlass = CreatePrimitive(PrimitiveType.Cube, "Glass_Back", tankRoot.transform, new Vector3(0, 0, 2.5f), new Vector3(10f, 5f, 0.1f), glassColor, true);
        GameObject frontGlass = CreatePrimitive(PrimitiveType.Cube, "Glass_Front", tankRoot.transform, new Vector3(0, 0, -2.5f), new Vector3(10f, 5f, 0.1f), glassColor, true);
        GameObject leftGlass = CreatePrimitive(PrimitiveType.Cube, "Glass_Left", tankRoot.transform, new Vector3(-5f, 0, 0), new Vector3(0.1f, 5f, 5f), glassColor, true);
        GameObject rightGlass = CreatePrimitive(PrimitiveType.Cube, "Glass_Right", tankRoot.transform, new Vector3(5f, 0, 0), new Vector3(0.1f, 5f, 5f), glassColor, true);
        
        // 水槽の当たり判定用（魚が外に出ないようにする）
        BoxCollider bounds = tankRoot.AddComponent<BoxCollider>();
        bounds.size = new Vector3(9.8f, 4.8f, 4.8f);
        bounds.isTrigger = true; // 実際の物理衝突ではなく領域判定用
    }

    [ContextMenu("Generate Fish - Neon Tetra")]
    public void CreateNeonTetra()
    {
        GameObject fishRoot = new GameObject("Pref_Fish_NeonTetra");

        // 胴体 (Capsuleを寝かせる)
        GameObject body = CreatePrimitive(PrimitiveType.Capsule, "Body", fishRoot.transform, Vector3.zero, new Vector3(0.15f, 0.3f, 0.15f), new Color(0.1f, 0.6f, 0.9f));
        body.transform.rotation = Quaternion.Euler(90, 0, 0); // 前を向かせる

        // 赤い模様 (細長いCube)
        GameObject stripe = CreatePrimitive(PrimitiveType.Cube, "Stripe", fishRoot.transform, new Vector3(0, -0.05f, -0.05f), new Vector3(0.16f, 0.05f, 0.4f), Color.red);

        // 尻尾 (薄いCube)
        GameObject tail = CreatePrimitive(PrimitiveType.Cube, "Tail", fishRoot.transform, new Vector3(0, 0, -0.3f), new Vector3(0.02f, 0.15f, 0.15f), new Color(0.8f, 0.8f, 0.8f, 0.5f), true);

        // 目
        CreatePrimitive(PrimitiveType.Sphere, "Eye_R", fishRoot.transform, new Vector3(0.08f, 0.05f, 0.2f), new Vector3(0.04f, 0.04f, 0.04f), Color.black);
        CreatePrimitive(PrimitiveType.Sphere, "Eye_L", fishRoot.transform, new Vector3(-0.08f, 0.05f, 0.2f), new Vector3(0.04f, 0.04f, 0.04f), Color.black);

        AddFishComponents(fishRoot);
    }

    [ContextMenu("Generate Fish - Goldfish")]
    public void CreateGoldfish()
    {
        GameObject fishRoot = new GameObject("Pref_Fish_Goldfish");

        Color goldColor = new Color(1f, 0.5f, 0f);

        // 胴体 (丸っこいSphere)
        GameObject body = CreatePrimitive(PrimitiveType.Sphere, "Body", fishRoot.transform, Vector3.zero, new Vector3(0.3f, 0.4f, 0.5f), goldColor);

        // 大きな尻尾
        GameObject tail = CreatePrimitive(PrimitiveType.Cube, "Tail", fishRoot.transform, new Vector3(0, 0, -0.35f), new Vector3(0.05f, 0.4f, 0.3f), goldColor);
        tail.transform.localRotation = Quaternion.Euler(15, 0, 0);

        // 背びれ
        GameObject fin = CreatePrimitive(PrimitiveType.Cube, "DorsalFin", fishRoot.transform, new Vector3(0, 0.25f, 0), new Vector3(0.02f, 0.15f, 0.25f), goldColor);

        // 目
        CreatePrimitive(PrimitiveType.Sphere, "Eye_R", fishRoot.transform, new Vector3(0.15f, 0.05f, 0.15f), new Vector3(0.05f, 0.05f, 0.05f), Color.black);
        CreatePrimitive(PrimitiveType.Sphere, "Eye_L", fishRoot.transform, new Vector3(-0.15f, 0.05f, 0.15f), new Vector3(0.05f, 0.05f, 0.05f), Color.black);

        AddFishComponents(fishRoot);
    }

    [ContextMenu("Generate Decor - Water Plant")]
    public void CreateWaterPlant()
    {
        GameObject plantRoot = new GameObject("Pref_Decor_WaterPlant");
        Color leafColor = new Color(0.1f, 0.8f, 0.2f);

        for (int i = 0; i < 5; i++)
        {
            GameObject leaf = CreatePrimitive(PrimitiveType.Capsule, $"Leaf_{i}", plantRoot.transform, Vector3.zero, new Vector3(0.1f, 1.0f, 0.1f), leafColor);
            
            // 葉っぱを放射状に広げる
            float height = Random.Range(0.5f, 1.5f);
            leaf.transform.localScale = new Vector3(0.1f, height, 0.1f);
            leaf.transform.localPosition = new Vector3(Random.Range(-0.2f, 0.2f), height * 0.8f, Random.Range(-0.2f, 0.2f));
            leaf.transform.localRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(0, 360), Random.Range(-30, 30));
        }
    }

    [ContextMenu("Generate Decor - Rock")]
    public void CreateRock()
    {
        GameObject rockRoot = new GameObject("Pref_Decor_Rock");
        
        // ゴツゴツした岩を表現するためにスケールを歪ませたSphere
        GameObject rock = CreatePrimitive(PrimitiveType.Sphere, "RockMesh", rockRoot.transform, new Vector3(0, 0.5f, 0), new Vector3(1.5f, 1.0f, 1.2f), Color.gray);
        rock.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    [ContextMenu("Generate Food")]
    public void CreateFood()
    {
        GameObject foodRoot = CreatePrimitive(PrimitiveType.Sphere, "Pref_Food", null, Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f), new Color(0.6f, 0.4f, 0.2f));
        foodRoot.AddComponent<Rigidbody>(); // 物理演算で沈むように
        // foodRoot.AddComponent<Food>(); // 前に作成したFoodスクリプトがあればアタッチ
    }

    [ContextMenu("Generate Moss (For Cleaning)")]
    public void CreateMoss()
    {
        // ガラス面に張り付くコケ（半透明の緑の板）
        GameObject mossRoot = CreatePrimitive(PrimitiveType.Quad, "Pref_Moss", null, Vector3.zero, new Vector3(1.5f, 1.5f, 1f), new Color(0.2f, 0.6f, 0.1f, 0.6f), true);
        mossRoot.layer = LayerMask.NameToLayer("Default"); // 本来は専用のMossレイヤーを設定
        // mossRoot.AddComponent<MossObject>(); // 前に作成したMossObjectスクリプトがあればアタッチ
    }

    // ==========================================
    // ヘルパーメソッド群
    // ==========================================

    private GameObject CreatePrimitive(PrimitiveType type, string objName, Transform parent, Vector3 localPos, Vector3 scale, Color color, bool isTransparent = false)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = objName;
        
        if (parent != null) obj.transform.SetParent(parent);
        
        obj.transform.localPosition = localPos;
        obj.transform.localScale = scale;

        // マテリアルの設定
        Renderer renderer = obj.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;

        if (isTransparent)
        {
            // Standardシェーダーを透明モードにする処理
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        renderer.material = mat;

        // 魚のパーツなどはコライダーが邪魔になることがあるので、ルート以外は削除（お好みで調整）
        if (parent != null)
        {
            DestroyImmediate(obj.GetComponent<Collider>());
        }

        return obj;
    }

    private void AddFishComponents(GameObject fishRoot)
    {
        // 魚のルートオブジェクトには全体の当たり判定をつける
        BoxCollider col = fishRoot.AddComponent<BoxCollider>();
        col.size = new Vector3(0.5f, 0.5f, 1.0f);

        // 前のステップで作成したスクリプト群をアタッチ（コメントアウトを外して利用）
        fishRoot.AddComponent<FishStatus>();
        fishRoot.AddComponent<FishMovement>();
        fishRoot.AddComponent<BaseFishAI>();
        fishRoot.AddComponent<FishBreeding>();
    }
}