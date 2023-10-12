# HTML To JS Compiler

This is a small project made to teach myself how to program in c#. The end product of this will be a small compiler
that converts HTML files into Javascript files containing functions that replicate the structures represented
by the files.

To generate a component a root tag must have a `data-component-name` attribute with the name inside. With this it is possible to generate multiple components.

To generate a getter for a part of a certain component, put a `data-getter` attribute in the tag you wish to get with a name for the getter.