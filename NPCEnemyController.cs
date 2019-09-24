using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

class NPCEnemyController : MonoBehaviour{

    private static Animator animator;

    private NavMeshAgent nav;

    private SphereCollider colcav;
  
    private GameObject player;
    
    private bool isAtacando,isAndandoCA,isAndandoSA,isMeterA,isSacarA,isParado;

    private float speed = 0.0f;
    private float h = 0.0f, v = 0.0f;
  
    private bool DEBUG = false, DEBUG_DRAW = false;

    private Vector3 direction; //Donde está player en relación a NPC

    private static float distance = 0.0f; //Distancia entre jugador y NPC

    private float angle = 0.0f; //Ángulo entre jugador y NPC

    private static  bool playerInSight = false; //Está el jugador en el FOV del NPC?

    private float fieldOfViewAngle = 150;
    private int sacar = 0;
    



	// Use this for initialization
	private void Start () {

        animator = gameObject.GetComponent<Animator>();
        nav = gameObject.GetComponent<NavMeshAgent>();
        colcav = gameObject.GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player");

	}
	
	// Update is called once per frame
	private void Update () {
        Debug.Log(playerInSight);

        

       
        if(playerInSight){
            gameObject.transform.rotation = //Girar al NPC 
                Quaternion.Slerp(gameObject.transform.rotation, //desde donde mira ahora
                                 Quaternion.LookRotation(direction), //hacia la dirección del player
                                 0.1f //en un tiempo de 0.1f
                                );

                                
          
        
        }else{
             
            nav.velocity = new Vector3(0,0,0); 
            //Debug.Log(distance);
            if(isMeterA){

               animator.SetBool("Parado",true);
               animator.SetBool("MeterA",false);
               animator.SetBool("SacarA",false);
               isMeterA = false;
               isParado = true;

            }


        }

        

        
	}


    private void FixedUpdate()
    {
        h = angle;
        v = distance;
        speed = distance / Time.deltaTime;
     
    }


    private void OnTriggerStay(Collider colpollo)
    {
        
       
        if(colpollo.transform.tag.Equals("Player")){
           
            //vector = destino - origen
            //direction = player.transform.position - this.transform.position;
            direction = player.transform.position - gameObject.transform.position;
            distance = Vector3.Magnitude(direction);
            angle = Vector3.Dot(gameObject.transform.forward, player.transform.position);

          

            playerInSight = false;
            float calculateAngle = Vector3.Angle(direction, gameObject.transform.forward);
            //Si el player está en el campo de visión
            if(calculateAngle < 0.5f*fieldOfViewAngle){
                RaycastHit hit; 
                if(DEBUG_DRAW){
                    Debug.DrawRay(gameObject.transform.position + gameObject.transform.up,
                                  direction.normalized, Color.green);
                }
                //Trazo un rayo desde NPC hasta player
                if(Physics.Raycast(gameObject.transform.position + gameObject.transform.up, direction.normalized, 
                                   out hit, colcav.radius)){
                    //Si lo primero que localiza el rayo es el jugador
                    if(hit.collider.gameObject == player){
                        playerInSight = true;
                        
                        
                    }
                }
            }

            //Si después de toda la comprobación anterior, el player está en FoV del NPC
            if(playerInSight && distance < 22.5){
                
                nav.SetDestination(player.transform.position);
                CalculatePathLength(player.transform.position);

                isAndandoCA = true;
                animator.SetBool("AndarCA",true);
                animator.SetBool("SacarA",false);
               
            }

            if(distance < 23 && distance > 22.5 && sacar == 1  && playerInSight){
                
                isSacarA = true;
                animator.SetBool("SacarA",true); 
                animator.SetBool("Parado",false);

            }

            if(distance < 2.5 && playerInSight ){

                isAtacando = true;
                float n = Random.Range(0,1);

                if(isSacarA){
                    
                  animator.SetBool("Atacar",true);
                  animator.SetBool("SacarA",false);
                  isSacarA = false;

                    

                }

                if(isAndandoCA){
                  animator.SetBool("Atacar",true);
                  animator.SetBool("AndarCA",false);
                  isAndandoCA = false;

                 
                }


                

            }

            if(distance > 2.5 && distance < 3 && isAtacando && playerInSight){

                isAtacando = false;
                isAndandoCA = true;
                animator.SetBool("AndarCA",true);
                animator.SetBool("Atacar",false);


                

            }

            if(distance > 23.5f && distance < 25){

              if(isAndandoCA){
                animator.SetBool("Parado",true);
                animator.SetBool("AndarCA",false);
                isAndandoCA = false;
                isParado = true;
              }

              if(isSacarA){
                animator.SetBool("Parado",true);
                animator.SetBool("SacarA",false);
                isSacarA = false;
                isParado = true;

              }



            }
            
            if(!playerInSight){

                animator.SetBool("SacarA",true);
                isSacarA = true;

                if(isAndandoCA){
                  animator.SetBool("AndarCA",false);
                  isAndandoCA = false;
                }

                if(isMeterA){
                  animator.SetBool("MeterA",false);
                  isMeterA = false;
                }

                if(isAtacando){
                  animator.SetBool("Atacar",false);
                  isAtacando = false;

                }

                if(isParado){
                  animator.SetBool("Parado",false);
                  isParado = false;

                }


            }

           





        }
    }



    private void OnTriggerExit(Collider colpollo)
    {
        if(colpollo.transform.tag == "Player"){
            distance = 0;
            angle = 0;
            playerInSight = false;

            if(isAtacando){

                isAtacando = false;
                isMeterA = true;
                isSacarA = false;
                animator.SetBool("MeterA",true);
                animator.SetBool("Atacar",false);
                

            }

            if(isAndandoCA){
                
                isAndandoCA = false;
                isMeterA = true;
                isSacarA = false;
                animator.SetBool("MeterA",true);
                animator.SetBool("AndarCA",false);


            }

            sacar = 0;
            
        }



    }


    private void OnTriggerEnter(Collider colpollo){

        if(colpollo.transform.tag == "Player"){
           
            sacar = 1;
            
        }


    }






    private float CalculatePathLength(Vector3 targetPosition){

        NavMeshPath path = new NavMeshPath();
        if (nav.enabled)
        {
            nav.CalculatePath(targetPosition, path);
            Vector3[] allTheWaypoints = new Vector3[path.corners.Length + 2];
            allTheWaypoints[0] = gameObject.transform.position;
            allTheWaypoints[allTheWaypoints.Length - 1] = targetPosition;

            for (int i = 0; i < path.corners.Length; i++)
            {
                allTheWaypoints[i + 1] = path.corners[i];
            }

            float pathLength = 0;
            for (int i = 0; i < allTheWaypoints.Length - 1; i++)
            {
                pathLength += Vector3.Distance(allTheWaypoints[i], allTheWaypoints[i + 1]);
                if (DEBUG_DRAW)
                {
                    Debug.DrawLine(allTheWaypoints[i], allTheWaypoints[i + 1], Color.gray);
                }
            }

            return pathLength;
        }
        else
        {
            return 0;
        }



    }



  


   
}
