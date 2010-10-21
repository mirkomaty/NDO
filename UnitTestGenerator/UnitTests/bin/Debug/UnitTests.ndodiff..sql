-- NDO accumulates all schema changes in this diff file.
-- Note: If you change the mapping schema version, a new diff file will be created.
-- You can change the mapping schema version in the NDO configuration dialog.
-- Don't use the Mapping Tool to change the schema information, because it will be
-- overwritten by the value set in the configuration. For automatic builds set
-- the schema version value in the .ndoproj file.

CREATE TABLE [AgrDir1OthpconNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1NoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1NoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1TblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBin1TblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBaseCmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OthpconTblOwNgnOtNgnGuidLeftCmpBin1OthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBin1OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBin1OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBin1OthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpDir1OthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOthpconTblOwNgnOtNgnGuidLeftAgrDirnOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrDirnOthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrDirnOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDirnOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [AgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseAgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [CmpBi11OwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBaseAgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpabsTblOwNgnOtNgnGuidLeftBaseCmpBi1nOwnpabsTblOwNgnOtNgnGuidRight](
[IDCmpBi1nOwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDCmpBi1nOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi1nOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1OwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1OwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OthpabsTblOwNgnOtNgnAutoLeftAgrBin1OthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBin1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBin1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBin1OthpabsTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [AgrDirnOwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBaseCmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDCmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpBinnOwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpconTblOwNgnOtNgnAutoLeftBaseCmpBinnOwnpconTblOwNgnOtNgnAutoRight](
[IDCmpBinnOwnpconTblOwNgnOtNgnAutoRight] Int NULL,
[IDCmpBinnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBinnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBaseAgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1OwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpabsTblOwNgnOtNgnAutoLeftBaseCmpBinnOwnpabsTblOwNgnOtNgnAutoRight](
[IDCmpBinnOwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
[IDCmpBinnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBinnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBin1OwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1NoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1NoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBin1NoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBaseAgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpBin1OwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnTblOwNgnOtNgnGuidLeftCmpBinnTblOwNgnOtNgnGuidRight](
[IDCmpBinnTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBinnTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [CmpDirnOwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBaseCmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [AgrBin1OwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBaseAgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBaseCmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpabsTblOwNgnOtNgnGuidLeftBaseAgrBinnOwnpabsTblOwNgnOtNgnGuidRight](
[IDAgrBinnOwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBinnOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBinnOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOthpabsTblOwNgnOtNgnGuidLeftCmpDirnOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpDirnOthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpDirnOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDirnOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [AgrDir1OwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi1nTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1OwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBaseAgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOthpabsTblOwNgnOtNgnGuidLeftCmpBinnOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBinnOthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBinnOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBinnOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpBi1nNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi1nNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBaseCmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrBi1nOthpabsNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOthpabsTblOwNgnOtNgnAutoLeftCmpBinnOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBinnOthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBinnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBinnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrBi11OthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseCmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrBi11OthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11OthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBaseAgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [AgrDir1OthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBaseCmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1OwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi1nTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OthpabsTblOwNgnOtNgnGuidLeftAgrBin1OthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBin1OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBin1OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBin1OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [CmpBi11OthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OthpconTblOwNgnOtNgnGuidLeftAgrBin1OthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBin1OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBin1OthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBin1OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOthpabsTblOwNgnOtNgnAutoLeftAgrBinnOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBinnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBinnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBinnOthpabsTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDirnTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpconTblOwNgnOtNgnGuidLeftBaseAgrBi1nOwnpconTblOwNgnOtNgnGuidRight](
[IDAgrBi1nOwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBi1nOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi1nOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [AgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseAgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrDirnNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1TblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1TblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1TblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11OthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpconTblOwNgnOtNgnAutoLeftBaseCmpBi1nOwnpconTblOwNgnOtNgnAutoRight](
[IDCmpBi1nOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBi1nOwnpconTblOwNgnOtNgnAutoRight] Int NULL,
[TCCmpBi1nOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBi1nOwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpconTblOwNgnOtNgnAutoLeftBaseAgrBi1nOwnpconTblOwNgnOtNgnAutoRight](
[IDAgrBi1nOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBi1nOwnpconTblOwNgnOtNgnAutoRight] Int NULL,
[TCAgrBi1nOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOthpconTblOwNgnOtNgnGuidLeftAgrBinnOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBinnOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBinnOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBinnOthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [AgrDir1OthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OthpconTblOwNgnOtNgnAutoLeftAgrBin1OthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBin1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBin1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBin1OthpconTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [AgrDir1OthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBaseCmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrBi11OthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1NoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1NoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1TblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1TblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBin1TblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1NoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1NoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBin1NoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseCmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDCmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [AgrBin1OwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1TblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1TblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1NoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1NoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpabsNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1TblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1TblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [AgrBi11OwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpabsTblOwNgnOtNgnAutoLeftBaseAgrBi1nOwnpabsTblOwNgnOtNgnAutoRight](
[IDAgrBi1nOwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
[IDAgrBi1nOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi1nOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrDirnTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1NoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1NoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1NoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11OthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseCmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOthpabsTblOwNgnOtNgnGuidLeftAgrBinnOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBinnOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBinnOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBinnOthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [CmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseCmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrDir1OthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OthpconTblOwNgnOtNgnAutoLeftCmpBin1OthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBin1OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBin1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBin1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrDirnTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDirnTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1TblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1TblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1NoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1NoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBin1NoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi1nTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11NoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11NoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11NoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBi11OwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBi1nOwnpabsTblOwNgnOtNgnGuidLeftBaseAgrBi1nOwnpabsTblOwNgnOtNgnGuidRight](
[IDAgrBi1nOwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBi1nOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi1nOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [AgrBi1nOwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpconTblOwNgnOtNgnGuidLeftBaseCmpBi1nOwnpconTblOwNgnOtNgnGuidRight](
[IDCmpBi1nOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[IDCmpBi1nOwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[TCCmpBi1nOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpBin1OwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1NoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1NoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1NoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseAgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11TblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11TblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11TblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOthpconTblOwNgnOtNgnGuidLeftCmpBinnOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBinnOthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBinnOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBinnOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OthpabsTblOwNgnOtNgnGuidLeftCmpBin1OthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBin1OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBin1OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBin1OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpBin1OwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpabsTblOwNgnOtNgnGuidLeftBaseCmpBinnOwnpabsTblOwNgnOtNgnGuidRight](
[IDCmpBinnOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBinnOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBinnOwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOthpconTblOwNgnOtNgnAutoLeftCmpDirnOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpDirnOthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpDirnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDirnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1TblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1TblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBin1TblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OthpabsTblOwNgnOtNgnAutoLeftCmpBin1OthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBin1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBin1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBin1OthpabsTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOthpabsTblOwNgnOtNgnAutoLeftCmpDirnOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpDirnOthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpDirnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDirnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseCmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDCmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOthpconTblOwNgnOtNgnAutoLeftAgrBinnOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBinnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBinnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBinnOthpconTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi1nNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseAgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrDir1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1NoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1NoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBin1NoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOwnpconTblOwNgnOtNgnGuidLeftBaseCmpBinnOwnpconTblOwNgnOtNgnGuidRight](
[IDCmpBinnOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBinnOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBinnOwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [AgrBi11OthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11TblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11TblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11TblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseAgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBaseAgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpDirnNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11NoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11NoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11NoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1OwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1TblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1TblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOthpconTblOwNgnOtNgnGuidLeftCmpDirnOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpDirnOthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpDirnOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDirnOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11TblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11TblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11NoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11NoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11NoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOthpabsTblOwNgnOtNgnAutoLeftAgrDirnOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrDirnOthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrDirnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDirnOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [CmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBi1nOwnpabsTblOwNgnOtNgnAutoLeftBaseCmpBi1nOwnpabsTblOwNgnOtNgnAutoRight](
[IDCmpBi1nOwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
[IDCmpBi1nOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi1nOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrDirnOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnTblOwNgnOtNgnGuidLeftAgrBinnTblOwNgnOtNgnGuidRight](
[IDAgrBinnTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBinnTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [AgrBi11OthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11TblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11TblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1TblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1TblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpabsNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpabsNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnTblOwNgnOtNgnAutoLeftCmpBinnTblOwNgnOtNgnAutoRight](
[IDCmpBinnTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBinnTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [CmpBi1nOwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpconTblOwNgnOtNgnAutoLeftBaseAgrBinnOwnpconTblOwNgnOtNgnAutoRight](
[IDAgrBinnOwnpconTblOwNgnOtNgnAutoRight] Int NULL,
[IDAgrBinnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBinnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [AgrBi1nOthpabsNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1TblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1TblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1TblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1TblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpconTblOwNgnOtNgnGuidLeftBaseAgrBinnOwnpconTblOwNgnOtNgnGuidRight](
[IDAgrBinnOwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBinnOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBinnOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1NoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1NoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnOwnpabsTblOwNgnOtNgnAutoLeftBaseAgrBinnOwnpabsTblOwNgnOtNgnAutoRight](
[IDAgrBinnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBinnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBinnOwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11NoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11NoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11NoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBaseAgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDirnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrBi11TblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11TblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11TblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDirnNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1OwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBaseCmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpBinnOwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11TblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11TblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11TblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpBinnOthpconTblOwNgnOtNgnAutoLeftCmpBinnOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBinnOthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBinnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBinnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [CmpBi11NoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11NoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11NoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrDirnTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1TblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1TblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1TblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relCmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseCmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpconNoTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconNoTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11NoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11NoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11NoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nNoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi1nNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpDir1OwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpabsNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11OthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1NoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1NoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1NoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1NoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1NoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOthpconTblOwNgnOtNgnAutoLeftAgrDirnOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrDirnOthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrDirnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDirnOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [AgrBi11OwnpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OwnpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1NoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1NoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseAgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDirnOwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [CmpBi1nOthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrBinnTblOwNgnOtNgnAutoLeftAgrBinnTblOwNgnOtNgnAutoRight](
[IDAgrBinnTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrBinnTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDirnTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpDirnTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1NoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1NoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpDir1NoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1TblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBin1TblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpabsNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11OwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11TblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11TblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDirnNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDirnNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconNoTblOwNgnOtNgnGuidLeftBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1OwnpconNoTblOwNgnOtNgnGuidLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpabsNoTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpabsTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpabsTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpabsNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpabsNoTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1OwnpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpabsNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpabsNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOwnpabsTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOwnpabsTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1TblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1TblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11NoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11NoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrBi11NoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpconNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OthpconNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpconTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpconTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_AgrBi1nTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi1nNoTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi1nNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1NoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1NoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [relAgrDirnOthpabsTblOwNgnOtNgnGuidLeftAgrDirnOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrDirnOthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrDirnOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDirnOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [CmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOthpconTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOthpconTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDirnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpconNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_AgrBi11OthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1OthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OthpconTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OthpconTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OthpabsNoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OthpabsNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OthpabsNoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsNoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsNoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOwnpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOwnpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11OwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1TblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1TblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpabsNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11TblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_CmpBi11TblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBinnOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBinnOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OthpabsNoTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi11OthpabsNoTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OthpconNoTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OthpconNoTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDirnOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDirnOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi1nOthpabsTblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi1nOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_AgrBi1nOthpabsTblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi1nOthpconNoTblOwNgnOtNgnAutoRightBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBi1nOthpconNoTblOwNgnOtNgnAutoRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoRightBase] Int NULL,
CONSTRAINT [PK_AgrDir1OwnpconOthpconNoTblOwNgnOtNgnAutoLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_CmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBin1OwnpabsTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDAgrBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
CONSTRAINT [PK_AgrBin1OwnpabsTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpconTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpconTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBin1OwnpconNoTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_CmpBin1OwnpconNoTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconTblOwNgnOtNgnGuidRight](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconTblOwNgnOtNgnGuidRight] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11OthpconNoTblOwNgnOtNgnAutoRightDerived](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11OthpconNoTblOwNgnOtNgnAutoLeft] Int NULL,
CONSTRAINT [PK_CmpBi11OthpconNoTblOwNgnOtNgnAutoRightDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1NoTblOwNgnOtNgnAutoRight](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrDir1NoTblOwNgnOtNgnAutoRight] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpBi11NoTblOwNgnOtNgnAutoLeft](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
[IDCmpBi11NoTblOwNgnOtNgnAutoRight] Int NULL,
CONSTRAINT [PK_CmpBi11NoTblOwNgnOtNgnAutoLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrDir1TblOwNgnOtNgnGuidLeft](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrDir1TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
CONSTRAINT [PK_AgrDir1TblOwNgnOtNgnGuidLeft] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase](
[ID] Int  IDENTITY (1, 1)  NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconOthpconTblOwNgnOtNgnAutoLeftBase] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBinnOwnpconTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
CONSTRAINT [PK_AgrBinnOwnpconTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDCmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidRightBase] Int NULL,
CONSTRAINT [PK_CmpDir1OwnpabsOthpabsNoTblOwNgnOtNgnGuidLeftDerived] PRIMARY KEY ([ID])
);

CREATE TABLE [AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived](
[ID] UniqueIdentifier NOT NULL,
[Dummy] Int NULL,
[IDAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidLeftBase] Int NULL,
CONSTRAINT [PK_AgrBi11OwnpconOthpconNoTblOwNgnOtNgnGuidRightDerived] PRIMARY KEY ([ID])
);

