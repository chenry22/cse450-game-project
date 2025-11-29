using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CityController : MonoBehaviour {
    public FileUploader fileUploader;
    public GameObject map;
    public GameObject cityUploadButton;
    public TMP_Text cityName;
    public Transform[] spawnPoints;

    public GameObject testCity1;
    public GameObject testCity2;

    public float spawnVariability = 1f;
    public int populationLimit = 25;  

    void Start() {
        // map.SetActive(false);
        if (GameManager.instance?.lastCity != null) {
            CreateCityFromDirectory(GameManager.instance.lastCity);
        }
    }
    
    public void LoadTestCity1() {
        cityName.text = "Test City 1 | Population: 8";
        // transfer player to current scene (if possible)
        if (GameManager.instance?.playerCreature != null) {
            AddPlayerAndBuddy();
        } else {
            Debug.LogWarning("No player creature found for city");
        }

        testCity1.SetActive(true);
        cityUploadButton.SetActive(false);
        map.SetActive(true);
        foreach(CreatureStats c in testCity1.transform.GetComponentsInChildren<CreatureStats>()) {
            c.gameObject.AddComponent<CityCreatureBehavior>();
        }
    }
    public void LoadTestCity2() {
        cityName.text = "Test City 2 | Population: 16";
        // transfer player to current scene (if possible)
        if (GameManager.instance?.playerCreature != null) {
            AddPlayerAndBuddy();
        } else {
            Debug.LogWarning("No player creature found for city");
        }
        
        testCity2.SetActive(true);
        cityUploadButton.SetActive(false);
        map.SetActive(true);
        foreach(CreatureStats c in testCity2.transform.GetComponentsInChildren<CreatureStats>()) {
            c.gameObject.AddComponent<CityCreatureBehavior>();
        }
    }

    // TODO: translate to WebGL functionality
    public void UploadAllFilesInDirectory() {
#if UNITY_EDITOR
        string directory = EditorUtility.OpenFolderPanel("Upload all files in folder", "", "");
        if (directory != "") {
            DirectoryInfo d = new DirectoryInfo(directory);
            if (GameManager.instance != null) {
                GameManager.instance.lastCity = d;
            }
            CreateCityFromDirectory(d);
        }
#endif
    }

    public void AddPlayerAndBuddy(){
        var player = Instantiate(GameManager.instance.playerCreature);
        player.GetComponent<CreatureSelect>().SetTextSelected();
        player.tag = "Player";
        GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().selectedCreature = player;
        player.SetActive(true);
        GameObject.FindWithTag("MainCamera").transform.parent = player.transform;

        if (GameManager.instance.buddyCreature != null) {
            var buddy = Instantiate(GameManager.instance.buddyCreature);
            buddy.GetComponent<CreatureSelect>().SetTextBuddy();
            buddy.tag = "Partner";
            GameObject.Find("CreatureHandler").GetComponent<CreatureMove>().buddyCreature = buddy;
            buddy.SetActive(true);
        }
    }
    
    public void CreateCityFromDirectory(DirectoryInfo d) {
        map.SetActive(true);  
        cityUploadButton.SetActive(false);

        // transfer player to current scene (if possible)
        if (GameManager.instance?.playerCreature != null) {
            AddPlayerAndBuddy();
        } else {
            Debug.LogWarning("No player creature found for city");
        }
               
        // load actual city
        int population = 0;
        population += UploadFilesFromDir(d, populationLimit);
        if (population >= populationLimit) {
            cityName.text = d.Name + " City | Population: " + population;
            return; 
        }

        foreach (DirectoryInfo sub in d.GetDirectories()) {
            population += UploadFilesFromDir(sub, population - populationLimit);
            if (population >= populationLimit) {
                cityName.text = d.Name + " City | Population: " + population;
                return; 
            }
        }
        
        cityName.text = d.Name + " City | Population: " + population;
    }

    public int UploadFilesFromDir(DirectoryInfo d, int populationLim) {
        int added = 0;
        foreach (FileInfo f in d.GetFiles()) {
            // Debug.Log(f.FullName);
            var mon = fileUploader.HandleFileUpload(f.FullName);
            if (mon != null) { added++; }

            // attach behavior script
            mon.AddComponent<CityCreatureBehavior>();

            // place at random spawnpoint
            Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position +
                new Vector3(Random.Range(-spawnVariability, spawnVariability),
                            Random.Range(-spawnVariability, spawnVariability), 0);
            // max 10 retries...
            for (int i = 0; i < 10; i++) {
                if (!SpawnOverlapping(spawnPoint)) { break; }
                Debug.Log("Spawn overlap!!!!");
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].position +
                    new Vector3(Random.Range(-spawnVariability, spawnVariability),
                                Random.Range(-spawnVariability, spawnVariability), 0);
            }

            mon.transform.position = spawnPoint;

            if (added >= populationLim) {
                return added;
            }
        }
        return added;
    }
    
    private bool SpawnOverlapping(Vector3 pos) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 4f);
        for(int i = 0; i < colliders.Length; i++) {
            Vector3 center = colliders[i].bounds.center;
            float w = colliders[i].bounds.extents.x;
            float h = colliders[i].bounds.extents.y;
            float left = center.x - w;
            float right = center.x + w;
            float top = center.y + h;
            float bot = center.y - h;

            if (pos.x >= left && pos.x <= right && pos.y <= top && pos.y >= bot) {
                return true;
            }
        }
        Debug.Log("No spawn overlap :)");
        return false;
    }
}
