using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace TheOrangeRun.UI
{
    /// <summary>A utility class for building HUD elements on the go.</summary>
    public class HtmlElementBuilder
    {
        public static implicit operator Panel( HtmlElementBuilder builder )
        {
            var panel = new Panel
            {
                ElementName = builder.ElementName,
                Id = builder.Id,
                Classes = string.Join( ' ', builder.Classes )
            };
            foreach ( var child in builder.Children )
                if ( !string.IsNullOrWhiteSpace( child.Text ) )
                    panel.AddChild( new Label { ElementName = child.ElementName, Text = child.Text } );
                else
                    panel.AddChild( child );

            return panel;
        }

        /// <summary>Initializes a new instance of the <see cref="HtmlElementBuilder"/> class.</summary>
        public HtmlElementBuilder()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="HtmlElementBuilder"/> class.</summary>
        /// <param name="elementName">The HTML element name.</param>
        public HtmlElementBuilder( string elementName )
            => ElementName = elementName;

        /// <summary>Gets or sets the element name.</summary>
        public string ElementName { get; set; }

        /// <summary>Gets or sets the ID of the element.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the inner text of the element.</summary>
        public string Text { get; set; }

        /// <summary>Gets the CSS classes of the element.</summary>
        public IList<string> Classes { get; } = new List<string>();

        /// <summary>Adds the provided <paramref name="classes"/> to the related <see cref="Classes"/> list.</summary>
        /// <param name="classes">The CSS classes to add for the element.</param>
        /// <returns>Returns the same instance for building the HTML element.</returns>
        public HtmlElementBuilder WithClasses( IEnumerable<string> classes )
        {
            if ( classes is not null )
                foreach ( var @class in classes )
                    Classes.Add( @class );

            return this;
        }

        /// <summary>Adds the provided <paramref name="classes"/> to the related <see cref="Classes"/> list.</summary>
        /// <param name="classes">The CSS classes to add for the element.</param>
        /// <returns>Returns the same instance for building the HTML element.</returns>
        public HtmlElementBuilder WithClasses( params string[] classes )
            => WithClasses( classes.AsEnumerable() );

        /// <summary>Gets the children of this element.</summary>
        public IList<HtmlElementBuilder> Children { get; } = new List<HtmlElementBuilder>();

        /// <summary>Adds the provided <paramref name="children"/> to the related <see cref="Children"/> list.</summary>
        /// <param name="children">The <see cref="HtmlElementBuilder"/> elements to add as children.</param>
        /// <returns>Returns the same instance for building the HTML element.</returns>
        public HtmlElementBuilder WithChildren( IEnumerable<HtmlElementBuilder> children )
        {
            if ( children is not null )
                foreach ( var child in children )
                    Children.Add( child );

            return this;
        }

        /// <summary>Adds the provided <paramref name="children"/> to the related <see cref="Children"/> list.</summary>
        /// <param name="children">The <see cref="HtmlElementBuilder"/> elements to add as children.</param>
        /// <returns>Returns the same instance for building the HTML element.</returns>
        public HtmlElementBuilder WithChildren( params HtmlElementBuilder[] children )
            => WithChildren( children.AsEnumerable() );
    }
}
