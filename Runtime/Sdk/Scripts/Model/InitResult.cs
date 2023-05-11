/*
 * Created by mjgu (mjgu@neowiz.com)
 */
using System;

namespace Adiscope.Model
{
    /// <summary>
    /// success information for showing ad
    /// </summary>
    public class LoadResult : EventArgs
    {
        /// <summary>
        /// unit id of ad shown
        /// </summary>
        public string UnitId { get; private set; }

        /// <summary>
        /// constructor for show success result
        /// </summary>
        /// <param name="unitId">unit id of ad shown</param>
        public LoadResult(string unitId)
        {
            this.UnitId = unitId;
        }

        public override string ToString()
        {
            return
                "LoadResult{" +
                "UnitId=\"" + this.UnitId + "\"" +
                "}";
        }
    }
}
