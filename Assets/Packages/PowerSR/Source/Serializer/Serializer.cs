using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerSR
{
    #region Serializer Class XML
    /// <summary>
    /// The class used for serialization.
    /// </summary>
    #endregion
    [Serializable]
    public static class Serializer
    {
        #region NewlineOperatorRegex String XML
        /// <summary>
        /// The regex pattern used to detect the <see cref="NewlineOperator">newline operator</see>, goes hand in hand with it in the serializer implementation.
        /// </summary>
        #endregion
        public const string NewlineOperatorRegex = @"\${Newline(\d+)?}";

        #region NewlineOperator String XML
        /// <summary>
        /// The raw newline operator, used by <see cref="SerializerUtilities.ComposeNewlineOperator">the compose function</see>. To get an actual newline operator string, compose one with the aforementioned function.
        /// </summary>
        #endregion
        public const string NewlineOperator = "${Newline~Index~}";

        #region AssignOperator String XML
        /// <summary>
        /// Used to parse the value of properties.
        /// </summary>
        #endregion
        public const string AssignOperator = " = ";


        // Private / Inaccessible variables..
        private static readonly Regex NewlineRegex = new Regex(NewlineOperatorRegex, RegexOptions.Compiled);



        #region Set Method XML
        /// <summary>
        /// Serializes a property.
        /// </summary>
        /// <param name="SerializedString">The pre-existing serialized string to modify / add the serialized property to.</param>
        /// <param name="Identifier">The name of the property.</param>
        /// <param name="Value">The value of the property.</param>
        /// <returns>All the serialized properties (the pre-existing ones + the one that was just serialized) as a string.</returns>
        #endregion
        public static string Set(this string SerializedString, string Identifier, Object Value)
        {
            SerializedString = ((SerializedString != null) ? SerializedString : String.Empty);
            Identifier = ((Identifier != null) ? Identifier : String.Empty);
            Value = ((Value != null) ? Value : String.Empty);


            Value = NewlineRegex.Replace(Value.ToString(), new MatchEvaluator((Match) => {
                int CurrentNumber = ((Match.Groups[1].Success) ? int.Parse(Match.Groups[1].Value) : 0);
                int NewNumber = CurrentNumber + 1;
                return SerializerUtilities.ComposeNewlineOperator(NewNumber);
            }));


            List<string> ExistingProperties = SerializedString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int I = 0; I < ExistingProperties.Count; I++)
            {
                if (ExistingProperties[I].StartsWith($"{Identifier.Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())}{AssignOperator}"))
                {
                    ExistingProperties[I] = $"{Identifier.Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())}{AssignOperator}{Value.ToString().Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())}";
                    return String.Join(Environment.NewLine, ExistingProperties);
                }
            }

            if (ExistingProperties.Count > 0 && (String.IsNullOrEmpty(ExistingProperties[0]) || String.IsNullOrWhiteSpace(ExistingProperties[0]))) ExistingProperties.RemoveAt(0);
            ExistingProperties.Add($"{Identifier.Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())}{AssignOperator}{Value.ToString().Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())}");
            return String.Join(Environment.NewLine, ExistingProperties);
        }

        #region Get Method XML
        /// <summary>
        /// Gets the value of a serialized property.
        /// </summary>
        /// <param name="SerializedString">The serialized string that contains the property.</param>
        /// <param name="Identifier">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        #endregion
        public static Object Get(this string SerializedString, string Identifier)
        {
            SerializedString = ((SerializedString != null) ? SerializedString : String.Empty);
            Identifier = ((Identifier != null) ? Identifier : String.Empty);

            List<string> Properties = SerializedString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int I = 0; I < Properties.Count; I++)
            {
                if (Properties[I].StartsWith(Identifier.Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())))
                {
                    string Return = Properties[I].Remove(0, ($"{Identifier.Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())}{AssignOperator}").Length).Replace(SerializerUtilities.ComposeNewlineOperator(), Environment.NewLine);
                    Return = NewlineRegex.Replace(Return.ToString(), new MatchEvaluator((Match) => {
                        int CurrentNumber = ((Match.Groups[1].Success) ? int.Parse(Match.Groups[1].Value) : 0);
                        int NewNumber = CurrentNumber - 1;
                        return ((CurrentNumber > 0) ? SerializerUtilities.ComposeNewlineOperator(NewNumber) : Match.Value);
                    }));

                    return Return;
                }
            }

            return null;
        }


        #region Delete Method XML
        /// <summary>
        /// Deletes a property from the serialized string.
        /// </summary>
        /// <param name="Identifier">The name of the property.</param>
        /// <param name="SerializedString">The serialized string to remove the property from.</param>
        /// <returns>The serialized string.</returns>
        #endregion
        public static string Delete(this string SerializedString, string Identifier)
        {
            SerializedString = ((SerializedString != null) ? SerializedString : String.Empty);
            Identifier = ((Identifier != null) ? Identifier : String.Empty);

            List<string> Properties = SerializedString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int I = 0; I < Properties.Count; I++)
            {
                if (Properties[I].StartsWith(Identifier.Replace(Environment.NewLine, SerializerUtilities.ComposeNewlineOperator())))
                {
                    Properties.RemoveAt(I);
                    break;
                }
            }

            return String.Join(Environment.NewLine, Properties);
        }
    }
}