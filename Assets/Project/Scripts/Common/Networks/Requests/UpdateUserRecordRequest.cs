using Treevel.Common.Entities;
using Treevel.Common.Utils;

namespace Treevel.Common.Networks.Requests
{
    public class UpdateUserRecordRequest : UpdateServerRequestBase<UserRecord>
    {
        public UpdateUserRecordRequest(UserRecord data)
        {
            key = Constants.PlayFabKeys.USER_RECORD;
            this.data = data;
        }
    }
}
