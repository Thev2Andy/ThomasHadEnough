using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSR
{
    #region SerializerUtilities Class XML
    /// <summary>
    /// Helper class containing methods used by the <see cref="Serializer">Serializer</see>.
    /// </summary>
    #endregion
    public static class SerializerUtilities
    {
        #region ComposeNewlineOperator Method XML
        /// <summary>
        /// Composes a newline operator with the specified index.
        /// </summary>
        #endregion
        public static string ComposeNewlineOperator(uint Index = 0) {
            return Serializer.NewlineOperator.Replace("~Index~", ((Index > 0) ? Index.ToString() : String.Empty));
        }
    }
}
