using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheOrangeRun.UI
{
    /// <summary>A set of utility methods for manipulating the UI.</summary>
    public static class PanelExtensions
    {
        /// <summary>Replaces the children of a <see cref="Panel"/>.</summary>
        /// <param name="panel">The <see cref="Panel"/> whose children to replace.</param>
        /// <param name="children">The <see cref="Panel"/> children to replace with.</param>
        public static void ReplaceChildren( this Panel panel, IEnumerable<Panel> children )
        {
            panel.DeleteChildren();
            foreach ( var child in children )
                panel.AddChild( child );
        }

        /// <summary>Replaces the children of a <see cref="Panel"/>.</summary>
        /// <param name="panel">The <see cref="Panel"/> whose children to replace.</param>
        /// <param name="children">The <see cref="Panel"/> children to replace with.</param>
        public static void ReplaceChildren( this Panel panel, params Panel[] children )
            => panel.ReplaceChildren( children.AsEnumerable() );

        /// <summary>Attempts to find a child <see cref="Panel"/> by their HTML ID.</summary>
        /// <param name="panel">The <see cref="Panel"/> from which to start searching.</param>
        /// <param name="id">The ID of the element to search for.</param>
        /// <returns>Returns the <see cref="Panel"/> having the provided <paramref name="id"/></returns>
        public static Panel TryFindById( this Panel panel, string id )
            => panel.TryFindById( id, StringComparison.Ordinal );

        /// <summary>Attempts to find a child <see cref="Panel"/> by their HTML ID.</summary>
        /// <param name="panel">The <see cref="Panel"/> from which to start searching.</param>
        /// <param name="id">The ID of the element to search for.</param>
        /// <param name="stringComparison">The comparison to perform when checking IDs.</param>
        /// <returns>Returns the <see cref="Panel"/> having the provided <paramref name="id"/></returns>
        public static Panel TryFindById( this Panel panel, string id, StringComparison stringComparison )
            => _GetChildren( panel ).FirstOrDefault( child => string.Equals( child.Id, id, stringComparison ) );

        private static IEnumerable<Panel> _GetChildren( Panel panel )
            => Enumerable.Repeat( panel, 1 ).Concat( panel.Children.SelectMany( _GetChildren ) );
    }
}
