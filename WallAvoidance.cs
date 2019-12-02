using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision
{
    Vector3 position;
    Vector3 normal;

    public Collision(Vector3 pos, Vector3 norm)
    {
        position = pos;
        normal = norm;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public Vector3 getNormal()
    {
        return normal;
    }
}

public class CollisionDetector
{

    public CollisionDetector()
    {

    }

    public Collision getCollision(Vector3 agentPosition, Vector3 rayVector)
    {
        RaycastHit hit;
        float maxDistance = 3f;
        bool impacto = false;

        if (Physics.Raycast(agentPosition, rayVector, out hit, maxDistance))
            impacto = true;

        if (impacto)
            return new Collision(hit.point, hit.normal);
        return null;
    }
}

public class WallAvoidance : Seek
{
    public AgenteNPC targetWallAvoidance;
    public CollisionDetector cd;
    public float avoidDistance = 4f;
    public float lookAhead = 3f;
    public float anguloApertura;
    public Vector3 rayVector, rayVectorL, rayVectorR;

    public override Steering getSteering(AgenteNPC agente)
    {
        // Inicializo el detector de colisiones
        cd = new CollisionDetector();
        // Calculo  el rayo del vector de colision
        Vector3 targetPosition = targetWallAvoidance.getPosition();
        // Vector3 que representan los "bigotes" que buscaran colisiones 
        //rayVector = agente.getVelocity().normalized;
        rayVector = (targetPosition - agente.getPosition()).normalized;

        //float angle = Cuerpo.positionToAngle(agente.getPosition());
        float angle = positionToAngle(rayVector);
        rayVectorL = orientationToVector(angle + anguloApertura * Mathf.Deg2Rad);
        rayVectorR = orientationToVector(angle - anguloApertura * Mathf.Deg2Rad);
        rayVector *= lookAhead;
        rayVectorL *= lookAhead * .8f;
        rayVectorR *= lookAhead * .8f;

        // Encuentra las posibles colisiones
        Collision collision, collisionL, collisionR;
        collision = cd.getCollision(agente.getPosition(), rayVector);
        collisionL = cd.getCollision(agente.getPosition(), rayVectorL);
        collisionR = cd.getCollision(agente.getPosition(), rayVectorR);

        target = new AgenteNPC(targetPosition);
        if (collision == null && collisionL == null && collisionR == null) return base.getSteering(agente);

        if (collision != null) targetPosition = collision.getPosition() + collision.getNormal() * avoidDistance;
        if (collisionL != null) targetPosition = collisionL.getPosition() + collisionL.getNormal() * avoidDistance;
        if (collisionR != null) targetPosition = collisionR.getPosition() + collisionR.getNormal() * avoidDistance;

        target = new AgenteNPC(targetPosition);
        return base.getSteering(agente);
    }

    private float positionToAngle(Vector3 linear)
    {
        return Mathf.Atan2(linear.x, linear.z) /* Mathf.Rad2Deg*/;
    }

    private Vector3 orientationToVector(float angle)
    {
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    private void OnDrawGizmos() // Gizmo: una línea en la dirección del objetivo
    {
        Vector3 from = transform.position; // Origen de la línea
        Vector3 to = transform.position + rayVector; // Detino de la línea
        Vector3 toL = transform.position + rayVectorL; // Detino de la línea
        Vector3 toR = transform.position + rayVectorR; // Detino de la línea

        Vector3 elevation = new Vector3(0, 0.5f, 0); // Elevación para no tocar el suelo

        from = from + elevation;
        to = to + elevation;

        Gizmos.color = Color.yellow;   // Bigote central
        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.red;   // Bigote izq
        Gizmos.DrawLine(from, toL);

        Gizmos.color = Color.blue;   // Bigote der
        Gizmos.DrawLine(from, toR);
    }
}
