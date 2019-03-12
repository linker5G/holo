﻿using Unity;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using System.IO;


public class STLSeriesConverter
{
    GameObject series_GameObject;
    
    [MenuItem("Holo/Convert STL series to a .prefab")]
    public static void ConvertSTL()
    {
        STLSeriesConverter stlSeriesConverter = new STLSeriesConverter();
        STLSeriesImporter stlImporter = new STLSeriesImporter();

        stlSeriesConverter.series_GameObject = stlImporter.Get_GameObject();
        stlSeriesConverter.series_GameObject.AddComponent<BlendShapeAnimation>();

        stlSeriesConverter.ExportToPrefab();
    }

    // Exports finished GameObject to a .prefab
    private void ExportToPrefab()
    {
        string save_path = EditorUtility.SaveFilePanel("Choose a folder where for a .prefab file", "", "", ".prefab");
        PrefabUtility.SaveAsPrefabAsset(series_GameObject, save_path);
    }

}

public class STLSeriesImporter
{
    GameObject imported_STLs = new GameObject();
    
    List<string> file_paths = new List<string>();

    public GameObject Get_GameObject()
    {
        return this.imported_STLs;
    }

    // Object constructor, initiates STL series import.
    public STLSeriesImporter()
    {
        this.Get_file_paths();
        this.Load_files();
    }


    // gets path to the subsequent STL meshes stored in a root folder.
    private void Get_file_paths()
    {
        string root_folder = EditorUtility.OpenFolderPanel("Select STL series root folder", "", "");
        string[] filenames = Directory.GetFiles(root_folder);
    }


    //loads meshes from separate files into one GameObject
    private void Load_files()
    {
        SkinnedMeshRenderer skinnedMesh = imported_STLs.AddComponent<SkinnedMeshRenderer>();
        Mesh mesh = new Mesh();

        STLFileImporter stlFileImporter = new STLFileImporter();

        bool first_mesh = true;
        foreach (string filepath in this.file_paths)
        {
            stlFileImporter.Load_STL_file(filepath, first_mesh);
            mesh.AddBlendShapeFrame(Path.GetFileName(filepath), 100f, stlFileImporter.AllVertices, null, null);
            first_mesh = false;
        }
        mesh.vertices = stlFileImporter.BaseVertices;
        mesh.triangles = stlFileImporter.Indexes;
        skinnedMesh.sharedMesh = mesh;
    }
}


//Loads a single STL file and turns it into a list of vertices (x,y,z) and if first_mesh: a list of indexes
public class STLFileImporter
{
    public Vector3[] AllVertices { get; } = new Vector3[] { };
    public Vector3[] BaseVertices { get; } = new Vector3[] { };
    public int[] Indexes { get; } = new int[] { };
    // TODO: Change arrays into list but getting them gives you an array

    public void Load_STL_file(string file_path, bool first_mesh)
    {
        using (FileStream filestream = new FileStream(file_path, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader binary_reader = new BinaryReader(filestream, new ASCIIEncoding()))
            {
                // read header
                byte[] header = binary_reader.ReadBytes(80);
                uint facetCount = binary_reader.ReadUInt32();


                for (uint i = 0; i < facetCount; i++)
                {
                    Adapt_Facet(binary_reader, first_mesh);
                }
            }
        }
    }

    private void Adapt_Facet(BinaryReader binary_reader, bool first_mesh)
    {
        binary_reader.GetVector3(); // A normal we don't use

        for (int i = 0; i < 3; i++)
        {
            Vector3 vertix = binary_reader.GetVector3();
            if (first_mesh == true)
            {
                //TODO: Map vertices to indices
            }

        }
        binary_reader.ReadUInt16(); // non-sense attribute byte
    }

    private void SetIndexForVertix(Vector3 vertix)
    {
        // TODO: Set an index for a vertix, checking if it isn't already in the allVertices list, if so, give the used index
    }
}


public static class STLImportUtils
{
    public static Vector3[] Get_vertices(this BinaryReader binary_reader)
    {

        Facet facet = new Facet();
        facet.normal = binary_reader.GetVector3();

        // maintain counter-clockwise orientation of vertices:
        facet.a = binary_reader.GetVector3();
        facet.c = binary_reader.GetVector3();
        facet.b = binary_reader.GetVector3();
        binary_reader.ReadUInt16(); // padding

        return facet;
    }
    public static Vector3 GetVector3(this BinaryReader binaryReader)
    {
        Vector3 vector3 = new Vector3();
        for (int i = 0; i < 3; i++)
            vector3[i] = binaryReader.ReadSingle();
        return vector3.UnityCoordTrafo();
    }

    private static Vector3 UnityCoordTrafo(this Vector3 vector3)
    {
        return new Vector3(-vector3.y, vector3.z, vector3.x);
    }
}