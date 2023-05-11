/*
 * Created by moonjin Noh
 */
namespace Adiscope.Model
{
    /// <summary>
    /// unit status information
    /// </summary>
    public class UnitStatus
    {
        /// <summary>
        /// monetization of unit's offerwall or unit's rewarded video
        /// </summary>
        public bool Live { get; private set; }

        /// <summary>
        /// activation of unit
        /// </summary>
        public bool Active { get; private set; }

        public UnitStatus(bool live, bool active)
        {
            this.Live = live;
            this.Active = active;
        }

        public override string ToString()
        {
            return
                "UnitStatus{" +
                "Live=\"" + this.Live + "\"" +
                ", Active=\"" + this.Active + "\"" +
                "}";
        }
    }
}
