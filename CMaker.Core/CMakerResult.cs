namespace CMaker.Core
{
    /// <summary>
    /// CMaker execution results
    /// </summary>
    public enum CMakerResult
    {
        /// <summary>
        /// Project was generated without error
        /// </summary>
        Success,
        
        /// <summary>
        /// Generation was abort, a directory with the same
        /// name as the specified project already exists.
        /// </summary>
        ProjectAlreadyExists
    }
}