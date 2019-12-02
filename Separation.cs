using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBehaviour
{
    // Lista de todos los posibles objetivos a considerar
    public List<Agente> targets;

    // Limite para el cual considerar la accion
    [Range(1f,10f)]
    public float threshold = 1f;

    [Range(1f, 10f)]
    public float decayCoefficient = 5f;

    public override Steering getSteering(AgenteNPC agente)
    {
        float maxAcc = agente.getMaxAcceleration();
        Steering steering = new Steering(0,Vector3.zero);
        Vector3 direction;
        float distance;
        float strength;

        foreach (Agente t in targets)
        {
            direction =  agente.getPosition() - t.getPosition();
            distance = direction.magnitude;

            if (distance < threshold)
            {
                strength = Mathf.Min(decayCoefficient/(distance*distance),maxAcc);
                direction = direction.normalized;
                steering.linear += strength * direction;
            }
        }

        return steering;
    }

    private void OnDrawGizmos() // Gizmo: una línea en la dirección del objetivo
    {
        Vector3 from = transform.position; // Origen de la línea
        from += new Vector3(0, 1, 0);

        Gizmos.color = Color.red;   // Esfera de comprobacion de separacion
        //Gizmos.DrawWireSphere(from, threshold);
    }

}
