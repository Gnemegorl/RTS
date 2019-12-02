using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Seek
{
    // Limite para determinar si se considera uno de los objetivos para calcular el centro de masa
    [Range(1, 10)]
    public float threshold = 2;

    public List<Agente> targets;

    override
    public Steering getSteering(AgenteNPC agente)
    {
        int count = 0;
        Vector3 centerOfMass = Vector2.zero;
        Vector3 direction;
        float distance;

        foreach (Agente t in targets)
        {
            direction = t.getPosition() - agente.getPosition();
            distance = Vector3.Magnitude(direction);

            if (distance > threshold) continue;
            
            centerOfMass += t.getPosition();
            count++;
            
        }

        if (count == 0) return null;

        centerOfMass /= count;
        target = new Agente(centerOfMass);
        

        return base.getSteering(agente);
        
    }


    private void OnDrawGizmos()
    {
        Vector3 from = transform.position; // Origen de la línea
        from += new Vector3(0, 1, 0);

        Gizmos.color = Color.yellow;   // Esfera de comprobacion de separacion
        //Gizmos.DrawWireSphere(from, threshold);
    }

}
