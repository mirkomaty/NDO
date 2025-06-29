﻿//
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
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using NDOEnhancer.ILCode;
using NDO;
using NDO.Mapping;

namespace NDOEnhancer.Patcher
{
	/// <summary>
	/// Summary description for Class.
	/// </summary>
	internal class ClassPatcher
	{
		static ClassPatcher()
		{
			indTypes = new Dictionary<string,string>(12);
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
			NDOMapping mappings, 
			ClassDictionary<ClassNode> externalPersistentBases,
			MessageAdapter messages,
			IEnumerable<KeyValuePair<string,ILField>> sortedFields,
			IEnumerable<ILReference> references,
			string oidTypeName)
		{
			m_classElement			= classElement;
			m_name					= classElement.ClassFullName;
            m_refName               = makeRefName();
            m_nonGenericRefName     = m_name;
			m_references			= references;

			if (references == null)
				throw new ArgumentNullException( "references" );

            int p = m_nonGenericRefName.IndexOf('<');
            if (p > -1)
                m_nonGenericRefName = m_nonGenericRefName.Substring(0, p);

			m_persistentBase		= classElement.BaseFullName;
			this.externalPersistentBases = externalPersistentBases;

			if (null != externalPersistentBases)
			{
				//m_hasPersistentBase	= classElement.hasPersistentBase(temp, typeof(NDOPersistentAttribute));
				m_hasPersistentBase	= (externalPersistentBases.ContainsKey(m_persistentBase));
			}

			this.m_mappings				= mappings;
			this.m_classMapping			= mappings.FindClass(classElement.MappingName);

			if (this.m_classMapping == null)
                throw new Exception("Can't find mapping for class " + classElement.MappingName);

			this.messages			= messages;
			this.sortedFields = sortedFields;
			this.oidTypeName = oidTypeName;

			int i = 0;
			foreach (var reference in m_references)
				reference.Ordinal = i++;


			// sortedFields ist eine flache Ansicht auf die Felder, wie 
			// sie in der Datenbank sein werden.
			// Wir benötigen aber auch die hierarchische Sicht, die die 
			// Namen der ValueTypes und embedded Objects liefert.
			this.ownFieldsHierarchical = GetHierarchicalFieldList();

			if (null != externalPersistentBases)
			{
				checkPersistentRoot();
				checkNextPersistentBase();
			}
		}

		static private Dictionary<string,string> indTypes;

		const string ldarg_0 = "ldarg.0";
		const string ldarg_1 = "ldarg.1";
		private MessageAdapter			messages;
		private ILClassElement			m_classElement;
		private string					m_name;
		private string					m_refName;
		private string					m_nonGenericRefName;
		private string					m_persistentBase;
		private bool					m_hasPersistentBase;
		private NDOMapping				m_mappings;
		private string					oidTypeName;
		private Class					m_classMapping;

		private List<ILField>				ownFieldsHierarchical	= new List<ILField>();
		private IEnumerable<ILReference>		m_references;
		private HashSet<ILMethodElement>		dirtyDone = new HashSet<ILMethodElement>();
		private ClassDictionary<ClassNode>		externalPersistentBases;
		private string					persistentRoot = null;
		private IEnumerable<KeyValuePair<string,ILField>>	sortedFields;
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
			if (baseClass.AssemblyName != this.m_classElement.AssemblyName)
				m_persistentBase = "[" + baseClass.AssemblyName + "]" + baseClass.Name;
			else
				m_persistentBase = baseClass.Name;
		}

