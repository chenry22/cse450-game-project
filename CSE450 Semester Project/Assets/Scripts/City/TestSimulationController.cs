using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestSimulationController : MonoBehaviour
{
    public FileUploader fileUploader;
    public Transform[] spawnPoints;
    public float spawnVariability = 1f;
    
    // TODO: translate to WebGL functionality
    public void UploadAllFilesInDirectory() {
#if UNITY_EDITOR
        int populationLimit = 30;
        string directory = EditorUtility.OpenFolderPanel("Upload all files in folder", "", "");
        if (directory != "") {
            DirectoryInfo d = new DirectoryInfo(directory);
            int population = 0;
            population += UploadFilesFromDir(d, populationLimit);
            
            if (population >= populationLimit) { return; }
            foreach (DirectoryInfo sub in d.GetDirectories()) {
                population += UploadFilesFromDir(sub, population - populationLimit);
                if (population >= populationLimit) { return; }
            }
        }
#endif
    }

    public int UploadFilesFromDir(DirectoryInfo d, int populationLimit){
        int added = 0;
        foreach (FileInfo f in d.GetFiles()) {
            Debug.Log(f.FullName);
            var mon = fileUploader.HandleFileUpload(f.FullName);
            if (mon != null) { added++; }

            // attach behavior script
            mon.AddComponent<CityCreatureBehavior>();

            // place at random spawn point
            Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            mon.transform.position = spawnPoint + new Vector3(Random.Range(-spawnVariability, spawnVariability),
                                                              Random.Range(-spawnVariability, spawnVariability), 0);

            if (added >= populationLimit) {
                return added; 
            }
        }
        return added;
    }
}
