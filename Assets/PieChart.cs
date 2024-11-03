using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PieChart : MonoBehaviour
{
    [SerializeField] int chartPoints;
    [SerializeField] float chartRadius;
    [SerializeField] float backgroundRadius;
    [SerializeField] float edgePadding = 0.5f;
    [SerializeField] float leftSideLimit = 4.25f;

    [SerializeField] MeshFilter backgroundMeshFilter;

    float scaledChartRadius, scaledBackgroundRadius;
    float oldScreenWidth = 0;

    void Update()
    {
        if (Screen.width != oldScreenWidth)
        {
            // Calculate position
            float chartPercent = chartRadius / backgroundRadius;
            float screenEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
            float chartRightX = screenEdge - edgePadding;
            float xPos = (chartRightX + leftSideLimit) / 2f;
            scaledBackgroundRadius = Mathf.Min((chartRightX - leftSideLimit) / 2f, chartRadius);
            scaledChartRadius = scaledBackgroundRadius * chartPercent;

            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

            GoGame go = FindObjectOfType<GoGame>();
            CreatePieChart(go.player1score, go.player2score, go.player3score);
            oldScreenWidth = Screen.width;
        }
    }

    public void CreatePieChart(int player1Score, int player2Score, int player3Score)
    {
        // Calculate player percentages
        int totalScore = player1Score + player2Score + player3Score;
        float p1Percent = player1Score / (float)totalScore;
        float p2Percent = player2Score / (float)totalScore;
        float p3Percent = player3Score / (float)totalScore;

        // Generate mesh
        List<Vector3> vertices = new List<Vector3>() { new Vector3(0, 0, 0) };
        List<Vector3> verticesBack = new List<Vector3>() { new Vector3(0, 0, 1) };
        List<int> indicesP1 = new List<int>();
        List<int> indicesP2 = new List<int>();
        List<int> indicesP3 = new List<int>();
        List<int> indicesBack = new List<int>();

        int vertCount = 1;
        for (int i = 0; i < chartPoints + 1; i++)
        {
            // Calculate point player
            float chartPercent = i / (float)chartPoints;
            int pointPlayer;
            if (chartPercent <= p1Percent)
                pointPlayer = 1;
            else if (chartPercent <= p2Percent + p1Percent)
                pointPlayer = 2;
            else
                pointPlayer = 3;

            // Calculate point position
            float angle = (2 * Mathf.PI * i) / chartPoints;
            float x = scaledChartRadius * Mathf.Cos(angle);
            float y = scaledChartRadius * Mathf.Sin(angle);

            // Add vertices and indices
            vertices.Add(new Vector3(x, y, 0));
            switch (pointPlayer)
            {
                case 1:
                    indicesP1.Add(0);
                    indicesP1.Add(vertCount);
                    indicesP1.Add(vertCount - 1);
                    break;
                case 2:
                    indicesP2.Add(0);
                    indicesP2.Add(vertCount);
                    indicesP2.Add(vertCount - 1);
                    break;
                case 3:
                    indicesP3.Add(0);
                    indicesP3.Add(vertCount);
                    indicesP3.Add(vertCount - 1);
                    break;
            }
            vertCount++;

            // Create background vertices and indices
            float bx = scaledBackgroundRadius * Mathf.Cos(angle);
            float by = scaledBackgroundRadius * Mathf.Sin(angle);
            verticesBack.Add(new Vector3(bx, by, 1));
            indicesBack.Add(0);
            indicesBack.Add(vertCount - 1);
            indicesBack.Add(vertCount - 2);
        }

        // Create main mesh
        if (player1Score > 0 || player2Score > 0 || player3Score > 0)
        {
            Mesh mesh = new Mesh();
            mesh.subMeshCount = 3;
            mesh.vertices = vertices.ToArray();
            mesh.SetTriangles(indicesP1.ToArray(), 0);
            mesh.SetTriangles(indicesP2.ToArray(), 1);
            mesh.SetTriangles(indicesP3.ToArray(), 2);
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Create background mesh
        Mesh backMesh = new Mesh();
        backMesh.vertices = verticesBack.ToArray();
        backMesh.triangles = indicesBack.ToArray();
        backMesh.RecalculateBounds();
        backgroundMeshFilter.mesh = backMesh;
    }
}
