using UnityEngine;
using System.Collections;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

public class Fish : MonoBehaviour
{
    private GameObject shark;
    private double _fitness = 0;
    double distanceToShark;
    private bool _isHit = false;

    public bool IsHit
    {
        get { return _isHit; }
    }

    public double Fitness
    {
        get { return _fitness; }
    }

    public Fish(Vector2 pos, GameObject prefab)
    {
    }
    void Start()
    {
        NeatGenomeFactory neatGenomFact = new NeatGenomeFactory();
        NeatEvolutionAlgorithm neatEvoAlgorithm= new NeatEvolutionAlgorithm();
        
        shark = GameObject.FindGameObjectWithTag("Shark");
    }
    void Update()
    {
        EvaluateFitness();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shark")
        {
            _isHit = true;
        }
    }

    public void EvaluateFitness()
    {
        //distanceToShark = Vector3.Distance(shark.transform.position, transform.position); //Distance from shark
        _fitness += 10 * Time.deltaTime;
        /*if (distanceToShark > FishSettings.SafeDistance)
        {
            _fitness += 5*Time.deltaTime;
        }
        else
        {
            _fitness += (distanceToShark / FishSettings.SafeDistance) * 5 * Time.deltaTime;
        }*/
    }
}
    