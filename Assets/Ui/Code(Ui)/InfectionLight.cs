using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionLight : MonoBehaviour
{
    public UnityEngine.Experimental.Rendering.Universal.Light2D freeForm;
    public static InfectionLight Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        freeForm = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
    }
}
