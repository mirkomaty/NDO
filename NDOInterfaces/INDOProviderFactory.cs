//
// Copyright (c) 2002-2024 Mirko Matytschak 
// (www.netdataobjects.de)
//
// Author: Mirko Matytschak
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
// Software, and to permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:

using System.Collections.Generic;

namespace NDOInterfaces
{
    /// <summary>
    /// Interface to implementations which can get IProvider and ISqlGenerator instances
    /// </summary>
    public interface INDOProviderFactory
    {
        /// <summary>
        /// Gets a provider with the given name or null
        /// </summary>
        /// <param name="name">The name of the provider</param>
        /// <returns></returns>
        IProvider this[string name] { get; set; }

        /// <summary>
        /// Get all generators
        /// </summary>
        IDictionary<string, ISqlGenerator> Generators { get; }

        /// <summary>
        /// Gets all provider names for use in UI.
        /// </summary>
        string[] ProviderNames { get; }

        /// <summary>
        /// Gets a provider with the givenname or, if it doesn't exist, the first provider available
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IProvider GetProviderOrDefault( string name );
    }
}