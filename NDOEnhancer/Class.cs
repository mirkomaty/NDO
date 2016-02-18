//
// Copyright (C) 2002-2009 Mirko Matytschak 
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
// there is a commercial licence available at www.netdataobjects.de.
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
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Data;

using ILCode;
using NDO;

namespace NDOEnhancer.Patcher
{
	/// <summary>
	/// Summary description for Class.
	/// </summary>
	internal class ClassPatcher
	{
		static ClassPatcher()
		{
			indTypes = new Hashtable(12);
			indTypes.Add("int32", "i4");
			indTypes.Add("bool", "i1");
			indTypes.Add("int8", "i1");
			indTypes.Add("unsigned int8", "u1");
			indTypes.Add("float64", "r8");
			indTypes.Add("float32", "r4");
			indTypes.Add("int16", "i2");
			indTypes.Add("int64", "i8");
			indTypes.Add("char", "u2");
			indTypes.Add("unsigned int16", "u2");
			indTypes.Add("unsigned int32", "u4");
			indTypes.Add("unsigned int64", "i8");
		}

		public ClassPatcher(	ILClassElement classElement, 
			NDO.Mapping.NDOMapping mappings, 
			ClassHashtable externalPersistentBases,
			NDOEnhancer.MessageAdapter messages,
			IList sortedFields,
			IList references,
			string oidTypeName)
		{
			m_classElement			= classElement;
			m_name					= classElement.getClassFullName();
            m_refName               = makeRefName();
            m_nonGenericRefName     = m_name;
			m_references			= references;

			if (references == null)
				throw new ArgumentNullException( "references" );

            int p = m_nonGenericRefName.IndexOf('<');
            if (p > -1)
                m_nonGenericRefName = m_nonGenericRefName.Substring(0, p);

			m_persistentBase		= classElement.getBaseFullName();
			this.externalPersistentBases = externalPersistentBases;

			if (null != externalPersistentBases)
			{
				//m_hasPersistentBase	= classElement.hasPersistentBase(temp, typeof(NDOPersistentAttribute));
				m_hasPersistentBase	= (externalPersistentBases.Contains(m_persistentBase));
			}

			this.m_mappings				= mappings;
			this.m_classMapping			= mappings.FindClass(classElement.getMappingName());

			if (this.m_classMapping == null)
                throw new Exception("Can't find mapping for class " + classElement.getMappingName());

			this.messages			= messages;
			this.sortedFields = sortedFields;
			this.oidTypeName = oidTypeName;

			for (int i = 0; i < m_references.Count; i++)
				((ILReference) m_references[i]).Ordinal = i;


			// sortedFields ist eine flache Ansicht auf die Felder, wie 
			// sie in der Datenbank sein werden.
			// Wir benötigen aber auch die hierarchische Sicht, die die 
			// Namen der ValueTypes und embedded Objects liefert.
			ownFieldsHierarchical = getHierarchicalFieldList();

			if (null != externalPersistentBases)
			{
				checkPersistentRoot();
				checkNextPersistentBase();
			}
		}

		static private Hashtable indTypes;

		const string ldarg_0 = "ldarg.0";
		const string ldarg_1 = "ldarg.1";
		private NDOEnhancer.MessageAdapter messages;
		private ILClassElement			m_classElement;
		private string					m_name;
		private string					m_refName;
		private string					m_nonGenericRefName;
		private string					m_persistentBase;
		private bool					m_hasPersistentBase;
		private NDO.Mapping.NDOMapping	m_mappings;
		private string					oidTypeName;
		private NDO.Mapping.Class		m_classMapping;

		private ArrayList				ownFieldsHierarchical	= new ArrayList();
		private IList					m_references;
		private ArrayList				dirtyDone = new ArrayList();
		private ClassHashtable			externalPersistentBases;
		private string					persistentRoot = null;
		private IList					sortedFields;
		private int						mappedFieldCount;
		private ClassNode				myClassNode;


        private string makeRefName()
        {
            int p = m_name.IndexOf('<');
            if (p == -1)
                return m_name;
            Ecma335.EcmaGenericParameter genPar = new NDOEnhancer.Ecma335.EcmaGenericParameter();
            genPar.Parse(m_name.Substring(p));
            string newGenPars = "<";
            int count = genPar.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                string element = genPar.Elements[i];
                newGenPars += "!" + element;
                if (i < count - 1)
                    newGenPars += ',';
            }
            return "class " + m_name.Substring(0, p) + newGenPars + '>';
        }

		private void checkNextPersistentBase()
		{
			ClassNode baseClass = getPersistentBaseClassElement(MyClassNode);
			if (baseClass == null)
				return;
			while (!baseClass.IsPersistent)
			{
				baseClass = getPersistentBaseClassElement(baseClass);
				if (baseClass == null)
					throw new Exception("Internal error #126 in Class.cs");
			}
			if (baseClass.AssemblyName != this.m_classElement.getAssemblyName())
				m_persistentBase = "[" + baseClass.AssemblyName + "]" + baseClass.Name;
			else
				m_persistentBase = baseClass.Name;
		}

		private ArrayList getHierarchicalFieldList()
		{
			this.mappedFieldCount = 0;  // count of the flat fields
			ArrayList fields = new ArrayList();

			if (sortedFields != null)
			{
                // Sorted fields is an array of DictionaryEntries
				foreach (DictionaryEntry e in sortedFields)
				{
					ILField field = (ILField) e.Value;
					if (field.IsInherited)
						continue;
					this.mappedFieldCount++;
					if (field.Parent != null)
					{
						if (!fields.Contains(field.Parent))
							fields.Add(field.Parent);
					}
					else
					{
						fields.Add(field);
					}
				}
			}
			return fields;
		}


		private ClassNode MyClassNode
		{
			get
			{
				if (myClassNode == null)
				{
					ILClassElement classEl = this.m_classElement;
					string className = classEl.getMappingName();
					myClassNode = (ClassNode) externalPersistentBases[className];
					if (myClassNode == null)
					{
						throw new Exception(String.Format("Persistent class {0} must be public.", className));
					}
				}
				return myClassNode;
			}
		}


		private ClassNode getPersistentBaseClassElement(ClassNode parent)
		{
			string baseName;
			string className = parent.Name;
			baseName = parent.BaseName;
			if (null == baseName)
			{
				if (null == className)
					className = "(null)";
				throw new Exception("Persistent base class name for class " + className + " not found");
			}
			return externalPersistentBases[baseName];
		}


		private void checkPersistentRoot()
		{
			ClassNode baseClass = MyClassNode;
			while (true)
			{
				ClassNode newBaseClass = getPersistentBaseClassElement(baseClass);
				if (newBaseClass == null)
					break;
				baseClass = newBaseClass;
			}

			if (baseClass == MyClassNode)
				return;
			if (baseClass.AssemblyName != MyClassNode.AssemblyName)
				persistentRoot = "[" + baseClass.AssemblyName + "]" + baseClass.Name;
			else
				persistentRoot = baseClass.Name;
		}





		public void
			enhance(  )
		{
			addInterfaces(m_classElement);
#if NET11
			addInterfaces(m_classElement.getForwardClassElement());
#endif
			addRelationAttributes();
			addFieldsAndOidAttribute();
			replaceLdfldAndStfld();
			markRelations();
			addFieldAccessors();
			patchConstructor();
			addMethods();
			addMetaClass();
			addQueryHelper();
		}

#if NET11
		public ILClassElement
			addQueryHelperForwardDecl()
		{
			ILClassElement nestedClass = new ILClassElement();
			nestedClass.addLine(".class auto ansi nested public beforefieldinit QueryHelper");
			nestedClass.addLine("extends [mscorlib]System.Object");
			m_classElement.getForwardClassElement().addElement(nestedClass);
			return nestedClass;
		}
#endif

#if NET11
		public ILClassElement
			addNestedMetaClass()
		{
			ILClassElement nestedClass = new ILClassElement();
			nestedClass.addLine(".class auto ansi nested private beforefieldinit MetaClass");
			nestedClass.addLine("extends [mscorlib]System.Object");
			nestedClass.addLine("implements [NDO]NDO.IMetaClass");
			m_classElement.getForwardClassElement().addElement(nestedClass);
			return nestedClass;
		}
#endif


        private void
			addInterfaces(ILClassElement parent)
		{
			if ( ! m_hasPersistentBase )
			{
				parent.AddImplements( new string[] { "[NDO]NDO.IPersistenceCapable", "[NDO]NDO.IPersistentObject" } );
			}
		}

		public void
			addFieldsAndOidAttribute()
		{
			ILMethodElement.Iterator methodIter = m_classElement.getMethodIterator();
			ILMethodElement firstMethod = methodIter.getNext();
//			m_classElement.insertFieldBefore(".field private static string[] _ndoFieldNames", firstMethod);
			
            //if (this.oidTypeName != null)  // Assembly has an OidType
            //{
            //    if (MyClassNode.OidTypeName == null					// Class hasn't an OidType
            //        && this.m_classMapping.Oid.FieldName == null)   // Class hasn't an oid field
            //    {
            //        ILElementIterator it = m_classElement.getAllIterator(false);
            //        ILElement firstEl = it.getNext();
            //        string bytes = ILString.CodeBytes(oidTypeName);
            //        string line = string.Format(".custom instance void [NDO]NDO.NDOOidTypeAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 {0} 00 00 )", bytes);
            //        firstEl.insertBefore(new ILCustomElement(line, m_classElement));
            //    }
            //}

			if (  m_hasPersistentBase )
				return;
			
			m_classElement.insertFieldBefore( ".field family class [NDO]NDO.LoadState _ndoLoadState", firstMethod);
			m_classElement.insertFieldBefore( ".field family valuetype [NDO]NDO.NDOObjectState _ndoObjectState", firstMethod );
			m_classElement.insertFieldBefore( ".field family class [NDO]NDO.ObjectId _ndoObjectId", firstMethod );
			m_classElement.insertFieldBefore( ".field family notserialized class [NDO]NDO.IStateManager _ndoSm", firstMethod );
			m_classElement.insertFieldBefore( ".field family notserialized valuetype [mscorlib]System.Guid _ndoTimeStamp", firstMethod );
		}


		private void addRelationAttributes()
		{
			// Relations with generic containers and with multiplicity 1 can omit
			// the NDORelation attribute. We insert the attribute here.
			ILFieldElement.Iterator it = m_classElement.getFieldIterator();
			ILFieldElement fieldElement;
			for (fieldElement = (ILFieldElement)it.getFirst(); fieldElement != null; fieldElement = (ILFieldElement)it.getNext())
			{
				foreach (Patcher.ILReference reference in this.m_references)
				{
					if (reference.CleanName == fieldElement.getName())
					{
						bool customFound = false;
						for (ILElement custElement = fieldElement.getSuccessor(); custElement != null; custElement = custElement.getSuccessor())
						{
							if (!custElement.getLine(0).StartsWith(".custom"))
								break;
							if (custElement.getLine(0).IndexOf("NDORelationAttribute") > -1)
							{
								customFound = true;
								break;
							}
						}
						if (!customFound)
						{
							fieldElement.insertAfter(new ILCustomElement(".custom instance void [NDO]NDO.NDORelationAttribute::.ctor() = ( 01 00 00 00 )", this.m_classElement));
						}
						break;
					}
				}
			}
		}

		private bool
			replaceValueTypeAccessorCall(ILStatementElement statementElement, string line)
		{
			foreach ( ILField field in ownFieldsHierarchical )
			{
				if (!field.Valid)
					continue;
				if (field.IsEmbeddedType)
					continue;
				if (field.IsValueType)
				{
					string pureTypeName = field.ILType.Substring(10);
					if (line.StartsWith("call"))
					{
						if (line.IndexOf(pureTypeName + "::set_") > -1)
						{
							// Diese Statements landen in umgekehrter Reihenfolge in der Funktion
							statementElement.insertAfter(new ILStatementElement(callMarkDirty()));
							statementElement.insertAfter(new ILStatementElement(ldarg_0));
							return true;
						}
					}
				}
			}
			return false;
		}


		
		private bool
			replaceLdfldAndStfldForLine(ILStatementElement statementElement, string line)
		{
			// At this point it is clear, that the line starts with ldfld or stfld
			bool needsMaxstackAdjustment = false;

			//Durchsuche alle privaten, persistenten Felder
			foreach ( ILField field in ownFieldsHierarchical )
			{
				if (!field.Valid)
					continue;

				int p = line.IndexOf("::");
				string nameInLine = line.Substring(p + 2).Replace("'", string.Empty);

				if (nameInLine == field.Name.Replace("'", string.Empty))
				{
					if ( line.StartsWith( "ldfld" ) )
					{
                        NDO.Mapping.Field fieldMapping = this.m_classMapping.FindField(nameInLine);
                        if (fieldMapping == null)
                        {
                            messages.WriteLine("Warning: can't determine ordinal for field " + m_name + "." + nameInLine);
                            messages.WriteInsertedLine("Fetch Groups don't work with this field.");                            
							statementElement.insertBefore(new ILStatementElement("dup"));
							statementElement.insertBefore(new ILStatementElement(callLoadData()));
							needsMaxstackAdjustment = true;
						}
						else
						{
                            statementElement.insertBefore(new ILStatementElement("dup"));  // this argument
                            statementElement.insertBefore(new ILStatementElement("call       instance void " + m_refName + "::" + getAccName("ndoget_", field.Name) + "()"));
                            // the ldfld[a] statement remains in the code
                            needsMaxstackAdjustment = true;
                        }
					}
                    else if (line.StartsWith("stfld"))
                    {
                        statementElement.setFirstLine("call       instance void " + m_refName + "::" + getAccName("ndoset_", field.Name) + '(' + field.ILType + ')');
                    }
					return needsMaxstackAdjustment;
				}

			} // foreach ( Field field in ownFieldsHierarchical )
			return false;
		}
	
		private void
			replaceLdfldAndStfld()
		{
			ILMethodElement.Iterator methodIter = m_classElement.getMethodIterator();
				  
			for ( ILMethodElement methodElement = methodIter.getNext(); null != methodElement; methodElement = methodIter.getNext() )
			{
				bool adjustMaxStack = false;

				if ( methodElement.isConstructor() )
					continue;

				//string name = methodElement.getLine(methodElement.getLineCount() - 1);

				ILStatementElement.Iterator statementIter = methodElement.getStatementIterator(true);

                // We'll change the collection of statements, so we need a new collection
                // to iterate through.
				IList statements = new ArrayList();
				for ( ILStatementElement statementElement = statementIter.getNext(); null != statementElement; statementElement = statementIter.getNext() )
				{
					statements.Add(statementElement);
				}

				foreach ( ILStatementElement statementElement in statements )
				{
					bool result;
					string line = ILElement.stripLabel(statementElement.getLine( 0 ));
					if ( line.StartsWith( "ldfld" )
						||	 line.StartsWith( "stfld" ) )
					{
						result = replaceLdfldAndStfldForLine(statementElement, line);
						adjustMaxStack = adjustMaxStack || result;
					}
                    // There is a lack in the logic. If a set-Accessor of a value type
                    // is called, we can't determine statically the parent object of the value type.
                    // So we aren't able to set the right object to the dirty state.
				}
				if (adjustMaxStack)
				{
					int newMaxStack = this.getMaxStackVal(methodElement) + 1;
					this.adjustMaxStackVal(methodElement, newMaxStack);
                    MakeLongBranches(methodElement);
				}
			} // ILMethodElement
		}		

		string loadStateManager()
		{
			if (m_hasPersistentBase)
				return "ldfld      class [NDO]NDO.IStateManager " + persistentRoot + "::_ndoSm";
			else
				return "ldfld      class [NDO]NDO.IStateManager " + m_refName+ "::_ndoSm";
		}

		string callLoadData()
		{
			if (m_hasPersistentBase)
				return "call       instance void " + persistentRoot + "::NDOLoadData()";
			else
				return "call       instance void " + m_refName + "::NDOLoadData()";
		}

