-- NDO accumulates all schema changes in this diff file.
-- Note: If you change the mapping schema version, a new diff file will be created.
-- You can change the mapping schema version in the NDO configuration dialog.
-- Don't use the Mapping Tool to change the schema information, because it will be
-- overwritten by the value set in the configuration. For automatic builds set
-- the schema version value in the .ndoproj file.

CREATE TABLE [relAgrDir1OthpconTblOwNgnOtNgnGuidLeftAgrDir1OthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrDir1OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrDir1OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relCmpBin1TblOwNgnOtNgnGuidLeftCmpBin1TblOwNgnOtNgnGuidRight](
[IDCmpBin1TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDCmpBin1TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi11OwnpabsTblOwNgnOtNgnGuidLeftBaseAgrBi11OwnpabsTblOwNgnOtNgnGuidRight](
[IDAgrBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBi11OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBaseAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrBin1OwnpconTblOwNgnOtNgnGuidLeftBaseAgrBin1OwnpconTblOwNgnOtNgnGuidRight](
[IDAgrBin1OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBin1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDirnOwnpconTblOwNgnOtNgnGuidLeftBaseCmpDirnOwnpconTblOwNgnOtNgnGuidRight](
[IDCmpDirnOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDirnOwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrDirnOwnpconTblOwNgnOtNgnAutoLeftBaseAgrDirnOwnpconTblOwNgnOtNgnAutoRight](
[IDAgrDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDirnOwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi11OwnpabsTblOwNgnOtNgnGuidLeftBaseCmpBi11OwnpabsTblOwNgnOtNgnGuidRight](
[IDCmpBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBi11OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDirnOwnpabsTblOwNgnOtNgnAutoLeftBaseCmpDirnOwnpabsTblOwNgnOtNgnAutoRight](
[IDCmpDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDirnOwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBaseAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relCmpBin1OwnpabsTblOwNgnOtNgnGuidLeftBaseCmpBin1OwnpabsTblOwNgnOtNgnGuidRight](
[IDCmpBin1OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDCmpBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [relCmpDir1OwnpabsTblOwNgnOtNgnGuidLeftBaseCmpDir1OwnpabsTblOwNgnOtNgnGuidRight](
[IDCmpDir1OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDir1OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBaseAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [relCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relCmpDir1OwnpconTblOwNgnOtNgnGuidLeftBaseCmpDir1OwnpconTblOwNgnOtNgnGuidRight](
[IDCmpDir1OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDir1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi1nOthpabsTblOwNgnOtNgnGuidLeftAgrBi1nOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBi1nOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBi1nOthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBin1OwnpabsTblOwNgnOtNgnGuidLeftBaseAgrBin1OwnpabsTblOwNgnOtNgnGuidRight](
[IDAgrBin1OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBin1OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [relCmpDir1OthpabsTblOwNgnOtNgnAutoLeftCmpDir1OthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpDir1OthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBaseCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [relCmpBin1OwnpconTblOwNgnOtNgnGuidLeftBaseCmpBin1OwnpconTblOwNgnOtNgnGuidRight](
[IDCmpBin1OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBin1OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBin1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDir1OwnpconTblOwNgnOtNgnAutoLeftBaseCmpDir1OwnpconTblOwNgnOtNgnAutoRight](
[IDCmpDir1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDir1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDir1OwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi11OthpabsTblOwNgnOtNgnAutoLeftCmpBi11OthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBi11OthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpBi11OwnpconTblOwNgnOtNgnGuidLeftBaseCmpBi11OwnpconTblOwNgnOtNgnGuidRight](
[IDCmpBi11OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL
);

CREATE TABLE [relAgrBi1nTblOwNgnOtNgnGuidLeftAgrBi1nTblOwNgnOtNgnGuidRight](
[IDAgrBi1nTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrBi1nTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrDir1OwnpconTblOwNgnOtNgnGuidLeftBaseAgrDir1OwnpconTblOwNgnOtNgnGuidRight](
[IDAgrDir1OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDir1OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBaseAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relAgrBi11OthpabsTblOwNgnOtNgnGuidLeftAgrBi11OthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBi11OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBi11OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relCmpBi1nTblOwNgnOtNgnGuidLeftCmpBi1nTblOwNgnOtNgnGuidRight](
[IDCmpBi1nTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBi1nTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBaseCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [relCmpBi11OthpconTblOwNgnOtNgnGuidLeftCmpBi11OthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBi11OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi11OthpconTblOwNgnOtNgnAutoLeftAgrBi11OthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relAgrBi11OthpabsTblOwNgnOtNgnAutoLeftAgrBi11OthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDAgrBi11OthpabsTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relAgrDir1OthpconTblOwNgnOtNgnAutoLeftAgrDir1OthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrDir1OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relAgrBin1TblOwNgnOtNgnAutoLeftAgrBin1TblOwNgnOtNgnAutoRight](
[IDAgrBin1TblOwNgnOtNgnAutoRight] Int NULL,
[IDAgrBin1TblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relAgrBi1nOthpabsTblOwNgnOtNgnAutoLeftAgrBi1nOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBi1nOthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relAgrDirnOwnpabsTblOwNgnOtNgnGuidLeftBaseAgrDirnOwnpabsTblOwNgnOtNgnGuidRight](
[IDAgrDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDirnOwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi11OwnpabsTblOwNgnOtNgnAutoLeftBaseAgrBi11OwnpabsTblOwNgnOtNgnAutoRight](
[IDAgrBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBi11OwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDirnTblOwNgnOtNgnAutoLeftAgrDirnTblOwNgnOtNgnAutoRight](
[IDAgrDirnTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrDirnTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDir1OthpabsTblOwNgnOtNgnGuidLeftAgrDir1OthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrDir1OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrDir1OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrBi11OwnpconTblOwNgnOtNgnGuidLeftBaseAgrBi11OwnpconTblOwNgnOtNgnGuidRight](
[IDAgrBi11OwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relAgrDirnTblOwNgnOtNgnGuidLeftAgrDirnTblOwNgnOtNgnGuidRight](
[IDAgrDirnTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrDirnTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpBin1OwnpconTblOwNgnOtNgnAutoLeftBaseCmpBin1OwnpconTblOwNgnOtNgnAutoRight](
[IDCmpBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBin1OwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relCmpBi1nOthpabsTblOwNgnOtNgnGuidLeftCmpBi1nOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBi1nOthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBi1nOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relCmpDirnOwnpabsTblOwNgnOtNgnGuidLeftBaseCmpDirnOwnpabsTblOwNgnOtNgnGuidRight](
[IDCmpDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDirnOwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDirnOwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpBi1nTblOwNgnOtNgnAutoLeftCmpBi1nTblOwNgnOtNgnAutoRight](
[IDCmpBi1nTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBi1nTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBaseCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpBi11OwnpabsTblOwNgnOtNgnAutoLeftBaseCmpBi11OwnpabsTblOwNgnOtNgnAutoRight](
[IDCmpBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBi11OwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDirnOwnpconTblOwNgnOtNgnGuidLeftBaseAgrDirnOwnpconTblOwNgnOtNgnGuidRight](
[IDAgrDirnOwnpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDirnOwnpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDirnOwnpconTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi11OwnpconTblOwNgnOtNgnAutoLeftBaseAgrBi11OwnpconTblOwNgnOtNgnAutoRight](
[IDAgrBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBi11OwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBin1TblOwNgnOtNgnAutoLeftCmpBin1TblOwNgnOtNgnAutoRight](
[IDCmpBin1TblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBin1TblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi11TblOwNgnOtNgnAutoLeftCmpBi11TblOwNgnOtNgnAutoRight](
[IDCmpBi11TblOwNgnOtNgnAutoRight] Int NULL,
[IDCmpBi11TblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relCmpDir1OwnpabsTblOwNgnOtNgnAutoLeftBaseCmpDir1OwnpabsTblOwNgnOtNgnAutoRight](
[IDCmpDir1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDir1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDir1OwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi1nOthpabsTblOwNgnOtNgnAutoLeftCmpBi1nOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpBi1nOthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relAgrBin1OwnpconTblOwNgnOtNgnAutoLeftBaseAgrBin1OwnpconTblOwNgnOtNgnAutoRight](
[IDAgrBin1OwnpconTblOwNgnOtNgnAutoRight] Int NULL,
[IDAgrBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL
);

CREATE TABLE [relAgrBi1nTblOwNgnOtNgnAutoLeftAgrBi1nTblOwNgnOtNgnAutoRight](
[IDAgrBi1nTblOwNgnOtNgnAutoRight] Int NULL,
[IDAgrBi1nTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relAgrBi1nOthpconTblOwNgnOtNgnAutoLeftAgrBi1nOthpconTblOwNgnOtNgnAutoRightBase](
[IDAgrBi1nOthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpDirnTblOwNgnOtNgnGuidLeftCmpDirnTblOwNgnOtNgnGuidRight](
[IDCmpDirnTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpDirnTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpBi11OwnpconTblOwNgnOtNgnAutoLeftBaseCmpBi11OwnpconTblOwNgnOtNgnAutoRight](
[IDCmpBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBi11OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBi11OwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi11OthpabsTblOwNgnOtNgnGuidLeftCmpBi11OthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBi11OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBi11OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBaseAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrBi11OwnpabsOthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpBi11OthpconTblOwNgnOtNgnAutoLeftCmpBi11OthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi11OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBi11OthpconTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relAgrBin1OwnpabsTblOwNgnOtNgnAutoLeftBaseAgrBin1OwnpabsTblOwNgnOtNgnAutoRight](
[IDAgrBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrBin1OwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpDir1TblOwNgnOtNgnAutoLeftCmpDir1TblOwNgnOtNgnAutoRight](
[IDCmpDir1TblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpDir1TblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpDir1TblOwNgnOtNgnGuidLeftCmpDir1TblOwNgnOtNgnGuidRight](
[IDCmpDir1TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpDir1TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi11TblOwNgnOtNgnGuidLeftAgrBi11TblOwNgnOtNgnGuidRight](
[IDAgrBi11TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDAgrBi11TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relCmpBin1OwnpabsTblOwNgnOtNgnAutoLeftBaseCmpBin1OwnpabsTblOwNgnOtNgnAutoRight](
[IDCmpBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpBin1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpBin1OwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrBi11OthpconTblOwNgnOtNgnGuidLeftAgrBi11OthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBi11OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrBi11OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi11OthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relCmpBi1nOthpconTblOwNgnOtNgnAutoLeftCmpBi1nOthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpBi1nOthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[IDCmpBi1nOthpconTblOwNgnOtNgnAutoLeft] Int NULL
);

CREATE TABLE [relCmpBi11TblOwNgnOtNgnGuidLeftCmpBi11TblOwNgnOtNgnGuidRight](
[IDCmpBi11TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL,
[IDCmpBi11TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relAgrBi1nOthpconTblOwNgnOtNgnGuidLeftAgrBi1nOthpconTblOwNgnOtNgnGuidRightBase](
[IDAgrBi1nOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCAgrBi1nOthpconTblOwNgnOtNgnGuidRightBase] Int NULL,
[IDAgrBi1nOthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDirnOwnpconTblOwNgnOtNgnAutoLeftBaseCmpDirnOwnpconTblOwNgnOtNgnAutoRight](
[IDCmpDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCCmpDirnOwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDCmpDirnOwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDir1OthpabsTblOwNgnOtNgnAutoLeftAgrDir1OthpabsTblOwNgnOtNgnAutoRightBase](
[IDAgrDir1OthpabsTblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCAgrDir1OthpabsTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpDir1OthpconTblOwNgnOtNgnGuidLeftCmpDir1OthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpDir1OthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpDir1OthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrBi11TblOwNgnOtNgnAutoLeftAgrBi11TblOwNgnOtNgnAutoRight](
[IDAgrBi11TblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrBi11TblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDir1OwnpabsTblOwNgnOtNgnAutoLeftBaseAgrDir1OwnpabsTblOwNgnOtNgnAutoRight](
[IDAgrDir1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDir1OwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDir1OwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBaseCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi11OwnpabsOthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrDir1TblOwNgnOtNgnAutoLeftAgrDir1TblOwNgnOtNgnAutoRight](
[IDAgrDir1TblOwNgnOtNgnAutoLeft] Int NULL,
[IDAgrDir1TblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpBi1nOthpconTblOwNgnOtNgnGuidLeftCmpBi1nOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpBi1nOthpconTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpBi1nOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpBi1nOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrBin1TblOwNgnOtNgnGuidLeftAgrBin1TblOwNgnOtNgnGuidRight](
[IDAgrBin1TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrBin1TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDir1OthpconTblOwNgnOtNgnAutoLeftCmpDir1OthpconTblOwNgnOtNgnAutoRightBase](
[IDCmpDir1OthpconTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL,
[TCCmpDir1OthpconTblOwNgnOtNgnAutoRightBase] Int NULL
);

CREATE TABLE [relCmpDir1OthpabsTblOwNgnOtNgnGuidLeftCmpDir1OthpabsTblOwNgnOtNgnGuidRightBase](
[IDCmpDir1OthpabsTblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDCmpDir1OthpabsTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OthpabsTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrDirnOwnpabsTblOwNgnOtNgnAutoLeftBaseAgrDirnOwnpabsTblOwNgnOtNgnAutoRight](
[IDAgrDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDirnOwnpabsTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDirnOwnpabsTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDir1OwnpconTblOwNgnOtNgnAutoLeftBaseAgrDir1OwnpconTblOwNgnOtNgnAutoRight](
[IDAgrDir1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[TCAgrDir1OwnpconTblOwNgnOtNgnAutoLeftBase] Int NULL,
[IDAgrDir1OwnpconTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBaseCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase](
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] UniqueIdentifier NULL,
[TCCmpDir1OwnpconOthpconTblOwNgnOtNgnGuidRightBase] Int NULL
);

CREATE TABLE [relAgrDir1OwnpabsTblOwNgnOtNgnGuidLeftBaseAgrDir1OwnpabsTblOwNgnOtNgnGuidRight](
[IDAgrDir1OwnpabsTblOwNgnOtNgnGuidLeftBase] UniqueIdentifier NULL,
[TCAgrDir1OwnpabsTblOwNgnOtNgnGuidLeftBase] Int NULL,
[IDAgrDir1OwnpabsTblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

CREATE TABLE [relCmpDirnTblOwNgnOtNgnAutoLeftCmpDirnTblOwNgnOtNgnAutoRight](
[IDCmpDirnTblOwNgnOtNgnAutoLeft] Int NULL,
[IDCmpDirnTblOwNgnOtNgnAutoRight] Int NULL
);

CREATE TABLE [relAgrDir1TblOwNgnOtNgnGuidLeftAgrDir1TblOwNgnOtNgnGuidRight](
[IDAgrDir1TblOwNgnOtNgnGuidLeft] UniqueIdentifier NULL,
[IDAgrDir1TblOwNgnOtNgnGuidRight] UniqueIdentifier NULL
);

