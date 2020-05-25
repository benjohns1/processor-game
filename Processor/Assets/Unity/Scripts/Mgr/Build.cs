using System;
using System.Collections.Generic;
using Unity.Scripts.UI;
using UnityEngine;
using Unity.Scripts.Targetable;

namespace Unity.Scripts.Mgr
{

    [RequireComponent(typeof(Graph), typeof(Input), typeof(Grid))]
    public class Build : MonoBehaviour
    {
        [SerializeField] private Processor processorPrefab;
        [SerializeField] private GameObject connectorPrefab;
        [SerializeField] private LineRenderer currentLine;

        public enum Selection
        {
            None,
            Line,
            Delete,
            AddOne,
            SubOne
        }

        [Serializable]
        public class BuildButton
        {
            public Selection selection;
            public UnityEngine.UI.Button button;
            public Button handler;

            public BuildButton(Selection s)
            {
                selection = s;
            }
        }

        // Build menu
        [SerializeField] private List<BuildButton> buttons = new List<BuildButton>
        {
            new BuildButton(Selection.Line),
            new BuildButton(Selection.Delete),
            new BuildButton(Selection.AddOne),
            new BuildButton(Selection.SubOne),
        };
        
        private GameObject currentLineStart;
        private Selection selection;
        private ITargetable target;
        private const float LineZ = 1;
        private const float CurrentLineZ = -1;
        private const float PlaceZ = 0;
        
        private Graph mGraph;
        private Input mInput;
        private Grid grid;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            mGraph = GetComponent<Graph>();
            mInput = GetComponent<Input>();
            mInput.Toggled += ActionToggled;
            InitBuildMenu();

        }
        
        private void InitBuildMenu()
        {
            var group = new List<Button>();
            foreach (var btn in buttons)
            {
                if (btn.button == null)
                {
                    throw new Exception("button required for " + btn.selection);
                }

                btn.handler = new Button(btn.button);
                btn.handler.Activated += (sender, args) => Select(btn.selection); 
                group.Add(btn.handler);
            }
            foreach (var btn in buttons)
            {
                btn.handler.SetGroup(group);
            }
        }

        private void Update()
        {
            DrawCurrentLine();
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            if (selection != Selection.Delete)
            {
                return;
            }
            
            if (target != null)
            {
                target.Deselect();
                target = null;
            }

            var (hit, go) = Trace();
            if (!hit)
            {
                return;
            }

            if (!(go.GetComponent(typeof(ITargetable)) is ITargetable newTarget))
            {
                return;
            }
            target = newTarget;
            target.Select();
        }

        private void ActionToggled(object _, Input.ToggledArgs args)
        {
            if (args.On(Input.Action.Quit))
            {
                Application.Quit();
                return;
            }
            
            if (currentLine.positionCount != 0 && args.Off(Input.Action.Primary))
            {
                StopDrawingLine();
            }

            if (!args.On(Input.Action.Primary) || args.PointerOverUi)
            {
                return;
            }
            
            PlaceSelection(selection);
        }

        private void PlaceSelection(Selection s)
        {
            switch (s)
            {
                case Selection.Line:
                    StartDrawingLine();
                    return;
                case Selection.Delete:
                    DeleteTarget();
                    return;
                case Selection.AddOne:
                    CreateProcessorNode(ProcessType.AddOne);
                    return;
                case Selection.SubOne:
                    CreateProcessorNode(ProcessType.SubOne);
                    return;
                case Selection.None:
                    break;
                default:
                    throw new Exception("unhandled processor type " + s);
            }
        }

        private void DeleteTarget()
        {
            if (target == null)
            {
                return;
            }

            var go = target.GameObject();
            if (!(go.GetComponent(typeof(IDeletable)) is IDeletable deletable))
            {
                return;
            }

            if (!deletable.Delete())
            {
                return;
            }
            
            target = null;
        }

        private void DrawCurrentLine()
        {
            if (currentLine.positionCount == 0)
            {
                return;
            }

            if (currentLine.positionCount < 2)
            {
                return;
            }

            currentLine.SetPosition(1, mInput.CursorPosition(CurrentLineZ));
        }

        private void StartDrawingLine()
        {
            var (hit, go) = TraceConnectable();
            if (!hit) return;
            
            var pos = go.transform.position;

            currentLine.enabled = false;
            currentLine.positionCount = 2;
            currentLine.SetPosition(0, grid.Pos(pos, CurrentLineZ));
            DrawCurrentLine();
            currentLineStart = go;
            currentLine.enabled = true;
        }

        private (bool Hit, GameObject Go) Trace()
        {
            var hit = Physics2D.Raycast(mInput.CursorPosition(), Vector2.zero);
            return hit.collider == null ? (false, null) : (true, hit.collider.gameObject);
        }

        private (bool hit, GameObject Go) TraceConnectable()
        {
            var (hit, traced) = Trace();
            if (!hit)
            {
                return (false, null);
            }

            if (!(traced.GetComponent(typeof(IConnectable)) is IConnectable connectable))
            {
                return (false, null);
            }

            var go = connectable.GameObject();
            return go == null ? (false, null) : (true, go);
        }

        private void StopDrawingLine()
        {
            if (currentLine.positionCount == 0)
            {
                return;
            }

            var (hit, go) = TraceConnectable();
            if (!hit || go.Equals(currentLineStart))
            {
                currentLine.positionCount = 0;
                return;
            }

            mGraph.CreateTransportConnector(new Graph.DrawConnector{
                Prefab = connectorPrefab,
                Start = grid.Pos(currentLineStart.transform.position, LineZ),
                Upstream = currentLineStart.GetComponent(typeof(INode)) as INode,
                End = grid.Pos(go.transform.position, LineZ),
                Downstream = go.GetComponent(typeof(INode)) as INode,
                Z = LineZ
            });

            currentLine.positionCount = 0;
            currentLineStart = null;
        }

        private void CreateProcessorNode(ProcessType process)
        {
            var gridPos = grid.Pos(mInput.CursorPosition(), PlaceZ);
            mGraph.CreateProcessorNode(processorPrefab, process, gridPos);
        }

        private void Select(Selection s)
        {
            mInput.InputHandled();
            selection = s;
        }
    }
}