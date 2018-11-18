using UnityEngine;
using UnityEditor;
using System.IO;

public class NewEditorScript1 : ScriptableObject
{
    [MenuItem("Tools/MyTool/Do It #&E")]
    static void DoIt()
    {
        var objs = Selection.gameObjects;
        if (null == objs || objs.Length <= 0)
            return;
        var obj1 = objs[0];
        var obj2 = objs[1];
        if (obj1.name.Contains("("))
        {
            obj1.transform.position = obj2.transform.position;
            obj1.GetComponent<StationItem>().Index = obj2.GetComponent<StationItem>().Index;
            obj1.name = obj2.name;
        }
        else if (obj2.name.Contains("("))
        {
            obj2.transform.position = obj1.transform.position;
            obj2.GetComponent<StationItem>().Index = obj1.GetComponent<StationItem>().Index;
            obj2.name = obj1.name;
        }

    }
    [MenuItem("Tools/Create AssetBundles By themselves")]
    static void CreateAssetBundleThemelves()
    {
        ////获取要打包的对象（在Project视图中）  
        //Object[] selects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        ////遍历选中的对象  
        //foreach (Object obj in selects)
        //{
        //    //这里建立一个本地测试  
        //    //注意本地测试中可以是任意的文件，但是到了移动平台只能读取路径StreamingAssets里面的  
        //    //StreamingAssets是只读路径，不能写入  
        //    string targetPath = Application.dataPath + "/AssetBundleLearn/StreamingAssets/" + obj.name + ".assetbundle";//文件的后缀名是assetbundle和unity都可以  
        //    if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies))
        //    {

        //        Debug.Log(obj.name + "is packed successfully!");
        //    }
        //    else
        //    {
        //        Debug.Log(obj.name + "is packed failly!");
        //    }
        //}
        ////刷新编辑器（不写的话要手动刷新,否则打包的资源不能及时在Project视图内显示）  
        //AssetDatabase.Refresh();
    }
}

public class MyWindow : EditorWindow
{
    [MenuItem("Tools/MyWindow")]//在unity菜单Window下有MyWindow选项
    static void Init()
    {
        MyWindow myWindow = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow), false, "MyWindow", true);//创建窗口

        myWindow.Show();//展示
    }
    private GameObject HUD;

    void OnGUI()
    {
        //    EditorGUILayout.LabelField(EditorWindow.focusedWindow.ToString());
        //    HUD = EditorGUILayout.ObjectField("源", HUD, typeof(GameObject), true) as GameObject;

        //    if (GUILayout.Button("应用"))
        //    {
        //        var objs = Selection.gameObjects;
        //        for (int i = 0; i < objs.Length; i++)
        //        {
        //            var bg = GameObject.Instantiate<GameObject>(HUD);

        //        }
        //    }
    }
}
