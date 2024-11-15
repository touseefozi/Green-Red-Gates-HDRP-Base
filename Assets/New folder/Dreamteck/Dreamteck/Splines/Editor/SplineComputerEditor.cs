namespace Dreamteck.Splines.Editor
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEditor;
    using Dreamteck.Editor;

    [CustomEditor(typeof(SplineComputer), true)]
    [CanEditMultipleObjects]
    public partial class SplineComputerEditor : Editor 
    {
        private List<int> selectedPoints = new List<int>();

        public int[] pointSelection
        {
            get
            {
                return selectedPoints.ToArray();
            }
        }
        public SplineComputer spline;
        public SplineComputer[] splines = new SplineComputer[0];


        protected bool closedOnMirror = false;

        public static bool hold = false;

        private DreamteckSplinesEditor pathEditor;
        private ComputerEditor computerEditor;
        private SplineTriggersEditor triggersEditor;
        private SplineDebugEditor debugEditor;

        private bool _rebuildSpline = false;

        public int selectedPointsCount
        {
            get { return selectedPoints.Count; }
            set { }
        }


        [MenuItem("GameObject/3D Object/Spline Computer")]
        private static void NewEmptySpline()
        {
            int count = GameObject.FindObjectsOfType<SplineComputer>().Length;
            string objName = "Spline";
            if (count > 0) objName += " " + count;
            GameObject obj = new GameObject(objName);
            obj.AddComponent<SplineComputer>();
            if (Selection.activeGameObject != null)
            {
                if (EditorUtility.DisplayDialog("Make child?", "Do you want to make the new spline a child of " + Selection.activeGameObject.name + "?", "Yes", "No"))
                {
                    obj.transform.parent = Selection.activeGameObject.transform;
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                }
            }
            Selection.activeGameObject = obj;
        }

        [MenuItem("GameObject/3D Object/Spline Node")]
        private static void NewSplineNode()
        {
            int count = Object.FindObjectsOfType<Node>().Length;
            string objName = "Node";
            if (count > 0) objName += " " + count;
            GameObject obj = new GameObject(objName);
            obj.AddComponent<Node>();
            if(Selection.activeGameObject != null)
            {
                if(EditorUtility.DisplayDialog("Make child?", "Do you want to make the new node a child of " + Selection.activeGameObject.name + "?", "Yes", "No"))
                {
                    obj.transform.parent = Selection.activeGameObject.transform;
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                }
            }
            Selection.activeGameObject = obj;
        }

        public void UndoRedoPerformed()
        {
            pathEditor.points = spline.GetPoints();
            pathEditor.UndoRedoPerformed();
            spline.EditorUpdateConnectedNodes();
            spline.Rebuild();
        }

        void OnEnable()
        {
            splines = new SplineComputer[targets.Length];
            for (int i = 0; i < splines.Length; i++)
            {
                splines[i] = (SplineComputer)targets[i];
                splines[i].EditorAwake();
                if (splines[i].editorAlwaysDraw)
                {
                    DSSplineDrawer.RegisterComputer(splines[i]);
                }
            }
            spline = splines[0];
            InitializeSplineEditor();
            InitializeComputerEditor();
            debugEditor = new SplineDebugEditor(spline, serializedObject);
            debugEditor.undoHandler += RecordUndo;
            debugEditor.repaintHandler += OnRepaint;
            triggersEditor = new SplineTriggersEditor(spline);
            triggersEditor.undoHandler += RecordUndo;
            triggersEditor.repaintHandler += OnRepaint;
            hold = false;
#if UNITY_2019_1_OR_NEWER
            SceneView.beforeSceneGui += BeforeSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
#else
            SceneView.onSceneGUIDelegate += BeforeSceneGUI;
            SceneView.onSceneGUIDelegate += DuringSceneGUI;
#endif
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        void BeforeSceneGUI(SceneView current)
        {
            pathEditor.BeforeSceneGUI(current);

            if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.button == 0)
                {
                    for (int i = 0; i < splines.Length; i++)
                    {
                        if (splines[i].editorUpdateMode == SplineComputer.EditorUpdateMode.OnMouseUp)
                        {
                            splines[i].RebuildImmediate();
                        }
                    }
                }
            }
        }

        void InitializeSplineEditor()
        {
            pathEditor = new DreamteckSplinesEditor(spline, "DreamteckSplines");
            pathEditor.undoHandler = RecordUndo;
            pathEditor.repaintHandler = OnRepaint;
            pathEditor.space = (SplineEditor.Space)SplinePrefs.pointEditSpace;
        }

        void InitializeComputerEditor()
        {
            computerEditor = new ComputerEditor(splines, serializedObject, pathEditor);
            computerEditor.undoHandler = RecordUndo;
            computerEditor.repaintHandler = OnRepaint;
        }

        void RecordUndo(string title)
        {
            for (int i = 0; i < splines.Length; i++)
            {
                Undo.RecordObject(splines[i], title);
            }
        }

        void OnRepaint()
        {
            SceneView.RepaintAll();
            Repaint();
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
#if UNITY_2019_1_OR_NEWER
            SceneView.beforeSceneGui -= BeforeSceneGUI;
            SceneView.duringSceneGui -= DuringSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= BeforeSceneGUI;
            SceneView.onSceneGUIDelegate -= DuringSceneGUI;
#endif
            if (pathEditor != null) pathEditor.Destroy();
            if (computerEditor != null) computerEditor.Destroy();
            if (debugEditor != null) debugEditor.Destroy();
            if (triggersEditor != null) triggersEditor.Destroy();
        }

        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _rebuildSpline = true;
            }
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                _rebuildSpline = true;
            }
            base.OnInspectorGUI();
            spline = (SplineComputer)target;
            Undo.RecordObject(spline, "Edit Points");

            if (splines.Length == 1)
            {
                SplineEditorGUI.BeginContainerBox(ref pathEditor.open, "Edit");
                if (pathEditor.open)
                {
                    SplineEditor.Space lastSpace = pathEditor.space;
                    pathEditor.DrawInspector();
                    if (lastSpace != pathEditor.space)
                    {
                        SplinePrefs.pointEditSpace = (SplineComputer.Space)pathEditor.space;
                        SplinePrefs.SavePrefs();
                    }
                }
                else if (pathEditor.lastEditorTool != Tool.None && Tools.current == Tool.None)
                {
                    Tools.current = pathEditor.lastEditorTool;
                }
                SplineEditorGUI.EndContainerBox();
            }

            SplineEditorGUI.BeginContainerBox(ref computerEditor.open, "Spline Computer");
            if (computerEditor.open) computerEditor.DrawInspector();
            SplineEditorGUI.EndContainerBox();

            if (splines.Length == 1)
            {
                SplineEditorGUI.BeginContainerBox(ref triggersEditor.open, "Triggers");
                if (triggersEditor.open) triggersEditor.DrawInspector();
                SplineEditorGUI.EndContainerBox();
            }

            SplineEditorGUI.BeginContainerBox(ref debugEditor.open, "Editor Properties");
            if (debugEditor.open) debugEditor.DrawInspector();
            SplineEditorGUI.EndContainerBox();

            if (GUI.changed)
            {
               if (spline.isClosed) pathEditor.points[pathEditor.points.Length - 1] = pathEditor.points[0];
                EditorUtility.SetDirty(spline);
            }


            if (Event.current.type == EventType.Layout && _rebuildSpline)
            {
                for (int i = 0; i < splines.Length; i++)
                {
                    if (splines[i].editorUpdateMode == SplineComputer.EditorUpdateMode.OnMouseUp)
                    {
                        splines[i].RebuildImmediate(true);
                    }
                }
                _rebuildSpline = false;
            }

        }

        public bool IsPointSelected(int index)
        {
            return selectedPoints.Contains(index);
        }

        private void DuringSceneGUI(SceneView currentSceneView)
        {
            debugEditor.DrawScene(currentSceneView);
            computerEditor.drawComputer = !(pathEditor.currentModule is CreatePointModule);
            computerEditor.DrawScene(currentSceneView);
            if (splines.Length == 1 && triggersEditor.open) triggersEditor.DrawScene(currentSceneView);
            if (splines.Length == 1 && pathEditor.open) pathEditor.DrawScene(currentSceneView);
        }
    }
}
