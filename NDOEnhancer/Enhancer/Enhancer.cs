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
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Data;
using Microsoft.Win32;

using NDO;
using NDO.Mapping;
using NDO.Mapping.Attributes;

using ILCode;
using System.Reflection;


namespace NDOEnhancer
{
	/// <summary>
	/// Der ganze Enhancement-Prozess beginnt bei DoIt()
	/// </summary>
	internal class Enhancer
	{
		public
			Enhancer( ProjectDescription projectDescription, MessageAdapter messages )
		{
			this.projectDescription	= projectDescription;
			this.debug		= projectDescription.Debug;
			this.binFile	= projectDescription.BinFile;
			this.objPath	= projectDescription.ObjPath;
			this.messages	= messages;

			binPdbFile = Path.Combine(Path.GetDirectoryName( binFile ), Path.GetFileNameWithoutExtension( binFile ) + ".pdb");
            tempDir = Path.Combine(objPath, "ndotemp");
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);
            string fileWithoutExtension = Path.GetFileNameWithoutExtension(binFile);
			ilFileName	 = Path.Combine(tempDir, fileWithoutExtension + ".org.il");
            resFile = Path.Combine(tempDir, fileWithoutExtension + ".org.res");
            resEnhFile = Path.Combine(tempDir, fileWithoutExtension + ".res");
            ilEnhFile = Path.Combine(tempDir, fileWithoutExtension + ".il");
            objFile = Path.Combine(objPath, Path.GetFileName(binFile));
            enhFile = Path.Combine(tempDir, Path.GetFileName(binFile));
            enhPdbFile = Path.Combine(tempDir, fileWithoutExtension + ".pdb");
			projPath = projectDescription.ProjPath;
            schemaFile = Path.Combine(Path.GetDirectoryName(binFile), fileWithoutExtension + ".ndo.xsd");
			mappingDestFile = Path.Combine(Path.GetDirectoryName( binFile ), "NDOMapping.Xml");
			mappingFile = projectDescription.DefaultMappingFileName;
			options = projectDescription.NewConfigurationOptions();

			//			foreach (EnvDTE.Property p in project.Properties)
			//				messages.WriteLine("  " + p.Name + " " + p.Value.ToString());

		}

		private ProjectDescription	projectDescription;
		private bool				debug;
		private bool				isEnhanced;
        private bool                verboseMode;
		private string				oidTypeName = null;
		private bool				hasPersistentClasses;
		private string				binFile;
		private string				binPdbFile;
		private string				objPath;
        private string              tempDir;
        private string              projPath;

		private string				ilFileName;
		private string				resFile;
        private string              resEnhFile;
        private string              ilEnhFile;
		private string				objFile;
		private string				enhFile;
		private string				enhPdbFile;
		private string				schemaFile;
		private string				mappingDestFile;
		private string				mappingFile;
		private string				ownAssemblyName = null; 
		private StreamWriter		sortedFieldsFile;


		
		private ClassHashtable		allPersistentClasses = new ClassHashtable();
		private Hashtable			allSortedFields = new ClassHashtable();
		private Hashtable			allReferences = new ClassHashtable();
		private Hashtable			assemblyFullNames = new Hashtable();
		private IList				tabuClasses = new ArrayList();
		private NDOMapping          mappings;
		private MessageAdapter		messages;
		private NDODataSet			dsSchema;
		private ConfigurationOptions options;
		private string				assemblyKeyFile = null;
		

		

		private static string assemblyPath = null;


		public static string AssemblyPath
		{
			get 
			{
				if (null == assemblyPath)
				{
#if NDO11
					RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{D861E693-1993-4C4E-B9A7-5657D7F4F338}\InprocServer32");
#endif
#if NDO12
					RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{D861E693-1993-4C4E-B9A7-5657D7F4F339}\InprocServer32");
#endif
#if NDO20
					RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{D861E693-1993-4C4E-B9A7-5657D7F4F33A}\InprocServer32");
#endif
                    if (key == null)
						throw new Exception("Can't find NDO dll in the registry. Please reinstall NDO.");
					assemblyPath = Path.GetDirectoryName((string) key.GetValue(string.Empty));
				}
				return assemblyPath;
			}
		}

        void CheckNDO(Assembly ass)
        {
            AssemblyName[] references = ass.GetReferencedAssemblies();
            NDOAssemblyName refAn = null;
            foreach (AssemblyName an in references)
            {
                if (an.Name == "NDO")
                    refAn = new NDOAssemblyName(an.FullName);
            }
            if (refAn == null)
                return;
            NDOAssemblyName ndoAn = new NDOAssemblyName(typeof(NDOPersistentAttribute).Assembly.FullName);
            if (refAn.PublicKeyToken != ndoAn.PublicKeyToken)
            {
                throw new Exception("Assembly " + ass.FullName + " references a wrong NDO.dll. Expected: " + ndoAn.FullName + ". Found: " + refAn.FullName + ".");
            }
        }




