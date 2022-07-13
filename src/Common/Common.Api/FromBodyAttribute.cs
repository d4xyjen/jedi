// <copyright file="FromDataAttribute.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>

namespace Jedi.Common.Api
{
    /// <summary>
    /// Method parameters with this attribute will be deserialized from
    /// the message data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromBodyAttribute : Attribute
    {
    }
}
