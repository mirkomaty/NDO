!IFNDEF V
V = MYSQL
!ENDIF

!IF ("$(V)" == "ORACLE")
TABLE_PREFIX = SCOTT
!ELSE
TABLE_PREFIX =
!ENDIF

!IF ("$(V)" == "ORACLE")
DIALECT = Oracle
!ELSEIF ("$(V)" == "SQLSERVER")
DIALECT = SqlServer
!ELSEIF ("$(V)" == "ACCESS")
DIALECT = Access
!ELSEIF ("$(V)" == "MYSQL")
DIALECT = MySql
!ELSEIF ("$(V)" == "FIREBIRD")
DIALECT = Firebird
!ELSEIF ("$(V)" == "POSTGRE")
DIALECT = Postgre
!ELSEIF ("$(V)" == "SQLITE")
DIALECT = Sqlite
!ENDIF

NUNIT_DIR = C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\framework
PERS_DIR = D:\Projekte\NDO
NDO_INST_DIR = D:\Program Files\NDO 2.1 Enterprise Edition
TEST_DIR = $(PERS_DIR)\UnitTests
VSDIR = D:\Programme Win7\Microsoft Visual Studio 2010
DEVENV = "$(VSDIR)\Common7\IDE\devenv"


!IFDEF USEGUIDS
GUIDDEFINE = /d:USEGUIDS
OID_TYPE = Guid
!ELSE
GUIDDEFINE =
OID_TYPE = Auto
!ENDIF

PBC_DIR = $(PERS_DIR)\TestEnhancerVersion4\PureBusinessClasses
PBC_REFS = /r:System.dll /r:System.Data.dll /r:System.XML.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:C:\WINDOWS\assembly\GAC\NDO\1.1.0.0__37bfdffa92a0cd24\NDO.DLL 
PBC_CSFILES = Adresse.cs AssemblyInfo.cs Beleg.cs Büro.cs ClassWithBlock.cs Contact.cs DataTypeTestClasses.cs DateTestClasses.cs DerivedStateTestClasses.cs Email.cs ForeignObjectManipulationClasses.cs Institution.cs Intermediate.cs IntermediateSingle.cs Kostenpunkt.cs Land.cs Manager.cs Mitarbeiter.cs OidTestClasses.cs Peer.cs Person.cs PKWFahrt.cs Projekt.cs Reise.cs Reisebüro.cs Signatur.cs Sozialversicherungsnummer.cs Subject.cs TimeStampContainer.cs Topic.cs TopicIndirect.cs Track.cs Zertifikat.cs 
PBC_CSC_CMD = /t:library $(PBC_REFS) $(PBC_CSFILES)


UT_DIR = $(PERS_DIR)\TestEnhancerVersion4\NdoUnitTests
UT_REFS = /r:System.dll /r:System.Data.dll /r:System.XML.dll /r:..\PureBusinessClasses\bin\release\PureBusinessClasses.dll /r:C:\WINDOWS\assembly\GAC\NDO\1.1.0.0__37bfdffa92a0cd24\NDO.DLL /r:"$(NUNIT_DIR)\nunit.framework.dll" /r:System.Drawing.dll /r:System.Data.OracleClient.dll 
UT_CSFILES = ConnectionGenerator.cs ArrayListTests.cs AssemblyInfo.cs DataTypeTest.cs DateTest.cs ForeignObjectTests.cs IdGenerator.cs NDOMitarbeiterReiseTests.cs NDOMitarbeiterTests.cs NDOObjectIdTests.cs NDOPerformanceTests.cs NDOReiseLänderTests.cs NDOReiseTests.cs Objektbäume.cs PersistenceManagerFactory.cs PmFactoryTests.cs PolymorphicWTable.cs Rel1to1Bidirectional.cs Rel1to1BidirectionalWTable.cs Rel1to1Directed.cs Rel1ToNBidirectional.cs Rel1toNDirectedWTable.cs Rel1toNDirectedWTableComposition.cs RelationLockTest.cs SelfReference.cs TestDerivedState.cs TestEmbedded.cs TestMappingSchema.cs TestWithBlock.cs TimeStampTest.cs 
UT_CSC_CMD = $(UT_REFS) $(UT_CSFILES)



all: hallo pbc unittests dbbatch fertig

hallo:
	@echo -------- Configure $(V) $(USEGUIDS) --------
!IF ("$(DIALECT)" == "")
	@echo ******* error: wrong database version $(V) ********
!ENDIF

pbc:
	@echo Building BusinessClasses
	@cd $(PBC_DIR)
	@copy /Y NDOMapping.xml NDOMapping.xml.bak
	@$(TEST_DIR)\ManipulateTableNames\bin\Debug\ManipulateTableNames.exe $(PBC_DIR)\NDOMapping.xml $(TABLE_PREFIX)
	@$(TEST_DIR)\ManipulateOidColumns\bin\Debug\ManipulateOidColumns.exe $(PBC_DIR)\NDOMapping.xml $(OID_TYPE)
	@$(TEST_DIR)\ManipulateConnection\bin\Debug\ManipulateConnection.exe $(PBC_DIR)\NDOMapping.xml $(DIALECT) -t
	@$(TEST_DIR)\ManipulateConnection\bin\Debug\ManipulateConnection.exe $(UT_DIR)\NDOMapping.xml $(DIALECT)
	@$(TEST_DIR)\SetDefineConstants\bin\Debug\SetDefineConstants.exe $(PBC_DIR)\PureBusinessClasses.csproj $(USEGUIDS)
	$(DEVENV) ..\TestEnhancer.sln /rebuild Debug /project PureBusinessClasses
	@$(TEST_DIR)\SetEnhSriptLanguage\bin\Debug\SetEnhSriptLanguage.exe PureBusinessClasses.ndoproj $(DIALECT)
	@"$(NDO_INST_DIR)\NDOEnhancer.exe" $(PBC_DIR)\PureBusinessClasses.ndoproj 

unittests:
	@echo Building Unit Tests
	@cd $(UT_DIR)
	$(TEST_DIR)\SetEnhSriptLanguage\bin\Debug\SetEnhSriptLanguage.exe NdoUnitTests.ndoproj $(DIALECT)
	$(TEST_DIR)\SetDefineConstants\bin\Debug\SetDefineConstants.exe NdoUnitTests.csproj $(V)
	@del $(UT_DIR)\NDOMapping.xml


dbbatch:
	@echo Sql Batch
	@cd $(TEST_DIR)
	@ExecuteSqlBatch\bin\debug\ExecuteSqlBatch $(PBC_DIR)\bin\Debug\PureBusinessClasses.ndo.sql $(PBC_DIR)\NDOMapping.Xml

fertig:
	@echo ------ Fertig -------
