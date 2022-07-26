// <copyright file="AsyncConstants.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Api.Constants
{
    /// <summary>
    /// Constants for asynchronous operations.
    /// </summary>
    public class AsyncConstants
    {
        /// <summary>
        /// Time in milliseconds before an asynchronous send operation times out.
        /// If no response is received before the timeout, the operation will be cancelled.
        /// </summary>
        public const int AsyncSendTimeout = 5000;

        /// <summary>
        /// Time in milliseconds before an asynchronous protocol handler operation times out.
        /// </summary>
        public const int AsyncHandleTimeout = 5000;
    }
}