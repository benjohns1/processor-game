using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Scripts.Mgr
{
    internal enum Selection
    {
        Line,
        Delete,
        AddOne,
        SubOne
    }

    [RequireComponent(typeof(Graph), typeof(Input))]
    public class Build : MonoBehaviour
    {
        [SerializeField] private Processor processorPrefab;
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private GameObject connectorPrefab;
        [SerializeField] private LineRenderer currentLine;
        [SerializeField] private Button buttonAddOne;
        [SerializeField] private Button buttonSubOne;
        [SerializeField] private Button buttonLine;
        [SerializeField] private Button buttonDelete;
        
        private GameObject currentLineStart;
        private Selection selection;
        private float posRound;
        private const float LineZ = 1;
        private const float CurrentLineZ = -1;
        private const float PlaceZ = 0;
        
        private Graph mGraph;
        private Input mInput;

        private void Awake()
        {
            posRound = 1 / gridSize;
            buttonLine.onClick.AddListener(delegate { Select(Selection.Line); });
            buttonDelete.onClick.AddListener(delegate { Select(Selection.Delete); });
            buttonAddOne.onClick.AddListener(delegate { Select(Selection.AddOne); });
            buttonSubOne.onClick.AddListener(delegate { Select(Selection.SubOne); });

            mGraph = GetComponent<Graph>();
            mInput = GetComponent<Input>();
            mInput.Toggled += (sender, args) => ActionToggled(args);
        }

        private float OnGrid(float x)
        {
            return Mathf.Round(x * posRound) / posRound;
        }

        private Vector3 OnGrid(Vector2 xy, float z)
        {
            return new Vector3(OnGrid(xy.x), OnGrid(xy.y), z);
        }

        private void Update()
        {
            DrawCurrentLine();
        }

        private void ActionToggled(Input.ToggledArgs args)
        {
            if (currentLine.positionCount != 0 && args.Off(Input.Action.Primary))
            {
                StopDrawingLine();
            }

            if (!args.On(Input.Action.Primary) || args.PointerOverUi)
            {
                return;
            }

            if (selection == Selection.Line)
            {
                StartDrawingLine();
                return;
            } 
            
            PlaceProcessor(selection);
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
            var (hit, go) = ScreenTrace();
            if (!hit)
            {
                return;
            }

            var pos = go.transform.position;

            currentLine.enabled = false;
            currentLine.positionCount = 2;
            currentLine.SetPosition(0, OnGrid(pos, CurrentLineZ));
            DrawCurrentLine();
            currentLineStart = go;
            currentLine.enabled = true;
        }

        private (bool Hit, GameObject Go) ScreenTrace()
        {
            var hit = Physics2D.Raycast(mInput.CursorPosition(), Vector2.zero);
            return hit.collider == null ? (false, null) : (true, hit.collider.gameObject);
        }

        private void StopDrawingLine()
        {
            if (currentLine.positionCount == 0)
            {
                return;
            }

            var (hit, go) = ScreenTrace();
            if (!hit)
            {
                currentLine.positionCount = 0;
                return;
            }

            if (go.Equals(currentLineStart))
            {
                currentLine.positionCount = 0;
                return;
            }

            mGraph.CreateTransportConnector(new Graph.DrawConnector{
                Prefab = connectorPrefab,
                Start = OnGrid(currentLineStart.transform.position, LineZ),
                Upstream = currentLineStart.GetComponentInParent(typeof(INode)) as INode,
                End = OnGrid(go.transform.position, LineZ),
                Downstream = go.GetComponentInParent(typeof(INode)) as INode,
            });

            currentLine.positionCount = 0;
            currentLineStart = null;
        }

        private void PlaceProcessor(Selection s)
        {
            var gridPos = OnGrid(mInput.CursorPosition(), PlaceZ);
            switch (s)
            {
                case Selection.AddOne:
                    mGraph.CreateProcessorNode(processorPrefab, ProcessType.AddOne, gridPos);
                    return;
                case Selection.SubOne:
                    mGraph.CreateProcessorNode(processorPrefab, ProcessType.SubOne, gridPos);
                    return;
            }
            
            throw new Exception("unhandled processor type " + s);
        }

        private void Select(Selection s)
        {
            mInput.InputHandled();
            selection = s;
        }
    }
}