		private void 
			searchPersistentBases()
		{
			Dictionary<string, NDOReference> references = projectDescription.References;
			ArrayList ownClassList = null;
			AssemblyName binaryAssemblyName = null;

            if (!projectDescription.IsWebProject)
            {
                // Check, if we can load the project bin file.
                try
                {
                    binaryAssemblyName = AssemblyName.GetAssemblyName(projectDescription.BinFile);
                }
                catch (Exception ex)
                {
                    throw new Exception("Can't load referenced Assembly '" + projectDescription.BinFile + ". " + ex.Message);
                }
            }

			// Durchsuche alle referenzierten Assemblies
			foreach (NDOReference reference in references.Values)
			{
				if ( !reference.CheckThisDLL )
					continue;

				string dllPath = reference.Path;
				if (dllPath.IndexOf(@"Microsoft.NET\Framework") != -1 
					|| String.Compare(Path.GetFileName(dllPath), "NDO.dll") == 0
                    || String.Compare(Path.GetFileName(dllPath), "NDOInterfaces.dll") == 0)
					continue;

				AssemblyName assToLoad = null;
				Assembly ass = null; 
				try
				{
					assToLoad = AssemblyName.GetAssemblyName(dllPath);
					NDOAssemblyName ndoAn = new NDOAssemblyName(assToLoad.FullName);
					// Don't need to analyze Microsofts assemblies.
					if ( ndoAn.PublicKeyToken == "b03f5f7f11d50a3a" || ndoAn.PublicKeyToken == "b77a5c561934e089" )  
						continue;
					ass = Assembly.Load(assToLoad);
				}
				catch (Exception ex)
				{
					if (assToLoad != null && (binaryAssemblyName == null || string.Compare(binaryAssemblyName.FullName, assToLoad.FullName, true) != 0))
					{
						// Not bin file - enhancer may work, if assembly doesn't contain
						// persistent classes.
						messages.WriteLine("Can't load referenced Assembly '" + dllPath +". The enhancer may not work correctly.");
					}
					else
					{
						throw new Exception("Can't load referenced Assembly '" + projectDescription.BinFile + ". " + ex.Message);
					}
					continue;
				}

				string assName = assToLoad.Name;
				if (this.assemblyFullNames.Contains(assName))
				{
					messages.WriteLine("Assembly '" + assName + "' analyzed twice. Check your enhancer parameter file.");
					continue;
				}
				this.assemblyFullNames.Add(assName, assToLoad.FullName);

                if (verboseMode)
                {
                    messages.WriteLine("Checking DLL: " + dllPath);
                    messages.WriteLine("BinFile: " + binFile);
                }
				bool ownAssembly = (string.Compare(dllPath, binFile, true) == 0);
				AssemblyNode assemblyNode = null;
                CheckNDO(ass);
				//############## searcher neu #####################
				try
				{
					assemblyNode = new AssemblyNode(ass, this.mappings);
				}
				catch (Exception ex)
				{
                    if (verboseMode)
					    messages.ShowError("Error while reflecting types of assembly " + dllPath + ". " + ex);
                    else
				    	messages.ShowError("Error while reflecting types of assembly " + dllPath + ". " + ex.Message);
				}
				if (ownAssembly)
				{
					ownClassList = assemblyNode.PersistentClasses;
					this.isEnhanced = assemblyNode.IsEnhanced;
					this.oidTypeName = assemblyNode.OidTypeName;
					this.ownAssemblyName = assName;
				}
				ArrayList classList = assemblyNode.PersistentClasses;
				foreach(ClassNode classNode in classList)
				{
					string clName = classNode.Name;
					if (!allPersistentClasses.Contains(clName))
						allPersistentClasses.Add(clName, classNode);
					else if (verboseMode)
						messages.WriteLine("Multiple definition of Class " + clName + '.');
				}

				// Wir haben externe persistente Klassen gefunden.
				// Mapping-Info einlesen.
				if (!ownAssembly)
					mergeMappingFile(dllPath, classList);
				if (!ownAssembly) 
					mergeDataSet(dllPath, classList);

				if (ownAssembly)
				{
					this.hasPersistentClasses = (classList.Count > 0);
				}

				// TODO: Check, how to handle value types
//				XmlNodeList vtnl = pcDoc.SelectNodes(@"/PersistentClasses/ValueType");
//				ValueTypes.Instance.Merge(vtnl);
			}

            if (projectDescription.IsWebProject)
            {
                ownClassList = new ArrayList();
            }
            else
            {
                if (ownClassList == null)
                    throw new Exception("A reference to the assembly " + binFile + " is needed. Check your parameter file.");
            }
#if PRO
			checkTypeList();
#endif

			checkClassMappings(ownClassList);
            checkOidColumnMappings(); // Incorporates the Attributes and NDOOidType.
			checkAllRelationMappings(ownClassList); // Makes sure, that a mapping entry for each relation exists
            determineOidTypes(); // needs the relation mappings, calls InitFields
            checkRelationForeignKeyMappings(); // Calls r.InitFields, therefore requires, that InitFields for the Oid's was called
			generateAllSchemas();
            removeRedundantOidColumnNames();
		}


        private void removeRedundantOidColumnNames()
        {
            foreach (Class cl in mappings.Classes)
            {
                new OidColumnIterator(cl).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    if (oidColumn.FieldName == string.Empty)
                        oidColumn.FieldName = null;
                    if (oidColumn.RelationName == string.Empty)
                        oidColumn.RelationName = null;
                    if (oidColumn.FieldName != null || oidColumn.RelationName != null)
                    {
                        oidColumn.Name = null;      // Let the field or relation define the name
                        oidColumn.NetType = null;   // Let the field or relation determine the type
                    }
                });
            }
        }

        private void checkOidColumnMappings()
        {
            foreach (Class cl in mappings.Classes)
            {

                ClassNode classNode = allPersistentClasses[cl.FullName];
                if (classNode == null)
                {
                    messages.WriteLine("Warning: can't find ClassNode for class " + cl.FullName);
                    continue;
                }
                ClassOid oid = cl.Oid;
                if (oid.OidColumns.Count == 0)
                {
                    OidColumn oidColumn = oid.NewOidColumn();
                    oidColumn.Name = "ID";
                }

                // Check, if the oid columns match the OidColumnAttributes, if any of them exist
                // In case of NDOOidType the ClassNode constructor creates an OidColumnAttribute

                // We decided, not to remap oid's since explicit entries for a certain class in the
                // mapping file would be overwritten by assembly-wide attribute declarations.

                // oid.RemapOidColumns(classNode.ColumnAttributes);

                bool noNameError = false;

                new OidColumnIterator(cl).Iterate(delegate(OidColumn oidColumn, bool isLastElement)
                {
                    if (string.IsNullOrEmpty(oidColumn.Name) && string.IsNullOrEmpty(oidColumn.FieldName) && string.IsNullOrEmpty(oidColumn.RelationName))
                    {
                        noNameError = true;
                        messages.WriteLine("Error: Oid column of class " + cl.FullName + " doesn't have a column name.");
                    }
                });
                if (noNameError)
                    throw new Exception("If you define several OidColumns with the OidColumnAttribute, you have to assign a name for each column.");
            }
        }

        /// <summary>
        /// Checks, if all foreign key mapping entries match the oid columns of the target types
        /// </summary>
        private void checkRelationForeignKeyMappings()
        {
            // Now check, if all relations have correct foreign keys
            foreach (Class cl in mappings.Classes)
            {
                foreach (Relation r in cl.Relations)
                {
                    /*
                     * 1. klären, ob ForeignKeyColumns oder ChildForeignKeyColumns relevant sind
                     *      - Multiplicity  Ist die schon bekannt?
                     *      - ChildForeignKeyColumnAttributes
                     *      - r.MappingTable  RelationMappings sollten schon bestimmt sein
                     * 2. Count vergleichen, bei MappingTable auch für eigenen Oid-Typ. Warnung bei !=
                     * 3. Bei Bedarf remappen
                     * Präzedenz:   1. Attribute
                     *              2. Mapping File
                     *              3. Defaults
                     */
                    //Class relClass = allPersistentClasses[r.ReferencedTypeName].Class;
                    //if (relClass.Oid.OidColumns != r.
                    //r.RemapForeignKeyColumns();
                }

                foreach (IFieldInitializer fi in cl.Relations)
                    fi.InitFields();

            }
        }
				
		private IList checkRelationTargetAssemblies()
		{
			ArrayList foreignAssemblies = new ArrayList();
			foreach(Class cl in this.mappings.Classes)
			{
				foreach(Relation r in cl.Relations)
				{
					ClassNode classNode = this.allPersistentClasses[r.ReferencedTypeName];
					if (classNode == null)
						throw new InternalException(242, "Enhancer.checkRelationTargetAssemblies");
					if (classNode.AssemblyName != this.ownAssemblyName)
					{
						if (!foreignAssemblies.Contains(classNode.AssemblyName))
							foreignAssemblies.Add(classNode.AssemblyName);
					}
				}
			}
			return foreignAssemblies;
		}
		
