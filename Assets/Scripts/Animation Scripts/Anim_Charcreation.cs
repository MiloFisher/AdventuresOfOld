using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Anim_Charcreation : MonoBehaviour
{
    //==============================================
    // Custom Anim Struct
    //==============================================
    private List<Anim> AnimList = new List<Anim>();

    private struct Anim {
        private Vector3 start;
        private Vector3 end;
        private float duration;
        private Transform subject;
        private float startTime;

        public Anim(Vector3 setStart, Vector3 setEnd, float setDuration, Transform setSubject) {
            start = setStart;
            end = setEnd;
            duration = setDuration;
            subject = setSubject;
            startTime = Time.time;
        }

        // Returns true if completed
        public bool update() {
            float timeElapsed = Time.time - startTime;
            float ratio = timeElapsed/duration;
            subject.localPosition = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, ratio));

            return timeElapsed >= duration;
        }
    }

    //==============================================
    // Fields
    //==============================================
    // Case Dictionary
    private Dictionary<string, int> ANIM_CASES = new Dictionary<string, int> {
        ["setRace"] = 0,
        ["confirmRace"] = 1,
        ["confirmRaceBack"] = 2,
        ["setClass"] = 3,
        ["setClassBack"] = 4,
        ["confirmClass"] = 5,
        ["confirmClassBack"] = 6,
        ["setTrait"] = 7,
        ["setTraitBack"] = 8,
        ["finished"] = 9
    };

    // UI Starting Positions + vertical offset
    private Dictionary<string, Vector3> UI_START_POS = new Dictionary<string, Vector3> {
        ["rightTall"] = new Vector3(540, -44, 0),
        ["rightShort"] = new Vector3 (540, -112, 0),
        ["rightMed"] = new Vector3 (540, -50, 0),
        ["leftSquare"] = new Vector3 (-540, -12, 0),
        ["leftSkill"] = new Vector3 (-540, -12, 0),
        ["leftTrait"] = new Vector3 (-540, 26, 0),
        ["forward"] = new Vector3 (880, -418, 0),
        ["back"] = new Vector3 (186, -418, 0),
        ["title"] = new Vector3 (0, 600, 1),
        ["stats"] = new Vector3 (0, 600, 0),
    };
    private Dictionary<string, Vector3> UI_OFFSET = new Dictionary<string, Vector3> {
        ["rightTall"] = new Vector3(0, -810, 0),
        ["rightShort"] = new Vector3(0, -678, 0),
        ["rightMed"] = new Vector3(0, -800, 0),
        ["leftSquare"] = new Vector3(0, -550, 0),
        ["leftSkill"] = new Vector3(0, -550, 0),
        ["leftTrait"] = new Vector3(0, -630, 0),
        ["forward"] = new Vector3(0, -121, 0),
        ["back"] = new Vector3(0, -121, 0),
        ["title"] = new Vector3 (0, -105, 0),
        ["stats"] = new Vector3 (0, -178, 0),
    };

    // Total Playback time for animations (seconds)
    private Dictionary<string, float> PLAYBACK_TIME = new Dictionary<string, float> {
        ["setRace_Up"] = RAISE_TIME,
        ["setRace_Down"] = LOWER_TIME,
        ["confirmRace_Up"] = RAISE_TIME + 0.1f + 0.1f,
        ["confirmRace_Down"] = LOWER_TIME + 0.1f + 0.1f,
        ["setClass_Up"] = RAISE_TIME + 0.1f + 0.1f,
        ["setClass_Down"] = LOWER_TIME + 0.1f + 0.1f,
        ["confirmClass_Up"] = RAISE_TIME + 0.1f + 0.1f,
        ["confirmClass_Down"] = LOWER_TIME + 0.1f + 0.1f,
        ["setTrait_Up"] = RAISE_TIME + 0.1f + 0.1f,
        ["setTrait_Down"] = LOWER_TIME + 0.1f + 0.1f,
        ["finalConfirm_Up"] = RAISE_TIME + 0.1f + 0.1f,
        ["finalConfirm_Down"] = LOWER_TIME + 0.1f + 0.1f,
    };
    // Gathering Scenes
    public GameObject scene_SetRace;
    public GameObject scene_ConfirmRace;
    public GameObject scene_SetClass;
    public GameObject scene_ConfirmClass;
    public GameObject scene_setTrait;
    public GameObject scene_FinalConfirm;
    public GameObject mouseLock;
    public GameObject scene_Loading;

    // Reference fields
    public GameObject container_Title;
    public GameObject container_Stats;
    public TMP_InputField inputName;

    // State Management
    private bool clickedClass = false;
    private bool clickedTrait = false;

    //==============================================
    // Privateer Movement Functions
    //==============================================
    // Time it takes for anims to play (seconds)
    private const float RAISE_TIME = 1.2f;
    private const float LOWER_TIME = 0.7f;
    private const float TRANSITION_DELAY = 0.2f;
    
    // Moves the title element
    private IEnumerator moveTitle(bool enable) {
        //container_Title
        // Enact Animation ====================================
        AnimList.Add(new Anim ( // Title
            (enable) ? UI_START_POS["title"] : UI_START_POS["title"]+UI_OFFSET["title"],
            (enable) ? UI_START_POS["title"]+UI_OFFSET["title"] : UI_START_POS["title"],
            (enable) ? RAISE_TIME * 0.5f : LOWER_TIME * 0.5f, container_Title.transform
        ));
        yield return new WaitForSeconds(0f);
    }

    // Moves the stats element
    private IEnumerator moveStats(bool enable) {
        //container_Stats
        // Enact Animation ====================================
        AnimList.Add(new Anim ( // Title
            (enable) ? UI_START_POS["stats"] : UI_START_POS["stats"]+UI_OFFSET["stats"],
            (enable) ? UI_START_POS["stats"]+UI_OFFSET["stats"] : UI_START_POS["stats"],
            (enable) ? RAISE_TIME * 0.5f: LOWER_TIME * 0.5f, container_Stats.transform
        ));
        yield return new WaitForSeconds(0f);
    }

    private IEnumerator moveSetRace(bool raise) { // Enacts setRace Screen Behavior
        // Referencing Subjects ====================================
        Transform rightTall = scene_SetRace.transform.Find("Race_SCROLL");

        // Enact Animations ====================================
        AnimList.Add(new Anim ( // Right Tall
            (raise) ? UI_START_POS["rightTall"]+UI_OFFSET["rightTall"] : UI_START_POS["rightTall"], 
            (raise) ? UI_START_POS["rightTall"] : UI_START_POS["rightTall"]+UI_OFFSET["rightTall"], 
            (raise) ? RAISE_TIME : LOWER_TIME, rightTall
        ));
        // Final Delay
        yield return new WaitForSeconds((raise) ? RAISE_TIME : LOWER_TIME);
    }

    private IEnumerator moveConfirmRace(bool raise) { // Enacts confirmRace Screen Behavior
        // Referencing Subjects ====================================
        Transform rightTall = scene_ConfirmRace.transform.Find("RaceDetails_Container");
        Transform leftSlab = scene_ConfirmRace.transform.Find("Race_Slate_IMAGE");
        Transform leftImg = scene_ConfirmRace.transform.Find("Race_IMAGE");
        Transform confirm = scene_ConfirmRace.transform.Find("Confirm_BUTTON");
        Transform back = scene_ConfirmRace.transform.Find("Back_BUTTON");

        // Enact Animation ====================================
        AnimList.Add(new Anim ( // Right Tall
            (raise) ? UI_START_POS["rightTall"]+UI_OFFSET["rightTall"] : UI_START_POS["rightTall"],
            (raise) ? UI_START_POS["rightTall"] : UI_START_POS["rightTall"]+UI_OFFSET["rightTall"],
            (raise) ? RAISE_TIME : LOWER_TIME, rightTall
        ));
        yield return new WaitForSeconds(0.1f); //Fixed Delay
        AnimList.Add(new Anim ( // Left Slab
            (raise) ? UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"] : UI_START_POS["leftSquare"],
            (raise) ? UI_START_POS["leftSquare"] : UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"],
            (raise) ? RAISE_TIME : LOWER_TIME, leftSlab
        ));
        AnimList.Add(new Anim ( // Left Image
            (raise) ? UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"] : UI_START_POS["leftSquare"],
            (raise) ? UI_START_POS["leftSquare"] : UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"],
            (raise) ? RAISE_TIME : LOWER_TIME, leftImg
        ));
        yield return new WaitForSeconds(0.1f); //Fixed Delay
        AnimList.Add(new Anim ( // Confirm
            (raise) ? UI_START_POS["forward"]+UI_OFFSET["forward"] : UI_START_POS["forward"],
            (raise) ? UI_START_POS["forward"] : UI_START_POS["forward"]+UI_OFFSET["forward"],
            (raise) ? RAISE_TIME : LOWER_TIME, confirm
        ));
        AnimList.Add(new Anim ( // Back
            (raise) ? UI_START_POS["back"]+UI_OFFSET["back"] : UI_START_POS["back"],
            (raise) ? UI_START_POS["back"] : UI_START_POS["back"]+UI_OFFSET["back"],
            (raise) ? RAISE_TIME : LOWER_TIME, back
        ));
        // Final Delay
        yield return new WaitForSeconds((raise) ? RAISE_TIME : LOWER_TIME);
    }

    private IEnumerator moveSetClass(bool raise) { // Enacts setClass Screen Behavior
        // Referencing Subjects ====================================
        Transform rightTall = scene_SetClass.transform.Find("Class_SCROLL");
        Transform leftSlab = scene_SetClass.transform.Find("Class_Slate_IMAGE");
        Transform leftImg = scene_SetClass.transform.Find("Class_IMAGE");
        Transform confirm = scene_SetClass.transform.Find("Confirm_BUTTON");
        Transform back = scene_SetClass.transform.Find("Back_BUTTON");

        // Enact Animations ====================================
        AnimList.Add(new Anim ( // Right Tall
            (raise) ? UI_START_POS["rightTall"]+UI_OFFSET["rightTall"] : UI_START_POS["rightTall"],
            (raise) ? UI_START_POS["rightTall"] : UI_START_POS["rightTall"]+UI_OFFSET["rightTall"],
            (raise) ? RAISE_TIME : LOWER_TIME, rightTall
        ));
        yield return new WaitForSeconds(0.1f); //Fixed Delay
        AnimList.Add(new Anim ( // Left Slab
            (raise) ? UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"] : UI_START_POS["leftSquare"],
            (raise) ? UI_START_POS["leftSquare"] : UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"],
            (raise) ? RAISE_TIME : LOWER_TIME, leftSlab
        ));
        AnimList.Add(new Anim ( // Left Image
            (raise) ? UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"] : UI_START_POS["leftSquare"],
            (raise) ? UI_START_POS["leftSquare"] : UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"],
            (raise) ? RAISE_TIME : LOWER_TIME, leftImg
        ));
        yield return new WaitForSeconds(0.1f); //Fixed Delay
        AnimList.Add(new Anim ( // Back
            (raise) ? UI_START_POS["back"]+UI_OFFSET["back"] : UI_START_POS["back"],
            (raise) ? UI_START_POS["back"] : UI_START_POS["back"]+UI_OFFSET["back"],
            (raise) ? RAISE_TIME : LOWER_TIME, back
        ));

        // Following behavior only enacts on lowering, as raising has seperate behavior
        if(!raise && clickedClass) {
            AnimList.Add(new Anim ( // Confirm
                UI_START_POS["forward"],
                UI_START_POS["forward"]+UI_OFFSET["forward"],
                LOWER_TIME, confirm
            ));
        }

        // Final Delay
        yield return new WaitForSeconds((raise) ? RAISE_TIME : LOWER_TIME);
    }

    // Used to raise the confirm button of the setClass scene
    private IEnumerator moveSetClassConfirm() {
        if(!clickedClass) {
            clickedClass = true;
            // Referencing Subjects ====================================
            Transform confirm = scene_SetClass.transform.Find("Confirm_BUTTON");

            // Enact Animations ====================================
            AnimList.Add(new Anim ( // Confirm
                UI_START_POS["forward"]+UI_OFFSET["forward"],
                UI_START_POS["forward"],
                RAISE_TIME/4, confirm
            ));
            yield return new WaitForSeconds(0f);
        }
    }

    private IEnumerator moveConfirmClass(bool raise) { // Enacts confirmClass Screen Behavior
        // Referencing Subjects ====================================
        Transform rightShort = scene_ConfirmClass.transform.Find("ClassAbilityContainer");
        Transform rightImg = scene_ConfirmClass.transform.Find("Class_Icon");
        Transform rightSlab = scene_ConfirmClass.transform.Find("RightSlab");
        Transform leftSkill = scene_ConfirmClass.transform.Find("AbilityDetailsContainer");
        Transform confirm = scene_ConfirmClass.transform.Find("Confirm_BUTTON");
        Transform back = scene_ConfirmClass.transform.Find("Back_BUTTON");

        // Enact Animations ====================================
        AnimList.Add(new Anim ( // Right Short
            (raise) ? UI_START_POS["rightShort"]+UI_OFFSET["rightShort"] : UI_START_POS["rightShort"], 
            (raise) ? UI_START_POS["rightShort"] : UI_START_POS["rightShort"]+UI_OFFSET["rightShort"], 
            (raise) ? RAISE_TIME : LOWER_TIME, rightShort
        ));
        AnimList.Add(new Anim ( // Right Image
            (raise) ? UI_START_POS["rightShort"]+UI_OFFSET["rightShort"] : UI_START_POS["rightShort"], 
            (raise) ? UI_START_POS["rightShort"] : UI_START_POS["rightShort"]+UI_OFFSET["rightShort"], 
            (raise) ? RAISE_TIME : LOWER_TIME, rightImg
        ));
        AnimList.Add(new Anim ( // Right Slab
            (raise) ? UI_START_POS["rightShort"]+UI_OFFSET["rightShort"] : UI_START_POS["rightShort"], 
            (raise) ? UI_START_POS["rightShort"] : UI_START_POS["rightShort"]+UI_OFFSET["rightShort"], 
            (raise) ? RAISE_TIME : LOWER_TIME, rightSlab
        ));
        yield return new WaitForSeconds(0.1f); // Hardcoded Wait
        AnimList.Add(new Anim ( // Left Skill
            (raise) ? UI_START_POS["leftSkill"]+UI_OFFSET["leftSkill"] : UI_START_POS["leftSkill"], 
            (raise) ? UI_START_POS["leftSkill"] : UI_START_POS["leftSkill"]+UI_OFFSET["leftSkill"], 
            (raise) ? RAISE_TIME : LOWER_TIME, leftSkill
        ));
        yield return new WaitForSeconds(0.1f); // Hardcoded Wait
        AnimList.Add(new Anim ( // Confirm
            (raise) ? UI_START_POS["forward"]+UI_OFFSET["forward"] : UI_START_POS["forward"],
            (raise) ? UI_START_POS["forward"] : UI_START_POS["forward"]+UI_OFFSET["forward"],
            (raise) ? RAISE_TIME : LOWER_TIME, confirm
        ));
        AnimList.Add(new Anim ( // Back
            (raise) ? UI_START_POS["back"]+UI_OFFSET["back"] : UI_START_POS["back"],
            (raise) ? UI_START_POS["back"] : UI_START_POS["back"]+UI_OFFSET["back"],
            (raise) ? RAISE_TIME : LOWER_TIME, back
        ));
        // Final Delay
        yield return new WaitForSeconds((raise) ? RAISE_TIME : LOWER_TIME);
    }

    private IEnumerator moveSetTrait(bool raise) { // Enacts setTrait Screen Behavior
        // Referencing Subjects ====================================
        Transform rightTall = scene_setTrait.transform.Find("Trait_SCROLL");
        Transform leftTrait = scene_setTrait.transform.Find("TraitDetailsContainer");
        Transform confirm = scene_setTrait.transform.Find("Confirm_BUTTON");
        Transform back = scene_setTrait.transform.Find("Back_BUTTON");

        // Enact Animations ====================================
        AnimList.Add(new Anim ( // Right Tall
            (raise) ? UI_START_POS["rightTall"]+UI_OFFSET["rightTall"] : UI_START_POS["rightTall"], 
            (raise) ? UI_START_POS["rightTall"] : UI_START_POS["rightTall"]+UI_OFFSET["rightTall"], 
            (raise) ? RAISE_TIME : LOWER_TIME, rightTall
        ));
        yield return new WaitForSeconds(0.1f);
        AnimList.Add(new Anim ( // Left Trait
            (raise) ? UI_START_POS["leftTrait"]+UI_OFFSET["leftTrait"] : UI_START_POS["leftTrait"], 
            (raise) ? UI_START_POS["leftTrait"] : UI_START_POS["leftTrait"]+UI_OFFSET["leftTrait"], 
            (raise) ? RAISE_TIME : LOWER_TIME, leftTrait
        ));
        yield return new WaitForSeconds(0.1f);
        AnimList.Add(new Anim ( // Back
            (raise) ? UI_START_POS["back"]+UI_OFFSET["back"] : UI_START_POS["back"],
            (raise) ? UI_START_POS["back"] : UI_START_POS["back"]+UI_OFFSET["back"],
            (raise) ? RAISE_TIME : LOWER_TIME, back
        ));

        // Only triggers when lowering while raised/clicked
        if(!raise && clickedTrait) {
           AnimList.Add(new Anim ( // Confirm
                UI_START_POS["forward"],
                UI_START_POS["forward"]+UI_OFFSET["forward"],
                LOWER_TIME, confirm
            )); 
        }

        // Final Delay
        yield return new WaitForSeconds((raise) ? RAISE_TIME : LOWER_TIME);
    }

    // Used to raise the confirm button of the setTrait scene
    private IEnumerator moveSetTraitConfirm() {
        if(!clickedTrait) {
            clickedTrait = true;
            // Referencing Subjects ====================================
            Transform confirm = scene_setTrait.transform.Find("Confirm_BUTTON");

            // Enact Animations ====================================
            AnimList.Add(new Anim ( // Confirm
                UI_START_POS["forward"]+UI_OFFSET["forward"],
                UI_START_POS["forward"],
                RAISE_TIME/4, confirm
            )); 
            yield return new WaitForSeconds(0f);
        }
    }

    // Used to lower the confirm button of setTrait scene
    private IEnumerator moveSetTraitDisableConfirm() {
        if(clickedTrait) {
            clickedTrait = false;
            // Referencing Subjects ====================================
            Transform confirm = scene_setTrait.transform.Find("Confirm_BUTTON");

            // Enact Animations ====================================
            AnimList.Add(new Anim ( // Confirm
                UI_START_POS["forward"],
                UI_START_POS["forward"]+UI_OFFSET["forward"],
                LOWER_TIME/4, confirm
            )); 
            yield return new WaitForSeconds(0f);
        }
    }

    private IEnumerator moveFinalConfirm(bool raise) { // Enacts finalConfirm Screen Behavior
        // Referencing Subjects ====================================
        Transform rightMed = scene_FinalConfirm.transform.Find("FinalConfirmContainer");
        Transform leftSquare = scene_FinalConfirm.transform.Find("Race_Icon_Container");
        Transform confirm = scene_FinalConfirm.transform.Find("Start_BUTTON");
        Transform back = scene_FinalConfirm.transform.Find("Back_BUTTON");

        // Enact Animations ====================================
        AnimList.Add(new Anim ( // Right Tall
            (raise) ? UI_START_POS["rightMed"]+UI_OFFSET["rightMed"] : UI_START_POS["rightMed"], 
            (raise) ? UI_START_POS["rightMed"] : UI_START_POS["rightMed"]+UI_OFFSET["rightMed"], 
            (raise) ? RAISE_TIME : LOWER_TIME, rightMed
        ));
        yield return new WaitForSeconds(0.1f);
        AnimList.Add(new Anim ( // Right Tall
            (raise) ? UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"] : UI_START_POS["leftSquare"], 
            (raise) ? UI_START_POS["leftSquare"] : UI_START_POS["leftSquare"]+UI_OFFSET["leftSquare"], 
            (raise) ? RAISE_TIME : LOWER_TIME, leftSquare
        ));
        yield return new WaitForSeconds(0.1f);
        AnimList.Add(new Anim ( // Confirm
            (raise) ? UI_START_POS["forward"]+UI_OFFSET["forward"] : UI_START_POS["forward"],
            (raise) ? UI_START_POS["forward"] : UI_START_POS["forward"]+UI_OFFSET["forward"],
            (raise) ? RAISE_TIME : LOWER_TIME, confirm
        ));
        AnimList.Add(new Anim ( // Back
            (raise) ? UI_START_POS["back"]+UI_OFFSET["back"] : UI_START_POS["back"],
            (raise) ? UI_START_POS["back"] : UI_START_POS["back"]+UI_OFFSET["back"],
            (raise) ? RAISE_TIME : LOWER_TIME, back
        ));
        // Final Delay
        yield return new WaitForSeconds((raise) ? RAISE_TIME : LOWER_TIME);
    }

    //==============================================
    // Privateer Enactment Functions
    //==============================================
    private IEnumerator test() {
        // Load In Delay
        yield return new WaitForSeconds(5);

        Debug.Log("Lowering");
        StartCoroutine(moveSetRace(false));
        yield return new WaitForSeconds(LOWER_TIME);
        Debug.Log("Raising");
        StartCoroutine(moveSetRace(true));
        
        yield return null;    
    }

    private IEnumerator setRace() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveSetRace(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["setRace_Down"]);
        
        // Disabling old, enabling new ==========
        scene_SetRace.SetActive(false);
        scene_ConfirmRace.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveConfirmRace(true));
        StartCoroutine(moveStats(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmRace_Up"]);

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator confirmRace() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveConfirmRace(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmRace_Down"]);
        
        // Disabling old, enabling new ==========
        scene_ConfirmRace.SetActive(false);
        scene_SetClass.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveSetClass(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["setClass_Up"]);

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator confirmRaceBack() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveConfirmRace(false));
        StartCoroutine(moveStats(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmRace_Down"]);
        
        // Disabling old, enabling new ==========
        scene_ConfirmRace.SetActive(false);
        scene_SetRace.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveSetRace(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["setRace_Up"]);

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator setClass() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveSetClass(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["setClass_Down"]);
        
        // Disabling old, enabling new ==========
        scene_SetClass.SetActive(false);
        scene_ConfirmClass.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveConfirmClass(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmClass_Up"]);

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator setClassBack() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveSetClass(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["setClass_Down"]);
        
        // Disabling old, enabling new ==========
        scene_SetClass.SetActive(false);
        scene_ConfirmRace.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveConfirmRace(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmRace_Up"]);

        // Enabling Input ==========
        clickedClass = false;
        mouseLock.SetActive(false);
    }

    private IEnumerator confirmClass() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveConfirmClass(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmClass_Down"]);
        
        // Disabling old, enabling new ==========
        scene_ConfirmClass.SetActive(false);
        scene_setTrait.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveSetTrait(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["setTrait_Up"]);

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator confirmClassBack() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveConfirmClass(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmClass_Down"]);
        
        // Disabling old, enabling new ==========
        scene_ConfirmClass.SetActive(false);
        scene_SetClass.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveSetClass(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["setClass_Up"]);
        clickedClass = false;

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator setTrait() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveSetTrait(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["setTrait_Down"]);
        
        // Disabling old, enabling new ==========
        scene_setTrait.SetActive(false);
        scene_FinalConfirm.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveFinalConfirm(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["finalConfirm_Up"]);
        clickedTrait = false;

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator setTraitBack() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveSetTrait(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["setTrait_Down"]);
        
        // Disabling old, enabling new ==========
        scene_setTrait.SetActive(false);
        scene_ConfirmClass.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveConfirmClass(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["confirmClass_Up"]);
        clickedTrait = false;

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    private IEnumerator finalConfirm() { //==================
        mouseLock.SetActive(true); // Locking Input
        
        // Enabling Loading Screen
        scene_Loading.SetActive(true);

        // Exiting final scene
        StartCoroutine(moveFinalConfirm(false));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(moveTitle(false));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(moveStats(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["finalConfirm_Down"]);

        // Disabling final scene
        scene_FinalConfirm.SetActive(false);
    }

    private IEnumerator finalConfirmBack() { //==================
        mouseLock.SetActive(true); // Locking Input

        // Exiting Current Scene ==========
        StartCoroutine(moveFinalConfirm(false));
        yield return new WaitForSeconds(PLAYBACK_TIME["finalConfirm_Down"]);
        
        // Disabling old, enabling new ==========
        scene_FinalConfirm.SetActive(false);
        scene_setTrait.SetActive(true);
        yield return new WaitForSeconds(TRANSITION_DELAY);
        
        // Entering New Scene ==========
        StartCoroutine(moveSetTrait(true));
        yield return new WaitForSeconds(PLAYBACK_TIME["setTrait_Up"]);
        clickedTrait = false;

        // Enabling Input ==========
        mouseLock.SetActive(false);
    }

    //==============================================
    // Public Functions
    //==============================================
    // Returns the string/int dictionary for animCases
    public Dictionary<string, int> getAnimCases() {
        return ANIM_CASES;
    }

    // Enables the confirm button of the setClass scene
    public void enableConfirmClass() {
        StartCoroutine(moveSetClassConfirm());
    }

    // Enables the confirm button of the setClass scene
    public void enableConfirmTrait() {
        StartCoroutine(moveSetTraitConfirm());
    }

    // Disables the confirm button of the setClass scene
    // Soley used for powerful
    public void disableConfirmTrait() {
        StartCoroutine(moveSetTraitDisableConfirm());
    }

    // Plays an animation based off of the case number
    // Returns the required wait time (seconds) for each animation
    public void PlayAnim(int animCase) {
        switch (animCase) {
            case 0:
                Debug.Log("Playing setRace");
                StartCoroutine(setRace());
            break;
            case 1:
                Debug.Log("Playing confirmRace");
                StartCoroutine(confirmRace());
            break;
            case 2:
                Debug.Log("Playing confirmRaceBack");
                StartCoroutine(confirmRaceBack());
            break;
            case 3:
                Debug.Log("Playing setClass");
                StartCoroutine(setClass());
            break;
            case 4:
                Debug.Log("Playing setClassBack");
                StartCoroutine(setClassBack());
            break;
            case 5:
                Debug.Log("Playing confirmClass");
                StartCoroutine(confirmClass());
            break;
            case 6:
                Debug.Log("Playing confirmClassBack");
                StartCoroutine(confirmClassBack());
            break;
            case 7:
                Debug.Log("Playing setTrait");
                StartCoroutine(setTrait());
            break;
            case 8:
                Debug.Log("Playing setTraitBack");
                StartCoroutine(setTraitBack());
            break;
            case 9:
                string name = inputName.text;
                if(string.IsNullOrEmpty(name)) {
                    Debug.Log("Name does not meet criteria");
                } else {
                    Debug.Log("Playing finalConfirm");
                    StartCoroutine(finalConfirm());    
                }
            break;
            case 10:
                Debug.Log("Playing finalConfirmBack");
                StartCoroutine(finalConfirmBack());
            break;
            default:
                Debug.Log("How did this even happen...");
            break;
        }
    }

    // Debugging
    public void Start() {
        // StartCoroutine(test());
    }

    // Plays the animations
    public void Update() {
        List<Anim> clonedAnimList = new List<Anim>(AnimList);
        foreach (Anim animation in clonedAnimList) {
            if(animation.update()) {
                Debug.Log("Animation Completed!");
                AnimList.Remove(animation);
            }
        }
    }

}
