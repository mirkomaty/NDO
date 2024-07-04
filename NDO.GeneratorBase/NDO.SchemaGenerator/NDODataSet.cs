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

// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.


using System;
using NDO.Mapping;
using NDOInterfaces;

namespace NDO.SchemaGenerator
{
    /// <summary>
    /// This class is used by the NDO Enhancer.
    /// </summary>
    public class NDODataSet : System.Data.DataSet, ICloneable
    {
        /// <summary>
        /// Constructs an uninitialized NDODataSet object. Needed by Clone() and the Enhancer.
        /// </summary>
        public NDODataSet() : base()
        {
        }

        /// <summary>
        /// Creates a NDODataSet object from an Xml Schema.
        /// </summary>
        /// <param name="path"></param>
        public NDODataSet( String path )
        {
            this.ReadXmlSchema( path );
            this.EnforceConstraints = false;
        }

        /// <summary>
        /// Creates a NDODataSet object from a mapping file. 
        /// Note: The mapping file must be initialized, all assemblies of all mapped types must be loadable. 
        /// This constructor works with NDOMapping objects returned by the PersistenceManager.NDOMapping property.
        /// </summary>
        /// <param name="mapping">A NDOMapping object.</param>
        /// <param name="providerFactory">The provider factory</param>
        public NDODataSet( NDOMapping mapping, INDOProviderFactory providerFactory )
        {
            Remap( mapping, providerFactory );
        }

        /// <summary>
        /// Used by the NDO Enhancer. Makes sure, that all classes of a certain mapping file 
        /// are represented in the schema.
        /// </summary>
        /// <param name="mapping">A NDOMapping object.</param>
        /// <param name="providerFactory">The provider factory</param>
        internal void Remap( NDOMapping mapping, INDOProviderFactory providerFactory )
        {
            foreach (Class cl in mapping.Classes)
            {
                if (cl.IsAbstract)
                    continue;
                new SchemaGenerator( providerFactory, cl, mapping, this ).GenerateTables();
            }
            foreach (Class cl in mapping.Classes)
            {
                if (cl.IsAbstract)
                    continue;
                new SchemaGenerator( providerFactory, cl, mapping, this ).GenerateRelations();
            }
            this.EnforceConstraints = false;
        }


        #region ICloneable Member

        ///<inheritdoc/>
        public new object Clone()
        {
            return base.Clone();
        }

        #endregion
    }
}