#if nix   // should now be done in InitFields
		private void checkAllMultiplicities()
		{
			foreach(Class cl in mappings.Classes)
			{
				foreach(Relation r in cl.Relations)
				{
					// Setting the class with reflection
					RelationNode definingNode = getDefiningNodeForRelation(r);
                    if (definingNode == null) // Relation might be deleted
                        continue;
					Class definingClass = allPersistentClasses[definingNode.Parent.Name].Class;
					SetDefiningClass(r, definingClass);

					r.Multiplicity = definingNode.IsElement 
						? RelationMultiplicity.Element 
						: RelationMultiplicity.List;
				}
			}
		}

		private RelationNode getDefiningNodeForRelation(Relation r)
		{
			ClassNode classNode = allPersistentClasses[r.Parent.FullName];
			if (classNode == null)
				throw new InternalException(239, "Enhancer: getDefiningClassForRelation; can't find ClassNode for " + r.Parent.FullName);

			while (classNode != null)
			{
				foreach(RelationNode rn in classNode.Relations)
					if (rn.Name == r.FieldName)
						return rn;
				classNode = this.allPersistentClasses[classNode.BaseName];
			}
			//throw new Exception("Can't find defining ClassNode for relation " + r.Parent.FullName + "." + r.FieldName + ". Check your mapping file for unnecessary Relation entries.");
            return null; // Relation might be deleted
		}

#endif

#if PRO

		private void checkTypeList()
		{
			string typeFile = Path.Combine(Path.GetDirectoryName(binFile), "NDOTypes.Xml");
			TypeManager tm = new TypeManager(typeFile);
			tm.CheckTypeList(allPersistentClasses);
			if (!File.Exists(typeFile))
				this.messages.WriteLine("Generating type list file " + typeFile);
			else if (tm.IsModified)
				this.messages.WriteLine("Updating type list file");
					
			tm.Update();			
		}
#endif

		void generateAllSchemas()
		{
            dsSchema.Remap(mappings);
#if nix
			foreach(DictionaryEntry de in this.allPersistentClasses)
			{
				ClassNode classNode = (ClassNode) de.Value;
				if (classNode.IsAbstractOrInterface)
					continue;
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				string pureName = classNode.Name;
				Class classMapping = classNode.Class;
				new SchemaGenerator(classMapping, mappings, dsSchema, messages, allSortedFields, allPersistentClasses, verboseMode).GenerateTables();
			}
			foreach(DictionaryEntry de in this.allPersistentClasses)
			{
				ClassNode classNode = (ClassNode) de.Value;
				if (classNode.IsAbstractOrInterface)
					continue;
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				string pureName = classNode.Name;
				Class classMapping = classNode.Class;
				new SchemaGenerator(classMapping, mappings, dsSchema, messages, allSortedFields, allPersistentClasses, verboseMode).GenerateRelations();
			}
#endif
		}



		public void
		mergeDataSet(string absDllPath, ArrayList classList)
		{			
			string dsFile = Path.Combine(Path.GetDirectoryName(absDllPath), Path.GetFileNameWithoutExtension(absDllPath) + ".ndo.xsd");
			if (!File.Exists(dsFile))
				return;

			NDODataSet dsToMerge = new NDODataSet(dsFile);
			foreach(DataTable dt in dsToMerge.Tables)
			{
				if (null == dsSchema.Tables[dt.TableName])
				{
					DataTable newdt = dt.Clone();
					dsSchema.Tables.Add(newdt);
				}
			}
			foreach(DataRelation dr in dsToMerge.Relations)
			{
				if (null == dsSchema.Relations[dr.RelationName])
				{
					DataRelation newdr = null;
					try
					{
						dsSchema.Relations.Add( newdr = new DataRelation( dr.RelationName, dsSchema.Tables[dr.ParentTable.TableName].Columns[dr.ParentColumns[0].ColumnName], dsSchema.Tables[dr.ChildTable.TableName].Columns[dr.ChildColumns[0].ColumnName], true ) );
					}
					catch(Exception ex)
					{
                        string relName = "null";
                        if (dr != null && dr.RelationName != null)
                            relName = dr.RelationName;

						throw new Exception("Error while merging relation '" + relName + "' into the new dataset: " + ex.Message);
					}
					newdr.ChildKeyConstraint.DeleteRule = dr.ChildKeyConstraint.DeleteRule;
					newdr.ChildKeyConstraint.UpdateRule = dr.ChildKeyConstraint.UpdateRule;
				}
			}
		}

#if nix
		private void getOidType(FieldNode fieldNode, Class cls)
		{
			Type t = fieldNode.OidType;
			if (t == null)
			{
				messages.WriteLine(String.Format("Invalid Oid type {0} in class {1} - using int instead.", fieldNode.DataType, cls.FullName));
				t = typeof(int);
			}
			cls.Oid.FieldType = t;
		}
