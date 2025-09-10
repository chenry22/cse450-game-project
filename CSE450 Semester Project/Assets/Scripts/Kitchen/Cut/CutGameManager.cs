using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// this class manages the UI overlay and the actual game mechanics of the cut minigame
// TODO: currently it uses a temp player movement script, to be replaced with a final implementation

// TODO: maybe add functionality for different number of cuts...
// this would mean adding the guide markers programmatically

// TODO: it would also be cool to transfer the cuts made onto the actual pizza object
// and this wouldn't be too crazy since we're storing them as GameObjects already

public class CutGameManager : MonoBehaviour
{
    private const int numCuts = 4;
    private const int baseRotationSpeed = 110; // in degrees per second
    private const int rotationSpeedIncrease = 38; // how much to speed up after each cut
    private const float qualityLenience = 5f;
    // used to calculate penalty for cut error
    // formula is [ maxQuality - (degreesOff / qualityLenience) ]
    // basically # of degrees a cut has to be off to subtract 1 point from quality score
    private const string mainHelpText = "[Space] to cut\n[Q] to cancel";
    private const string completionHelpTxt = "[E] to continue";

    [Header("Game")]
    public Rigidbody2D pie;
    public GameObject cutIndicator;
    public GameObject cutPrefab;
    public GameObject actualGame;

    [Header("UI")]
    public TMP_Text countdown;
    public TMP_Text progressTxt;
    public TMP_Text helpTxt;

    private bool gameActive = false;
    private List<GameObject> cuts = new List<GameObject>(); // keep track of rotation amount of cuts
                                                            // for a perfect cut, we sohuld have one at 0/180, 45/225, 90/270, 135/315

    private int rotationSpeed = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // end game without saving progress
            // basically kill this UI
            gameActive = false;
            this.gameObject.SetActive(false);
            ResetCutGame();
            // and enable user movement again
            // TODO: This is currently interfacing with the placeholder movement script (should be changed)
            GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();
        }
        else if (!gameActive && cuts.Count == numCuts && Input.GetKeyDown(KeyCode.E))
        {
            gameObject.SetActive(false); // basically just kill UI
            GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();
            Debug.Log("GAME END TRIGGERED");

            // set cut of current pizza (find from parent)
            // then claim back to user
        }
        else if (gameActive && cuts.Count != numCuts)
        {
            pie.angularVelocity = rotationSpeed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CutPie();
            }
        }
    }

    // compares cut angles to target cut lines and computes score based on cumulative difference/error
    private int CalculateCutScore()
    {
        int quality = 0;
        var maxPerCut = 25;
        // 180 is basically 0 for our purposes... probably a better way to do this somewhere
        var targets = new List<float> { 0, 45, 90, 135, 180 };
        cuts.ForEach((cut) =>
        {
            var rot = cut.transform.localEulerAngles.z;
            if (rot < 0) { rot += 360f; }
            var comp = (rot + 180) % 360; // check wrap around
            rot = Mathf.Min(comp, rot);

            var bestDiff = 360f;
            var bestTarget = 0f;
            targets.ForEach((target) =>
            {
                var diff = Mathf.Abs(Mathf.DeltaAngle(target, rot));
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    bestTarget = target;
                }
            });
            Debug.Log("Rot: " + rot + ", Closest to: " + bestTarget + ", Diff: " + bestDiff);
            targets.Remove(bestTarget);
            if (bestTarget == 0)
            {
                targets.Remove(180);
            }
            else if (bestTarget == 180)
            {
                targets.Remove(0);
            }
            quality += (int)Mathf.Max(maxPerCut - (bestDiff / qualityLenience), 0);
        });
        return quality;
    }


    // Main game managers
    public void BeginCutGame()
    {
        // TODO: replace this with final script
        GameObject.FindWithTag("Player").GetComponent<TempPlayerMove>().ToggleMovement();
        ResetCutGame();
        countdown.gameObject.SetActive(true);
        StartCoroutine(DoCountdown());
    }
    public void ResetCutGame()
    {
        cuts.ForEach((obj) => Destroy(obj));
        cuts.Clear();
        cutIndicator.SetActive(true);
        helpTxt.text = mainHelpText;
        progressTxt.text = cuts.Count + " / " + numCuts;
        gameActive = false;
        actualGame.SetActive(false);
    }
    private void EndCutGame()
    {
        cutIndicator.SetActive(false);
        gameActive = false;
        pie.angularVelocity = 0;
        helpTxt.text = completionHelpTxt;

        int quality = CalculateCutScore();
        // TODO: either transfer ownership to this station
        //   or just update the player pizza
        GameObject.FindWithTag("Player").GetComponentInChildren<PizzaObject>().CutPizza(quality);
        progressTxt.text = "Quality: " + quality + " / 100";
    }


    // async manager functions
    private IEnumerator DoCountdown()
    {
        var count = 3;
        while (count > 0)
        {
            countdown.text = "" + count;
            yield return new WaitForSeconds(1);
            count--;
        }
        countdown.gameObject.SetActive(false);
        actualGame.SetActive(true);
        yield return BeginSpinning(0.3f);
    }
    private IEnumerator BeginSpinning(float delay)
    {
        yield return new WaitForSeconds(delay); // small delay to allow for some reaction time
        rotationSpeed = baseRotationSpeed;
        gameActive = true;
    }


    // actual game interaction
    // single cut
    public void CutPie()
    {
        gameActive = false;
        pie.angularVelocity = 0; // stop movement temporarily to do cut

        // create new cut and attach to parent
        var newCut = Instantiate(cutPrefab);
        newCut.transform.parent = pie.gameObject.transform;
        newCut.transform.localPosition = newCut.transform.position;
        cuts.Add(newCut);

        progressTxt.text = cuts.Count + " / " + numCuts;
        if (cuts.Count == numCuts)
        {
            EndCutGame();
        }
        else
        {
            gameActive = true;
            rotationSpeed += rotationSpeedIncrease;
        }
    }
}
