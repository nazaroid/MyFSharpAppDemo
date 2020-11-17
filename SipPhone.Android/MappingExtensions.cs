using Org.Linphone.Core;
using SipPhone.Core.Models;

namespace SipPhone.Android
{
    static class MappingExtensions
    {
        public static RegistrationStatus ToRegistrationStatus(this LinphoneCoreRegistrationState state)
        {
            if (LinphoneCoreRegistrationState.RegistrationOk.Equals(state))
            {
                return RegistrationStatus.Online;
            }
            else if (LinphoneCoreRegistrationState.RegistrationFailed.Equals(state))
            {
                return RegistrationStatus.Registering;
            }
            else if (LinphoneCoreRegistrationState.RegistrationProgress.Equals(state))
            {
                return RegistrationStatus.Error;
            }

            return RegistrationStatus.Offline;
        }
    }
}