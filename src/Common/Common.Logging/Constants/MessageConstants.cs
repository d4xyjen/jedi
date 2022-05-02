// <copyright file="MessageConstants.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
namespace Jedi.Common.Logging.Constants
{
    /// <summary>
    /// Event message constants.
    /// </summary>
    public class MessageConstants
    {
        /// <summary>
        /// Value used for null parameters.
        /// </summary>
        public const string NullParameterValue = "NULL";

        /// <summary>
        /// The regex used to match parameters in event messages.
        /// </summary>
        public const string ParameterRegexPattern = @"\{(\w*?)\}";
    }
}