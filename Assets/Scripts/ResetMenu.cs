using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetMenu : MonoBehaviour
{
    public Transform playerHead;
    public GameObject resetMenu;
    public float distancefromHead = 1.8f;
    InputActionProperty showbutton;

    // Start is called before the first frame update
    void Start()
    {
        resetMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        resetMenu.transform.position = playerHead.position + new Vector3(playerHead.forward.x, 0, playerHead.forward.z).normalized * distancefromHead;
        resetMenu.transform.LookAt(new Vector3(playerHead.position.x, resetMenu.transform.position.y, playerHead.position.z));
        resetMenu.transform.forward *= -1;
    }
}
