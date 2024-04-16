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
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

using NDO;
using NDO.Mapping;

using NDOEnhancer.ILCode;
using NDOEnhancer.Patcher;

namespace NDOEnhancer
{
	/// <summary>
	/// Der ganze Enhancement-Prozess beginnt bei DoIt()
	/// </summary>
	internal class Enhancer
	{
		public Enhancer( ProjectDescription projectDescription, MessageAdapter messages )
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
			mappingDestFile = Path.Combine(Path.GetDirectoryName( binFile ), projectDescription.ConfigurationOptions.TargetMappingFileName);
			mappingFile = projectDescription.DefaultMappingFileName;
			options = projectDescription.ConfigurationOptions;

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

		private ClassDictionary<ClassNode> allPersistentClasses = new ClassDictionary<ClassNode>();
		private ClassDictionary<List<KeyValuePair<string,ILField>>> allSortedFields = new ClassDictionary<List<KeyValuePair<string, ILField>>>();
		private ClassDictionary<List<ILReference>>      allReferences = new ClassDictionary<List<ILReference>>();
		private Dictionary<string,string> assemblyFullNames = new Dictionary<string,string>();
		private List<string> tabuClasses = new List<string>();
		private NDOMapping          mappings;
		private MessageAdapter		messages;
		private NDODataSet			dsSchema;
		private ConfigurationOptions options;
		private string				assemblyKeyFile = null;

        void CheckNDO(Assembly ass)
        {
#warning we should implement a check for the wrong NDO dll referencde here
			//         AssemblyName[] references = ass.GetReferencedAssemblies();
			//         NDOAssemblyName refAn = null;
			//         foreach (AssemblyName an in references)
			//         {
			//             if (an.Name == "NDO")
			//                 refAn = new NDOAssemblyName(an.FullName);
			//}
			//         if (refAn == null)
			//             return;
			//         NDOAssemblyName ndoAn = new NDOAssemblyName(typeof(NDOPersistentAttribute).Assembly.FullName);  // give us the NDO version the enhancer belongs to
			//Version refVersion = refAn.AssemblyVersion;
			//bool isRightVersion = refVersion.Major > 2 || refVersion.Major == 2 && refVersion.Minor >= 1;
			//         if (refAn.PublicKeyToken != ndoAn.PublicKeyToken || !isRightVersion)
			//         {
			//             throw new Exception("Assembly " + ass.FullName + " references a wrong NDO.dll. Expected: " + ndoAn.FullName + ". Found: " + refAn.FullName + ".");
			//         }   
		}

		string CreateShadowCopy(string dllPath)
		{
			var dir = Path.Combine( Path.GetDirectoryName( dllPath ), "org" );
			if (!Directory.Exists( dir ))
				Directory.CreateDirectory( dir );

			var newPath = Path.Combine( dir, Path.GetFileName( dllPath ) );
			File.Copy( dllPath, newPath, true );

			if (this.debug)
			{
				var source = Path.ChangeExtension(dllPath, ".pdb");
				var target = Path.Combine( dir, Path.ChangeExtension( Path.GetFileName( dllPath ), ".pdb" ) );
				File.Copy( source, target, true );
			}

			return newPath;
		}

