Alright, so in the previous post we've added [basic networking](https://wiki.facepunch.com/sbox/Network_Basics)
for the player stats, now it's time to add a some UI for this.

[Sbox](https://sbox.facepunch.com) uses [Razor](https://learn.microsoft.com/aspnet/core/mvc/views/razor)
to render the UI, it's basically an engine that allows you to mix HTML and C# together.
It's quite powerful, it even has intellisense support.

Alright, we'll be using a table to display the stats. The bad news is that there is no
support for them in [sbox](https://sbox.facepunch.com) so we need to improvise.

The good news is that [flex-box](https://developer.mozilla.org/docs/Learn/CSS/CSS_layout/Flexbox)
works and we can style our table related elements to simulate one.

If we expand the generated `Hud.razor` file, we have a `Hud.razor.scss` file, this is
the place where we will define all of our styling and apply it in the view. Generally,
this means styling either elements or defining (CSS) classes and applying them on the
elements. This is standard web development stuff.

Our `.scss` file contains [SASS](https://sass-lang.com/documentation/syntax/) which is
a superset of [CSS](https://developer.mozilla.org/docs/Web/CSS). If you are familiar with
the later, you can do everything exactly the same in the former. With [SASS](https://sass-lang.com/documentation/syntax/)
we get more features, most notable the nesting that we're about to use.

Alright, in our `Hud.razor` let's define a table showing the stats that we are getting
from the server.

```razor
<root>
    <table class="users-list">
        <tr>
            <th>Player</th>
            <th>Oranges</th>
        </tr>
        @foreach (var userAndOranges in TheOrangeRunGameManager.Current.PawnsStats)
        {
            <tr>
                <td>@userAndOranges.Name</td>
                <td>@userAndOranges.TotalOranges</td>
            </tr>
        }
    </table>
</root>
```

Isn't that just nice how we can just add C# code in the midle of it, do a [`foreach`](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/iteration-statements#the-foreach-statement)
and then for each element render some more HTML and then add the information for the
item itself?

We can reference anything that would normally be accessible in a C# file, in this
case, we need to go through the `PawnsStats` and add a row for each.

Now, as mentioned before, tables are not supported and the result on the screen is
bad, really bad, but not to worry, we're going to apply some styling and make our
table look like one.

```scss
table {
    display: flex;
    flex-direction: column;

    tr {
        display: flex;
        flex-direction: row;

        th, td {
            label {
                width: 100%;
            }
        }
    }
}
```

This will make rows stack on top of each other and cells be next to each other.
Whenever we write text it will actually be wrapped in a `label` tag, so we set
the width to take all the available space.


The remaining issue here is the sizing of the columns, they do not have consistent
sizing. For this we will use [CSS pseudo classes](https://developer.mozilla.org/docs/Web/CSS/Pseudo-classes)
to select the 1st and 2nd column, but this can be expanded as required.

```scss
table.users-list {
    tr {
        // Player name
        :nth-child(1) {
            width: 500px;
        }
        // Oranges
        :nth-child(2) {
            width: 200px;

            label {
                text-align: right;
            }
        }
    }
}
```

With this, we set the size of all 1st children of a `tr` element inside a `table`
element that has the CSS class `users-list` to `500px`, and the 2nd child, or column
cell, to `200px`. This can be expanded to any number of columns.

In addition to setting the width, the 2nd column, the one with the orange stats, is
aligned to the right.

Finally we have something we can call a table. Now, to position it, add some color,
and make it look generally better.

```scss
table.users-list {
    position: absolute;
    top: 50px;
    left: 50px;
    padding: 8px;
    background-color: orange;
    color: white;
    font-weight: 600;

    tr {
        margin-top: 6px;
        border-top: 1px solid white;
        padding-top: 6px;

        &:first-child {
            margin-top: 0;
            border-top: 0;
        }

        th {
            font-size: 54px;
            margin-bottom: 10px;
        }

        td {
            font-size: 30px;
        }
    }
}
```

A remaining issue, if we end up having longer player names, it can mess with the table,
but in this case we can put ellipsis.


```scss
table.users-list {
    tr {
        td, th {
            white-space: nowrap;
            text-overflow: ellipsis;
            overflow: hidden;
        }
    }
}
```

And that is it for the table. Only that.. it kind of takes up space on the screen doesn't
it? We should only show it if the user presses the tab key.

Let's define an input for the `scoreScreen` and bind it to the `tab` key.

We'll define a property on our Hud that controls when the score screen is visible.

```razor
<root>
@if (IsScoreScreenVisible || true)
{
    <table class="users-list">
        <!-- ... -->
    </table>
}
</root>


@code
{
    public bool IsScoreScreenVisible { get; set; }

    // Let's not forget about this
    protected override int BuildHash()
        => HashCode.Combine(IsScoreScreenVisible, TheOrangeRunGameManager.Current.PawnsStats.Aggregate(0, (previous, pawnStats) => HashCode.Combine(previous, pawnStats.Name, pawnStats.TotalOranges)));
}
```

And now to bind the input.

```cshap
public override void BuildInput()
{
    Camera?.BuildInput();

    if ( Input.Pressed( "scoreScreen" ) )
        TheOrangeRunGameManager.RootPanel.IsScoreScreenVisible = true;
    if ( Input.Released( "scoreScreen" ) )
        TheOrangeRunGameManager.RootPanel.IsScoreScreenVisible = false;
}
```

And that's it! Hitting the `attack1` button (left click) will update the orange count as well.
Besides to previous logs, which now can be removed, we see this on the screen as well.
