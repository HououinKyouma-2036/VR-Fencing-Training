using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; // The AudioSource component
    public AudioClip[] audioClips; // Array of audio clips to play
    public RectTransform arrowImage; // The arrow image to show with audio clip 2
    public RectTransform leftFootE; // Left foot (E) to show with audio clip 3
    public RectTransform rightFootE; // Right foot (E) to show with audio clip 3
    public RectTransform leftFootA; // Left foot (A) to show with audio clip 4
    public RectTransform rightFootA; // Right foot (A) to show with audio clip 4
    public RectTransform leftFootL; // Left foot (L) to show with audio clip 5
    public RectTransform rightFootL; // Right foot (L) to show with audio clip 5

    public Transform dummy;

    public Transform target;

    public Vector3 dummyPosition1 = new Vector3(-10.6334105f,2.00732088f,2.13599992f);
    public Vector3 dummyPosition2 = new Vector3(-10.6334105f,2.00732088f,1.38100004f);
    public Vector3 dummyPosition3 = new Vector3(-10.6334105f,2.00732088f,0.866999984f);

    private GameObject player;
    private GameObject carp;
    private GameObject sword;
    private XRGrabInteractable grabInteractable;
    private float distance;
    private bool triggered;
    private bool waitingForSwordPickup = false;
    private bool readyForExtensionHit = false;
    private bool readyForAdvanceHit = false;
    private bool readyForLungeHit = false;
    private bool readyForFlickHit = false;
    public Button confusedButton; // Assign this in Unity Editor
    private DummyCollider hitDetection;



    private void Start()
    {
        confusedButton.gameObject.SetActive(false); // Hide the button initially
        confusedButton.onClick.AddListener(ButtonPressed);
        // Ensure the arrow image and foot UI elements are initially inactive
        if (arrowImage != null)
            arrowImage.gameObject.SetActive(false);

        if (leftFootE != null)
            leftFootE.gameObject.SetActive(false);

        if (rightFootE != null)
            rightFootE.gameObject.SetActive(false);

        if (leftFootA != null)
            leftFootA.gameObject.SetActive(false);

        if (rightFootA != null)
            rightFootA.gameObject.SetActive(false);

        if (leftFootL != null)
            leftFootL.gameObject.SetActive(false);

        if (rightFootL != null)
            rightFootL.gameObject.SetActive(false);

        player = GameObject.Find("Main Camera");
        carp = GameObject.Find("Carpet (3)");
        sword = GameObject.Find("Fencing Foil Sword");
        grabInteractable = sword.GetComponent<XRGrabInteractable>();
        triggered = false;
        if (sword != null)
        {
            hitDetection = sword.GetComponent<DummyCollider>();
        }
        StartCoroutine(startingRoutine());
    }



    private GameState currentState = GameState.Normal;
    private void Update() {
        sword = GameObject.Find("Fencing Foil Sword");
        grabInteractable = sword.GetComponent<XRGrabInteractable>();
        distance = (player.transform.position-carp.transform.position).sqrMagnitude;
        if (distance<3*3 && !triggered && grabInteractable.isSelected) {
            StartCoroutine(reachedStripRoutine());
            triggered = true;
        }
        if (waitingForSwordPickup && grabInteractable.isSelected)
        {
            StartCoroutine(swordPickedUpRoutine());
            waitingForSwordPickup = false;
        }
        if (readyForExtensionHit && hitDetection.CheckIfCorrectedHit() && hitDetection != null)
        {
            StartCoroutine(extensionAttackFinishedRoutine());
            readyForExtensionHit = false;
        }
        if (readyForAdvanceHit && CheckIfMovedToNewPosition(new Vector3(-10.6334105f, 2.00732088f, 1.38100004f)))
        {
            StartCoroutine(advanceAttackFinishedRoutine());
            readyForAdvanceHit = false;
        }
        if (readyForLungeHit && CheckIfMovedToNewPosition(new Vector3(-10.6334105f, 2.00732088f, 1.38100004f)))
        {
            StartCoroutine(lungeAttackFinishedRoutine());
            readyForLungeHit = false;
        }
        if (readyForFlickHit && hitDetection.CheckIfCorrectedHit() && hitDetection != null)
        {
            StartCoroutine(flickAttackFinishedRoutine());
        }
        {

        }
    }


    bool CheckIfMovedToNewPosition(Vector3 targetPosition, float threshold = 1.0f)
    {
        float distance = Vector3.Distance(player.transform.position, targetPosition);
        return distance <= threshold;
    }

    // Executes as soon as the game starts
    IEnumerator startingRoutine()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[0].length + 3f);

        // if after the length of the first audio clip + 3 seconds the user hasn't reached the fencing strip,
        // provide the dialogue to reach the fencing strip and draw the arrow
        distance = (player.transform.position-carp.transform.position).sqrMagnitude;
        if (distance >= 3*3) {

                Debug.Log("Showing arrow image");
                arrowImage.gameObject.SetActive(true);

                audioSource.clip = audioClips[1];
                audioSource.Play();

                yield return new WaitForSeconds(audioClips[1].length);

        }
        // if not grab sword, clip[3], else clip[4]

    }

    // Executes as soon as the user reaches the fencing strip
    IEnumerator reachedStripRoutine()
    {

        // Hide the arrow once the fencing strip has been found.
        if (arrowImage != null)
        {
            arrowImage.gameObject.SetActive(false);
        }

        Debug.Log("Showing extension UI elements");
        
        if (leftFootE != null)
            leftFootE.gameObject.SetActive(true);

        if (rightFootE != null)
            rightFootE.gameObject.SetActive(true);

        if (dummy != null)
            dummy.position = dummyPosition1;

        
        audioSource.clip = audioClips[2];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[2].length + 3f);

        Debug.Log("Hiding extension UI elements");
        if (leftFootE != null)
            leftFootE.gameObject.SetActive(false);

        if (rightFootE != null)
            rightFootE.gameObject.SetActive(false);

        audioSource.clip = audioClips[3];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[3].length);

        waitingForSwordPickup = true;

    }  

    // Executes when the user picks up the sword.
    IEnumerator swordPickedUpRoutine()
    {   

        audioSource.clip = audioClips[4];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[4].length + 3f);
        target.localScale = new Vector3(0.0500000007f,0.0500000007f,0.0500000007f);
        readyForExtensionHit = true;

    }  


    // Executes when the user finishes the extension attack.
    IEnumerator extensionAttackFinishedRoutine()
    {

        audioSource.clip = audioClips[5];
        audioSource.Play();

        Debug.Log("Showing advance UI elements");
        
        if (leftFootA != null)
            leftFootA.gameObject.SetActive(true);

        if (rightFootA != null)
                rightFootA.gameObject.SetActive(true);

        if (dummy != null) {
                dummy.position = dummyPosition2;
        }

        yield return new WaitForSeconds(audioClips[5].length + 5f);

        // if the user didn't move forward yet, share the gulf
        if (CheckIfMovedToNewPosition(new Vector3(-10.6334105f, 2.00732088f, 1.38100004f))) {
            audioSource.clip = audioClips[6];
            audioSource.Play();

            yield return new WaitForSeconds(audioClips[6].length + 3f);

            Debug.Log("Hiding advance UI elements");
            if (leftFootA != null)
                leftFootA.gameObject.SetActive(false);

            if (rightFootA != null)
                rightFootA.gameObject.SetActive(false);

        } else {
            Debug.Log("Hiding advance UI elements");
            if (leftFootA != null)
                leftFootA.gameObject.SetActive(false);

            if (rightFootA != null)
                rightFootA.gameObject.SetActive(false);
        }

        readyForAdvanceHit = true;

    }  

    // Executes when the user finishes the advance attack.
    IEnumerator advanceAttackFinishedRoutine()
    {

        audioSource.clip = audioClips[7];
        audioSource.Play();

        if (dummy != null) {
            dummy.position = dummyPosition3;
        }

        yield return new WaitForSeconds(audioClips[7].length + 2f);

        // SPAWN IN THE BUTTON FOR THE USER TO SAY THEY ARE CONFUSED


    }  

    // Executes when the user notifies the AI that no instructions were provided for lunge attack.
    IEnumerator confusedOnLungeAttackRoutine()
    {

        // HIDE THE BUTTON FOR THE USER TO SAY THEY ARE CONFUSED

        audioSource.clip = audioClips[8];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[8].length + 2f);

        // SPAWN IN THE BUTTON FOR THE USER TO SAY THEY ARE CONFUSED

    }  

    // Executes when the user notifies the AI that no footprints were provided for lunge attack.
    IEnumerator confusedOnLungeAttackFootprintsRoutine()
    {

        // HIDE IN THE BUTTON FOR THE USER TO SAY THEY ARE CONFUSED


        Debug.Log("Showing lunge UI elements");

        if (leftFootL != null)
            leftFootL.gameObject.SetActive(true);

        if (rightFootL != null)
            rightFootL.gameObject.SetActive(true);

        readyForLungeHit = true;

        yield return new WaitForSeconds(5f);

        Debug.Log("Hiding lunge UI elements");
        if (leftFootL != null)
            leftFootL.gameObject.SetActive(false);

        if (rightFootL != null)
            rightFootL.gameObject.SetActive(false);

    }

    private void ButtonPressed()
    {
        if (currentState == GameState.AdvanceAttackFinished)
        {
            StartCoroutine(confusedOnLungeAttackRoutine());
        }
        else if (currentState == GameState.LungeAttackFinished)
        {
            StartCoroutine(confusedOnLungeAttackFootprintsRoutine());
        }
    }

    public enum GameState
    {
        Normal,
        AdvanceAttackFinished,
        LungeAttackFinished
    }

    // Executes when the user finishes the lunge attack
    IEnumerator lungeAttackFinishedRoutine()
    {
        // hide in case they finish the lunge before 5 seconds
        Debug.Log("Hiding lunge UI elements");
        if (leftFootL != null)
            leftFootL.gameObject.SetActive(false);

        if (rightFootL != null)
            rightFootL.gameObject.SetActive(false);


        Debug.Log("Showing flick UI elements");
        if (leftFootE != null)
            leftFootE.gameObject.SetActive(true);

        if (rightFootE != null)
            rightFootE.gameObject.SetActive(true);

        if (dummy != null) {
            dummy.position = dummyPosition1;
        }

        audioSource.clip = audioClips[9];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[9].length + 5f);

        Debug.Log("Hiding flick UI elements");
        if (leftFootE != null)
                leftFootE.gameObject.SetActive(false);

        if (rightFootE != null)
            rightFootE.gameObject.SetActive(false);

        audioSource.clip = audioClips[10];
        audioSource.Play();

        readyForFlickHit = true;
    }  

    // Executes when the flick attack is done.
    IEnumerator flickAttackFinishedRoutine()
    {
        audioSource.clip = audioClips[11];
        audioSource.Play();
        yield return null;
    }  


}
