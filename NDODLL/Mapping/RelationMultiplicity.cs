using System;

namespace NDO.Mapping
{
    /// <summary>
    /// This enum is used to determine, where the foreign key of a relation resides.
    /// </summary>
    public enum RelationMultiplicity
    {
        /// <summary>
        /// 1:1 relation - foreign key resides in the own table or in a mapping table
        /// </summary>
        Element,
        /// <summary>
        /// 1:n relation - foreign key resides in the foreign table or in a mapping table
        /// </summary>
        List
    }
}
