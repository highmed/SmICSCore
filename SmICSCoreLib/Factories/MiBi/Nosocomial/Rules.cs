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
                List<RuleResultTree> ruleResult = engine.ExecuteAllRulesAsync("Resistance", input).Result;
                foreach(RuleResultTree result in ruleResult)
                {
                    if(result.IsSuccess)
                    {
                        resistances.Add(result.Rule.SuccessEvent);
                    }
                }
            }
            return resistances.Count > 0 ? resistances : null;
        }

        public static List<string> GetPossibleMREClasses(string pathogen)
        {
            List<string> possibleMREClasses = new List<string>();
            using (StreamReader reader = new StreamReader(@"./Resources/Rules/ResistanceRules.json"))
            {
                RuleParameter input = new RuleParameter("input", pathogen);
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