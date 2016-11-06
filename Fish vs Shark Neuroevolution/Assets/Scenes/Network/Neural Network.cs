/*using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class NeuralNetwork {
    public const float ActivationCost = 0.5f; 
    List<float> WeightsHidden = new List<float>();
    List<float> WeightsOutput = new List<float>();
    List<float> WeightsInput= new List<float>();
    List<List<float>> Weights = new List<List<float>>();

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Weights.Add(WeightsInput);
        Weights.Add(WeightsHidden);
        Weights.Add(WeightsOutput);
    }
}
/// <summary>
/// Updates weights depending on the result, to make the actual output closer to the target output
/// </summary>
public class BackPropogation
{
    private BackPropogation() { }
    public static void GetFitness(float TimeAlive)
    {
        
    }

    public float ErrorDeterminator(Vector2 x, Vector2 y)
    {
        float distance;
        distance = Mathf.Abs(Vector2.Distance(x, y));
        return distance;
    }
    public float[] BackProp()
    {
        float[] weights = null;
        return weights;
    }
    /*
    InitNetworkWeights(float smallRand, List<List<float>> Weights )
    {
        NeuralNetwork neuro = new NeuralNetwork();

        do
        {
            foreach (List<float> item in Weights)
            {
                float prediction = NeuralNetwork.PredictOutput(neuro);  // forward pass
                float actual = NeuralNetwork.CalcFitness(neuro);
                float error = prediction - actual;
                //∆wkj = εδkaj
                //förändringen av outputvikten = lilla epsilon * produkten av felet med 
                // derivatan * aktiveringsfunktionen i output * aktiveringskostnaden  i hidden 

                float differenceInOutput = float.Epsilon * NeuralNetwork.ActivationCost;

            }
        } while (true);
        
    }
}
    /*
      /*initialize network weights (often small random values)
  do
     forEach training example named ex
        compute {\displaystyle \Delta w_{h}} \Delta w_h for all weights from hidden layer to output layer  // backward pass
        compute {\displaystyle \Delta w_{i}} \Delta w_i for all weights from input layer to hidden layer   // backward pass continued
        update network weights // input layer not modified by error estimate
  until all examples classified correctly or another stopping criterion satisfied
  return the network*/


  