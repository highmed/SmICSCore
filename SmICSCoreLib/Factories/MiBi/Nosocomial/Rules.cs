using Newtonsoft.Json;
using RulesEngine.Extensions;
using RulesEngine.Models;
using SmICSCoreLib.Factories.MiBi.PatientView;
using System;
using System.Collections.Generic;
using System.IO;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public class Rules
    {
        public static List<string> GetResistances(Pathogen pathogen)
        {
            List<string> resistances = new List<string>();
            using (StreamReader reader = new StreamReader(@"./Resources/Rules/ResistanceRules.json"))
            {
                RuleParameter input = new RuleParameter("input", pathogen);
                string json = reader.ReadToEnd();
                Workflow[] resistanceRules = JsonConvert.DeserializeObject<List<Workflow>>(json).ToArray();
                RulesEngine.RulesEngine engine = new RulesEngine.RulesEngine(resistanceRules, null);
                List<RuleResultTree> ruleResult = engine.ExecuteAllRulesAsync("MRERecognition", input).Result;
                foreach(RuleResultTree result in ruleResult)
                {
                    if(result.IsSuccess && !string.IsNullOrEmpty(result.Rule.SuccessEvent))
                    {
                        resistances.Add(result.Rule.SuccessEvent);
                    }
                    else if (result.IsSuccess && string.IsNullOrEmpty(result.Rule.SuccessEvent))
                    {
                        resistances.Add(GetSuccessEvent(result));
                    }
                }
            }
            return resistances.Count > 0 ? resistances : null;
        }

        private static string GetSuccessEvent(RuleResultTree ruleResult)
        {
            foreach(RuleResultTree child in ruleResult.ChildResults)
            {
                if(child.IsSuccess && !string.IsNullOrEmpty(child.Rule.SuccessEvent))
                {
                    return child.Rule.SuccessEvent;
                }
                else if(child.IsSuccess && string.IsNullOrEmpty(child.Rule.SuccessEvent))
                {
                    return GetSuccessEvent(child);
                }
            }
            throw new NotImplementedException("Missed Case");
        }

        public static List<string> GetPossibleMREClasses(List<string> pathogenCodes)
        {
            List<string> possibleMREClasses = new List<string>();
            using (StreamReader reader = new StreamReader(@"./Resources/Rules/ResistanceRules.json"))
            {
                RuleParameter input = new RuleParameter("input", pathogenCodes);
                string json = reader.ReadToEnd();
                Workflow[] resistanceRules = JsonConvert.DeserializeObject<List<Workflow>>(json).ToArray();
                RulesEngine.RulesEngine engine = new RulesEngine.RulesEngine(resistanceRules, null);
                List<RuleResultTree> ruleResult = engine.ExecuteAllRulesAsync("MREClassByPathogen", input).Result;
                foreach (RuleResultTree result in ruleResult)
                {
                    if (result.IsSuccess)
                    {
                        possibleMREClasses.Add(result.Rule.SuccessEvent);
                    }
                }
            }
            return possibleMREClasses;
        }
    }
}