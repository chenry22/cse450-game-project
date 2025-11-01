using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CitySceneLoader : MonoBehaviour
{
    public void LoadKitchenScene()
    {
        SceneManager.LoadScene("MainKitchenScene");
    }

    public void LoadBasketballScene()
    {
        SceneManager.LoadScene("Basketball");
    }

    public void LoadBandRhythmScene()
    {
        SceneManager.LoadScene("BandRhythmGame");
    }
}
