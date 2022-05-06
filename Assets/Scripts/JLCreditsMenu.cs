using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class JLCreditsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       JLAudioManager.Instance.PlaySound("Death"); 
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.anyKeyDown) {
           SceneManager.LoadScene("JLMainMenu");
       } 
    }
}
