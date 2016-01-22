using Microsoft.Lync.Model;
using System;

namespace BetterBlync
{
    public class LyncStatus
    {
        private LyncClient lyncClient;
        public ContactAvailability LyncAvailability { get; private set; }
        public bool InACall { get; private set; }
        public bool NewMessage { get; set; }

        public LyncStatus()
        {
            getClient();
        }

        private void getClient()
        {
            lyncClient = LyncClient.GetClient();
            if ( lyncClient != null && lyncClient.State == ClientState.SignedIn )
            {
                GetStatus();
                lyncClient.ConversationManager.ConversationAdded += ConversationManager_ConversationAdded;
            }
            else
                // TODO throw custom exception
                throw new Exception( "Lync client not found." );
        }

        private void ConversationManager_ConversationAdded(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        {
            e.Conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.InstantMessage].ModalityStateChanged += LyncStatus_IMStatus;
            e.Conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.AudioVideo].ModalityStateChanged += LyncStatus_AVStatus;
        }

        private void LyncStatus_AVStatus(object sender, Microsoft.Lync.Model.Conversation.ModalityStateChangedEventArgs e)
        {
            if ( e.NewState == Microsoft.Lync.Model.Conversation.ModalityState.Notified )
                InACall = true;
            else if ( e.NewState == Microsoft.Lync.Model.Conversation.ModalityState.Disconnected )
                InACall = false;
        }

        private void LyncStatus_IMStatus(object sender, Microsoft.Lync.Model.Conversation.ModalityStateChangedEventArgs e)
        {
            if ( e.NewState == Microsoft.Lync.Model.Conversation.ModalityState.Notified )
                NewMessage = true;
            else
                NewMessage = false;
        }

        /// <summary>
        /// Reads the current status/availability of the signed in user on Lync/Skype for Bus.
        /// </summary>
        /// <returns>ContactAvailability</returns>
        public ContactAvailability GetStatus()
        {
            // Get the contact information for the current signed in user
            Contact contact = lyncClient.Self.Contact;

            // Availability and activity must both be used or activity by itself, but it returns a string rather than an enum like the latter
            // Get the current activity string from the client
            string activity = (string)contact.GetContactInformation( ContactInformationType.Activity );

            if ( activity.Equals( "In a call" ) )
                InACall = true;
            else
                InACall = false;

            // Get the current prescence enum from the client
            var prescence = contact.GetContactInformation( ContactInformationType.Availability );
            LyncAvailability = (ContactAvailability)Enum.Parse( typeof( ContactAvailability ), prescence.ToString() );

            return LyncAvailability;
        }
    }
}