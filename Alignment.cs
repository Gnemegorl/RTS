using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : SteeringBehaviour
{
    public List<Agente> targets;

    // Limite para determinar si se considera uno de los objetivos para calcular la direccion
    [Range(1, 10)]
    public float threshold = 5;

    [Range(0.1f, 0.5f)]
    public float epsilon = 5;

    public override Steering getSteering(AgenteNPC agente)
    {
        
        int count = 0;
        float distance;
        float orientacion = Cuerpo.positionToAngle(Vector3.zero);
        float direction = 1;
        float alpha;

        foreach (Agente t in targets)
        {   
            distance = (t.getPosition() - agente.getPosition()).magnitude;
            
            if (distance > threshold) continue;

            orientacion += t.getOrientation();
            count++;
        }

        if (count > 0)
        {
            orientacion /= count;
            orientacion -= agente.getOrientation();
        }


        return new Steering(orientacion ,Vector3.zero);
    }

    private void OnDrawGizmos()
    {
        Vector3 from = transform.position; // Origen de la línea
        from += new Vector3(0, 1, 0);   

        Gizmos.color = Color.yellow;   // Esfera de comprobacion de separacion
        //Gizmos.DrawWireSphere(from, threshold);
    }
}
