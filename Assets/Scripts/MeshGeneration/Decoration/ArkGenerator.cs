using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkGenerator : MonoBehaviour
{
    public float scale;
    public float gapLength;
    public Material mat;

    private void Start()
    {
        GenerateArk();
    }

    private void GenerateArk()
    {
        Quaternion rot = this.transform.rotation;
        this.transform.rotation = Quaternion.identity;

        Vector3 tempSize;

        tempSize = scale * new Vector3(0.13f, 0.3f, 0.17f);
        CubeGenerator.SetupCubeMesh("baseLeft(Ark)", this.transform, this.transform.position + scale * new Vector3(-0.5f, 0.15f, 0.0f), tempSize, gapLength, mat);
        CubeGenerator.SetupCubeMesh("baseRight(Ark)", this.transform, this.transform.position + scale * new Vector3(0.5f, 0.15f, 0.0f), tempSize, gapLength, mat);
        tempSize = scale * new Vector3(0.09f, 0.8f, 0.12f);
        CubeGenerator.SetupCubeMesh("armLeft(Ark)", this.transform, this.transform.position + scale * new Vector3(-0.5f, 0.70f, 0.0f), tempSize, gapLength, mat);
        CubeGenerator.SetupCubeMesh("armRight(Ark)", this.transform, this.transform.position + scale * new Vector3(0.5f, 0.70f, 0.0f), tempSize, gapLength, mat);
        tempSize = scale * new Vector3(1.3f, 0.05f, 0.05f);
        CubeGenerator.SetupCubeMesh("firstRod(Ark)", this.transform, this.transform.position + scale * new Vector3(0.0f, 0.9f, 0.0f), tempSize, gapLength, mat);
        tempSize = scale * new Vector3(1.2f, 0.07f, 0.2f);
        CubeGenerator.SetupCubeMesh("MainRod(Ark)", this.transform, this.transform.position + scale * new Vector3(0.0f, 1.135f, 0.0f), tempSize, gapLength, mat);
        tempSize = scale * new Vector3(0.3f, 0.07f, 0.18f);
        Transform cubeTr = CubeGenerator.SetupCubeMesh("rightMainRod(Ark)", this.transform, this.transform.position + scale * new Vector3(0.7f, 1.207f, 0.0f), tempSize, gapLength, mat);
        cubeTr.Rotate(Vector3.back, -30.0f);
        tempSize = scale * new Vector3(0.3f, 0.07f, 0.18f);
        cubeTr = CubeGenerator.SetupCubeMesh("leftMainRod(Ark)", this.transform, this.transform.position + scale * new Vector3(-0.7f, 1.207f, 0.0f), tempSize, gapLength, mat);
        cubeTr.Rotate(Vector3.back, 30.0f);




        this.transform.rotation = rot;
    }
}
