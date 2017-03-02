/*
using UnityEngine;
using System.Collections;

public class Neuron : MonoBehaviour {

    private readonly uint _id;
    private readonly NodeType _neuronType;
    double _inputValue, _outputValue;

    public Neuron(uint id, NodeType neuronType)
    {
        _id = id;
        _neuronType = neuronType;

        // If the node is a bias set it to 1, otherwise set it to 0.
        _outputValue = (NodeType.Bias == _neuronType) ? 1.0 : 0.0;
    }

    public uint Id
    {
        get { return _id; }
    }
    public NodeType NeuronType
    {
        get { return _neuronType;  }
    }    
    /// <summary>
    /// Gets or sets the neuron's current input value. This is set to a fixed value for bias neurons.
    /// </summary>
    public double InputValue
    {
        get { return _inputValue; }
        set
        {
            if (NodeType.Bias == _neuronType || NodeType.Input == _neuronType)
            {
                throw new System.ApplicationException("Attempt to set the InputValue of bias or input neuron. Bias neurons have no input, and Input neuron signals should be passed in via their OutputValue property setter.");
            }
            _inputValue = value;
        }
    }

    /// <summary>
    /// Gets or sets the neuron's current output value. This is set to a fixed value for bias neurons.
    /// </summary>
    public double OutputValue
    {
        get { return _outputValue; }
        set
        {
            if (NodeType.Bias == _neuronType)
            {
                throw new System.ApplicationException("Attempt to set the OutputValue of a bias neuron.");
            }
            _outputValue = value;
        }
    }

}
*/