		private List<ILField> GetHierarchicalFieldList()
		{
			this.mappedFieldCount = 0;  // count of the flat fields
			var fields = new List<ILField>();

			if (sortedFields != null)
			{
                // Sorted fields is an array of DictionaryEntries
				foreach (var e in sortedFields)
				{
					var field = e.Value;
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
					string className = classEl.MappingName;
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
			addRelationAttributes();
			addFieldsAndOidAttribute();
			replaceLdfldAndStfld();
			MarkRelations();
			addFieldAccessors();
			patchConstructor();
			addMethods();
		}


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
			if (  m_hasPersistentBase )
				return;
			
			m_classElement.AppendField( ".field family class [NDO]NDO.LoadState _ndoLoadState" );
			m_classElement.AppendField( ".field family valuetype [NDO]NDO.NDOObjectState _ndoObjectState" );
			m_classElement.AppendField( ".field family class [NDO]NDO.ObjectId _ndoObjectId" );
			m_classElement.AppendField( ".field family notserialized class [NDO]NDO.IStateManager _ndoSm" );
			m_classElement.AppendField( $".field family notserialized valuetype {Corlib.Name}System.Guid _ndoTimeStamp" );
		}


		private void addRelationAttributes()
		{
			// Relations with generic containers and with multiplicity 1 can omit
			// the NDORelation attribute. We insert the attribute here.
			foreach (var fieldElement in m_classElement.Fields)
			{
				foreach (Patcher.ILReference reference in this.m_references)
				{
					if (reference.CleanName == fieldElement.getName())
					{
						bool customFound = false;
						for (ILElement custElement = fieldElement.GetSuccessor(); custElement != null; custElement = custElement.GetSuccessor())
						{
							if (!custElement.GetLine(0).StartsWith(".custom"))
								break;
							if (custElement.GetLine(0).IndexOf("NDORelationAttribute") > -1)
							{
								customFound = true;
								break;
							}
						}
						if (!customFound)
						{
							fieldElement.InsertAfter(new ILCustomElement(".custom instance void [NDO]NDO.NDORelationAttribute::.ctor() = ( 01 00 00 00 )", this.m_classElement));
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
							statementElement.InsertAfter(new ILStatementElement(callMarkDirty()));
							statementElement.InsertAfter(new ILStatementElement(ldarg_0));
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

                // The line might contain a ldfld to a member of a nested class
                // so make sure the line contains a reference to m_refName class
                if (line.IndexOf( m_refName + "::" ) == -1)
                    return false;

				int p = line.IndexOf("::");
				string nameInLine = line.Substring(p + 2).Replace("'", string.Empty);

				if (nameInLine == field.Name.Replace("'", string.Empty))
				{
					if ( line.StartsWith( "ldfld" ) )
					{
                        Field fieldMapping = this.m_classMapping.FindField(nameInLine);
                        if (fieldMapping == null)
                        {
                            messages.WriteLine("Warning: can't determine ordinal for field " + m_name + "." + nameInLine);
                            messages.WriteInsertedLine("Fetch Groups don't work with this field.");                            
							statementElement.InsertBefore(new ILStatementElement("dup"));
							statementElement.InsertBefore(new ILStatementElement(callLoadData()));
							needsMaxstackAdjustment = true;
						}
						else
						{
                            statementElement.InsertBefore(new ILStatementElement("dup"));  // this argument
                            statementElement.InsertBefore(new ILStatementElement("call       instance void " + m_refName + "::" + getAccName("ndoget_", field.Name) + "()"));
                            // the ldfld[a] statement remains in the code
                            needsMaxstackAdjustment = true;
                        }
					}
                    else if (line.StartsWith("stfld"))
                    {
                        statementElement.SetFirstLine("call       instance void " + m_refName + "::" + getAccName("ndoset_", field.Name) + '(' + field.ILType + ')');
                    }
					return needsMaxstackAdjustment;
				}

			} // foreach ( Field field in ownFieldsHierarchical )
			return false;
		}
	
		private void
		replaceLdfldAndStfld()
		{				  
			foreach ( ILMethodElement methodElement in m_classElement.Methods)
			{
				bool adjustMaxStack = false;

				if ( methodElement.isConstructor() )
					continue;


                // We'll change the collection of statements, so we need a new collection
                // to iterate through.
				var statements = new List<ILStatementElement>();
				foreach (var statementElement in methodElement.Statements)
				{
					statements.Add(statementElement);
				}

				foreach ( ILStatementElement statementElement in statements )
				{
					bool result;
					string line = ILElement.StripLabel(statementElement.GetLine( 0 ));
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
					this.AdjustMaxStackVal(methodElement, newMaxStack);
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
				return $"ldfld      valuetype {Corlib.Name}System.Guid " + persistentRoot + "::_ndoTimeStamp";
			else
				return $"ldfld      valuetype {Corlib.Name}System.Guid " + m_refName + "::_ndoTimeStamp";
		}
		string storeTimeStamp()
		{
			if (m_hasPersistentBase)
				return $"stfld      valuetype {Corlib.Name}System.Guid " + persistentRoot + "::_ndoTimeStamp";
			else
				return $"stfld      valuetype {Corlib.Name}System.Guid " + m_refName + "::_ndoTimeStamp";
		}


		void addDirtyStatements(ILMethodElement methodElement, bool markDirty)
		{
			if (dirtyDone.Contains(methodElement))
				return;
			AdjustMaxStackVal(methodElement, 2);

			ILStatementElement beforeElement = methodElement.Statements.First( se => se.GetLine(0).StartsWith("IL"));
			ILStatementElement el = new ILStatementElement();
			el.AddLine("ldarg.0");
			el.AddLine(loadStateManager());
			el.AddLine("brfalse.s  NoSm");
			el.AddLine("ldarg.0");
			el.AddLine(loadStateManager());
			el.AddLine("ldarg.0");
			if (markDirty) 
				el.AddLine("callvirt   instance void [NDO]NDO.IStateManager::MarkDirty(class [NDO]NDO.IPersistenceCapable)");
			else
				el.AddLine("callvirt   instance void [NDO]NDO.IStateManager::LoadData(class [NDO]NDO.IPersistenceCapable)");
			el.AddLine("NoSm:");
			beforeElement.InsertBefore(el);
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

		private void 
		AdjustMaxStackVal(ILMethodElement methodElement, int maxStackVal)
		{
			ILElement msEl = methodElement.FindElement(".maxstack");
			if (null == msEl)
				throw new Exception("Method " + m_refName + "." + methodElement.Name + " doesn't have a .maxstack statement.");
			string msline = msEl.GetLine(0); 
			string num = msline.Substring(msline.IndexOf(".maxstack") + 9);
			if (int.Parse(num) < maxStackVal)
			{
				msEl.Remove();
				msEl = new ILStatementElement();
				msEl.AddLine(".maxstack " + maxStackVal.ToString());
				methodElement.Statements.First().InsertBefore(msEl);
			}
		}

		private void 
		AddToMaxStackVal(ILMethodElement methodElement, int diff)
		{
			ILElement msEl = methodElement.FindElement(".maxstack");
			if (null == msEl)
                throw new Exception("Method " + m_refName + "." + methodElement.Name + " doesn't have a .maxstack statement.");
            string msline = msEl.GetLine(0); 
			string num = msline.Substring(msline.IndexOf(".maxstack") + 9);
			int newNum = int.Parse(num) + diff;
			ILStatementElement newEl = new ILStatementElement();
			msEl.InsertBefore(newEl);
			msEl.Remove();
			newEl.AddLine(".maxstack " + newNum.ToString());
		}

		private int 
			getMaxStackVal(ILMethodElement methodElement)
		{
			ILElement msEl = methodElement.FindElement(".maxstack");
			if (null == msEl)
                throw new Exception("Method " + m_refName + "." + methodElement.Name + " doesn't have a .maxstack statement.");
            string msline = msEl.GetLine(0); 
			string num = msline.Substring(msline.IndexOf(".maxstack") + 9);
			return (int.Parse(num));
		}


		private void addLocalVariable(ILMethodElement methodElement, string name, string ilType)
		{
			ILLocalsElement localsElement = methodElement.GetLocals();
			if (localsElement == null)
			{
				localsElement = new ILLocalsElement(".locals init ([0] " + ilType + " " + name + ")", methodElement);
				ILElement maxstackElement = methodElement.FindElement(".maxstack");
				if (maxstackElement == null)
                    throw new Exception("Method " + m_refName + "." + methodElement.Name + " doesn't have a .maxstack statement.");
                maxstackElement.InsertAfter(localsElement);
				return;
			}
			string line = localsElement.GetAllLines();
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
			localsElement.InsertBefore(newLocalsElement);
			localsElement.Remove();
		}

		public void MarkRelations()
		{
			if (!m_references.Any())
				return;

			ListAccessManipulator accessManipulator = new ListAccessManipulator(this.m_classElement.AssemblyName);
			
			foreach ( ILMethodElement methodElement in m_classElement.Methods )
			{
				bool needsContainerStack = false;
				bool methodTouched = false;
                string mname = methodElement.Name;
				if ( methodElement.isConstructor() )
					continue;

				List<ILStatementElement> statements = new List<ILStatementElement>(100);
				ILElement firstElement = null;
				ILLocalsElement localsElement = methodElement.GetLocals();
				var listReflectors = new Dictionary<Type,IListReflector>();
				string line;
				int pos;

				// We copy the statements into an extra List because the original
				// List is manipulated with InsertBefore.
				foreach (ILStatementElement statementElement in methodElement.Statements)
				{
					statements.Add(statementElement);
					line = statementElement.GetLine( 0 );
					if (firstElement == null && line.StartsWith("IL_"))
						firstElement = statementElement;
				}
				ILElement lineEl = firstElement;
				if (firstElement != null)
				{
					lineEl = firstElement.GetPredecessor();					
					if (lineEl != null && lineEl.GetLine(0).StartsWith(".line"))
						firstElement = lineEl;
				}

				foreach ( ILStatementElement statementElement in statements )
				{
					line = statementElement.GetLine( 0 );
					pos  = line.IndexOf( ":" );
					line = line.Substring( pos + 1 ).Trim();
					int p = line.IndexOf("::");

					string cleanName = null;
					if (p > -1)
						cleanName = line.Substring(p + 2).Replace("'", string.Empty);

					if ( line.StartsWith( "ldfld" ) && -1 < line.IndexOf( m_refName + "::" ))
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
									statementElement.SetFirstLine("call       instance " + reference.ILType + " " + m_refName + "::" + getAccName("ndoget_", reference.Name) + "()"); 
								}
								else
								{
									if (!listReflectors.ContainsKey(reference.FieldType))
										listReflectors.Add(reference.FieldType, ListReflectorFactory.CreateListReflector(reference.FieldType));
									statementElement.InsertBefore(new ILStatementElement("dup"));
									var elements = new List<ILStatementElement>();
									elements.Add(new ILStatementElement("ldloc __ndocontainertable"));
									elements.Add(new ILStatementElement(@"ldstr """ + reference.CleanName + @""""));
									elements.Add(new ILStatementElement($"call       object [NDO]NDO._NDOContainerStack::RegisterContainer(object,object,class {Corlib.Name}System.Collections.Hashtable,string)"));
                                    elements.Add(new ILStatementElement("castclass " + new ReflectedType(reference.FieldType, this.m_classElement.AssemblyName).QuotedILName));
									// Achtung: insertAfter benötigt die Statements in umgekehrter Reihenfolge
									for (int i = elements.Count - 1; i >=0; i--)
										statementElement.InsertAfter((ILStatementElement)elements[i]);
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
									statementElement.SetFirstLine("call       instance void " + m_refName + "::" + getAccName("ndoset_", reference.Name) + "(" + reference.ILType +")");
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
						line = ILElement.StripLabel( statementElement.GetLine( 0 ) );

						if ( line.StartsWith( "callvirt" ) || line.StartsWith("call ") )
						{
								if (accessManipulator.Manipulate(listReflectors, statementElement))
									methodTouched = true;
						}
					}
				}

				if (needsContainerStack)
				{
					addLocalVariable(methodElement, "__ndocontainertable", $"class {Corlib.Name}System.Collections.Hashtable");
					firstElement.InsertBefore(new ILStatementElement($"newobj     instance void {Corlib.Name}System.Collections.Hashtable::.ctor()"));
					firstElement.InsertBefore(new ILStatementElement("stloc __ndocontainertable"));
					AddToMaxStackVal(methodElement, 3);
				}

				//				if (needsContainerStack || readEntries)
				//					addDirtyStatements(methodElement, false);
				if (methodTouched || needsContainerStack)
					MakeLongBranches(methodElement);
			} // ILMethodElement
		}


		public void MakeLongBranches(ILMethodElement methodElement)
		{
			foreach ( ILStatementElement statementElement in methodElement.Statements )
			{
				string line = statementElement.GetLine(0);
				if (line.StartsWith("IL"))
				{
					for (int i = 0; i < AllBranches.Opcodes.Length; i++)
					{
						string s = AllBranches.Opcodes[i];
						if (line.IndexOf(s) > -1)
						{
							statementElement.ReplaceText(s, AllBranches.LongOpcodes[i]);
							break;
						}
					}
				}
			}				
		}

		public void
		patchConstructor()
		{
			var constructors = new List<ILMethodElement>();

			bool hasDefaultConstructor = false;
			foreach ( ILMethodElement methodElement in m_classElement.Methods )
			{
				if ( !methodElement.isConstructor() )
					continue;

				constructors.Add(methodElement);

				if (methodElement.getParameterCount() == 0)
					hasDefaultConstructor = true;
			}

			if (!hasDefaultConstructor && constructors.Count > 0)
				messages.WriteLine( "Warning: class " + m_refName.Replace( "'", string.Empty ) + " doesn't have a default constructor. NDO will try to resolve other constructors during runtime." );

			int maxStack = this.m_hasPersistentBase ? 2 : 2;
			
			foreach(ILMethodElement me in constructors)
			{
				AdjustMaxStackVal(me, maxStack);

				ILStatementElement lastStatement = me.Statements.Last();
				string line = ILElement.StripLabel(lastStatement.GetLine(0));
								
				var elements = new List<ILStatementElement>();

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
					lastStatement.InsertBefore(el);
			}


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

		void addLoadStateProperty()
		{
			ILMethodElement methodElement = m_classElement.GetMethod( "get_NDOLoadState" );
			ILPropertyElement propElement = m_classElement.GetProperty( "NDOLoadState" );

			if ( m_hasPersistentBase )
			{
				if (methodElement != null)
					methodElement.Remove();
				if ( propElement != null )
					propElement.Remove();
				return;
			}
	
			if ( methodElement == null )
			{
				methodElement = new ILMethodElement();
				methodElement.AddLine( ".method public hidebysig newslot specialname final virtual" );
				methodElement.AddLine( "instance class [NDO]NDO.LoadState get_NDOLoadState() cil managed" );
				this.m_classElement.AddElement( methodElement );
			}
			else
			{				
				methodElement.ClearElements();
			}
			methodElement.addStatement(".maxstack  3");
			methodElement.addStatement(ldarg_0);
			methodElement.addStatement("ldfld      class [NDO]NDO.LoadState " + m_refName + "::_ndoLoadState");
			methodElement.addStatement("ret");

			if ( propElement == null )
			{
				propElement = new ILPropertyElement();
				propElement.AddLine( ".property instance class [NDO]NDO.LoadState NDOLoadState()" );
				propElement.AddElement( new ILCustomElement( $".custom instance void {Corlib.SystemComponentModel}System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", propElement ) );
				propElement.AddElement( new ILCustomElement( $".custom instance void {Corlib.XmlIgnoreAssembly}System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", propElement ) );
				propElement.AddElement( new ILGetElement( ".get instance class [NDO]NDO.LoadState " + m_nonGenericRefName + "::get_NDOLoadState()" ) );
				this.m_classElement.AddElement( propElement );
			}
		}

		void addGetLoadState()
		{
			ILMethodElement methodElement = m_classElement.GetMethod( "NDOGetLoadState" );
			
			if ( m_hasPersistentBase )
			{
				if ( methodElement != null )
					methodElement.Remove();
				return;
			}

			if ( methodElement == null )
			{
				methodElement = new ILMethodElement();
				methodElement.AddLine( ".method public hidebysig newslot virtual" );
				methodElement.AddLine( "instance bool NDOGetLoadState(int32 ordinal) cil managed" );
				this.m_classElement.AddElement( methodElement );
			}
			else
			{
				methodElement.ClearElements();
			}
			methodElement.addStatement(".maxstack  3");

			methodElement.addStatement(ldarg_0);
			methodElement.addStatement("ldfld      class [NDO]NDO.LoadState " + m_refName + "::_ndoLoadState");
			methodElement.addStatement($"callvirt   instance class {Corlib.SystemCollections}System.Collections.BitArray [NDO]NDO.LoadState::get_RelationLoadState()");
			methodElement.addStatement(ldarg_1);
			methodElement.addStatement($"callvirt instance bool {Corlib.SystemCollections}System.Collections.BitArray::get_Item(int32)");
			methodElement.addStatement("ret");
		}

		void addSetLoadState()
		{
			ILMethodElement methodElement = m_classElement.GetMethod( "NDOSetLoadState" );
			if ( m_hasPersistentBase )
			{
				if ( methodElement != null )
					methodElement.Remove();
				return;
			}
			if ( methodElement == null )
			{
				methodElement = new ILMethodElement();
				methodElement.AddLine( ".method public hidebysig newslot virtual" );
				methodElement.AddLine( "instance void NDOSetLoadState(int32 ordinal, bool isLoaded) cil managed" );
				this.m_classElement.AddElement( methodElement );
			}
			else
			{
				methodElement.ClearElements();
			}
			methodElement.addStatement(".maxstack  3");

			methodElement.addStatement(ldarg_0);
			methodElement.addStatement("ldfld      class [NDO]NDO.LoadState "  + m_refName + "::_ndoLoadState");
			methodElement.addStatement($"callvirt   instance class {Corlib.SystemCollections}System.Collections.BitArray [NDO]NDO.LoadState::get_RelationLoadState()");
			methodElement.addStatement("ldarg.1");
			methodElement.addStatement("ldarg.2");
			methodElement.addStatement($"callvirt instance void {Corlib.SystemCollections}System.Collections.BitArray::set_Item(int32, bool)");
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
					Add1to1RelationAccessors(reference);
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
			Add1to1RelationGetAccessor(ILReference reference)
		{
			// Get-Method only for 1:1
			ILMethodElement method = new ILMethodElement();
			method.AddLine( ".method private hidebysig instance " );
			method.AddLine( reference.ILType + " " + getAccName("ndoget_", reference.Name) + "() cil managed");
			method.addStatement(".maxstack  5" );
			method.addStatement(ldarg_0);
			method.addStatement( loadStateManager() );
			method.addStatement( "brfalse.s  noSm");

			method.addStatement(ldarg_0);
			method.addStatement( loadStateManager() );
			method.addStatement(ldarg_0);
			method.addStatement(@"ldstr      """ + reference.CleanName + @"""");
			method.addStatement($"callvirt   instance class {Corlib.Name}System.Collections.IList [NDO]NDO.IStateManager::LoadRelation(class [NDO]NDO.IPersistenceCapable,string)");
			method.addStatement("pop");

			method.addStatement("noSm:");
			method.addStatement(ldarg_0);
			method.addStatement("ldfld      " + reference.ILType + " " + m_refName + "::" + reference.Name);
			method.addStatement( "ret");
			m_classElement.AppendMethod( method );
		}

		private void
			Add1to1RelationAccessors(ILReference reference)
		{
			Add1to1RelationGetAccessor(reference);

			// Set-Methode
			ILMethodElement method = new ILMethodElement();
			method.AddLine( ".method private hidebysig specialname instance void" );
			method.AddLine( getAccName("ndoset_", reference.Name) + "(" + reference.ILType + " 'value') cil managed");
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
			m_classElement.AppendMethod( method );
		}


		private void
			add1toNRelationAccessors(ILReference reference)
		{
			//addRelationGetAccessor(reference);

			// Set-Methode
			ILMethodElement method = new ILMethodElement();
			method.AddLine( ".method private hidebysig specialname instance void" );
			method.AddLine( getAccName("ndoset_", reference.Name) + "(" + reference.ILType + " 'value') cil managed");
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

			method.addStatement( $"callvirt   instance void [NDO]NDO.IStateManager::AssignRelation(class [NDO]NDO.IPersistenceCapable, string, class {Corlib.Name}System.Collections.IList, class {Corlib.Name}System.Collections.IList)");

			method.addStatement( "NoSm:");
			method.addStatement(ldarg_0);
			method.addStatement( ldarg_1 );
			method.addStatement( "stfld      " + reference.ILType + " "+ m_refName + "::" + reference.Name);
			method.addStatement( "ret");

			m_classElement.AppendMethod( method );
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
			method.AddLine( ".method private hidebysig instance void" );
			//			string classStr = field.IsEmbeddedType ? "class " : string.Empty;
			method.AddLine( getAccName("ndoset_", field.Name) + "(" + field.ILType + " 'value') cil managed");
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

			m_classElement.AppendMethod( method );

			Field fieldMapping = this.m_classMapping.FindField(field.CleanName);

			// We can't get Value Types with getter functions, because that would result in a
            // copy of the object. So the original object couldn't be manipulated.            
			if (!field.IsValueType && fieldMapping != null)
			{
				method = new ILMethodElement();
				method.AddLine(".method private hidebysig instance void");
                method.AddLine(getAccName("ndoget_", field.Name) + "() cil managed");
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
				m_classElement.AppendMethod( method );
			}
		}

		public void
		removeMarkDirty()
		{
			ILMethodElement method = m_classElement.GetMethod( "NDOMarkDirty" );

			if ( method != null )
				method.Remove();
		}

		public void 
		addMarkDirty()
		{
			ILMethodElement method = m_classElement.GetMethod( "NDOMarkDirty" );

			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot virtual final" );
				method.AddLine( "instance void  NDOMarkDirty() cil managed" );
				m_classElement.AppendMethod( method );
			}

			method.addStatement( ".maxstack  2" );
			method.addStatement( ldarg_0 );
			method.addStatement( loadStateManager());
			method.addStatement( "dup" );
			method.addStatement( "brfalse.s  NoSm" );

			method.addStatement( ldarg_0);
			method.addStatement( "callvirt   instance void [NDO]NDO.IStateManager::MarkDirty(class [NDO]NDO.IPersistenceCapable)");
			method.addStatement( "NoSm: ret");
	}

		public void
		removeLoadData()
		{
			ILMethodElement method = m_classElement.GetMethod( "NDOLoadData" );
			if ( method != null )
				method.Remove();
		}

		public void 
		addLoadData()
		{
			ILMethodElement method = m_classElement.GetMethod( "NDOLoadData" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				m_classElement.AppendMethod( method );
				method.AddLine( ".method family hidebysig newslot final virtual" );
				method.AddLine( "instance void  NDOLoadData() cil managed" );
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
			ILMethodElement method = m_classElement.GetMethod( "get_NDOObjectId" );
			if ( method != null )
				method.Remove();
			method = m_classElement.GetMethod( "set_NDOObjectId" );
			if ( method != null )
				method.Remove();
			ILPropertyElement prop = m_classElement.GetProperty( "NDOObjectId" );
			if ( prop != null )
				prop.Remove();
		}

		public void 
		addObjectId()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOObjectId" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( "instance class [NDO]NDO.ObjectId get_NDOObjectId() cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement(loadObjectId());
			method.addStatement( "ret");


			method = m_classElement.GetMethod( "set_NDOObjectId" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				m_classElement.AppendMethod( method );
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( "instance void  set_NDOObjectId(class [NDO]NDO.ObjectId 'value') cil managed" );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			method.addStatement(storeObjectId());
			method.addStatement( "ret");


			ILPropertyElement prop = m_classElement.GetProperty( "NDOObjectId" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance class [NDO]NDO.ObjectId NDOObjectId()", m_classElement );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.SystemComponentModel}System.ComponentModel.CategoryAttribute::.ctor(string) = ( 01 00 03 4E 44 4F 00 00 )", prop ) );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.XmlIgnoreAssembly}System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.AddElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOObjectId(class [NDO]NDO.ObjectId)" ) );
				prop.AddElement( new ILGetElement( ".get instance class [NDO]NDO.ObjectId " + m_nonGenericRefName + "::get_NDOObjectId()" ) );
				m_classElement.AppendProperty( prop );
			}
		}

		public void
		removeTimeStamp()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOTimeStamp" );
			if ( method != null )
				method.Remove();
			method = m_classElement.GetMethod( "set_NDOTimeStamp" );
			if ( method != null )
				method.Remove();
			ILPropertyElement prop = m_classElement.GetProperty( "NDOTimeStamp" );
			if ( prop != null )
				prop.Remove();
		}

		public void 
		addTimeStamp()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOTimeStamp" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( $"instance valuetype {Corlib.Name}System.Guid get_NDOTimeStamp() cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement(loadTimeStamp());
			method.addStatement( "ret");


			method = m_classElement.GetMethod( "set_NDOTimeStamp" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( $"instance void  set_NDOTimeStamp(valuetype {Corlib.Name}System.Guid 'value') cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			method.addStatement(storeTimeStamp());
			method.addStatement( "ret");

			ILPropertyElement prop = m_classElement.GetProperty( "NDOTimeStamp" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( $".property instance valuetype {Corlib.Name}System.Guid NDOTimeStamp()", m_classElement );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.SystemComponentModel}System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", prop ) );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.XmlIgnoreAssembly}System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.AddElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + $"::set_NDOTimeStamp(valuetype {Corlib.Name}System.Guid)" ) );
				prop.AddElement( new ILGetElement( $".get instance valuetype {Corlib.Name}System.Guid " + m_nonGenericRefName + "::get_NDOTimeStamp()" ) );
				m_classElement.AppendProperty( prop );
			}
		}

		public void
		removeObjectState()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOObjectState" );
			if ( method != null )
				method.Remove();
			method = m_classElement.GetMethod( "set_NDOObjectState" );
			if ( method != null )
				method.Remove();
			ILPropertyElement prop = m_classElement.GetProperty( "NDOObjectState" );
			if ( prop != null )
				prop.Remove();
		}

		public void
		addGetObjectHashCode()
		{
			ILMethodElement method = new ILMethodElement();
			if ( m_hasPersistentBase )
				method.AddLine( ".method public hidebysig virtual" );
			else
				method.AddLine( ".method public hidebysig newslot virtual" );
			method.AddLine( "instance int32 NDOGetObjectHashCode() cil managed" );
			m_classElement.AppendMethod( method );

			method.addStatement(".maxstack  1");
			method.addStatement(ldarg_0);
			method.addStatement($"call       instance int32 {Corlib.Name}System.Object::GetHashCode()");
			method.addStatement("ret");			   
		}

		public void 
		addObjectState()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOObjectState" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( "instance valuetype [NDO]NDO.NDOObjectState get_NDOObjectState() cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( this.loadObjectState() );
			method.addStatement( "ret");


			method = m_classElement.GetMethod( "set_NDOObjectState" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( "instance void  set_NDOObjectState(valuetype [NDO]NDO.NDOObjectState 'value') cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			method.addStatement(this.storeObjectState());
			method.addStatement( "ret");


			ILPropertyElement prop = m_classElement.GetProperty( "NDOObjectState" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance valuetype [NDO]NDO.NDOObjectState NDOObjectState()", m_classElement );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.SystemComponentModel}System.ComponentModel.CategoryAttribute::.ctor(string) = ( 01 00 03 4E 44 4F 00 00 )", prop ) );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.XmlIgnoreAssembly}System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.AddElement( new ILGetElement( ".get instance valuetype [NDO]NDO.NDOObjectState " + m_nonGenericRefName + "::get_NDOObjectState()" ) );
				prop.AddElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOObjectState(valuetype [NDO]NDO.NDOObjectState)" ) );

				m_classElement.AppendProperty( prop );
			}

		}

		public void
		removeStateManager()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOStateManager" );
			if ( method != null )
				method.Remove();
			method = m_classElement.GetMethod( "set_NDOStateManager" );
			if ( method != null )
				method.Remove();
			ILPropertyElement prop = m_classElement.GetProperty( "NDOStateManager" );
			if ( prop != null )
				prop.Remove();
		}


		public void 
		addStateManager()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOStateManager" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( "instance class [NDO]NDO.IStateManager get_NDOStateManager() cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  1" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( loadStateManager());
			method.addStatement( "ret");


			method = m_classElement.GetMethod( "set_NDOStateManager" );
			if ( method != null )
			{
				method.ClearElements();
			}
			else
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual" );
				method.AddLine( "instance void  set_NDOStateManager(class [NDO]NDO.IStateManager 'value') cil managed" );
				m_classElement.AppendMethod( method );
			}
			method.addStatement( ".maxstack  2" );
            
			method.addStatement( "  ldarg.0");
			method.addStatement( "  ldarg.1");
			string statement = loadStateManager();
			method.addStatement( loadStateManager().Replace("ldfld", "stfld"));
			method.addStatement( "ret");


			ILPropertyElement prop = m_classElement.GetProperty( "NDOStateManager" );
			if ( prop == null )
			{
				prop = new ILPropertyElement( ".property instance class [NDO]NDO.IStateManager NDOStateManager()", m_classElement );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.SystemComponentModel}System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", prop ) );
				prop.AddElement( new ILCustomElement( $".custom instance void {Corlib.XmlIgnoreAssembly}System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", prop ) );
				prop.AddElement( new ILGetElement( ".get instance class [NDO]NDO.IStateManager " + m_nonGenericRefName + "::get_NDOStateManager()" ) );
				prop.AddElement( new ILSetElement( ".set instance void " + m_nonGenericRefName + "::set_NDOStateManager(class [NDO]NDO.IStateManager)" ) );

				m_classElement.AppendProperty( prop );
			}

		}


		private void insertReadForGuidVtPublicField(ILMethodElement method, ILField field, int nr)
		{
			string vtname = field.Parent.ILType;
			if (vtname.StartsWith("valuetype"))
				vtname = vtname.Substring(10);

			method.addStatement("\r\n//insertReadForGuidVtPublicField\r\n");


			method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
			method.addStatement("beq      GuidIsDbNull" + nr.ToString());

			method.addStatement("ldloc.0");
			method.addStatement($"isinst     {Corlib.Name}System.String");
			method.addStatement("brfalse.s  nostr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement($"ldflda     valuetype {Corlib.Name}System.Guid " + vtname + "::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement($"castclass  {Corlib.Name}System.String");
			method.addStatement($"call       instance void {Corlib.Name}System.Guid::.ctor(string)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nostr" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement($"isinst     {Corlib.Name}System.Guid");
			method.addStatement("brfalse.s  noguid" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement($"unbox      {Corlib.Name}System.Guid");
			method.addStatement($"ldobj      {Corlib.Name}System.Guid");
			method.addStatement($"stfld      valuetype {Corlib.Name}System.Guid " + vtname + "::" + field.Name);
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("noguid" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     unsigned int8[]");
			method.addStatement("brfalse.s  nobytearr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement($"ldflda     valuetype {Corlib.Name}System.Guid " + vtname +"::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement("castclass  unsigned int8[]");
			method.addStatement($"call       instance void {Corlib.Name}System.Guid::.ctor(unsigned int8[])");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nobytearr" + nr.ToString() + ":");
			method.addStatement(@"ldstr      ""Can't convert Guid field to column type {0}""");
			method.addStatement("ldloc.0");
			method.addStatement($"callvirt   instance class {Corlib.Name}System.Type {Corlib.Name}System.Object::GetType()");
			method.addStatement($"callvirt   instance string {Corlib.Name}System.Type::get_FullName()");
			method.addStatement($"call       string {Corlib.Name}System.String::Format(string,object)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("GuidIsDbNull" + nr.ToString() + ":");
			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement($"ldsfld     valuetype {Corlib.Name}System.Guid {Corlib.Name}System.Guid::Empty");
			method.addStatement($"stfld      valuetype {Corlib.Name}System.Guid " + vtname + "::" + field.Name);

			method.addStatement("guidready" + nr.ToString() + ":");
		}

		private void insertReadForGuidVtProperty(ILMethodElement method, ILField field, int nr)
		{
			string vtname = field.Parent.ILType;
			if (vtname.StartsWith("valuetype"))
				vtname = vtname.Substring(10);
			method.addStatement("\r\n//insertReadForGuidVtProperty\r\n");

			method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
			method.addStatement("beq.s      GuidIsDbNull" + nr.ToString());

			method.addStatement("ldloc.0");
			method.addStatement($"isinst     {Corlib.Name}System.String");
			method.addStatement("brfalse.s  nostr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement($"castclass  {Corlib.Name}System.String");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Guid::.ctor(string)");
			method.addStatement($"call       instance void " + vtname +"::" + getAccName("set_", field.Name) + $"(valuetype {Corlib.Name}System.Guid)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nostr" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement($"isinst     {Corlib.Name}System.Guid");
			method.addStatement("brfalse.s  noguid" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement($"unbox      {Corlib.Name}System.Guid");
			method.addStatement($"ldobj      {Corlib.Name}System.Guid");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + $"(valuetype {Corlib.Name}System.Guid)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("noguid" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     unsigned int8[]");
			method.addStatement("brfalse.s  nobytearr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement("ldloc.0");
			method.addStatement("castclass  unsigned int8[]");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Guid::.ctor(unsigned int8[])");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + $"(valuetype {Corlib.Name}System.Guid)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nobytearr" + nr.ToString() +":");
			method.addStatement(@"ldstr      ""Can't convert Guid field to column type {0}""");
			method.addStatement("ldloc.0");
			method.addStatement($"callvirt   instance class {Corlib.Name}System.Type {Corlib.Name}System.Object::GetType()");
			method.addStatement($"callvirt   instance string {Corlib.Name}System.Type::get_FullName()");
			method.addStatement($"call       string {Corlib.Name}System.String::Format(string,object)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("GuidIsDbNull" + nr.ToString() + ":");
			method.addStatement(ldarg_0);
			method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name );
			method.addStatement($"ldsfld     valuetype {Corlib.Name}System.Guid {Corlib.Name}System.Guid::Empty");
			method.addStatement("call       instance void " + vtname +"::" + getAccName("set_", field.Name) + $"(valuetype {Corlib.Name}System.Guid)");

			method.addStatement("guidready" + nr.ToString() +":");
		}


		private void insertReadForGuidMember(ILMethodElement method, ILField field, int nr)
		{
			method.addStatement("\n//insertReadForGuidMember\n");
			//
			method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
			method.addStatement("beq.s      GuidIsDbNull" + nr.ToString());
			method.addStatement("ldloc.0");

			method.addStatement($"isinst     {Corlib.Name}System.String");
			method.addStatement("brfalse.s  nostr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement($"ldflda     valuetype {Corlib.Name}System.Guid " + m_refName + "::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement($"castclass  {Corlib.Name}System.String");
			method.addStatement($"call       instance void {Corlib.Name}System.Guid::.ctor(string)");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nostr" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement($"isinst     {Corlib.Name}System.Guid");
			method.addStatement("brfalse.s  noguid" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement("ldloc.0");
			method.addStatement($"unbox      {Corlib.Name}System.Guid");
			method.addStatement($"ldobj      {Corlib.Name}System.Guid");
			method.addStatement($"stfld      valuetype {Corlib.Name}System.Guid " + m_refName + "::" + field.Name);
			method.addStatement("br.s       guidready" + nr.ToString());
			method.addStatement("noguid" + nr.ToString() + ":");
			method.addStatement("ldloc.0");
			method.addStatement("isinst     unsigned int8[]");
			method.addStatement("brfalse.s  nobytearr" + nr.ToString());

			method.addStatement(ldarg_0);
			method.addStatement($"ldflda     valuetype {Corlib.Name}System.Guid " + m_refName + "::" + field.Name);
			method.addStatement("ldloc.0");
			method.addStatement("castclass  unsigned int8[]");
			method.addStatement($"call       instance void {Corlib.Name}System.Guid::.ctor(unsigned int8[])");
			method.addStatement("br.s       guidready" + nr.ToString());

			method.addStatement("nobytearr" + nr.ToString() + ":");
			method.addStatement(@"ldstr      ""Can't convert Guid field to column type {0}""");
			method.addStatement("ldloc.0");
			method.addStatement($"callvirt   instance class {Corlib.Name}System.Type {Corlib.Name}System.Object::GetType()");
			method.addStatement($"callvirt   instance string {Corlib.Name}System.Type::get_FullName()");
			method.addStatement($"call       string {Corlib.Name}System.String::Format(string,object)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("GuidIsDbNull" + nr.ToString() + ":");
			method.addStatement(ldarg_0);
			method.addStatement($"ldsfld     valuetype {Corlib.Name}System.Guid {Corlib.Name}System.Guid::Empty");
			method.addStatement($"stfld      valuetype {Corlib.Name}System.Guid " + m_refName + "::" + field.Name);

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
			method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
			method.addStatement("beq.s      DateTimeIsNull" + nr);
			if (!parentIsValueType)		// Member
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldloc.0");
				method.addStatement($"unbox      {Corlib.Name}System.DateTime");
				method.addStatement($"ldobj      {Corlib.Name}System.DateTime");
				method.addStatement($"stfld      valuetype {Corlib.Name}System.DateTime " + m_refName + "::" + field.Name);
				method.addStatement("br.s       DateTimeReady" + nr);
				method.addStatement("DateTimeIsNull" + nr + ":");
				method.addStatement(ldarg_0);
				method.addStatement($"ldsfld     valuetype {Corlib.Name}System.DateTime {Corlib.Name}System.DateTime::MinValue");
				method.addStatement($"stfld      valuetype {Corlib.Name}System.DateTime " + m_refName + "::" + field.Name);
			}
			else if (field.IsProperty)  // Vt with property
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldloc.0");
				method.addStatement($"unbox      {Corlib.Name}System.DateTime");
				method.addStatement($"ldobj      {Corlib.Name}System.DateTime");
				method.addStatement("call       instance void " + field.Parent.PureTypeName + "::" + getAccName("set_", field.Name) + $"(valuetype {Corlib.Name}System.DateTime)");
				method.addStatement("br.s       DateTimeReady" + nr);
				method.addStatement("DateTimeIsNull" + nr + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement($"ldsfld     valuetype {Corlib.Name}System.DateTime {Corlib.Name}System.DateTime::MinValue");
				method.addStatement("call       instance void " + field.Parent.PureTypeName + "::" + getAccName("set_", field.Name) + $"(valuetype {Corlib.Name}System.DateTime)");
			}
			else						// Vt with public field
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement("ldloc.0");
				method.addStatement($"unbox      {Corlib.Name}System.DateTime");
				method.addStatement($"ldobj      {Corlib.Name}System.DateTime");
				method.addStatement($"stfld      valuetype {Corlib.Name}System.DateTime " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement("br.s       DateTimeReady" + nr);
				method.addStatement("DateTimeIsNull" + nr + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
				method.addStatement($"ldsfld     valuetype {Corlib.Name}System.DateTime {Corlib.Name}System.DateTime::MinValue");
				method.addStatement($"stfld      valuetype {Corlib.Name}System.DateTime " + field.Parent.PureTypeName + "::" + field.Name);
			}
			method.addStatement("DateTimeReady" + nr + ":");
		}


        private void
        insertReadForGenericType(ILMethodElement method, ILField field, int nr)
        {
            method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
            method.addStatement("ceq");
            method.addStatement("brtrue.s   FieldOver" + nr);
            method.addStatement(ldarg_0);
            method.addStatement("ldloc.0");
            method.addStatement("ldtoken    " + field.ILType);
            method.addStatement($"call       class {Corlib.Name}System.Type {Corlib.Name}System.Type::GetTypeFromHandle(valuetype {Corlib.Name}System.RuntimeTypeHandle)");
            method.addStatement($"call       object [NDO]NDO.GenericFieldConverter::FromString(object,class {Corlib.Name}System.Type)");
            method.addStatement("unbox.any  " + field.ILType);
            method.addStatement("stfld      "+ field.ILType + " " + m_refName + "::" + field.Name);
        }


        private void
            addReadForGuidNullable(ILMethodElement method, ILField field, int nr)
        {
            method.addStatement($"isinst     {Corlib.Name}System.String");
            method.addStatement("brfalse.s  nostring" + nr);
            method.addStatement("ldloc.0");
            method.addStatement($"castclass  {Corlib.Name}System.String");
            method.addStatement($"newobj     instance void {Corlib.Name}System.Guid::.ctor(string)");
            method.addStatement("br.s       guidready" + nr);
            method.addStatement("nostring" + nr + ':');
            method.addStatement("ldloc.0");
            method.addStatement($"isinst     {Corlib.Name}System.Guid");
            method.addStatement("brfalse.s  noguid" + nr);
            method.addStatement("ldloc.0");
            method.addStatement($"unbox.any  {Corlib.Name}System.Guid");
            method.addStatement("br.s       guidready" + nr);
            method.addStatement("noguid" + nr + ':');
            method.addStatement("ldstr      \"Can't convert type \"");
            method.addStatement("ldloc.0");
            method.addStatement($"callvirt   instance class {Corlib.Name}System.Type {Corlib.Name}System.Object::GetType()");
            method.addStatement($"callvirt   instance string {Corlib.Name}System.Type::get_FullName()");
            method.addStatement("ldstr      \" to field type System.Guid.\"");
            method.addStatement($"call       string {Corlib.Name}System.String::Concat(string,string,string)");
            method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
            method.addStatement("throw");
            method.addStatement("guidready" + nr + ':');
            method.addStatement($"newobj     instance void valuetype {Corlib.Name}System.Nullable`1<valuetype {Corlib.Name}System.Guid>::.ctor(!0)");

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
			//			method.addStatement($"call       void {Corlib.Name}System.Console::WriteLine(string)");

            bool isNullable = false;

            Type argType = null;
            string argTypeName = string.Empty;
            if (tname.StartsWith($"valuetype {Corlib.Name}System.Nullable`1<"))
            {
                isNullable = true;
                argType = t.GetGenericArguments()[0];
                argTypeName = new ReflectedType(argType, this.m_classElement.AssemblyName).QuotedILName;
            }

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
			method.addStatement($"callvirt   instance object {Corlib.SystemDataCommon}System.Data.DataRow::get_Item(string)");
			method.addStatement("dup");
			method.addStatement("stloc.0");

			// Special treatment for DateTime and Guid values to map to and from DBNull
			if (tname == $"valuetype {Corlib.Name}System.Guid")
			{
				insertReadForGuid(method, field, nr, parentIsValueType);
				goto theexit;
			}
            else if (tname == $"valuetype {Corlib.Name}System.DateTime")
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
                method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
                method.addStatement("beq      FieldNull" + nr.ToString());
            }

			method.addStatement(ldarg_0);
			if (parentIsValueType)
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);
			method.addStatement("ldloc.0");
			if (tname == "string" || tname == "unsigned int8[]" || tname == $"class {Corlib.Name}System.String" || tname == $"class {Corlib.Name}System.Byte[]")
				method.addStatement("  castclass  " + tname);
			else if (tname.StartsWith("valuetype"))
			{
				string tname2 = tname.Substring(10);

                if (isNullable)
                {
                    if (typeof(System.Enum).IsAssignableFrom(argType))
                    {
                        method.addStatement("unbox.any " + argTypeName);
                        method.addStatement($"newobj     instance void valuetype {Corlib.Name}System.Nullable`1<" + argTypeName + ">::.ctor(!0)");
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
                    method.addStatement("unbox  " + tname2);
                    method.addStatement("ldobj  " + tname2);
                }
			}
			else
			{
				if (tname == "unsigned int8")
					method.addStatement($"unbox {Corlib.Name}System.Byte");
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
                    method.addStatement($"ldflda     valuetype {Corlib.Name}System.Nullable`1<" + argTypeName + "> " + m_refName + "::" + field.Name);
                    method.addStatement($"initobj    valuetype {Corlib.Name}System.Nullable`1<" + argTypeName + ">");
                }
            #endif
                method.addStatement("FieldOver" + nr.ToString() + ":");

		}

		//
		// addRead
		//
		public void addRead()
		{
			ILMethodElement method = m_classElement.GetMethod( "NDORead" );
			if ( method == null )
			{
				method = new ILMethodElement();

				if ( m_hasPersistentBase )
					method.AddLine( ".method public hidebysig virtual" );
				else
					method.AddLine( ".method public hidebysig newslot virtual" );
				method.AddLine( $"instance void  NDORead(class {Corlib.SystemDataCommon}System.Data.DataRow dr, string[] fields, int32 startind) cil managed" );
				m_classElement.AppendMethod( method );
			}
			else
			{
				method.ClearElements();
			}
			method.addStatement(".maxstack  8" );
			addLocalVariable(method, "theObject", "object");
			addLocalVariable(method, "i", "int32");
			addLocalVariable(method, $"ex", $"class {Corlib.Name}System.Exception");

			method.addStatement("ldc.i4.0");
			method.addStatement("stloc.1");

			method.addStatement("ldarg.1");
			method.addStatement("brtrue   rownotnull");
			method.addStatement(@"ldstr      ""NDORead: DataRow ist null""");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("rownotnull: ");

			ILStatementElement[] fixupElements = new ILStatementElement[2];

			method.addStatement("ldarg.3");
			fixupElements[0] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.AddElement(fixupElements[0]);
			method.addStatement("add");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ble.s      indexbigger");
			method.addStatement(@"ldstr      ""NDORead: Index {0} is bigger than maximum index of fields array ({1})""");
			method.addStatement("ldarg.3");
			fixupElements[1] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.AddElement(fixupElements[1]);
			method.addStatement("add");
			method.addStatement($"box        {Corlib.Name}System.Int32");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ldc.i4.1");
			method.addStatement("sub");

			method.addStatement($"box        {Corlib.Name}System.Int32");
			method.addStatement($"call       string {Corlib.Name}System.String::Format(string, object, object)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("indexbigger: ");

			method.addStatement(".try {");
			int nr = 0;

			foreach(var e in this.sortedFields)
			{
				if (nr == this.mappedFieldCount)
					break;

				ILField field = e.Value;
				if (field.Parent != null && field.Parent.IsEmbeddedType)
					continue;
				addReadForField(method, field, nr);
				nr++;
			}

			fixupElements[0].ReplaceText("##fieldcount", nr.ToString());
			nr--;
			fixupElements[1].ReplaceText("##fieldcount", nr.ToString());
			nr++;

			method.addStatement("leave.s    aftercatch");

			method.addStatement("}  // end .try");
			method.addStatement($"catch {Corlib.Name}System.Exception");
			method.addStatement("{");
			method.addStatement("stloc.2");
			method.addStatement(@"ldstr      ""Error while writing to field """);
			method.addStatement("ldarg.2");
			method.addStatement("ldloc.1");
			method.addStatement("ldelem.ref");
			method.addStatement(@"ldstr      ""\n""");
			method.addStatement("ldloc.2");
			method.addStatement($"callvirt   instance string {Corlib.Name}System.Exception::get_Message()");
			method.addStatement($"call       string {Corlib.Name}System.String::Concat(string,string,string,string)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("}  // end handler");

			method.addStatement("aftercatch: ");

			if (m_hasPersistentBase)
			{
				method.addStatement( ldarg_0 );
				method.addStatement( ldarg_1 );
				method.addStatement( "ldarg.2" );
				method.addStatement( "ldarg.3" );
				method.addStatement( "ldc.i4.s " + nr.ToString() );
				method.addStatement( "add" );
				method.addStatement( $"call       instance void {m_persistentBase}::NDORead(class {Corlib.SystemDataCommon}System.Data.DataRow dr, string[] fields, int32 startind)");
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
				method.addStatement($"ldfld      valuetype {Corlib.Name}System." + type + " " + m_refName + "::" + field.Name);
				method.addStatement($"ldsfld     valuetype {Corlib.Name}System." + type + $" {Corlib.Name}System." + empty);
				method.addStatement($"call       bool {Corlib.Name}System.{type}::op_Inequality(valuetype {Corlib.Name}System.{type}, valuetype {Corlib.Name}System.{type})");
				method.addStatement("brtrue.s   VtFieldIsNotEmpty" + nr.ToString());
				method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
				method.addStatement("br.s       VtFieldReady" + nr.ToString());
				method.addStatement("VtFieldIsNotEmpty" + nr.ToString() + ":");
				method.addStatement(ldarg_0);
				method.addStatement($"ldfld      valuetype {Corlib.Name}System." + type + " " + m_refName + "::" + field.Name);
				method.addStatement($"box        {Corlib.Name}System." + type + "");
				method.addStatement("VtFieldReady" + nr.ToString() + ":");
			}
			else if (field.IsProperty) // ValueType mit Property
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement($"call       instance valuetype {Corlib.Name}System." + type + " " + field.Parent.PureTypeName + "::" + getAccName("get_", field.Name) + "()");

				method.addStatement($"ldsfld     valuetype {Corlib.Name}System." + type + $" {Corlib.Name}System." + empty);
				method.addStatement($"call       bool {Corlib.Name}System." + type + $"::op_Inequality(valuetype {Corlib.Name}System." + type + $",valuetype {Corlib.Name}System." + type + ")");
				method.addStatement("brtrue.s   VtFieldIsNotEmpty" + nr.ToString());
				method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
				method.addStatement("br.s       VtFieldReady" + nr.ToString());
				method.addStatement("VtFieldIsNotEmpty" + nr.ToString() + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement($"call       instance valuetype {Corlib.Name}System." + type + " " + field.Parent.PureTypeName + "::" + getAccName("get_", field.Name) + "()");
				method.addStatement($"box        {Corlib.Name}System." + type + "");
				method.addStatement("VtFieldReady" + nr.ToString() + ":");
			}
			else // Vt. mit public field
			{
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement($"ldfld      valuetype {Corlib.Name}System." + type + " " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement($"ldsfld     valuetype {Corlib.Name}System." + type + $" {Corlib.Name}System." + empty);
				method.addStatement($"call       bool {Corlib.Name}System." + type + $"::op_Inequality(valuetype {Corlib.Name}System." + type + $", valuetype {Corlib.Name}System." + type + ")");
				method.addStatement("brtrue.s   VtFieldIsNotEmpty" + nr.ToString());
				method.addStatement($"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value");
				method.addStatement("br.s       VtFieldReady" + nr.ToString());
				method.addStatement("VtFieldIsNotEmpty" + nr.ToString() + ":");
				method.addStatement(ldarg_0);
				method.addStatement("ldflda     " + field.Parent.ILType + " " + m_refName + "::" + field.Parent.Name);																																 
				method.addStatement($"ldfld      valuetype {Corlib.Name}System." + type + " " + field.Parent.PureTypeName + "::" + field.Name);
				method.addStatement($"box        {Corlib.Name}System." + type + "");				
				method.addStatement("VtFieldReady" + nr.ToString() + ":");
			}
		}

		private void insertWriteForNullable(ILMethodElement method, ILField field, int nr, bool parentIsValueType)
		{
			string loadDbNull = $"ldsfld     class {Corlib.Name}System.DBNull {Corlib.Name}System.DBNull::Value";
			string callInstance = "call       instance bool " + field.ILType + "::";
			Type t = field.FieldType;
			Type argType = t.GetGenericArguments()[0];
			string argTypeName = new ReflectedType(argType, this.m_classElement.AssemblyName).QuotedILName;
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
				ILLocalsElement localsElement = method.GetLocals();
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
			if (tname == $"valuetype {Corlib.Name}System.Guid")
			{
				insertWriteForVtField(method, field, nr, parentIsValueType, "Guid");
			}
			else if (tname == $"valuetype {Corlib.Name}System.DateTime")
			{
				insertWriteForVtField(method, field, nr, parentIsValueType, "DateTime");
			}
			else if (tname.StartsWith($"valuetype {Corlib.Name}System.Nullable`1"))
			{
				insertWriteForNullable(method, field, nr, parentIsValueType);
			}
            else if (tname.StartsWith("!"))
            {
                insertWriteForGenericTypes(method, field, nr);
            }
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
				if (!(tname == $"class {Corlib.Name}System.String" || tname == $"class {Corlib.Name}System.Byte[]" || tname == "string" || tname == "unsigned int8[]"))
					method.addStatement("box  " + tname);
			}
			method.addStatement($"callvirt   instance void {Corlib.SystemDataCommon}System.Data.DataRow::set_Item(string, object)");
		}


		public void
		addWrite()
		{
			ILMethodElement method = m_classElement.GetMethod( "NDOWrite" );
			if ( method == null )
			{
				method = new ILMethodElement();

				if ( m_hasPersistentBase )
					method.AddLine( ".method public hidebysig virtual" );
				else
					method.AddLine( ".method public hidebysig newslot virtual" );
				method.AddLine( $"instance void  NDOWrite(class {Corlib.SystemDataCommon}System.Data.DataRow dr, string[] fields, int32 startind) cil managed" );
				m_classElement.AppendMethod( method );
			}
			else
			{
				method.ClearElements();
			}
			method.addStatement( ".maxstack  8" );

			addLocalVariable(method, "i", "int32");
			addLocalVariable(method, "ex", $"class {Corlib.Name}System.Exception");

			method.addStatement("ldc.i4.0");
			method.addStatement("stloc.0");

			method.addStatement("ldarg.1");
			method.addStatement("brtrue.s   rownotnull");
			method.addStatement(@"ldstr      ""NDOWrite: DataRow ist null""");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");
			method.addStatement("rownotnull: ");

			ILStatementElement[] fixupElements = new ILStatementElement[2];
			method.addStatement("ldarg.3");
			fixupElements[0] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.AddElement(fixupElements[0]);
			method.addStatement("add");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ble.s      indexbigger");
			method.addStatement(@"ldstr      ""NDOWrite: Index {0} is bigger than maximum index of fields array ({1})""");
			method.addStatement("ldarg.3");
			fixupElements[1] = new ILStatementElement("ldc.i4.s ##fieldcount");
			method.AddElement(fixupElements[1]);
			method.addStatement("add");
			method.addStatement($"box        {Corlib.Name}System.Int32");
			method.addStatement("ldarg.2");
			method.addStatement("ldlen");
			method.addStatement("conv.i4");
			method.addStatement("ldc.i4.1");
			method.addStatement("sub");
			method.addStatement($"box        {Corlib.Name}System.Int32");
			method.addStatement($"call       string {Corlib.Name}System.String::Format(string, object, object)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
			method.addStatement("throw");

			method.addStatement("indexbigger: ");

			method.addStatement(".try {");
 
			int nr = 0;
			// sortedFields contains the inherited fields.
			// But we only need the fields declared in this class.
			// The own fields are in ownFieldsHierarchical
			// but they are not sorted.
			// So we use the fields in the range 0..mappedFieldCount.
			foreach(var e in sortedFields)
			{
				if (nr == this.mappedFieldCount)
					break;
				var field = e.Value;
				if (field.Parent != null && field.Parent.IsEmbeddedType)
					continue;
				addWriteForField(method, field, nr);
				nr++;
			}

			fixupElements[0].ReplaceText("##fieldcount", nr.ToString());
			nr--;
			fixupElements[1].ReplaceText("##fieldcount", nr.ToString());
			nr++;

			method.addStatement("leave.s    aftercatch");
			method.addStatement("}  // end .try");
			method.addStatement($"catch {Corlib.Name}System.Exception");
			method.addStatement("{");
			method.addStatement("stloc.1");
			method.addStatement(@"ldstr      ""Error while reading from field """);
			method.addStatement("ldarg.2");
			method.addStatement("ldloc.0");
			method.addStatement("ldelem.ref");
			method.addStatement(@"ldstr      ""\n""");
			method.addStatement("ldloc.1");
			method.addStatement($"callvirt   instance string {Corlib.Name}System.Exception::get_Message()");
			method.addStatement($"call       string {Corlib.Name}System.String::Concat(string,string,string,string)");
			method.addStatement($"newobj     instance void {Corlib.Name}System.Exception::.ctor(string)");
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
				method.addStatement($"call       instance void {m_persistentBase}::NDOWrite(class {Corlib.SystemDataCommon}System.Data.DataRow dr, string[] fields, int32 startind)" );
			}

			method.addStatement("ret");
		}

		public void removeGetNDOHandler()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOHandler" );
			if ( method != null )
				method.Remove();

			ILPropertyElement propEl = m_classElement.GetProperty( "NDOHandler" );
			if ( propEl != null )
				propEl.Remove();
		}

		public void addGetNDOHandler()
		{
			ILMethodElement method = m_classElement.GetMethod( "get_NDOHandler" );

			m_classElement.AppendField(".field family static class [NDO]NDO.IPersistenceHandler _ndoPersistenceHandler" );

			if ( method == null )
			{
				method = new ILMethodElement();
				method.AddLine( ".method public hidebysig newslot specialname final virtual instance class [NDO]NDO.IPersistenceHandler get_NDOHandler() cil managed" );
				m_classElement.AddElement( method );
			}
			else
			{
				method.ClearElements();
			}
			method.AddElement(new ILMaxstackElement(".maxstack  1", method));
			method.AddElement(new ILStatementElement("ldsfld     class [NDO]NDO.IPersistenceHandler " + m_refName + "::_ndoPersistenceHandler"));
			method.AddElement(new ILStatementElement("ret"));

			method = new ILMethodElement();
			method.AddLine(".method public hidebysig static void NDOSetPersistenceHandler(class [NDO]NDO.IPersistenceHandler ph) cil managed");
			method.AddElement(new ILMaxstackElement(".maxstack  1", method));
			method.AddElement(new ILStatementElement(ldarg_0));
			method.AddElement(new ILStatementElement("stsfld     class [NDO]NDO.IPersistenceHandler " + m_refName + "::_ndoPersistenceHandler"));
			method.AddElement(new ILStatementElement("ret"));
			m_classElement.AddElement(method);

			ILPropertyElement propEl = m_classElement.GetProperty( "NDOHandler" );
			if ( propEl == null )
			{
				propEl = new ILPropertyElement();
				propEl.AddLine( ".property instance class [NDO]NDO.IPersistenceHandler NDOHandler()" );
				propEl.AddElement( new ILCustomElement( $".custom instance void {Corlib.SystemComponentModel}System.ComponentModel.BrowsableAttribute::.ctor(bool) = ( 01 00 00 00 00 )", propEl ) );
				propEl.AddElement( new ILCustomElement( $".custom instance void {Corlib.XmlIgnoreAssembly}System.Xml.Serialization.XmlIgnoreAttribute::.ctor() = ( 01 00 00 00 )", propEl ) );
				propEl.AddElement( new ILGetElement( ".get instance class [NDO]NDO.IPersistenceHandler " + m_nonGenericRefName + "::get_NDOHandler()" ) );
				m_classElement.AddElement( propEl );
			}
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

	}  // internal class Class


	/// <summary>
	/// The class represents a persistent field
	/// </summary>
	internal class ILField : IComparable
	{
		protected string		m_name;
		protected string		m_ilType;
		private bool			isValueType;
        private string assemblyName;
        public List<ILField> Fields;
		private ILField parent;
		private bool isProperty = false;
		string pureTypeName;
		bool valid = true;
		private bool isEmbeddedType = false;
		private bool isEnum = false;
		private IEnumerable<FieldNode> embeddedFieldList;
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

        public ILField(Type type, string iltype, string name, string assemblyName, IEnumerable<FieldNode> embeddedFieldList, bool isEnum)
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

			if (isEmbeddedType)
			{
				Fields = new List<ILField>();
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
					Fields = new List<ILField>();

					if (!vtn.Fields.Any())
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
				referencedType = ILCode.ILType.GetNetTypeName(m_ilType);
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
