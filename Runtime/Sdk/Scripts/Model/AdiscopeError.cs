/*
 * Created by Hyunchang Kim (martin.kim@neowiz.com)
 */

namespace Adiscope.Model
{
    /// <summary>
    /// error information with code and description
    /// </summary>
    public class AdiscopeError
    {
        public enum ErrorCode
        {
            UNKNOWN_ERROR           = -1,
            INTERNAL_ERROR          = 0,
            MEDIATION_ERROR         = 1,
            INITIALIZE_ERROR        = 2,
            SERVER_SETTING_ERROR    = 3,
            INVALID_REQUEST         = 4,
            NETWORK_ERROR           = 5,
            NO_FILL                 = 6,
            TIME_LIMIT              = 7,
            NOT_EXIST_IDFA          = 8,
            GOOGLE_FAMILY_ERROR     = 9,
            INVALID_ADID            = 10,
            TIME_OUT                = 11,
            SHOW_CALLED_BEFORE_LOAD = 12,
            ADID_IS_NOT_AVAILABLE   = 13
        };

        public ErrorCode Code { get; private set; }
        public string Description { get; private set; }
        public string XB3TraceID { get; private set; }

        public AdiscopeError(int code, string description) {
            updateData(code, description, "");
        }

        public AdiscopeError(int code, string description, string XB3TraceID)
        {
            updateData(code, description, XB3TraceID);
        }

        private void updateData(int code, string description, string XB3TraceID)
        {
            if (false == System.Enum.IsDefined(typeof(ErrorCode), code))
            {
                this.Code = ErrorCode.UNKNOWN_ERROR;
            }
            else
            {
                this.Code = (ErrorCode)code;
            }

            this.Description = description ?? string.Empty;
            this.XB3TraceID = XB3TraceID ?? string.Empty;
        }

        public override string ToString()
        {
            return
                "AdiscopeError {" +
                "Code=\"" + this.Code + "\"" +
                ", Description=\"" + this.Description + "\"" +
                ", XB3TraceID=\"" + this.XB3TraceID + "\"" +
                "}";
        }
    }
}
