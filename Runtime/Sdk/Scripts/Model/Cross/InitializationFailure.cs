/*
 * Created by nmj
 */
using System;

namespace Adiscope.Model.Cross
{
    /// <summary>
    /// failure information for cross promotion failure
    /// </summary>
    public class InitializationFailure : EventArgs
    {
        /// <summary>
        /// cross promotion error for Initialization failure
        /// </summary>
        public AdiscopeError Error { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="error">error</param>
        public InitializationFailure(AdiscopeError error)
        {
            this.Error = error;
        }

        public override string ToString()
        {
            return
                "InitializationFailure{" +
                "Error=\"" + this.Error + "\"" +
                "}";
        }
    }
}
