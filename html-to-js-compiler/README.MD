# HTML To JS Compiler

This is a small project made to teach myself how to program in c#. The end product of this will be a small compiler
that converts HTML files into Javascript files containing functions that replicate the structures represented
by the files.

To generate a component a root tag must have a `data-component` attribute with the name inside. With this it is possible to generate multiple components.

To generate a getter for a part of a certain component, put a `data-getter` attribute in the tag you wish to get with a name for the getter.

Two kinds of html files can be provided to the program. The first is an html file where the components are directly defined in the document. Like this:

```html
<!-- Commands Components -->
<div class="commands" data-component="Commands">
    <div class="commands__mode mode" data-side="top">
        <img class="mode__icon">
        <ul class="mode__content" data-getter="ModeElement">
            <li class="mode__option" data-value="add-mode"><img src="./icons/plus.svg" alt="" class="mode__icon"></li>
            <li class="mode__option" data-value="edit-mode"><img src="./icons/pencil.svg" alt="" class="mode__icon"></li>
        </ul>
    </div>
    <div class="commands__input" contenteditable="true"></div>
    <div class="commands__send" data-getter="SendElement">
        <img src="icons/arrow.svg">
    </div>
</div>

<!-- Button Component -->
<button data-component="Button"></button>
```

It is possible to define more than one component per file making it possible to put a main and child components in the same file.

The second kind of html file is a properly structured one with a `<!DOCTYPE html>` declaration at the top and the structure declared inside the `<html>` tag. Here is an example:

```html
<!DOCTYPE html>
<html>

<head>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="">
    <script type="module" src=""></script>
</head>

<body>
    <!--Hello-->
    <div class="commands" data-component="Commands">
        <div class="commands__mode mode" data-side="top">
            <img class="mode__icon">
            <ul class="mode__content" data-getter="ModeElement">
                <li class="mode__option" data-value="add-mode"><img src="./icons/plus.svg" alt="" class="mode__icon">
                </li>
                <li class="mode__option" data-value="edit-mode"><img src="./icons/pencil.svg" alt="" class="mode__icon">
                </li>
            </ul>
        </div>
        <div class="commands__input" contenteditable="true"></div>
        <div class="commands__send" data-getter="SendElement">
            <img src="icons/arrow.svg">
        </div>
    </div>

    <button data-component="Button"></button>
</body>

</html>
```