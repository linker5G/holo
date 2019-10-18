﻿using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleEditorLoader
{
    [MenuItem("Holo/Load all layers from an AssetBundle")]
    public static void LoadAssetBundle()
    {
        string bundlePath = EditorUtility.OpenFilePanel("Get The Bundle","","");
        string bundleName = Path.GetFileName(bundlePath);
        AssetBundleLoader assetBundleLoader = new AssetBundleLoader(bundleName);
        assetBundleLoader.LoadBundle(bundlePath);
        assetBundleLoader.InstantiateAllLayers();
    }

    [MenuItem("Holo/Unload All Asset Bundles (allows to test again by 'Load all layers from an AssetBundle')")]
    public static void UnloadAllAssetBundles()
    {
        AssetBundle.UnloadAllAssetBundles(true);
    }
}