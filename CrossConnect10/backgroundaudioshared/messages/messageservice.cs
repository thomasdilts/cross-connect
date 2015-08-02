// <copyright file="MessageService.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;

namespace BackgroundAudioShared.Messages
{
    /// <summary>
    /// MessageService makes it easy to send strongly typed messages
    /// between the foreground and background processes.
    /// </summary>
    /// <remarks>
    /// JSON is used as the underlying serialization mechanism,
    /// but you don't need to know JSON formatting to create new
    /// messages.
    /// 
    /// See some of the related Message implementations which are
    /// simple data objects serialized through the standard DataContract
    /// interface.
    /// </remarks>
    public static class MessageService
    {
        // The underlying BMP methods can pass a ValueSet. MessageService
        // relies on this to pass a type and body payload.
        const string MessageType = "MessageType";
        const string MessageBody = "MessageBody";

        public static void SendMessageToForeground<T>(T message)
        {
            var payload = new ValueSet();
            payload.Add(MessageService.MessageType, typeof(T).FullName);
            payload.Add(MessageService.MessageBody, JsonHelper.ToJson(message));
            BackgroundMediaPlayer.SendMessageToForeground(payload);
        }
    
        public static void SendMessageToBackground<T>(T message)
        {
            // this will crash if the background is too busy to receive the message.
            try
            {
                var payload = new ValueSet();
                payload.Add(MessageService.MessageType, typeof(T).FullName);
                payload.Add(MessageService.MessageBody, JsonHelper.ToJson(message));
                BackgroundMediaPlayer.SendMessageToBackground(payload);
            }
            catch (Exception)
            {
            }
        }

        public static bool TryParseMessage<T>(ValueSet valueSet, out T message)
        {
            object messageTypeValue;
            object messageBodyValue;

            message = default(T);

            // Get message payload
            if (valueSet.TryGetValue(MessageService.MessageType, out messageTypeValue)
                && valueSet.TryGetValue(MessageService.MessageBody, out messageBodyValue))
            {
                // Validate type
                if ((string)messageTypeValue != typeof(T).FullName)
                {
                    //Debug.WriteLine("Message type was {0} but expected type was {1}", (string)messageTypeValue, typeof(T).FullName);
                    return false;
                }
                Debug.WriteLine("Message received={0}", typeof(T).Name);
                message = JsonHelper.FromJson<T>(messageBodyValue.ToString());
                return true;
            }

            return false;
        }
    }
}
