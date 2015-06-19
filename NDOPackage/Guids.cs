// Guids.cs
// MUST match guids.h
using System;

namespace NETDataObjects.NDOVSPackage
{
    static class GuidList
    {
        public const string guidNDOPackagePkgString = "9a3bfde0-3b01-4a99-9ddc-1544345635fc";
        public const string guidNDOPackageCmdSetString = "7bf3372d-80b5-47ce-8104-704a904fcb3e";

        public static readonly Guid guidNDOPackageCmdSet = new Guid(guidNDOPackageCmdSetString);
    };
}