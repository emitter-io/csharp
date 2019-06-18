/*
Copyright (c) 2016 Roman Atachiants
Copyright (c) 2013, 2014 Paolo Patierno

All rights reserved. This program and the accompanying materials
are made available under the terms of the Eclipse Public License v1.0
and Eclipse Distribution License v1.0 which accompany this distribution. 

The Eclipse Public License:  http://www.eclipse.org/legal/epl-v10.html
The Eclipse Distribution License: http://www.eclipse.org/org/documents/edl-v10.php.

Contributors:
   Paolo Patierno - initial API and implementation and/or initial documentation
   Roman Atachiants - integrating with emitter.io
*/


using System;

namespace Emitter
{

    /// <summary>
    /// Represents a set of event codes used for emitter.
    /// </summary>
    public enum EmitterEventCode : long
    {
        Message = 100,
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        ServerError = 500,
        NotImplemented = 501,
    }


    /// <summary>
    /// Represents an error response.
    /// </summary>
    internal sealed class EmitterException: Exception
    {
        /// <summary>
        /// Constructs a new error.
        /// </summary>
        /// <param name="status">The status code for the error.</param>
        /// <param name="message">The message for the error.</param>
        public EmitterException(EmitterEventCode status, string message) : base(message)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public readonly EmitterEventCode Status;

        public static readonly EmitterException NotImplemented = new EmitterException(EmitterEventCode.NotImplemented, "The server either does not recognize the request method, or it lacks the ability to fulfill the request.");
        public static readonly EmitterException BadRequest = new EmitterException(EmitterEventCode.BadRequest, "The request was invalid or cannot be otherwise served.");
        public static readonly EmitterException NoDefaultKey = new EmitterException(EmitterEventCode.BadRequest, "The default key was not provided. Either provide a default key in the constructor or specify a key for the operation.");
        public static readonly EmitterException Unauthorized = new EmitterException(EmitterEventCode.Unauthorized, "The security key provided is not authorized to perform this operation.");
        public static readonly EmitterException PaymentRequired = new EmitterException(EmitterEventCode.PaymentRequired, "The request can not be served, as the payment is required to proceed.");
        public static readonly EmitterException Forbidden = new EmitterException(EmitterEventCode.Forbidden, "The request is understood, but it has been refused or access is not allowed.");
        public static readonly EmitterException NotFound = new EmitterException(EmitterEventCode.NotFound, "The resource requested does not exist.");
        public static readonly EmitterException ServerError = new EmitterException(EmitterEventCode.ServerError, "An unexpected condition was encountered and no more specific message is suitable.");

        public static EmitterException FromStatus(int status)
        {
            switch(status)
            {
                case 200: return null;
                case 400: return BadRequest;
                case 401: return Unauthorized;
                case 402: return PaymentRequired;
                case 403: return Forbidden;
                case 404: return NotFound;
                case 500: return ServerError;
                case 501: return NotImplemented;
                default: return new EmitterException(EmitterEventCode.NotImplemented, "Unknown status received. Code: " + status + ".");
            }
        }
    }
}
