!IFNDEF V
V = SQLSERVER
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
!ELSEIF ("$(V)" == "SQLCE")
DIALECT = SqlCe
!ENDIF

NUNIT_DIR = D:\Program Files (x86)\NUnit 2.6.3\bin\framework
PERS_DIR = D:\Projekte\NDO22
NDO_INST_DIR = D:\Program Files (x86)\NDO 2.1 Dev Edition
TEST_DIR = $(PERS_DIR)\UnitTests
VSDIR = D:\Program Files (x86)\Microsoft Visual Studio 12.0
DEVENV = "$(VSDIR)\Common7\IDE\devenv"


!IFDEF USEGUIDS
GUIDDEFINE = /d:USEGUIDS
OID_TYPE = Guid
!ELSE
GUIDDEFINE =
OID_TYPE = Auto
!ENDIF

PBC_DIR = $(PERS_DIR)\TestEnhancerVersion4\PureBusinessClasses


UT_DIR = $(PERS_DIR)\TestEnhancerVersion4\NdoUnitTests



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
	@$(TEST_DIR)\ManipulateOidColumns\bin\Debug\ManipulateOidColumns.exe $(PBC_DIR)\NDOMapping.xml $(OID_TYPE)
	@$(TEST_DIR)\ManipulateConnection\bin\Debug\ManipulateConnection.exe $(PBC_DIR)\NDOMapping.xml $(DIALECT) -t
	@$(TEST_DIR)\ManipulateConnection\bin\Debug\ManipulateConnection.exe $(UT_DIR)\NDOMapping.xml $(DIALECT)
	@echo --- SetDefineConstants
	@$(TEST_DIR)\SetDefineConstants\bin\Debug\SetDefineConstants.exe $(PBC_DIR)\PureBusinessClasses.csproj $(USEGUIDS)
	@echo --- Rebuild
	$(DEVENV) ..\TestEnhancer.sln /rebuild Debug /project PureBusinessClasses
	@echo --- SetEnhSriptLanguage
	@$(TEST_DIR)\SetEnhSriptLanguage\bin\Debug\SetEnhSriptLanguage.exe PureBusinessClasses.ndoproj $(DIALECT)
#	@"$(NDO_INST_DIR)\NDOEnhancer.exe" $(PBC_DIR)\PureBusinessClasses.ndoproj 

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