		string callMarkDirty()
		{
			if (m_hasPersistentBase)
				return "call       instance void " + persistentRoot + "::NDOMarkDirty()";
			else
				return "call       instance void " + m_refName + "::NDOMarkDirty()";
		}

		string loadObjectId()
		{
			if (m_hasPersistentBase)
				return "ldfld      class [NDO]NDO.ObjectId " + persistentRoot + "::_ndoObjectId";
			else
				return "ldfld      class [NDO]NDO.ObjectId " + m_refName + "::_ndoObjectId";
		}
		string storeObjectId()
		{
			if (m_hasPersistentBase)
				return "stfld      class [NDO]NDO.ObjectId " + persistentRoot + "::_ndoObjectId";
			else
				return "stfld      class [NDO]NDO.ObjectId " + m_refName + "::_ndoObjectId";
		}

		string loadObjectState()
		{
			if (m_hasPersistentBase)
				return "ldfld      valuetype [NDO]NDO.NDOObjectState " + persistentRoot + "::_ndoObjectState";
			else
				return "ldfld      valuetype [NDO]NDO.NDOObjectState " + m_refName + "::_ndoObjectState";
		}
		string storeObjectState()
		{
			if (m_hasPersistentBase)
				return "stfld      valuetype [NDO]NDO.NDOObjectState " + persistentRoot + "::_ndoObjectState";
			else
				return "stfld      valuetype [NDO]NDO.NDOObjectState " + m_refName + "::_ndoObjectState";
		}

		string loadTimeStamp()
		{
			if (m_hasPersistentBase)
				return "ldfld      valuetype [mscorlib]System.Guid " + persistentRoot + "::_ndoTimeStamp";
			else
				return "ldfld      valuetype [mscorlib]System.Guid " + m_refName + "::_ndoTimeStamp";
		}
		string storeTimeStamp()
		{
			if (m_hasPersistentBase)
				return "stfld      valuetype [mscorlib]System.Guid " + persistentRoot + "::_ndoTimeStamp";
			else
				return "stfld      valuetype [mscorlib]System.Guid " + m_refName + "::_ndoTimeStamp";
		}


		void addDirtyStatements(ILMethodElement methodElement, bool markDirty)
		{
			if (dirtyDone.Contains(methodElement))
				return;
			adjustMaxStackVal(methodElement, 2);

			ILStatementElement.Iterator statementIter = methodElement.getStatementIterator();
			ILStatementElement beforeElement = statementIter.getNext();
			while(!beforeElement.getLine(0).StartsWith("IL"))
				beforeElement = statementIter.getNext();
			ILStatementElement el = new ILStatementElement();
			el.addLine("ldarg.0");
			el.addLine(loadStateManager());
			el.addLine("brfalse.s  NoSm");
			el.addLine("ldarg.0");
			el.addLine(loadStateManager());
			el.addLine("ldarg.0");
			if (markDirty) 
				el.addLine("callvirt   instance void [NDO]NDO.IStateManager::MarkDirty(class [NDO]NDO.IPersistenceCapable)");
			else
				el.addLine("callvirt   instance void [NDO]NDO.IStateManager::LoadData(class [NDO]NDO.IPersistenceCapable)");
			el.addLine("NoSm:");
			beforeElement.insertBefore(el);
			dirtyDone.Add(methodElement);
		}




		class ReferenceAndElement
		{
			public ReferenceAndElement(ILReference r, ILElement e)
			{
				this.r = r;
				this.e = e;
			}
			public ILReference r;
			public ILElement e;
		}



		/*
				void addAddCalls(IList addEntries, ref int lbl) 
				{

					foreach (ReferenceAndElement refEl in addEntries)
					{
						int mark = lbl++;
						ILElement elToInsert = refEl.e.getPredecessor();
						if (null == elToInsert)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Vorgänger)");
						ILElement elParameter = refEl.e.getSuccessor();
						if (null == elParameter)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Nachfolger)");

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement("brfalse.s  Nosm" + mark.ToString()));

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(@"ldstr      """ + refEl.r.CleanName + @""""));
						elToInsert.insertBefore(new ILStatementElement(stripILx(elParameter.getLine(0))));
						elToInsert.insertBefore(new ILStatementElement(@"callvirt   instance void [NDO]NDO.IStateManager::AddRelatedObject(class [NDO]NDO.IPersistenceCapable, string, class [NDO]NDO.IPersistenceCapable)"));
						elToInsert.insertBefore(new ILStatementElement("Nosm" + mark.ToString() + ":"));
					}

				}
		
				void addRemoveCalls(IList removeEntries, ref int lbl)
				{
					foreach (ReferenceAndElement refEl in removeEntries)
					{
						int mark = lbl++;
						ILElement elToInsert = refEl.e.getPredecessor();
						if (null == elToInsert)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Vorgänger)");
						ILElement elParameter = refEl.e.getSuccessor();
						if (null == elParameter)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Nachfolger)");

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement("brfalse.s  Nosm"+ mark.ToString()));

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(@"ldstr      """ + refEl.r.CleanName + @""""));
						elToInsert.insertBefore(new ILStatementElement(stripILx(elParameter.getLine(0))));
						elToInsert.insertBefore(new ILStatementElement(@"callvirt   instance void [NDO]NDO.IStateManager::RemoveRelatedObject(class [NDO]NDO.IPersistenceCapable, string, class [NDO]NDO.IPersistenceCapable)"));
						elToInsert.insertBefore(new ILStatementElement("Nosm"+ mark.ToString() + ":"));
					}
				}
		
				void addClearCalls(IList clearEntries, ref int lbl)
				{
					foreach (ReferenceAndElement refEl in clearEntries)
					{
						int mark = lbl++;
						ILElement elToInsert = refEl.e.getPredecessor();
						if (null == elToInsert)
							throw new Exception("Ungültiger IL-Code bei IList.Clear (kein Vorgänger)");
						ILElement elParameter = refEl.e;

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement("brfalse.s  Nosm" + mark.ToString()));

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(@"ldstr      """ + refEl.r.CleanName + @""""));
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(stripILx(elParameter.getLine(0))));
						elToInsert.insertBefore(new ILStatementElement(@"callvirt   instance void [NDO]NDO.IStateManager::RemoveRangeRelatedObjects(class [NDO]NDO.IPersistenceCapable, string, class [mscorlib]System.Collections.IList)"));
						elToInsert.insertBefore(new ILStatementElement("Nosm" + mark.ToString() + ":"));
					}
				}

				void addRemoveAtCalls(IList removeAtEntries, ref int lbl)
				{
					foreach (ReferenceAndElement refEl in removeAtEntries)
					{
						int mark = lbl++;
						ILElement elToInsert = refEl.e.getPredecessor();
						if (null == elToInsert)
							throw new Exception("Ungültiger IL-Code bei IList.RemoveAt (kein Vorgänger)");
						ILElement elParameter = refEl.e.getSuccessor();
						if (null == elParameter)
							throw new Exception("Ungültiger IL-Code bei IList.RemoveAt (kein Nachfolger)");
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement("brfalse.s  Nosm" + mark.ToString()));

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(@"ldstr      """ + refEl.r.CleanName + @""""));

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(stripILx(refEl.e.getLine(0))));
						elToInsert.insertBefore(new ILStatementElement(stripILx(elParameter.getLine(0))));
						elToInsert.insertBefore(new ILStatementElement("callvirt   instance object [mscorlib]System.Collections.IList::get_Item(int32)"));
						elToInsert.insertBefore(new ILStatementElement("castclass  [NDO]NDO.IPersistenceCapable"));
						elToInsert.insertBefore(new ILStatementElement("callvirt   instance void [NDO]NDO.IStateManager::RemoveRelatedObject(class [NDO]NDO.IPersistenceCapable, string, class [NDO]NDO.IPersistenceCapable)"));
						elToInsert.insertBefore(new ILStatementElement("Nosm" + mark.ToString() + ":"));
					}
				}

		
				void addInsertCalls(IList insertEntries, ref int lbl)
				{
					foreach (ReferenceAndElement refEl in insertEntries)
					{
						int mark = lbl++;
						ILElement elToInsert = refEl.e.getPredecessor();
						if (null == elToInsert)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Vorgänger)");
						ILElement elParameter = refEl.e.getSuccessor();
						if (null == elParameter)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Nachfolger 1)");
						elParameter = elParameter.getSuccessor();
						if (null == elParameter)
							throw new Exception("Ungültiger IL-Code bei IList.Add (kein Nachfolger 2)");

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement("brfalse.s  Nosm" + mark.ToString()));

						elToInsert.insertBefore(new ILStatementElement(ldarg_0));
						elToInsert.insertBefore(new ILStatementElement(loadStateManager()));
						elToInsert.insertBefore(new ILStatementElement(ldarg_0));

						elToInsert.insertBefore(new ILStatementElement(@"ldstr      """ + refEl.r.CleanName + @""""));
						elToInsert.insertBefore(new ILStatementElement(stripILx(elParameter.getLine(0))));

						elToInsert.insertBefore(new ILStatementElement("callvirt   instance void [NDO]NDO.IStateManager::AddRelatedObject(class [NDO]NDO.IPersistenceCapable, string, class [NDO]NDO.IPersistenceCapable)"));
						elToInsert.insertBefore(new ILStatementElement("Nosm" + mark.ToString() + ":"));

					}
				}
		*/
		private void 
			adjustMaxStackVal(ILMethodElement methodElement, int maxStackVal)
		{
			ILElement msEl = methodElement.findElement(".maxstack");
			if (null == msEl)
				throw new Exception("Method " + m_refName + "." + methodElement.getName() + " doesn't have a .maxstack statement.");
			string msline = msEl.getLine(0); 
			string num = msline.Substring(msline.IndexOf(".maxstack") + 9);
			if (int.Parse(num) < maxStackVal)
			{
				msEl.remove();
				msEl = new ILStatementElement();
				msEl.addLine(".maxstack " + maxStackVal.ToString());
				methodElement.getStatementIterator().getNext().insertBefore(msEl);
			}
		}

		private void 
			addToMaxStackVal(ILMethodElement methodElement, int diff)
		{
			ILElement msEl = methodElement.findElement(".maxstack");
			if (null == msEl)
                throw new Exception("Method " + m_refName + "." + methodElement.getName() + " doesn't have a .maxstack statement.");
            string msline = msEl.getLine(0); 
			string num = msline.Substring(msline.IndexOf(".maxstack") + 9);
			int newNum = int.Parse(num) + diff;
			ILStatementElement newEl = new ILStatementElement();
			msEl.insertBefore(newEl);
			msEl.remove();
			newEl.addLine(".maxstack " + newNum.ToString());
		}

		private int 
			getMaxStackVal(ILMethodElement methodElement)
		{
			ILElement msEl = methodElement.findElement(".maxstack");
			if (null == msEl)
                throw new Exception("Method " + m_refName + "." + methodElement.getName() + " doesn't have a .maxstack statement.");
            string msline = msEl.getLine(0); 
			string num = msline.Substring(msline.IndexOf(".maxstack") + 9);
			return (int.Parse(num));
		}


		private void addLocalVariable(ILMethodElement methodElement, string name, string ilType)
		{
			ILLocalsElement localsElement = methodElement.getLocals();
			if (localsElement == null)
			{
				localsElement = new ILLocalsElement(".locals init ([0] " + ilType + " " + name + ")", methodElement);
				ILElement maxstackElement = methodElement.findElement(".maxstack");
				if (maxstackElement == null)
                    throw new Exception("Method " + m_refName + "." + methodElement.getName() + " doesn't have a .maxstack statement.");
                maxstackElement.insertAfter(localsElement);
				return;
			}
			string line = localsElement.getAllLines();
			Regex regex = new Regex(@"\[(\d+)\]");
			MatchCollection matches = regex.Matches(line);
			ILLocalsElement newLocalsElement;
			int lastBracketPos = line.LastIndexOf(")");
			line = line.Substring(0, lastBracketPos);
			if (matches.Count == 0)
			{
				line += String.Format(", {0} {1})", ilType, name);
			}
			else
			{
				int lastOrdinal = int.Parse(matches[matches.Count - 1].Groups[1].Value) + 1;
				line += String.Format(", [{0}] {1} {2})", lastOrdinal, ilType, name);
			}
			newLocalsElement = new ILLocalsElement(line, methodElement);
			localsElement.insertBefore(newLocalsElement);
			localsElement.remove();
		}

