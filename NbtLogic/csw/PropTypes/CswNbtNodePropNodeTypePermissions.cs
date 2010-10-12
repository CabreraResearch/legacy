namespace ChemSW.Nbt
{
    /// <summary>
    /// Type of Permission on NodeTypes
    /// </summary>
    public enum NodeTypePermission
    {
        /// <summary>
        /// Permission to view nodes of this type
        /// </summary>
        View,
        /// <summary>
        /// Permission to create new nodes of this type
        /// </summary>
        Create,
        /// <summary>
        /// Permission to delete nodes of this type
        /// </summary>
        Delete,
        /// <summary>
        /// Permission to edit property values of nodes of this type
        /// </summary>
        Edit
    }
}
