using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DSPEmulatorUI
{
    class EffectsFactory
    {
        private Dictionary<string, Type> effectsDict = new Dictionary<string, Type>();
        public string[] EffectsNames => effectsDict.Keys.OrderBy(x => x).ToArray();

        public EffectsFactory()
        {
            var asm = Assembly.GetExecutingAssembly();

            Type typeIEffect = asm.GetType("DSPEmulatorUI.IEffect");

            var effectsTypes = asm.DefinedTypes.Where(x => x.IsClass && x.ImplementedInterfaces.Contains(typeIEffect)).Select(x => x.AsType());

            foreach (var effectType in effectsTypes)
            {
                object effectObj = Activator.CreateInstance(effectType);

                effectsDict.Add((effectObj as IEffect).EffectDisplayName, effectType);
            }
        }

        public IEffect CreateEffect(string effectName)
        {
            if (!effectsDict.ContainsKey(effectName))
            {
                return null;
            }

            return (IEffect)Activator.CreateInstance(effectsDict[effectName]);
        }
        public IEffect DeserializeEffect(JToken jsonToken)
        {
            string type = jsonToken["EffectType"].Value<string>();

            foreach(Type effectType in effectsDict.Values)
            {
                if (effectType.Name.Contains(type))
                {
                    return (IEffect)Activator.CreateInstance(effectType, jsonToken);
                }
            }

            throw new Exception($"Can't deserialize effect type: {type}.");
        }
    }
}
