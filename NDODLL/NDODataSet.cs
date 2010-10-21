//
// Copyright (C) 2002-2008 HoT - House of Tools Development GmbH 
// (www.netdataobjects.com)
//
// Author: Mirko Matytschak
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License (v3) as published by
// the Free Software Foundation.
//
// If you distribute copies of this program, whether gratis or for 
// a fee, you must pass on to the recipients the same freedoms that 
// you received.
//
// Commercial Licence:
// For those, who want to develop software with help of this program 
// and need to distribute their work with a more restrictive licence, 
// there is a commercial licence available at www.netdataobjects.com.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.IO;
using System.Reflection;
using NDO.Mapping;
using NDO;

namespace NDO
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
        public NDODataSet(String path)
		{
			this.ReadXmlSchema(path);
			this.EnforceConstraints = false;
		}

		/// <summary>
		/// Creates a NDODataSet object from a mapping file. 
        /// Note: The mapping file must be initialized, all assemblies of all mapped types must be loadable. 
        /// This constructor works with NDOMapping objects returned by the PersistenceManager.NDOMapping property.
		/// </summary>
        /// <param name="mapping">A NDOMapping object.</param>
        public NDODataSet(NDOMapping mapping)
		{
            Remap(mapping);
		}

        /// <summary>
        /// Used by the NDO Enhancer. Makes sure, that all classes of a certain mapping file 
        /// are represented in the schema.
        /// </summary>
        /// <param name="mapping">A NDOMapping object.</param>
        public void Remap(NDOMapping mapping)
        {
            foreach (Class cl in mapping.Classes)
            {
                if (cl.IsAbstract)
                    continue;
                new SchemaGenerator(cl, mapping, this).GenerateTables();
            }
            foreach (Class cl in mapping.Classes)
            {
                if (cl.IsAbstract)
                    continue;
                new SchemaGenerator(cl, mapping, this).GenerateRelations();
            }
            this.EnforceConstraints = false;
        }


		#region ICloneable Member

		public new object Clone()
		{
			return base.Clone();
		}

		#endregion
	}
}
