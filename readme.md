Movement Composition Library

This tool allows you to "compose" complex 2d motion by chaining simple
2D movements together in configurable ways!

All your interaction will be with the Movement class.
There are 5 types of simple movements, I will call these "primitives" :

{Line, Curve, Circle, Wait, Sin}

Complex movements are formed by chaining primitives.
For all the methods listed below, there are more detailed usage notes near the function itself in the code.

There are also a bunch of examples under the Assets/Examples folder.

NOTE: "Teleportation" can be achieved by setting a waitPoint in the WAIT primitive.