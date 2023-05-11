/*
 * Created by nmj
 */
using System;

namespace Adiscope.Model.Cross
{
    /// <summary>
    /// banner information for closing cross promotion banner
    /// </summary>
    public class BannerCloseResult : EventArgs
    {
        /// <summary>
        /// banner type. 1 : MoreGamesBanner, 2 : FullScreenBanner, 3 : EndingBanner
        /// </summary>
        public int BannerType { get; private set; }

        /// <summary>
        /// flag. 1 when user click termination button of ending banner, otherwise 0
        /// </summary>
        public int Flag { get; private set; }
        /// <summary>
        /// constructor for cross promotion banner result
        /// </summary>
        /// <param name="bannerType">banner type</param>
        /// <param name="flag">flag</param>
        public BannerCloseResult(int bannerType, int flag)
        {
            this.BannerType = bannerType;
            this.Flag = flag;
        }

        public override string ToString()
        {
            return
                "BannerCloseResult{" +
                "BannerType=\"" + this.BannerType + "\"" +
                "Flag=\"" + this.Flag + "\"" +
                "}";
        }
    }
}
