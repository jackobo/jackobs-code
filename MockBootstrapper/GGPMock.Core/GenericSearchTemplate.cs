using System.Collections.Generic;

namespace GGPMockBootstrapper
{
    public class GenericSearchTemplate<T>
    {
        public static List<T> GetMatchingElements(List<T> inputList, List<string> propertyNames, string template)
        {
            var outputList = new List<T>();
            foreach (var element in inputList)
            {
                var concatenatedValues = string.Empty;
                foreach (var propertyName in propertyNames)
                {
                    concatenatedValues += element.GetType().GetProperty(propertyName).GetValue(element, null).ToString();
                }

                if (concatenatedValues.Contains(template))
                {
                    outputList.Add(element);
                }
            }
            
            return outputList;
        }
    }
}