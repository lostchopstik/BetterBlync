﻿using Microsoft.Lync.Model;
using System;

namespace BetterBlync
{
    public class LyncStatus : IDisposable
    {
        private LyncClient lyncClient = LyncClient.GetClient();
        private bool callNotify = false;
        private string activity;
        private Contact contact;
        private object prescence;
        public ContactAvailability LyncAvailability { get; private set; }
        public bool InACall { get; private set; }
        public bool NewMessage { get; set; }

        public LyncStatus()
        {
            getClient();
        }

        private void getClient()
        {
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
            {
                InACall = true;
                callNotify = true;
            }
            else if ( e.NewState == Microsoft.Lync.Model.Conversation.ModalityState.Disconnected )
            {
                InACall = false;
                callNotify = false;
            }
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
            contact = lyncClient.Self.Contact;

            // Availability and activity must both be used or activity by itself, but it returns a string rather than an enum like the latter
            // Get the current activity string from the client
            activity = (string)contact.GetContactInformation( ContactInformationType.Activity );

            // Check the activity string to see if user is currently in a call, and check that a call isnt currently incoming
            if ( activity.Equals( "In a call" ) || callNotify )
                InACall = true;
            else
                InACall = false;

            // Get the current prescence enum from the client
            prescence = contact.GetContactInformation( ContactInformationType.Availability );
            LyncAvailability = (ContactAvailability)Enum.Parse( typeof( ContactAvailability ), prescence.ToString() );
            prescence = null;

            return LyncAvailability;
        }

        public void Dispose()
        {
            lyncClient = null;
            activity = string.Empty;
            contact = null;
            prescence = null;
        }
    }
}