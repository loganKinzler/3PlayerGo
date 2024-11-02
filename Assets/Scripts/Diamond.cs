using System;
using Unity.Collections;
using UnityEngine;


public class Diamond : MonoBehaviour
{

    // DIAMOND VARS
    public int player = 0;
    public Texture player1SpriteSheet;
    public Texture player2SpriteSheet;
    public Texture player3SpriteSheet;
    public Texture nullSprite;

    private Vector3 diamondPosition = new Vector3(0,0,0);

    // MESH VARS    
    private Mesh mesh;
    private Vector3[] verts = new Vector3[4];
    private int[] tris = new int[6];
    private Vector2[] uvs = new Vector2[4];


    public void InstantiateDiamond() {
        SetPosition(0,0,0);
        PlaceDiamond(0);
        CreateDiamondMesh();
    }

    public void InstantiateDiamond(int x, int y, int z) {
        SetPosition(x, y, z);
        PlaceDiamond(0);
        CreateDiamondMesh();
    }

    public void InstantiateDiamond(Vector3 vect) {
        SetPosition((int) vect.x, (int) vect.y, (int) vect.z);
        PlaceDiamond(0);
        CreateDiamondMesh();
    }

    public Vector3 GetPosition() {return diamondPosition;}

    public void SetPosition(int x, int y, int z) {
        z %= 4;
        diamondPosition.Set(x, y, z);
        transform.position = GetEuclideanPosition(diamondPosition);
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * GetEuclideanRotation(diamondPosition));
    }

    public void SetPosition(Vector3 newPosition) {
        newPosition.z %= 4;
        diamondPosition.Set(newPosition.x, newPosition.y, newPosition.z);
        transform.position = GetEuclideanPosition(newPosition);
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * GetEuclideanRotation(newPosition));
    }

    public void PlaceDiamond(int newPlayer) {
        player = newPlayer%4;

        switch (player) {
            case 0:
                GetComponent<MeshRenderer>().material.mainTexture = nullSprite;
            break;

            case 1:
                GetComponent<MeshRenderer>().material.mainTexture = player1SpriteSheet;
                break;

            case 2:
                GetComponent<MeshRenderer>().material.mainTexture = player2SpriteSheet;
                break;

            case 3:
                GetComponent<MeshRenderer>().material.mainTexture = player3SpriteSheet;
                break;
        }
    }

    // MESH METHODS
    private void CreateDiamondMesh() {

        mesh = new Mesh();
        mesh.name = "Diamond Mesh";

        GenerateMeshData();
        mesh.vertices = verts;
        mesh.triangles = tris;
        
        SetUVs();
        mesh.uv = uvs;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateMeshData() {

        verts[0] = new Vector3(-0.5f, 0, 0);
        verts[1] = new Vector3(0, (float)Math.Sqrt(3) / 2.0f, 0);
        verts[2] = new Vector3(0.5f, 0, 0);
        verts[3] = new Vector3(0, -(float)Math.Sqrt(3) / 2.0f, 0);


        tris[0] = 0;
        tris[1] = 3;
        tris[2] = 2;

        tris[3] = 2;
        tris[4] = 1;
        tris[5] = 0;
    }

    private void SetUVs() {
        switch (diamondPosition.z) {
            case 0:
                uvs[0] = new Vector2(0.5f, 0.5f);
                uvs[1] = new Vector2(0.75f, 0.25f * (2 + (float)Math.Sqrt(3)));
                uvs[2] = new Vector2(1, 0.5f);
                uvs[3] = new Vector2(0.75f, 0.25f * (2 - (float)Math.Sqrt(3)));
            break;

            case 1:
                uvs[0] = new Vector2(0.5f, 0.5f);
                uvs[1] = new Vector2(0, 0.5f);
                uvs[2] = new Vector2(0.25f, 0.25f * (2 + (float)Math.Sqrt(3)));
                uvs[3] = new Vector2(0.75f, 0.25f * (2 + (float)Math.Sqrt(3)));
            break;

            case 2:
                uvs[0] = new Vector2(0.5f, 0.5f);
                uvs[1] = new Vector2(0.75f, 0.25f * (2 - (float)Math.Sqrt(3)));
                uvs[2] = new Vector2(0.25f, 0.25f * (2 - (float)Math.Sqrt(3)));
                uvs[3] = new Vector2(0, 0.5f);
            break;
        }
    }


    // GRID METHODS
    public float GetEuclideanRotation(Vector3 diamondVector) {
        return (float) Math.PI * 2f/3f * diamondVector.z;
    }

    public Vector2 GetEuclideanPosition(Vector3 diamondVector) {
        double X = 1.5f * diamondVector.x;// hexagonal space
        X += 0.5f * Math.Cos( GetEuclideanRotation(diamondVector) );
        X *= gameObject.transform.localScale.x;

        double Y = Math.Sqrt(3f) * (diamondVector.y + diamondVector.x * 0.5f);// hexagonal space
        Y += 0.5f * Math.Sin( GetEuclideanRotation(diamondVector) );
        Y *= gameObject.transform.localScale.x;

        return new Vector2((float) X, (float) Y);
    }
}
