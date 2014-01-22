using System;
namespace Sage.Core.Framework.Common
{


    //TODO: Remove this class during merge. Use Domain project's ContextChainLink instead.
    /// <summary>
    /// 
    /// </summary>
    public class ContextChainLink
    {
        /// <summary>
        /// Id of the context.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Child context.
        /// </summary>
       // public ContextChainLink Child { get; set; }
    }
}
