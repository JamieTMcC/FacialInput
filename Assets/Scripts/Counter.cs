using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Counter : MonoBehaviour
{
    /// <summary>
    /// container for keeping track of the score
    /// </summary>


    private TMP_Text text;
    private string default_text;
    public int score = 0;
    public int count = 0;

    AudioSource audioData;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        default_text = text.text;
        audioData = GetComponent<AudioSource>();
    }

    public void Increment()
    {
        audioData.Play(0);
        score++;
        text.text = default_text + "\n" + score.ToString() + "/" + count.ToString();
    }

}
