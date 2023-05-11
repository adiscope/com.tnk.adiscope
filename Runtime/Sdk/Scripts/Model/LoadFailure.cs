/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using System;

namespace Adiscope.Model
{
    /// <summary>
    /// failure information for loading ad
    /// </summary>
    public class LoadFailure : EventArgs
    {
        /// <summary>
        /// unit id of ad
        /// </summary>
        public string UnitId { get; private set; }

        /// <summary>
        /// adiscope error for load failure
        /// </summary>
        public AdiscopeError Error { get; private set; }

        /// <summary>
        /// constructor for load failure
        /// </summary>
        /// <param name="error">adiscope error</param>
        public LoadFailure(string unitId, AdiscopeError error)
        {
            this.UnitId = unitId;
            this.Error = error;
        }

        public override string ToString()
        {
            return
                "LoadFailure{" +
                "Error=\"" + this.Error + "\"" +
                "}";
        }
    }
}
