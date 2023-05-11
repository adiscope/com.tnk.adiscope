/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */
using System;

namespace Adiscope.Model
{
    /// <summary>
    /// rewarded item information when ad is rewarded
    /// </summary>
    public class RewardItem : EventArgs
    {
        /// <summary>
        /// unit id of the ad which is showed and rewarded
        /// </summary>
        public string UnitId { get; private set; }

        /// <summary>
        /// type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// amount
        /// </summary>
        public long Amount { get; private set; }

        /// <summary>
        /// constructor for reward info
        /// </summary>
        /// <param name="unitId">unit id of ad requested</param>
        /// <param name="type">reward type</param>
        /// <param name="amount">reward amount</param>
        public RewardItem(string unitId, string type, long amount)
        {
            this.UnitId = unitId;
            this.Type = type;
            this.Amount = amount;
        }

        public override string ToString()
        {
            return
                "RewardItem{" +
                "UnitId=\"" + this.UnitId + "\"" +
                ", Type=\"" + this.Type + "\"" +
                ", Amount=\"" + this.Amount + "\"" +
                "}";
        }
    }
}
