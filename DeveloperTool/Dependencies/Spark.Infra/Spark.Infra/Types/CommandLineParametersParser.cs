using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Exceptions;

namespace Spark.Infra.Types
{
    public class CommandLineParametersParser
    {
        public CommandLineParametersParser(string[] parameters, string usageInfo)
        {
            _cliParams = StringKeyValueCollection.Parse(parameters);
            _usageInfo = usageInfo;
        }

        string _usageInfo;
        StringKeyValueCollection _cliParams;
       
        public TValue GetMandatoryParameterValue<TValue>(string parameterName)
        {
            var prop = _cliParams.TryGetProperty(parameterName);
            if (!prop.Any())
                throw new InvalidCommandLineArgumnentsException($"Missing {parameterName} parameter!", _usageInfo);
            
            try
            {
                if (typeof(TValue).IsEnum)
                    return ParseEnum<TValue>(parameterName, prop.First().Value);

                var converter = TypeDescriptor.GetConverter(typeof(TValue));
                return (TValue)converter.ConvertFromString(prop.First().Value);
            }
            catch(Exception ex)
            {
                throw new InvalidCommandLineArgumnentsException($"Failed to parse {parameterName} parameter! {ex.Message}", _usageInfo, ex);
            }
        }

        private TValue ParseEnum<TValue>(string parameterName, string value)
        {
            try
            {
                return (TValue)Enum.Parse(typeof(TValue), value, true);
            }
            catch(Exception ex)
            {
                var acceptedValues = string.Join(" | ", Enum.GetNames(typeof(TValue)));
                throw new InvalidCommandLineArgumnentsException($"Invalid value '{value}' for {parameterName} parameter! Accepted values are: {acceptedValues}", _usageInfo, ex);
            }
            
            
        }

        public Optional<TValue> GetOptionalParameterValue<TValue>(string parameterName)
        {
            var prop = _cliParams.TryGetProperty(parameterName);
            if (!prop.Any())
                return Optional<TValue>.None();

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(TValue));
                return Optional<TValue>.Some((TValue)converter.ConvertFromString(prop.First().Value));
            }
            catch (Exception ex)
            {
                throw new InvalidCommandLineArgumnentsException($"Failed to parse {parameterName} parameter! {ex.Message}", _usageInfo, ex);
            }
        }
    }
}
