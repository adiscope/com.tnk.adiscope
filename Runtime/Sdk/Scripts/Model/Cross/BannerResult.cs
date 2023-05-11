/*
 * Created by nmj
 */
using System;

namespace Adiscope.Model.Cross
{
    /// <summary>
    /// banner information
    /// </summary>
    public class BannerResult : EventArgs
    {
        /// <summary>
        /// banner type. 1 : MoreGamesBanner, 2 : FullScreenBanner, 3 : EndingBanner
        /// </summary>
        public int BannerType { get; private set; }

        /// <summary>
        /// constructor for cross promotion banner result
        /// </summary>
        /// <param name="bannerType">banner type</param>
        public BannerResult(int bannerType)
        {
            this.BannerType = bannerType;
        }

        public override string ToString()
        {
            return
                "BannerResult{" +
                "BannerType=\"" + this.BannerType + "\"" +
                "}";
        }
    }
}
