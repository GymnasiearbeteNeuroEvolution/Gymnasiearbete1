using UnityEngine;
using System.Collections;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Collections.Generic;
namespace FishTest
{

    public class FishEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        ulong _evalCount;
        bool _stopConditionSatisfied;
        NEATController neController;
        FitnessInfo fitness;

        Dictionary<IBlackBox, FitnessInfo> dict = new Dictionary<IBlackBox, FitnessInfo>();

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        public FishEvaluator(NEATController se)
        {
            this.neController = se;
        }

        public IEnumerator Evaluate(IBlackBox box)
        {
            if (neController != null)
            {
                neController.Evaluate(box);
                yield return new WaitForSeconds(neController.TrialDuration);
                neController.StopEvaluation(box);
                float fit = neController.GetFitness(box);

                FitnessInfo fitness = new FitnessInfo(fit, fit);
                dict.Add(box, fitness);
            }
        }

        public void Reset()
        {
            this.fitness = FitnessInfo.Zero;
            dict = new Dictionary<IBlackBox, FitnessInfo>();
        }

        public FitnessInfo GetLastFitness()
        {

            return this.fitness;
        }


        public FitnessInfo GetLastFitness(IBlackBox phenome)
        {
            if (dict.ContainsKey(phenome))
            {
                FitnessInfo fit = dict[phenome];
                dict.Remove(phenome);

                return fit;
            }

            return FitnessInfo.Zero;
        }
    }

}