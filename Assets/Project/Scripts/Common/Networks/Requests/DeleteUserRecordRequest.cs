using Treevel.Common.Utils;

namespace Treevel.Common.Networks.Requests
{
    public class DeleteUserRecordRequest : DeleteServerRequestBase
    {
        public DeleteUserRecordRequest()
        {
            key = Constants.PlayerPrefsKeys.USER_RECORD;
        }
    }
}
