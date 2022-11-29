using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Animator animator;
    public float speed;
    Gamemanage gamemanage;
    inventory inventorys;
    public UnityEngine.Rendering.Universal.Light2D light;
    public static Lazy<Movement> instance;


    private void Awake()
    {
        instance = new Lazy<Movement>(() => this, true);
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gamemanage = Gamemanage.instance.Value;
        inventorys = inventory.instance.Value;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate()
    {
        if (!inventorys.openInventory && !gamemanage.onFishing)
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            if (inputX > 0f || inputY > 0f || inputY < 0f || inputX < 0f)
            {
                animator.SetBool("moved", true);
                animator.SetBool("idle", false);
                animator.SetFloat("x", inputX);
                animator.SetFloat("y", inputY);
                Vector2 move = new Vector2(inputX * speed, inputY * speed);
                transform.Translate(move * Time.deltaTime);
            }
            else
            {
                animator.SetBool("moved", false);
                animator.SetBool("idle", true);
            }

        }
        else
        {
            animator.SetBool("moved", false);
            animator.SetBool("idle", true);
        }
       
    }

   
}
