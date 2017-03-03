using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Collections;
using UnityEngine;

namespace OrganismTest
{
    class UnityParallelListEvaluatorOrganism<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
            where TGenome : class, IGenome<TGenome>
            where TPhenome : class
    {

        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        //readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        NEATControllerOrg controller;

        #region Constructor

        /// <summary>
        /// Constructor taking a genomedecoder and phenomeEvaluator.
        /// </summary>
        public UnityParallelListEvaluatorOrganism(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                         IPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                          NEATControllerOrg controller)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            this.controller = controller;
        }

        #endregion

        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        public IEnumerator Evaluate(IList<TGenome> genomeList)
        {
            yield return Coroutiner.StartCoroutine(evaluateList(genomeList));
        }

        private IEnumerator evaluateList(IList<TGenome> genomeList)
        {
            Dictionary<TGenome, TPhenome> dict = new Dictionary<TGenome, TPhenome>();
            Dictionary<TGenome, FitnessInfo[]> fitnessDict = new Dictionary<TGenome, FitnessInfo[]>();
            for (int i = 0; i < controller.Trials; i++)
            {
                Utility.Log("Iteration " + (i + 1));
                _phenomeEvaluator.Reset();
                dict = new Dictionary<TGenome, TPhenome>();
                foreach (TGenome genome in genomeList)
                {

                    TPhenome phenome = _genomeDecoder.Decode(genome);
                    if (null == phenome)
                    {   //Bad genome
                        genome.EvaluationInfo.SetFitness(0.0);
                        genome.EvaluationInfo.AuxFitnessArr = null;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            fitnessDict.Add(genome, new FitnessInfo[controller.Trials]);
                        }
                        dict.Add(genome, phenome);
                        //if (!dict.ContainsKey(genome))
                        //{
                        //    dict.Add(genome, phenome);
                        //    fitnessDict.Add(phenome, new FitnessInfo[_optimizer.Trials]);
                        //}
                        Coroutiner.StartCoroutine(_phenomeEvaluator.Evaluate(phenome));


                    }
                    yield return new WaitForSeconds(controller.TrialDuration/4); 
                }
                yield return new WaitForSeconds(controller.TrialDuration);
                foreach (TGenome genome in dict.Keys)
                {
                    TPhenome phenome = dict[genome];
                    if (phenome != null)
                    {

                        FitnessInfo fitnessInfo = _phenomeEvaluator.GetLastFitness(phenome);

                        fitnessDict[genome][i] = fitnessInfo;
                    }
                }
            }
            foreach (TGenome genome in dict.Keys)
            {
                TPhenome phenome = dict[genome];
                if (phenome != null)
                {
                    double fitness = 0;

                    for (int i = 0; i < controller.Trials; i++)
                    {

                        fitness += fitnessDict[genome][i]._fitness;

                    }
                    var fit = fitness;
                    fitness /= controller.Trials; //Average fitness


                    genome.EvaluationInfo.SetFitness(fitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessDict[genome][0]._auxFitnessArr;
                }
            }
        }

        private IEnumerator StopForBool(bool _bool)
        {
            while (_bool != true)
            {
                yield return null;
            }
        }

        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }
    }
}
