using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Lync.Utilities;
using Microsoft.Lync.Model;

namespace BetterBlync
{
    public class LyncStatus
    {
        private LyncClient lyncClient;

        public LyncStatus()
        {
            getClient();
        }

        public ContactAvailability LyncAvailability { get; private set; }
        public bool InACall { get; private set; }

        private void getClient()
        {
            lyncClient = LyncClient.GetClient();
            if ( lyncClient != null && lyncClient.State == ClientState.SignedIn )
            {
                GetStatus();
            }
            else
            {
                //TODO throw custom exception
            }
        }

        public ContactAvailability GetStatus()
        {
            // Get the contact information for the current signed in user
            Contact contact = lyncClient.Self.Contact;

            // Availability and activity must both be used or activity by itself, but it returns a string rather than an enum like the latter
            // Get the current activity string from the client
            string activity = (string) contact.GetContactInformation( ContactInformationType.Activity );
            if ( activity.Equals("In a call") )
            {
                InACall = true;
            }
            //else if(activity.Equals("In a meeting"))
            //{
            //    // Set status as busy and return immediately
            //    LyncAvailability = ContactAvailability.Busy;
            //    return LyncAvailability;
            //}
            else
            {
                InACall = false;
            }

            // Get the current prescence enum from the client
            var prescence =  contact.GetContactInformation( ContactInformationType.Availability );
            LyncAvailability = (ContactAvailability)Enum.Parse( typeof( ContactAvailability ), prescence.ToString() );

            return LyncAvailability;
        }
    }
}