#endif
		private void
		determineOidTypes()
		{
            foreach (Class cl in mappings.Classes)
            {
                ClassNode classNode = (ClassNode)allPersistentClasses[cl.FullName];
                cl.IsAbstract = classNode.IsAbstract;  // usually this is set in Class.InitFields, which isn't called by the Enhancer.
                cl.SystemType = classNode.ClassType;
            }
			foreach(Class cl in mappings.Classes)
			{
                string className = cl.FullName;

                // Even abstract types should have an oid type,
                // because they can be targets of relations - thus 
                // we need a foreign key column type.

                ClassOid oid = cl.Oid;
				if (oid == null)
					throw new Exception("MergeOidTypes: Can't find Oid Mapping for class " + className);

#if DEBUG
                if (cl.FullName.IndexOf("OrderDetail") > -1)
                    Console.WriteLine();
#endif

                ((IFieldInitializer)oid).InitFields();
			}
		}


		private void checkMappingForField(string prefix, Patcher.ILField field, ArrayList sortedFields, Class classMapping, bool isOnlyChild, bool isOidField)
		{
			if (field.CleanName.StartsWith("_ndo"))
			{
                if (this.verboseMode)
				    messages.WriteLine("Warning: **** found _ndo field: " + field.CleanName);
				return;
			}
			string fname = prefix + field.CleanName;
			if (field.HasNestedFields)
			{
				bool oneChildOnly = field.Fields.Count == 1;
				foreach(Patcher.ILField newf in field.Fields)
				{
					checkMappingForField(field.CleanName + ".", newf, sortedFields, classMapping, oneChildOnly, false);
				}
				return;
			}
			DictionaryEntry deToDelete = new DictionaryEntry(string.Empty, string.Empty);
			if (field.IsInherited)
			{
				foreach(DictionaryEntry de in sortedFields)
					if (de.Key.Equals(fname))
						deToDelete = de;
			}
			if (((string)deToDelete.Key) != string.Empty)
				deToDelete.Value = field;
			else
				sortedFields.Add(new DictionaryEntry(fname, field));

			if (classMapping != null)
			{
				Field f = classMapping.FindField(fname);
				try
				{
					if (null == f)
					{
						f = classMapping.AddStandardField(fname, isOidField);
						if (isOnlyChild)
							f.Column.Name = classMapping.ColumnNameFromFieldName(field.Parent.CleanName, false);
						messages.WriteLine("Generating field mapping: " + classMapping.FullName + "." + fname + " -> " + f.Column.Name);
						if (classMapping.IsAbstract)
							f.Column.Name = "Unused";
					}
				}
				catch(ArgumentOutOfRangeException) 
				{
					throw new Exception("NDO Communitiy version: Too much fields in class " + classMapping.FullName);
				}
			}
		}

		/// <summary>
		/// Helps sorting the fields
		/// </summary>
		private class FieldSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				if (!(x is DictionaryEntry) || !(y is DictionaryEntry))
					throw new Exception("Interner Fehler: FieldSorter.Compare: DictionaryEntry erwartet; x= " + x.GetType().FullName + " y = " + y.GetType().FullName);
				return String.CompareOrdinal((string)((DictionaryEntry) x).Key, (string)((DictionaryEntry) y).Key);
			}
		}


		private void IterateFieldNodeList(IList fieldList, bool isEmbeddedObject, 
			List<FieldNode> oidFields, Class classMapping, ArrayList sortedFields, 
			bool isInherited)
		{
			string tn;
			foreach(FieldNode fieldNode in fieldList)
			{
				// Beim Anlegen eines Fields werden auch gleich die SubFields angelegt,
				// wenn das Field ein Value Type ist
				string thename = fieldNode.Name;
				tn = fieldNode.DataType;
				//Debug.WriteLine(tn);
				string pattern = @"(class\s|valuetype\s|)(\[[^\]]+\]|)";
				Regex regex = new Regex(pattern);
				Match match = regex.Match(tn);
				if (match.Groups[2].Value == "[" + ownAssemblyName + "]")
					tn = tn.Replace(match.Groups[2].Value, "");

				if (!isEmbeddedObject && fieldNode.IsOid)
					oidFields.Add(fieldNode);
				IList subFieldList = null;
				if (isEmbeddedObject)
				{
					subFieldList = fieldNode.Fields;
				}

				bool isEnum = (fieldNode.IsEnum);
				Patcher.ILField field = new Patcher.ILField(fieldNode.FieldType, tn, 
					fieldNode.Name, this.ownAssemblyName, subFieldList, isEnum);
				field.IsInherited = isInherited;
				System.Diagnostics.Debug.Assert (field.Valid, "field.Valid is false");
				if (classMapping != null)
					checkMappingForField("", field, sortedFields, classMapping, false, fieldNode.IsOid);
			}
		}
			 

		private void
		checkFieldMappings(ClassNode classNode, Class classMapping)
		{
			ArrayList sortedFields = new ArrayList();
			string className = classNode.Name;
			allSortedFields.Add(className, sortedFields);

			List<FieldNode> oidFields = new List<FieldNode>();

			// All own fields
			IList fieldList = classNode.Fields;
			IterateFieldNodeList(fieldList, false, oidFields, classMapping, sortedFields, false);

			// All embedded objects
			IList embeddedObjectsList = classNode.EmbeddedTypes;
			IterateFieldNodeList(embeddedObjectsList, true, oidFields, classMapping, sortedFields, false);

			FieldSorter fieldSorter = new FieldSorter();
			sortedFields.Sort(fieldSorter);

			// Alle ererbten Felder
			ClassNode derivedclassNode = this.allPersistentClasses[classNode.BaseName];
			
			while (null != derivedclassNode)
			{
				if (derivedclassNode.IsPersistent)
				{
					int startind = sortedFields.Count;

					IList nl = derivedclassNode.Fields;
					IterateFieldNodeList(nl, false, oidFields, classMapping, sortedFields, true);
				
					IList enl = derivedclassNode.EmbeddedTypes;
					IterateFieldNodeList(enl, true, oidFields, classMapping, sortedFields, true);

					int len = sortedFields.Count - startind;
					sortedFields.Sort(startind, len, fieldSorter);
				}
				derivedclassNode = this.allPersistentClasses[derivedclassNode.BaseName];
			}

			// Ab hier nur noch Zeug für die Mapping-Datei
			if (classMapping == null)
				return;


			if (oidFields.Count > 0)
			{
                foreach (FieldNode oidField in oidFields)
                {
                    OidColumn oidColumn = null;
                    new OidColumnIterator(classMapping).Iterate(delegate(OidColumn oidCol, bool isLastElement)
                    {
                        if (oidCol.FieldName == oidField.Name)
                            oidColumn = oidCol;
                    });
                    if (oidColumn == null)
                    {
                        oidColumn = classMapping.Oid.NewOidColumn();
                        oidColumn.FieldName = oidField.Name;
                        oidColumn.Name = classMapping.FindField(oidField.Name).Column.Name;
                    }
                }
			}

			foreach(DictionaryEntry de in sortedFields)
				sortedFieldsFile.WriteLine((string)de.Key);
			sortedFieldsFile.WriteLine();


			// Und nun die überflüssigen entfernen
			ArrayList fieldsToRemove = new ArrayList();
			foreach(Field f in classMapping.Fields)
			{
				bool isExistent = false;
				foreach(DictionaryEntry e in sortedFields)
				{
					if ((string)e.Key == f.Name)
					{
						isExistent = true;
						break;
					}
				}
				if (!isExistent)
					fieldsToRemove.Add(f);
			}
			foreach(object o in fieldsToRemove)
				classMapping.Fields.Remove(o);

            ArrayList fieldMappings = new ArrayList(classMapping.Fields);
            fieldMappings.Sort(new FieldMappingSorter());
            for (int i = 0; i < fieldMappings.Count; i++)
                ((Field)fieldMappings[i]).Ordinal = i;
		}

        private class FieldMappingSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                return string.CompareOrdinal(((Field)x).Name, ((Field)y).Name);
            }
        }

		private void SetDefiningClass(Relation r, Class parent)
		{
			Type t = typeof(Relation);
			FieldInfo fi = t.GetField("definingClass", BindingFlags.NonPublic | BindingFlags.Instance);
			fi.SetValue(r, parent);
		}

		public void
		checkInheritedRelationMappings(ClassNode classNode, Class classMapping)
		{
			string className = classNode.Name;

			ReferenceArrayList references = (ReferenceArrayList) this.allReferences[className];
			// Alle ererbten Relationen
			ClassNode baseClassNode = this.allPersistentClasses[classNode.BaseName];
			
			while (null != baseClassNode)
			{
				if (baseClassNode.IsPersistent)
				{
					Class baseClassMapping = baseClassNode.Class;
					ArrayList temp = new ArrayList();
					IList nl = baseClassNode.Relations;
					foreach (RelationNode relNode in nl)
					{
						// Relation nur aus der Klasse nehmen, die das Feld deklariert
						if (relNode.DeclaringType != null)
							continue;
						//					string tn = relNode.Attributes["Type"].Value;
						RelationInfo ri = relNode.RelationInfo;
						string rname = relNode.Name;
						
						// Die Relation sollte wirklich am unteren Ende der
						// Klassenhierarchie erzeugt werden.
						//					if(references.Contains(rname))
						//					Debug.Write("NDO635");
						Debug.Assert(!references.Contains(rname));
										

						// relation wird als ererbte Reference in die Liste eingetragen.
						bool is1To1 = relNode.IsElement;
						string relTypeName = relNode.RelatedType;
						string relName = relNode.RelationName;
						string ilType = relNode.DataType;
						Type containerType = relNode.FieldType;

						ClassNode relClassNode = allPersistentClasses[relTypeName];
						if (relClassNode.AssemblyName != this.ownAssemblyName && !relTypeName.StartsWith("["))
							relTypeName = "[" + relClassNode.AssemblyName + "]" + relTypeName;
						//TODO: warnen wenn Oid-Typen nicht übereinstimmen
						references.Add(new Patcher.ILReference(containerType, relTypeName, ilType, rname, this.ownAssemblyName, ri, relName, true, is1To1, "class " + className));

						if (classMapping != null)
						{
							// Ist diese Relation schon definiert? Wenn ja, wird sie nicht geändert
							Relation r = classMapping.FindRelation(rname);
							if (null == r)
							{
								messages.WriteLine(String.Format("Ererbte Relation {0}.{1} wird kopiert", classNode.Name, rname));
								if (null == baseClassMapping)
									throw new Exception(String.Format("Kann die Mapping-Information für die Basisklasse {0} nicht finden", baseClassNode.Name));
								r = baseClassMapping.FindRelation(rname);
								if (r == null)
									throw new Exception(String.Format("Schwerwiegender interner Fehler: Ererbte Relation {0} in Basisklasse {1} nicht gefunden.", rname, baseClassMapping.FullName));
								classMapping.Relations.Add(r);
							}
							if (is1To1)
								r.Multiplicity = RelationMultiplicity.Element;
							else
								r.Multiplicity = RelationMultiplicity.List;
						}
					}
				}
				baseClassNode = this.allPersistentClasses[baseClassNode.BaseName];
			}
		}

		public void
		checkRelationMappings(ClassNode classNode, Class classMapping)
		{
			IList references = new ReferenceArrayList();
			string className = (classNode.Name);
			allReferences.Add(className, references);
			IList refList = classNode.Relations;
			foreach (RelationNode relationNode in refList)
			{
				// Übernehme die Relation nur aus der deklarierenden Klasse,
				// damit gleich die richtigen Mappings eingetragen werden.
				if (relationNode.DeclaringType != null)
					continue;
				string fieldName = relationNode.Name;
				string relTypeName = relationNode.RelatedType;
				bool is1To1 = relationNode.IsElement;
				string relName = relationNode.RelationName;
				RelationInfo ri = relationNode.RelationInfo;
				string ilType = relationNode.DataType;
				Type containerType = relationNode.FieldType;

				if (!classNode.IsInterface)
				{
					if (classMapping == null)
						continue;
					Relation r = classMapping.FindRelation(fieldName);
                    if (null == r)
                    {
                        //TODO: ForeignKeyColumnAttributes...
                        string relTypeFullName = relTypeName.Substring(relTypeName.IndexOf("]") + 1);
                        ClassNode relClassNode = allPersistentClasses[relTypeName];
                        if (relClassNode == null)
                            throw new Exception(String.Format("Class '{1}' has a relation to a non persistent type '{0}'.", relTypeFullName, classNode.Name));
                        messages.WriteLine("Creating standard relation " + classNode.Name + "." + fieldName);
                        r = classMapping.AddStandardRelation(fieldName, relTypeFullName, is1To1, relName, classNode.IsPoly, relClassNode.IsPoly || relClassNode.IsAbstract);
                    }
                    else
                    {
                        //TODO: Do remapping here
                    }
					SetDefiningClass(r, classMapping);
					if (is1To1)
						r.Multiplicity = RelationMultiplicity.Element;
					else
						r.Multiplicity = RelationMultiplicity.List;
				}
				references.Add(new Patcher.ILReference(containerType, relTypeName, ilType, fieldName, this.ownAssemblyName, ri, relName, false, is1To1, null));
			}
		}


		private void
		checkClassMappings(ArrayList classList)
		{
			if (options.DatabaseOwner != string.Empty)
				mappings.StandardDbOwner = options.DatabaseOwner;
			sortedFieldsFile = new StreamWriter(Path.ChangeExtension(binFile, ".fields.txt"));
			foreach(ClassNode classNode in classList)
			{
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				string assName = classNode.AssemblyName;
                if (verboseMode)
                {
                    if (assName != this.ownAssemblyName)
                        messages.WriteLine("Warning: Inconsistency: Class from foreign assembly: " + classNode.Name);
                }
				Class classMapping = null;
				if (!classNode.IsInterface)
				{
					string className = classNode.Name;
					sortedFieldsFile.WriteLine(className + ":");
					classMapping = mappings.FindClass(className);
					if (classMapping == null)
					{
#if !STD
						try
						{
#endif
							messages.WriteLine("Generating class mapping for class '" + className + "'");

							if (classNode.IsAbstract)
								classMapping = mappings.AddAbstractClass(className, assName, classNode.ColumnAttributes);
							else
								classMapping = mappings.AddStandardClass(className, assName, classNode.ColumnAttributes);
						
#if !STD
						}
						catch (ArgumentOutOfRangeException ex)
						{
							throw new Exception(Decryptor.Decrypt(NDOErrors.TooMuchClasses));
						} 
#endif
                            
					}
					if (options.UseTimeStamps && (classMapping.TimeStampColumn == null || classMapping.TimeStampColumn == string.Empty))
						classMapping.TimeStampColumn = "NDOTimeStamp";
                    if (classNode.ClassType.IsGenericType && classMapping.TypeNameColumn == null)
                        classMapping.AddTypeNameColumn();
				}
				checkFieldMappings(classNode, classMapping);
			}
			sortedFieldsFile.Close();

			// Lösche ungebrauchte Class Mappings
			ArrayList classesToDelete = new ArrayList();
			foreach(Class c in mappings.Classes)
			{
				if (!tabuClasses.Contains(c.FullName) 
				&& allPersistentClasses[c.FullName] == null)
					    classesToDelete.Add(c);
			}
			foreach (Class c in classesToDelete)
			{
				messages.WriteLine(String.Format("Deleting unused class mapping {0}", c.FullName));
				mappings.RemoveClass(c);
			}
		}

		private void checkAllRelationMappings(ArrayList classList)
		{
			foreach(ClassNode classNode in classList)
			{
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				Class classMapping = classNode.Class;
				checkRelationMappings(classNode, classMapping);
			}
#if PRO
			foreach(ClassNode classNode in classList)
			{
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				Class classMapping = classNode.Class;
				checkInheritedRelationMappings(classNode, classMapping);
			}
#endif

			foreach(ClassNode classNode in classList)
			{
				if (classNode.IsInterface)
					continue;
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				Class classMapping = classNode.Class;
				if (classMapping == null)
					continue;
				deleteUnusedRelationMappings(classMapping);
				checkDoubleComposites(classMapping);
			}
		}


		private void deleteUnusedRelationMappings(Class classMapping)
		{
			ReferenceArrayList references = (ReferenceArrayList) this.allReferences[classMapping.FullName];
			ArrayList relationsToDelete = new ArrayList();
			foreach (Relation r in classMapping.Relations)
			{
				if (!references.Contains(r.FieldName))
					relationsToDelete.Add(r);
			}
			foreach (Relation r in relationsToDelete)
			{
				messages.WriteLine(String.Format("Delete unused Relation Mapping {0}", classMapping.FullName + "." + r.FieldName));
				classMapping.Relations.Remove(r);
			}
		}

		private void checkDoubleComposites(Class classMapping)
		{
			ReferenceArrayList references = (ReferenceArrayList) this.allReferences[classMapping.FullName];
			foreach (Relation r in classMapping.Relations)
			{
				Patcher.ILReference reference = references.FindReference(r.FieldName);
				if (reference != null)
				{
					Relation r2 = r.ForeignRelation;
					if (r2 != null)
					{
						ReferenceArrayList references2 = (ReferenceArrayList) this.allReferences[r.ReferencedTypeName];
						if(references2 == null)  // Type is not from our assembly
							continue;
						Patcher.ILReference reference2 = references2.FindReference(r2.FieldName);
						if (reference2 != null)
						{
							if (reference.ReferenceInfo == RelationInfo.Composite
								&& reference2.ReferenceInfo == RelationInfo.Composite)
								throw new Exception(String.Format("Error: Bidirectional relation between class {0} and class {1} is a composite in both directions. Please remove the composite flag at one of the two classes.", classMapping.FullName, r.ReferencedTypeName));
						}
					}
				}
			}
		}


		public void
		mergeMappingFile(string absDllPath, ArrayList classList)
		{			
			string mapFileName = Path.Combine(Path.GetDirectoryName(absDllPath), "NDOMapping.xml");
			if (classList.Count > 0 && !File.Exists(mapFileName))
			{
				messages.WriteLine("Mapping file for assembly " + absDllPath + " not found.");
				return;
			}

			NDOMapping mergeMapping;

			try
			{
				mergeMapping = new NDOMapping(mapFileName);
			}
			catch (Exception ex)
			{
				throw new Exception("Can't read mapping file " + mapFileName + ".\n"+ex.Message);
			}
			foreach(Class classMapping in mergeMapping.Classes)
				tabuClasses.Add(classMapping.FullName);

			mappings.MergeMapping(mergeMapping);

			foreach(ClassNode classNode in classList)
			{
				Class cls;
				string className = classNode.Name;
				if (null == (cls = mappings.FindClass(className)))
				{
					messages.WriteLine("Mapping information for class " + className + " in file " + mapFileName + " not found.");
					messages.WriteInsertedLine("Try to recompile the assembly " + absDllPath + ".");
				}
			}
		}

		private string CombinePath(string path, string file)
		{
			string p = path;
			if (p.EndsWith("\\"))
				p = p.Substring(0, p.Length - 1);
			while (file.StartsWith(@"..\"))
			{
				p = Path.GetDirectoryName(p);
				file = file.Substring(3);
			}
			return p + "\\" + file;
		}

		public void
			doIt()
		{

#if BETA || TRIAL
            expired = !NDOKey.CheckDate(new LicenceKey().TheKey, DateTime.Now);
#endif
            bool sourcesUpToDate = !options.EnableEnhancer;
#if BETA || TRIAL
            if (expired)
                throw new Exception("NDO Licence expired");
#endif
#if DEBUG
            this.verboseMode = true;
#else
            this.verboseMode = options.VerboseMode;
#endif
            DateTime objTime = DateTime.MinValue;

            if (options.EnableEnhancer)
            {
                DateTime enhTime;
                DateTime ilEnhTime;

                if (!File.Exists(objFile))
                {
                    messages.WriteLine("Enhancer: Kann Datei " + objFile + " nicht finden");
                    return;
                }

                objTime = File.GetLastWriteTime(objFile);

                if (File.Exists(ilEnhFile) && File.Exists(enhFile))
                {
                    enhTime = File.GetLastWriteTime(enhFile);
                    ilEnhTime = File.GetLastWriteTime(ilEnhFile);

                    // objFile   = obj\debug\xxx.dll
                    // ilEnhFile = obj\ndotemp\xxx.il
                    // enhFile   = obj\ndotemp\xxx.dll
                    // binFile   = bin\debug\xxx.dll
                    if (objTime < ilEnhTime && ilEnhTime <= enhTime)
                    {
                        // Sicherstellen, dass das Binary existiert
                        File.Copy(enhFile, binFile, true);
                        if (debug)
                            File.Copy(enhPdbFile, binPdbFile, true);

                        sourcesUpToDate = true;
                    }
                }
            }
			// Mapping-Datei im Bin-Verzeichnis muss jünger oder gleich alt sein wie Mapping-Source-Datei
			if (sourcesUpToDate)
			{
				if (File.Exists(mappingFile) && File.Exists(mappingDestFile))
				{
					DateTime mapSourceTime = File.GetLastWriteTime(mappingFile);
					DateTime mapDestTime = File.GetLastWriteTime(mappingDestFile);
					sourcesUpToDate = mapDestTime >= mapSourceTime && mapDestTime >= objTime;
                    // Mapping-Datei muss jünger sein als die bin-Datei
                    if (!projectDescription.IsWebProject && !File.Exists(projectDescription.BinFile))
                        throw new Exception("Can't find binary " + projectDescription.BinFile);
                    DateTime binFileTime = File.GetLastWriteTime(projectDescription.BinFile);
                    if (binFileTime > mapSourceTime)
                        sourcesUpToDate = false;
				}
				else 
					sourcesUpToDate = false;
			}

			// Schemadatei muss nach der letzten Kompilierung erzeugt worden sein
#if DEBUG
			sourcesUpToDate = false;
#endif
			if (sourcesUpToDate)
			{
				if (File.Exists(schemaFile))
				{
					if (File.GetLastWriteTime(schemaFile) >= objTime)
						return;
				}
			}
			
			if (options.DefaultConnection != string.Empty)
			{
				Connection.StandardConnection.Name = options.DefaultConnection;
				Connection.StandardConnection.Type = options.SQLScriptLanguage;
			}
			else
			{
				Connection.StandardConnection.Name = Connection.DummyConnectionString;
			}

			string wrongDll = null;

			try
			{
				if (options.NewMapping)
					File.Delete(mappingFile);
                if (this.verboseMode)
				    messages.WriteLine("Mapping file is: " + mappingFile);

#if !STD
				try
				{
#endif
					mappings = new NDOMapping(mappingFile);
					mappings.SchemaVersion = options.SchemaVersion;
                    ((IEnhancerSupport)mappings).IsEnhancing = true;
#if !STD
				}
				catch (ArgumentOutOfRangeException ex)
				{
					if (ex.Source == "System.Collections")
						throw new Exception("Too much elements for Community edition: max. 1 connection, 10 classes, 500 objects, 8 relations per class, 8 fields per class are allowed.");
					else
						throw ex;
				}
#endif
//#if !STD
//				Type t = mappings.Classes.GetType();
//				NDOAssemblyName an = new NDOAssemblyName(t.Assembly.FullName);
//				if (an.Name != "System.Collections")
//				{
//					wrongDll = Decryptor.Decrypt(NDOErrors.WrongDll);
//				}
//#endif
			}
#if !STD
			catch (ArgumentOutOfRangeException ex)
			{
				throw new Exception(Decryptor.Decrypt(NDOErrors.TooMuchElements));
			}
#endif
			catch (Exception ex)
			{
				if (null != ex.Message)
					throw new Exception("Can't find Mapping File " + mappingFile + ".\n" + ex.ToString());
				else
                    throw new Exception("Can't find Mapping File " + mappingFile);
            }

			if (wrongDll != null)
				throw new Exception(wrongDll);

			dsSchema = new NDODataSet();
            dsSchema.EnforceConstraints = false;

			// The mapping und schema files
			// will be merged here
			searchPersistentBases();
			bool doEnhance = options.EnableEnhancer && !this.isEnhanced;
#if DEBUG
			doEnhance = options.EnableEnhancer;
#endif
            if (this.verboseMode)
            {
                messages.WriteLine(options.EnableEnhancer ? "Enhancer enabled" : "Enhancer disabled");
                if (doEnhance)
                    messages.WriteLine(this.isEnhanced ? "Assembly is already enhanced" : "Assembly is not yet enhanced");
                else
                    messages.WriteLine("Assembly won't be enhanced");
            }
            if (doEnhance)
			{
				// Hier wird ILDasm bemüht, um einen Dump des Assemblies herzustellen
				disassemble();

				ILFile ilfile = new ILFile();

				messages.WriteLine( "Enhancing Assembly" );
				messages.Indent();

				// Hier wird der Elementbaum erzeugt
				ilfile.parse( ilFileName );
				ILAssemblyElement.Iterator assit = ilfile.getAssemblyIterator();
				ILAssemblyElement el;
                if (projectDescription.KeyFile == string.Empty)
                    this.assemblyKeyFile = null;
                else
                    this.assemblyKeyFile = projectDescription.KeyFile;
				for (el = assit.getNext(); el != null; el = assit.getNext() )
				{
					if (!el.isExtern())
					{
						ILCustomElement.Iterator cusit = el.getCustomIterator();
						ILCustomElement custEl;
						for (custEl = cusit.getNext(); custEl != null; custEl = cusit.getNext())
						{
							ILCustomElement.AttributeInfo ai = custEl.getAttributeInfo();
							if (ai.Name == "System.Reflection.AssemblyKeyAttribute")
							{
								string s = (string) ai.ParamValues[0];
								if (s != null && s != string.Empty)
								{
									this.assemblyKeyFile = ("@" + s);
									break;
								}
							}
							if (ai.Name == "System.Reflection.AssemblyKeyFileAttribute")
							{
								string s = (string) ai.ParamValues[0];
								if (s != null && s != string.Empty)
								{
									string fn;
									if (s.IndexOf(@":\") == -1)
										fn = CombinePath(this.objPath, s);
									else
										fn = s;
									if (File.Exists(fn))
										this.assemblyKeyFile = fn;
									break;
								}
							}
						}
						if (this.assemblyKeyFile != null)
							break;
					}
				}

				//mergeValueTypes(ilfile);

				analyzeAndEnhance(ilfile);

				messages.Unindent();
				messages.WriteLine( "Generating Binary" );
					
				// Hier wird der enhanced Elementbaum als IL-Datei geschrieben
				ilfile.write( ilEnhFile );

				// ILAsm assembliert das Ganze
				reassemble();
			}


			// Store the mapping information
			messages.WriteLine("Saving mapping file");
			mappings.Save();
            if (verboseMode)
			    Console.WriteLine("Copying mapping file to '" + mappingDestFile + "'");
			File.Copy(mappingFile, mappingDestFile, true);

			messages.WriteLine("Generating schema file");
			if (File.Exists(schemaFile))
				File.Copy(schemaFile, schemaFile.Replace(".ndo.xsd", ".ndo.xsd.bak"), true);
			dsSchema.WriteXmlSchema(schemaFile);

			if (options.GenerateSQL)
			{
				messages.WriteLine("Generating sql file");
				string sqlFileName = schemaFile.Replace(".xsd", ".sql");
                TypeManager tm = null;
                if (options.IncludeTypecodes)
                {
                    string typeFile = Path.Combine(Path.GetDirectoryName(binFile), "NDOTypes.Xml");
                    tm = new TypeManager(typeFile);
                }
				string oldSchemaFile = schemaFile.Replace(".ndo.xsd", ".ndo.xsd.bak");
				NDODataSet dsOld = null;
				if (File.Exists(oldSchemaFile))
				{
                    dsOld = new NDODataSet(oldSchemaFile);
					new SQLDiffGenerator().Generate(options.SQLScriptLanguage, options.Utf8Encoding, dsSchema, dsOld, sqlFileName, mappings, messages);
				}
				if (!this.options.DropExistingElements)
					dsOld = null;  // causes, that no drop statements will be generated.
				new SQLGenerator().Generate(options.SQLScriptLanguage, options.Utf8Encoding, dsSchema, dsOld, sqlFileName, mappings, messages, tm, this.options.GenerateConstraints);
			}

		}


		private string getAssemblyInfo(Assembly ass, string infoName, string defaultStr)
		{
			string assInfo = ass.ToString();
			string result;
			//NDO, Version=1.0.1003.28687, Culture=neutral, PublicKeyToken=null
			int pos = assInfo.IndexOf(infoName);
			if (-1 == pos)
			{
				result = defaultStr;
			}
			else
			{
				pos += infoName.Length;
				result = assInfo.Substring(pos, assInfo.IndexOf(",", pos) - pos);
			}
			return result;
		}

		void analyzeAndEnhance( ILFile ilFile )
		{			
			IList classes = ilFile.getAllClassElements();

			if (!isEnhanced)
			{
				IList foreignAssemblies = checkRelationTargetAssemblies();

				ILAssemblyElement.Iterator ai = ilFile.getAssemblyIterator();
				bool insertData = true;
				bool insertXml = true;
				bool insertNdo = true;
				bool insertNdoInterfaces = true;
				bool insertSystem = true;

				for ( ILAssemblyElement assElem = ai.getNext(); null != assElem; assElem = ai.getNext() )
				{ 
					string nameLower = assElem.getName().ToLower();
					if (foreignAssemblies.Contains(assElem.getName()))
						foreignAssemblies.Remove(assElem.getName());
					string line = assElem.getLine(0);
					if (!assElem.isExtern())
					{
                        ILElement lastEl = assElem.getCustomIterator().getLast();
						lastEl.insertBefore(new ILCustomElement(".custom instance void [NDO]NDO.NDOEnhancedAttribute::.ctor() = ( 01 00 00 00 )", assElem));
					}
					if (line.StartsWith(".assembly"))
					{
						if (nameLower == "system.data")
							insertData = false;
						if (nameLower == "system.xml")
							insertXml = false;
						if (nameLower == "system")
							insertSystem = false;
						if (nameLower == "ndo")
						{
                            if (this.verboseMode)
                            {
                                messages.WriteLine("NDO Dll:");
                                ILElementIterator it = assElem.getAllIterator(true);
                                for (ILElement subEl = it.getFirst(); subEl != null; subEl = it.getNext())
                                    messages.WriteInsertedLine(subEl.getAllLines());
                            }
							insertNdo = false;
#if NDO20
                            if (assElem.Major != 2 && assElem.Minor != 0)
                            {
                                throw new Exception("This assembly is built with NDO.dll Version " + assElem.VersionString.Replace(':', '.')
                                    + ". This NDO enhancer only works with NDO.dll version 2.0. Please correct your assembly reference and rebuild the assembly.");
                            }
#endif
#if NDO12
                            if (assElem.Major != 1 && assElem.Minor != 2)
                            {
                                throw new Exception("This assembly is built with NDO.dll Version " + assElem.VersionString.Replace(':', '.')
                                    + ". This NDO enhancer only works with NDO.dll version 1.2. Please correct your assembly reference and rebuild the assembly.");
                            }
#endif
#if NDO11
							if (assElem.Major != 1 && assElem.Minor != 1)
							{
								throw new Exception("This assembly is built with NDO.dll Version " + assElem.VersionString.Replace(':', '.')
									+ ". This NDO enhancer only works with NDO.dll version 1.1. Please correct your assembly reference and rebuild the assembly.");
							}                      
#endif
                        }
						if (nameLower == "ndointerfaces")
						{
							insertNdoInterfaces = false;
#if NDO12
                            if (assElem.Major != 1 && assElem.Minor != 2)
                            {
                                throw new Exception("This assembly is built with NDOInterfaces.dll Version " + assElem.VersionString.Replace(':', '.')
                                    + ". This NDO enhancer only works with NDOInterfaces.dll version > 1.2. Please correct your assembly reference and rebuild the assembly.");
                            }
#endif
#if NDO11
                            if (assElem.Major != 1 && assElem.Minor != 1)
							{
								throw new Exception("This assembly is built with NDOInterfaces.dll Version " + assElem.VersionString.Replace(':', '.')
									+ ". This NDO enhancer only works with NDOInterfaces.dll version 1.1. Please correct your assembly reference and rebuild the assembly.");
							}                      
#endif
                        }
					}
#if DEBUG
					else
					{
						throw new Exception("Assembly element doesn't start with .assembly.");
					}
#endif
				}

				ai = ilFile.getAssemblyIterator();
				ILAssemblyElement ael = ai.getNext();
				//				string verString;


				if (insertData)
				{
#if NET20
					string line = @".assembly extern System.Data
{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver 2:0:0:0
}
";
#else
					string line = @".assembly extern System.Data
{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver 1:0:3300:0
}
";
#endif
					ael.insertBefore(new ILElement(line));
				}

				if (insertSystem)
				{
#if NET20
					string line = @".assembly extern System
{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver 2:0:0:0
}
";
#else
					string line = @".assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 1:0:5000:0
}
";
#endif
					ael.insertBefore(new ILElement(line));
				}

				if (insertXml)
				{
//					Assembly ass = Assembly.GetAssembly(typeof(System.Data.DataRow));
//					verString = getAssemblyInfo(ass, "Version=", "");
//					verString = ".ver " + verString.Replace(".", ":");
#if NET20
                    string line = @".assembly extern System.Xml
{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver 2:0:0:0
}
";
#else
					string line = @".assembly extern System.Xml
{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver 1:0:3300:0
}
";
#endif
					ael.insertBefore(new ILElement(line));
				}

				if (insertNdo)
				{
                    string fullName = typeof(NDOPersistentAttribute).Assembly.FullName;
					NDOAssemblyName assName = new NDOAssemblyName(fullName);
                    messages.WriteLine("Inserting reference to NDO assembly: " + fullName);
					string line = @".assembly extern NDO
{
.publickeytoken = (" + assName.PublicKeyTokenBytes + @")
.ver " + assName.Version.Replace(".", ":") + @"
}
";

					ael.insertBefore(new ILElement(line));
				}

				if (insertNdoInterfaces)
				{
					NDOAssemblyName assName = new NDOAssemblyName(typeof(NDOException).Assembly.FullName);
					string line = @".assembly extern NDOInterfaces
{
.publickeytoken = (" + assName.PublicKeyTokenBytes + @")
.ver " + assName.Version.Replace(".", ":") + @"
}
";

					ael.insertBefore(new ILElement(line));
				}

				foreach(string s in foreignAssemblies)
				{
					string fullName = (string) assemblyFullNames[s];
                    if (fullName == null)
                    {
                        messages.WriteLine("*** Can't find assembly with name '" + s + "' to be inserted as an assembly reference.");
                        continue;
                    }
                    else
                    {
                        if (verboseMode)
                            messages.WriteLine("Insert reference to assembly " + fullName);
                    }

					NDOAssemblyName assName = new NDOAssemblyName(fullName);
					string publicKeyToken = string.Empty;
					if (assName.PublicKeyToken != "null")
						publicKeyToken = ".publickeytoken = (" + assName.PublicKeyTokenBytes + ")\n";
					string line = @".assembly extern " + assName.Name + @"
{
" + publicKeyToken + @"
.ver " + assName.Version.Replace(".", ":") + @"
}
";

					ael.insertBefore(new ILElement(line));
					
				}

			} // !enhanced


			// Jetzt alle Klassen durchlaufen und ggf. Enhancen
			foreach ( ILClassElement classElement in  classes )
			{
				if (classElement.isPersistent(typeof (NDOPersistentAttribute)))
				{
					string mappingName = classElement.getMappingName();
					IList sortedFields = (IList) allSortedFields[mappingName];
					IList references = (IList) allReferences[mappingName];
					Patcher.ClassPatcher cls = new Patcher.ClassPatcher( classElement, mappings, allPersistentClasses, messages, sortedFields, references, this.oidTypeName );
					if (!isEnhanced)
					{
						// Klasse enhancen
						cls.enhance();
					}
				}
			}			
		}




		private void
		disassemble()
		{
			Dasm dasm = new Dasm(messages, this.verboseMode);
			dasm.DoIt(objFile, ilFileName);
			if (File.Exists(resFile))
			{
				File.Copy(resFile, resEnhFile, true);
				File.Delete(resFile);
			}
		}

		private void
		reassemble()
		{
			Asm asm = new Asm(messages, this.verboseMode);
            if (this.verboseMode)
			    messages.WriteLine("KeyFile: " + this.assemblyKeyFile);

			asm.DoIt(ilEnhFile, enhFile, this.assemblyKeyFile, debug);
			if (! File.Exists(enhFile))
					throw new Exception("Codeerzeugung: temporäre Datei " + enhFile + " konnte nicht erstellt werden.");
            string resFile = Path.ChangeExtension(enhFile, ".res");
            if (File.Exists(resFile))
                File.Delete(resFile);
//			File.Copy( enhFile, binFile, true );

			DateTime ct = File.GetCreationTime(objFile);
			DateTime at = File.GetLastAccessTime(objFile);
			DateTime wt = File.GetLastWriteTime(objFile);
			
//			File.Copy( enhFile, objFile, true );

			File.SetCreationTime(enhFile, ct);
			File.SetLastAccessTime(enhFile, at);
			File.SetLastWriteTime(enhFile, wt);

//			if ( debug )
//			{
//				if (! File.Exists(enhPdbFile))
//					throw new Exception("Codeerzeugung: temporäre Datei " + enhPdbFile + " konnte nicht erstellt werden.");
//				File.Copy( enhPdbFile, binPdbFile, true );
//			}

		}

	}
}
