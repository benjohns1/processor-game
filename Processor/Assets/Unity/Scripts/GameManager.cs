using System;
using System.Collections.Generic;
using SupplyChain;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Unity.Scripts
{
    internal enum Selection
    {
        Line,
        Plus
    }

    [RequireComponent(typeof(MonoGraph))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject plus;
        [SerializeField] private Camera cam;
        [SerializeField] private float gridSize = 0.5f;
        [SerializeField] private LineRenderer connectorPrefab;
        [SerializeField] private LineRenderer currentLine;
        [SerializeField] private Button buttonPlus;
        [SerializeField] private Button buttonLine;
        
        private GameObject currentLineStart;
        private Selection selection;
        private float posRound;
        private bool inputHandled = false;
        private EventSystem es;
        private float lineZ = 1;
        private float currentLineZ = -1;
        private float placeZ = 0;

        private Graph graph;
        private MonoGraph mGraph;

        private void Awake()
        {
            posRound = 1 / gridSize;
            buttonPlus.onClick.AddListener(delegate { Select(Selection.Plus); });
            buttonLine.onClick.AddListener(delegate { Select(Selection.Line); });
            es = EventSystem.current;

            mGraph = GetComponent<MonoGraph>();
            graph = mGraph.GetGraph();
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
            HandleInput();
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

            currentLine.SetPosition(1, OnGrid(cam.ScreenToWorldPoint(Input.mousePosition), currentLineZ));
        }

        private void HandleInput()
        {
            if (inputHandled)
            {
                inputHandled = false;
                return;
            }

            if (currentLine.positionCount > 0)
            {

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    StopDrawingLine();
                }
            }

            if (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                return;
            }

            if (es.IsPointerOverGameObject())
            {
                return;
            }

            switch (selection)
            {
                case Selection.Plus:
                    PlaceProcessor(selection);
                    break;
                case Selection.Line:
                    StartDrawingLine();
                    break;
                default:
                    break;
            }
        }

        private void StartDrawingLine()
        {
            var (hit, go, worldPoint) = ScreenTrace(Input.mousePosition);
            if (!hit)
            {
                return;
            }

            var pos = go.transform.position;

            currentLine.enabled = false;
            currentLine.positionCount = 2;
            currentLine.SetPosition(0, OnGrid(pos, currentLineZ));
            DrawCurrentLine();
            currentLineStart = go;
            currentLine.enabled = true;
        }

        private (bool Hit, GameObject Go, Vector2 WorldPoint) ScreenTrace(Vector3 pos)
        {
            var pt = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0));
            var wp = new Vector2(pt.x, pt.y);
            var hit = Physics2D.Raycast(pt, Vector2.zero);
            return hit.collider == null ? (false, null, wp) : (true, hit.collider.gameObject, wp);
        }

        private void StopDrawingLine()
        {
            if (currentLine.positionCount == 0)
            {
                return;
            }

            var (hit, go, worldPoint) = ScreenTrace(Input.mousePosition);
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

            MonoGraph.CreateConnector(mGraph, new MonoGraph.DrawConnector{
                Prefab = connectorPrefab,
                Start = OnGrid(currentLineStart.transform.position, lineZ),
                Upstream = currentLineStart.GetComponentInParent(typeof(IMonoNode)) as IMonoNode,
                End = OnGrid(go.transform.position, lineZ),
                Downstream = go.GetComponentInParent(typeof(IMonoNode)) as IMonoNode,
            });

            currentLine.positionCount = 0;
            currentLineStart = null;
        }

        private void PlaceProcessor(Selection s)
        {
            var pos = cam.ScreenToWorldPoint(Input.mousePosition);
            var gridPos = new Vector3(OnGrid(pos.x), OnGrid(pos.y), placeZ);
            switch (s)
            {
                case Selection.Plus:
                    MonoGraph.CreateNode(mGraph, plus, gridPos);
                    return;
            }
            
            throw new Exception("unhandled processor type " + s);
        }

        private void Select(Selection s)
        {
            inputHandled = true;
            selection = s;
        }
    }
}