using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioTextSynchronizer;

public class Test : MonoBehaviour
{

    [SerializeField] private TextSynchronizer textSynchronizer;
    [SerializeField] private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        textSynchronizer.Play(true); // Text sync begins
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
