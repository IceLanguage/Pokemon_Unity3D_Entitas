using UnityEngine;
using UnityEditor;

using UnityEngine;
using System.Collections;
using UnityEditor;
public class EditorTest : EditorWindow
{
    enum EnumTest { 网, 虫, 测, 试 }
    static EditorTest window;
    [MenuItem("Tools/测试编辑器功能")]
    static void Test()
    {
        window = (EditorTest)EditorWindow.GetWindow(typeof(EditorTest), false, "测试编辑器功能");
        window.Show();
    }
    #region 属性
    int testInt = 0;
    float testFloat = 0;
    float floatSlider = 0;
    float maxValue = 20;
    float minValue = -50;
    string testStr = "网虫虫";
    bool testBool = true;
    int toolbarOption = 0;
    string[] toolbarStr = new string[] { "这", "是", "Toolbar" };
    EnumTest enumTest;
    EnumTest enumTest2;
    int enumInt = 0;
    int selectedSize = 1;
    string[] names = { "网", "虫", "测", "试" };
    int[] sizes = { 1, 2, 4 };
    string tagStr = "";
    int layerInt = 0;
    int maskInt = 0;
    Vector3 testVector3;
    Color testColor;
    Rect testRect;
    GameObject gameObject;
    Texture texture;
    bool isShowScrollView = false;
    Vector2 scrollPosition;
    #endregion
    private void OnGUI()
    {
        this.Repaint();     // 强制重绘
        if (secondWindow || secondWindow2)
            GUI.enabled = false;
        GUILayout.Label(testStr);        // 文本格式:网虫虫      不可输入、不可选
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;    //设置背景填充  
        fontStyle.normal.textColor = new Color(1, 0, 0);   //设置字体颜色  
        fontStyle.fontStyle = FontStyle.BoldAndItalic;      // 字体加粗倾斜
        fontStyle.fontSize = 18;       //字体大小  
        GUILayout.Label(testStr, fontStyle);       // 文本格式:网虫虫      不可输入、不可选  添加字体样式
        GUILayout.TextField(testStr);        // 文本格式:网虫虫      可输入、不可选
        EditorGUILayout.LabelField("姓名：", testStr);       // 文本格式: 姓名：网虫虫      不可输入、不可选
        testStr = EditorGUILayout.TextField("姓名：", testStr);       // 文本格式:姓名：网虫虫      可输入、不可选
        testInt = EditorGUILayout.IntField("IntField：", testInt);
        testFloat = EditorGUILayout.FloatField("FloatField：", testFloat);
        testStr = GUILayout.TextArea(testStr, GUILayout.Height(40));    // 区域输入文本
        EditorGUILayout.SelectableLabel(testStr);       // 可选择文本
        testStr = GUILayout.PasswordField(testStr, "*"[0]);
        testStr = EditorGUILayout.PasswordField("密码:", testStr);
        floatSlider = EditorGUILayout.Slider(floatSlider, 1, 100);
        EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, -100, 100);
        testBool = GUILayout.Toggle(testBool, "开关");
        testBool = EditorGUILayout.Toggle("开关：", testBool);
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarStr);
        switch (toolbarOption)
        {
            case 0:
                GUILayout.Label("1111111111111111111");
                break;
            case 1:
                GUILayout.Label("2222222222222222222");
                break;
            case 2:
                GUILayout.Label("3333333333333333333");
                break;
        }
        EditorGUILayout.Space();        // 空一行
        enumTest = (EnumTest)EditorGUILayout.EnumPopup("Enum类型Popup：", enumTest);
        enumTest2 = (EnumTest)EditorGUILayout.EnumMaskField("Enum枚举多选:", enumTest2);
        enumInt = EditorGUILayout.Popup("String类型Popup：", enumInt, names);
        selectedSize = EditorGUILayout.IntPopup("Int类型Popup: ", selectedSize, names, sizes);
        tagStr = EditorGUILayout.TagField("选择Tag:", tagStr);
        layerInt = EditorGUILayout.LayerField("选择Layer:", layerInt);
        maskInt = EditorGUILayout.MaskField("数组多选:", maskInt, names);
        testColor = EditorGUILayout.ColorField("颜色:", testColor);
        GUI.backgroundColor = Color.magenta;     // 修改背景颜色
        testVector3 = EditorGUILayout.Vector3Field("Vector3坐标:", testVector3);
        GUI.backgroundColor = Color.green;
        testRect = EditorGUILayout.RectField("Rect尺寸:", testRect);
        GUI.backgroundColor = Color.gray * 1.8f;        // 恢复背景默认颜色
        gameObject = (GameObject)EditorGUILayout.ObjectField("任意类型 举例GameObject:", gameObject, typeof(GameObject));
        texture = EditorGUILayout.ObjectField("任意类型 举例贴图", texture, typeof(Texture), true) as Texture;
        GUILayout.BeginHorizontal();
        GUILayout.Label("横向自动排列演示：");
        testStr = GUILayout.PasswordField(testStr, "*"[0]);
        testBool = GUILayout.Toggle(testBool, "开关");
        GUILayout.Button("按钮");
        GUILayout.EndHorizontal();
        if (GUILayout.Button("点击按钮 弹出系统提示消息"))
            ShowNotification(new GUIContent("这是网虫虫提示消息~~"));
        if (GUILayout.Button("点击按钮 显示滚动视图"))
            isShowScrollView = !isShowScrollView;
        if (isShowScrollView)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i>=0&i<100; i++  )
            {
                GUILayout.Label(i.ToString());
            }
            GUILayout.EndScrollView();
        }
        if (GUILayout.Button("点击按钮 显示二级窗口"))
            secondWindow = !secondWindow;
        if (secondWindow)
        {
            GUI.enabled = true;
            BeginWindows();
            secondWindowRect = GUILayout.Window(1, secondWindowRect, SecondWindow, "二级窗口");
            EndWindows();
        }
        if (GUILayout.Button("点击按钮 绘制图形"))
            secondWindow2 = !secondWindow2;
        if (secondWindow2)
        {
            GUI.backgroundColor = Color.red / 2;     // 修改背景颜色
            GUI.enabled = true;
            BeginWindows();
            secondWindowRect2 = GUILayout.Window(2, secondWindowRect2, SecondWindow2, "绘制图形");
            EndWindows();
        }
        GUI.backgroundColor = Color.gray * 1.8f;        // 恢复背景默认颜色
        GUI.enabled = false;
        GUILayout.Button("置灰按钮");
        GUI.enabled = true;
    }
    Rect secondWindowRect = new Rect(0, 0, 400, 400);
    bool secondWindow = false;
    void SecondWindow(int unusedWindowID)
    {
        Application.targetFrameRate = EditorGUILayout.IntSlider("限定帧率：", Application.targetFrameRate, 10, 300);
        Application.runInBackground = EditorGUILayout.Toggle("允许Unity后台运行：", Application.runInBackground);
        gameObject = (GameObject)EditorGUILayout.ObjectField("当前选中的物体：", Selection.activeGameObject, typeof(GameObject));
        EditorGUILayout.Vector3Field("鼠标在Scene视图的坐标：", mousePosition);
        EditorGUILayout.Vector3Field("鼠标在当前二级窗口坐标：", Event.current.mousePosition);
        hitGo = (GameObject)EditorGUILayout.ObjectField("鼠标发送射线：", hitGo != null ? hitGo : null, typeof(GameObject));
        //GUILayout.Label("UsedTextureCount: "   UnityStats.usedTextureCount);
        //GUILayout.Label("UsedTextureMemorySize: "(UnityStats.usedTextureMemorySize / 1000000f   "Mb"));
        //GUILayout.Label("RenderTextureCount: "   UnityStats.renderTextureCount);
        //GUILayout.Label("FrameTime: "   UnityStats.frameTime);
        //GUILayout.Label("RenderTime: "   UnityStats.renderTime);
        //GUILayout.Label("DrawCalls: "   UnityStats.drawCalls);
        //GUILayout.Label("Batchs: "   UnityStats.batches);
        //GUILayout.Label("Static Batch DC: "   UnityStats.staticBatchedDrawCalls);
        //GUILayout.Label("Static Batch: "   UnityStats.staticBatches);
        //GUILayout.Label("DynamicBatch DC: "   UnityStats.dynamicBatchedDrawCalls);
        //GUILayout.Label("DynamicBatch: "   UnityStats.dynamicBatches);
        //GUILayout.Label("Triangles: "   UnityStats.triangles);
        //GUILayout.Label("Vertices: "   UnityStats.vertices);
        if (GUILayout.Button("关闭二级窗口"))
            secondWindow = false;
        GUI.DragWindow();//画出子窗口
    }
    Rect secondWindowRect2 = new Rect(0, 0, 400, 400);
    bool secondWindow2 = false;
    int capSize = -50;
    Vector3 capEuler = new Vector3(200, 200, 200);
    void SecondWindow2(int unusedWindowID)
    {
        capSize = EditorGUILayout.IntField("尺寸：", capSize);
        capEuler = EditorGUILayout.Vector3Field("testVector3：", capEuler);
        if (GUILayout.Button("关闭绘制图形"))
            secondWindow2 = false;
        Handles.color = Color.red;
        Handles.DrawLine(new Vector2(75, 100), new Vector3(150, 200));
        Handles.CircleCap(1, new Vector2(300, 150), Quaternion.identity, capSize);
        Handles.color = Color.green;
        Handles.SphereCap(2, new Vector2(100, 250), Quaternion.Euler(capEuler), capSize);
        Handles.CubeCap(3, new Vector2(300, 250), Quaternion.Euler(capEuler), capSize);
        Handles.color = Color.blue;
        Handles.CylinderCap(4, new Vector2(100, 350), Quaternion.Euler(capEuler), capSize);
        Handles.ConeCap(5, new Vector2(300, 350), Quaternion.Euler(capEuler), capSize);
        GUI.DragWindow();//画出子窗口
    }
    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate = SceneGUI;
    }
    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= SceneGUI;
    }
    GameObject hitGo;
    Vector3 mousePosition;
    void SceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseMove)
        {
            mousePosition = Event.current.mousePosition;
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                hitGo = hit.collider.gameObject;
            }
            else
                hitGo = null;
        }
    }
}