## Customised Radio Buttons in Blazor WASM .Net 5

I recently worked on a [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) project with an exciting requirement: **Create a custom image for every radio button**.  Finding documentation, help or tips was but of a struggle, so I decided to share my findings with the world!  

## Requirement and result

Lets start at the end.  This gif shows customised animated radio buttons that are bound to enums with display attributes.  Animation is done in CSS (*the whole point was to build the app without any JS!*).  

The end result pure **C#** + pure **Razor**+ pure **CSS** = pure ***awesomeness***.

![demo](https://user-images.githubusercontent.com/20985071/138364179-6cb0511b-e198-49ed-ad1c-37878fbf9ea3.gif)

## Implementation

### Enum Property

The key value of the enums is string type and will be used in the backend.  The front end needs to display a different name for each option. I decided to use enum with `Display` attributes.

```c#
public enum EmojiEnum
{
  [Display(Name="Happy")]
  MM01,
  [Display(Name="Angry")]
  DB01
}
```

To use the enum in a Blazor form, a property that implements the enum needs to be added.  A default value is also assigned at this point.

```C#
public EmojiEnum EmojiOptions = EmojiEnum.DB01;
```

### Enum Extensions

The above code blocks show that the key of the enum is a string.  That poses a problem because using any of the build in C# methods requires the key to be numeric.  Not to worry, adding an extension method, that we can call to get the value in the `Display` attribute, will clear this problem out real fast: 

```c#
public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName;
            displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()!
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
            if (String.IsNullOrEmpty(displayName))
            {
                displayName = enumValue.ToString();
            }
            return displayName;
        }
    }
```

To avoid diverting attention from the exciting Blazor challenge, I will not include more details on the extension method but you can have a read [here](https://www.c-sharpcorner.com/blogs/fetching-values-from-enum-in-c-sharp).

### Blazor component

The `EditForm` component in Blazor caters for radio buttons.  We just need to bound the `EditForm` to the `Model` attribute (which is the class containing our enum)  and setup the radio button group.  

```c#
<EditForm Model="_emoji">

</EditForm> 
@code {
   private Emoji _emoji = new Emoji();
}
```

The `InputRadioGroup` component is required to group the individual `InputRadio` components.   There is no way that I am going to write each and every InputRadio button....  This is where I am super thankful for Razor! Lets wrap the `InputRadio` in a foreach. 

```c#
<EditForm Model="_emoji">
    <InputRadioGroup @bind-Value="_emoji.EmojiOptions" >
        @foreach (Enum value in Enum.GetValues(typeof(Emoji.EmojiEnum)))
        {
          <InputRadio Value=@value />
          <span class="ml-2 mr-2">@value.GetDisplayName()</span>
        }
    </InputRadioGroup>
</EditForm>
```

#### Explanation

`InputRadioGroup` is required when using
 `InputRadio`, it groups the radio buttons together.  Adding the `@bind-Value` attribute (it is case sensitive) to the `_emoji.EmojiOptions` property wires it up to the property in the Model.  Looping through each `Enum.GetValues` in `EmojiEnum` will return all the key values in the enum.  

`InputRadio` has a `Value` attribute which are binding to the enums iterated value.  Using the above extension method it can be called `@value.GetDisplayName()` in the `<span>` element and just like that the display attribute value is shown! So cool right!?  Just a little bit of space to the left is added using the build in `ml-2 mr-2` class.

This is the result when running the code now : 

![image-20211021141448875](https://user-images.githubusercontent.com/20985071/138357574-20799a77-441c-4df5-9281-6ccd16d16583.png)

On the right we can see the elements that Blazor created!

More information about EditForms can be read [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-5.0).

## CSS Styling

A custom stylesheet can be added - just remember to update the `wwwroot\index.html`!  However for this exercise I will be using `wwwroot\css\app.css`.

This [JSFiddle](http://jsfiddle.net/La8wQ/10/) pointed me in the right direction but when implementing it, the model was no longer updated.  

To avoiding  interference with Blazors' binding, the radio button appearance must be hidden.  But doing that, its gone...  unless you set a width & height for the blank space and prioritise it z-index as high.  The emoji images are added to the `label` element, placing it in the same spot as the radio button.   This means when clicking on the images, the blank radio space is hit and Blazor thinks the radio button was clicked and work as normal! 

### Implementation

####  Radio buttons

This class will be customising the `<input type='radio'../>` element.

As explained in the above CSS Styling intro,  the radio button appearance needs to be hidden and new sizes must be set.  The sizes needs to be the same, if not bigger than the images that will be used as buttons.  Position `absolute` and `z-index` properties is set to ensure that its index are high and will be hit when clicking the space.

```css
.selector{
    cursor:pointer;

    -webkit-appearance: none;
    -moz-appearance: none;
    appearance: none;

    border-radius: 50%;
    width: 30px;
    height: 30px;

    position: absolute;
    z-index:1;
}
```

#### Radio Options

Each button has a customised image and animation.  There are some settings that are the same, but the image and animation has their own classes. A `<label for="@value"../>` element will be added, after the `InputRadio` element.  Both classes (global and custom) CSS classes created in this section will be applied to this `<label for="@value"../>` element.

##### Global options styling

Each option will have a background image.  Standard image size and placement is set.  The unselected image style is set in the filter section, this gives the unselected image that faded out, grey look. 

```css
.options{
    background-size:contain;
    background-repeat:no-repeat;

    display:inline-block;
    position:relative;

    -webkit-filter: brightness(1.8) grayscale(1) opacity(.7);
    -moz-filter: brightness(1.8) grayscale(1) opacity(.7);
    filter: brightness(1.8) grayscale(1) opacity(.7);

    margin-bottom:0; /*override label margin bottom*/
    height:30px;
    width:30px;
    top:0.5em; /*top position depends on size of fonts*/

    border: dotted 1px var(--main-color);

    border-radius:50%
}
```

##### Custom Image per option

Background images will be added to a class that will have the same name as the value of the enum key property.  i.e. adding `@value` as a class.  Different styling per custom image are used for checked vs unchecked :

```css
.DB01{
    background-image: url(../images/angry-solid.svg);
}
.MM01{
    background-image: url(../images/laugh-wink-solid.svg);
}
.selector:checked +.MM01{
    background-image:url(../images/laugh-wink-selected.svg);
}
.selector:checked +.DB01{
    background-image:url(../images/angry-selected.svg);
}
.selector:checked + .MM01{
    border:solid 1px green;
    
    -webkit-filter: none;
    -moz-filter: none;
    filter: none;
    
    -webkit-animation:selected 1s ease-out;
    -moz-animation: selected 1s ease-out;
    animation:  selected 1s ease-out;
}
.selector:checked + .DB01{
    border:solid 1px red;

    -webkit-filter: none;
    -moz-filter: none;
    filter: none;
    
    -webkit-animation:selected 1s ease-in;
    -moz-animation: selected 1s ease-in;
    animation:  selected 1s ease-in;
}
```

The last little bit missing is the animation of the image:

```css
@-moz-keyframes selected {100% { -moz-transform: rotate(360deg); } }
@-webkit-keyframes selected  { 100% { -webkit-transform: rotate(360deg); } }
@keyframes selected { 100% { -webkit-transform: rotate(360deg); transform:rotate(360deg); } }
```

## Update Form with classes

The classes created above will be added to the elements inside `InputRadioGroup`.

The interesting part is the `<label>` element with the `@value` class.  `@value` will be replaced with the enum key value (DB01\MM01).  Above relevant class will then be applied and whala - we have custom radio buttons!

```html
<InputRadioGroup @bind-Value="_emoji.EmojiOptions" >
    @foreach (Enum value in Enum.GetValues(typeof(Emoji.EmojiEnum)))
    {
        <label>
            <InputRadio class="selector" Value=@value />
            <label for="@value" class="@value options" />
            <span class="ml-2 mr-2 option-name">@value.GetDisplayName()</span>
        </label>
    }
</InputRadioGroup>
```

#### Result

Notice the class names for the `<label for=".../>` element, on the inspection page on the right: 
![image-20211022090505806](https://user-images.githubusercontent.com/20985071/138364758-12830d72-adad-4d6a-bc12-37c4ef431c34.png)

Clicking on the **Submit** button will display the Model values.  Have a look at the console window and view the values in the Model, when the changes are submitted: 

![detailDemo](https://user-images.githubusercontent.com/20985071/138364693-bc380b08-5518-4c45-b0aa-b79cee860eb2.gif)


I really enjoyed this challenge and would love to hear possible improvements or feedback from you!