		public void
			markRelations()
		{
			if (m_references.Count == 0)
				return;

            ILMethodElement.Iterator methodIter = m_classElement.getMethodIterator();
			ListAccessManipulator accessManipulator = new ListAccessManipulator(this.m_classElement.getAssemblyName());
			
			for ( ILMethodElement methodElement = methodIter.getNext(); null != methodElement; methodElement = methodIter.getNext() )
			{
				bool needsContainerStack = false;
				bool methodTouched = false;
                string mname = methodElement.getName();
				if ( methodElement.isConstructor() )
					continue;

				ILStatementElement.Iterator statementIter = methodElement.getStatementIterator(true);

				ArrayList statements = new ArrayList(100);
				ILElement firstElement = null;
				ILLocalsElement localsElement = methodElement.getLocals();
				Hashtable listReflectors = new Hashtable();
				string line;
				int pos;

				// Wir kopieren die Statements in eine extra ArrayList, weil die originale 
				// Liste mit InsertBefore manipuliert wird.
				for ( ILStatementElement statementElement = statementIter.getNext(); null != statementElement; statementElement = statementIter.getNext() )
				{
					statements.Add(statementElement);
					line = statementElement.getLine( 0 );
					if (firstElement == null && line.StartsWith("IL_"))
						firstElement = statementElement;
				}
				ILElement lineEl = firstElement;
				if (firstElement != null)
				{
					lineEl = firstElement.getPredecessor();					
					if (lineEl != null && lineEl.getLine(0).StartsWith(".line"))
						firstElement = lineEl;
				}

				foreach ( ILStatementElement statementElement in statements )
				{
					line = statementElement.getLine( 0 );
					pos  = line.IndexOf( ":" );
					line = line.Substring( pos + 1 ).Trim();
					int p = line.IndexOf("::");

					string cleanName = null;
					if (p > -1)
						cleanName = line.Substring(p + 2).Replace("'", string.Empty);

					if ( line.StartsWith( "ldfld" ) )
					{
						//Durchsuche alle privaten, persistenten Felder
						foreach ( ILReference reference in m_references)
						{
							if (reference.IsInherited)
								continue;
							if (reference.CleanName == cleanName && (-1 < line.IndexOf( m_refName + "::")))
							{
								if (reference.Is1to1)
								{
									statementElement.setFirstLine("call       instance " + reference.ILType + " " + m_refName + "::" + getAccName("ndoget_", reference.Name) + "()"); 
								}
								else
								{
									if (!listReflectors.Contains(reference.FieldType))
										listReflectors.Add(reference.FieldType, ListReflectorFactory.CreateListReflector(reference.FieldType));
									statementElement.insertBefore(new ILStatementElement("dup"));
									IList elements = new ArrayList();
									elements.Add(new ILStatementElement("ldloc __ndocontainertable"));
									elements.Add(new ILStatementElement(@"ldstr """ + reference.CleanName + @""""));
									elements.Add(new ILStatementElement("call       object [NDO]NDO._NDOContainerStack::RegisterContainer(object,object,class [mscorlib]System.Collections.Hashtable,string)"));
                                    elements.Add(new ILStatementElement("castclass " + new ReflectedType(reference.FieldType, this.m_classElement.getAssemblyName()).QuotedILName));
									// Achtung: insertAfter benötigt die Statements in umgekehrter Reihenfolge
									for (int i = elements.Count - 1; i >=0; i--)
										statementElement.insertAfter((ILStatementElement)elements[i]);
									needsContainerStack = true;
									break;
								}
							}
						}
					}
					if ( line.StartsWith( "stfld" ) )
					{
						//Durchsuche alle Relationen
						foreach (ILReference reference in m_references)
						{
							if (reference.IsInherited)
								continue;
							if (reference.CleanName == cleanName)
							{
								if ( -1 < line.IndexOf( m_refName + "::"))
								{
									statementElement.setFirstLine("call       instance void " + m_refName + "::" + getAccName("ndoset_", reference.Name) + "(" + reference.ILType +")");
									break;
								}
							}
						}
					}
				}
				if (needsContainerStack)
				{
					foreach ( ILStatementElement statementElement in statements )
					{
						line = ILElement.stripLabel( statementElement.getLine( 0 ) );

						if ( line.StartsWith( "callvirt" ) || line.StartsWith("call ") )
						{
								if (accessManipulator.Manipulate(listReflectors, statementElement))
									methodTouched = true;
#if ReplacedWith_ListAccessManipulator
							if (line.IndexOf("List::AddRange") > -1 || line.IndexOf("Collection::AddRange") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::AddRange(object,class [mscorlib]System.Collections.ICollection,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
							else if (line.IndexOf("List::InsertRange") > -1 || line.IndexOf("Collection::InsertRange") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::InsertRange(object,int32,class [mscorlib]System.Collections.ICollection,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
							else if (line.IndexOf("List::RemoveRange") > -1 || line.IndexOf("Collection::RemoveRange") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::RemoveRange(object,int32,int32,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
							else if (line.IndexOf("List::SetRange") > -1 || line.IndexOf("Collection::SetRange") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::SetRange(object,int32,class [mscorlib]System.Collections.ICollection,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
							else if (line.IndexOf("List::Add(object)") > -1 || line.IndexOf("Collection::Add(object)") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       int32 [NDO]NDO._NDOContainerStack::AddRelatedObject(object,object,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
							else if (line.IndexOf("List::Clear()") > -1 || line.IndexOf("Collection::Clear()") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::RemoveRange(object,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
							else if (line.IndexOf("List::Insert") > -1 || line.IndexOf("Collection::Insert") > -1)
							{
								// int index, object value
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::Insert(object,int32,object,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();

							}
							else if (line.IndexOf("List::RemoveAt") > -1 || line.IndexOf("Collection::RemoveAt") > -1)
							{
								// includes RemoveAt
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::RemoveRelatedObjectAt(object,int32,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();

							}
							else if (line.IndexOf("List::Remove") > -1 || line.IndexOf("Collection::Remove") > -1)
							{
								methodTouched = true;
								statementElement.insertBefore(new ILStatementElement("ldloc __ndocontainertable"));
								statementElement.insertBefore(new ILStatementElement("call       void [NDO]NDO._NDOContainerStack::RemoveRelatedObject(object,object,class [mscorlib]System.Collections.Hashtable)"));
								statementElement.remove();
							}
#endif
						}
					}
				}

				if (needsContainerStack)
				{
					addLocalVariable(methodElement, "__ndocontainertable", "class [mscorlib]System.Collections.Hashtable");
					firstElement.insertBefore(new ILStatementElement("newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor()"));
					firstElement.insertBefore(new ILStatementElement("stloc __ndocontainertable"));
					addToMaxStackVal(methodElement, 3);
				}

				//				if (needsContainerStack || readEntries)
				//					addDirtyStatements(methodElement, false);
				if (methodTouched || needsContainerStack)
					MakeLongBranches(methodElement);
			} // ILMethodElement
		}


		public void MakeLongBranches(ILMethodElement methodElement)
		{
			ILStatementElement.Iterator statementIter = methodElement.getStatementIterator(true);

			for ( ILStatementElement statementElement = statementIter.getNext(); null != statementElement; statementElement = statementIter.getNext() )
			{
				string line = statementElement.getLine(0);
				if (line.StartsWith("IL"))
				{
					for (int i = 0; i < AllBranches.Opcodes.Length; i++)
					{
						string s = AllBranches.Opcodes[i];
						if (line.IndexOf(s) > -1)
						{
							statementElement.replaceText(s, AllBranches.LongOpcodes[i]);
							break;
						}
					}
				}
			}				
		}

		public void
			patchConstructor()
		{
			ArrayList constructors = new ArrayList();
			ILMethodElement.Iterator methodIter = m_classElement.getMethodIterator();

			bool hasDefaultConstructor = false;
			for ( ILMethodElement methodElement = methodIter.getNext(); null != methodElement; methodElement = methodIter.getNext() )
			{
				if ( !methodElement.isConstructor() )
					continue;

				constructors.Add(methodElement);

				if (methodElement.getParameterCount() == 0)
					hasDefaultConstructor = true;
			}

			if (!hasDefaultConstructor)
				constructors.Add(addDefaultConstructor());

			int maxStack = this.m_hasPersistentBase ? 2 : 2;
			
			foreach(ILMethodElement me in constructors)
			{
				adjustMaxStackVal(me, maxStack);

				ILStatementElement statementElement = (ILStatementElement) me.getStatementIterator().getLast();
				string line = ILElement.stripLabel(statementElement.getLine(0));
								
				IList elements = new ArrayList();

				if (!this.m_hasPersistentBase)
				{
					elements.Add(new ILStatementElement("ldarg.0"));
					elements.Add(new ILStatementElement("ldc.i4.0"));
					elements.Add(new ILStatementElement("stfld      valuetype [NDO]NDO.NDOObjectState " + m_refName + "::_ndoObjectState"));
					elements.Add(new ILStatementElement("ldarg.0"));
					elements.Add(new ILStatementElement("newobj     instance void [NDO]NDO.LoadState::.ctor()"));
					elements.Add(new ILStatementElement("stfld      class [NDO]NDO.LoadState " + m_refName + "::_ndoLoadState"));
				}
				if (!(line == "ret") && elements.Count > 0)
					messages.WriteLine("Can't find return statement of the constructor of class " + m_refName);

				foreach (ILStatementElement el in elements)
					statementElement.insertBefore(el);
			}


		}


		public ILMethodElement addDefaultConstructor()
		{
			messages.WriteLine("Warning: A default constructor for class " + m_refName.Replace("'", string.Empty) + " has been created by the enhancer. The constructor doesn't contain any initialization code.");
			ILMethodElement methodElement = new ILMethodElement();
			methodElement.addLine(".method public hidebysig specialname rtspecialname instance void  .ctor() cil managed");
			methodElement.addStatement(".maxstack 2");
			methodElement.addStatement( ldarg_0 );  // this-Parameter for Constructor
			if (!m_hasPersistentBase)
			{
				methodElement.addStatement( "call       instance void [mscorlib]System.Object::.ctor()" );  // System.Object
			}
			else
			{
				methodElement.addStatement( "call       instance void " + m_persistentBase + "::.ctor()" );  // Base class
			}
			methodElement.addStatement("ret");
			this.m_classElement.addElement(methodElement);
			return methodElement;
		}

		public void
			addMethods()
		{
			if ( !m_hasPersistentBase )
			{
				//IPersistenceCapable-Implementierung
				addMarkDirty();
				addLoadData();
				addObjectId();
				addTimeStamp();
				addGetObjectHashCode();
				addObjectState();
				addStateManager();
				addGetNDOHandler();
			}
			else
			{
				removeMarkDirty();
				removeLoadData();
				removeObjectId();
				removeTimeStamp();
				removeObjectState();
				removeStateManager();
				removeGetNDOHandler();
			}
			addRead();
			addWrite();
			addRelationAccessors();
			addLoadStateProperty();
			addGetLoadState();
			addSetLoadState();
		}

		string createNDOException()
		{
			return "newobj instance void [NDOInterfaces]NDO.NDOException::.ctor(int32, string)";
		}

		void addGetOrdinal(ILClassElement metaClass)
		{
			ILMethodElement methodElement = new ILMethodElement();
			methodElement.addLine( ".method public hidebysig newslot final virtual instance int32 GetRelationOrdinal(string fieldName) cil managed" );

			methodElement.addStatement(".maxstack  4");
			addLocalVariable(methodElement, "result", "int32");
			foreach(ILReference r in m_references)
			{
				methodElement.addStatement(ldarg_1);
				methodElement.addStatement(@"ldstr      """ + r.CleanName + @"""");
				methodElement.addStatement("call       bool [mscorlib]System.String::op_Equality(string,string)");
				methodElement.addStatement("brfalse.s  notthis" + r.Ordinal.ToString());
				methodElement.addStatement(loadIntConst(r.Ordinal));
				methodElement.addStatement("stloc.0");
				methodElement.addStatement("br       exit");
				methodElement.addStatement("notthis" + r.Ordinal.ToString() + ":");
			}
			methodElement.addStatement(loadIntConst(20));
			methodElement.addStatement(@"ldstr      ""GetRelationOrdinal: Can't find field " + m_refName + @".""");
			methodElement.addStatement(ldarg_1);
			methodElement.addStatement("call       string [mscorlib]System.String::Concat(string,string)");
			methodElement.addStatement(createNDOException());
			methodElement.addStatement("throw");
			methodElement.addStatement("exit:  ldloc.0");
			methodElement.addStatement("ret");

			metaClass.addElement(methodElement);
		}

		void addLoadStateProperty()
		{
			ILMethodElement methodElement = m_classElement.getMethod( "get_NDOLoadState" );
			ILPropertyElement propElement = m_classElement.getProperty( "NDOLoadState" );

			if ( m_hasPersistentBase )
			{
				if (methodElement != null)
					methodElement.remove();
				if ( propElement != null )
					propElement.remove();
				return;
			}
	
			if ( methodElement == null )
			{
				methodElement = new ILMethodElement();
				methodElement.addLine( ".method public hidebysig newslot specialname final virtual" );
				methodElement.addLine( "instance class [NDO]NDO.LoadState get_NDOLoadState() cil managed" );
				this.m_classElement.addElement( methodElement );
			}
			else
			{				
				methodElement.clearElements();
			}
			methodElement.addStatement(".maxstack  3");
			methodElement.addStatement(ldarg_0);
			methodElement.addStatement("ldfld      class [NDO]NDO.LoadState " + m_refName + "::_ndoLoadState");
			methodElement.addStatement("ret");

			if ( propElement == null )
			{
				propElement = new ILPropertyElement();
				propElement.addLine( ".property instance class [NDO]NDO.LoadState NDOLoadState()" );
				propElement.addElement( new ILCustomElement( ".custom instance void [System]System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", propElement ) );
				propElement.addElement( new ILCustomElement( ".custom instance void [System.Xml]System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", propElement ) );
				propElement.addElement( new ILGetElement( ".get instance class [NDO]NDO.LoadState " + m_nonGenericRefName + "::get_NDOLoadState()" ) );
				this.m_classElement.addElement( propElement );
			}
		}

		void addGetLoadState()
		{
			ILMethodElement methodElement = m_classElement.getMethod( "NDOGetLoadState" );
			
			if ( m_hasPersistentBase )
			{
				if ( methodElement != null )
					methodElement.remove();
				return;
			}

			if ( methodElement == null )
			{
				methodElement = new ILMethodElement();
				methodElement.addLine( ".method public hidebysig newslot virtual" );
				methodElement.addLine( "instance bool NDOGetLoadState(int32 ordinal) cil managed" );
				this.m_classElement.addElement( methodElement );
			}
			else
			{
				methodElement.clearElements();
			}
			methodElement.addStatement(".maxstack  3");

			methodElement.addStatement(ldarg_0);
			methodElement.addStatement("ldfld      class [NDO]NDO.LoadState " + m_refName + "::_ndoLoadState");
			methodElement.addStatement("callvirt   instance class [mscorlib]System.Collections.BitArray [NDO]NDO.LoadState::get_RelationLoadState()");
			methodElement.addStatement(ldarg_1);
			methodElement.addStatement("callvirt instance bool [mscorlib]System.Collections.BitArray::get_Item(int32)");
			methodElement.addStatement("ret");
		}

		void addSetLoadState()
		{
			ILMethodElement methodElement = m_classElement.getMethod( "NDOSetLoadState" );
			if ( m_hasPersistentBase )
			{
				if ( methodElement != null )
					methodElement.remove();
				return;
			}
			if ( methodElement == null )
			{
				methodElement = new ILMethodElement();
				methodElement.addLine( ".method public hidebysig newslot virtual" );
				methodElement.addLine( "instance void NDOSetLoadState(int32 ordinal, bool isLoaded) cil managed" );
				this.m_classElement.addElement( methodElement );
			}
			else
			{
				methodElement.clearElements();
			}
			methodElement.addStatement(".maxstack  3");

			methodElement.addStatement(ldarg_0);
			methodElement.addStatement("ldfld      class [NDO]NDO.LoadState "  + m_refName + "::_ndoLoadState");
			methodElement.addStatement("callvirt   instance class [mscorlib]System.Collections.BitArray [NDO]NDO.LoadState::get_RelationLoadState()");
			methodElement.addStatement("ldarg.1");
			methodElement.addStatement("ldarg.2");
			methodElement.addStatement("callvirt instance void [mscorlib]System.Collections.BitArray::set_Item(int32, bool)");
			methodElement.addStatement("ret");
		}




		string getAccName(string prefix, string fieldName)
		{
			if (fieldName.StartsWith("'"))
				return "'" + prefix + fieldName.Replace("'", string.Empty) + "'";
			else
				return prefix + fieldName;
		}

		private void
			addRelationAccessors()
		{
			foreach ( ILReference reference in m_references)
			{
				if (reference.IsInherited)
					continue;
				if (reference.Is1to1)
					add1to1RelationAccessors(reference);
				else
					add1toNRelationAccessors(reference);
			}
		}

		private string loadIntConst(int val)
		{
			string valStr = val.ToString();
			if (val >= 0 && val < 8)
				return "ldc.i4." + valStr;
			else if (val >= -128 && val <= 127)
				return "ldc.i4.s " + valStr;
			else
				return "ldc.i4 " + valStr;
		}

		private void
			add1to1RelationGetAccessor(ILReference reference)
		{
			// Get-Methode nur für 1:1
			ILMethodElement method = new ILMethodElement();
			method.addLine( ".method private hidebysig instance " );
			method.addLine( reference.ILType + " " + getAccName("ndoget_", reference.Name) + "() cil managed");
#if nix
			method.addStatement(".maxstack  2");
			method.addStatement(ldarg_0);
			method.addStatement(loadStateManager());
			method.addStatement("brfalse.s  noSm");
			method.addStatement(ldarg_0);
			method.addStatement(loadStateManager());
			method.addStatement(ldarg_0);
			method.addStatement("callvirt   instance void [NDO]NDO.IStateManager::LoadData(class [NDO]NDO.IPersistenceCapable)");
			method.addStatement("noSm:");
			method.addStatement(ldarg_0);
			method.addStatement("ldfld      " + reference.ILType + " " + m_refName + "::" + reference.Name);
			method.addStatement( "ret");
#else
			method.addStatement(".maxstack  5" );
			method.addStatement(ldarg_0);
			method.addStatement( loadStateManager() );
			method.addStatement( "brfalse.s  noSm");
			//			method.addStatement(ldarg_0);
			//			method.addStatement("ldfld      bool[] " + m_refName + "::" + GetRelationStateName());
			//			method.addStatement(loadIntConst(reference.Ordinal));
			//			method.addStatement("ldelem.i1");
			//			method.addStatement("brtrue.s   noSm");
			method.addStatement(ldarg_0);
			method.addStatement( loadStateManager() );
			method.addStatement(ldarg_0);
			method.addStatement(@"ldstr      """ + reference.CleanName + @"""");
			method.addStatement("callvirt   instance class [mscorlib]System.Collections.IList [NDO]NDO.IStateManager::LoadRelation(class [NDO]NDO.IPersistenceCapable,string)");
			method.addStatement("pop");
			//			method.addStatement(ldarg_0);
			//			method.addStatement("ldfld      bool[] " + m_refName + "::" + GetRelationStateName());
			//			method.addStatement(loadIntConst(reference.Ordinal));
			//			method.addStatement("ldc.i4.1");
			//			method.addStatement("stelem.i1");
			method.addStatement("noSm:");
			method.addStatement(ldarg_0);
			method.addStatement("ldfld      " + reference.ILType + " " + m_refName + "::" + reference.Name);
			method.addStatement( "ret");
#endif
			m_classElement.getMethodIterator().getLast().insertAfter( method );
		}

		private void
			add1to1RelationAccessors(ILReference reference)
		{
			add1to1RelationGetAccessor(reference);

			// Set-Methode
			ILMethodElement method = new ILMethodElement();
			method.addLine( ".method private hidebysig specialname instance void" );
			method.addLine( getAccName("ndoset_", reference.Name) + "(" + reference.ILType + " 'value') cil managed");
			method.addStatement( ".maxstack  4" );
			method.addStatement( ldarg_0 );
			method.addStatement( loadStateManager());
			method.addStatement( "brfalse.s  IL_003c");

			method.addStatement( ldarg_0);
			method.addStatement( loadStateManager() );
			method.addStatement( ldarg_0);
			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::LoadData(class [NDO]NDO.IPersistenceCapable)");

			method.addStatement( ldarg_0 );
			method.addStatement( "ldfld      " + reference.ILType + " "+ m_refName + "::" + reference.Name);
			method.addStatement( "brfalse.s  IL_0027");

			method.addStatement( ldarg_0 );
			method.addStatement( loadStateManager());
			method.addStatement( ldarg_0 );
			method.addStatement( @"ldstr      """ + reference.CleanName + @"""");
			method.addStatement( ldarg_0 );
			method.addStatement( "ldfld      " + reference.ILType + " "+ m_refName + "::" + reference.Name);
			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::RemoveRelatedObject(class [NDO]NDO.IPersistenceCapable,string,class [NDO]NDO.IPersistenceCapable)");
			method.addStatement( "IL_0027: ldarg.1");
			method.addStatement( "brfalse.s  IL_003c");

			method.addStatement( ldarg_0);
			method.addStatement( loadStateManager());
			method.addStatement( ldarg_0);
			method.addStatement( @"ldstr      """ + reference.CleanName + @"""");
			method.addStatement( ldarg_1);
			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::AddRelatedObject(class [NDO]NDO.IPersistenceCapable,string,class [NDO]NDO.IPersistenceCapable)");
			method.addStatement( "IL_003c: ldarg.0");
			method.addStatement( ldarg_1);
			method.addStatement( "stfld      " + reference.ILType + " "+ m_refName + "::" + reference.Name);
			method.addStatement( "ret");
			m_classElement.getMethodIterator().getLast().insertAfter( method );
		}


		private void
			add1toNRelationAccessors(ILReference reference)
		{
			//addRelationGetAccessor(reference);

			// Set-Methode
			ILMethodElement method = new ILMethodElement();
			method.addLine( ".method private hidebysig specialname instance void" );
			method.addLine( getAccName("ndoset_", reference.Name) + "(" + reference.ILType + " 'value') cil managed");
			method.addStatement( ".maxstack  5" );

			method.addStatement( ldarg_0 );
			method.addStatement( loadStateManager());
			method.addStatement( "brfalse.s  NoSm");

			method.addStatement( ldarg_0);
			method.addStatement( loadStateManager() );

			method.addStatement( ldarg_0);  // Arg. 1
			method.addStatement(@"ldstr      """ + reference.CleanName + @"""");

			method.addStatement(ldarg_0);
			method.addStatement( "ldfld      " + reference.ILType + " "+ m_refName + "::" + reference.Name);

			method.addStatement( ldarg_1 );

			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::AssignRelation(class [NDO]NDO.IPersistenceCapable, string, class [mscorlib]System.Collections.IList, class [mscorlib]System.Collections.IList)");

			method.addStatement( "NoSm:");
			method.addStatement(ldarg_0);
			method.addStatement( ldarg_1 );
			method.addStatement( "stfld      " + reference.ILType + " "+ m_refName + "::" + reference.Name);
			method.addStatement( "ret");

			m_classElement.getMethodIterator().getLast().insertAfter( method );
		}

		private void addFieldAccessors()
		{
			foreach (ILField f in this.ownFieldsHierarchical)
			{
				if (f.IsEmbeddedType)
					messages.WriteLine("The type of the field " + this.m_refName + "." + f.CleanName + " is not a storable type; it will be treated as embedded type.");
				addFieldAccessorPair(f);
			}
		}

		private void addFieldAccessorPair(ILField field)
		{
			ILMethodElement method;
			method = new ILMethodElement();
			method.addLine( ".method private hidebysig instance void" );
			//			string classStr = field.IsEmbeddedType ? "class " : string.Empty;
			method.addLine( getAccName("ndoset_", field.Name) + "(" + field.ILType + " 'value') cil managed");
			method.addStatement(".maxstack  2");
			method.addStatement(ldarg_0);
			method.addStatement(this.loadStateManager());
			method.addStatement("brfalse.s  NoSm");
			method.addStatement(ldarg_0);
			method.addStatement(this.loadStateManager());
			method.addStatement(ldarg_0);
			method.addStatement("callvirt   instance void [NDO]NDO.IStateManager::MarkDirty(class [NDO]NDO.IPersistenceCapable)");
			method.addStatement("NoSm:");
			method.addStatement(ldarg_0);
			method.addStatement(ldarg_1);
			
			method.addStatement("stfld      " + field.ILType + " " + m_refName + "::" + field.Name);
			method.addStatement("ret");

			m_classElement.getMethodIterator().getLast().insertAfter( method );

            NDO.Mapping.Field fieldMapping = this.m_classMapping.FindField(field.CleanName);

			// We can't get Value Types with getter functions, because that would result in a
            // copy of the object. So the original object couldn't be manipulated.            
			if (!field.IsValueType && fieldMapping != null)
			{
				method = new ILMethodElement();
				method.addLine(".method private hidebysig instance void");
                method.addLine(getAccName("ndoget_", field.Name) + "() cil managed");
				method.addStatement(".maxstack  3");
				method.addStatement(ldarg_0);
				method.addStatement(this.loadStateManager());
				method.addStatement("brfalse.s  NoSm");
				method.addStatement(ldarg_0);
				method.addStatement(this.loadStateManager());
				method.addStatement(ldarg_0);
                method.addStatement(this.loadIntConst(fieldMapping.Ordinal)); 
				method.addStatement("callvirt   instance void [NDO]NDO.IStateManager::LoadField(class [NDO]NDO.IPersistenceCapable,int32)");
                method.addStatement("NoSm: ret");
                // The original ldfld[a] statement remains in the code.
                // method.addStatement("NoSm:  ldarg.0");
                // method.addStatement("ldfld      " + field.ILType + " " + m_refName + "::" + field.Name);
				// method.addStatement("ret"); 
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
		}

		public void
		removeMarkDirty()
		{
			ILMethodElement method = m_classElement.getMethod( "NDOMarkDirty" );

			if ( method != null )
				method.remove();
		}

		public void 
		addMarkDirty()
		{
			ILMethodElement method = m_classElement.getMethod( "NDOMarkDirty" );

			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot virtual final" );
				method.addLine( "instance void  NDOMarkDirty() cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  2" );
			method.addStatement( ldarg_0 );
			method.addStatement( loadStateManager());
			method.addStatement( "dup" );
			method.addStatement( "brfalse.s  NoSm" );

//			method.addStatement( ldarg_0);  // used dup instead
//			method.addStatement( loadStateManager() );
			method.addStatement( ldarg_0);
			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::MarkDirty(class [NDO]NDO.IPersistenceCapable)");
			method.addStatement( "NoSm: ret");
	}

		public void
		removeLoadData()
		{
			ILMethodElement method = m_classElement.getMethod( "NDOLoadData" );
			if ( method != null )
				method.remove();
		}

		public void 
		addLoadData()
		{
			ILMethodElement method = m_classElement.getMethod( "NDOLoadData" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				m_classElement.getMethodIterator().getLast().insertAfter( method );
				method.addLine( ".method family hidebysig newslot final virtual" );
				method.addLine( "instance void  NDOLoadData() cil managed" );
			}
			method.addStatement( ".maxstack  2" );
			method.addStatement( ldarg_0 );
			method.addStatement( loadStateManager());
			method.addStatement( "brfalse.s  NoSm");

			method.addStatement( ldarg_0);
			method.addStatement( loadStateManager() );
			method.addStatement( ldarg_0);
			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::LoadData(class [NDO]NDO.IPersistenceCapable)");
			method.addStatement( "NoSm: ret");

		}

		public void
		removeObjectId()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOObjectId" );
			if ( method != null )
				method.remove();
			method = m_classElement.getMethod( "set_NDOObjectId" );
			if ( method != null )
				method.remove();
			ILPropertyElement prop = m_classElement.getProperty( "NDOObjectId" );
			if ( prop != null )
				prop.remove();
		}

		public void 
		addObjectId()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOObjectId" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance class [NDO]NDO.ObjectId get_NDOObjectId() cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement(loadObjectId());
			method.addStatement( "ret");


			method = m_classElement.getMethod( "set_NDOObjectId" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				m_classElement.getMethodIterator().getLast().insertAfter( method );
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance void  set_NDOObjectId(class [NDO]NDO.ObjectId 'value') cil managed" );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			method.addStatement(storeObjectId());
			method.addStatement( "ret");


			ILPropertyElement prop = m_classElement.getProperty( "NDOObjectId" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance class [NDO]NDO.ObjectId NDOObjectId()", m_classElement );
				prop.addElement( new ILCustomElement( ".custom instance void [System]System.ComponentModel.CategoryAttribute::.ctor(string) = ( 01 00 03 4E 44 4F 00 00 )", prop ) );
				prop.addElement( new ILCustomElement( ".custom instance void [System.Xml]System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.addElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOObjectId(class [NDO]NDO.ObjectId)" ) );
				prop.addElement( new ILGetElement( ".get instance class [NDO]NDO.ObjectId " + m_nonGenericRefName + "::get_NDOObjectId()" ) );
				m_classElement.getMethodIterator().getLast().insertAfter( prop );
			}
		}

		public void
		removeTimeStamp()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOTimeStamp" );
			if ( method != null )
				method.remove();
			method = m_classElement.getMethod( "set_NDOTimeStamp" );
			if ( method != null )
				method.remove();
			ILPropertyElement prop = m_classElement.getProperty( "NDOTimeStamp" );
			if ( prop != null )
				prop.remove();
		}

		public void 
		addTimeStamp()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOTimeStamp" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				//.method public hidebysig specialname instance valuetype [mscorlib]System.Guid 
				// get_G2() cil managed
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance valuetype [mscorlib]System.Guid get_NDOTimeStamp() cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement(loadTimeStamp());
			method.addStatement( "ret");


			method = m_classElement.getMethod( "set_NDOTimeStamp" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance void  set_NDOTimeStamp(valuetype [mscorlib]System.Guid 'value') cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			method.addStatement(storeTimeStamp());
			method.addStatement( "ret");

			ILPropertyElement prop = m_classElement.getProperty( "NDOTimeStamp" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance valuetype [mscorlib]System.Guid NDOTimeStamp()", m_classElement );
				prop.addElement( new ILCustomElement( ".custom instance void [System]System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", prop ) );
				prop.addElement( new ILCustomElement( ".custom instance void [System.Xml]System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.addElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOTimeStamp(valuetype [mscorlib]System.Guid)" ) );
				prop.addElement( new ILGetElement( ".get instance valuetype [mscorlib]System.Guid " + m_nonGenericRefName + "::get_NDOTimeStamp()" ) );
				m_classElement.getMethodIterator().getLast().insertAfter( prop );
			}
		}

		public void
		removeObjectState()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOObjectState" );
			if ( method != null )
				method.remove();
			method = m_classElement.getMethod( "set_NDOObjectState" );
			if ( method != null )
				method.remove();
			ILPropertyElement prop = m_classElement.getProperty( "NDOObjectState" );
			if ( prop != null )
				prop.remove();
		}

		public void
		addGetObjectHashCode()
		{
			ILMethodElement method = new ILMethodElement();
			if ( m_hasPersistentBase )
				method.addLine( ".method public hidebysig virtual" );
			else
				method.addLine( ".method public hidebysig newslot virtual" );
			method.addLine( "instance int32 NDOGetObjectHashCode() cil managed" );
			m_classElement.getMethodIterator().getLast().insertAfter( method );

			method.addStatement(".maxstack  1");
			method.addStatement(ldarg_0);
			method.addStatement("call       instance int32 [mscorlib]System.Object::GetHashCode()");
			method.addStatement("ret");			   
		}

		public void 
		addObjectState()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOObjectState" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance valuetype [NDO]NDO.NDOObjectState get_NDOObjectState() cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( this.loadObjectState() );
			method.addStatement( "ret");


			method = m_classElement.getMethod( "set_NDOObjectState" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance void  set_NDOObjectState(valuetype [NDO]NDO.NDOObjectState 'value') cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			method.addStatement(this.storeObjectState());
			method.addStatement( "ret");


			ILPropertyElement prop = m_classElement.getProperty( "NDOObjectState" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance valuetype [NDO]NDO.NDOObjectState NDOObjectState()", m_classElement );
				prop.addElement( new ILCustomElement( ".custom instance void [System]System.ComponentModel.CategoryAttribute::.ctor(string) = ( 01 00 03 4E 44 4F 00 00 )", prop ) );
				prop.addElement( new ILCustomElement( ".custom instance void [System.Xml]System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.addElement( new ILGetElement( ".get instance valuetype [NDO]NDO.NDOObjectState " + m_nonGenericRefName + "::get_NDOObjectState()" ) );
				prop.addElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOObjectState(valuetype [NDO]NDO.NDOObjectState)" ) );

				m_classElement.getPropertyIterator().getLast().insertAfter( prop );
			}

		}

		public void
		removeStateManager()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOStateManager" );
			if ( method != null )
				method.remove();
			method = m_classElement.getMethod( "set_NDOStateManager" );
			if ( method != null )
				method.remove();
			ILPropertyElement prop = m_classElement.getProperty( "NDOStateManager" );
			if ( prop != null )
				prop.remove();
		}


		public void 
		addStateManager()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOStateManager" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance class [NDO]NDO.IStateManager get_NDOStateManager() cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( loadStateManager());
			method.addStatement( "ret");


			method = m_classElement.getMethod( "set_NDOStateManager" );
			if ( method != null )
			{
				method.clearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual" );
				method.addLine( "instance void  set_NDOStateManager(class [NDO]NDO.IStateManager 'value') cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			string statement = loadStateManager();
			method.addStatement( loadStateManager().Replace("ldfld", "stfld"));
			method.addStatement( "ret");


			ILPropertyElement prop = m_classElement.getProperty( "NDOStateManager" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance class [NDO]NDO.IStateManager NDOStateManager()", m_classElement );
				prop.addElement( new ILCustomElement( ".custom instance void [System]System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", prop ) );
				prop.addElement( new ILCustomElement( ".custom instance void [System.Xml]System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.addElement( new ILGetElement( ".get instance class [NDO]NDO.IStateManager " + m_nonGenericRefName + "::get_NDOStateManager()" ) );
				prop.addElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOStateManager(class [NDO]NDO.IStateManager)" ) );

				m_classElement.getPropertyIterator().getLast().insertAfter( prop );
			}

		}


		private void insertReadForGuidVtPublicField(ILMethodElement method, ILField field, int nr)
		{
			string vtname = field.Parent.ILType;
			if (vtname.StartsWith("valuetype"))
				vtname = vtname.Substring(10);

			method.addStatement("\r\n//insertReadForGuidVtPublicField\r\n");


			method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
			method.addStatement("beq      GuidIsDbNull" + nr.ToString());

			method.addStatement("ldloc.0");
			method.addStatement("isinst     [mscorlib]System.String");
			method.addStatement("brfalse.s  nostr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldflda     valuetype [mscorlib]System.Guid " + vtname + "::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement("castclass  [mscorlib]System.String");
			method.addStatement("call       instance void [mscorlib]System.Guid::.ctor(string)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nostr" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     [mscorlib]System.Guid");
			method.addStatement("brfalse.s  noguid" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement("unbox      [mscorlib]System.Guid");
			method.addStatement("ldobj      [mscorlib]System.Guid");
			method.addStatement("stfld      valuetype [mscorlib]System.Guid " + vtname + "::" + field.Name);
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("noguid" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     unsigned int8[]");
			method.addStatement("brfalse.s  nobytearr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldflda     valuetype [mscorlib]System.Guid " + vtname +"::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement("castclass  unsigned int8[]");
			method.addStatement("call       instance void [mscorlib]System.Guid::.ctor(unsigned int8[])");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nobytearr" + nr.ToString() + ":");
			method.addStatement(@"ldstr      ""Can't convert Guid field to column type {0}""");
			method.addStatement("ldloc.0");
			method.addStatement("callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()");
			method.addStatement("callvirt   instance string [mscorlib]System.Type::get_FullName()");
			method.addStatement("call       string [mscorlib]System.String::Format(string,object)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("GuidIsDbNull" + nr.ToString() + ":");
			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldsfld     valuetype [mscorlib]System.Guid [mscorlib]System.Guid::Empty");
			method.addStatement("stfld      valuetype [mscorlib]System.Guid " + vtname + "::" + field.Name);

			method.addStatement("guidready" + nr.ToString() + ":");
		}

		private void insertReadForGuidVtProperty(ILMethodElement method, ILField field, int nr)
		{
			string vtname = field.Parent.ILType;
			if (vtname.StartsWith("valuetype"))
				vtname = vtname.Substring(10);
			method.addStatement("\r\n//insertReadForGuidVtProperty\r\n");

			method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
			method.addStatement("beq.s      GuidIsDbNull" + nr.ToString());

			method.addStatement("ldloc.0");
			method.addStatement("isinst     [mscorlib]System.String");
			method.addStatement("brfalse.s  nostr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement("castclass  [mscorlib]System.String");
			method.addStatement("newobj     instance void [mscorlib]System.Guid::.ctor(string)");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + "(valuetype [mscorlib]System.Guid)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nostr" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     [mscorlib]System.Guid");
			method.addStatement("brfalse.s  noguid" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement("unbox      [mscorlib]System.Guid");
			method.addStatement("ldobj      [mscorlib]System.Guid");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + "(valuetype [mscorlib]System.Guid)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("noguid" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     unsigned int8[]");
			method.addStatement("brfalse.s  nobytearr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement("castclass  unsigned int8[]");
			method.addStatement("newobj     instance void [mscorlib]System.Guid::.ctor(unsigned int8[])");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + "(valuetype [mscorlib]System.Guid)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nobytearr" + nr.ToString() +":");
			method.addStatement(@"ldstr      ""Can't convert Guid field to column type {0}""");
			method.addStatement("ldloc.0");
			method.addStatement("callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()");
			method.addStatement("callvirt   instance string [mscorlib]System.Type::get_FullName()");
			method.addStatement("call       string [mscorlib]System.String::Format(string,object)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("GuidIsDbNull" + nr.ToString() + ":");
			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldsfld     valuetype [mscorlib]System.Guid [mscorlib]System.Guid::Empty");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + "(valuetype [mscorlib]System.Guid)");

			method.addStatement("guidready" + nr.ToString() +":");
		}


		private void insertReadForGuidMember(ILMethodElement method, ILField field, int nr)
		{
			method.addStatement("\n//insertReadForGuidMember\n");
			//
			method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
			method.addStatement("beq.s      GuidIsDbNull" + nr.ToString());
			method.addStatement("ldloc.0");

			method.addStatement("isinst     [mscorlib]System.String");
			method.addStatement("brfalse.s  nostr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     valuetype [mscorlib]System.Guid " + m_refName + "::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement("castclass  [mscorlib]System.String");
			method.addStatement("call       instance void [mscorlib]System.Guid::.ctor(string)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nostr" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     [mscorlib]System.Guid");
			method.addStatement("brfalse.s  noguid" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldloc.0");
			method.addStatement("unbox      [mscorlib]System.Guid");
			method.addStatement("ldobj      [mscorlib]System.Guid");
			method.addStatement("stfld      valuetype [mscorlib]System.Guid " + m_refName + "::" + field.Name);
			method.addStatement("br.s       guidready" + nr.ToString());
			method.addStatement("noguid" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     unsigned int8[]");
			method.addStatement("brfalse.s  nobytearr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     valuetype [mscorlib]System.Guid " + m_refName + "::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement("castclass  unsigned int8[]");
			method.addStatement("call       instance void [mscorlib]System.Guid::.ctor(unsigned int8[])");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nobytearr" + nr.ToString() + ":");
			method.addStatement(@"ldstr      ""Can't convert Guid field to column type {0}""");
			method.addStatement("ldloc.0");
			method.addStatement("callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()");
			method.addStatement("callvirt   instance string [mscorlib]System.Type::get_FullName()");
			method.addStatement("call       string [mscorlib]System.String::Format(string,object)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("GuidIsDbNull" + nr.ToString() + ":");
			method.addStatement(ldarg_0);
			method.addStatement("ldsfld     valuetype [mscorlib]System.Guid [mscorlib]System.Guid::Empty");
			method.addStatement("stfld      valuetype [mscorlib]System.Guid " + m_refName + "::" + field.Name);


			method.addStatement("guidready" + nr.ToString() + ":");
		}

		private void
			insertReadForGuid(ILMethodElement method, ILField field, int nr, bool parentIsValueType)
		{
			if (!parentIsValueType)
				insertReadForGuidMember(method, field, nr);
			else if (field.IsProperty)
				insertReadForGuidVtProperty(method, field, nr);
			else
				insertReadForGuidVtPublicField(method, field, nr);
		}

		private void
			insertReadForDateTime(ILMethodElement method, ILField field, int nr, bool parentIsValueType)
		{
			method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
			method.addStatement("beq.s      DateTimeIsNull" + nr);
			if (!parentIsValueType)		// Member
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldloc.0");
				method.addStatement("unbox      [mscorlib]System.DateTime");
				method.addStatement("ldobj      [mscorlib]System.DateTime");
				method.addStatement("stfld      valuetype [mscorlib]System.DateTime " + m_refName + "::" + field.Name);
				method.addStatement("br.s       DateTimeReady" + nr);
				method.addStatement("DateTimeIsNull" + nr + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldsfld     valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::MinValue");
				method.addStatement("stfld      valuetype [mscorlib]System.DateTime " + m_refName + "::" + field.Name);
			}
			else if (field.IsProperty)  // Vt with property
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldloc.0");
				method.addStatement("unbox      [mscorlib]System.DateTime");
				method.addStatement("ldobj      [mscorlib]System.DateTime");
				method.addStatement("call       instance void " + field.Parent.PureTypeName + "::" + getAccName("set_", field.Name) + "(valuetype [mscorlib]System.DateTime)");
				method.addStatement("br.s       DateTimeReady" + nr);
				method.addStatement("DateTimeIsNull" + nr + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldsfld     valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::MinValue");
				method.addStatement("call       instance void " + field.Parent.PureTypeName + "::" + getAccName("set_", field.Name) + "(valuetype [mscorlib]System.DateTime)");
			}
			else						// Vt with public field
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldloc.0");
				method.addStatement("unbox      [mscorlib]System.DateTime");
				method.addStatement("ldobj      [mscorlib]System.DateTime");
				method.addStatement("stfld      valuetype [mscorlib]System.DateTime " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement("br.s       DateTimeReady" + nr);
				method.addStatement("DateTimeIsNull" + nr + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldsfld     valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::MinValue");
				method.addStatement("stfld      valuetype [mscorlib]System.DateTime " + field.Parent.PureTypeName + "::" + field.Name);
			}
			method.addStatement("DateTimeReady" + nr + ":");
		}


        private void
        insertReadForGenericType(ILMethodElement method, ILField field, int nr)
        {
            method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
            method.addStatement("ceq");
            method.addStatement("brtrue.s   FieldOver" + nr);
            method.addStatement(ldarg_0);
            method.addStatement("ldloc.0");
            method.addStatement("ldtoken    " + field.ILType);
            method.addStatement("call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)");
            method.addStatement("call       object [NDO]NDO.GenericFieldConverter::FromString(object,class [mscorlib]System.Type)");
            method.addStatement("unbox.any  " + field.ILType);
            method.addStatement("stfld      "+ field.ILType + " " + m_refName + "::" + field.Name);
        }


        private void
            addReadForGuidNullable(ILMethodElement method, ILField field, int nr)
        {
            method.addStatement("isinst     [mscorlib]System.String");
            method.addStatement("brfalse.s  nostring" + nr);
            method.addStatement("ldloc.0");
            method.addStatement("castclass  [mscorlib]System.String");
            method.addStatement("newobj     instance void [mscorlib]System.Guid::.ctor(string)");
            method.addStatement("br.s       guidready" + nr);
            method.addStatement("nostring" + nr + ':');
            method.addStatement("ldloc.0");
            method.addStatement("isinst     [mscorlib]System.Guid");
            method.addStatement("brfalse.s  noguid" + nr);
            method.addStatement("ldloc.0");
            method.addStatement("unbox.any  [mscorlib]System.Guid");
            method.addStatement("br.s       guidready" + nr);
            method.addStatement("noguid" + nr + ':');
            method.addStatement("ldstr      \"Can't convert type \"");
            method.addStatement("ldloc.0");
            method.addStatement("callvirt   instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()");
            method.addStatement("callvirt   instance string [mscorlib]System.Type::get_FullName()");
            method.addStatement("ldstr      \" to field type System.Guid.\"");
            method.addStatement("call       string [mscorlib]System.String::Concat(string,string,string)");
            method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
            method.addStatement("throw");
            method.addStatement("guidready" + nr + ':');
            method.addStatement("newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>::.ctor(!0)");

        }

		private void
			addReadForField(ILMethodElement method, ILField field, int nr)
		{
            Type t = field.FieldType;
            if (t == null)
                throw new InternalException(1917, "Class.cs: Type for field  " + m_refName + "." + field.CleanName + " is null.");
			string prefix;
			bool parentIsValueType = field.Parent != null && field.Parent.IsValueType;
			if (parentIsValueType)
				prefix = field.Parent.CleanName;
			else
				prefix = string.Empty;
			string fname = prefix + field.CleanName;
			string tname = field.ILType;
			//			Testweise Ausgabe der Feldnamen
			//			method.addStatement("ldarg.2");
			//			method.addStatement("  ldarg.3");
			//			method.addStatement("  ldc.i4.s " + nr.ToString());
			//			method.addStatement("  add");
			//			method.addStatement("ldelem.ref");
			//			method.addStatement("call       void [mscorlib]System.Console::WriteLine(string)");

            bool isNullable = false;

#if !NET11
            Type argType = null;
            string argTypeName = string.Empty;
            if (tname.StartsWith("valuetype [mscorlib]System.Nullable`1<"))
            {
                isNullable = true;
                argType = t.GetGenericArguments()[0];
                argTypeName = new ReflectedType(argType, this.m_classElement.getAssemblyName()).QuotedILName;
            }
#endif

			method.addStatement("ldarg.1");
			method.addStatement("ldarg.2");
			method.addStatement("ldarg.3");
			if (nr != 0)
			{
				method.addStatement("ldc.i4.s " + nr.ToString());
				method.addStatement("add");
			}

			method.addStatement("dup");
			method.addStatement("stloc.1");

			method.addStatement("ldelem.ref");
			method.addStatement("callvirt   instance object [System.Data]System.Data.DataRow::get_Item(string)");
			method.addStatement("dup");
			method.addStatement("stloc.0");

			// Special treatment for DateTime and Guid values to map to and from DBNull
			if (tname == "valuetype [mscorlib]System.Guid")
			{
				insertReadForGuid(method, field, nr, parentIsValueType);
				goto theexit;
			}
            else if (tname == "valuetype [mscorlib]System.DateTime")
            {
                insertReadForDateTime(method, field, nr, parentIsValueType);
                goto theexit;
            }
            else if (tname.StartsWith("!"))
            {
                insertReadForGenericType(method, field, nr);
                goto theexit;
            }
            else
            {
                method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
                method.addStatement("beq      FieldNull" + nr.ToString());
            }

			method.addStatement(ldarg_0);
			if (parentIsValueType)
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
			method.addStatement("ldloc.0");
			if (tname == "string" || tname == "unsigned int8[]" || tname == "class [mscorlib]System.String" || tname == "class [mscorlib]System.Byte[]")
				method.addStatement("  castclass  " + tname);
			else if (tname.StartsWith("valuetype"))
			{
				string tname2 = tname.Substring(10);
#if !NET11
                if (isNullable)
                {
                    if (typeof(System.Enum).IsAssignableFrom(argType))
                    {
                        method.addStatement("unbox.any " + argTypeName);
                        method.addStatement("newobj     instance void valuetype [mscorlib]System.Nullable`1<" + argTypeName + ">::.ctor(!0)");
                    }
                    else if (argType == typeof(Guid))
                    {
                        addReadForGuidNullable(method, field, nr);
                    }
                    else
                    {
                        method.addStatement("unbox.any  " + tname);
                    }
                }
                else
                {
#endif
                    method.addStatement("unbox  " + tname2);
                    method.addStatement("ldobj  " + tname2);
#if !NET11
                }
#endif
			}
			else
			{
				if (tname == "unsigned int8")
					method.addStatement("unbox [mscorlib]System.Byte");
				else
					method.addStatement("unbox  " + tname);
				string indType = indTypes[tname] as string;
				if (indType == null)
				{
					messages.ShowError("Don't know how to handle the type of the field '" + m_refName + '.' + fname + "'. Trying to continue enhancing.");
					goto theexit;
				}
				else
					method.addStatement("ldind." + indType);
			}

			if (!parentIsValueType)
			{
				// nicht fname verwenden, weil evtl. die Anführungsstriche gebraucht werden
				method.addStatement("  stfld      " + tname + " " + m_refName+ "::" + field.Name);
			}
			else
			{
				string vtname = field.Parent.ILType;
				if (vtname.StartsWith("valuetype"))
					vtname = vtname.Substring(10);
				if (field.IsProperty)
				{
					string accname = this.getAccName("set_", field.Name);
					method.addStatement("  call       instance void " + vtname + "::" + accname + "(" + field.ILType + ")");
					//					method.addStatement("  call       instance void " + vtname + "::" + accname + "(" + field.ILAsmType + ")");
				}
				else
				{
					method.addStatement("  stfld       " + field.ILType + " " + vtname + "::" + field.Name);
				}
			}
            method.addStatement("br      FieldOver" + nr.ToString());

			theexit:
				method.addStatement("FieldNull" + nr.ToString() + ":");
            #if !NDO11
                if (isNullable)
                {
                    method.addStatement(ldarg_0);
                    method.addStatement("ldflda     valuetype [mscorlib]System.Nullable`1<" + argTypeName + "> " + m_refName + "::" + field.Name);
                    method.addStatement("initobj    valuetype [mscorlib]System.Nullable`1<" + argTypeName + ">");
                }
            #endif
                method.addStatement("FieldOver" + nr.ToString() + ":");

		}

		//
		// addRead
		//
		public void addRead()
		{
			ILMethodElement method = m_classElement.getMethod( "NDORead" );
			if ( method == null )
			{
				method = new ILMethodElement();

				if ( m_hasPersistentBase )
					method.addLine( ".method public hidebysig virtual" );
				else
					method.addLine( ".method public hidebysig newslot virtual" );
				method.addLine( "instance void  NDORead(class [System.Data]System.Data.DataRow dr, string[] fields, int32 startind) cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			else
			{
				method.clearElements();
			}
			method.addStatement(".maxstack  8" );
			addLocalVariable(method, "theObject", "object");
			addLocalVariable(method, "i", "int32");
			addLocalVariable(method, "ex", "class [mscorlib]System.Exception");

			method.addStatement("ldc.i4.0");
			method.addStatement("stloc.1");

			method.addStatement("ldarg.1");
			method.addStatement("brtrue   rownotnull");
			method.addStatement(@"ldstr      ""NDORead: DataRow ist null""");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("rownotnull: ");

			ILStatementElement[] fixupElements = new ILStatementElement[2];

			method.addStatement("ldarg.3");
			fixupElements[0] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.addElement(fixupElements[0]);
			method.addStatement("add");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ble.s      indexbigger");
			method.addStatement(@"ldstr      ""NDORead: Index {0} is bigger than maximum index of fields array ({1})""");
			method.addStatement("ldarg.3");
			fixupElements[1] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.addElement(fixupElements[1]);
			method.addStatement("add");
			method.addStatement("box        [mscorlib]System.Int32");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ldc.i4.1");
			method.addStatement("sub");

			method.addStatement("box        [mscorlib]System.Int32");
			method.addStatement("call       string [mscorlib]System.String::Format(string, object, object)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("indexbigger: ");

			method.addStatement(".try {");
			int nr = 0;

			for(int i = 0; i < this.mappedFieldCount; i++)
			{
				DictionaryEntry e = (DictionaryEntry) sortedFields[i];
				ILField field = (ILField) e.Value;
				if (field.Parent != null && field.Parent.IsEmbeddedType)
					continue;
				addReadForField(method, field, nr);
				nr++;
			}
			fixupElements[0].replaceText("##fieldcount", nr.ToString());
			nr--;
			fixupElements[1].replaceText("##fieldcount", nr.ToString());
			nr++;

			method.addStatement("leave.s    aftercatch");

			method.addStatement("}  // end .try");
			method.addStatement("catch [mscorlib]System.Exception");
			method.addStatement("{");
			method.addStatement("stloc.2");
			method.addStatement(@"ldstr      ""Error while writing to field """);
			method.addStatement("ldarg.2");
			method.addStatement("ldloc.1");
			method.addStatement("ldelem.ref");
			method.addStatement(@"ldstr      ""\n""");
			method.addStatement("ldloc.2");
			method.addStatement("callvirt   instance string [mscorlib]System.Exception::get_Message()");
			method.addStatement("call       string [mscorlib]System.String::Concat(string,string,string,string)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("}  // end handler");

			method.addStatement("aftercatch: ");

			if (m_hasPersistentBase)
			{
				method.addStatement(ldarg_0);
				method.addStatement(ldarg_1);
				method.addStatement("ldarg.2");
				method.addStatement("ldarg.3");
				method.addStatement("ldc.i4.s " + nr.ToString());
				method.addStatement("add");
				method.addStatement("call       instance void " + m_persistentBase + "::NDORead(class [System.Data]System.Data.DataRow dr, string[] fields, int32 startind)");
			}
			method.addStatement("ret");
		}

        private void insertWriteForGenericTypes(ILMethodElement method, ILField field, int nr)
        {
            method.addStatement(ldarg_0);
            method.addStatement("ldfld      " + field.ILType + " " + m_refName + "::" + field.Name);
            method.addStatement("box        " + field.ILType);
            method.addStatement("call       string [NDO]NDO.GenericFieldConverter::ToString(object)");
        }

		private void insertWriteForVtField(ILMethodElement method, ILField field, int nr, bool parentIsValueType, string type)
		{
			string empty = (type == "Guid") ? type + "::Empty" : type + "::MinValue";
			if (!parentIsValueType) // Member
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldfld      valuetype [mscorlib]System." + type + " " + m_refName + "::" + field.Name);
				method.addStatement("ldsfld     valuetype [mscorlib]System." + type + " [mscorlib]System." + empty);
				method.addStatement("call       bool [mscorlib]System." + type + "::op_Inequality(valuetype [mscorlib]System." + type + ", valuetype [mscorlib]System." + type + ")");
				method.addStatement("brtrue.s   VtFieldIsNotEmpty" + nr.ToString());
				method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
				method.addStatement("br.s       VtFieldReady" + nr.ToString());
				method.addStatement("VtFieldIsNotEmpty" + nr.ToString() + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldfld      valuetype [mscorlib]System." + type + " " + m_refName + "::" + field.Name);
				method.addStatement("box        [mscorlib]System." + type + "");
				method.addStatement("VtFieldReady" + nr.ToString() + ":");
			}
			else if (field.IsProperty) // ValueType mit Property
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement("call       instance valuetype [mscorlib]System." + type + " " + field.Parent.PureTypeName + "::" + getAccName("get_", field.Name) + "()");

				method.addStatement("ldsfld     valuetype [mscorlib]System." + type + " [mscorlib]System." + empty);
				method.addStatement("call       bool [mscorlib]System." + type + "::op_Inequality(valuetype [mscorlib]System." + type + ",valuetype [mscorlib]System." + type + ")");
				method.addStatement("brtrue.s   VtFieldIsNotEmpty" + nr.ToString());
				method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
				method.addStatement("br.s       VtFieldReady" + nr.ToString());
				method.addStatement("VtFieldIsNotEmpty" + nr.ToString() + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement("call       instance valuetype [mscorlib]System." + type + " " + field.Parent.PureTypeName + "::" + getAccName("get_", field.Name) + "()");
				method.addStatement("box        [mscorlib]System." + type + "");
				method.addStatement("VtFieldReady" + nr.ToString() + ":");
			}
			else // Vt. mit public field
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement("ldfld      valuetype [mscorlib]System." + type + " " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement("ldsfld     valuetype [mscorlib]System." + type + " [mscorlib]System." + empty);
				method.addStatement("call       bool [mscorlib]System." + type + "::op_Inequality(valuetype [mscorlib]System." + type + ", valuetype [mscorlib]System." + type + ")");
				method.addStatement("brtrue.s   VtFieldIsNotEmpty" + nr.ToString());
				method.addStatement("ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value");
				method.addStatement("br.s       VtFieldReady" + nr.ToString());
				method.addStatement("VtFieldIsNotEmpty" + nr.ToString() + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement("ldfld      valuetype [mscorlib]System." + type + " " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement("box        [mscorlib]System." + type + "");				
				method.addStatement("VtFieldReady" + nr.ToString() + ":");
			}
		}

#if !NET11
		private void insertWriteForNullable(ILMethodElement method, ILField field, int nr, bool parentIsValueType)
		{
			string loadDbNull = "ldsfld     class [mscorlib]System.DBNull [mscorlib]System.DBNull::Value";
			string callInstance = "call       instance bool " + field.ILType + "::";
			Type t = field.FieldType;
			Type argType = t.GetGenericArguments()[0];
			string argTypeName = new ReflectedType(argType, this.m_classElement.getAssemblyName()).QuotedILName;
			if (!parentIsValueType) // Member
			{
				method.addStatement(ldarg_0);
				string ldflda = "ldflda     " + field.ILType + " " + m_refName + "::" + field.Name;
				method.addStatement(ldflda);
				method.addStatement(callInstance + "get_HasValue()");
				method.addStatement("ldc.i4.0");
				method.addStatement("ceq");
				method.addStatement("brtrue.s   NullabeIsEmpty" + nr);
				method.addStatement(ldarg_0);
				method.addStatement(ldflda);
				method.addStatement("call instance !0 " + field.ILType + "::get_Value()");
				method.addStatement("box " + argTypeName);
				method.addStatement("br.s       VtFieldReady" + nr);
			}
			else if (field.IsProperty) // ValueType mit Property
			{
				ILLocalsElement localsElement = method.getLocals();
				string nullableName = "__ndonullable" + nr;
				addLocalVariable(method, nullableName, field.ILType);
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
			    method.addStatement("call       instance " + field.ILType + " " + field.Parent.PureTypeName + "::" + getAccName("get_", field.Name) + "()");
				method.addStatement("stloc " + nullableName);
				method.addStatement("ldloca.s " + nullableName);
				method.addStatement(callInstance + "get_HasValue()");
				method.addStatement("ldc.i4.0");
				method.addStatement("ceq");
				method.addStatement("brtrue.s   NullabeIsEmpty" + nr);
				method.addStatement("ldloca.s " + nullableName);
				method.addStatement("call instance !0 " + field.ILType + "::get_Value()");
				method.addStatement("box " + argTypeName);
				method.addStatement("br.s       VtFieldReady" + nr);
			}
			else // Vt. mit public field
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " "  + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldflda     " + field.ILType + " " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement(callInstance + "get_HasValue()");
				method.addStatement("ldc.i4.0");
				method.addStatement("ceq");
				method.addStatement("brtrue.s   NullabeIsEmpty" + nr);
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldflda     " + field.ILType + " " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement("call instance !0 " + field.ILType + "::get_Value()");
				method.addStatement("box " + argTypeName);
				method.addStatement("br.s       VtFieldReady" + nr);
			}
			method.addStatement("NullabeIsEmpty" + nr + ':');
			method.addStatement(loadDbNull);
			method.addStatement("VtFieldReady" + nr.ToString() + ":");
		}

#endif
		private void 
			addWriteForField(ILMethodElement method, ILField field, int nr)
		{
			string prefix;
			string tname = field.ILType;

			bool parentIsValueType = field.Parent != null && field.Parent.IsValueType;
			if (parentIsValueType)
				prefix = field.Parent.CleanName;
			else
				prefix = string.Empty;
			string fname = prefix + field.CleanName;

			method.addStatement(ldarg_1);
			method.addStatement("ldarg.2");
			method.addStatement("ldarg.3");
			if (nr != 0)
			{
				method.addStatement("ldc.i4.s " + nr.ToString());
				method.addStatement("add");
			}

			method.addStatement("dup");
			method.addStatement("stloc.0");

			method.addStatement("ldelem.ref");

			// Guids and DateTimes have different Write logic, because
			// they map to and from DbNull.
			if (tname == "valuetype [mscorlib]System.Guid")
			{
				insertWriteForVtField(method, field, nr, parentIsValueType, "Guid");
			}
			else if (tname == "valuetype [mscorlib]System.DateTime")
			{
				insertWriteForVtField(method, field, nr, parentIsValueType, "DateTime");
			}
#if !NET11
			else if (tname.StartsWith("valuetype [mscorlib]System.Nullable`1"))
			{
				insertWriteForNullable(method, field, nr, parentIsValueType);
			}
            else if (tname.StartsWith("!"))
            {
                insertWriteForGenericTypes(method, field, nr);
            }
#endif
			else
			{
				method.addStatement(ldarg_0);

				if (!parentIsValueType)
				{
					// Nicht fname verwenden!
					method.addStatement("ldfld      " + field.ILType + " " + m_refName + "::" + field.Name);
				}
				else
				{
					method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
					string vtname = field.Parent.ILType;
					if (vtname.StartsWith("valuetype"))
						vtname = vtname.Substring(10);
					if (field.IsProperty)
					{
						string accname = this.getAccName("get_", field.Name);
						method.addStatement("call       instance " + field.ILType + " " + vtname + "::" + accname + "()");
					}
					else
					{
						method.addStatement("  ldfld       " + field.ILType + " " + vtname + "::" + field.Name);
					}
				}
				if (!(tname == "class [mscorlib]System.String" || tname == "class [mscorlib]System.Byte[]" || tname == "string" || tname == "unsigned int8[]"))
					method.addStatement("box  " + tname);
			}
			method.addStatement("callvirt   instance void [System.Data]System.Data.DataRow::set_Item(string, object)");
		}


		public void
		addWrite()
		{
			ILMethodElement method = m_classElement.getMethod( "NDOWrite" );
			if ( method == null )
			{
				method = new ILMethodElement();

				if ( m_hasPersistentBase )
					method.addLine( ".method public hidebysig virtual" );
				else
					method.addLine( ".method public hidebysig newslot virtual" );
				method.addLine( "instance void  NDOWrite(class [System.Data]System.Data.DataRow dr, string[] fields, int32 startind) cil managed" );
				m_classElement.getMethodIterator().getLast().insertAfter( method );
			}
			else
			{
				method.clearElements();
			}
			method.addStatement( ".maxstack  8" );

			addLocalVariable(method, "i", "int32");
			addLocalVariable(method, "ex", "class [mscorlib]System.Exception");

			method.addStatement("ldc.i4.0");
			method.addStatement("stloc.0");

			method.addStatement("ldarg.1");
			method.addStatement("brtrue.s   rownotnull");
			method.addStatement(@"ldstr      ""NDOWrite: DataRow ist null""");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("rownotnull: ");

			ILStatementElement[] fixupElements = new ILStatementElement[2];
			method.addStatement("ldarg.3");
			fixupElements[0] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.addElement(fixupElements[0]);
			method.addStatement("add");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ble.s      indexbigger");
			method.addStatement(@"ldstr      ""NDOWrite: Index {0} is bigger than maximum index of fields array ({1})""");
			method.addStatement("ldarg.3");
			fixupElements[1] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.addElement(fixupElements[1]);
			method.addStatement("add");
			method.addStatement("box        [mscorlib]System.Int32");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ldc.i4.1");
			method.addStatement("sub");
			method.addStatement("box        [mscorlib]System.Int32");
			method.addStatement("call       string [mscorlib]System.String::Format(string, object, object)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("indexbigger: ");

			method.addStatement(".try {");
 

			int nr = 0;
			// SortedFields enthält auch die ererbten Felder
			// Wir brauchen aber nur die eigenen. In ownFieldsHierarchical
			// sind die eigenen, aber unsortiert
			for(int i = 0; i < this.mappedFieldCount; i++)
			{
				DictionaryEntry e = (DictionaryEntry) sortedFields[i];
				ILField field = (ILField) e.Value;
				if (field.Parent != null && field.Parent.IsEmbeddedType)
					continue;
				addWriteForField(method, field, nr);
				nr++;
			}

			fixupElements[0].replaceText("##fieldcount", nr.ToString());
			nr--;
			fixupElements[1].replaceText("##fieldcount", nr.ToString());
			nr++;

			method.addStatement("leave.s    aftercatch");
			method.addStatement("}  // end .try");
			method.addStatement("catch [mscorlib]System.Exception");
			method.addStatement("{");
			method.addStatement("stloc.1");
			method.addStatement(@"ldstr      ""Error while reading from field """);
			method.addStatement("ldarg.2");
			method.addStatement("ldloc.0");
			method.addStatement("ldelem.ref");
			method.addStatement(@"ldstr      ""\n""");
			method.addStatement("ldloc.1");
			method.addStatement("callvirt   instance string [mscorlib]System.Exception::get_Message()");
			method.addStatement("call       string [mscorlib]System.String::Concat(string,string,string,string)");
			method.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("}  // end handler");

			method.addStatement("aftercatch: ");

			if (m_hasPersistentBase)
			{
				method.addStatement(ldarg_0);
				method.addStatement(ldarg_1);
				method.addStatement("ldarg.2");
				method.addStatement("ldarg.3");
				method.addStatement("ldc.i4.s " + nr.ToString());
				method.addStatement("add");
				method.addStatement("call       instance void " + m_persistentBase + "::NDOWrite(class [System.Data]System.Data.DataRow dr, string[] fields, int32 startind)");
			}

			method.addStatement("ret");
		}

		public void removeGetNDOHandler()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOHandler" );
			if ( method != null )
				method.remove();

			ILPropertyElement propEl = m_classElement.getProperty( "NDOHandler" );
			if ( propEl != null )
				propEl.remove();
		}

		public void addGetNDOHandler()
		{
			ILMethodElement method = m_classElement.getMethod( "get_NDOHandler" );

			m_classElement.insertFieldBefore(".field family static class [NDO]NDO.IPersistenceHandler _ndoPersistenceHandler", m_classElement.getMethodIterator().getNext());

			if ( method == null )
			{
				method = new ILMethodElement();
				method.addLine( ".method public hidebysig newslot specialname final virtual instance class [NDO]NDO.IPersistenceHandler get_NDOHandler() cil managed" );
				m_classElement.addElement( method );
			}
			else
			{
				method.clearElements();
			}
			method.addElement(new ILMaxstackElement(".maxstack  1", method));
			method.addElement(new ILStatementElement("ldsfld     class [NDO]NDO.IPersistenceHandler " + m_refName + "::_ndoPersistenceHandler"));
			method.addElement(new ILStatementElement("ret"));

			method = new ILMethodElement();
			method.addLine(".method public hidebysig static void NDOSetPersistenceHandler(class [NDO]NDO.IPersistenceHandler ph) cil managed");
			method.addElement(new ILMaxstackElement(".maxstack  1", method));
			method.addElement(new ILStatementElement(ldarg_0));
			method.addElement(new ILStatementElement("stsfld     class [NDO]NDO.IPersistenceHandler " + m_refName + "::_ndoPersistenceHandler"));
			method.addElement(new ILStatementElement("ret"));
			m_classElement.addElement(method);

			ILPropertyElement propEl = m_classElement.getProperty( "NDOHandler" );
			if ( propEl == null )
			{
				propEl = new ILPropertyElement();
				propEl.addLine( ".property instance class [NDO]NDO.IPersistenceHandler NDOHandler()" );
				propEl.addElement( new ILCustomElement( ".custom instance void [System]System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", propEl ) );
				propEl.addElement( new ILCustomElement( ".custom instance void [System.Xml]System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", propEl ) );
				propEl.addElement( new ILGetElement( ".get instance class [NDO]NDO.IPersistenceHandler " + m_nonGenericRefName + "::get_NDOHandler()" ) );
				m_classElement.addElement( propEl );
			}
		}



		void addCreateObject(ILClassElement parent)
		{
			ILMethodElement newMethod = new ILMethodElement();
			newMethod.addLine(".method public hidebysig newslot final virtual instance class [NDO]NDO.IPersistenceCapable CreateObject() cil managed");
			newMethod.addElement(new ILMaxstackElement(".maxstack  1", newMethod));
            if (!this.m_classElement.isAbstract())
                newMethod.addElement(new ILStatementElement("newobj     instance void " + m_refName + "::.ctor()"));
            else
                newMethod.addElement(new ILStatementElement("ldnull"));
			newMethod.addElement(new ILStatementElement("ret"));
			parent.addElement(newMethod);
		}

		void addMetaClassCtor(ILClassElement parent)
		{

			ILMethodElement newMethod = new ILMethodElement();
			newMethod.addLine(".method public hidebysig specialname rtspecialname instance void  .ctor() cil managed");
			newMethod.addElement(new ILMaxstackElement(".maxstack  1", newMethod));
			newMethod.addElement(new ILStatementElement(ldarg_0));
			newMethod.addElement(new ILStatementElement("call       instance void [mscorlib]System.Object::.ctor()"));
			newMethod.addElement(new ILStatementElement("ret"));
			parent.addElement(newMethod);
		}


		public void addMetaClass()
		{
			ILClassElement newClass = new ILClassElement();
			newClass.addLine(".class auto ansi nested private beforefieldinit MetaClass " + this.m_classElement.getGenericArguments());
			newClass.addLine("extends [mscorlib]System.Object");
			newClass.addLine("implements [NDO]NDO.IMetaClass");
			m_classElement.addElement(newClass);
#if NET11
			newClass.setForwardClassElement(addNestedMetaClass());
#endif
			addMetaClassCtor(newClass);
			addCreateObject(newClass);
			addGetOrdinal(newClass);
		}


		private void 
			addForeignQueryHelperAccessor(ILClassElement newClass, ILReference r, string qhTypeName)
		{
			string refType = QuotedName.ConvertTypename(r.RefType);
			ILMethodElement method = new ILMethodElement();
			method.addLine(".method public hidebysig specialname");
			method.addLine("instance class " + refType + "/QueryHelper");
			string propName = getAccName("get_", r.Name);
			method.addLine(propName + "() cil managed");
			method.addStatement(".maxstack  3");
			method.addStatement(ldarg_0);
			method.addStatement("ldfld      class " + refType + "/QueryHelper " + qhTypeName + "::" + getAccName("__", r.Name));
			method.addStatement("brtrue.s   IL_0023");
			method.addStatement(ldarg_0);
			method.addStatement(ldarg_0);

			method.addStatement("ldfld      string "+ qhTypeName + "::__ndoqhname");
			method.addStatement(@"ldstr      """ + r.CleanName + @".""");
			method.addStatement("call       string [mscorlib]System.String::Concat(string,string)");
			method.addStatement("newobj     instance void " + refType + "/QueryHelper::.ctor(string)");
			method.addStatement("stfld      class " + refType + "/QueryHelper " + qhTypeName + "::" + getAccName("__", r.Name));
			method.addStatement("IL_0023: " + ldarg_0);
			method.addStatement("ldfld      class " + refType + "/QueryHelper " + qhTypeName + "::" + getAccName("__", r.Name));
			method.addStatement("ret");
			newClass.addElement(method);

			ILPropertyElement property = new ILPropertyElement();
			property.addLine(".property instance class " + refType + "/QueryHelper " + r.Name + "()");
            string nonGenericQhTypeName = refType;
            int p = nonGenericQhTypeName.IndexOf('<');
            if (p > 1)
                nonGenericQhTypeName = nonGenericQhTypeName.Substring(0, p);
            if (nonGenericQhTypeName.StartsWith("valuetype "))
                nonGenericQhTypeName = nonGenericQhTypeName.Substring(10);
            property.addElement(new ILGetElement(".get instance class " + nonGenericQhTypeName + "/QueryHelper " + qhTypeName + "::" + getAccName("get_", r.Name) + "()"));
            newClass.addElement(property);
		}

		private void 
			addQHelperInitEntry(ILMethodElement initQhelper, ILReference r)
		{
			string refType = QuotedName.ConvertTypename(r.RefType);
			initQhelper.addStatement(ldarg_0);
			initQhelper.addStatement("ldnull");
			initQhelper.addStatement("stfld      class " + refType + "/QueryHelper " + m_refName + "/QueryHelper::" + getAccName("__", r.Name));
		}

		private void
			addQHelperGetFieldProperty(ILClassElement newClass, ILField f, string qhTypeName)
		{
			ILMethodElement method = new ILMethodElement();
			method.addLine(".method public hidebysig specialname");
			string propName = getAccName("get_", f.Name);
			method.addLine("instance string  " + propName + "() cil managed");
			method.addStatement(".maxstack  2");
			method.addStatement(ldarg_0);
			method.addStatement("ldfld      string " + qhTypeName + "::__ndoqhname");
			method.addStatement(@"ldstr      """ + f.CleanName + @"""");
			method.addStatement("call       string [mscorlib]System.String::Concat(string,string)");
			method.addStatement("ret");
			newClass.addElement(method);

			ILPropertyElement property = new ILPropertyElement();
			property.addLine(".property instance string " + f.Name + "()");
            string nonGenericQhTypeName = qhTypeName;
            int p = nonGenericQhTypeName.IndexOf('<');
            if (p > -1)
                nonGenericQhTypeName = nonGenericQhTypeName.Substring(0, p);
            if (nonGenericQhTypeName.StartsWith("class "))
                nonGenericQhTypeName = nonGenericQhTypeName.Substring(6);
            property.addElement(new ILGetElement(".get instance string " + nonGenericQhTypeName + "::" + propName + "()"));
            newClass.addElement(property);
		}

		private void
		addValueTypeAccessor(ILClassElement valType, ILField subField)
		{
			string vtFullName = m_name + "/QueryHelper/" + valType.getName();
			ILMethodElement methEl = new ILMethodElement();
			methEl.addLine(".method public hidebysig specialname ");
			string accName = this.getAccName("get_", subField.Name);
			methEl.addLine("instance string  " + accName + "() cil managed");
			methEl.addStatement(".maxstack  3");
			methEl.addStatement(ldarg_0);
			methEl.addStatement("ldfld      string " + vtFullName + "::__ndoqhname");
			methEl.addStatement(@"ldstr """ + subField.CleanName + @"""");
			methEl.addStatement("call       string [mscorlib]System.String::Concat(string, string)");
			methEl.addStatement("ret");
			valType.addElement(methEl);

			ILPropertyElement propEl = new ILPropertyElement();
			propEl.addLine(".property instance string " + subField.Name + "()");
			propEl.addElement(new ILGetElement(".get instance string " + vtFullName + "::" + accName + "()"));
            valType.addElement(propEl);
		}

        private string getGenericRefParameters()
        {
            int p = m_refName.IndexOf('<');
            if (p > -1)
            {
                return m_refName.Substring(p);
            }
            return string.Empty;
        }

#if NET11
		private void
			addQueryHelperElements(ILClassElement newClass, ILClassElement fwdQhElement)
#else
        private void
            addQueryHelperElements(ILClassElement newClass)
#endif
		{
            string outerTypeName = m_name;
            int p = m_name.IndexOf('<');
            if (p > -1)
                outerTypeName = "class " + m_name.Substring(0, p);
            string qhTypeName = outerTypeName + "/QueryHelper";
                qhTypeName += getGenericRefParameters();

			newClass.addElement(new ILStatementElement(".field private string __ndoqhname"));

			// initQhelper setzt alle Query-Helpers auf null
			ILMethodElement initQhelper = new ILMethodElement();
			initQhelper.addLine(".method private hidebysig instance void");
			initQhelper.addLine("InitQhelper() cil managed");
			initQhelper.addStatement(".maxstack 2");
			newClass.addElement(initQhelper);

			// ToString()
			ILMethodElement toString = new ILMethodElement();
			toString.addLine(".method public hidebysig virtual instance string ToString() cil managed");
			toString.addStatement(".maxstack  4");
			addLocalVariable(toString, "CS$00000003$00000000", "string");

			toString.addStatement(ldarg_0);
			toString.addStatement("ldfld      string " + qhTypeName + "::__ndoqhname");

			toString.addStatement("ldsfld     string [mscorlib]System.String::Empty");
			toString.addStatement("call       bool [mscorlib]System.String::op_Equality(string, string)");
			toString.addStatement("brfalse.s  notEmpty");
			toString.addStatement("ldsfld     string [mscorlib]System.String::Empty");
			toString.addStatement("stloc.0");
			toString.addStatement("br.s       exit");
			toString.addStatement("notEmpty:  ldarg.0");
			toString.addStatement("ldfld      string " + qhTypeName + "::__ndoqhname");
			toString.addStatement("ldc.i4.0");
			toString.addStatement(ldarg_0);
			toString.addStatement("ldfld      string " + qhTypeName + "::__ndoqhname");
			toString.addStatement("callvirt   instance int32 [mscorlib]System.String::get_Length()");
			toString.addStatement("ldc.i4.1");
			toString.addStatement("sub");
			toString.addStatement("callvirt   instance string [mscorlib]System.String::Substring(int32,int32)");
			toString.addStatement("stloc.0");
			toString.addStatement("exit:  ldloc.0");
			toString.addStatement("ret");
			newClass.addElement(toString);



			// Zwei Konstruktoren, in denen der Namensprefix gesetzt wird
			// Entweder leer (Standard-Konstruktor) oder mit String ("prefix.fieldname")
			ILMethodElement strCtor = new ILMethodElement();
			ILMethodElement emptyCtor = new ILMethodElement();
			strCtor.addLine(".method public hidebysig specialname rtspecialname");
			strCtor.addLine("instance void  .ctor(string n) cil managed");
			strCtor.addStatement(".maxstack  4");
			strCtor.addStatement(ldarg_0);
			string s1 = "call       instance void [mscorlib]System.Object::.ctor()";
			string s2 = "call       instance void " + qhTypeName + "::InitQhelper()";
			strCtor.addStatement(s1);
			strCtor.addStatement(ldarg_0);
			strCtor.addStatement(s2);
			strCtor.addStatement(ldarg_0);
			strCtor.addStatement(ldarg_1);
			strCtor.addStatement("stfld      string " + qhTypeName + "::__ndoqhname");

			emptyCtor.addLine(".method public hidebysig specialname rtspecialname");
			emptyCtor.addLine("instance void  .ctor() cil managed");
			emptyCtor.addStatement(".maxstack  3");
			emptyCtor.addStatement(ldarg_0);
			emptyCtor.addStatement(s1);
			emptyCtor.addStatement(ldarg_0);
			emptyCtor.addStatement(s2);
			emptyCtor.addStatement(ldarg_0);
			emptyCtor.addStatement("ldsfld     string [mscorlib]System.String::Empty");
			emptyCtor.addStatement("stfld      string " + qhTypeName + "::__ndoqhname");

			newClass.addElement(strCtor);
			newClass.addElement(emptyCtor);

			ILField oidf = new ILField(typeof(int), "int32", "oid", this.m_classElement.getAssemblyName(), null);  // Der Typ ist in diesem Fall unerheblich
			addQHelperGetFieldProperty(newClass, oidf, qhTypeName);

			// Wir bauen hier eine hierarchische Ordnung der Felder auf,
			// wie sie in ownFieldsHierarchical vorliegt. Dort sind aber nur
			// die eigenen Felder berücksichtigt, wir benötigen hier aber auch
			// die ererbten Felder.
			ArrayList addedFields = new ArrayList();

			foreach (DictionaryEntry de in this.sortedFields)
			{
				ILField f = (ILField) de.Value;
				// Wir können hier nur die Parents brauchen, die kommen
				// aber für jedes Child einmal vor
				if (f.Parent != null)
				{
					f = f.Parent;
					if (addedFields.Contains(f.CleanName))
						continue;
					addedFields.Add(f.CleanName);
				}
					
				//messages.WriteLine(f.CleanName);
				if (! f.HasNestedFields) // direkt speicherbare Typen
				{
					addQHelperGetFieldProperty(newClass, f, qhTypeName);
				}
				else // Value Types und Embedded Types
				{
					// Für die Subfelder eines ValueTypes legen wir einen ValueType an,
					// der diese Felder repräsentiert.
					ILClassElement valType = new ILClassElement();
					System.Random r = new System.Random();
					string vtName = this.getAccName("ndo" + r.Next().ToString(), f.Name);					
					string vtFullName = m_name + "/QueryHelper/" + vtName;
					valType.addLine(".class sequential ansi sealed nested public beforefieldinit " + vtName);
					valType.addLine("extends [mscorlib]System.ValueType");

					// Konstruktor für den ValueType
					ILMethodElement vtCtor = new ILMethodElement();
					vtCtor.addLine(".method public hidebysig specialname rtspecialname instance void  .ctor(string name) cil managed");
					valType.addElement(vtCtor);
					vtCtor.addStatement(ldarg_0);
					vtCtor.addStatement(ldarg_1);
					vtCtor.addStatement("stfld      string " + vtFullName + "::__ndoqhname");
					vtCtor.addStatement("ret");
					ILMethodElement vtToString = new ILMethodElement();
					valType.addElement(vtToString);
					vtToString.addLine(".method public hidebysig virtual instance string ");
					vtToString.addLine("ToString() cil managed");
					vtToString.addStatement(".maxstack  3");
					vtToString.addStatement(@"ldstr      ""Wrong query helper argument: """);
					vtToString.addStatement(ldarg_0);
					vtToString.addStatement("ldfld      string " + vtFullName + "::__ndoqhname");
					vtToString.addStatement(@"ldstr      "" Use members of that type instead.""");
					vtToString.addStatement("call       string [mscorlib]System.String::Concat(string, string, string)");
					vtToString.addStatement("newobj     instance void [mscorlib]System.Exception::.ctor(string)");
					vtToString.addStatement("throw");

					// Aufruf des Konstruktors des Value Types
					strCtor.addStatement(ldarg_0);
					strCtor.addStatement("ldflda     valuetype " + vtFullName + " " + qhTypeName + "::" + f.Name);
					strCtor.addStatement(ldarg_1);
					strCtor.addStatement(@"ldstr     """ + f.CleanName + "." + @"""");
					strCtor.addStatement("call       string [mscorlib]System.String::Concat(string,string)");
					strCtor.addStatement("call       instance void " + vtFullName + "::.ctor(string)");
					emptyCtor.addStatement(ldarg_0);
					emptyCtor.addStatement("ldflda     valuetype " + vtFullName + " " + qhTypeName + "::" + f.Name);
					emptyCtor.addStatement(@"ldstr     """ + f.CleanName + "." + @"""");
					emptyCtor.addStatement("call       instance void " + vtFullName + "::.ctor(string)");

					valType.addElement(new ILFieldElement(".field private string __ndoqhname"));
#if NET11
					// Vorwärtsdeklaration für den Value Type
					ILClassElement fwdValType = new ILClassElement();
					fwdValType.addLine(".class sequential ansi sealed nested public beforefieldinit " + vtName);
					fwdValType.addLine("extends [mscorlib]System.ValueType");
					fwdQhElement.addElement(fwdValType);
#endif
					newClass.addElement(new ILFieldElement(".field public valuetype " + vtFullName + " " + f.Name));

					foreach(ILField subField in f.Fields)
						addValueTypeAccessor(valType, subField);

					newClass.addElement(valType);
				}
			}

			foreach (ILReference r in this.m_references)
			{
				this.addQHelperInitEntry(initQhelper, r);
				newClass.addElement(new ILFieldElement(".field private class " + QuotedName.ConvertTypename(r.RefType) + "/QueryHelper " + getAccName("__", r.Name)));
				addForeignQueryHelperAccessor(newClass, r, qhTypeName);
			}

			strCtor.addStatement("ret");
			emptyCtor.addStatement("ret");

			initQhelper.addStatement("ret");
		}

		public void
			addQueryHelper()
		{
			ILClassElement nestedClass = new ILClassElement();
#if NET11
			nestedClass.setForwardClassElement(addQueryHelperForwardDecl());
#endif
			nestedClass.addLine(".class auto ansi nested public beforefieldinit QueryHelper " + m_classElement.getGenericArguments());
			nestedClass.addLine("extends [mscorlib]System.Object");
			m_classElement.addElement(nestedClass);
#if NET11
			addQueryHelperElements(nestedClass, nestedClass.getForwardClassElement());
#else
            addQueryHelperElements(nestedClass);
#endif
		}

	}  // internal class Class


	/// <summary>
	/// Die Klasse repräsentiert ein persistentes Feld
	/// </summary>
	internal class ILField : IComparable
	{
		protected string		m_name;
		protected string		m_ilType;
		private bool			isValueType;
        private string assemblyName;
        public ArrayList Fields;
		private ILField parent;
		private bool isProperty = false;
		string pureTypeName;
		bool valid = true;
		private bool isEmbeddedType = false;
		private bool isEnum = false;
		private IList embeddedFieldList;
		protected bool isInherited = false;
		string m_ilTypeWithoutPrefix;
		const string classPrefix = "class ";
		const string vtPrefix = "valuetype ";
		protected string declaringType;
		private Type fieldType;

		public ILField( Type type, string iltype, string name, string assemblyName, string declaringType )
		{
			Init(type, iltype, name, declaringType, assemblyName);
		}

        public ILField(Type type, string iltype, string name, string assemblyName, IList embeddedFieldList, bool isEnum)
		{
			this.isEnum = isEnum;
			isEmbeddedType = embeddedFieldList != null;
			this.embeddedFieldList = embeddedFieldList;
			Init(type, iltype, name, null, assemblyName);
		}

        public ILField(Type type, string iltype, string name, string assemblyName, ILField parent, bool isProperty, bool isEnum)
		{
			this.isProperty = isProperty;
			Init(type, iltype, name, null, assemblyName);
			this.parent = parent;
			this.isEnum = isEnum;
		}

		/// <summary>
		/// Erzeuge die strings m_ilType und pureTypeName
		/// </summary>
		/// <param name="ilTypeName">Typstring, der als Konstruktorparameter kommt</param>
		private void PrepareType(string ilTypeName)
		{
            ReflectedType rt = new ReflectedType(this.fieldType, this.assemblyName);
            string tname = rt.ILName;
            if (this.isBuiltInType(tname))
            {
                pureTypeName = m_ilType = tname;
                return;
            }
            m_ilType = rt.QuotedILName;
            pureTypeName = m_ilType.Substring(m_ilType.IndexOf("]") + 1);
            if (pureTypeName.StartsWith("valuetype"))
                pureTypeName = pureTypeName.Substring(10);

            if (m_ilType.StartsWith(vtPrefix))
                m_ilTypeWithoutPrefix = m_ilType.Substring(vtPrefix.Length);
            else if (m_ilType.StartsWith(classPrefix))
                m_ilTypeWithoutPrefix = m_ilType.Substring(classPrefix.Length);
            else
                m_ilTypeWithoutPrefix = m_ilType;

            //Regex regex = new Regex(@"(class\s|valuetype\s|)(\[[^\]]+\]|)([^<]+)(<[^>]+>|)$");
            //Match match = regex.Match(ilTypeName);
#if nix
			if (isBuiltInType(ilTypeName))
			{
				pureTypeName = m_ilType = ilTypeName;
				return;
			}
			if (isEmbeddedType)
			{
				if (!ilTypeName.StartsWith(classPrefix))
					ilTypeName = classPrefix + ilTypeName;
			}
			string tempName;
			string prefix;
			bool isArray = false;

			if (ilTypeName.StartsWith(vtPrefix))
			{
				tempName = ilTypeName.Substring(10);
				prefix = vtPrefix;
				isValueType = true;  
				this.Fields = new ArrayList();
			}
			else if (ilTypeName.StartsWith(classPrefix))
			{
				tempName = ilTypeName.Substring(6);
				prefix = classPrefix;
			}
			else
			{
				tempName = ilTypeName;
				prefix = string.Empty;
			}
			if (tempName.EndsWith("[]"))
			{
				isArray = true;
				tempName = tempName.Substring(0, tempName.Length - 2);
			}

			int p = tempName.IndexOf("]") + 1;
			string dottedName = UmlautName.Convert(tempName.Substring(p));
			if (isArray)
				dottedName += "[]";
			string ilType = ILFromType(dottedName);
			// Hier können nochmal eingebaute Typen entstehen.
			if (this.isBuiltInType(ilType))
			{
//				this.isValueType = false;  // Mirko 23.9.
//				this.Fields = null;
				pureTypeName = m_ilType = ilType;
				return;
			}
//			else
//			{
//				if (!prefix.StartsWith("v"))
//					valid = false;
//			}
			m_ilType = prefix + tempName.Substring(0, p) + dottedName;
			pureTypeName = m_ilType.Substring(m_ilType.IndexOf("]") + 1);
			if (pureTypeName.StartsWith("valuetype"))
				pureTypeName = pureTypeName.Substring(10);
#endif
		}
		private void Init( Type type, string iltype, string name, string declaringType, string assemblyName )
		{
			this.fieldType = type;
			this.declaringType = declaringType;
            this.assemblyName = assemblyName;

			// We quote all names. It makes no difference in the metadata.
			if (!name.StartsWith("'"))
				m_name = '\'' + name + '\'';
			else
				m_name = name;

			parent = null;
			string quotedName;
			PrepareType(iltype);
#if nix
			if (m_ilType.StartsWith(vtPrefix))
				m_ilTypeWithoutPrefix = m_ilType.Substring(vtPrefix.Length);
			else if (m_ilType.StartsWith(classPrefix))
				m_ilTypeWithoutPrefix = m_ilType.Substring(classPrefix.Length);
			else
				m_ilTypeWithoutPrefix = m_ilType;
#endif
			if (isEmbeddedType)
			{
				Fields = new ArrayList();
				foreach(FieldNode fieldNode in embeddedFieldList)
				{
					quotedName = QuotedName.ConvertTypename(fieldNode.Name);
					Fields.Add(new ILField(fieldNode.FieldType, ILFromType(fieldNode.DataType), quotedName, assemblyName, this, false, fieldNode.IsEnum));
				}
			}
			else if (type.IsValueType && !StorableTypes.Contains(type))
			{
				ValueTypeNode vtn = ValueTypes.Instance[pureTypeName];
				if (null != vtn)
				{
					isValueType = true;
					Fields = new ArrayList();
					if (vtn.Fields.Count == 0)
						new MessageAdapter().WriteLine("Warning: Mapped value type " + type.FullName + " doesn't have any public member to store.");
					foreach(FieldNode fn in vtn.Fields)
					{
						quotedName = QuotedName.ConvertTypename(fn.Name);
						Fields.Add(new ILField(fn.FieldType, fn.DataType, quotedName, this.assemblyName, this, fn.IsProperty, fn.IsEnum));
					}
				}
			}
		}

		public bool IsEmbeddedType
		{
			get { return isEmbeddedType; }
			set { isEmbeddedType = value; }
		}

		public bool HasNestedFields
		{
			get { return !IsEnum && (IsEmbeddedType || ( IsValueType && Fields != null && Fields.Count > 0)); }
		}

		public bool IsEnum
		{
			get { return isEnum; }
		}

		public bool IsProtected
		{
			get { return declaringType != null; }
		}

		public bool Valid
		{
			get { return valid; }
			set { valid = value; }
		}

		public Type FieldType
		{
			get { return fieldType; }
		}

		public string PureTypeName
		{
		    get { return pureTypeName; }
		    set { pureTypeName = value; }
		}

		public bool IsProperty
		{
		    get { return isProperty; }
		}

		public string
		CleanName
		{
			get{ return m_name.Replace("'", string.Empty); }
		}

		public string
		Name
		{
			get{ return m_name; }
		}

		public ILField Parent
		{
			get { return parent; }
		}


		public bool IsValueType
		{
			get { return isValueType; }
		}

		public virtual bool IsInherited
		{
			get 
			{
				if (this.parent != null) return parent.IsInherited;
				return isInherited;
			}
			set { isInherited = value; }
		}


		public string
		ILType
		{
			get { return m_ilType; }
		}

		public string
		ILTypeWithoutPrefix
		{
			get { return m_ilTypeWithoutPrefix; }
		}

		public string
		CsType
		{
			get { return typeFromIL(m_ilType); }
		}

//		public string
//		ILAsmType
//		{
//			get { return ILFromType(m_ilType); }
//		}

		protected bool isBuiltInType(string typeName)
		{
			typeName = typeName.Trim();

			if ( typeName == "bool" )
				return true;
			if ( typeName == "byte" )
				return true;
			if ( typeName == "sbyte" )
				return true;
			if ( typeName == "char" )
				return true;
			if ( typeName == "unsigned char" )
				return true;
			if ( typeName == "short" || typeName == "int16" )
				return true;
			if ( typeName == "unsigned int16" )
				return true;
			if ( typeName == "unsigned int8" )
				return true;
			if ( typeName == "unsigned int8[]" )
				return true;
			if ( typeName == "int" || typeName == "int32" )
				return true;
			if ( typeName == "unsigned int32" )
				return true;
			if ( typeName == "long" || typeName == "int64" )
				return true;
			if ( typeName == "unsigned int64" )
				return true;
			if ( typeName == "float32" || typeName == "float" || typeName == "single" )
				return true;
			if ( typeName == "float64" || typeName == "double" )
				return true;
			if ( typeName == "string" )
				return true;
			else 
				return false;
		}

		protected string
		typeFromIL( string typeName )
		{
			typeName = typeName.Trim();
			Regex regex = new Regex("System.Nullable`1<(.*)>");
			Match match = regex.Match(typeName);
			if (match.Success)
				typeName = match.Groups[1].Value;

			if ( typeName == "bool" )
				return "System.Boolean";
			else if ( typeName == "byte" )
				return "System.Byte";
			else if ( typeName == "sbyte" )
				return "System.SByte";
			else if ( typeName == "char" )
				return "System.Char";
			else if ( typeName == "unsigned char" )
				return "System.UChar";
			else if ( typeName == "short" || typeName == "int16" )
				return "System.Int16";
			else if ( typeName == "unsigned int16" )
				return "System.UInt16";
			else if ( typeName == "unsigned int8" )
				return "System.Byte";
			else if ( typeName == "unsigned int8[]" )
				return "System.Byte[]";
			else if ( typeName == "int" || typeName == "int32" )
				return "System.Int32";
			else if ( typeName == "unsigned int32" )
				return "System.UInt32";
			else if ( typeName == "long" || typeName == "int64" )
				return "System.Int64";
			else if ( typeName == "unsigned int64" )
				return "System.UInt64";
			else if ( typeName == "float32" || typeName == "float" || typeName == "single" )
				return "System.Single";
			else if ( typeName == "float64" || typeName == "double" )
				return "System.Double";
			else if ( typeName == "string" )
				return "System.String";
			else 
			{
				string tn = typeName;
				if (tn.StartsWith(vtPrefix)) 
					tn = tn.Substring(10);
				else if (tn.StartsWith(classPrefix)) 
					tn = tn.Substring(6);
				tn = tn.Trim();
				if (tn.StartsWith("[mscorlib]"))
					tn = tn.Substring(10).Trim();
				if (!tn.StartsWith("["))
					return tn;
				tn = tn.Substring(1);
				int pos = tn.IndexOf("]");
				return (tn.Substring(pos + 1) + ", " + tn.Substring(0, pos));
			}
		}

		private string
		ILFromType( string tName )
		{
			bool isArray = false;
			string tempName;
			if (tName.EndsWith("[]"))
			{
				isArray = true;
				tempName = tName.Substring(0, tName.Length -2);
			}
			else
				tempName = tName;

			string typeName = tempName.Substring(tempName.IndexOf("]") + 1).Trim();

			if (isArray)
				typeName += "[]";

			if ( typeName == "System.String" )
				return "string";
			else if ( typeName == "System.Int32" )
				return "int32";
			else if ( typeName == "System.Boolean" )
				return "bool";
			else if ( typeName == "System.Byte")
				return "unsigned int8";
			else if ( typeName == "System.SByte")
				return "int8";
			else if ( typeName == "System.Byte[]")
				return "unsigned int8[]";
			else if ( typeName == "System.SByte" )
				return "sbyte";
			else if ( typeName == "System.Char" )
				return "char";
			else if ( typeName == "System.UChar" )
				return "unsigned char";
			else if ( typeName == "System.Int16" )
				return "int16";
			else if ( typeName == "System.UInt16" )
				return "unsigned int16";
			else if ( typeName == "System.UInt32" )
				return "unsigned int32";
			else if ( typeName == "System.Int64" )
				return "int64";
			else if ( typeName == "System.UInt64" )
				return "unsigned int64";
			else if ( typeName == "System.Single" )
				return "float32";  
			else if ( typeName == "System.Double" )
				return "float64";
			else return typeName;
		}

		// Implementation of IComparable
		public int CompareTo(object obj)
		{
			return this.m_name.CompareTo(((ILField)obj).m_name);
		}

	}

	internal class ILReference : ILField
	{
		string referencedType;
		RelationInfo relInfo;
		string relationName;
		bool is1to1;
		int ordinal;

		public ILReference (Type fieldType, string refType, string ilType, string fieldName, string assemblyName, RelationInfo ri, string relName, bool isInherited, bool is1to1, string declaringType) 
			: base (fieldType, ilType, fieldName, assemblyName, declaringType)
		{
			this.is1to1 = is1to1;
			this.isInherited = isInherited;
			if (null != refType)
				referencedType = refType;
			else
			{
				referencedType = CsType;
				if (referencedType.StartsWith("class"))
					referencedType = referencedType.Substring(6);
				if (referencedType.StartsWith("valuetype"))
					referencedType = referencedType.Substring(10);
			}

			relInfo = ri;
			if (relName != null)
				relationName = relName;
			else
				relationName = "";
		}

		public int Ordinal
		{
			get { return ordinal; }
			set { ordinal = value; }
		}


		public override bool IsInherited
		{
			get { return isInherited; }
			set { isInherited = value; }
		}

		public RelationInfo ReferenceInfo
		{
			get { return relInfo; }
		}

		public string RelationName
		{
			get { return relationName; }
		}

		public bool Is1to1
		{
			get { return (is1to1); }
		}

		public string RefTypeShortName
		{
			get 
			{ 
				int pos = referencedType.IndexOf("]");
				// im Fehlerfall ist pos -1, dann wird der ganze String kopiert
				return (referencedType.Substring(pos + 1)).Replace("'", string.Empty);
			}
		}

		public string RefType
		{
			get 
			{ 
				return referencedType;
			}
		}
	}



}