		private void SearchPersistentBases()
		{
			Dictionary<string, NDOReference> references = projectDescription.References;
			List<ClassNode> ownClassList = null;
			string binaryAssemblyFullName = null;

            // Check, if we can load the project bin file.
            try
            {
                binaryAssemblyFullName = NDOAssemblyChecker.GetAssemblyName(this.binFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't load referenced Assembly '" + this.binFile + ". " + ex.Message);
            }

			// Durchsuche alle referenzierten Assemblies
			foreach (NDOReference reference in references.Values)
			{
				if ( !reference.CheckThisDLL )
					continue;

				string dllPath = reference.Path;
				bool ownAssembly = (string.Compare( dllPath, this.binFile, true ) == 0);
				if (ownAssembly)
				{
					// We need to copy the assembly to another path, because we
					// want to overwrite the assembly after enhancement.
					dllPath = CreateShadowCopy( dllPath );
				}

				if (!ownAssembly && !NDOAssemblyChecker.IsEnhanced( dllPath ))
					continue;

				AssemblyName assyToLoad = null;
				Assembly assy = null;
				string assyName;
				try
				{
					if (verboseMode)
						messages.WriteLine($"Loading assembly {dllPath}");
                    assyToLoad = AssemblyName.GetAssemblyName(dllPath);
					assyName = assyToLoad.Name;
					assy = Assembly.Load(assyName);
				}
				catch (Exception ex)
				{
					if (assyToLoad != null && (binaryAssemblyFullName == null || string.Compare(binaryAssemblyFullName, assyToLoad.FullName, true) != 0))
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

				if (this.assemblyFullNames.ContainsKey(assyName))
				{
					messages.WriteLine("Assembly '" + assyName + "' analyzed twice. Check your .ndoproj file.");
					continue;
				}
				this.assemblyFullNames.Add(assyName, assyToLoad.FullName);

                if (verboseMode)
                {
                    messages.WriteLine("Checking DLL: " + dllPath);
                    messages.WriteLine("BinFile: " + binFile);
                }

				AssemblyNode assemblyNode = null;
                CheckNDO(assy);

				try
				{
					assemblyNode = new AssemblyNode(assy, this.mappings);
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
					this.ownAssemblyName = assyName;
					Corlib.FxType = assemblyNode.TargetFramework.StartsWith(".NETStandard,Version=") ? FxType.Standard2 : FxType.Net;
					//.NETCoreApp,Version=v6.0
					int p = assemblyNode.TargetFramework.IndexOf( "Version=v" );
					if (p == -1)
					{
						throw new Exception( $"Target Framework doesn't contain version number: '{assemblyNode.TargetFramework}'" );
					}
					else
					{
						if (Version.TryParse( assemblyNode.TargetFramework.Substring( p + 9 ), out var v ))
						{
							var minor = Math.Max( 0, v.Minor );
							var rev = Math.Max( 0, v.Revision );
							var build = Math.Max( 0, v.Build );
							Corlib.FxVersion = $"{v.Major}:{minor}:{build}:{rev}";
						}
						else
						{
							throw new Exception( $"Version number invalid in '{assemblyNode.TargetFramework}'" );
						}
					}
					if (this.verboseMode)
					{
						messages.WriteLine( $"FxType: {ownAssemblyName}: {Corlib.FxType}" );
						messages.WriteLine( $"Version: {Corlib.FxVersion}" );
					}
				}

				var classList = assemblyNode.PersistentClasses;
				foreach(ClassNode classNode in classList)
				{
					string clName = classNode.Name;
					if (!allPersistentClasses.ContainsKey(clName))
						allPersistentClasses.Add(clName, classNode);
					else if (verboseMode)
						messages.WriteLine("Multiple definition of Class " + clName + '.');
				}

				// Wir haben externe persistente Klassen gefunden.
				// Mapping-Info einlesen.
				if (!ownAssembly)
					MergeMappingFile(dllPath, classList);
				if (!ownAssembly) 
					MergeDataSet(dllPath, classList);

				if (ownAssembly)
				{
					this.hasPersistentClasses = (classList.Count > 0);
				}

				// TODO: Check, how to handle value types
//				XmlNodeList vtnl = pcDoc.SelectNodes(@"/PersistentClasses/ValueType");
//				ValueTypes.Instance.Merge(vtnl);
			}

            if (!options.EnableEnhancer)
            {
                ownClassList = new List<ClassNode>();
            }
            else
            {
                if (ownClassList == null)
                    throw new Exception("A reference to the assembly " + binFile + " is needed. Check your parameter file.");
            }

			CheckClassMappings(ownClassList);
            CheckTypeList();
            CheckOidColumnMappings(); // Incorporates the Attributes and NDOOidType.
			CheckAllRelationMappings(ownClassList); // Makes sure, that a mapping entry for each relation exists
            DetermineOidTypes(); // needs the relation mappings, calls InitFields
            CheckRelationForeignKeyMappings(); // Calls r.InitFields, therefore requires, that InitFields for the Oid's was called
			GenerateAllSchemas();
            RemoveRedundantOidColumnNames();
		}


        private void RemoveRedundantOidColumnNames()
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

        private void CheckOidColumnMappings()
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
                if (oid.OidColumns.Count == 0 && !classNode.ColumnAttributes.Any())
                {
                    OidColumn oidColumn = oid.NewOidColumn();
                    oidColumn.Name = "ID";
                }

                // Check, if the oid columns match the OidColumnAttributes, if any of them exist
                // In case of NDOOidType the ClassNode constructor creates an OidColumnAttribute

                oid.RemapOidColumns(classNode.ColumnAttributes);

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
        private void CheckRelationForeignKeyMappings()
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
				
		private List<string> CheckRelationTargetAssemblies()
		{
			List<string> foreignAssemblies = new List<string>();
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
		

		private void CheckTypeList()
		{
			string typeFile = Path.Combine(Path.GetDirectoryName(binFile), "NDOTypes.Xml");
			TypeManager tm = new TypeManager(typeFile, this.mappings);
			tm.CheckTypeList(allPersistentClasses);
		}

		void GenerateAllSchemas()
		{
			if (this.projectDescription.ConfigurationOptions.GenerateSQL)
				dsSchema.Remap(mappings);
		}



		public void
		MergeDataSet(string absDllPath, List<ClassNode> classList)
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

		private void DetermineOidTypes()
		{
            foreach (Class cl in mappings.Classes)
            {
                ClassNode classNode = (ClassNode)allPersistentClasses[cl.FullName];
				if (classNode == null)
				{
					mappings.RemoveClass( cl );
					continue;
				}
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


		private void CheckMappingForField(string prefix, Patcher.ILField field, List<KeyValuePair<string,ILField>> sortedFields, Class classMapping, bool isOnlyChild, FieldNode fieldNode)
		{
			bool isOidField = fieldNode.IsOid;
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
					CheckMappingForField(field.CleanName + ".", newf, sortedFields, classMapping, oneChildOnly, fieldNode);
				}
				return;
			}

			if (field.IsInherited)
			{
				foreach(var entry in sortedFields.ToList())
					if (entry.Key == fname)
						sortedFields.Remove(entry);
			}

			sortedFields.Add(new KeyValuePair<string, ILField>( fname, field ));

			if (classMapping != null)
			{
				Field f = classMapping.FindField(fname);
				if (null == f)
				{
					f = classMapping.AddStandardField(fname, isOidField);
					if (isOnlyChild)
						f.Column.Name = classMapping.ColumnNameFromFieldName(field.Parent.CleanName, false);
					messages.WriteLine("Generating field mapping: " + classMapping.FullName + "." + fname + " -> " + f.Column.Name);
					if (classMapping.IsAbstract)
						f.Column.Name = "Unused";
				}
				if (fieldNode.ColumnAttribute != null)
				{
					fieldNode.ColumnAttribute.SetColumnValues( f.Column );
				}
			}
		}

		private void IterateFieldNodeList(IEnumerable<FieldNode> fieldList, bool isEmbeddedObject, 
			List<FieldNode> oidFields, Class classMapping, List<KeyValuePair<string, ILField>> sortedFields, 
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
				IEnumerable<FieldNode> subFieldList = null;
				if (isEmbeddedObject)
				{
					subFieldList = fieldNode.Fields;
				}

				bool isEnum = (fieldNode.IsEnum);
				Patcher.ILField field = new Patcher.ILField(fieldNode.FieldType, tn, 
					fieldNode.Name, this.ownAssemblyName, subFieldList, isEnum);
				field.IsInherited = isInherited;
				Debug.Assert (field.Valid, "field.Valid is false");
				if (classMapping != null)
					CheckMappingForField("", field, sortedFields, classMapping, false, fieldNode);
			}
		}


		/// <summary>
		/// Helps sorting the fields
		/// </summary>
		private class FieldComparer : IComparer<KeyValuePair<string,ILField>>
		{
			public int Compare( KeyValuePair<string, ILField> x, KeyValuePair<string, ILField> y )
			{
				return String.CompareOrdinal( x.Key, y.Key );
			}
		}

		private void CheckFieldMappings( ClassNode classNode, Class classMapping )
		{
			var sortedFields = new List<KeyValuePair<string,ILField>>();
			string className = classNode.Name;
			allSortedFields.Add( className, sortedFields );

			List<FieldNode> oidFields = new List<FieldNode>();

			// All own fields
			var fieldList = classNode.Fields;
			IterateFieldNodeList( fieldList, false, oidFields, classMapping, sortedFields, false );

			// All embedded objects
			var embeddedObjectsList = classNode.EmbeddedTypes;
			IterateFieldNodeList( embeddedObjectsList, true, oidFields, classMapping, sortedFields, false );

			var fieldComparer = new FieldComparer();
			sortedFields.Sort( fieldComparer );

			// Alle ererbten Felder
			ClassNode derivedclassNode = this.allPersistentClasses[classNode.BaseName];

			while (null != derivedclassNode)
			{
				if (derivedclassNode.IsPersistent)
				{
					int startind = sortedFields.Count;

					var nl = derivedclassNode.Fields;
					IterateFieldNodeList( nl, false, oidFields, classMapping, sortedFields, true );

					var enl = derivedclassNode.EmbeddedTypes;
					IterateFieldNodeList( enl, true, oidFields, classMapping, sortedFields, true );

					int len = sortedFields.Count - startind;
					sortedFields.Sort( startind, len, fieldComparer );
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
					new OidColumnIterator( classMapping ).Iterate( delegate ( OidColumn oidCol, bool isLastElement )
					{
						if (oidCol.FieldName == oidField.Name)
							oidColumn = oidCol;
					} );
					if (oidColumn == null)
					{
						oidColumn = classMapping.Oid.NewOidColumn();
						oidColumn.FieldName = oidField.Name;
						oidColumn.Name = classMapping.FindField( oidField.Name ).Column.Name;
					}
				}
			}

			foreach (var de in sortedFields)
				sortedFieldsFile.WriteLine( de.Key );

			sortedFieldsFile.WriteLine();


			// Und nun die überflüssigen entfernen
			List<Field> fieldsToRemove = new List<Field>();
			foreach (Field f in classMapping.Fields)
			{
				bool isExistent = false;
				foreach (var e in sortedFields)
				{
					if (e.Key == f.Name)
					{
						isExistent = true;
						break;
					}
				}

				if (!isExistent)
					fieldsToRemove.Add( f );

			}

			foreach (Field field in fieldsToRemove)
				classMapping.RemoveField( field );

			List<Field> sortedFieldMappings = classMapping.Fields.ToList();
			sortedFieldMappings.Sort( ( f1, f2 ) => string.CompareOrdinal( f1.Name, f2.Name ) );
			for (int i = 0; i < sortedFieldMappings.Count; i++)
				sortedFieldMappings[i].Ordinal = i;
		}


		private void SetDefiningClass(Relation r, Class parent)
		{
			Type t = typeof(Relation);
			FieldInfo fi = t.GetField("definingClass", BindingFlags.NonPublic | BindingFlags.Instance);
			fi.SetValue(r, parent);
		}

		public void CheckInheritedRelationMappings(ClassNode classNode, Class classMapping)
		{
			string className = classNode.Name;

			var references = this.allReferences[className];
			// Alle ererbten Relationen
			ClassNode baseClassNode = this.allPersistentClasses[classNode.BaseName];
			
			while (null != baseClassNode)
			{
				if (baseClassNode.IsPersistent)
				{
					Class baseClassMapping = baseClassNode.Class;
					var nl = baseClassNode.Relations;
					foreach (var relNode in nl)
					{
						// Relation nur aus der Klasse nehmen, die das Feld deklariert
						if (relNode.DeclaringType != null)
							continue;
						//					string tn = relNode.Attributes["Type"].Value;
						RelationInfo ri = relNode.RelationInfo;
						string rname = relNode.Name;

						// The Relation should be generated at the lowest end of
						// the class hiearchy.
						Debug.Assert( !references.Any( r => r.CleanName == rname ) );
										

						// add the relation as inherited reference
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
								classMapping.AddRelation(r);
							}
							else
							{
								Relation orgRel = baseClassMapping.FindRelation( rname );
								if (r.AccessorName == null && r.AccessorName != orgRel.AccessorName)
									r.AccessorName = orgRel.AccessorName;
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

		public void CheckRelationMappings(ClassNode classNode, Class classMapping)
		{
			var references = new List<ILReference>();
			string className = (classNode.Name);
			allReferences.Add(className, references);
			var refList = classNode.Relations;
			foreach (var relationNode in refList)
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
					ClassNode relClassNode = allPersistentClasses[relTypeName];
                    if (null == r)
                    {
                        //TODO: ForeignKeyColumnAttributes...
                        string relTypeFullName = relTypeName.Substring(relTypeName.IndexOf("]") + 1);                        
                        if (relClassNode == null)
                            throw new Exception(String.Format("Class '{1}' has a relation to a non persistent type '{0}'.", relTypeFullName, classNode.Name));
                        messages.WriteLine("Creating standard relation " + classNode.Name + "." + fieldName);
                        r = classMapping.AddStandardRelation(fieldName, relTypeFullName, is1To1, relName, classNode.IsPoly, relClassNode.IsPoly || relClassNode.IsAbstract, relationNode.MappingTableAttribute);
                    }
                    else
                    {
						r.RemapMappingTable( classNode.IsPoly, relClassNode.IsPoly || relClassNode.IsAbstract, relationNode.MappingTableAttribute );
						r.RemapForeignKeyColumns( relationNode.ForeignKeyColumnAttributes, relationNode.ChildForeignKeyColumnAttributes );  // currently nothing happens there.
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


		private void CheckClassMappings(List<ClassNode> classList)
		{
			if (options.DatabaseOwner != string.Empty)
				mappings.StandardDbOwner = options.DatabaseOwner;
			sortedFieldsFile = new StreamWriter(Path.ChangeExtension(binFile, ".fields.txt"));
			foreach(var classNode in classList)
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
						messages.WriteLine("Generating class mapping for class '" + className + "'");

						if (classNode.IsAbstract)
							classMapping = mappings.AddAbstractClass(className, assName, classNode.ColumnAttributes);
						else
							classMapping = mappings.AddStandardClass(className, assName, classNode.ColumnAttributes);					                            
					}
					if (options.UseTimeStamps && (classMapping.TimeStampColumn == null || classMapping.TimeStampColumn == string.Empty))
						classMapping.TimeStampColumn = "NDOTimeStamp";
                    if (classNode.ClassType.IsGenericType && classMapping.TypeNameColumn == null)
                        classMapping.AddTypeNameColumn();
				}
				CheckFieldMappings(classNode, classMapping);
			}
			sortedFieldsFile.Close();

			// Lösche ungebrauchte Class Mappings
			var classesToDelete = new List<Class>();
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

		private void CheckAllRelationMappings(List<ClassNode> classList)
		{
			foreach(ClassNode classNode in classList)
			{
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				Class classMapping = classNode.Class;
				CheckRelationMappings(classNode, classMapping);
			}

			foreach(ClassNode classNode in classList)
			{
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				Class classMapping = classNode.Class;
				CheckInheritedRelationMappings(classNode, classMapping);
			}

			foreach(ClassNode classNode in classList)
			{
				if (classNode.IsInterface)
					continue;
				if (!classNode.IsPersistent) // non persistent classes derived from persistent classes
					continue;
				Class classMapping = classNode.Class;
				if (classMapping == null)
					continue;
				DeleteUnusedRelationMappings(classMapping);
				CheckDoubleComposites(classMapping);
			}
		}


		private void DeleteUnusedRelationMappings(Class classMapping)
		{
			var references = this.allReferences[classMapping.FullName];
			var relationsToDelete = new List<Relation>();
			foreach (Relation r in classMapping.Relations)
			{
				if (!references.Any( x => x.CleanName == r.FieldName ))
					relationsToDelete.Add( r );
			}
			foreach (Relation r in relationsToDelete)
			{
				messages.WriteLine(String.Format("Delete unused Relation Mapping {0}", classMapping.FullName + "." + r.FieldName));
				classMapping.RemoveRelation(r);
			}
		}

		private void CheckDoubleComposites(Class classMapping)
		{
			var references = this.allReferences[classMapping.FullName];
			foreach (Relation r in classMapping.Relations)
			{
				Patcher.ILReference reference = references.FirstOrDefault(x => x.CleanName == r.FieldName);
				if (reference != null)
				{
					Relation r2 = r.ForeignRelation;
					if (r2 != null)
					{
						var references2 = this.allReferences[r.ReferencedTypeName];
						if(references2 == null)  // Type is not from our assembly
							continue;
						Patcher.ILReference reference2 = references2.FirstOrDefault(x=>x.CleanName == r2.FieldName);
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


		public void MergeMappingFile(string absDllPath, List<ClassNode> classList)
		{
			var dir = Path.GetDirectoryName(absDllPath);
			var mapFileName = Path.Combine(dir, Path.GetFileNameWithoutExtension(absDllPath) + ".ndo.mapping");
			if (!File.Exists( mapFileName )) 
				mapFileName = Path.Combine(dir, "NDOMapping.xml");

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

		/// <summary>
		/// This is the Enhancer entry point
		/// </summary>
		/// <exception cref="Exception"></exception>
		public void DoIt()
		{

            bool sourcesUpToDate = !options.EnableEnhancer;
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
                    if (!File.Exists(projectDescription.BinFile))
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

					mappings = new NDOMapping(mappingFile);
					mappings.SchemaVersion = options.SchemaVersion;
                    ((IEnhancerSupport)mappings).IsEnhancing = true;
			}
			catch (Exception ex)
			{
				if (null != ex.Message)
					throw new Exception("Can't load Mapping File " + mappingFile + ".\n" + ex.ToString());
				else
                    throw new Exception("Can't load Mapping File " + mappingFile);
            }

			if (wrongDll != null)
				throw new Exception(wrongDll);

			dsSchema = new NDODataSet();
            dsSchema.EnforceConstraints = false;

			// The mapping und schema files
			// will be merged here
			SearchPersistentBases();
			bool doEnhance = options.EnableEnhancer && !this.isEnhanced;
#if xDEBUG
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
				Disassemble();

				ILFile ilfile = new ILFile();

				messages.WriteLine( "Enhancing Assembly" );
				messages.Indent();

				// Hier wird der Elementbaum erzeugt
				ilfile.Parse( ilFileName );

                if (projectDescription.KeyFile == string.Empty)
                    this.assemblyKeyFile = null;
                else
                    this.assemblyKeyFile = projectDescription.KeyFile;
				foreach (var el in ilfile.AssemblyElements)
				{
					if (!el.IsExtern)
					{
						var customElements = el.CustomElements;

						foreach (var custEl in customElements)
						{
							ILCustomElement.AttributeInfo ai = custEl.GetAttributeInfo();
							if (ai.TypeName == "System.Reflection.AssemblyKeyAttribute")
							{
								string s = (string) ai.ParamValues[0];
								if (s != null && s != string.Empty)
								{
									this.assemblyKeyFile = ("@" + s);
									break;
								}
							}
							if (ai.TypeName == "System.Reflection.AssemblyKeyFileAttribute")
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

				AnalyzeAndEnhance(ilfile);

				messages.Unindent();
				if (this.verboseMode)
					messages.WriteLine( "Generating Binary" );
					
				// Hier wird der enhanced Elementbaum als IL-Datei geschrieben
				ilfile.write( ilEnhFile, Corlib.FxType == FxType.Standard2 );

				// ILAsm assembliert das Ganze
				Reassemble();
			}


			// Store the mapping information
			if (this.verboseMode)
				messages.WriteLine( "Saving mapping file" );
			mappings.Save();
            if (verboseMode)
			    Console.WriteLine("Copying mapping file to '" + mappingDestFile + "'");
			File.Copy(mappingFile, mappingDestFile, true);

			if (this.verboseMode)
				messages.WriteLine( "Generating schema file" );
			if (File.Exists(schemaFile))
				File.Copy(schemaFile, schemaFile.Replace(".ndo.xsd", ".ndo.xsd.bak"), true);
			dsSchema.WriteXmlSchema(schemaFile);

			if (options.GenerateSQL)
			{
				if (this.verboseMode)
					messages.WriteLine( "Generating sql file" );
				string sqlFileName = schemaFile.Replace(".xsd", ".sql");
                TypeManager tm = null;
                if (options.IncludeTypecodes)
                {
                    string typeFile = Path.Combine(Path.GetDirectoryName(binFile), "NDOTypes.Xml");
                    tm = new TypeManager(typeFile, this.mappings);
                }
				string oldSchemaFile = schemaFile.Replace(".ndo.xsd", ".ndo.xsd.bak");
				NDODataSet dsOld = null;
				if (File.Exists(oldSchemaFile))
				{
                    dsOld = new NDODataSet(oldSchemaFile);
					new SQLDiffGenerator().Generate(options.SQLScriptLanguage, options.Utf8Encoding, dsSchema, dsOld, sqlFileName, mappings, messages);
					new NdoTransDiffGenerator().Generate(dsSchema, dsOld, sqlFileName, mappings, messages);
				}
				else
				{
					new NdoTransDiffGenerator().Generate( dsSchema, new DataSet(), sqlFileName, mappings, messages );
				}
				if (!this.options.DropExistingElements)
					dsOld = null;  // causes, that no drop statements will be generated.
				new SQLGenerator().Generate(options.SQLScriptLanguage, options.Utf8Encoding, dsSchema, dsOld, sqlFileName, mappings, messages, tm, this.options.GenerateConstraints);
			}

		}

		void AnalyzeAndEnhance( ILFile ilFile )
		{			
			var classes = ilFile.GetAllClassElements();

			if (!isEnhanced)
			{
				var foreignAssemblies = CheckRelationTargetAssemblies();

				bool insertSystemDataCommon = true;
				bool insertXml = true;
				bool insertNdo = true;
				bool insertNdoInterfaces = true;
				bool insertSystemComponentmodelPrimitives = true;

				foreach ( var assyElem in ilFile.AssemblyElements)
				{ 
					string nameLower = assyElem.Name.ToLower();
					if (foreignAssemblies.Contains(assyElem.Name))
						foreignAssemblies.Remove(assyElem.Name);
					string line = assyElem.GetLine(0);

					// If it's the own assemblyElement, we add the NDOEnhanced attribute
					if (!assyElem.IsExtern)
					{
                        ILElement lastEl = assyElem.CustomElements.Last();
						lastEl.InsertBefore(new ILCustomElement(".custom instance void [NDO]NDO.NDOEnhancedAttribute::.ctor() = ( 01 00 00 00 )", assyElem));
					}
					if (line.StartsWith(".assembly"))
					{
						if (nameLower == "system.data")
							insertSystemDataCommon = false;
						if (nameLower == "system.xml")
							insertXml = false;
						if (nameLower == "system.componentmodel.primitives")
							insertSystemComponentmodelPrimitives = false;
						if (nameLower == "ndo")
						{
                            if (this.verboseMode)
                            {
                                messages.WriteLine("NDO Dll:");
								foreach (var subEl in assyElem.Elements)
								{
									messages.WriteInsertedLine( subEl.GetAllLines() );
								}
                            }

							insertNdo = false;

							/* We don't need a version check anymore. This might be necessary again, if it comes to .NET Version 5
                            if (assElem.Major != 2 && assElem.Minor != 0)
                            {
								string version = EnhDate.String;
								Regex regex = new Regex( @"V\. (\d+\.\d+)" );
								Match match = regex.Match( version );
								if (match.Success)
									version = match.Groups[1].Value;
                                throw new Exception("This assembly is built with NDO.dll Version " + assElem.VersionString.Replace(':', '.')
                                    + ". This NDO enhancer only works with NDO.dll version " + version + ". Please correct your assembly reference and rebuild the assembly.");
                            }
							*/

                        }
					}
					else
					{
						throw new Exception("Assembly element doesn't start with .assembly.");
					}
				}

				ILAssemblyElement ael = ilFile.AssemblyElements.First();

				if (insertSystemDataCommon && Corlib.FxType == FxType.Net)
				{
					string line = $@".assembly extern System.Data.Common
{{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver {Corlib.FxVersion}
}}";
					ael.InsertBefore(new ILElement(line));
				}


				if (insertSystemComponentmodelPrimitives && Corlib.FxType == FxType.Net)
				{
					string line = $@".assembly extern System.ComponentModel.Primitives
{{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver {Corlib.FxVersion}
}}
";
					ael.InsertBefore(new ILElement(line));
				}

				if (insertXml && Corlib.FxType == FxType.Net)
				{
//					Assembly ass = Assembly.GetAssembly(typeof(System.Data.DataRow));
//					verString = getAssemblyInfo(ass, "Version=", "");
//					verString = ".ver " + verString.Replace(".", ":");
                    string line = $@".assembly extern System.Xml.ReaderWriter
{{
.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
.ver {Corlib.FxVersion}
}}
";
					ael.InsertBefore(new ILElement(line));
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

					ael.InsertBefore(new ILElement(line));
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

					ael.InsertBefore(new ILElement(line));
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

					ael.InsertBefore(new ILElement(line));
					
				}

			} // !enhanced			

			// Jetzt alle Klassen durchlaufen und ggf. Enhancen
			foreach ( ILClassElement classElement in  classes )
			{
				if (classElement.IsPersistent(typeof (NDOPersistentAttribute)))
				{
					Dictionary<string, string> accessors = classElement.GetAccessorProperties();
					Class classMapping = mappings.FindClass( classElement.MappingName );
					if (classMapping != null && accessors.Count > 0)
					{
						foreach (var item in accessors)
						{
							Field field = classMapping.FindField( item.Key );
							if (field != null)
							{
								field.AccessorName = item.Value;
							}
							else
							{
								Relation rel = classMapping.FindRelation( item.Key );
								if (rel != null)
									rel.AccessorName = item.Value;
							}
						}
					}
					string mappingName = classElement.MappingName;
					var sortedFields = allSortedFields[mappingName];
					var references = allReferences[mappingName];
					Patcher.ClassPatcher cls = new Patcher.ClassPatcher( classElement, mappings, allPersistentClasses, messages, sortedFields, references, this.oidTypeName );
					if (!isEnhanced)
					{
						// Klasse enhancen
						cls.enhance();
					}
				}
			}			
		}


		private void Disassemble()
		{
			Dasm dasm = new Dasm(messages, this.verboseMode);
			dasm.DoIt(objFile, ilFileName);
			if (File.Exists(resFile))
			{
				File.Copy(resFile, resEnhFile, true);
				File.Delete(resFile);
			}
		}

		private void Reassemble()
		{
			Asm asm = new Asm(messages, this.verboseMode);
            if (this.verboseMode)
			    messages.WriteLine("KeyFile: " + this.assemblyKeyFile);

			asm.DoIt(ilEnhFile, enhFile, this.assemblyKeyFile, debug);
			
			if (! File.Exists(enhFile))
					throw new Exception("Temporary file " + enhFile + " could not be written.");
            string resFile = Path.ChangeExtension(enhFile, ".res");
            if (File.Exists(resFile))
                File.Delete(resFile);
//			File.Copy( enhFile, binFile, true );

			DateTime ct = File.GetCreationTime(objFile);
			DateTime at = File.GetLastAccessTime(objFile);
			DateTime wt = File.GetLastWriteTime(objFile);
			
//			File.Copy( enhFile, objFile, true );

			File.SetCreationTime( enhFile, ct);
			File.SetLastAccessTime( enhFile, at);
			File.SetLastWriteTime( enhFile, wt);

			//if (debug)
			//{
			//	if (!File.Exists( enhPdbFile ))
			//		throw new Exception( "Temporary file " + enhPdbFile + " could not be written." );
			//	File.Copy( enhPdbFile, binPdbFile, true );
			//}

		}

	}
}
