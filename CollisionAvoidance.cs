using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoidance : SteeringBehaviour
{

    // Lista de todos los posibles objetivos a considerar
    public List<Agente> targets;

    [Range(1,10)]
    public float radius = 1;

    private Vector3 evasion;

    public override Steering getSteering(AgenteNPC agente)
    {

        Vector3 relativePos, relativeVelocity;
        float relativeSpeed, timeToCollision;
        float distance, minSeparation;
        float shortestTime = Mathf.Infinity;
        Agente firstTarget = null;
        Vector3 firstRelativePos = Vector3.zero, firstRelativeVelocity = Vector3.zero;
        float firstMinSeparation = 0, firstDistance = 0;

        foreach(Agente t in targets)
        {
            // Calculo del tiempo hasta la colision
            relativePos = t.getPosition() - agente.getPosition();
            relativeVelocity = t.getVelocity() - agente.getVelocity();
            relativeSpeed = relativeVelocity.magnitude;
            timeToCollision = Vector3.Dot(relativePos,relativeVelocity) /relativeSpeed * relativeSpeed;


            // Determina si va a haber una colision
            distance = relativePos.magnitude;
            minSeparation = distance - (relativeSpeed * timeToCollision);

            if (minSeparation > (2 * radius))
                continue;

            // Comrpuebo si es el mas cercano/corto
             if (timeToCollision > 0 && timeToCollision < shortestTime)
            {
                shortestTime = timeToCollision;
                firstTarget = t;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVelocity = relativeVelocity;
            }
        }

        // Si no tenemos objtivo, salimos
        if (firstTarget == null)
            return new Steering(0,Vector3.zero);

        // Si vamos a colisionar y ya estamos colisionando, hacemos steering 
        // basado en la posicion actual;
        if (firstMinSeparation <= 0 || firstDistance < 2 * radius)
            relativePos = firstTarget.getPosition() - agente.getPosition();
        else
            relativePos = firstRelativePos + (firstRelativeVelocity * shortestTime);

        // Evitar al objetivo
        relativePos = relativePos.normalized;
        evasion = relativePos * agente.getMaxAcceleration();
        return new Steering(0,evasion);

    }

    private void OnDrawGizmos() // Gizmo: una línea en la dirección del objetivo
    {
        Vector3 from = transform.position;// Origen de la línea
        from += new Vector3(0, 1, 0);

        Gizmos.color = Color.red;   // Esfera de comprobacion de separacion
        Gizmos.DrawWireSphere(from, radius);

    }

}
