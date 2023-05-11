/*
 * Created by nmj
 */
using System;

namespace Adiscope.Model.Cross
{
    /// <summary>
    /// failure information for showing cross promotion banner
    /// </summary>
    public class BannerFailure : EventArgs
    {
        /// <summary>
        /// banner type. 1 : MoreGamesBanner, 2 : FullScreenBanner, 3 : EndingBanner
        /// </summary>
        public int BannerType { get; private set; }

        /// <summary>
        /// adiscope error for load failure
        /// </summary>
        public AdiscopeError Error { get; private set; }

        /// <summary>
        /// constructor for cross promotion banner result
        /// </summary>
        /// <param name="bannerType">banner type</param>
        public BannerFailure(int bannerType, AdiscopeError error)
        {
            this.BannerType = bannerType;
            this.Error = error;
        }

        public override string ToString()
        {
            return
                "BannerFailure{" +
                "BannerType=\"" + this.BannerType + "\"" +
                "Error=\"" + this.Error + "\"" +
                "}";
        }
    }
}
